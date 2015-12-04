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
///     Main namespace for all things related to threading in Phosphorus Five
/// </summary>
namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [fork] keyword
    /// </summary>
    public static class Fork
    {
        /*
         * Internal helper class for starting and managing threads
         */
        internal class ThreadStart
        {
            // Actual thread wrapped by this instance
            private Thread _thread;

            /*
             * CTOR, creates a new thread, and stores context and lambda object
             */
            public ThreadStart (ApplicationContext context, Node lambda)
            {
                Context = context;
                Lambda = lambda;
                _thread = new Thread (Execute);
            }

            /*
             * Wrapper around context
             */
            public ApplicationContext Context {
                get;
                private set;
            }

            /*
             * Which piece of lambda to actually evaluate
             */
            public Node Lambda {
                get;
                private set;
            }

            /*
             * Evaluates its lambda object on the thread associated with this instance
             */
            public void Start ()
            {
                _thread.Start ();
            }

            /*
             * Waits for thread to finish, or x milliseconds to pass, whichever occurs first, before
             * allowing execution to run through
             */
            public void Join (int milliseconds = -1)
            {
                if (milliseconds == -1)
                    _thread.Join ();
                else
                    _thread.Join (milliseconds);
            }

            /*
             * Entrance method for Thread to start executing once initialized. Simply evaluates its lambda object
             */
            private void Execute ()
            {
                Context.RaiseLambda ("eval-mutable", Lambda);
            }
        }

        /// <summary>
        ///     Forks a new thread of execution.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "fork", Protection = EventProtection.LambdaClosed)]
        private static void fork (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each lambda object in fork statement
            foreach (var idxThread in GetForkObjects (context, e.Args)) {

                // Starts a new thread, notice CLONE!
                new ThreadStart (context, idxThread.Clone ()).Start ();
            }
        }

        /*
         * Helper to retrieve each lambda object in a fork statement
         */
        internal static IEnumerable<Node> GetForkObjects (ApplicationContext context, Node args)
        {
            if (args.Value == null) {

                // Executing current scope only
                yield return args;
            } else {

                // Iterating each result
                foreach (var idxResult in XUtil.Iterate<Node> (context, args)) {

                    // Returning each iterated result
                    yield return idxResult;
                }
            }
        }
    }
}
