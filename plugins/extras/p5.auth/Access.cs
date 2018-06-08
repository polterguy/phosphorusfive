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
    ///     Class wrapping access associated Active Events.
    /// </summary>
    static class Access
    {
        /// <summary>
        ///     Returns all access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.list")]
        public static void p5_auth_access_list (ApplicationContext context, ActiveEventArgs e)
        {
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.ListAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Returns all access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.add")]
        public static void p5_auth_access_add (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.AddAccess (context, e.Args);
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
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.DeleteAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Deletes specified access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.set-all")]
        public static void p5_auth_access_set_all (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.SetAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Verifies that user has access to some sort of path.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.has-access-to-path")]
        public static void p5_auth_has_access_to_path (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, false)) {
                AuthenticationHelper.HasAccessToPath (context, e.Args);
            }
        }
    }
}