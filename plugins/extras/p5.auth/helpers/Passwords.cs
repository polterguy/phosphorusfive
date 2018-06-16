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

using System.Web;
using System.Text.RegularExpressions;
using DevOne.Security.Cryptography.BCrypt;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping password features of Phosphorus Five.
    /// </summary>
    static class Passwords
    {
        /*
         * Checks if password is good and in accordance to password regime.
         */
        public static bool IsGoodPassword (ApplicationContext context, string password)
        {
            // Retrieving password rules from web.config, if any.
            var pwdRule = context.RaiseEvent (
                ".p5.config.get",
                new Node (".p5.config.get", "p5.auth.password-rules")) [0]?.Get (context, "");

            // Verifying we have a password rule.
            if (!string.IsNullOrEmpty (pwdRule)) {

                // Verifying that specified password obeys by rules from web.config.
                var regex = new Regex (pwdRule);
                if (!regex.IsMatch (password)) {

                    // New password was not accepted, returning false.
                    return false;
                }
            }
            return true;
        }

        /*
         * Returns the friendly description of the password rules for the installation.
         */
        internal static string PasswordRuleDescription (ApplicationContext context)
        {
            return context.RaiseEvent (
                ".p5.config.get",
                new Node (".p5.config.get", "p5.auth.password-rules-info")) [0]?.Get<string> (context)
                          ?? "No description of your password rules exists.";
        }

        /*
         * Changes the password for currently logged in user.
         */
        public static void ChangeMyPassword (ApplicationContext context, Node args)
        {
            // Retrieving new password.
            var password = args.GetExValue (context, "");

            // Verifying new password is good.
            if (!IsGoodPassword (context, password)) {

                // New password was not accepted, throwing an exception.
                args.FindOrInsert ("password").Value = "xxx";
                throw new LambdaSecurityException (
                    "Password didn't obey by your configuration settings, which are as follows; " +
                    PasswordRuleDescription (context),
                    args,
                    context);
            }

            // Figuring out username of current context.
            var username = context.Ticket.Username;

            // Salting and hashing password before we enter "auth" file lock to minimize the amount of time file is locked.
            password = SaltAndHashPassword (context, password);

            // Locking access to password file as we edit user object.
            AuthFile.ModifyAuthFile (
                context,
                delegate (Node authFile) {

                    // Changing user's password by first salting it, and then hashing it.
                    authFile ["users"] [username] ["password"].Value = password;
                });
        }

        /*
         * Salts and hashes password and returns results back to caller.
         */
        public static string SaltAndHashPassword (ApplicationContext context, string password)
        {
            var logRounds = context.RaiseEvent (
                ".p5.config.get",
                new Node (".p5.config.get", ".p5.crypto.blow-fish-workload")) [0]?.Get (context, 10) ?? 10;
            return BCryptHelper.HashPassword (password, BCryptHelper.GenerateSalt (logRounds));
        }

        /*
         * Checks if password is correct.
         */
        public static bool VerifyPasswordIsCorrect (string password, string hashed)
        {
            return BCryptHelper.CheckPassword (password, hashed);
        }

        /*
         * Hashes password for storing it in cookie.
         */
        public static string HashPasswordForCookieStorage (ApplicationContext context, string password)
        {
            return context.RaiseEvent (
                "p5.crypto.sha256.hash",
                new Node ("", password + GetClientFingerprint ())).Get<string> (context);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Returns a fingerprint for current client, reducing the possibility of credential
         * cookie theft.
         * 
         * Notice, this means the credential cookie becomes invalidated when the browser is updated,
         * or the language preferences of the user is changed, etc.
         */
        static string GetClientFingerprint ()
        {
            var retVal = HttpContext.Current.Request.UserAgent ?? "";
            foreach (var idxLang in HttpContext.Current.Request.UserLanguages ?? new string [] { }) {
                retVal += idxLang;
            }
            return retVal;
        }

        #endregion
    }
}