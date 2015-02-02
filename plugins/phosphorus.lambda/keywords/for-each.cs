
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda
{
    /// <summary>
    /// class wrapping execution engine keyword "for-each", which allows for iteratively executing code for every instance in an expression
    /// </summary>
    public static class forEach
    {
        /// <summary>
        /// [for-each] keyword for execution engine, allowing for iterating over an expression returning a list of nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "for-each")]
        private static void lambda_for_each (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Iterate<Node> (e.Args, 
            delegate (Node idxSource) {
                if (idxSource == null)
                    throw new ArgumentException ("source expression for [for-each] returned value that was not of type 'node'");
                Node dp = new Node ("__dp", idxSource);
                e.Args.Insert (0, dp);
                context.Raise ("lambda.immutable", e.Args);
                e.Args [0].Untie ();
            });
        }
    }
}
