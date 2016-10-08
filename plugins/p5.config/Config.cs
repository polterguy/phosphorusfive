/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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

using System.Configuration;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.events
{
    /// <summary>
    ///     Active Events for retrieving configuration settings
    /// </summary>
    public static class Config
    {
        /// <summary>
        ///     Retrieves an app.config/web.config setting
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-config-setting")]
        [ActiveEvent (Name = ".get-config-setting")]
        public static void get_config_setting (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.GetCollection (context, e.Args, delegate (string key) {
                return ConfigurationManager.AppSettings[key];
            }, e.Name.StartsWith ("."));
        }

        /// <summary>
        ///     Retrieves an app.config/web.config setting
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-config-settings")]
        [ActiveEvent (Name = ".list-config-settings")]
        public static void list_config_settings (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.ListCollection (context, e.Args, ConfigurationManager.AppSettings.AllKeys, e.Name.StartsWith ("."));
        }
    }
}
