/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Security;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping authentication helper features of Phosphorus Five
    /// </summary>
    static class AuthenticationHelper
    {
        // Name of credential cookie, used to store username and hashsalted password
        const string _credentialCookieName = "_p5_user";

        /*
         * Returns user Context Ticket (Context "user")
         */
        public static ContextTicket GetTicket (ApplicationContext context)
        {
            if (HttpContext.Current.Session[".p5.auth.context-ticket"] == null) {

                // No user is logged in, using default impersonated user
                HttpContext.Current.Session [".p5.auth.context-ticket"] = CreateDefaultTicket (context);
            }
            return HttpContext.Current.Session[".p5.auth.context-ticket"] as ContextTicket;
        }

        /*
         * Returns true if Context Ticket is already set
         */
        public static bool ContextTicketIsSet
        {
            get {
                return HttpContext.Current.Session != null && HttpContext.Current.Session [".p5.auth.context-ticket"] != null;
            }
        }

        /*
         * Tries to login user according to given user credentials
         */
        public static void Login (ApplicationContext context, Node args)
        {
            // Defaulting result of Active Event to unsuccessful
            args.Value = false;

            // Retrieving supplied credentials
            string username = args.GetExChildValue<string> ("username", context);
            string password = args.GetExChildValue<string> ("password", context);
            bool persist = args.GetExChildValue ("persist", context, false);

            // Getting password file in Node format, but locking file access as we retrieve it
            Node pwdFile = AuthFile.GetAuthFile(context);

            // Checking for match on specified username
            Node userNode = pwdFile["users"][username];
            if (userNode == null)
                throw new LambdaSecurityException ("Credentials not accepted", args, context);

            // Getting system salt
            var serverSalt = context.RaiseEvent (".p5.auth.get-server-salt").Get<string> (context);

            // Then creating system fingerprint from given password
            var hashedPassword = context.RaiseEvent ("p5.crypto.hash.create-sha256", new Node ("", serverSalt + password)).Get<string> (context);

            // Checking for match on password
            if (userNode["password"].Get<string> (context) != hashedPassword)
                throw new LambdaSecurityException ("Credentials not accepted", args, context); // Exact same wording as above! IMPORTANT!!

            // Success, creating our ticket
            string role = userNode["role"].Get<string>(context);
            SetTicket (new ContextTicket(username, role, false));
            args.Value = true;

            // Removing last login attempt, to reset brute force login cool off seconds for user's IP address
            LastLoginAttemptForIP = DateTime.MinValue;

            // Associating newly created Ticket with Application Context, since user now possibly have extended rights
            context.UpdateTicket (GetTicket (context));

            // Checking if we should create persistent cookie on disc to remember username for given client
            if (persist) {

                // Caller wants to create persistent cookie to remember username/password
                var cookie = new HttpCookie (_credentialCookieName);
                cookie.Expires = DateTime.Now.AddDays (context.RaiseEvent (
                    ".p5.config.get", 
                    new Node (".p5.config.get", "p5.auth.credential-cookie-valid")) [0].Get<int> (context));
                cookie.HttpOnly = true; // To avoid JavaScript access to credential cookie

                // Notice, we use another fingerprint as password for cookie than what we use for storing cookie in auth file
                // The "system salted fingerprint" hence never leaves the server
                // If this was not the case, then the system fingerprint would effectively BE the password, allowing anyone
                // who somehow gets access to "auth" file also to log in by creating false cookies
                cookie.Value = username + " " + hashedPassword;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            // Making sure we invoke an [.onlogin] lambda callbacks for user.
            var onLogin = new Node ();
            GetSettings (context, onLogin);
            if (onLogin [".onlogin"] != null) {
                var lambda = onLogin[".onlogin"].Clone ();
                context.RaiseEvent ("eval", lambda);
            }
        }

        /*
         * Logs out user
         */
        public static void Logout (ApplicationContext context)
        {
            // Making sure we invoke an [.onlogin] lambda callbacks for user.
            var onLogout = new Node ();
            GetSettings (context, onLogout);
            if (onLogout[".onlogout"] != null) {
                var lambda = onLogout[".onlogout"].Clone ();
                context.RaiseEvent ("eval", lambda);
            }

            // By destroying Ticket, default user will be used for current session, until user logs in again
            SetTicket (null);

            // Destroying persistent credentials cookie, if there is one
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(_credentialCookieName);
            if (cookie != null) {

                // Making sure cookie is destroyed on the client side by setting its expiration date to "today - 1 day"
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /*
         * Lists all users in system
         */
        public static void ListUsers (ApplicationContext context, Node args)
        {
            // Retrieving "auth" file in node format
            var authFile = AuthFile.GetAuthFile (context);

            // Looping through each user in [users] node of "auth" file
            foreach (var idxUserNode in authFile["users"].Children) {

                // Returning user's name, and role he belongs to
                args.Add (idxUserNode.Name, idxUserNode["role"].Value);
            }
        }

        /*
         * Returns server-salt for application
         */
        public static string ServerSalt (ApplicationContext context)
        {
            // Retrieving "auth" file in node format
            var authFile = AuthFile.GetAuthFile(context);
            return authFile.GetChildValue<string> ("server-salt", context);
        }

        /*
         * Sets the server salt for application
         */
        public static void SetServerSalt (ApplicationContext context, Node args, string salt)
        {
            AuthFile.ModifyAuthFile (context, delegate (Node node) {
                if (node.Children.Any (ix => ix.Name == "server-salt"))
                    throw new LambdaSecurityException ("Tried to change server salt after initial creation", args, context);
                node.FindOrInsert ("server-salt").Value = salt;
            });
        }

        /*
         * Creates a new user
         */
        public static void CreateUser (ApplicationContext context, Node args)
        {
            string username = args.GetExValue<string>(context);
            string password = args.GetExChildValue<string>("password", context);
            string role = args.GetExChildValue<string>("role", context);

            // Making sure [password] never leaves method, in case of exceptions
            args.FindOrInsert ("password").Value = "xxx";

            // Basic syntax checking
            if (string.IsNullOrEmpty (username) || string.IsNullOrEmpty (password) || string.IsNullOrEmpty (role))
                throw new LambdaException (
                    "User must have username as value, [password] and [role] at the very least",
                    args,
                    context);

            // Verifying username is valid, since we'll need to create a folder for user
            VerifyUsernameValid (username);

            // Retrieving system salt before we enter write lock.
            var serverSalt = context.RaiseEvent (".p5.auth.get-server-salt").Get<string> (context);

            // Then salting password with user salt, before salting it with system salt
            var userPasswordFingerprint = context.RaiseEvent ("p5.crypto.hash.create-sha256", new Node ("", serverSalt + password)).Get<string> (context);

            // Locking access to password file as we create new user object
            AuthFile.ModifyAuthFile (
                context, 
                delegate (Node authFile) {

                    // Checking if user exist from before
                    if (authFile["users"][username] != null)
                        throw new LambdaException(
                            "Sorry, that [username] is already taken by another user in the system", 
                            args, 
                            context);

                    // Adding user
                    authFile["users"].Add(username);

                    // Creates a salt and password for user
                    authFile ["users"].LastChild.Add("password", userPasswordFingerprint);

                    // Adding user to specified role
                    authFile ["users"].LastChild.Add("role", role);

                    // Adding all other specified objects to user
                    foreach (var idxNode in args.Children.Where (ix => ix.Name != "username" && ix.Name != "password" && ix.Name != "role")) {

                        // Only adding nodes with some sort of actual value
                        if (idxNode.Value != null || idxNode.Count > 0)
                            authFile["users"].LastChild.Add (idxNode.Clone ());
                    }
                });

            // Creating newly created user's directory structure
            CreateUserDirectory (context.RaiseEvent(".p5.core.application-folder").Get<string>(context), username);
        }

        /*
         * Retrieves a specific user from system
         */
        public static void GetUser (ApplicationContext context, Node args)
        {
            // Retrieving "auth" file in node format
            var authFile = AuthFile.GetAuthFile (context);

            // Iterating all users requested by caller
            foreach (var idxUsername in XUtil.Iterate<string> (context, args)) {

                // Checking if user exist
                if (authFile ["users"] [idxUsername] == null)
                    throw new LambdaException (
                        string.Format ("User '{0}' does not exist", idxUsername),
                        args,
                        context);

                // Adding user's node as return value, and each property of user, except [password]
                args.Add (idxUsername);
                args [idxUsername].AddRange (authFile["users"][idxUsername].Clone ().Children.Where (ix => ix.Name != "password"));
            }
        }

        /*
         * Retrieves a specific user from system
         */
        public static void DeleteUser (ApplicationContext context, Node args)
        {

            // Locking access to password file as we create new user object
            AuthFile.ModifyAuthFile (
                context, 
                delegate (Node authFile) {

                    // Iterating all users requested deleted by caller
                    foreach (var idxUsername in XUtil.Iterate<string> (context, args)) {

                        // Checking if user exist
                        if (authFile ["users"] [idxUsername] == null)
                            throw new LambdaException (
                                string.Format ("User '{0}' does not exist", idxUsername),
                                args,
                                context);

                        // Deleting currently iterated user
                        authFile["users"][idxUsername].UnTie();

                        // Deleting user's home directory
                        context.RaiseEvent ("p5.io.folder.delete", new Node ("", "/users/" + idxUsername + "/"));
                    }
                });
        }

        /*
         * Edits an existing user
         */
        public static void EditUser (ApplicationContext context, Node args)
        {
            string username = args.GetExValue<string>(context);
            string password = args.GetExChildValue<string>("password", context);
            string userRole = args.GetExChildValue<string>("role", context);
            if (args["username"] != null)
                throw new LambdaSecurityException ("Cannot change username for user", args, context);

            // Retrieving system salt before we enter write lock.
            var serverSalt = context.RaiseEvent (".p5.auth.get-server-salt").Get<string> (context);

            // Locking access to password file as we edit user object
            AuthFile.ModifyAuthFile (
                context, 
                delegate (Node authFile) {

                    // Checking to see if user exist
                    if (authFile["users"][username] == null)
                        throw new LambdaException(
                            "Sorry, that user does not exist", 
                            args, 
                            context);

                    // Updating user's password, if a new one was given
                    if (!string.IsNullOrEmpty (password)) {

                        // Changing user's password
                        // Then salting password with user salt and system, before salting it with system salt
                        var userPasswordFingerprint = context.RaiseEvent ("p5.crypto.hash.create-sha256", new Node ("", serverSalt + password)).Get<string> (context);
                        authFile ["users"][username]["password"].Value = userPasswordFingerprint;
                    }

                    // Updating user's role
                    if (userRole != null) {
                        authFile["users"][username]["role"].Value = userRole;
                    }

                    // Removing old settings
                    authFile["users"][username].RemoveAll (ix => ix.Name != "password" && ix.Name != "role");

                    // Adding all other specified objects to user
                    foreach (var idxNode in args.Children.Where (ix => ix.Name != "password" && ix.Name != "role")) {

                        authFile["users"][username].Add (idxNode.Clone ());
                    }
                });
        }
            
        /*
         * Retrieves settings for currently logged in user
         */
        public static void GetSettings (ApplicationContext context, Node args)
        {
            // Retrieving "auth" file in node format
            var authFile = AuthFile.GetAuthFile (context);

            // Checking if user exist
            if (authFile ["users"] [context.Ticket.Username] == null)
                throw new LambdaException (
                    "You do not exist",
                    args,
                    context);

            args.AddRange (authFile["users"][context.Ticket.Username].Clone ().Children.Where (ix => ix.Name != "password" && ix.Name != "role"));
        }

        /*
         * Changes the settings for currently logged in user
         */
        public static void ChangeSettings (ApplicationContext context, Node args)
        {
            string username = context.Ticket.Username;

            // Locking access to password file as we edit user object
            AuthFile.ModifyAuthFile (
                context, 
                delegate (Node authFile) {

                    // Removing old settings
                    authFile["users"][username].RemoveAll (ix => ix.Name != "password" && ix.Name != "role");

                    // Changing all settings for user
                    foreach (var idxNode in args.Children) {
                        authFile["users"][username].Add (idxNode.Clone ());
                    }
                });
        }

        /*
         * Changes the password for currently logged in user
         */
        public static void ChangePassword (ApplicationContext context, Node args)
        {
            string password = args.GetExValue(context, "");
            if (string.IsNullOrEmpty (password))
                throw new LambdaException ("No password supplied", args, context);
            
            string username = context.Ticket.Username;

            // Retrieving system salt before we enter write lock.
            var serverSalt = context.RaiseEvent (".p5.auth.get-server-salt").Get<string> (context);

            // Locking access to password file as we edit user object
            AuthFile.ModifyAuthFile (
                context, 
                delegate (Node authFile) {

                    // Changing user's password
                    // Then salting password with user salt and system, before salting it with system salt
                    var userPasswordFingerprint = context.RaiseEvent ("p5.crypto.hash.create-sha256", new Node ("", serverSalt + password)).Get<string> (context);
                    authFile ["users"][username]["password"].Value = userPasswordFingerprint;
                });
        }

        /*
         * Deletes the currently logged in user
         */
        public static void DeleteMyUser (ApplicationContext context, Node args)
        {
            // Retrieving username to delete.
            string username = context.Ticket.Username;

            // Deleting user's home directory
            context.RaiseEvent ("p5.io.folder.delete", new Node ("", "/users/" + username + "/"));

            // Locking access to password file as we delete user object
            AuthFile.ModifyAuthFile (
                context, 
                delegate (Node authFile) {

                    // Removing user
                    authFile["users"][username].UnTie ();
                });

            var def = CreateDefaultTicket (context);
            SetTicket (def);
            context.UpdateTicket (def);
        }

        /*
         * Returns all existing roles in system
         */
        public static void GetRoles (ApplicationContext context, Node args)
        {
            // Making sure default role is added first.
            string defaultRole = context.RaiseEvent (".p5.auth.get-default-context-role").Get<string> (context);
            if (!string.IsNullOrEmpty (defaultRole)) {

                // There exist a default role, checking if it's already added
                if (args.Children.FirstOrDefault (ix => ix.Name == defaultRole) == null) {

                    // Default Role was not already added, therefor we add it to return lambda node
                    args.Add (defaultRole);
                }
            }

            // Getting password file in Node format, such that we can traverse file for all roles
            Node pwdFile = AuthFile.GetAuthFile(context);

            // Looping through each user object in password file, retrieving all roles
            foreach (var idxUserNode in pwdFile["users"].Children) {

                // Retrieving role name of currently iterated user
                var role = idxUserNode["role"].Get<string>(context);

                // Adding currently iterated role, unless already added, and incrementing user count for it
                args.FindOrInsert (role).Value = args[role].Get (context, 0) + 1;
            }
        }

        /*
         * Changes password of "root" account, but only if existing root account's password 
         * is null. Used during setup of system
         */
        public static void SetRootPassword (ApplicationContext context, Node args)
        {
            // Retrieving password given
            string password = args.GetExChildValue<string>("password", context);
            if (string.IsNullOrEmpty(password))
                throw new LambdaSecurityException("You cannot set the root password to empty", args, context);

            // Creating root account
            var rootAccountNode = new Node ("", "root");
            rootAccountNode.Add ("password", password);
            rootAccountNode.Add ("role", "root");
            CreateUser (context, rootAccountNode);
        }

        /*
         * Returns true if root account's password is null, which means that server is not setup yet
         */
        public static bool NoExistingRootAccount (ApplicationContext context)
        {
            // Retrieving password file, and making sure we lock access to file as we do
            Node rootPwdNode = AuthFile.GetAuthFile (context)["users"]["root"];

            // Returning true if root account does not exist
            return rootPwdNode == null;
        }

        /*
         * Will try to login from persistent cookie
         */
        public static void TryLoginFromPersistentCookie(ApplicationContext context)
        {
            try {
                // Making sure we do NOT try to login from persistent cookie if root password is null, at which
                // case the system has been reset, and cookie (obviously) is not valid!
                if (NoExistingRootAccount (context)) {

                    // Making sure we delete cookie, since (obviously) it is no longer valid!
                    // The simplest way to do this, is simply to throw exception, which will be handled 
                    // further down, and deletes current cookie!
                    throw null;
                }

                // Checking if client has persistent cookie
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(_credentialCookieName);
                if (cookie != null) {

                    // We have a cookie, try to use it as credentials
                    LoginFromCookie (cookie, context);
                }
            } catch {

                // Making sure we delete cookie
                // We do not rethrow this, since reason might be because "salt" has changed, to explicitly log user
                // out, and that is actually not a "security issue", but a "feature". Besides, login-cooloff-seconds
                // will make sure "brute force" login through cookies are virtually impossible
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(_credentialCookieName);
                if (cookie != null) {

                    // Deleting cookie!
                    cookie.Expires = DateTime.Now.AddDays (-1);
                    HttpContext.Current.Response.Cookies.Add (cookie);
                }
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Sets user Context Ticket (context "user")
         */
        static void SetTicket (ContextTicket ticket)
        { 
            HttpContext.Current.Session[".p5.auth.context-ticket"] = ticket;
        }

        /*
         * Verifies that given username is valid
         */
        static void VerifyUsernameValid (string username)
        {
            foreach (var charIdx in username) {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_-".IndexOf(charIdx) == -1)
                    throw new SecurityException("Sorry, you cannot use character '" + charIdx + "' in username");
            }
        }

        /*
         * Creates folder structure for user
         */
        static void CreateUserDirectory (string rootFolder, string username)
        {
            // Creating folders for user, and making sure private directory stays private ...
            if (!Directory.Exists (rootFolder + "/users/" + username))
                Directory.CreateDirectory (rootFolder + "/users/" + username);

            if (!Directory.Exists (rootFolder + "/users/" + username + "/documents"))
                Directory.CreateDirectory(rootFolder + "/users/" + username + "/documents");

            if (!Directory.Exists (rootFolder + "/users/" + username + "/documents/private"))
                Directory.CreateDirectory(rootFolder + "/users/" + username + "/documents/private");

            if (!Directory.Exists (rootFolder + "/users/" + username + "/documents/public"))
                Directory.CreateDirectory(rootFolder + "/users/" + username + "/documents/public");

            if (!Directory.Exists (rootFolder + "/users/" + username + "/temp"))
                Directory.CreateDirectory(rootFolder + "/users/" + username + "/temp");

            if (!File.Exists (rootFolder + "/users/" + username + "/documents/private/web.config"))
                File.Copy (
                    rootFolder + "/users/root/documents/private/web.config", 
                    rootFolder + "/users/" + username + "/documents/private/web.config");
            if (!File.Exists (rootFolder + "/users/" + username + "/documents/private/README.md"))
                File.Copy (
                    rootFolder + "/users/root/documents/private/README.md", 
                    rootFolder + "/users/" + username + "/documents/private/README.md");
            if (!File.Exists (rootFolder + "/users/" + username + "/documents/public/README.md"))
                File.Copy (
                    rootFolder + "/users/root/documents/public/README.md", 
                    rootFolder + "/users/" + username + "/documents/public/README.md");
            if (!File.Exists (rootFolder + "/users/" + username + "/temp/README.md"))
                File.Copy (
                    rootFolder + "/users/root/temp/README.md", 
                    rootFolder + "/users/" + username + "/temp/README.md");
        }

        /*
         * Tries to login with the given cookie as credentials
         */
        static void LoginFromCookie (HttpCookie cookie, ApplicationContext context)
        {
            // User has persistent cookie associated with client
            var cookieSplits = cookie.Value.Split (' ');
            if (cookieSplits.Length != 2)
                throw new SecurityException ("Cookie not accepted");

            string cookieUsername = cookieSplits[0];
            string hashedPassword = cookieSplits[1];
            Node pwdFile = AuthFile.GetAuthFile(context);

            // Checking if user exist
            Node userNode = pwdFile["users"][cookieUsername];
            if (userNode == null)
                throw new SecurityException ("Cookie not accepted");

            // Notice, we do NOT THROW if passwords do not match, since it might simply mean that user has explicitly created a new "salt"
            // to throw out other clients that are currently persistently logged into system under his account
            if (hashedPassword   == userNode["password"].Get<string> (context)) {

                // MATCH, discarding previous Context Ticket and creating a new Ticket
                SetTicket (new ContextTicket(
                    userNode.Name, 
                    userNode ["role"].Get<string>(context), 
                    false));
                LastLoginAttemptForIP = DateTime.MinValue;
            }
        }

        /*
         * Creates default Context Ticket according to settings from config file
         */
        static ContextTicket CreateDefaultTicket (ApplicationContext context)
        {
            return new ContextTicket (
                context.RaiseEvent (".p5.auth.get-default-context-username").Get<string> (context), 
                context.RaiseEvent (".p5.auth.get-default-context-role").Get<string> (context), 
                true);
        }

        /*
         * Helper to store "last login attempt" for a specific IP address
         */
        static DateTime LastLoginAttemptForIP
        {
            get {

                // Retrieving Client's IP address, to use as lookup for last login attempt
                string clientIP = HttpContext.Current.Request.UserHostAddress;

                // Checking application object if we have a previous login attempt for given IP
                if (HttpContext.Current.Application ["_last-login-attempt-" + clientIP] != null)
                    return (DateTime)HttpContext.Current.Application ["_last-login-attempt-" + clientIP];

                // No previous login attempt on record, returning DateTime.MinValue
                return DateTime.MinValue;
            }
            set {

                // Retrieving Client's IP address, to use as lookup for last login attempt
                string clientIP = HttpContext.Current.Request.UserHostAddress;

                // Checking if this is a "reset login attempts"
                if (value == DateTime.MinValue)
                    HttpContext.Current.Application.Remove ("_last-login-attempt-" + clientIP);
                else
                    HttpContext.Current.Application ["_last-login-attempt-" + clientIP] = value;
            }
        }

        #endregion
    }
}