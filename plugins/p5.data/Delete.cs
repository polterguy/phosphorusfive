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
    ///     Class wrapping [delete-data].
    /// </summary>
    public static class Remove
    {
        /// <summary>
        ///     Removes nodes from your database.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "delete-data")]
        private static void delete_data (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return;

            if (!XUtil.IsExpression (e.Args.Value))
                throw new ArgumentException ("[delete-data] requires an expression as its value");

            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, true)) {

                // acquiring lock on database
                lock (Common.Lock) {

                    // making sure database is initialized
                    Common.Initialize (context);

                    // looping through database matches and removing nodes while storing which files have been changed
                    var changed = new List<Node> ();
                    foreach (var idxDest in e.Args.Get<Expression> (context).Evaluate (Common.Database, context, e.Args)) {

                        // making sure "file nodes" and "root node" cannot be remove
                        if (idxDest.Node.Path.Count < 2)
                            throw new ArgumentException ("You cannot remove the actual file node, or root node from your database");

                        // figuring out which file Node updated belongs to, and storing in changed list
                        Common.AddNodeToChanges (idxDest.Node, changed);

                        // setting value to null, which works if user chooses to remove "value", "name" and/or "node"
                        idxDest.Value = null;
                    }

                    // saving all affected files
                    Common.SaveAffectedFiles (context, changed);
                }
            }
        }
    }
}
