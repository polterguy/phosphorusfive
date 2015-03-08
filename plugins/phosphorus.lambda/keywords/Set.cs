/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.lambda.keywords
{
    /// \todo [set] should probably be allowed to create new nodes, think about it ...?
    /// <summary>
    ///     class wrapping execution engine keyword "put", which allows for changing the value and name of nodes, or the node
    ///     itself
    /// </summary>
    public static class Set
    {
        /// <summary>
        ///     [set] keyword for execution engine. allows changing the node tree. legal sources and destinations are 'name',
        ///     'value' or 'node'
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set")]
        private static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // figuring out source type, for then to execute the corresponding logic
            if (e.Args.Count > 0 && (e.Args.LastChild.Name == "rel-source" || e.Args.LastChild.Name == "rel-src")) {
                // iterating through all destinations, figuring out source relative to each destinations
                foreach (var idxDestination in XUtil.Iterate (e.Args, context)) {
                    // source is relative to destination, postponing figuring it out, until we're inside 
                    // our destination nodes, on each iteration, passing in destination node as data source
                    idxDestination.Value = XUtil.SourceSingle (e.Args, idxDestination.Node, context);
                }
            } else {
                // static source, hence retrieving source before iteration starts
                var source = XUtil.SourceSingle (e.Args, context);

                // iterating through all destinations, updating with source
                foreach (var idxDestination in XUtil.Iterate (e.Args, context)) {
                    idxDestination.Value = source;
                }
            }
        }
    }
}