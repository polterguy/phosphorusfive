/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for all things related to threading in Phosphorus Five.
/// 
///     Namespace wraps all classes necessary to create and manipulate threads within Phosphorus Five.
/// </summary>
namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [fork] keyword.
    /// 
    ///     The [fork] keyword, allows you to create new threads of execution within the system.
    /// </summary>
    public static class Fork
    {
        /// <summary>
        ///     Forks a new thread of execution.
        /// 
        ///     The [fork] keyword is useful when you have lenghty operations, that does not necessarily require a lot of CPU, such
        ///     as when downloading multiple files over HTTP, where you do not want to wait for the file(s) to be downloaded, before continuing
        ///     the execution of your main thread.
        /// 
        ///     [fork] works roughly the same way as [lambda.copy], since it executes the thread on a copied instance of its own
        ///     execution tree, except of course that [fork] creates a new thread, where it executes its code.
        /// 
        ///     If [fork] has a <see cref="phosphorus.threading.Wait.lambda_wait">[wait]</see> node as its parent, then the [wait] node will 
        ///     be passed in by reference, allowing your thread to return values to the main thread during its execution. If you access any shared 
        ///     objects inside of your thread, you will want to make sure you use a [lock] statement as you do, since otherwise you might up having 
        ///     race conditions towards your shared object.
        /// 
        ///     Example;
        /// 
        ///     <pre>fork
        ///   p5.web.get:"http://google.com"
        ///   append:@/+/0/?node
        ///     source:@/./-/*?node
        ///   insert-data
        ///     web-site-content
        /// fork
        ///   p5.web.get:"http://digg.com"
        ///   append:@/+/0/?node
        ///     source:@/./-/*?node
        ///   insert-data
        ///     web-site-content</pre>
        /// 
        ///     The above piece of code, forks two new threads, downloading the HTML from google.com and digg.com. If you execute the above code, you will
        ///     notice that your p5.lambda code returns immediately, without waiting for the pages to be downloaded. This is because the above threads are
        ///     created as <em>"fire-and-forget"</em> threads, where the caller does not care about waiting for the threads to finish their work.
        /// 
        ///     The observant reader might also notice that we do not use a [lock] statement, even though we're accessing our database, which of course is
        ///     a <em>"shared object"</em>. This is because access to the database is already thread-safe, and hence does not need a [lock] statement 
        ///     to avoid race-conditions.
        /// 
        ///     See the <see cref="phosphorus.threading.Wait.lambda_wait">[wait]</see> Active Event for an example of how to create multiple threads,
        ///     and a deeper explanation of how threading works.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "fork")]
        private static void fork (ApplicationContext context, ActiveEventArgs e)
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
            var exe = enumerables [0] as IEnumerable<Node>;
            var args = enumerables [1] as IEnumerable<Node>;
            var context = enumerables [2] as ApplicationContext;
            var wait = enumerables [3] as Node;

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
                if (context != null) context.Raise ("lambda", idxExe);
            }
        }
    }
}
