
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Threading;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.threading
{
    /// <summary>
    /// wraps [sleep] keyword
    /// </summary>
    public static class sleep
    {
        /// <summary>
        /// sleeps for n milliseconds, where n is given as value of node
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "sleep")]
        private static void lambda_sleep (ApplicationContext context, ActiveEventArgs e)
        {
            var milliseconds = XUtil.Single<int> (e.Args, context);
            Thread.Sleep (milliseconds);
        }
    }
}
