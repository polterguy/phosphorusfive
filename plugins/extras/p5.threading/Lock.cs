/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
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

using System.Linq;
using System.Threading;
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
        private static readonly Dictionary<string, ReaderWriterLockSlim> Lockers = new Dictionary<string, ReaderWriterLockSlim> ();

        // Locks access to above dictionary, to make sure access to "Lockers" is thread safe in itself
        private static readonly object GlobalLocker = new object ();

        // Delegate used for callback to execute once locker(s) is/are unlocked
        private delegate void LockFunctor ();

        /// <summary>
        ///     Locks the locker(s) with the given name(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lock")]
        [ActiveEvent (Name = "write-lock")]
        [ActiveEvent (Name = "p5.threading.lock")]
        [ActiveEvent (Name = "p5.threading.write-lock")]
        public static void p5_lock (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving all lockers caller wants to lock
            var lockers = XUtil.Iterate<string> (context, e.Args).ToList ();

            // Recursively waits for each locker to be unlocked, evaluating given lambda, once all lockers are unlocked
            WriteLock (
                lockers, delegate {
                    context.RaiseEvent ("eval-mutable", e.Args);
                });
        }

        /// <summary>
        ///     Locks the locker(s) with the given name(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "read-lock")]
        [ActiveEvent (Name = "p5.threading.read-lock")]
        public static void p5_read_lock (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving all lockers caller wants to lock
            var lockers = XUtil.Iterate<string> (context, e.Args).ToList ();

            // Recursively waits for each locker to be unlocked, evaluating given lambda, once all lockers are unlocked
            ReadLock (
                lockers, delegate {
                    context.RaiseEvent ("eval-mutable", e.Args);
                });
        }

        /*
         * Locks first string object in array, and pops it off list, before recursively calling self, until
         * no more objects remains. When all objects are unlocked, then it will execute given "functor" delegate
         */
        private static void WriteLock (List<string> lockers, LockFunctor functor)
        {
            // Checking if there are any more lockers to wait for.
            if (lockers.Count == 0) {

                // No more lockers to wait for, evaluating lambda.
                functor ();

            } else {

                // Retrieves next locker, and locks it as a write lock.
                var locker = GetLocker (lockers [0]);
                locker.EnterWriteLock ();
                try {

                    // Removing current lock from list.
                    lockers.RemoveAt (0);

                    // Recursively invoking "self", now with one locker less in list of lockers to wait for.
                    WriteLock (lockers, functor);

                } finally {

                    // Opening locker.
                    locker.ExitWriteLock ();
                }
            }
        }

        /*
         * Locks first string object in array, and pops it off list, before recursively calling self, until
         * no more objects remains. When all objects are unlocked, then it will execute given "functor" delegate
         */
        private static void ReadLock (List<string> lockers, LockFunctor functor)
        {
            // Checking if there are any more lockers to wait for.
            if (lockers.Count == 0) {

                // No more lockers to wait for, evaluating lambda.
                functor ();

            } else {

                // Retrieves next locker, and locks it as a write lock.
                var locker = GetLocker (lockers [0]);
                locker.EnterReadLock ();
                try {

                    // Removing current lock from list.
                    lockers.RemoveAt (0);

                    // Recursively invoking "self", now with one locker less in list of lockers to wait for.
                    ReadLock (lockers, functor);

                } finally {

                    // Opening locker.
                    locker.ExitReadLock ();
                }
            }
        }

        /*
         * Returns the locker with the given name. If locker does not exist, it will be created
         */
        private static ReaderWriterLockSlim GetLocker (string name)
        {
            // Synchronizing access to lockers, to avoid having multiple threads 
            // trying to create or retrieve locker(s) at the same time
            lock (GlobalLocker) {

                // Making sure currently requested locker is created if it does not exist
                if (!Lockers.ContainsKey (name))
                    Lockers [name] = new ReaderWriterLockSlim ();

                // Returning locker with given name
                return Lockers [name];
            }
        }
    }
}
