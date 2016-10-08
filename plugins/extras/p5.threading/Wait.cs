/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5t = p5.threading.helpers;

namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [wait] keyword
    /// </summary>
    public static class Wait
    {
        /// <summary>
        ///     Waits for all [fork] children to finish, or x milliseconds, before allowing execution to continue
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "wait")]
        public static void wait (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Children.Count (ix => ix.Name != "fork" && ix.Name != "") != 0)
                throw new LambdaException (
                    "[wait] cannot have any other types of children but [fork] nodes", 
                    e.Args, 
                    context);

            // Figuring out for how long we should wait, defaults to "infinite"
            int milliseconds = e.Args.GetExValue (context, -1);

            // Keeping a reference to all threads created, such that we can join them afterwards
            var threads = CreateThreads (context, e.Args);

            // Wait for all threads for as long as caller requested
            WaitForAll (milliseconds, threads);
        }

        /*
         * Creates and spawns all threads given lambda
         */
        private static IEnumerable<p5t.Thread> CreateThreads (ApplicationContext context, Node args)
        {
            // Return value
            var retVal = new List<p5t.Thread> ();

            // Looping through each fork child
            foreach (var idxFork in args.Children.Where (ix => ix.Name == "fork")) {

                // Iterating each lambda object in fork and creating and starting a new thread, notice NO CLONE!
                foreach (var idxThreadLambda in p5t.Thread.GetForkObjects (context, idxFork)) {

                    // Creates a new thread for each lambda object in fork statement, 
                    // making sure we keep a reference to thread, that we can return to caller once done.
                    // Notice, we cannot use "yield return" here, since that would postpone creation until
                    // IEnumerable is iterated over above, which means threads would be spawned and created
                    // sequentially ...!
                    var thread = new p5t.Thread (context, idxThreadLambda);
                    thread.Start ();
                    retVal.Add (thread);
                }
            }

            // Returning threads to caller
            return retVal;
        }

        /*
         * Waits for all threads to finish, or for the specified amount of milliseconds supplied. 
         * If "-1" is supplied, will wait for all threads to finish, for an "infinite2 amount of time
         */
        private static void WaitForAll (int milliseconds, IEnumerable<p5t.Thread> threads)
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
