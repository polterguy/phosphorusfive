/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.data.helpers;

/// <summary>
///     Main namespace for all p5.data Active Events.
/// </summary>
namespace p5.data
{
    /// <summary>
    ///     Class wrapping [insert-data].
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     Inserts nodes into database.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "insert-data")]
        private static void insert_data (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0 && e.Args.Value == null)
                return; // nothing to do here

            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, true)) {

                // acquiring lock on database
                lock (Common.Lock) {

                    // making sure database is initialized
                    Common.Initialize (context);

                    // looping through all nodes given as children or through expression
                    var changed = new List<Node> ();
                    foreach (var idx in XUtil.Iterate<Node> (e.Args, context)) {

                        // inserting node
                        if (e.Args.Value is string && !XUtil.IsExpression (e.Args.Value)) {

                            // source is a string, and not an expression, making sure we add children of converted
                            // string, since conversion routine creates a root node wrapping actual nodes in string
                            foreach (var idxInner in idx.Children) {

                                // inserting node
                                InsertNode (idxInner, context, changed);
                            }
                        } else {

                            // inserting node
                            InsertNode (idx, context, changed);
                        }
                    }

                    // saving all affected files
                    Common.SaveAffectedFiles (context, changed);
                }
            }
        }

        /*
         * inserts one node into database
         */
        private static void InsertNode (Node node, ApplicationContext context, List<Node> changed)
        {
            // syntax checking insert node
            SyntaxCheckInsertNode (node, context);

            // finding next available database file node
            var fileNode = Common.GetAvailableFileNode (context);

            // figuring out which file Node updated belongs to, and storing in changed list
            if (!changed.Contains (fileNode))
                changed.Add (fileNode);

            // actually appending node into database
            fileNode.Add (node.Clone ());
        }

        /*
         * syntax checks node before insertion is allowed
         */
        private static void SyntaxCheckInsertNode (Node node, ApplicationContext context)
        {
            // making sure it is impossible to insert items without a name into database
            if (string.IsNullOrEmpty (node.Name))
                throw new ArgumentException ("[insert-data] requires that each item you insert has a name");

            // making sure insert node gets an ID, unless one is explicitly given
            if (node.Value == null) {
                node.Value = Guid.NewGuid ();
            } else {

                // an ID was given, making sure it doesn't exist from before
                var tmpId = node.Get<string> (context);
                if (Expression.Create (string.Format (@"/*/*/""={0}""", tmpId), context)
                    .Evaluate (Common.Database, context)
                    .GetEnumerator ()
                    .MoveNext ()) {
                    throw new ArgumentException ("ID exists from before in database");
                }
            }
        }
    }
}
