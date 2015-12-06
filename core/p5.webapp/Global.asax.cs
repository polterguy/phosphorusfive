/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using p5.core;
using p5.webapp.code;
using p5.webapp.code.configuration;

/// <summary>
///     Main namespace for Phosphorus Five
/// </summary>
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
             * Loads up all plugins assemblies, raises the [p5.core.application-start] Active Event, and
             * executes the "application-startup-file" declared in web.config, if any
             */
            protected void Application_Start (Object sender, EventArgs e)
            {
                // Storing application base path for later usage
                _applicationBasePath = Server.MapPath ("~");

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

                // Creating our App Context, notice we do NOT raise [p5.core.initialize-application-context]
                var context = Loader.Instance.CreateApplicationContext (new ApplicationContext.ContextTicket("root", "root", false));

                // Raising the application start Active Event, making sure we do it with a "root" Context Ticket
                context.RaiseNative ("p5.core.application-start");

                // Execute our "startup file", if there is one defined
                if (!string.IsNullOrEmpty (ConfigurationManager.AppSettings ["application-startup-file"])) {

                    // There is an application-startup-file declared in web.config, executing it as a Hyperlisp file
                    var appStartFilePath = ConfigurationManager.AppSettings ["application-startup-file"];
                    ExecuteHyperlispFile (context, appStartFilePath);
                }
            }

            /*
             * Executes a Hyperlisp file
             */
            private static void ExecuteHyperlispFile (ApplicationContext context, string filePath)
            {
                // Loading file, converting to Lambda, for then to evaluate as p5.lambda
                context.RaiseNative ("eval", context.RaiseNative ("load-file", new Node("", filePath)) [0]);
            }

            /*
             * Rewrites path to support "URL rewriting"
             */
            protected void Application_BeginRequest (object sender, EventArgs e)
            {
                // Rewriting path such that "x.com/somefolder/somefile" becomes "x.com?file=somefolder/somefile"
                var localPath = HttpContext.Current.Request.Url.LocalPath;

                // Checking to see if this is a [p5.page] request, and if so, we rewrite the path to "Default.aspx"
                // and store the original URL in the HttpContext.Item collection for later references
                if (localPath.ToLower ().Trim ('/') == "default.aspx" || !localPath.Contains (".")) {

                    // If file requested is Default.aspx, we change it to simply "?file=/"
                    if (localPath == "/Default.aspx")
                        localPath = "/";

                    // Storing original path
                    HttpContext.Current.Items ["_p5_original_url"] = localPath;

                    // Rewriting path
                    HttpContext.Current.RewritePath ("~/Default.aspx");
                }
            }

            /// <summary>
            ///     Returns the Application base folder
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.core.application-folder", Protection = EventProtection.NativeClosed)]
            private static void p5_core_application_folder (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value = _applicationBasePath;
            }

            /// <summary>
            ///     Returns the "auth" file for application
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-auth-file", Protection = EventProtection.NativeClosed)]
            private static void p5_security_get_auth_file (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.AuthFile;
            }

            /// <summary>
            ///     Returns the number of days before persistent credential cookie expires
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-credential-cookie-days", Protection = EventProtection.NativeClosed)]
            private static void p5_security_get_credential_cookie_days (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.PersistCredentialCookieDays;
            }

            /// <summary>
            ///     Returns the default role used for the ApplicationContext Ticket
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-default-context-role", Protection = EventProtection.NativeClosed)]
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
            [ActiveEvent (Name = "p5.security.get-default-context-username", Protection = EventProtection.NativeClosed)]
            private static void p5_security_get_default_context_username (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.DefaultContextUsername;
            }

            /// <summary>
            ///     Returns the number of "cool off" seconds that must pass between unsuccessful login attempts
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-login-cooloff-seconds", Protection = EventProtection.NativeClosed)]
            private static void p5_security_get_login_cooloff_seconds (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.LoginCoolOffSeconds;
            }
        }
    }
}
