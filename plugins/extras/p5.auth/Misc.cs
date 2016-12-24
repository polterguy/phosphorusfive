/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
    ///     Class wrapping miscellaneous authentication/authorization Active Events.
    /// </summary>
    static class Misc
    {
        /// <summary>
        ///     Returns the currently logged in user object.
        ///     Will not return the password, neither in hashed form, nor in plain text form.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "whoami")]
        [ActiveEvent (Name = "p5.auth.misc.whoami")]
        public static void p5_auth_misc_whoami (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add("username", AuthenticationHelper.GetTicket (context).Username);
            e.Args.Add("role", AuthenticationHelper.GetTicket (context).Role);
            e.Args.Add("default", AuthenticationHelper.GetTicket (context).IsDefault);
        }

        /// <summary>
        ///     Changes the password for currently logged in user.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.misc.change-my-password")]
        public static void p5_auth_misc_change_my_password (ApplicationContext context, ActiveEventArgs e)
        {
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.ChangePassword (context, e.Args);
            }
        }

        /// <summary>
        ///     Deletes the currently logged in user.
        ///     Notice, this action cannot be undone, since it possibly results in the deletion of a bunch of files in the system, among other things.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.misc.delete-my-user")]
        public static void p5_auth_misc_delete_my_user (ApplicationContext context, ActiveEventArgs e)
        {
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.DeleteMyUser (context, e.Args);
            }
        }
    }
}