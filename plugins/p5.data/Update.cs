/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.core;
using p5.exp;
using p5.data.helpers;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [update-data].
    /// </summary>
    public static class Update
    {
        /// <summary>
        ///     Updates nodes in your database.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "update-data")]
        private static void update_data (ApplicationContext context, ActiveEventArgs e)
        {
            var ex = e.Args.Value as Expression;
            if (ex == null)
                throw new ArgumentException ("[update-data] requires an expression to select items from database");

            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // acquiring lock on database
                lock (Common.Lock) {

                    // making sure database is initialized
                    Common.Initialize (context);

                    // storing all affected nodes, such that we know which files to update
                    var changed = new List<Node> ();

                    // figuring out source, and executing the corresponding logic
                    if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                        // iterating through all destinations, figuring out source relative to each destinations
                        foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (Common.Database, context, e.Args)) {

                            // figuring out which file Node updated belongs to, and storing in changed list
                            Common.AddNodeToChanges (idxDestination.Node, changed);

                            // source is relative to destination
                            idxDestination.Value = XUtil.SourceSingle (e.Args, idxDestination.Node, context);
                        }
                    } else if (e.Args.Count > 0 && e.Args.LastChild.Name == "src") {

                        // figuring out source
                        var source = XUtil.SourceSingle (e.Args, context);

                        // iterating through all destinations, updating with source
                        foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (Common.Database, context, e.Args)) {

                            // figuring out which file Node updated belongs to, and storing in changed list
                            Common.AddNodeToChanges (idxDestination.Node, changed);

                            // doing actual update
                            idxDestination.Value = source;
                        }
                    } else {

                        // syntax error
                        throw new ArgumentException ("No [src] or [rel-src] was given to [update-data].");
                    }
                
                    // saving all affected files
                    Common.SaveAffectedFiles (context, changed);
                }
            }
        }
    }
}
