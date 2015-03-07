/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.data.helpers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.data
{
    /// <summary>
    ///     Class wrapping [pf.data.update] and its associated supporting methods
    /// </summary>
    public static class Update
    {
        /// <summary>
        ///     Updates the results of the given expression in database, either according to a static [source] node,
        ///     or a relative [rel-source] node
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.update")]
        private static void pf_data_update (ApplicationContext context, ActiveEventArgs e)
        {
            // acquiring lock on database
            lock (Common.Lock) {

                // making sure database is initialized
                Common.Initialize (context);

                // storing all affected nodes, such that we know which files to update
                var changed = new List<Node> ();

                // figuring out source, and executing the corresponding logic
                if (e.Args.Count > 0 && (e.Args.LastChild.Name == "rel-source" || e.Args.LastChild.Name == "rel-src")) {

                    // iterating through all destinations, figuring out source relative to each destinations
                    foreach (var idxDestination in XUtil.Iterate (e.Args, Common.Database, context)) {
                        // figuring out which file Node updated belongs to, and storing in changed list
                        Common.AddNodeToChanges (idxDestination.Node, changed);

                        // source is relative to destination
                        idxDestination.Value = XUtil.SourceSingle (e.Args, idxDestination.Node, context);
                    }
                } else if (e.Args.Count > 0 && (e.Args.LastChild.Name == "source" || e.Args.LastChild.Name == "src")) {

                    // figuring out source
                    var source = XUtil.SourceSingle (e.Args, context);

                    // iterating through all destinations, updating with source
                    foreach (var idxDestination in XUtil.Iterate (e.Args, Common.Database, e.Args, context)) {
                        // figuring out which file Node updated belongs to, and storing in changed list
                        Common.AddNodeToChanges (idxDestination.Node, changed);

                        // doing actual update
                        idxDestination.Value = source;
                    }
                } else {
                    // syntax error
                    throw new ArgumentException ("No [source] or [rel-source] was given to [pf.data.update].");
                }
                
                // saving all affected files
                Common.SaveAffectedFiles (context, changed);
            }
        }
    }
}