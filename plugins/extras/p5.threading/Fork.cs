/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5t = p5.threading.helpers;

/// <summary>
///     Main namespace for all things related to threading in Phosphorus Five
/// </summary>
namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [fork] keyword
    /// </summary>
    public static class Fork
    {
        /// <summary>
        ///     Forks a new thread with the given lambda object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "fork")]
        public static void fork (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each lambda object in fork statement
            foreach (var idxThreadLambda in p5t.Thread.GetForkObjects (context, e.Args)) {

                // Starts a new thread, notice CLONE, to avoid thread having access to nodes outside of itself,
                // or lambda object being manipulated after evaluation of thread has started
                new p5t.Thread (context, idxThreadLambda.Clone ()).Start ();
            }
        }
    }
}
