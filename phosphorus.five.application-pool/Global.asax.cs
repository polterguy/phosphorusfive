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

        protected void Application_Start(Object sender, EventArgs e)
        {
            var configuration = ConfigurationManager.GetSection ("activeEventAssemblies") as ActiveEventConfiguration;
            foreach (ActiveEventAssembly idxAssembly in configuration.Assemblies) {
                Loader.Instance.LoadAssembly (Server.MapPath (configuration.PluginDirectory), idxAssembly.Assembly);
            }

            // then adding up executing (this) assembly
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
        }

        protected void Session_End(Object sender, EventArgs e)
        {
        }

        protected void Application_End(Object sender, EventArgs e)
        {
        }
    }
}
