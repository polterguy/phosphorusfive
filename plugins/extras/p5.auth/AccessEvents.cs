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

using p5.core;
using p5.exp.exceptions;
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping access objects Active Events.
    /// </summary>
    static class AccessEvents
    {
        /// <summary>
        ///     Returns all access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.list")]
        public static void p5_auth_access_list (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {
                Access.ListAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Adds an access right object to the system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.add")]
        public static void p5_auth_access_add (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure only root account can invoke event.
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);

            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {
                Access.AddAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Deletes specified access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.delete")]
        public static void p5_auth_access_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure only root account can invoke event.
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);

            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {
                Access.DeleteAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Sets all access right objects for the system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.set")]
        public static void p5_auth_access_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure only root account can invoke event.
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);
            
            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {
                Access.SetAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Verifies that user has access to some sort of "path".
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.has-access")]
        public static void p5_auth_has_access (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, false)) {
                Access.HasAccessToPath (context, e.Args);
            }
        }
    }
}