/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using System.Net;
using System.Web;
using System.Threading;
using System.Reflection;
using System.Net.Security;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using p5.exp;
using p5.core;
using p5.webapp.code.configuration;

namespace p5
{
    namespace webapp
    {
        /// <summary>
        ///     The HttpApplication object for your web application
        /// </summary>
        public class Global : HttpApplication
        {
            static string _applicationBasePath;
            ReaderWriterLockSlim _lock = new ReaderWriterLockSlim ();

            /*
             * Loads up all plugins assemblies, raises the [.p5.core.application-start] Active Event, and
             * executes the "application-startup-file" declared in web.config, if any
             */
            protected void Application_Start (object sender, EventArgs e)
            {
                /*
                 * To avoid a bug in Mono, we'll need to lock the website while we're initialising our application.
                 * 
                 * This is done in case there are multiple requests going towards our server as it is starting up,
                 * at which point the requests will fail, due to a "race condition" (or something).
                 */
                _lock.EnterWriteLock ();

                try {

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

                } finally {

                    // Allowing requests to be handled.
                    _lock.ExitWriteLock ();
                }

                // Validating that at least no SSL policy errors occurred during handshake.
                ServicePointManager.ServerCertificateValidationCallback += delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors errors) {

                        /*
                         * Since we have no ways of logically handling this, besides checking for SSL errors,
                         * we just that, to make sure we at least have some rudimentary support.
                         * 
                         * Notice, our HTTP REST module (p5.http) overrides this with its own per request
                         * based validations. So this is only relevant in fact for MimeKit while fetching
                         * public PGP keys during verification of cryptographic signatures. Which anyways
                         * should use the web of trust features afterwards.
                         */
                        return errors == SslPolicyErrors.None;
                    };

            }

            /*
             * Rewrites path to support virtual URLs
             */
            protected void Application_BeginRequest (object sender, EventArgs e)
            {
                /*
                 * Checking to see if we should rewrite the URL/path to page.
                 *
                 * Notice, a request for some "page" is defined as something not containing a "." in its filename.
                 * This stops our logic from rewriting paths to things such as CSS files, JavaScript files, etc.
                 *
                 * First we'll need to figure out our filename.
                 */
                var filename = HttpContext.Current.Request.RawUrl;

                // Stripping HTTP GET query parameters, if there are any.
                filename = filename.Contains ("?") ? filename.Split ('?') [0] : filename;

                // Finding our last "entity", which serves as our "filename".
                var entities = filename.Split (new char [] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                filename = entities.Length == 0 ? "" : entities [entities.Length - 1];

                /*
                 * Then we'll need to figure out if our filename does not contain a ".", at which point we need to rewrite its path.
                 */
                if (!filename.Contains (".")) {

                    /*
                     * This is a request for a rewritten URL, which we need to route to our Default.aspx page.
                     * 
                     * Making sure we don't handle any requests while application is being started.
                     */
                    _lock.EnterReadLock ();

                    // Making sure we're able to release our read lock, regardless of what happens during our request.
                    try {

                        // Rewriting path (URL) of request to our "single handler", which handles all of our page requests,
                        // and arguably everything in our app, except static resources though.
                        HttpContext.Current.RewritePath ("~/Default.aspx");

                    } finally {

                        // Cleaning up by releasing our read lock.
                        _lock.ExitReadLock ();
                    }
                }
            }

#region [ -- Common helper Active Event sinks to retrieve system's configuration settings -- ]

            /// <summary>
            ///     Returns the Application base folder
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = ".p5.core.application-folder")]
            static void _p5_core_application_folder (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value = _applicationBasePath;
            }

            /// <summary>
            ///     Returns the "auth" file for application
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = ".p5.auth.get-auth-file")]
            static void _p5_auth_get_auth_file (ApplicationContext context, ActiveEventArgs e)
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
            static void _p5_auth_get_default_context_role (ApplicationContext context, ActiveEventArgs e)
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
            static void _p5_auth_get_default_context_username (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.DefaultContextUsername;
            }

#endregion

#region [ -- Private helper methods -- ]

            /*
             * Executes any startup Hyperlambda files, if any, according to web.config settings.
             */
            static void ExecuteStartupFiles (ApplicationContext context)
            {
                // Execute our "startup file", if there is one defined
                var appStartupFiles = context.RaiseEvent (
                    ".p5.config.get",
                    new Node (".p5.config.get", ".p5.webapp.application-startup-file")) [0].Get<string> (context);
                if (!string.IsNullOrEmpty (appStartupFiles)) {

                    // There is an application-startup-file declared in web.config, executing it as a Hyperlambda file
                    ExecuteHyperlispFile (context, appStartupFiles);
                }
            }

            /*
             * Loads plugin assemblies according to web.config.
             */
            void LoadPluginAssemblies ()
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
            void GetBasePath ()
            {
                // Storing application base path for later usage, making sure we normalize path across operating systems
                _applicationBasePath = Server.MapPath ("~");
                _applicationBasePath = _applicationBasePath.Replace ("\\", "/");
                if (_applicationBasePath.EndsWithEx ("/"))
                    _applicationBasePath = _applicationBasePath.Substring (0, _applicationBasePath.Length - 1);
            }

            /*
             * Executes a Hyperlambda file
             */
            static void ExecuteHyperlispFile (ApplicationContext context, string filePath)
            {
                // Loading file, converting to Lambda, for then to evaluate as p5 lambda
                context.RaiseEvent ("eval", context.RaiseEvent ("p5.io.file.load", new Node ("", filePath)) [0]);
            }

#endregion
        }
    }
}
