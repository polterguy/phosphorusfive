
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
        protected Node ExecuteLambda (string hyperlisp, string lambdaActiveEvent = "lambda")
        {
            var node = new Node { Value = hyperlisp };
            Context.Raise ("lisp2lambda", node);
            node.Value = null;
            return Context.Raise (lambdaActiveEvent, node);
        }

        /// <summary>
        /// creates a p5.lambda Node from the given Hyperlisp
        /// </summary>
        /// <returns>p5.lambda result nodes</returns>
        /// <param name="hyperlisp">Hyperlisp you wish to execute</param>
        protected Node CreateNode (string hyperlisp)
        {
            var node = new Node { Value = hyperlisp };
            Context.Raise ("lisp2lambda", node);
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
         * helper Active Event necessary as helper for many of our plugins
         */
        [ActiveEvent (Name = "p5.core.application-folder")]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = GetBasePath ();
        }
    }
}
