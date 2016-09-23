/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping execution engine keyword [src]
    /// </summary>
    public static class Src
    {
        /// <summary>
        ///     The [src] event evaluates an expression, constant or children nodes, and returns the results as list in value of node
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "src", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "dest", Protection = EventProtection.LambdaClosed)]
        public static void lambda_src_dest (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args)) {

                // Figuring out type of source
                Expression exp = e.Args.Value as Expression;
                if (exp != null) {

                    // Source is an expression, evaluating it, and returning results back to caller
                    var match = exp.Evaluate (context, e.Args, e.Args);
                    e.Args.Value = match.TypeOfMatch == Match.MatchType.count ? (object)match.Count : (object)match.Select (ix => ix.Value).ToList ();
                } else if (e.Args.Value != null) {

                    // Still there's a value, checking type of value
                    if (XUtil.IsFormatted (e.Args)) {

                        // Returning formatted value
                        e.Args.Value = new List<object> (new object[] { XUtil.FormatNode (context, e.Args) });
                    } else {

                        // Returning constant, making sure output of event is list
                        e.Args.Value = new List<object> (new object[] { e.Args.Value });
                    }
                } else {

                    // Returning all children nodes
                    e.Args.Value = e.Args.Children.Where (ix => ix.Name != "_dn").Select (ix => ix.Clone ()).ToList<object> ();
                }
            }
        }
    }
}
