/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.data.helpers;

/// <summary>
///     Main namespace for all p5.data Active Events
/// </summary>
namespace p5.data
{
    /// <summary>
    ///     Class wrapping [insert-data]
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     Inserts nodes into database
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "insert-data", Protection = EntranceProtection.Lambda)]
        private static void insert_data (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * Note, since [insert-data] creates an ID for items not explicitly giving an ID,
             * we do NOT remove arguments in this Active Event, since sometimes caller needs to
             * know the ID of the node inserted into database, and is not ready to create an ID
             * for it himself. Therefor the generated ID is returned as tha value of each item inserted,
             * and hence we cannot remove all arguments passed into Active Event
             * 
             * However, we DO remove children of each root node inserted, unless string is submitted 
             * as source somehow ...
             */

            // Acquiring lock on database
            lock (Common.Lock) {

                // Making sure database is initialized
                Common.Initialize (context);

                // Used to store how many items are actually affected
                int affectedItems = 0;

                // Looping through all nodes given as children, value, or as result from expression
                var changed = new List<Node> ();
                foreach (var idx in XUtil.Iterate<Node> (context, e.Args, false, false, true)) {

                    // Inserting node
                    if (e.Args.Value is string) {

                        // Source is a string, and not an expression, making sure we add children of converted
                        // string, since conversion routine creates a root node wrapping actual nodes in string
                        foreach (var idxInner in idx.Children) {

                            // Inserting node
                            InsertNode (idxInner, context, changed);
                        }
                    } else {

                        // Making sure we clean up and remove all arguments of inserted node passed in after execution
                        using (new Utilities.ArgsRemover (idx)) {

                            // Making sure user is authorized to insert currently iterated node
                            context.Raise ("authorize", new Node ("authorize").Add("insert-data", idx).Add ("args", e.Args));

                            // Inserting node
                            InsertNode (idx, context, changed);
                        }
                    }

                    // Incrementing affected items
                    affectedItems += 1;
                }

                // Saving all affected files
                Common.SaveAffectedFiles (context, changed);

                // Returning number of affected items
                e.Args.Value = affectedItems;
            }
        }

        /*
         * Inserts one node into database
         */
        private static void InsertNode (Node node, ApplicationContext context, List<Node> changed)
        {
            // Syntax checking insert node
            SyntaxCheckInsertNode (node, context);

            // Finding next available database file node
            var fileNode = Common.GetAvailableFileNode (context);

            // Figuring out which file Node updated belongs to, and storing in changed list
            if (!changed.Contains (fileNode))
                changed.Add (fileNode);

            // Actually appending node into database
            fileNode.Add (node.Clone ());
        }

        /*
         * Syntax checks node before insertion is allowed
         */
        private static void SyntaxCheckInsertNode (Node node, ApplicationContext context)
        {
            // Making sure it is impossible to insert items without a name into database
            if (string.IsNullOrEmpty (node.Name))
                throw new LambdaException ("[insert-data] requires that each item you insert has a name", node, context);

            // Making sure insert node gets an ID, unless one is explicitly given
            if (node.Value == null) {

                // Automatically generating an ID for item, since no ID was supplied by caller
                node.Value = Guid.NewGuid ();
            } else {

                // An ID was given, making sure it doesn't exist from before
                var tmpId = node.Get<string> (context);
                if (Expression.Create (string.Format (@"/*/*/""={0}""", tmpId), context)
                    .Evaluate (Common.Database, context)
                    .GetEnumerator ()
                    .MoveNext ()) {
                    throw new LambdaException ("ID exists from before in database", node, context);
                }
            }
        }
    }
}
