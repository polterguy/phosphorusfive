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

using System;
using System.Web;
using System.Reflection;
using System.Configuration;
using p5.core;
using p5.webapp.code.configuration;
using System.Web.UI;

namespace p5
{
    namespace webapp
    {
        /// <summary>
        ///     The HttpApplication object for your web application
        /// </summary>
        public class Global : HttpApplication
        {
            private static string _applicationBasePath;

            /*
             * Loads up all plugins assemblies, raises the [.p5.core.application-start] Active Event, and
             * executes the "application-startup-file" declared in web.config, if any
             */
            protected void Application_Start (object sender, EventArgs e)
            {
                // Making sure we set the base path for app for later usage.
                GetBasePath ();

                // Loading our plugin assemblies.
                LoadPluginAssemblies ();

                // Creating our App Context
                // Notice, the Application_Start's ApplicationContext is evaluated from within the context of "root" to have extended rights
                // during startup.
                var context = Loader.Instance.CreateApplicationContext (new ContextTicket ("root", "root", false));

                // Raising the application start Active Event, making sure we do it with a "root" Context Ticket
                context.RaiseEvent (".p5.core.application-start");

                // Executing our startup files.
                ExecuteStartupFiles (context);
            }

            /*
             * Rewrites path to support virtual URLs
             */
            protected void Application_BeginRequest (object sender, EventArgs e)
            {
                // Rewriting path such that "x.com/somefolder/somefile" becomes "x.com?file=somefolder/somefile"
                // Notice, "ToLower"!
                var localPath = HttpContext.Current.Request.Url.LocalPath;

                // Checking to see if we should rewrite the URL/path to page
                if (localPath.ToLower () == "/default.aspx" || !localPath.Contains (".")) {

                    // Rewriting path (URL) of request
                    RewritePath (localPath);
                }
            }

            #region [ -- Common helper Active Event sinks to retrieve system's configuration settings -- ]

            /// <summary>
            ///     Returns the Application base folder
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = ".p5.core.application-folder")]
            private static void _p5_core_application_folder (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value = _applicationBasePath;
            }

            /// <summary>
            ///     Returns the "auth" file for application
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = ".p5.auth.get-auth-file")]
            private static void _p5_auth_get_auth_file (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.AuthFile;
            }

            /// <summary>
            ///     Returns the default role used for the ApplicationContext Ticket
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = ".p5.auth.get-default-context-role")]
            private static void _p5_auth_get_default_context_role (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.DefaultContextRole;
            }

            /// <summary>
            ///     Returns the default username used for the ApplicationContext Ticket
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = ".p5.auth.get-default-context-username")]
            private static void _p5_auth_get_default_context_username (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.DefaultContextUsername;
            }

            #endregion

            #region [ -- Private helper methods -- ]

            /*
             * Executes any startup Hyperlambda files, if any, according to web.config settings.
             */
            private static void ExecuteStartupFiles (ApplicationContext context)
            {
                // Execute our "startup file", if there is one defined
                var appStartupFiles = context.RaiseEvent (
                    ".p5.config.get",
                    new Node (".p5.config.get", ".p5.webapp.application-startup-file"))[0].Get<string> (context);
                if (!string.IsNullOrEmpty (appStartupFiles)) {

                    // There is an application-startup-file declared in web.config, executing it as a Hyperlambda file
                    ExecuteHyperlispFile (context, appStartupFiles);
                }
            }

            /*
             * Loads plugin assemblies according to web.config.
             */
            private void LoadPluginAssemblies ()
            {
                // Adding up executing (this) assembly as Active Event handler
                Loader.Instance.RegisterAssembly (Assembly.GetExecutingAssembly ());

                // Adding all Active Event handler assemblies from web.config
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                if (configuration == null)
                    throw new ConfigurationErrorsException ("No Phosphorus configuration section found in web.config");

                // Looping through all assemblies from web.config
                foreach (ActiveEventAssembly idxAssembly in configuration.Assemblies) {

                    // Loading up and registering currently iterated assembly as Active Event handler assembly
                    Loader.Instance.LoadAssembly (Server.MapPath (configuration.PluginDirectory), idxAssembly.Assembly);
                }
            }

            /*
             * Sets the base path for later usage.
             */
            private void GetBasePath ()
            {
                // Storing application base path for later usage, making sure we normalize path across operating systems
                _applicationBasePath = Server.MapPath ("~");
                _applicationBasePath = _applicationBasePath.Replace ("\\", "/");
                if (_applicationBasePath.EndsWith ("/"))
                    _applicationBasePath = _applicationBasePath.Substring (0, _applicationBasePath.Length - 1);
            }

            /*
             * Executes a Hyperlambda file
             */
            private static void ExecuteHyperlispFile (ApplicationContext context, string filePath)
            {
                // Loading file, converting to Lambda, for then to evaluate as p5 lambda
                context.RaiseEvent ("eval", context.RaiseEvent ("p5.io.file.load", new Node("", filePath)) [0]);
            }

            /*
             * Rewrites URL/path to web page request
             */
            private static void RewritePath (string url)
            {
                // If file requested is Default.aspx, we change it to simply "?file=/"
                if (url.ToLower () == "/default.aspx")
                    url = "/";

                // Storing original path
                HttpContext.Current.Items [".p5.webapp.original-url"] = url;

                // Rewriting path
                HttpContext.Current.RewritePath ("~/Default.aspx");
            }

            #endregion
        }
    }
}
