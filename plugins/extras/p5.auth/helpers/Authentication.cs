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
using System.Web;
using System.Security;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping Login and Logout features of Phosphorus Five
    /// </summary>
    static class Authentication
    {
        // Name of credential cookie, used to store username and hashsalted password
        const string _credentialCookieName = "_p5_user";
        const string _credentialsNotAcceptedException = "Credentials not accepted";
        const string _contextTicketSessionName = ".p5.auth.context-ticket";
        const string _cooldownPeriodConfigName = ".p5.auth.cooldown-period";
        const string _bruteForceCacheName = ".p5.io.last-login-attempt-for-";
        const string _guestAccountActiveEventName = ".p5.auth.get-default-context-username";
        const string _guestAccountActiveEventRole = ".p5.auth.get-default-context-role";
        static readonly ContextTicket _rootAccountStartupTicket = new ContextTicket ("root", "root", false);

        /*
         * Returns user Context Ticket to caller.
         */
        public static ContextTicket Ticket {
            get {
                if (HttpContext.Current.Session == null) {

                    /*
                     * Since we have no session object yet, we impersonate the root account, to
                     * make sure initialization and startup of server happens within the context
                     * of the default root account.
                     */
                    return _rootAccountStartupTicket;
                }
                return HttpContext.Current.Session [_contextTicketSessionName] as ContextTicket;
            }
            private set {
                HttpContext.Current.Session [_contextTicketSessionName] = value;
            }
        }

        /*
         * Returns true if Context Ticket is already set.
         */
        public static bool ContextTicketIsSet {
            get {
                return HttpContext.Current.Session != null && Ticket != null;
            }
        }

        /*
         * Tries to login user according to given user credentials.
         */
        public static void Login (ApplicationContext context, Node args)
        {
            // Defaulting result of Active Event to unsuccessful.
            args.Value = false;

            // Retrieving supplied credentials
            string username = args.GetExChildValue<string> ("username", context);
            string password = args.GetExChildValue<string> ("password", context);
            args.FindOrInsert ("password").Value = "xxx"; // In case an exception occurs.
            bool persist = args.GetExChildValue ("persist", context, false);

            /*
             * Checking if current username has attempted to login just recently, and the
             * configured timespan for each successive login attempt per user, has not passed.
             * 
             * This should be able to defend us from a "brute force password attack".
             * 
             * Notice, we also use the web cache, to avoid having an adversary able to 
             * flood the memory of the server, by providing multiple different passwords,
             * which if we used the application object would flood it with new entries
             * until the server's memory was exhausted.
             */
            var bruteConf = new Node (".p5.config.get", _cooldownPeriodConfigName);
            var cooldown = context.RaiseEvent (".p5.config.get", bruteConf) [0]?.Get (context, -1) ?? -1;
            if (cooldown != -1) {

                // User has configured the system to have a "cooldown period" for successive login attempts.
                var bruteForceLastAttempt = new Node (".p5.web.cache.get", _bruteForceCacheName + username);
                var lastAttemptNode = context.RaiseEvent (".p5.web.cache.get", bruteForceLastAttempt);
                if (lastAttemptNode.Count > 0) {

                    // Previous attempt has been attempted.
                    var date = lastAttemptNode [0].Get<DateTime> (context, DateTime.MinValue);
                    int timeSpanSeconds = Convert.ToInt32 ((DateTime.Now - date).TotalSeconds);
                    if (timeSpanSeconds < cooldown) {

                        // Cooldown period has not passed.
                        throw new LambdaException ("You need to wait " + (cooldown - timeSpanSeconds) + " seconds before you can try again", args, context);
                    }
                }
            }
            
            // Getting system salt.
            var serverSalt = ServerSalt.GetServerSalt (context);

            // Then creating system fingerprint from given password.
            var hashedPassword = context.RaiseEvent ("p5.crypto.hash.create-sha256", new Node ("", serverSalt + password)).Get<string> (context);

            // Retrieving password file as a Node.
            Node pwdFile = AuthFile.GetAuthFile (context);

            /*
             * Checking for match on specified username.
             * 
             * Notice, we do this after we have retrieved server salt and created our hash, to make it more
             * difficult for an adversary to "guess" usernames in the system by brute force, trying multiple
             * different usernames, and record the time it took for a failed attempt to return to client.
             * 
             * We also throw the exact same exception if the username doesn't exist, as we do if the passwords
             * don't match. This might be paranoia level of 12 out of 10, but allows the application developer
             * to avoid communicating anything out about the system, if he needs that kind of security.
             */
            Node userNode = pwdFile ["users"] [username];
            if (userNode == null) {

                // Username doesn't exist.
                throw new LambdaSecurityException (_credentialsNotAcceptedException, args, context);
            }

            // Checking for match on password.
            if (userNode ["password"].Get<string> (context) != hashedPassword) {

                // Making sure we guard against brute force password attacks, before we throw security exception.
                var bruteForceLastAttempt = new Node (".p5.web.cache.set", _bruteForceCacheName + username);
                bruteForceLastAttempt.Add ("src", DateTime.Now);
                context.RaiseEvent (".p5.web.cache.set", bruteForceLastAttempt);
                throw new LambdaSecurityException (_credentialsNotAcceptedException, args, context);
            }

            // Success, creating our context ticket.
            string role = userNode ["role"].Get<string> (context);
            SetTicket (context, new ContextTicket (username, role, false));
            args.Value = true;

            // Checking if we should create persistent cookie on disc to remember username for given client.
            if (persist) {

                // Caller wants to create persistent cookie to remember username/password.
                var cookie = new HttpCookie (_credentialCookieName);
                cookie.Expires = DateTime.Now.AddDays (context.RaiseEvent (
                    ".p5.config.get",
                    new Node (".p5.config.get", "p5.auth.credential-cookie-valid")) [0].Get<int> (context));
                cookie.HttpOnly = true; // To avoid JavaScript access to credential cookie.

                /*
                 * The value of our cookie is in "username hashed-password" format.
                 *
                 * This is an entropy of roughly 1.1579e+77, making a brute force attack
                 * impossible, at least without a Rainbow/Dictionary attack, which should
                 * be effectively prevented, by having a single static server salt,
                 * which again is cryptographically secured and persisted to disc
                 * in the "auth" file, and hence normally inaccessible for an adversary.
                 */
                cookie.Value = username + " " + hashedPassword;
                HttpContext.Current.Response.Cookies.Add (cookie);
            }

            // Making sure we invoke an [.onlogin] lambda callbacks for user.
            var onLogin = new Node ();
            Settings.GetSettings (context, onLogin);
            if (onLogin [".onlogin"] != null) {
                var lambda = onLogin [".onlogin"].Clone ();
                context.RaiseEvent ("eval", lambda);
            }
        }
        
        /*
         * Will try to login from persistent cookie.
         */
        public static void TryLoginFromPersistentCookie (ApplicationContext context)
        {
            try {

                // Making sure we do NOT try to login from persistent cookie if root password is null, at which
                // case the system has not been initialized yet, and cookie (obviously) is not valid.
                if (Root.NoExistingRootAccount (context)) {

                    /*
                     * Making sure we delete cookie, since (obviously) it is no longer valid.
                     * This might occur if system has been previously setup, creating a persistent cookie, for then
                     * to have its "auth.hl" file explicitly deleted.
                     *
                     * The simplest way to do this, is simply to throw an exception, which will be handled 
                     * further down, and deletes current cookie.
                     */
                    throw null;
                }

                // Checking if client has persistent cookie.
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get (_credentialCookieName);
                if (cookie != null) {

                    // We have a cookie, try to use it as credentials.
                    LoginFromCookie (cookie, context);
                }

            } catch {

                // Making sure we delete cookie if it exists, by setting Expires to yesterday.
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get (_credentialCookieName);
                if (cookie != null) {

                    // Deleting cookie.
                    cookie.Expires = DateTime.Now.AddDays (-1);
                    HttpContext.Current.Response.Cookies.Add (cookie);
                }

                // Making sure we use default ticket.
                SetTicket (context, CreateDefaultTicket (context));
            }
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

            string cookieUsername = cookieSplits [0];
            string hashedPassword = cookieSplits [1];
            Node pwdFile = AuthFile.GetAuthFile (context);

            // Checking if user exist
            Node userNode = pwdFile ["users"] [cookieUsername];
            if (userNode == null)
                throw new SecurityException ("Cookie not accepted");

            // Notice, we do NOT THROW if passwords do not match, since it might simply mean that user has explicitly created a new "salt"
            // to throw out other clients that are currently persistently logged into system under his account
            if (hashedPassword == userNode ["password"].Get<string> (context)) {

                // MATCH, discarding previous Context Ticket and creating a new Ticket
                SetTicket (context, new ContextTicket (userNode.Name, userNode ["role"].Get<string> (context), false));
            } else {

                // Catched above, which destroys cookie, and associates the default context with user.
                throw new Exception ();
            }
        }

        /*
         * Logs out user.
         */
        public static void Logout (ApplicationContext context)
        {
            // Making sure we invoke an [.onlogin] lambda callbacks for user.
            var onLogout = new Node ();
            Settings.GetSettings (context, onLogout);
            if (onLogout [".onlogout"] != null) {
                var lambda = onLogout [".onlogout"].Clone ();
                context.RaiseEvent ("eval", lambda);
            }

            // By destroying Ticket, default context ticket will be used for current session, until user logs in again.
            DestroyTicket (context);

            // Destroying persistent credentials cookie, if there is one.
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get (_credentialCookieName);
            if (cookie != null) {

                // Making sure cookie is destroyed on the client side by setting its expiration date to "today - 1 day".
                cookie.Expires = DateTime.Now.AddDays (-1);
                HttpContext.Current.Response.Cookies.Add (cookie);
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Sets user's Context Ticket.
         */
        static void SetTicket (ApplicationContext context, ContextTicket ticket)
        {
            // Storing ticket in context.
            Ticket = ticket;
            
            // Associating newly created Ticket with Application Context, since user now possibly have extended rights.
            context.UpdateTicket (ticket);
        }

        /*
         * Destroys ticket, which occurs when user logs out for instance.
         * 
         * This will make sure the default "guest" account's ticket is associated with
         * the session.
         */
        static void DestroyTicket (ApplicationContext context)
        {
            context.UpdateTicket (CreateDefaultTicket (context));
            Ticket = context.Ticket;
        }

        /*
         * Creates default Context Ticket according to settings from config file.
         */
        static internal ContextTicket CreateDefaultTicket (ApplicationContext context)
        {
            return new ContextTicket (
                context.RaiseEvent (_guestAccountActiveEventName).Get<string> (context),
                context.RaiseEvent (_guestAccountActiveEventRole).Get<string> (context),
                true);
        }

        #endregion
    }
}