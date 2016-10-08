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

using System.Collections.Generic;
using p5.exp;
using p5.core;

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
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lock")]
        public static void p5_lock (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving all lockers caller wants to lock
            var lockers = new List<string> (XUtil.Iterate<string> (context, e.Args));

            // Recursively waits for each locker to be unlocked, evaluating given lambda, once all lockers are unlocked
            LockNext (
                lockers, delegate {
                    context.Raise ("eval-mutable", e.Args);
                });
        }

        /*
         * Locks first string object in array, and pops it off list, before recursively calling self, until
         * no more objects remains. When all objects are unlocked, then it will execute given "functor" delegate
         */
        private static void LockNext (List<string> lockers, LockFunctor functor)
        {
            // Checking if there are any more lockers to wait for
            if (lockers.Count == 0) {

                // No more lockers to wait for, evaluating lambda
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
            // trying to create or retrieve locker(s)
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
