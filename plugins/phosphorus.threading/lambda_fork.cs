
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Threading;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.threading
{
    /// <summary>
    /// wraps [lambda.fork] keyword
    /// </summary>
    public static class pfFork
    {
        /// <summary>
        /// forks a new thread, where it executes the given scope
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda.fork")]
        private static void lambda_fork (ApplicationContext context, ActiveEventArgs e)
        {
            // cloning arguments to pass into thread
            IEnumerable<Node> exe = null;
            IEnumerable<Node> args = null;
            if (e.Args.Value == null) {

                // executing children nodes
                exe = new Node [] { e.Args.Clone () };
            } else {

                // executing expression, or nodes, somehow
                List<Node> exeList = new List<Node> ();
                foreach (var idxExe in XUtil.Iterate<Node> (e.Args, context)) {
                    exeList.Add (idxExe.Clone ());
                }
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
            Thread thread = new Thread (new ParameterizedThreadStart (Execute));
            thread.Start (new object [] { exe, args, context, wait });

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
            object[] enumerables = threadArgs as object[];
            IEnumerable<Node> exe = enumerables [0] as IEnumerable<Node>;
            IEnumerable<Node> args = enumerables [1] as IEnumerable<Node>;
            ApplicationContext context = enumerables [2] as ApplicationContext;
            Node wait = enumerables [3] as Node;

            foreach (var idxExe in exe) {
                if (args != null) {
                    foreach (var idxArg in args) {
                        idxExe.Add (idxArg.Clone ());
                    }
                }
                if (wait != null) {

                    // passing in as reference node
                    idxExe.Add ("_wait", wait);
                }
                context.Raise ("lambda", idxExe);
            }
        }
    }
}
