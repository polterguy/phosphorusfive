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

using System.Text.RegularExpressions;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping password features of Phosphorus Five
    /// </summary>
    static class Passwords
    {
        /*
         * Checks if password is good and in accordance to password regime.
         */
        public static bool IsGoodPassword (ApplicationContext context, string password)
        {
            // Retrieving password rules from web.config, if any.
            var pwdRulesNode = new Node (".p5.config.get", "p5.auth.password-rules");
            var pwdRule = context.RaiseEvent (".p5.config.get", pwdRulesNode) [0]?.Get (context, "");
            if (!string.IsNullOrEmpty (pwdRule)) {

                // Verifying that specified password obeys by rules from web.config.
                Regex regex = new Regex (pwdRule);
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
        internal static string PasswordRule (ApplicationContext context)
        {
            var pwdRulesNode = new Node (".p5.config.get", "p5.auth.password-rules");
            return context.RaiseEvent (".p5.config.get", pwdRulesNode) [0]?.Get<string> (context) ?? "No description of your password rules exists.";
        }

        /*
         * Changes the password for currently logged in user
         */
        public static void ChangePassword (ApplicationContext context, Node args)
        {
            // Retrieving new password, and doing some basic sanity check.
            string password = args.GetExValue (context, "");
            if (string.IsNullOrEmpty (password))
                throw new LambdaException ("No password supplied", args, context);
            
            // Retrieving password rules from web.config, if any.
            var pwdRulesNode = new Node (".p5.config.get", "p5.auth.password-rules");
            var pwdRule = context.RaiseEvent (".p5.config.get", pwdRulesNode) [0]?.Get (context, "");
            if (!string.IsNullOrEmpty (pwdRule)) {

                // Verifying that specified password obeys by rules from web.config.
                Regex regex = new Regex (pwdRule);
                if (!regex.IsMatch (password)) {

                    // New password was not accepted, throwing an exception.
                    args.FindOrInsert ("password").Value = "xxx";
                    throw new LambdaSecurityException ("Password didn't obey by your configuration settings, which are as follows; " + pwdRule, args, context);
                }
            }

            // Figuring out username of current context.
            string username = context.Ticket.Username;

            // Retrieving system salt before we enter write lock.
            var serverSalt = ServerSalt.GetServerSalt (context);

            // Locking access to password file as we edit user object
            AuthFile.ModifyAuthFile (
                context,
                delegate (Node authFile) {

                    // Changing user's password
                    // Then salting password with user salt and system, before salting it with system salt
                    var userPasswordFingerprint = context.RaiseEvent ("p5.crypto.hash.create-sha256", new Node ("", serverSalt + password)).Get<string> (context);
                    authFile ["users"] [username] ["password"].Value = userPasswordFingerprint;
                });
        }
    }
}