/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;
using p5.exp;
using p5.data.helpers;

namespace p5.data
{
    /// <summary>
    ///     Class wrapping [p5.data.select].
    /// 
    ///     Encapsulates the [p5.data.select] Active Event, and its associated supporting methods.
    /// </summary>
    public static class Select
    {
        /// <summary>
        ///     Selects nodes from your database.
        /// 
        ///     Selects items from your database, according to expression given as value of node, and returns the matches
        ///     as children nodes.
        /// 
        ///     The database stores its nodes as the root node being the database itself, and beneath the root node, are
        ///     all file nodes. This means that your expressions should start with; <em>@/*/*</em>, before the rest of
        ///     your expression, referring to your actual data nodes.
        /// 
        ///     The node used as the "root node" for most database expressions, except [p5.data.insert] though, is the 
        ///     root node of your database, and not your execution tree root node.
        /// 
        ///     Example that will select all items from your database, having a type, containing the string "foo";
        /// 
        ///     <pre>
        /// p5.data.select:@/*/*/"/foo/"?node</pre>
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.data.select")]
        private static void p5_data_select (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here ...

            // acquiring lock on database
            lock (Common.Lock) {

                // verifying syntax
                if (!XUtil.IsExpression (e.Args.Value))
                    throw new ArgumentException ("[p5.data.select] requires an expression to select items from database");

                // making sure database is initialized
                Common.Initialize (context);

                var ex = e.Args.Value as Expression;

                // iterating through each result from database node tree
                foreach (var idxMatch in ex.Evaluate (Common.Database, context, e.Args)) {

                    if (idxMatch.Match.TypeOfMatch == Match.MatchType.count) {

                        // Expression is for 'count' type, ending iteration
                        e.Args.Add (new Node (string.Empty, idxMatch.Match.Count));
                        return;
                    }

                    // dependent upon type of expression, we either return a bunch of nodes, flat, with
                    // name being string.Empty, and value being matched value, or we append node itself back
                    // to caller. this allows us to select using expressions which are not of type 'node'
                    e.Args.Add (idxMatch.TypeOfMatch != Match.MatchType.node ? 
                                new Node (string.Empty, idxMatch.Value) : 
                                idxMatch.Node.Clone ());
                }
            }
        }
    }
}
