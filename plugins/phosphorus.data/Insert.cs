/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.data
{
    /// <summary>
    ///     class wrapping [pf.data.insert] and its associated supporting methods
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     inserts nodes into database
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.insert")]
        private static void pf_data_insert (ApplicationContext context, ActiveEventArgs e)
        {
            // acquiring lock on database
            lock (Common.Lock) {
                // making sure database is initialized
                Common.Initialize (context);

                // verifying syntax of statement
                if (e.Args.Count == 0 && e.Args.Value == null)
                    throw new ArgumentException ("[pf.data.insert] requires at least one child node, or a source expression, or source value");

                // looping through all nodes given as children and saving them to database
                var changed = new List<Node> ();
                foreach (var idx in XUtil.Iterate<Node> (e.Args, context)) {
                    if (e.Args.Value is string && !XUtil.IsExpression (e.Args.Value)) {
                        // source is a string, but not an expression, making sure we add children of converted
                        // string, since conversion routine creates a root node wrapping actual nodes in string
                        foreach (var idxInner in idx.Children) {
                            InsertNode (idxInner, context, changed);
                        }
                    } else {
                        InsertNode (idx, context, changed);
                    }
                }

                // saving all affected files
                Common.SaveAffectedFiles (context, changed);
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
                throw new ArgumentException ("[pf.data.insert] requires that each item you insert has a name");

            // making sure insert node gets an ID, unless one is explicitly given
            if (node.Value == null) {
                node.Value = Guid.NewGuid ();
            } else {
                var tmpId = node.Get<string> (context);
                if (XUtil.Iterate (
                    string.Format (@"@/*/*/""={0}""/?node", (tmpId.StartsWith ("/") ? "\\\\" + tmpId : tmpId)),
                    Common.Database,
                    context).GetEnumerator ().MoveNext ()) {
                    throw new ArgumentException ("ID exists from before in database");
                }
            }
        }
    }
}