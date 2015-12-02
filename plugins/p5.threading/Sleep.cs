/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Threading;
using p5.core;
using p5.exp;

namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [sleep] keyword.
    ///     milliseconds.
    /// </summary>
    public static class Sleep
    {
        /// <summary>
        ///     Sleeps the thread you invoke it on for a specified time interval.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sleep")]
        private static void lambda_sleep (ApplicationContext context, ActiveEventArgs e)
        {
            var milliseconds = XUtil.Single<int> (context, e.Args);
            Thread.Sleep (milliseconds);
        }
    }
}
