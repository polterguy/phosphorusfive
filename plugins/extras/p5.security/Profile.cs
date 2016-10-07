/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using p5.core;
using p5.security.helpers;

namespace p5.security
{
    /// <summary>
    ///     Class wrapping profile features of Phosphorus Five
    /// </summary>
    internal static class Profile
    {
        /// <summary>
        ///     Logs in a user to be associated with the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "login")]
        public static void login (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.Login (context, e.Args);
        }

        /// <summary>
        ///     Logs out a user from the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "logout")]
        public static void logout (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.Logout (context);
        }

        /// <summary>
        ///     Returns the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "whoami")]
        public static void whoami (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add("username", AuthenticationHelper.GetTicket (context).Username);
            e.Args.Add("role", AuthenticationHelper.GetTicket (context).Role);
            e.Args.Add("default", AuthenticationHelper.GetTicket (context).IsDefault);
        }

        /// <summary>
        ///     Returns the settings for the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "user-settings")]
        public static void user_settings (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.GetSettings (context, e.Args);
            }
        }

        /// <summary>
        ///     Changes the settings for the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "change-user-settings")]
        public static void change_user_settings (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.ChangeSettings (context, e.Args);
            }
        }

        /// <summary>
        ///     Changes the password for currently logged in user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "change-password")]
        public static void change_password (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.ChangePassword (context, e.Args);
            }
        }

        /// <summary>
        ///     Deletes the currently logged in user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "delete-my-user")]
        public static void delete_my_user (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.DeleteMyUser (context, e.Args);
            }
        }
    }
}