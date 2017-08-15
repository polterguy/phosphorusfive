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

using System.Web;
using p5.exp;
using p5.core;
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping initialization of authentication.
    /// </summary>
    static class Initialization
    {
        /// <summary>
        ///     Associates an ApplicationContext with a ContextTicket, meaning an authenticated user.
        ///     Invoked by the core whenever a new ApplicationContext is created.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.core.initialize-application-context")]
        static void _p5_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
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
        ///     Returns the server salt, used among other things, when serializing passwords and such to persistent medium.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent(Name = ".p5.auth.get-server-salt")]
        static void _p5_auth_get_server_salt(ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = AuthenticationHelper.ServerSalt(context);
        }

        /// <summary>
        ///     Returns true if a server salt is already created.
        ///     Notice, a server salt, can only be created initially during setup of server. Any attempts at trying to change it afterwards, will
        ///     result in an exception, since this would make it impossible to login to the system, sine the login process is dependent upon the same
        ///     server salt, as when initially creating the passwords for the user(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.auth._has-salt")]
        static void p5_auth__has_salt (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = !string.IsNullOrEmpty (AuthenticationHelper.ServerSalt (context));
        }

        /// <summary>
        ///     Sets the server salt for the server.
        ///     Invoked once initially during server setup.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.auth._set-server-salt")]
        static void p5_auth__set_server_salt(ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.SetServerSalt (context, e.Args, e.Args.GetExValue<string> (context));
        }
    }
}