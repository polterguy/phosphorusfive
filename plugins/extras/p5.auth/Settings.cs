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
    ///     Class wrapping user settings related Active Events.
    /// </summary>
    internal static class Settings
    {
        /// <summary>
        ///     Returns the settings for the currently logged in user.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.my-settings.get")]
        public static void p5_auth_my_settings_get (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.GetSettings (context, e.Args);
            }
        }

        /// <summary>
        ///     Updates the settings for the currently logged in user.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.my-settings.set")]
        public static void p5_auth_my_settings_set (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.ChangeSettings (context, e.Args);
            }
        }
    }
}