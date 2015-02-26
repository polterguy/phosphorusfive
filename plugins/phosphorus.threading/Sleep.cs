/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Threading;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.threading
{
    /// <summary>
    ///     wraps [sleep] keyword
    /// </summary>
    public static class Sleep
    {
        /// <summary>
        ///     sleeps for n milliseconds, where n is given as value of node
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "sleep")]
        private static void lambda_sleep (ApplicationContext context, ActiveEventArgs e)
        {
            var milliseconds = XUtil.Single<int> (e.Args, context);
            Thread.Sleep (milliseconds);
        }
    }
}