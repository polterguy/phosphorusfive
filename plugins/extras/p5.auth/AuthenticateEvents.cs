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
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping authentication Active Events.
    /// </summary>
    static class AuthenticateEvents
    {
        /// <summary>
        ///     Returns the currently logged in user and its role.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "whoami")]
        [ActiveEvent (Name = "p5.auth.whoami")]
        public static void p5_auth_whoami (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("username", Authentication.Ticket.Username);
            e.Args.Add ("role", Authentication.Ticket.Role);
            e.Args.Add ("default", Authentication.Ticket.IsDefault);
        }

        /// <summary>
        ///     Logs in a user to be associated with the current ApplicationContext, 
        ///     and any future ApplicationContext objects created within the same session.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "login")]
        [ActiveEvent (Name = "p5.auth.login")]
        public static void p5_auth_login (ApplicationContext context, ActiveEventArgs e)
        {
            Authentication.Login (context, e.Args);
        }

        /// <summary>
        ///     Logs out a user from the ApplicationContext.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "logout")]
        [ActiveEvent (Name = "p5.auth.logout")]
        public static void p5_auth_logout (ApplicationContext context, ActiveEventArgs e)
        {
            Authentication.Logout (context);
        }
    }
}