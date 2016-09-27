/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Threading;
using p5.exp;
using p5.core;

namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [sleep] keyword
    /// </summary>
    public static class Sleep
    {
        /// <summary>
        ///     Sleeps the thread you invoke it on for a specified amount of milliseconds
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "sleep")]
        public static void sleep (ApplicationContext context, ActiveEventArgs e)
        {
            Thread.Sleep (XUtil.Single<int> (context, e.Args, true));
        }
    }
}
