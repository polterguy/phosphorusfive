/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.data.helpers;
using p5.exp.exceptions;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [update-data]
    /// </summary>
    public static class Update
    {
        /// <summary>
        ///     Updates lambda objects in your database
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "update-data", Protection = EventProtection.LambdaClosed)]
        private static void update_data (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving expression and doing some basic syntax checking
            var ex = e.Args.Value as Expression;
            if (ex == null)
                throw new LambdaException ("[update-data] requires an expression to select items from database", e.Args, context);

            // acquiring lock on database
            lock (Common.Lock) {

                // Making sure database is initialized
                Common.Initialize (context);

                // Used for storing all affected database nodes, such that we know which files to update
                var changed = new List<Node> ();

                // Figuring out source, and executing the corresponding logic
                if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                    // Iterating through all destinations, figuring out source relative to each destinations
                    foreach (var idxDestination in ex.Evaluate (Common.Database, context, e.Args)) {

                        // Figuring out which file Node updated belongs to, and storing in changed list
                        Common.AddNodeToChanges (idxDestination.Node, changed);

                        // Source is relative to destination
                        idxDestination.Value = XUtil.SourceSingle (context, e.Args, idxDestination.Node);
                    }
                } else if (e.Args.Count > 0 && e.Args.LastChild.Name == "src") {

                    // Figuring out source
                    var source = XUtil.SourceSingle (context, e.Args);

                    // Iterating through all destinations, updating with source
                    foreach (var idxDestination in e.Args.Get<Expression> (context).Evaluate (Common.Database, context, e.Args)) {

                        // Making sure user is authorized to insert currently iterated node
                        context.RaiseNative ("authorize", new Node ("authorize").Add("update-data", idxDestination.Node).Add ("args", e.Args));

                        // Figuring out which file Node updated belongs to, and storing in changed list
                        Common.AddNodeToChanges (idxDestination.Node, changed);

                        // Doing actual update
                        idxDestination.Value = source;
                    }
                } else {

                    // Syntax error
                    throw new LambdaException ("No [src] or [rel-src] was given to [update-data]", e.Args, context);
                }
            
                // Saving all affected files
                Common.SaveAffectedFiles (context, changed);
            }
        }
    }
}
