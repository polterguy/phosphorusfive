/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [set] keyword in p5.lambda.
    /// 
    ///     The [set] keyword allows you to change nodes through p5.lambda.
    /// </summary>
    public static class Set
    {
        /// <summary>
        ///     The [set] keyword, allows you to change nodes through p5.lambda.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "set")]
        private static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // asserting destination is expression
            var destEx = e.Args.Value as Expression;
            if (destEx == null)
                throw new LambdaException ("Not a valid destination expression given, make sure you set [set]'s value to an expression using :x:", e.Args, context);

            // figuring out source type, for then to execute the corresponding logic
            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                // iterating through all destinations, figuring out source relative to each destinations
                foreach (var idxDestination in destEx.Evaluate (e.Args, context, e.Args)) {

                    // source is relative to destination, postponing figuring it out, until we're inside 
                    // our destination nodes, on each iteration, passing in destination node as data source
                    idxDestination.Value = XUtil.SourceSingle (e.Args, idxDestination.Node, context);
                }
            } else {

                // static source, hence retrieving source before iteration starts
                var source = XUtil.SourceSingle (e.Args, context);

                // iterating through all destinations, updating with source
                foreach (var idxDestination in destEx.Evaluate (e.Args, context, e.Args)) {
                    idxDestination.Value = source;
                }
            }
        }
    }
}
