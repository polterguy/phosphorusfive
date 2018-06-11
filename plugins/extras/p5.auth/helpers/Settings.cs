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
using System.Linq;
using System.Globalization;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping settings features of Phosphorus Five
    /// </summary>
    static class Settings
    {
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

            // Checking if caller is retieving a single section.
            var section = args.GetExValue (context, "");
            if (string.IsNullOrEmpty (section)) {
                
                // All settings invocation.
                args.AddRange (authFile ["users"] [context.Ticket.Username].Clone ().Children.Where (ix => ix.Name != "password" && ix.Name != "role"));

            } else if (section != "password" && section != "role") {

                // Single section invocation.
                var sectionNode = authFile ["users"] [context.Ticket.Username] [section]?.Clone ();
                if (sectionNode != null)
                    args.Add (sectionNode);

            } else {

                // Illegal attempt at trying to retrieve role or password.
                throw new LambdaSecurityException ("Illegal invocation, you can't retrieve [password] or [role]", args, context);
            }
        }

        /*
         * Changes the settings for currently logged in user
         */
        public static void ChangeSettings (ApplicationContext context, Node args)
        {
            // Getting username for current context.
            string username = context.Ticket.Username;

            // Making sure default user cannot change his settings.
            if (context.Ticket.IsDefault)
                throw new LambdaSecurityException ("The default user cannot change his settings", args, context);

            // Verifying that there's no "funny business" going on here.
            if (args ["password"] != null || args ["role"] != null)
                throw new LambdaSecurityException ("You cannot change your password or role with this Active Event", args, context);

            // Locking access to password file as we edit user object
            AuthFile.ModifyAuthFile (
                context,
                delegate (Node authFile) {

                    // Checking if invocation is for a single section, or if it's for everything.
                    var section = args.GetExValue (context, "");
                    if (string.IsNullOrEmpty (section)) {

                        // Removing old settings
                        authFile ["users"] [username].RemoveAll (ix => ix.Name != "password" && ix.Name != "role");

                        // Changing all settings for user
                        foreach (var idxNode in args.Children) {
                            authFile ["users"] [username].Add (idxNode.Clone ());
                        }

                    } else if (args.Count == 1) {

                        // Removing old settings
                        authFile ["users"] [username] [section]?.UnTie (); 

                        // Changing all settings for user.
                        authFile ["users"] [username].Add (args.FirstChild.Clone ());

                    } else {

                        // Oops, can't set a single section to multiple values.
                        throw new LambdaException ("You can't set a single section to multiple values", args, context);
                    }
                });
        }
    }
}