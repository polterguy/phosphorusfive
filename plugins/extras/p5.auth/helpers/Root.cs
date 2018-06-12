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
    ///     Class wrapping special root account features of Phosphorus Five.
    /// </summary>
    static class Root
    {

        /*
         * Returns true if root account's password is null, which means that server is not setup yet.
         */
        public static bool NoExistingRootAccount (ApplicationContext context)
        {
            // Retrieving password file, and making sure we lock access to file as we do
            var rootPwdNode = AuthFile.GetAuthFile (context) ["users"] ["root"];

            // Returning true if root account does not exist
            return rootPwdNode == null;
        }
        
        /*
         * Changes password of "root" account, but only if existing root account's password 
         * is null. Used during setup of system
         */
        public static void SetRootPassword (ApplicationContext context, Node args)
        {
            // Retrieving password given.
            var password = args.GetExChildValue<string> ("password", context);

            // Verifying password is accepted.
            if (!Passwords.IsGoodPassword (context, password)) {

                // Password was not accepted, throwing an exception.
                args.FindOrInsert ("password").Value = "xxx";
                var description = Passwords.PasswordRuleDescription (context);
                throw new LambdaSecurityException ("Password didn't obey by your configuration settings, which are as follows; " + description, args, context);
            }

            // Creating root account.
            var rootAccountNode = new Node ("", "root");
            rootAccountNode.Add ("password", password);
            rootAccountNode.Add ("role", "root");
            Users.CreateUser (context, rootAccountNode);

            // Creating "guest account" section, which is needed for settings among other things.
            AuthFile.ModifyAuthFile (
                context,
                delegate (Node authFile) {
                    authFile ["users"].Add (context.RaiseEvent (".p5.auth.get-default-context-username").Get<string> (context)).LastChild
                                      .Add ("role", context.RaiseEvent (".p5.auth.get-default-context-role").Get<string> (context));
                });
        }
    }
}