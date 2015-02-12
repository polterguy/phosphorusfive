
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.data
{
    /// <summary>
    /// class wrapping [pf.data.select] and its associated supporting methods
    /// </summary>
    public static class data_select
    {
        /// <summary>
        /// selects items from database according to expression given as value of node, and returns the matches
        /// as children nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.select")]
        private static void pf_data_select (ApplicationContext context, ActiveEventArgs e)
        {
            // acquiring lock on database
            lock (Common.Lock) {

                // verifying syntax
                if (!XUtil.IsExpression (e.Args.Value))
                    throw new ArgumentException ("[pf.data.select] requires an expression to select items from database");

                // making sure database is initialized
                Common.Initialize (context);

                // iterating through each result from database node tree
                foreach (var idxMatch in XUtil.Iterate (e.Args, Common.Database, context)) {

                    // aborting iteration early if it is a 'count' expression
                    if (idxMatch.TypeOfMatch == Match.MatchType.count) {
                        e.Args.Add (new Node (string.Empty, idxMatch.Match.Count));
                        return;
                    }

                    // dependent upon type of expression, we either return a bunch of nodes, flat, with
                    // name being string.Empty, and value being matched value, or we append node itself back
                    // to caller. this allows us to select using expressions which are not of type 'node'
                    if (idxMatch.TypeOfMatch != Match.MatchType.node) {

                        // returning 'value', 'name' or 'path' of expression as children nodes of argument node
                        // having name of returned node being string.Empty and value being result of expression
                        e.Args.Add (new Node (string.Empty, idxMatch.Value));
                    } else {

                        // returning node itself, after cloning
                        e.Args.Add (idxMatch.Node.Clone ());
                    }
                }
            }
        }
    }
}
