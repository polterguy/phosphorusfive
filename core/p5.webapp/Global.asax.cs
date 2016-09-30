/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using System.Reflection;
using System.Configuration;
using p5.core;
using p5.webapp.code.configuration;

/// <summary>
///     Main namespace for Phosphorus Five
/// </summary>
namespace p5
{
    /// <summary>
    ///     Main namespace for your Phosphorus Five web app
    /// </summary>
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
                // Storing application base path for later usage, making sure we normalize path across operating systems
                _applicationBasePath = Server.MapPath ("~");
                _applicationBasePath = _applicationBasePath.Replace("\\", "/");
                if (_applicationBasePath.EndsWith("/"))
                    _applicationBasePath = _applicationBasePath.Substring(0, _applicationBasePath.Length - 1);

                // Adding up executing (this) assembly as Active Event handler
                Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());

                // Adding all Active Event handler assemblies from web.config
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                if (configuration == null)
                    throw new ConfigurationErrorsException ("No Phosphorus configuration section found in web.config");

                // Looping through all assemblies from web.config
                foreach (ActiveEventAssembly idxAssembly in configuration.Assemblies) {

                    // Loading up and registering currently iterated assembly as Active Event handler assembly
                    Loader.Instance.LoadAssembly (Server.MapPath (configuration.PluginDirectory), idxAssembly.Assembly);
                }

                // Creating our App Context
                // Notice, the Application_Start's ApplicationContext is evaluated from within the context of "root"
                var context = Loader.Instance.CreateApplicationContext (new ApplicationContext.ContextTicket("root", "root", false));

                // Raising the application start Active Event, making sure we do it with a "root" Context Ticket
                context.Raise (".p5.core.application-start");

                // Execute our "startup file", if there is one defined
                var appStartupFiles = context.Raise (
                    ".get-config-setting",
                    new Node ("", "p5.webapp.application-startup-file")) [0].Get<string>(context);
                if (!string.IsNullOrEmpty (appStartupFiles)) {

                    // There is an application-startup-file declared in web.config, executing it as a Hyperlisp file
                    ExecuteHyperlispFile (context, appStartupFiles);
                }
            }

            /*
             * Rewrites path to support "URL rewriting"
             */
            protected void Application_BeginRequest (object sender, EventArgs e)
            {
                // Rewriting path such that "x.com/somefolder/somefile" becomes "x.com?file=somefolder/somefile"
                // Notice, "ToLower"!
                var localPath = HttpContext.Current.Request.Url.LocalPath.ToLower ();

                // Checking to see if we should rewrite the URL/path to page
                if (localPath == "/default.aspx" || !localPath.Contains (".")) {

                    // Rewriting path (URL) of request
                    RewritePath(localPath);
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
            [ActiveEvent (Name = ".p5.security.get-auth-file")]
            private static void _p5_security_get_auth_file (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.AuthFile;
            }

            /// <summary>
            ///     Returns the default role used for the ApplicationContext Ticket
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-default-context-role")]
            private static void p5_security_get_default_context_role (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.DefaultContextRole;
            }

            /// <summary>
            ///     Returns the default username used for the ApplicationContext Ticket
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-default-context-username")]
            private static void p5_security_get_default_context_username (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.DefaultContextUsername;
            }

            #endregion

            #region [ -- Private helper methods -- ]

            /*
             * Executes a Hyperlisp file
             */
            private static void ExecuteHyperlispFile (ApplicationContext context, string filePath)
            {
                // Loading file, converting to Lambda, for then to evaluate as p5 lambda
                context.Raise ("eval", context.Raise ("load-file", new Node("", filePath)) [0]);
            }

            /*
             * Rewrites URL/path to web page request
             */
            private static void RewritePath (string url)
            {
                // If file requested is Default.aspx, we change it to simply "?file=/"
                if (url == "/default.aspx")
                    url = "/";

                // Storing original path
                HttpContext.Current.Items ["_p5_original_url"] = url;

                // Rewriting path
                HttpContext.Current.RewritePath ("~/Default.aspx");
            }

            #endregion
        }
    }
}
