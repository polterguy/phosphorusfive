/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Security;
using System.Configuration;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.core.configuration;

/// <summary>
///     Main namespace for security features of Phosphorus Five
/// </summary>
namespace p5.security
{
    /// <summary>
    ///     Class wrapping authentication features of Phosphorus Five
    /// </summary>
    internal static class Authentication
    {
        // Used to lock access to password file
        private static object _passwordFileLocker = new object ();

        /*
         * Contains user Context Ticket (Context "user")
         */
        private static ApplicationContext.ContextTicket Ticket
        {
            get {
                if (HttpContext.Current.Session["_ContextTicket"] == null) {

                    // No user is logged in, using default impersonated user
                    HttpContext.Current.Session ["_ContextTicket"] = CreateDefaultTicket ();
                }
                return HttpContext.Current.Session["_ContextTicket"] as ApplicationContext.ContextTicket;
            }
            set { 
                HttpContext.Current.Session["_ContextTicket"] = value;
            }
        }

        /*
         * Returns the Phosphorus configuration values
         */
        private static PhosphorusConfiguration Configuration
        {
            get {
                return ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
            }
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

        /// <summary>
        ///     Sink to associate a Ticket with ApplicationContext and initialize ApplicationContext object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.core.initialize-application-context", Protected = true)]
        private static void p5_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            if (HttpContext.Current.Session == null)
                return; // Creation of ApplicationContext from [p5.core.application-start], before we have any session available - Ignoring ...

            // Try to login user from persistent cookie
            TryLoginFromPersistentCookie (context);
            context.UpdateTicket (Ticket);
        }

        /// <summary>
        ///     Logs in a user to be associated with the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "login", Protected = true)]
        private static void login (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking for a brute force login attack
            GuardAgainstBruteForce();

            // Defaulting result of Active Event to unsuccessful
            e.Args.Value = false;

            // Retrieving supplied credentials
            string username = e.Args.GetExChildValue<string> ("username", context);
            string password = e.Args.GetExChildValue<string> ("password", context);
            bool persist = e.Args.GetExChildValue ("persist", context, false);

            // Getting password file in Node format
            Node pwdFile = null;

            // Locking access to password file
            lock (_passwordFileLocker) {
                pwdFile = GetPasswordFile(context);
            }

            // Checking for match for specified username
            Node userNode = pwdFile["users"][username];
            if (userNode == null)
                throw new System.Security.SecurityException("Credentials not accepted");

            // Checking for match on password
            if (userNode["password"].Get<string> (context) != password)
                throw new System.Security.SecurityException("Credentials not accepted");

            // Success, figuring out if user must change passwords, and creating our ticket
            string role = userNode["role"].Get<string>(context);
            Ticket = new ApplicationContext.ContextTicket(
                username, 
                role, 
                false);
            e.Args.Value = true;

            // Removing last login attempt
            LastLoginAttemptForIP = DateTime.MinValue;

            // Associating newly created Ticket with Application Context, since user now possibly have extended rights
            context.UpdateTicket (Ticket);

            // Checking if we should create persistent cookie on disc to remember username for given client
            // Notice, we do NOT allow root account to persist to cookie
            if (role != "root" && persist) {

                // Caller wants to create persistent cookie to remember username/password
                HttpCookie cookie = new HttpCookie("_p5_user");
                cookie.Expires = DateTime.Now.AddDays(90);
                cookie.HttpOnly = true;
                string salt = userNode["salt"].Get<string>(context);
                cookie.Value = username + " " + context.Raise ("p5.crypto.hash-string", new Node(string.Empty, salt + password)).Value;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /*
         * Helper to guard against brute force login attempt
         */
        private static void GuardAgainstBruteForce()
        {
            TimeSpan span = DateTime.Now - LastLoginAttemptForIP;

            // Verifying delta is lower than threshold accepted
            if (span.TotalSeconds < Configuration.LoginCoolOffSeconds)
                throw new SecurityException (
                    string.Format (
                        "Your IP address is trying to login to frequently, please wait {0} seconds before trying again.", 
                        Configuration.LoginCoolOffSeconds));

            // Making sure we set the last login attempt to now!
            LastLoginAttemptForIP = DateTime.Now;
        }

        /// <summary>
        ///     Logs out a user from the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "logout", Protected = true)]
        private static void logout (ApplicationContext context, ActiveEventArgs e)
        {
            // By destroying this session value, default user will be used in future
            Ticket = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("_p5_user");
            if (cookie != null) {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        ///     Returns the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "user", Protected = true)]
        private static void user (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add("username", Ticket.Username);
            e.Args.Add("role", Ticket.Role);
            e.Args.Add("default", Ticket.IsDefault);
        }

        /// <summary>
        ///     Creates a new user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "create-user", Protected = true)]
        private static void create_user (ApplicationContext context, ActiveEventArgs e)
        {
            string username = e.Args.GetExChildValue<string>("username", context);
            string password = e.Args.GetExChildValue<string>("password", context);
            string role = e.Args.GetExChildValue<string>("role", context);
            if (role == "root")
                throw new SecurityException("Sorry, you cannot create a root account through the [create-user] Active Event");

            // We need this guy to save passwords file later
            string rootFolder = context.Raise("p5.core.application-folder").Get<string>(context);

            // Verifying username is valid, since we'll need to create a folder and associate with user later
            foreach (var charIdx in username) {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_-".IndexOf(charIdx) == -1)
                    throw new SecurityException("Sorry, you cannot use character '" + charIdx + "' in username");
            }

            // Locking access to password file
            lock (_passwordFileLocker) {

                Node pwdFile = GetPasswordFile(context);
                if (pwdFile["users"][username] != null)
                    throw new ApplicationException("Sorry, that username is already taken by another user in the system");
                pwdFile["users"].Add(username);

                // Creating a salt for user
                var salt = "";
                for (var idxRndNo = 0; idxRndNo < new Random (DateTime.Now.Millisecond).Next (1,5); idxRndNo++) {
                    salt += Guid.NewGuid().ToString();
                }
                pwdFile ["users"].LastChild.Add("salt", salt);
                pwdFile ["users"].LastChild.Add("password", password);
                pwdFile ["users"].LastChild.Add("role", role);

                // Saving password file
                string pwdFilePath = Configuration.AuthFile.Replace("~/", rootFolder);

                using (TextWriter writer = File.CreateText(pwdFilePath)) {
                    Node lambdaNode = new Node();
                    lambdaNode.AddRange(pwdFile.Children);
                    writer.Write(context.Raise ("lambda2lisp", lambdaNode).Get<string> (context));
                }

                // Creating folders for user, and making sure private directory stays private ...
                if (!Directory.Exists (rootFolder + "users/" + username))
                    Directory.CreateDirectory (rootFolder + "users/" + username);

                if (!Directory.Exists (rootFolder + "users/" + username + "/documents"))
                    Directory.CreateDirectory(rootFolder + "users/" + username + "/documents");

                if (!Directory.Exists (rootFolder + "users/" + username + "/documents/private"))
                    Directory.CreateDirectory(rootFolder + "users/" + username + "/documents/private");

                if (!Directory.Exists (rootFolder + "users/" + username + "/documents/public"))
                    Directory.CreateDirectory(rootFolder + "users/" + username + "/documents/public");

                if (!Directory.Exists (rootFolder + "users/" + username + "/tmp"))
                    Directory.CreateDirectory(rootFolder + "users/" + username + "/tmp");

                if (!File.Exists (rootFolder + "users/" + username + "/documents/private/web.config"))
                    File.Copy (
                        rootFolder + "users/root/documents/private/web.config", 
                        rootFolder + "users/" + username + "/documents/private/web.config");
            }
        }

        /// <summary>
        ///     Returns all roles in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "roles", Protected = true)]
        private static void roles (ApplicationContext context, ActiveEventArgs e)
        {
            // Getting password file in Node format, such that we can traverse file for all roles
            Node pwdFile = null;

            // Locking access to password file
            lock (_passwordFileLocker) {
                pwdFile = GetPasswordFile(context);
            }

            foreach (var idxUserNode in pwdFile["users"].Children) {
                if (e.Args.Children.FirstOrDefault(ix => ix.Name == idxUserNode["role"].Get<string>(context)) == null) {

                    // Role was not already added
                    e.Args.Add(idxUserNode["role"].Get<string>(context));
                }
            }

            // Making sure default role is added
            if (!string.IsNullOrEmpty(Configuration.DefaultContextRole)) {
                if (e.Args.Children.FirstOrDefault(ix => ix.Name == Configuration.DefaultContextRole) == null) {

                    // Role was not already added
                    e.Args.Add(Configuration.DefaultContextRole);
                }
            }
        }

        /*
         * Invoked during installation. Sets root password, but only if existing password is null!
         */
        [ActiveEvent (Name = "p5.web.set-root-password")]
        private static void p5_web_set_root_password (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving password given
            string password = e.Args.GetExChildValue<string>("password", context);
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("You cannot set the root password to empty");

            // Retrieving password file, and making sure existing root password is null!
            Node rootPwdNode = null;

            // Locking access to password file
            lock (_passwordFileLocker) {
                rootPwdNode = GetPasswordFile(context)["users"]["root"];
            }

            if (rootPwdNode["password"].Value != null)
                throw new System.Security.SecurityException("Somebody tried to use installation Active event [p5.web.set-root-password] to change password of root account");

            // Logging in root user now, before changing password
            Ticket = new ApplicationContext.ContextTicket("root", "root", false);
            context.UpdateTicket(Ticket);
            ChangePassword(context, password);
        }

        /*
         * Invoked during installation. Returns true if root password is null (server needs setup)
         */
        [ActiveEvent (Name = "p5.web.root-password-is-null", Protected = true)]
        private static void p5_web_root_password_is_null (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving password file, and making sure existing root password is null!
            Node rootPwdNode = null;

            // Locking access to password file
            lock (_passwordFileLocker) {
                rootPwdNode = GetPasswordFile(context)["users"]["root"];
            }

            e.Args.Value = rootPwdNode["password"].Value == null;
        }

        /*
         * Changes password of the currently logged in Context user account
         */
        private static void ChangePassword (ApplicationContext context, string newPwd)
        {
            string rootFolder = context.Raise("p5.core.application-folder").Get<string>(context);

            // Locking access to password file
            lock (_passwordFileLocker) {

                // Retrieving password file
                Node pwdFile = GetPasswordFile(context);

                // Changing user's password
                pwdFile["users"][context.Ticket.Username]["password"].Value = newPwd;

                // Saving password file to disc
                using (TextWriter writer = File.CreateText(Configuration.AuthFile.Replace("~/", rootFolder))) {

                    // Creating Hyperlisp out of lambda password file
                    Node lambdaNode = new Node();
                    lambdaNode.AddRange(pwdFile.Children);
                    writer.Write(context.Raise ("lambda2lisp", lambdaNode).Get<string> (context));
                }
            }
        }

        /*
         * Helper to retrieve "_passwords" file
         */
        private static Node GetPasswordFile (ApplicationContext context)
        {
            // Getting filepath to pwd file
            string rootFolder = context.Raise("p5.core.application-folder").Get<string>(context);
            string pwdFilePath = Configuration.AuthFile.Replace("~", rootFolder);

            // Checking file exist
            if (!File.Exists(pwdFilePath))
                CreateDefaultPasswordFile (context, pwdFilePath);

            // Reading up passwords file
            using (TextReader reader = new StreamReader(File.OpenRead(pwdFilePath))) {

                // Returning file as lambda
                string users = reader.ReadToEnd();
                Node usersNode = context.Raise("lisp2lambda", new Node(string.Empty, users));
                return usersNode;
            }
        }

        /*
         * Creates a default "_passwords" file, and a default "root" user
         */
        private static void CreateDefaultPasswordFile (ApplicationContext context, string pwdFile)
        {
            // Checking if ".passwords" file exist, and if not, creating a default
            using (TextWriter writer = File.CreateText(pwdFile)) {

                // Creating default root password, salt unique to user, and writing to file
                var salt = "";
                for (var idxRndNo = 0; idxRndNo < new Random (DateTime.Now.Millisecond).Next (1,5); idxRndNo++) {
                    salt += Guid.NewGuid().ToString();
                }
                salt = salt.Replace("-", "");
                writer.WriteLine(@"users");
                writer.WriteLine(@"  root");
                writer.WriteLine(@"    salt:" + salt);
                writer.WriteLine(@"    password");
                writer.WriteLine(@"    role:root");
            }
        }

        /*
         * Will try to login from persistent cookie
         */
        private static void TryLoginFromPersistentCookie(ApplicationContext context)
        {
            try {

                // Checking if client has persistent cookie
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("_p5_user");
                if (cookie != null) {

                    GuardAgainstBruteForce ();

                    // User has persistent cookie associated with client
                    var cookieSplits = cookie.Value.Split (' ');
                    if (cookieSplits.Length != 2)
                        throw new SecurityException ("Cookie not accepted");

                    string cookieUsername = cookieSplits[0];
                    string cookieHashSaltedPwd = cookieSplits[1];
                    Node pwdFile = null;

                    // Locking access to password file
                    lock (_passwordFileLocker) {
                        pwdFile = GetPasswordFile(context);
                    }

                    Node userNode = pwdFile["users"][cookieUsername];
                    if (userNode == null)
                        throw new SecurityException ("Cookie not accepted");

                    // User exists, retrieving salt and password to see if we have a match
                    string salt = userNode["salt"].Get<string> (context);
                    string password = userNode["password"].Get<string> (context);
                    string hashSaltedPwd = context.Raise("p5.crypto.hash-string", new Node(string.Empty, salt + password)).Get<string>(context);
                    if (hashSaltedPwd != cookieHashSaltedPwd)
                        throw new SecurityException ("Cookie not accepted");

                    // MATCH, discarding previous ticket and Context to create a new
                    Ticket = new ApplicationContext.ContextTicket(
                        userNode.Name, 
                        userNode ["role"].Get<string>(context), 
                        false);
                    LastLoginAttemptForIP = DateTime.MinValue;
                }
            }
            catch {

                // Making sure we delete cookie before we rethrow exception
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("_p5_user");
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
                throw;
            }
        }

        /*
         * Creates default Context Ticket according to settings from config file
         */
        private static ApplicationContext.ContextTicket CreateDefaultTicket ()
        {
            return new ApplicationContext.ContextTicket (
                Configuration.DefaultContextUsername, 
                Configuration.DefaultContextRole, 
                true);
        }
    }
}