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
    ///     Class wrapping the [lock] keyword.
    /// 
    ///     The [lock] keyword, allows you to lock access to a shared resource from within your threads.
    /// </summary>
    public static class Lock
    {
        // wraps a single lock object
        private static readonly Dictionary<string, object> Lockers = new Dictionary<string, object> ();
        // locks access to above dictionary, to make sure logic is thread safe in itself
        private static readonly object GlobalLocker = new object ();

        /// <summary>
        ///     Locks an object wrapped by the given string(s).
        /// 
        ///     [lock] allows you to make sure that only one thread within your system, is given access to some object, shared among multiple threads.
        /// 
        ///     If you use [lock] with for instance the string value of "foo", then all other objects trying to lock the same string of "foo", will
        ///     be left outside of their [lock] statement, having to wait for the first piece of code that locked the string "foo" to finish, before
        ///     they're allowed to enter the code, and create a lock on the same object, or string.
        /// 
        ///     Example;
        /// 
        ///     <pre>_foo
        /// set:@/+/0?value
        ///   source:@/./-?node
        /// lambda.fork
        ///   _foo
        ///   lock:foo
        ///     append:@/./-/#?node
        ///       source
        ///         bar:thread
        /// sleep:1
        /// lock:foo
        ///   append:@/../"*"/_foo?node
        ///     source
        ///       bar:main</pre>
        /// 
        ///     The above piece of code, will deny the main thread to enter the [lock] code-block, before the other thread, created through [lambda.fork],
        ///     is finished executing its work. This prevents a race-condition between the main thread, and the [lambda.fork] thread, when accessing and
        ///     modifying the [_foo] node.
        /// 
        ///     The [sleep] statement above, is simply there to ensure that the [lambda.fork] thread starts its work, before the
        ///     main thread enters its [lock]. Without the [sleep] statement above, the main thread might theoretically have entered its [lock], before
        ///     the worker thread, which would result in that you wouldn't have access to whatever the worker thread did, before the code was finished executing.
        /// 
        ///     See the <see cref="phosphorus.threading.Wait.lambda_wait">[wait]</see> Active Event for an example of how to create multiple threads,
        ///     and a deeper explanation of how threading works.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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