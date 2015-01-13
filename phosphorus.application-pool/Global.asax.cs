/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.applicationpool
{
    using System;
    using System.Web;
    using System.Reflection;
    using System.Configuration;
    using System.Collections;
    using System.ComponentModel;
    using System.Web.SessionState;
    using phosphorus.core;

    public class Global : HttpApplication
    {
        private static string _applicationBasePath;

        /*
         * loads up all plugins assemblies and raises the [pf.core.application-start] Active Event
         */
        protected void Application_Start(Object sender, EventArgs e)
        {
            // sotring application base path for later usage
            _applicationBasePath = Server.MapPath ("~");

            // adding up executing (this) assembly as Active Event handler
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());

            // adding all Active Event handler assemblies from web.config
            var configuration = ConfigurationManager.GetSection ("activeEventAssemblies") as ActiveEventAssemblies;
            foreach (ActiveEventAssembly idxAssembly in configuration.Assemblies) {
                Loader.Instance.LoadAssembly (Server.MapPath (configuration.PluginDirectory), idxAssembly.Assembly);
            }

            // then raising the application start active event
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.Raise ("pf.core.application-start", null);
        }

        /*
         * handled to create support for "beautiful URLs", to rewrite path, to support non-existing pages, through pf.lambda
         */
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // rewriting path such that "x.com/somefolder/somefile" becomes "x.com?file=somefolder/somefile"
            string localPath = HttpContext.Current.Request.Url.LocalPath;

            // checking to see if this is a [pf.page] request, and if so, we rewrite the path to "Default.aspx"
            // and store the original URL in the HttpContext.Item collection for later references
            // TODO: Support paths with "." in them, since now we don't support folders and paths with "." within their names
            if (localPath.ToLower ().Trim ('/') == "default.aspx" || !localPath.Contains (".")) {

                // if file requested is Default.aspx, we change it to simply "?file=default"
                if (localPath == "/Default.aspx")
                    localPath = "/";

                // storing original path
                HttpContext.Current.Items ["__pf_original_url"] = localPath;

                // rewriting path
                HttpContext.Current.RewritePath ("~/Default.aspx");
            }
        }
        
        /// <summary>
        /// returns the application base path as value of given args node
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.get-application-root-folder")]
        private static void pf_get_application_root_folder (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = _applicationBasePath;
        }
    }
}
