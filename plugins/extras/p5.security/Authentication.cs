/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Linq;
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
    public static class Authentication
    {
        // Used to lock access to password file
        private static object _passwordFileLocker = new object ();

        /*
         * Used for user Context Ticket (currently logged in Context user)
         */
        private static ApplicationContext.ContextTicket Ticket
        {
            get {
                if (HttpContext.Current.Session["_ApplicationContext.ContextTicket"] == null) {

                    // No user is logged in, using default impersonated user
                    var configuration = ConfigurationManager.GetSection("activeEventAssemblies") as ActiveEventAssemblies;
                    HttpContext.Current.Session["_ApplicationContext.ContextTicket"] = 
                        new ApplicationContext.ContextTicket (
                            configuration.DefaultContextUsername, 
                            configuration.DefaultContextRole, 
                            true);
                }
                return HttpContext.Current.Session["_ApplicationContext.ContextTicket"] as ApplicationContext.ContextTicket;
            }
            set { 
                HttpContext.Current.Session["_ApplicationContext.ContextTicket"] = value;
            }
        }

        /// <summary>
        ///     Sink to associate a Ticket with ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.core.initialize-application-context", Protected = true)]
        private static void p5_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            if (HttpContext.Current.Session == null)
                return; // Creation of ApplicationContext from [p5.core.application-start], before we have any session
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
            try {

                if (HttpContext.Current.Application["_Last-Forms-Login-Attempt-" + HttpContext.Current.Request.UserHostAddress] != null) {

                    // User has previous unsuccessful login attempts to the system, verifying we're not being "hammered" by brute force attack
                    DateTime lastAttempt = (DateTime)HttpContext.Current.Application["_Last-Forms-Login-Attempt-" + HttpContext.Current.Request.UserHostAddress];
                    if ((DateTime.Now - lastAttempt).TotalSeconds < 30)
                        throw new System.Security.SecurityException("You are trying to login too frequently, wait at least 30 seconds before you try again");
                }

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
                    pwdFile = null;//GetPasswordFile(context);
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
                HttpContext.Current.Application.Remove ("_Last-Forms-Login-Attempt-" + HttpContext.Current.Request.UserHostAddress);

                // Associating newly created Ticket with Application Context, since user now possibly have extended rights
                context.UpdateTicket (Ticket);

                // Checking if we should create persistent cookie on disc to remember username for given client
                // Notice, we do NOT allow root account to persist to cookie
                if (role != "root" && persist) {

                    // Caller wants to create persistent cookie to remember username/password
                    HttpCookie cookie = new HttpCookie("_p5_user");
                    cookie.Expires = DateTime.Now.AddDays(30);
                    cookie.HttpOnly = true;
                    string salt = userNode["salt"].Get<string>(context);
                    cookie.Value = username + " " + context.Raise ("p5.crypto.hash-string", new Node(string.Empty, salt + password)).Value;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
            catch {
                HttpContext.Current.Application["_Last-Forms-Login-Attempt-" + HttpContext.Current.Request.UserHostAddress] = DateTime.Now;
                throw;
            }
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
                throw new System.Security.SecurityException("Sorry, you cannot create a root account through the [create-user] Active Event");

            // We need this guy to save passwords file later
            string rootFolder = context.Raise("p5.core.application-folder").Get<string>(context);

            // Verifying username is valid, since we'll need to create a folder and associate with user later
            foreach (var charIdx in username) {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_-".IndexOf(charIdx) == -1)
                    throw new ApplicationException("Sorry, you cannot use character '" + charIdx + "' in username");
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
                var configuration = ConfigurationManager.GetSection ("activeEventAssemblies") as ActiveEventAssemblies;
                string pwdFilePath = configuration.PasswordFile.Replace("~/", rootFolder);

                using (TextWriter writer = File.CreateText(pwdFilePath)) {
                    Node lambdaNode = new Node();
                    lambdaNode.AddRange(pwdFile.Children);
                    writer.Write(context.Raise ("lambda2lisp", lambdaNode).Get<string> (context));
                }

                // Creating folders for user, and making sure private directory stays private ...
                Directory.CreateDirectory(rootFolder + "users/" + username);
                Directory.CreateDirectory(rootFolder + "users/" + username + "/documents");
                Directory.CreateDirectory(rootFolder + "users/" + username + "/documents/private");
                Directory.CreateDirectory(rootFolder + "users/" + username + "/documents/public");
                Directory.CreateDirectory(rootFolder + "users/" + username + "/tmp");
                File.Copy(
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
                    e.Args.Add(idxUserNode["role"].Get<string>(context));
                }
            }

            // Making sure default role is added
            var configuration = ConfigurationManager.GetSection ("activeEventAssemblies") as ActiveEventAssemblies;
            if (!string.IsNullOrEmpty(configuration.DefaultContextRole)) {
                if (e.Args.Children.FirstOrDefault(ix => ix.Name == configuration.DefaultContextRole) == null) {
                    e.Args.Add(configuration.DefaultContextRole);
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
            var configuration = ConfigurationManager.GetSection ("activeEventAssemblies") as ActiveEventAssemblies;
            string rootFolder = context.Raise("p5.core.application-folder").Get<string>(context);

            // Locking access to password file
            lock (_passwordFileLocker) {

                Node pwdFile = GetPasswordFile(context);
                pwdFile["users"][context.Ticket.Username]["password"].Value = newPwd;
                string pwdFilePath = configuration.PasswordFile.Replace("~/", rootFolder);

                using (TextWriter writer = File.CreateText(pwdFilePath)) {
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
            var configuration = ConfigurationManager.GetSection ("activeEventAssemblies") as ActiveEventAssemblies;
            string rootFolder = context.Raise("p5.core.application-folder").Get<string>(context);
            string pwdFilePath = configuration.PasswordFile.Replace("~", rootFolder);

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

                    if (HttpContext.Current.Application["_Last-Cookie-Login-Attempt-" + HttpContext.Current.Request.UserHostAddress] != null) {

                        // User has been trying to login with cookie previously, which is possibly an intrusion attempt
                        DateTime lastAttempt = (DateTime)HttpContext.Current.Application["_Last-Cookie-Login-Attempt-" + HttpContext.Current.Request.UserHostAddress];
                        if ((DateTime.Now - lastAttempt).TotalSeconds < 30)
                            throw new System.Security.SecurityException ("You have tried to login to this system to rapidly, wait 30 seconds before you try again.");
                    }

                    // User has persistent cookie associated with client
                    var cookieSplits = cookie.Value.Split (' ');
                    if (cookieSplits.Length != 2)
                        throw new System.Security.SecurityException ("Cookie not accepted");

                    string cookieUsername = cookieSplits[0];
                    string cookieHashSaltedPwd = cookieSplits[1];
                    Node pwdFile = null;

                    // Locking access to password file
                    lock (_passwordFileLocker) {
                        pwdFile = GetPasswordFile(context);
                    }

                    Node userNode = pwdFile["users"][cookieUsername];
                    if (userNode == null)
                        throw new System.Security.SecurityException ("Cookie not accepted");

                    // User exists, retrieving salt and password to see if we have a match
                    string salt = userNode["salt"].Get<string> (context);
                    string password = userNode["password"].Get<string> (context);
                    string hashSaltedPwd = context.Raise("p5.crypto.hash-string", new Node(string.Empty, salt + password)).Get<string>(context);
                    if (hashSaltedPwd != cookieHashSaltedPwd)
                        throw new System.Security.SecurityException ("Cookie not accepted");

                    // MATCH, discarding previous ticket and Context to create a new
                    Ticket = new ApplicationContext.ContextTicket(
                        userNode.Name, 
                        userNode ["role"].Get<string>(context), 
                        false);
                    HttpContext.Current.Application.Remove ("_Last-Cookie-Login-Attempt-" + HttpContext.Current.Request.UserHostAddress);
                }
            }
            catch {
                HttpContext.Current.Application["_Last-Cookie-Login-Attempt-" + HttpContext.Current.Request.UserHostAddress] = DateTime.Now;
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("_p5_user");
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
                throw;
            }
        }
    }
}