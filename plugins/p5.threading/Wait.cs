/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [wait] keyword.
    /// </summary>
    public static class Wait
    {
        /// <summary>
        ///     Waits for all [fork] children to finish, before execution passes on.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "wait", Protection = EventProtection.LambdaClosed)]
        private static void lambda_wait (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Children.Count( ix => ix.Name != "fork" && ix.Name != "") != 0)
                throw new LambdaException ("[wait] cannot have any other types of children but [fork] statements", e.Args, context);

            // looping through each fork child
            foreach (var idxFork in e.Args.Children.Where (ix => ix.Name == "fork")) {

                // iterating each lambda object in fork
                foreach (var idxThread in Fork.GetForkObjects (context, idxFork)) {

                    // creating a new thread for each lambda object in fork statement
                    Fork.ThreadStart thread = new Fork.ThreadStart (context, idxThread);
                    thread.Start ();
                    thread.Join (e.Args.GetExValue (context, -1));
                }
            }
        }
    }
}
