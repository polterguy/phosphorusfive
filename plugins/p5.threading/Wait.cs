/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [wait] keyword
    /// </summary>
    public static class Wait
    {
        /// <summary>
        ///     Waits for all [fork] children to finish, or x milliseconds, before execution passes on
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "wait", Protection = EventProtection.LambdaClosed)]
        private static void lambda_wait (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Children.Count( ix => ix.Name != "fork" && ix.Name != "") != 0)
                throw new LambdaException ("[wait] cannot have any other types of children but [fork] statements", e.Args, context);

            // Keeping a reference to all threads created, such that we can join them afterwards
            var threads = new List<Fork.ThreadStart> ();

            // Looping through each fork child
            foreach (var idxFork in e.Args.Children.Where (ix => ix.Name == "fork")) {

                // Iterating each lambda object in fork and creating and starting a new thread, notice NO CLONE!
                foreach (var idxThread in Fork.GetForkObjects (context, idxFork)) {

                    // Creating a new thread for each lambda object in fork statement, 
                    // making sure we keep a reference to thread, such that we can Join later
                    Fork.ThreadStart thread = new Fork.ThreadStart (context, idxThread);
                    thread.Start ();
                    threads.Add (thread);
                }
            }

            // Iterating through each thread created above, joining all with main thread
            foreach (var idxThread in threads) {

                // Invoking Join, which will wait for thread to finish, or "x milliseconds" to pass,
                // whichever occurs first
                // Notice, this means that it will wait for a maximum of "x" milliseconds for EACH THREAD,
                // and not in total for all threads. Meaning, if you create three threads, and thread one uses
                // 10 ms, thread two uses 20 ms, and thread three uses 30 ms. Meaning, if you set wait's milliseconds
                // to 11, then actually 30 milliseconds will pass before [wait] returns, since it will wait three times
                idxThread.Join (e.Args.GetExValue (context, -1));
            }
        }
    }
}
