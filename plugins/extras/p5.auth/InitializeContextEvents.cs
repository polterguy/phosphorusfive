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
using p5.core;
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping initialization of application context Active Events.
    /// </summary>
    static class InitializeContextEvents
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
            // Checking if session is null, which it might be, during for instance [p5.core.application-start].
            // At which point we (obviously) don't try to login user from persistent cookie, since there is no HTTP request yet.
            if (HttpContext.Current.Session != null) {

                // Checking if ContextTicket is already set, and if not, we try to login user from persistent cookie.
                if (!Authentication.ContextTicketIsSet) {

                    // No Context Ticket exists in session, trying to login user from persistent cookie.
                    Authentication.TryLoginFromPersistentCookie (context);

                } else {

                    // Associating current context with ticket from session.
                    context.UpdateTicket (Authentication.Ticket);
                }
            }
        }
    }
}