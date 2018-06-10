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

using p5.exp;
using p5.core;
using p5.auth.helpers;
using p5.exp.exceptions;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping user related Active Events.
    /// </summary>
    static class Users
    {
        /// <summary>
        ///     Returns boolean true if password is accepted, otherwise the friendly description for the password regime.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.is-good-password")]
        public static void p5_auth_is_good_password (ApplicationContext context, ActiveEventArgs e)
        {
            if (AuthenticationHelper.IsGoodPassword (context, e.Args.GetExValue<string> (context, ""))) {

                // Password was accepted.
                e.Args.Value = true;

            } else {

                // Password wasnot accepted.
                var pwdRulesNode = new Node (".p5.config.get", "p5.auth.password-rules-info");
                var pwdRule = context.RaiseEvent (".p5.config.get", pwdRulesNode) [0]?.Get (context, "");
                e.Args.Value = pwdRule;
            }
        }

        /// <summary>
        ///     Creates a new user.
        ///     Can only be invoked by a logged in root account.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.users.create")]
        public static void p5_auth_users_create (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to create new user", e.Args, context);
            AuthenticationHelper.CreateUser (context, e.Args);
        }

        /// <summary>
        ///     Retrieves a specific user in system.
        ///     Can only be invoked by a logged in root account.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.users.get")]
        public static void p5_auth_users_get (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to retrieve existing user", e.Args, context);
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.GetUser (context, e.Args);
            }
        }

        /// <summary>
        ///     Edits an existing user.
        ///     Can only be invoked by a logged in root account.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.users.edit")]
        [ActiveEvent (Name = "p5.auth.users.edit-keep-settings")]
        public static void p5_auth_users_edit (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to edit existing user", e.Args, context);
            AuthenticationHelper.EditUser (context, e.Args);
        }

        /// <summary>
        ///     Deletes a specific user in system.
        ///     Can only be invoked by a logged in root account.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.users.delete")]
        public static void p5_auth_users_delete (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to delete existing user", e.Args, context);
            AuthenticationHelper.DeleteUser (context, e.Args);
        }

        /// <summary>
        ///     Lists all users in system.
        ///     Can only be invoked by a logged in root account.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.users.list")]
        public static void p5_auth_users_list (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to list all users", e.Args, context);
            AuthenticationHelper.ListUsers (context, e.Args);
        }
    }
}