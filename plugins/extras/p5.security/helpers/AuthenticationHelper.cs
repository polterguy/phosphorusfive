/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Security;
using System.Configuration;
using System.Collections.Generic;
using System.Security.Cryptography;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.security
{
    /// <summary>
    ///     Class wrapping authentication helper features of Phosphorus Five
    /// </summary>
    internal static class AuthenticationHelper
    {
        // Name of credential cookie, used to store username and hashsalted password
        private const string _credentialCookieName = "_p5_user";

        /*
         * Returns user Context Ticket (Context "user")
         */
        internal static ApplicationContext.ContextTicket GetTicket (ApplicationContext context)
        {
            if (HttpContext.Current.Session["_ContextTicket"] == null) {

                // No user is logged in, using default impersonated user
                HttpContext.Current.Session ["_ContextTicket"] = CreateDefaultTicket (context);
            }
            return HttpContext.Current.Session["_ContextTicket"] as ApplicationContext.ContextTicket;
        }

        /*
         * Returns true if Context Ticket is already set
         */
        internal static bool ContextTicketIsSet
        {
            get {
                return HttpContext.Current.Session != null && HttpContext.Current.Session ["_ContextTicket"] != null;
            }
        }

        /*
         * Sets user Context Ticket (context "user")
         */
        internal static void SetTicket (ApplicationContext.ContextTicket ticket)
        { 
            HttpContext.Current.Session["_ContextTicket"] = ticket;
        }

        /*
         * Tries to login user according to given user credentials
         */
        internal static void Login (ApplicationContext context, Node args)
        {
            // Checking for a brute force login attack
            GuardAgainstBruteForce(context);

            // Defaulting result of Active Event to unsuccessful
            args.Value = false;

            // Retrieving supplied credentials
            string username = args.GetExChildValue<string> ("username", context);
            string password = args.GetExChildValue<string> ("password", context);
            bool persist = args.GetExChildValue ("persist", context, false);

            // Creating Hash of password, with salt from web.config
            using (var sha256 = SHA256.Create ()) {

                // Returning Sha256 hash as base64 encoded string
                var saltAndPassword = context.RaiseNative ("p5.security.get-password-salt").Get<string> (context) + password;
                password = Convert.ToBase64String (sha256.ComputeHash (Encoding.UTF8.GetBytes (saltAndPassword)));
            }

            // Getting password file in Node format, but locking file access as we retrieve it
            Node pwdFile = AuthFile.GetAuthFile(context);

            // Checking for match on specified username
            Node userNode = pwdFile["users"][username];
            if (userNode == null)
                throw new SecurityException("Credentials not accepted");

            // Checking for match on password
            if (userNode["password"].Get<string> (context) != password)
                throw new SecurityException("Credentials not accepted");

            // Success, creating our ticket
            string role = userNode["role"].Get<string>(context);
            SetTicket (new ApplicationContext.ContextTicket(username, role, false));
            args.Value = true;

            // Removing last login attempt, to reset brute force login cool off seconds for user's IP address
            LastLoginAttemptForIP = DateTime.MinValue;

            // Associating newly created Ticket with Application Context, since user now possibly have extended rights
            context.UpdateTicket (GetTicket (context));

            // Checking if we should create persistent cookie on disc to remember username for given client
            if (persist) {

                // Caller wants to create persistent cookie to remember username/password
                HttpCookie cookie = new HttpCookie(_credentialCookieName);
                cookie.Expires = DateTime.Now.AddDays(context.RaiseNative ("p5.security.get-credential-cookie-days").Get<int> (context));
                cookie.HttpOnly = true;
                string salt = userNode["cookie-salt"].Get<string>(context);
                cookie.Value = username + " " + context.RaiseNative ("sha256-hash", new Node("", salt + password)).Value;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /*
         * Logs out user
         */
        internal static void Logout (ApplicationContext context)
        {
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
        internal static void ListUsers (ApplicationContext context, Node args)
        {
            var authFile = AuthFile.GetAuthFile (context);
            foreach (var idxUserNode in authFile["users"].Children) {
                args.Add (idxUserNode.Name);
            }
        }

        /*
         * Creates a new user
         */
        internal static void CreateUser (ApplicationContext context, Node args)
        {
            string username = args.GetExChildValue<string>("username", context);
            string password = args.GetExChildValue<string>("password", context);
            string role = args.GetExChildValue<string>("role", context);
            if (role == "root")
                throw new LambdaSecurityException("[create-user] Active Event tried to create 'root' user", args, context);

            // We need this guy to save passwords file, and create user folder structure
            string rootFolder = context.RaiseNative("p5.core.application-folder").Get<string>(context);

            // Verifying username is valid, since we'll need to create a folder for user
            VerifyUsernameValid (username);

            // Creating Hash of password, with salt from web.config
            using (var sha256 = SHA256.Create ()) {

                // Returning Sha256 hash as base64 encoded string
                var saltAndPassword = context.RaiseNative ("p5.security.get-password-salt").Get<string> (context) + password;
                password = Convert.ToBase64String (sha256.ComputeHash (Encoding.UTF8.GetBytes (saltAndPassword)));
            }

            // Locking access to password file as we create new user object
            AuthFile.ModifyAuthFile (
                context, 
                delegate (Node authFile) {
                    if (authFile["users"][username] != null)
                        throw new ApplicationException("Sorry, that username is already taken by another user in the system");
                    authFile["users"].Add(username);

                    // Creates a salt for user
                    authFile ["users"].LastChild.Add("cookie-salt", AuthFile.CreateNewSalt ());
                    authFile ["users"].LastChild.Add("password", password);
                    authFile ["users"].LastChild.Add("role", role);
                });

            // Creating newly created user's directory structure
            CreateUserDirectory (rootFolder, username);
        }

        /*
         * Verifies that given username is valid
         */
        private static void VerifyUsernameValid (string username)
        {
            foreach (var charIdx in username) {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_-".IndexOf(charIdx) == -1)
                    throw new SecurityException("Sorry, you cannot use character '" + charIdx + "' in username");
            }
        }

        /*
         * Creates folder structure for user
         */
        private static void CreateUserDirectory (string rootFolder, string username)
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

            if (!Directory.Exists (rootFolder + "/users/" + username + "/tmp"))
                Directory.CreateDirectory(rootFolder + "/users/" + username + "/tmp");

            if (!File.Exists (rootFolder + "/users/" + username + "/documents/private/web.config"))
                File.Copy (
                    rootFolder + "/users/root/documents/private/web.config", 
                    rootFolder + "/users/" + username + "/documents/private/web.config");
        }

        /*
         * Returns all existing roles in system
         */
        internal static void GetRoles (ApplicationContext context, Node args)
        {
            // Getting password file in Node format, such that we can traverse file for all roles
            Node pwdFile = AuthFile.GetAuthFile(context);

            // Looping through each user object in password file, retrieving all roles
            foreach (var idxUserNode in pwdFile["users"].Children) {

                // Checking if currently iterated user's role was already added
                if (args.Children.FirstOrDefault(ix => ix.Name == idxUserNode["role"].Get<string>(context)) == null) {

                    // Default Context role was not already added
                    args.Add(idxUserNode["role"].Get<string>(context));
                }
            }

            // Making sure default role is added
            string defaultRole = context.RaiseNative ("p5.security.get-default-context-role").Get<string> (context);
            if (!string.IsNullOrEmpty(defaultRole)) {

                // There exist a default role, checking if it's already added
                if (args.Children.FirstOrDefault(ix => ix.Name == defaultRole) == null) {

                    // Default Role was not already added, therefor we add it to return lambda node
                    args.Add(defaultRole);
                }
            }
        }

        /*
         * Changes password of "root" account, but only if existing root account's password 
         * is null. Used during setup of server
         */
        internal static void SetRootPassword (ApplicationContext context, Node args)
        {
            // Retrieving password given
            string password = args.GetExChildValue<string>("password", context);
            if (string.IsNullOrEmpty(password))
                throw new SecurityException("You cannot set the root password to empty");

            // Creating Hash of password, with salt from web.config
            using (var sha256 = SHA256.Create ()) {

                // Returning Sha256 hash as base64 encoded string
                var saltAndPassword = context.RaiseNative ("p5.security.get-password-salt").Get<string> (context) + password;
                password = Convert.ToBase64String (sha256.ComputeHash (Encoding.UTF8.GetBytes (saltAndPassword)));
            }

            // Retrieving password file, locking access to it as we do, such that we can change root account's password
            // after first checking that password is actually null!
            AuthFile.ModifyAuthFile (
                context,
                delegate (Node authFile) {
                    if (authFile["users"]["root"]["password"].Value != null)
                        throw new SecurityException("Somebody tried to use installation Active event [p5.web.set-root-password] to change password of existing root account");

                    // Changing password of root account
                    authFile["users"]["root"]["password"].Value = password;
                    authFile["users"]["root"]["cookie-salt"].Value = AuthFile.CreateNewSalt ();
                });
        }

        /*
         * Returns true if root account's password is null, which means that server is not setup yet
         */
        internal static bool RootPasswordIsNull (ApplicationContext context)
        {
            // Retrieving password file, and making sure we lock access to file as we do
            Node rootPwdNode = AuthFile.GetAuthFile(context)["users"]["root"];

            // Returning true if root account's password is null
            return rootPwdNode["password"].Value == null;
        }

        /*
         * Will try to login from persistent cookie
         */
        internal static void TryLoginFromPersistentCookie(ApplicationContext context)
        {
            try {
                // Making sure we do NOT try to login from persistent cookie if root password is null, at which
                // case the system has been reset, and cookie (obviously) is not valid!
                if (RootPasswordIsNull (context)) {

                    // Making sure we delete cookie, since (obviously) it is no longer valid!
                    // The simplest way to do this, is simply to throw exception, which will be handled 
                    // further down, and deletes current cookie!
                    throw new Exception ("foo/bar");
                } else {

                    // Checking if client has persistent cookie
                    HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(_credentialCookieName);
                    if (cookie != null) {

                        // We have a cookie, try to use it as credentials
                        LoginFromCookie (cookie, context);
                    }
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
         * Tries to login with the given cookie as credentials
         */
        private static void LoginFromCookie (HttpCookie cookie, ApplicationContext context)
        {
            // Making sure nobody can reach us by brute force, by supplying a new 
            // cookie in a "brute force cookie login" attempt
            GuardAgainstBruteForce (context);

            // User has persistent cookie associated with client
            var cookieSplits = cookie.Value.Split (' ');
            if (cookieSplits.Length != 2)
                throw new SecurityException ("Cookie not accepted");

            string cookieUsername = cookieSplits[0];
            string cookieHashSaltedPwd = cookieSplits[1];
            Node pwdFile = AuthFile.GetAuthFile(context);

            // Checking if user exist
            Node userNode = pwdFile["users"][cookieUsername];
            if (userNode == null)
                throw new SecurityException ("Cookie not accepted");

            // User exist, retrieving salt and password to see if we have a match
            string salt = userNode["cookie-salt"].Get<string> (context);
            string password = userNode["password"].Get<string> (context);
            string hashSaltedPwd = context.RaiseNative("sha256-hash", new Node("", salt + password)).Get<string>(context);

            // Notice, we do NOT THROW if passwords do not match, since it might simply mean that user has explicitly created a new "salt"
            // to throw out other clients that are currently persistently logged into system under his account
            if (hashSaltedPwd == cookieHashSaltedPwd) {

                // MATCH, discarding previous Context Ticket and creating a new Ticket
                SetTicket (new ApplicationContext.ContextTicket(
                    userNode.Name, 
                    userNode ["role"].Get<string>(context), 
                    false));
                LastLoginAttemptForIP = DateTime.MinValue;
            }
        }

        /*
         * Creates default Context Ticket according to settings from config file
         */
        private static ApplicationContext.ContextTicket CreateDefaultTicket (ApplicationContext context)
        {
            return new ApplicationContext.ContextTicket (
                context.RaiseNative ("p5.security.get-default-context-username").Get<string> (context), 
                context.RaiseNative ("p5.security.get-default-context-role").Get<string> (context), 
                true);
        }

        /*
         * Helper to guard against brute force login attempt. Basically denies an IP address to attempt to login without having
         * to wait a configurable amount of seconds between each attempt
         */
        private static void GuardAgainstBruteForce(ApplicationContext context)
        {
            // We only "turn on" guard after root password has been set, since during installation process of server,
            // user will sign in and out multiple times
            if (AuthenticationHelper.RootPasswordIsNull (context))
                return;

            // Finding delta from last login attempt and "now"
            TimeSpan span = DateTime.Now - LastLoginAttemptForIP;

            // Verifying delta is lower than threshold accepted
            int seconds = context.RaiseNative ("p5.security.get-login-cooloff-seconds").Get<int> (context);
            if (span.TotalSeconds < seconds)
                throw new SecurityException (
                    string.Format (
                        "Your IP address is trying to login to frequently, please wait {0} seconds before trying again.", 
                        seconds));

            // Making sure we set the last login attempt to now!
            LastLoginAttemptForIP = DateTime.Now;
        }

        /*
         * Helper to store "last login attempt" for a specific IP address
         */
        private static DateTime LastLoginAttemptForIP
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