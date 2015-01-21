
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda
{
    /// <summary>
    /// class wrapping execution engine keyword "pf.while" for looping while condition is true
    /// conditional execution of nodes
    /// </summary>
    public static class pfWhile
    {
        /// <summary>
        /// "while" statement, allowing for evaluating condition, and executing lambda(s) as long as statement evaluates to true
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "while")]
        private static void lambda_while (ApplicationContext context, ActiveEventArgs e)
        {
            var condition = new Conditions (e.Args);
            while (condition.Evaluate ()) {
                foreach (Node idxExe in condition.ExecutionLambdas) {
                    context.Raise (idxExe.Name, idxExe);
                }
            }
        }
    }
}
