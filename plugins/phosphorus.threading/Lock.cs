/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.threading
{
    /// <summary>
    ///     wraps [lock] keyword
    /// </summary>
    public static class Lock
    {
        // wraps a single lock object
        private static readonly Dictionary<string, object> Lockers = new Dictionary<string, object> ();
        // locks access to above dictionary, to make sure logic is thread safe in itself
        private static readonly object GlobalLocker = new object ();

        /// <summary>
        ///     locks an object wrapped by the given string(s), which makes sure other threads trying to access lock
        ///     on same string(s), will have to wait for the first thread to exit the [lock] statement, before they're
        ///     being allowed access to execute the given code
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lock")]
        private static void lambda_lock (ApplicationContext context, ActiveEventArgs e)
        {
            var lockers = new List<string> (XUtil.Iterate<string> (e.Args, context));
            LockNext (
                lockers,
                delegate { context.Raise ("lambda", e.Args); });
        }

        /*
         * lock first string object in array, and pops it off list, before recursively calling self, until
         * no more objects remains, and when all objects are locked, it will execute given "functor" delegate
         */

        private static void LockNext (List<string> lockers, LockFunctor functor)
        {
            if (lockers.Count == 0) {
                functor ();
            } else {
                lock (GetLocker (lockers [0])) {
                    lockers.RemoveAt (0);
                    LockNext (lockers, functor);
                }
            }
        }

        /*
         * returns the locker with the given name
         */

        private static object GetLocker (string name)
        {
            lock (GlobalLocker) {
                if (!Lockers.ContainsKey (name))
                    Lockers [name] = new object ();
                return Lockers [name];
            }
        }

        // delegate used for callback when all objects are locked
        private delegate void LockFunctor ();
    }
}