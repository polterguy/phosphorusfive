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

/// <summary>
///     Main namespace for all [pd.data.xxx] Active Events.
/// 
///     The [pf.data.xxx] namespace in Phosphorus.Five contains a memory-bases database implementation, for 
///     small objects, allowing you a quick and dirty database, for smaller systems, and smaller pieces of data.
/// </summary>
namespace phosphorus.data
{
    /// <summary>
    ///     Class wrapping [pf.data.insert].
    /// 
    ///     Contains the Active Events [pf.data.insert], and its asociatedd helper methods.
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     Inserts nodes into database.
        /// 
        ///     Will insert one or more nodes into your database. The nodes inserted, are the children nodes, or the expression
        ///     pointed to by the value of the main node.
        /// 
        ///     Example;
        /// 
        ///     <pre>
        /// pf.data.insert
        ///   foo-bar
        ///     name:Thomas Hansen
        ///     age:1000 years</pre>
        /// 
        ///     All items inserted into your database, will have an ID automatically created for them, unless you explicitly
        ///     give your items an ID yourself. This ID wil be of type 'guid', and globally unique for your items.
        /// 
        ///     This ID will be the 'value' of your item 'root node'. For instance, in the example above, the ID for [foo-bar], 
        ///     will become the value of the [foo-bar] node.
        /// 
        ///     If you let this Active Event create an automatic ID for your items, then this ID will be returned as the 
        ///     value of all your inserted items.
        /// 
        ///     Below is an example of how to use this Active Event with an expression pointing to the items to insert;
        /// 
        ///     <pre>
        /// _items
        ///   foo1
        ///     name:John Doe
        ///   foo2
        ///     name:Jane Doe
        /// pf.data.insert:@/-/*?node</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.data.insert")]
        private static void pf_data_insert (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Count == 0 && e.Args.Value == null)
                return; // nothing to do here

            // acquiring lock on database
            lock (Common.Lock) {
                // making sure database is initialized
                Common.Initialize (context);

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
                // an ID was given, making sure it doesn't exist from before
                var tmpId = node.Get<string> (context);
                if (XUtil.Iterate (
                    string.Format (@"@/*/*/""=\\{0}""/?node", tmpId),
                    Common.Database,
                    context).GetEnumerator ().MoveNext ()) {
                    throw new ArgumentException ("ID exists from before in database");
                }
            }
        }
    }
}
