
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    /// <summary>
    /// base class for unit tests, contains helper methods for common unit tests operations
    /// </summary>
    public class TestBase
    {
        protected ApplicationContext _context;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.unittests.TestBase"/> class. pass
        /// in assemblies you wish to load as Active Event handlers
        /// </summary>
        /// <param name="assemblies">Assemblies.</param>
        public TestBase (params string[] assemblies)
        {
            foreach (var idx in assemblies) {
                Loader.Instance.LoadAssembly (idx);
            }
            Loader.Instance.LoadAssembly (this.GetType ());
            _context = Loader.Instance.CreateApplicationContext ();
        }

        /// <summary>
        /// executes the given Hyperlisp, and returns resulting node
        /// </summary>
        /// <returns>pf.lambda result nodes</returns>
        /// <param name="hyperlisp">Hyperlisp you wish to execute</param>
        protected Node ExecuteLambda (string hyperlisp, string lambdaActiveEvent = "lambda")
        {
            Node node = new Node ();
            node.Value = hyperlisp;
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", node);
            return _context.Raise (lambdaActiveEvent, node);
        }

        /// <summary>
        /// retrieves the base path to where your unit test dll is on disc
        /// </summary>
        /// <returns>path</returns>
        protected static string GetBasePath ()
        {
            string retVal = Assembly.GetExecutingAssembly ().Location;
            retVal = retVal.Substring (0, retVal.LastIndexOf ("/") + 1);
            return retVal;
        }

        /*
         * helper Active Event necessary as helper for many of our plugins
         */
        [ActiveEvent (Name = "pf.core.application-folder")]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = GetBasePath ();
        }
    }
}
