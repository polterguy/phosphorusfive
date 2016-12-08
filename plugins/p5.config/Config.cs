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

using System.Configuration;
using p5.exp;
using p5.core;

namespace p5.events
{
    /// <summary>
    ///     Active Events for retrieving configuration settings.
    /// </summary>
    public static class Config
    {
        /// <summary>
        ///     Retrieves an app.config/web.config setting.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-config-setting")]
        [ActiveEvent (Name = ".get-config-setting")]
        public static void get_config_setting (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Get (context, e.Args, ix => ConfigurationManager.AppSettings[ix]);
        }

        /// <summary>
        ///     Lists all configuration settings for application, optionally matching specified filter.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-config-settings")]
        [ActiveEvent (Name = ".list-config-settings")]
        public static void list_config_settings (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.List (context, e.Args, ConfigurationManager.AppSettings.AllKeys);
        }
    }
}
