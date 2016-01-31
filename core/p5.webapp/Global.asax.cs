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
             * Loads up all plugins assemblies, raises the [p5.core.application-start] Active Event, and
             * executes the "application-startup-file" declared in web.config, if any
             */
            protected void Application_Start (Object sender, EventArgs e)
            {
                // Storing application base path for later usage, making sure we normalize path across operating systems
                _applicationBasePath = Server.MapPath ("~");
                _applicationBasePath.Replace("\\", "/");
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

            #region [ -- Common helper Active Event sinks to retrieve global settings -- ]

            /// <summary>
            ///     Returns the TimeSpan for how old a Web Service invocation can be before it is considered to be "out of date"
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-web-service-invocation-fresh-time", Protection = EventProtection.LambdaClosed)]
            public static void p5_security_get_web_service_invocation_fresh_time (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = Utilities.Convert<TimeSpan> (context, configuration.WebServiceInvocationsFreshTime);
            }

            /// <summary>
            ///     Returns the name of the server PGP key to use for encryption operations
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-marvin-pgp-key", Protection = EventProtection.NativeClosed)]
            private static void p5_security_get_marvin_pgp_key (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.MarvinPgpKey;
            }

            /// <summary>
            ///     Returns the password for the server PGP key to use for encryption operations
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-marvin-pgp-key-password", Protection = EventProtection.NativeClosed)]
            private static void p5_security_get_marvin_pgp_key_password (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.MarvinPgpKeyPassword;
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

            /// <summary>
            ///     Returns the default SMTP server URL
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-smtp-server", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_smtp_server (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.SmtpServer;
            }

            /// <summary>
            ///     Returns the default SMTP server port
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-smtp-port", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_smtp_port (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.SmtpPort;
            }

            /// <summary>
            ///     Returns the default SMTP use SSL property
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-smtp-use-ssl", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_smtp_use_ssl (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.SmtpUseSsl;
            }

            /// <summary>
            ///     Returns the default SMTP username
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-smtp-username", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_smtp_username (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.SmtpUsername;
            }

            /// <summary>
            ///     Returns the default SMTP username
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-smtp-password", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_smtp_password (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.SmtpPassword;
            }

            /// <summary>
            ///     Returns the default POP3 server URL
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-pop3-server", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_pop3_server (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.Pop3Server;
            }

            /// <summary>
            ///     Returns the default POP3 server port
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-pop3-port", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_pop3_port (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.Pop3Port;
            }

            /// <summary>
            ///     Returns the default POP3 use SSL property
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-pop3-use-ssl", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_pop3_use_ssl (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.Pop3UseSsl;
            }

            /// <summary>
            ///     Returns the default POP3 username
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-pop3-username", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_pop3_username (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.Pop3Username;
            }

            /// <summary>
            ///     Returns the default POP3 username
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.mail.get-pop3-password", Protection = EventProtection.NativeClosed)]
            private static void p5_mail_get_pop3_password (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                e.Args.Value = configuration.Pop3Password;
            }

            /// <summary>
            ///     Returns the password salt for the server to use when storing passwords in "auth" file
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-password-salt", Protection = EventProtection.NativeClosed)]
            private static void p5_security_get_password_salt (ApplicationContext context, ActiveEventArgs e)
            {
                var configuration = ConfigurationManager.GetSection ("phosphorus") as PhosphorusConfiguration;
                string dynamicSalt = context.RaiseNative("p5.security.get-dynamic-password-salt").Get(context, "");
                e.Args.Value = configuration.PasswordSalt + dynamicSalt;
            }

            /// <summary>
            ///     Returns a pseudo random string, generated from client IP, browser string, etc
            /// </summary>
            /// <param name="context">Application Context</param>
            /// <param name="e">Parameters passed into Active Event</param>
            [ActiveEvent (Name = "p5.security.get-pseudo-random-seed", Protection = EventProtection.NativeOpen)]
            private static void p5_security_get_pseudo_random_seed (ApplicationContext context, ActiveEventArgs e)
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                    return;
                string retVal = HttpContext.Current.Request.RawUrl.ToString();
                retVal += HttpContext.Current.Request.ContentLength.ToString ();
                retVal += HttpContext.Current.Request.Headers.ToString();
                retVal += HttpContext.Current.Request.Params.ToString();
                retVal += HttpContext.Current.Request.PhysicalApplicationPath;
                retVal += HttpContext.Current.Request.Browser.ToString();
                retVal += HttpContext.Current.Session.SessionID;
                foreach (var idxCookie in HttpContext.Current.Request.Cookies.AllKeys) {
                    retVal += HttpContext.Current.Request.Cookies[idxCookie].Name + HttpContext.Current.Request.Cookies[idxCookie].Value;
                }
                foreach (string idxSession in HttpContext.Current.Session.Keys) {
                    retVal += HttpContext.Current.Session[idxSession].ToString();
                }
                e.Args.Value = e.Args.Get<string> (context, "") + context.RaiseNative ("sha256-hash", new Node ("", retVal)).Value;
            }

            #endregion

            #region [ -- Private helper methods -- ]

            /*
             * Executes a Hyperlisp file
             */
            private static void ExecuteHyperlispFile (ApplicationContext context, string filePath)
            {
                // Loading file, converting to Lambda, for then to evaluate as p5 lambda
                context.RaiseNative ("eval", context.RaiseNative ("load-file", new Node("", filePath)) [0]);
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
