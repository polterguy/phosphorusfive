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
    ///     Class wrapping the [set] keyword in p5 lambda
    /// </summary>
    public static class Set
    {
        /// <summary>
        ///     The [set] keyword, allows you to change nodes through p5 lambda
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set", Protection = EventProtection.LambdaClosed)]
        public static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Asserting destination is expression
            var destEx = e.Args.Value as Expression;
            if (destEx == null)
                throw new LambdaException (
                    string.Format ("Not a valid destination expression given to [set], value was '{0}', expected expression", e.Args.Value),
                    e.Args, 
                    context);

            // Figuring out source type, for then to execute the corresponding logic
            if (e.Args.Children.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                // Iterating through all destinations, figuring out source relative to each destinations
                foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                    // Source is relative to destination, postponing figuring it out, until we're inside 
                    // our destination nodes, on each iteration, passing in destination node as data source
                    idxDestination.Value = XUtil.SourceSingle (context, e.Args, idxDestination.Node);
                }
            } else {

                // Static source, hence retrieving source before iteration starts, in case destination and source overlaps
                var source = XUtil.SourceSingle (context, e.Args);

                // Iterating through all destinations, updating with source
                foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                    // Setting value on MatchEntity will work correctly for both removal and changing values/names and nodes
                    idxDestination.Value = source;
                }
            }
        }
    }
}
