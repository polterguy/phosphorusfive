/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.threading
{
    /// <summary>
    ///     wraps [lambda.fork] keyword
    /// </summary>
    public static class LambdaFork
    {
        /// <summary>
        ///     forks a new thread, where it executes the given scope
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda.fork")]
        private static void lambda_fork (ApplicationContext context, ActiveEventArgs e)
        {
            // cloning arguments to pass into thread
            IEnumerable<Node> exe;
            IEnumerable<Node> args = null;
            if (e.Args.Value == null) {
                // executing children nodes
                exe = new[] {e.Args.Clone ()};
            } else {
                // executing expression, or nodes, somehow
                var exeList = XUtil.Iterate<Node> (e.Args, context).Select (idxExe => idxExe.Clone ()).ToList ();
                exe = exeList;
                args = e.Args.Clone ().Children;
            }

            Node wait = null;
            if (e.Args.Parent != null && e.Args.Parent.Name == "wait") {
                // waiting for all threads to finish, hence we'll need a reference to our [wait] node
                // inside our thread
                wait = e.Args.Parent;
            }

            // creating new thread
            var thread = new Thread (Execute);
            thread.Start (new object[] {exe, args, context, wait});

            // checking to see if we should wait
            if (wait != null) {
                // we have a [wait] statement as parent
                if (wait ["__threads"] == null) {
                    wait.Add ("__threads");
                }
                wait ["__threads"].Add (new Node (string.Empty, thread));
            }
        }

        /*
         * implementation for our forked thread
         */

        private static void Execute (object threadArgs)
        {
            var enumerables = threadArgs as object[];
            // ReSharper disable once PossibleNullReferenceException
            var exe = enumerables [0] as IEnumerable<Node>;
            var args = enumerables [1] as IEnumerable<Node>;
            var context = enumerables [2] as ApplicationContext;
            var wait = enumerables [3] as Node;

            // ReSharper disable once PossibleNullReferenceException
            foreach (var idxExe in exe) {
                if (args != null) {
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var idxArg in args) {
                        idxExe.Add (idxArg.Clone ());
                    }
                }
                if (wait != null) {
                    // passing in as reference node
                    idxExe.Add ("_wait", wait);
                }
                if (context != null) context.Raise ("lambda", idxExe);
            }
        }
    }
}