/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
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
