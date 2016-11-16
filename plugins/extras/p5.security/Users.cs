/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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

using p5.core;
using p5.exp.exceptions;
using p5.security.helpers;

namespace p5.security
{
    /// <summary>
    ///     Class wrapping user features of Phosphorus Five
    /// </summary>
    internal static class Users
    {
        /// <summary>
        ///     Creates a new user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "create-user")]
        public static void create_user (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.IsDefault || context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to create new user", e.Args, context);
            AuthenticationHelper.CreateUser (context, e.Args);
        }

        /// <summary>
        ///     Edits an existing user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "edit-user")]
        public static void edit_user (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.IsDefault || context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to edit existing user", e.Args, context);
            AuthenticationHelper.EditUser (context, e.Args);
        }

        /// <summary>
        ///     Lists all users in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "list-users")]
        public static void list_users (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.IsDefault || context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to list all users", e.Args, context);
            AuthenticationHelper.ListUsers (context, e.Args);
        }

        /// <summary>
        ///     Retrieves a specific user in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "get-user")]
        public static void get_user (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.IsDefault || context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to retrieve existing user", e.Args, context);
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.GetUser (context, e.Args);
            }
        }

        /// <summary>
        ///     Deletes a specific user in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "delete-user")]
        public static void delete_user (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.IsDefault || context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to delete existing user", e.Args, context);
            AuthenticationHelper.DeleteUser (context, e.Args);
        }
    }
}