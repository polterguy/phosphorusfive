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

namespace p5.security
{
    /// <summary>
    ///     Class wrapping authentication helper features of Phosphorus Five
    /// </summary>
    internal static class AuthenticationHelper
    {
        // Used to lock access to password file
        private static object _passwordFileLocker = new object ();

        /*
         * Contains user Context Ticket (Context "user")
         */
        internal static ApplicationContext.ContextTicket Ticket
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
        internal static PhosphorusConfiguration Configuration
        {
            get {
                return ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
            }
        }

        /*
         * Helper to store "last login attempt" for a specific IP address
         */
        internal static DateTime LastLoginAttemptForIP
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

        internal static void Login (ApplicationContext context, Node args)
        {
            // Checking for a brute force login attack
            AuthenticationHelper.GuardAgainstBruteForce();

            // Defaulting result of Active Event to unsuccessful
            args.Value = false;

            // Retrieving supplied credentials
            string username = args.GetExChildValue<string> ("username", context);
            string password = args.GetExChildValue<string> ("password", context);
            bool persist = args.GetExChildValue ("persist", context, false);

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
            args.Value = true;

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

        internal static void Logout (ApplicationContext context)
        {
            // By destroying this session value, default user will be used in future
            Ticket = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("_p5_user");
            if (cookie != null) {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        internal static void CreateUser (ApplicationContext context, Node args)
        {
            string username = args.GetExChildValue<string>("username", context);
            string password = args.GetExChildValue<string>("password", context);
            string role = args.GetExChildValue<string>("role", context);
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

        internal static void GetRoles (ApplicationContext context, Node args)
        {
            // Getting password file in Node format, such that we can traverse file for all roles
            Node pwdFile = null;

            // Locking access to password file
            lock (_passwordFileLocker) {
                pwdFile = GetPasswordFile(context);
            }

            foreach (var idxUserNode in pwdFile["users"].Children) {
                if (args.Children.FirstOrDefault(ix => ix.Name == idxUserNode["role"].Get<string>(context)) == null) {

                    // Role was not already added
                    args.Add(idxUserNode["role"].Get<string>(context));
                }
            }

            // Making sure default role is added
            if (!string.IsNullOrEmpty(Configuration.DefaultContextRole)) {
                if (args.Children.FirstOrDefault(ix => ix.Name == Configuration.DefaultContextRole) == null) {

                    // Role was not already added
                    args.Add(Configuration.DefaultContextRole);
                }
            }
        }

        internal static void SetRootPassword (ApplicationContext context, Node args)
        {
            // Retrieving password given
            string password = args.GetExChildValue<string>("password", context);
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

        internal static bool RootPasswordIsNull (ApplicationContext context)
        {
            // Retrieving password file, and making sure existing root password is null!
            Node rootPwdNode = null;

            // Locking access to password file
            lock (_passwordFileLocker) {
                rootPwdNode = GetPasswordFile(context)["users"]["root"];
            }

            return rootPwdNode["password"].Value == null;
        }

        /*
         * Helper to guard against brute force login attempt
         */
        internal static void GuardAgainstBruteForce()
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

        /*
         * Changes password of the currently logged in Context user account
         */
        internal static void ChangePassword (ApplicationContext context, string newPwd)
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
        internal static Node GetPasswordFile (ApplicationContext context)
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
        internal static void CreateDefaultPasswordFile (ApplicationContext context, string pwdFile)
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
        internal static void TryLoginFromPersistentCookie(ApplicationContext context)
        {
            if (HttpContext.Current.Session == null)
                return; // Creation of ApplicationContext from [p5.core.application-start], before we have any session available - Ignoring ...
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
                    context.UpdateTicket (AuthenticationHelper.Ticket);
                }
            } catch {

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
        internal static ApplicationContext.ContextTicket CreateDefaultTicket ()
        {
            return new ApplicationContext.ContextTicket (
                Configuration.DefaultContextUsername, 
                Configuration.DefaultContextRole, 
                true);
        }
    }
}