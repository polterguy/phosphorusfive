
/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using p5.core;

namespace p5.unittests
{
    /// <summary>
    /// base class for unit tests, contains helper methods for common unit tests operations
    /// </summary>
    public abstract class TestBase
    {
        protected ApplicationContext Context;
        protected static string _role = "root";
        protected static string _username = "root";
        protected const string _auth = "auth";

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.unittests.TestBase"/> class. pass
        /// in assemblies you wish to load as Active Event handlers
        /// </summary>
        /// <param name="assemblies">Assemblies.</param>
        protected TestBase (params string[] assemblies)
        {
            foreach (var idx in assemblies) {
                Loader.Instance.LoadAssembly (idx);
            }
            Loader.Instance.LoadAssembly (GetType ());
            Context = Loader.Instance.CreateApplicationContext ();
        }

        /// <summary>
        /// executes the given Hyperlisp, and returns resulting node
        /// </summary>
        /// <returns>p5.lambda result nodes</returns>
        /// <param name="hyperlisp">Hyperlisp you wish to execute</param>
        /// <param name="lambdaActiveEvent">what type of lambda event to raise</param>
        protected Node ExecuteLambda (string hyperlisp, string lambdaActiveEvent = "eval")
        {
            var node = new Node { Value = hyperlisp };
            Context.RaiseNative ("lisp2lambda", node);
            node.Value = null;
            return Context.RaiseNative (lambdaActiveEvent, node);
        }

        /// <summary>
        /// creates a p5.lambda Node from the given Hyperlisp
        /// </summary>
        /// <returns>p5.lambda result nodes</returns>
        /// <param name="hyperlisp">Hyperlisp you wish to execute</param>
        protected Node CreateNode (string hyperlisp)
        {
            var node = new Node { Value = hyperlisp };
            Context.RaiseNative ("lisp2lambda", node);
            return node;
        }

        /// <summary>
        /// retrieves the base path to where your unit test dll is on disc
        /// </summary>
        /// <returns>path</returns>
        protected static string GetBasePath ()
        {
            string retVal = Assembly.GetExecutingAssembly ().Location.Replace ("\\", "/");
            retVal = retVal.Substring (0, retVal.LastIndexOf("/", StringComparison.Ordinal) + 1);
            return retVal;
        }

        /*
         * helper Active Events necessary as helper for some of our plugins
         */
        [ActiveEvent (Name = "p5.core.application-folder", Protection = EventProtection.Native)]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = GetBasePath ();
        }

        [ActiveEvent (Name = "_p5.security.get-auth-file", Protection = EventProtection.Native)]
        private static void _p5_security_get_auth_file (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = _auth;
        }

        [ActiveEvent (Name = "_p5.security.get-credential-cookie-days", Protection = EventProtection.Native)]
        private static void _p5_security_get_credential_cookie_days (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = 90;
        }

        /// <summary>
        ///     Returns the default role used for the ApplicationContext, unless a user is explicitly logged in
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "_p5.security.get-default-context-role", Protection = EventProtection.Native)]
        private static void _p5_security_get_default_context_role (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = _role;
        }

        /// <summary>
        ///     Returns the default username used for the ApplicationContext, unless a user is explicitly logged in
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "_p5.security.get-default-context-username", Protection = EventProtection.Native)]
        private static void _p5_security_get_default_context_username (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = _username;
        }

        /// <summary>
        ///     Returns the number of seconds that must pass from an unsuccessful login attempt to client is allowed to try again
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "_p5.security.get-login-cooloff-seconds", Protection = EventProtection.Native)]
        private static void _p5_security_get_login_cooloff_seconds (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = 90;
        }
    }
}
