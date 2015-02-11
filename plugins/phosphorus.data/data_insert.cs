
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
    /// class wrapping [pf.data.insert] and its associated supporting methods
    /// </summary>
    public static class data_insert
    {
        /// <summary>
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.insert")]
        private static void pf_data_insert (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure database is initialized
            Common.Initialize (context);

            // verifying syntax of statement
            if (e.Args.Count == 0 && string.IsNullOrEmpty (e.Args.Value as string))
                throw new ArgumentException ("[pf.data.insert] requires at least one child node, or a source expression, or source value");

            // looping through all nodes given as children and saving them to database
            List<Node> changed = new List<Node> ();
            foreach (Node idx in XUtil.Iterate<Node> (e.Args, context)) {

                // making sure it is impossible to imsert items wwithout name into database
                if (string.IsNullOrEmpty (idx.Name))
                    throw new ArgumentException ("[pf.data.insert] requires that each item you insert has a name");

                // making sure insert node gets an ID, unless one is explicitly given
                if (idx.Value == null) {
                    idx.Value = Guid.NewGuid ();
                } else {
                    if (XUtil.Iterate (
                        string.Format (@"@/*/*/=""{0}""/?node", idx.Value), 
                            Common.Database, 
                            context).GetEnumerator ().MoveNext ()) {
                        throw new ArgumentException ("ID exists from before in database");
                    }
                }

                // finding next available database file node
                Node fileNode = Common.GetAvailableFileNode (context);

                // figuring out which file Node updated belongs to, and storing in changed list
                if (!changed.Contains (fileNode))
                    changed.Add (fileNode);

                // actually appending node into database
                fileNode.Add (idx.Clone ());
            }
            
            // saving all affected files
            Common.SaveAffectedFiles (context, changed);
        }
    }
}
