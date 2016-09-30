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
        [ActiveEvent (Name = "_get-config-setting")]
        public static void get_config_setting (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Iterating through all settings to retrieve, adding setting to returned values
                foreach (var idxName in XUtil.Iterate<string> (context, e.Args)) {

                    // Making sure this is not a "protected setting", unless invoker was "native"
                    // Hint; Lambda cannot invoke Active Events that starts with an underscore (_)
                    if (!e.Name.StartsWith ("_") && idxName.StartsWith ("_"))
                        throw new LambdaException ("Caller tried to access protected config setting in [get-config-setting]", e.Args, context);

                    // Setting was not protected (did not start with an "_"), or caller was natively invoking this Active Event
                    e.Args.Add (idxName, ConfigurationManager.AppSettings [idxName]);
                }
            }
        }
    }
}
