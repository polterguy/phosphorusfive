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
            var pwdRulesNode = new Node (".p5.config.get", "p5.auth.password-rules");
            var pwdRule = context.RaiseEvent (".p5.config.get", pwdRulesNode) [0]?.Get (context, "");
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
            var pwdRulesNode = new Node (".p5.config.get", "p5.auth.password-rules");
            return context.RaiseEvent (".p5.config.get", pwdRulesNode) [0]?.Get<string> (context) ?? "No description of your password rules exists.";
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
                var description = PasswordRuleDescription (context);
                throw new LambdaSecurityException ("Password didn't obey by your configuration settings, which are as follows; " + description, args, context);
            }

            // Figuring out username of current context.
            var username = context.Ticket.Username;

            // Salting and hashing password.
            password = SaltAndHashPassword (context, password);

            // Locking access to password file as we edit user object
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
            var salt = ServerSalt.GetServerSalt (context);
            var hashedPassword = context.RaiseEvent ("p5.crypto.hash.create-sha256", new Node ("", salt + password)).Get<string> (context);
            return hashedPassword;
        }
    }
}