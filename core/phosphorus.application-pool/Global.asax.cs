/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using phosphorus.core;
using phosphorus.five.applicationpool.code;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace phosphorus.five.applicationpool
{
    public class Global : HttpApplication
    {
        private static string _applicationBasePath;

        /*
         * loads up all plugins assemblies, raises the [pf.core.application-start] Active Event, and
         * executes the "application-startup-file" declared in we.config, if any
         */
        protected void Application_Start (Object sender, EventArgs e)
        {
            // sotring application base path for later usage
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
            context.Raise ("pf.core.application-start");

            // for then to execute our "startup file", if there exists any
            if (!string.IsNullOrEmpty (ConfigurationManager.AppSettings ["application-startup-file"])) {
                // there is an application-startup-file declared in app.config file, executing it as a pf.lambda file
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
            context.Raise ("pf.file.load", loadFileNode);

            // raising file as pf.lambda object
            context.Raise ("lambda", Utilities.Convert<Node> (loadFileNode [0].Value, context));
        }

        /*
         * handled to create support for "beautiful URLs", to rewrite path, to support virtual pages, through pf.lambda
         */
        protected void Application_BeginRequest (object sender, EventArgs e)
        {
            // rewriting path such that "x.com/somefolder/somefile" becomes "x.com?file=somefolder/somefile"
            var localPath = HttpContext.Current.Request.Url.LocalPath;

            // checking to see if this is a [pf.page] request, and if so, we rewrite the path to "Default.aspx"
            // and store the original URL in the HttpContext.Item collection for later references
            // TODO: Support paths with "." in them, since now we don't support folders and paths with "." within their names
            if (localPath.ToLower ().Trim ('/') == "default.aspx" || !localPath.Contains (".")) {
                // if file requested is Default.aspx, we change it to simply "?file=/"
                if (localPath == "/Default.aspx")
                    localPath = "/";

                // storing original path
                HttpContext.Current.Items ["__pf_original_url"] = localPath;

                // rewriting path
                HttpContext.Current.RewritePath ("~/Default.aspx");
            }
        }

        /// <summary>
        ///     Returns the application base path as value of given args node. Necessary to make for instance
        ///     our [pf.file.xxx] namespace to work correctly
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.core.application-folder")]
        private static void pf_core_application_folder (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = _applicationBasePath;
        }
    }
}