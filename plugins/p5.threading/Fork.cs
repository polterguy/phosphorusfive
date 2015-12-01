/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for all things related to threading in Phosphorus Five.
/// </summary>
namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [fork] keyword.
    /// </summary>
    public static class Fork
    {
        /*
         * internal helper class for starting and managing threads
         */
        internal class ThreadStart
        {
            internal ThreadStart (ApplicationContext context, Node lambda)
            {
                Context = context;
                Lambda = lambda;
                Thread = new Thread (Execute);
            }

            public ApplicationContext Context {
                get;
                private set;
            }

            public Node Lambda {
                get;
                private set;
            }

            private Thread Thread {
                get;
                set;
            }

            public void Start ()
            {
                Thread.Start ();
            }

            public void Join (int milliseconds = -1)
            {
                if (milliseconds == -1)
                    Thread.Join ();
                else
                    Thread.Join (milliseconds);
            }

            private void Execute ()
            {
                Context.Raise ("eval-mutable", Lambda);
            }
        }

        /// <summary>
        ///     Forks a new thread of execution.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "fork")]
        private static void fork (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through each lambda object in fork statement
            foreach (var idxThread in GetForkObjects (context, e.Args)) {

                // starting a new thread
                new ThreadStart (context, idxThread).Start ();
            }
        }

        /*
         * helper to retrieve each lambda object in a fork statement
         */
        internal static IEnumerable<Node> GetForkObjects (ApplicationContext context, Node args)
        {
            if (args.Value == null) {

                // executing current scope only
                yield return args.Clone ();
            } else {

                // iterating each result
                foreach (var idxResult in XUtil.Iterate<Node> (context, args)) {

                    // returning each iterated result
                    yield return idxResult.Clone ();
                }
            }
        }
    }
}
