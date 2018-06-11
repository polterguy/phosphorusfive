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

using System.Linq;
using p5.exp.exceptions;
using p5.core;
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping role related Active Events.
    /// </summary>
    static class RolesEvents
    {
        /// <summary>
        ///     Returns all roles in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.roles.list")]
        public static void p5_auth_roles_list (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure only root account can invoke event.
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to list all roles in system", e.Args, context);

            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {
                GetRoles (context, e.Args);
            }
        }

        /*
         * Helper for above.
         * 
         * Returns all existing roles in system
         */
        private static void GetRoles (ApplicationContext context, Node args)
        {
            // Making sure default role is added first.
            string defaultRole = context.RaiseEvent (".p5.auth.get-default-context-role").Get<string> (context);
            if (!string.IsNullOrEmpty (defaultRole)) {

                // There exist a default role, checking if it's already added
                if (args.Children.FirstOrDefault (ix => ix.Name == defaultRole) == null) {

                    // Default Role was not already added, therefor we add it to return lambda node
                    args.Add (defaultRole);
                }
            }

            // Getting password file in Node format, such that we can traverse file for all roles
            Node pwdFile = AuthFile.GetAuthFile (context);

            // Looping through each user object in password file, retrieving all roles
            foreach (var idxUserNode in pwdFile ["users"].Children) {

                // Retrieving role name of currently iterated user
                var role = idxUserNode ["role"].Get<string> (context);

                // Adding currently iterated role, unless already added, and incrementing user count for it
                args.FindOrInsert (role).Value = args [role].Get (context, 0) + 1;
            }
        }
    }
}