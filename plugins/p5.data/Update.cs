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
    ///     Class wrapping [p5.data.update].
    /// 
    ///     Encapsulates the [p5.data.update] Active Event, and its associated helper methods.
    /// </summary>
    public static class Update
    {
        /// <summary>
        ///     Updates nodes in your database.
        /// 
        ///     Updates the results of the given expression in database, either according to a static [source] node,
        ///     or a relative [rel-source] node.
        /// 
        ///     The database stores its nodes as the root node being the database itself, and beneath the root node, are
        ///     all file nodes. This means that your expressions should start with; <em>@/*/*</em>, before the rest of
        ///     your expression, referring to your actual data nodes.
        /// 
        ///     The node used as the "root node" for most database expressions, except [p5.data.insert] though, is the 
        ///     root node of your database, and not your execution tree root node.
        /// 
        ///     Example that will update all names to "Johnny Doe", in items from your database, having a type, 
        ///     containing the string "foo";
        /// 
        ///     <pre>
        /// p5.data.update:@/*/*/"/foo/"/*/name?value
        ///   source:Johnny Doe</pre>
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.data.update")]
        private static void p5_data_update (ApplicationContext context, ActiveEventArgs e)
        {
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
                        idxDestination.Value = XUtil.SourceSingle (e.Args.LastChild, idxDestination.Node, context, true);
                    }
                } else if (e.Args.Count > 0 && e.Args.LastChild.Name == "src") {

                    // figuring out source
                    var source = XUtil.SourceSingle (e.Args.LastChild, context);

                    // iterating through all destinations, updating with source
                    foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (Common.Database, context, e.Args)) {

                        // figuring out which file Node updated belongs to, and storing in changed list
                        Common.AddNodeToChanges (idxDestination.Node, changed);

                        // doing actual update
                        idxDestination.Value = source;
                    }
                } else {

                    // syntax error
                    throw new ArgumentException ("No [src] or [rel-src] was given to [p5.data.update].");
                }
                
                // saving all affected files
                Common.SaveAffectedFiles (context, changed);
            }
        }
    }
}
