/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
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
        ///     Waits for all [fork] children to finish, or x milliseconds, before allowing execution to pass
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "wait", Protection = EventProtection.LambdaClosed)]
        private static void threading_wait (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Children.Count (ix => ix.Name != "fork" && ix.Name != "") != 0)
                throw new LambdaException ("[wait] cannot have any other types of children but [fork] statements", e.Args, context);

            // Figuring out for how long we should wait, defaults to "indefinitely"
            int milliseconds = e.Args.GetExValue (context, -1);

            // Keeping a reference to all threads created, such that we can join them afterwards
            var threads = CreateThreads (context, e.Args);

            // Wait for all threads for as long as caller requested
            WaitForAll (milliseconds, threads);
        }

        /*
         * Creates and spawns all threads given lambda
         */
        private static IEnumerable<Fork.ThreadStart> CreateThreads (ApplicationContext context, Node args)
        {
            // Return value
            var retVal = new List<Fork.ThreadStart> ();

            // Looping through each fork child
            foreach (var idxFork in args.Children.Where (ix => ix.Name == "fork")) {

                // Iterating each lambda object in fork and creating and starting a new thread, notice NO CLONE!
                foreach (var idxThread in Fork.GetForkObjects (context, idxFork)) {

                    // Creating a new thread for each lambda object in fork statement, 
                    // making sure we keep a reference to thread, that we can return to caller once done
                    // Notice, we cannot use "yield return" here, since that would postpone creation until
                    // IEnumerable is iterated over above, which means threads would be spawned and created
                    // sequentially ...!
                    Fork.ThreadStart thread = new Fork.ThreadStart (context, idxThread);
                    thread.Start ();
                    retVal.Add (thread);
                }
            }

            // Returning threads to caller
            return retVal;
        }

        /*
         * Waits for all threads to finish for the number of milliseconds supplied, or indefinitely if "-1" is supplied
         */
        private static void WaitForAll (int milliseconds, IEnumerable<Fork.ThreadStart> threads)
        {
            // Iterating through each thread created above, joining all with main thread.
            // Notice, we need to do weird math here to make the milliseconds property function properly,
            // to make the [wait] as a whole, wait x milliseconds, and not each Join operation
            DateTime begin = DateTime.Now;
            foreach (var idxThread in threads) {

                // Checking if caller expects [wait] to wait "forever"
                if (milliseconds == -1) {

                    // Waiting indefinitely!
                    idxThread.Join ();
                } else {

                    // Calculating delta, such that we can pass in correct amount of milliseconds to each Join
                    TimeSpan currentDelta = DateTime.Now - begin;

                    // Checking if we should wait at all, or if [wait]'s milliseconds already has passed
                    if (currentDelta.TotalMilliseconds < milliseconds) {

                        // Yup, we should wait, now we need to figure out for how long the wait is!
                        int currentMilliseconds = milliseconds - (int)currentDelta.TotalMilliseconds;
                        idxThread.Join (currentMilliseconds);
                    }
                }
            }
        }
    }
}
