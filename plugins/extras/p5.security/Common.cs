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

using System.Web;
using p5.exp;
using p5.core;
using p5.security.helpers;

/// <summary>
///     Main namespace for security features of Phosphorus Five
/// </summary>
namespace p5.security
{
    /// <summary>
    ///     Class wrapping authentication features of Phosphorus Five
    /// </summary>
    internal static class Common
    {
        /// <summary>
        ///     Sink to associate a Ticket with ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.core.initialize-application-context")]
        private static void _p5_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if session is null, which it might be, during for instance [p5.core.application-start]
            if (HttpContext.Current.Session != null) {

                // Checking if ContextTicket is already set, and if not, we try to login user from persistent cookie
                if (!AuthenticationHelper.ContextTicketIsSet) {

                    // No Context Ticket, try to login user from persistent cookie
                    AuthenticationHelper.TryLoginFromPersistentCookie (context);
                }

                // Updating Application Context ticket with ticket from AuthenticationHelper
                context.UpdateTicket (AuthenticationHelper.GetTicket (context));
            }
        }

        /// <summary>
        ///     Returns the password salt for the server to use when storing passwords in "auth" file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent(Name = ".p5.security.get-server-salt")]
        private static void _p5_security_get_server_salt(ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = AuthenticationHelper.ServerSalt(context);
        }

        /// <summary>
        ///     Returns true if server salt is alread set
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.security.has-salt")]
        private static void p5_security_has_salt (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = !string.IsNullOrEmpty (AuthenticationHelper.ServerSalt (context));
        }

        /// <summary>
        ///     Sets the password salt for the server to use when storing passwords in "auth" file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.security.set-server-salt")]
        private static void p5_security_set_server_salt(ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.SetServerSalt (context, e.Args.GetExValue<string> (context));
        }
    }
}