/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using p5.core;
using p5.exp;

namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [lock] keyword
    /// </summary>
    public static class Lock
    {
        // Wraps all lockers in system
        private static readonly Dictionary<string, object> Lockers = new Dictionary<string, object> ();

        // Locks access to above dictionary, to make sure access to "Lockers" is thread safe in itself
        private static readonly object GlobalLocker = new object ();

        // Delegate used for callback to execute once locker(s) is/are unlocked
        private delegate void LockFunctor ();

        /// <summary>
        ///     Locks the locker(s) with the given name(s)
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lock", Protection = EventProtection.LambdaClosed)]
        private static void threading_lock (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving all lockers caller wants to lock
            var lockers = new List<string> (XUtil.Iterate<string> (context, e.Args));

            // Recursively waits for each locker to be unlocked, evaluating given lambda, once
            // all lockers are unlocked
            LockNext (lockers, delegate { context.RaiseLambda ("eval-mutable", e.Args); });
        }

        /*
         * Lock first string object in array, and pops it off list, before recursively calling self, until
         * no more objects remains. When all objects are unlocked, then it will execute given "functor" delegate
         */
        private static void LockNext (List<string> lockers, LockFunctor functor)
        {
            if (lockers.Count == 0) {

                // No more lockers to wait for
                functor ();
            } else {

                // Retrieves next locker, and locks it again, once it is unlocked
                lock (GetLocker (lockers [0])) {

                    // Removing current locker from list
                    lockers.RemoveAt (0);

                    // Recursively invoking "self", now with one locker less in list of lockers to wait for
                    LockNext (lockers, functor);
                }
            }
        }

        /*
         * Returns the locker with the given name. If locker does not exist, it will be created
         */
        private static object GetLocker (string name)
        {
            // Locking access to lockers, to avoid race conditions by multiple threads 
            // trying to create or retrieve the same locker(s)
            lock (GlobalLocker) {

                // Making sure currently requested locker is created if it does not exist
                if (!Lockers.ContainsKey (name))
                    Lockers [name] = new object ();

                // Returning locker with given name
                return Lockers [name];
            }
        }
    }
}
