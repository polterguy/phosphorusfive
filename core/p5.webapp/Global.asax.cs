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

/// <summary>
///     Main namespace for the Phosphorus Five core functionality.
/// </summary>
namespace p5
{
    namespace webapp
    {
        /// <summary>
        ///     The HttpApplication object for your web application.
        /// </summary>
        public class Global : HttpApplication
        {
            private static string _applicationBasePath;

            /*
             * loads up all plugins assemblies, raises the [p5.core.application-start] Active Event, and
             * executes the "application-startup-file" declared in we.config, if any
             */
            protected void Application_Start (Object sender, EventArgs e)
            {
                // storing application base path for later usage
                _applicationBasePath = Server.MapPath ("~");

                // adding up executing (this) assembly as Active Event handler
                Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());

                // adding all Active Event handler assemblies from web.config
                var configuration = ConfigurationManager.GetSection ("activeEventAssemblies") as ActiveEventAssemblies;
                if (configuration == null)
                    throw new ConfigurationErrorsException ("No activeEventAssemblies section found in web.config");

                foreach (ActiveEventAssembly idxAssembly in configuration.Assemblies) {

                    Loader.Instance.LoadAssembly (Server.MapPath (configuration.PluginDirectory), idxAssembly.Assembly);
                }

                // then raising the application start active event
                var context = Loader.Instance.CreateApplicationContext ();
                context.Raise ("p5.core.application-start");

                // for then to execute our "startup file", if there exists any
                if (!string.IsNullOrEmpty (ConfigurationManager.AppSettings ["application-startup-file"])) {

                    // there is an application-startup-file declared in app.config file, executing it as a p5.lambda file
                    var appStartFilePath = ConfigurationManager.AppSettings ["application-startup-file"];
                    ExecuteLambdaFile (context, appStartFilePath);
                }
            }

            /*
             * executes a lambda file
             */
            private static void ExecuteLambdaFile (ApplicationContext context, string filePath)
            {
                // loading file
                var loadFileNode = new Node (string.Empty, filePath);
                context.Raise ("load-file", loadFileNode);

                // raising file as p5.lambda object
                context.Raise ("eval", loadFileNode [0]);
            }

            /*
             * handled to create support for "beautiful URLs", to rewrite path, to support virtual pages, through p5.lambda
             */
            protected void Application_BeginRequest (object sender, EventArgs e)
            {
                // rewriting path such that "x.com/somefolder/somefile" becomes "x.com?file=somefolder/somefile"
                var localPath = HttpContext.Current.Request.Url.LocalPath;

                // checking to see if this is a [p5.page] request, and if so, we rewrite the path to "Default.aspx"
                // and store the original URL in the HttpContext.Item collection for later references
                /// \todo Support paths with "." in them, since now we don't support folders and paths with "." within their names
                if (localPath.ToLower ().Trim ('/') == "default.aspx" || !localPath.Contains (".")) {

                    // if file requested is Default.aspx, we change it to simply "?file=/"
                    if (localPath == "/Default.aspx")
                        localPath = "/";

                    // storing original path
                    HttpContext.Current.Items ["__p5_original_url"] = localPath;

                    // rewriting path
                    HttpContext.Current.RewritePath ("~/Default.aspx");
                }
            }

            /// <summary>
            ///     Returns the Application base folder.
            /// </summary>
            /// <param name="context">Application context Active Event is raised within</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.core.application-folder")]
            private static void p5_core_application_folder (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value = _applicationBasePath;
            }
        }
    }
}
