
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
    /// class wrapping [pf.data.delete] and its associated supporting methods
    /// </summary>
    public static class data_delete
    {
        /// <summary>
        /// removes the nodes from the database matching the given expression and returns number of items affected
        /// as value of [pf.data.remove] node's child
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.delete")]
        private static void pf_data_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // acquiring lock on database
            lock (Common.Lock) {

                // making sure database is initialized
                Common.Initialize (context);
            
                // verifying syntax of statement
                if (e.Args.Count != 0)
                    throw new ArgumentException ("[pf.data.delete] does not take any arguments");

                // looping through database matches and removing nodes while storing which files have been changed
                List<Node> changed = new List<Node> ();
                foreach (var idxDest in XUtil.Iterate (e.Args.Get<string> (context), Common.Database, context)) {

                    // figuring out which file Node updated belongs to, and storing in changed list
                    Common.AddNodeToChanges (idxDest.Node, changed);

                    // replacing node in database
                    idxDest.Node.UnTie ();
                }

                // saving all affected files
                Common.SaveAffectedFiles (context, changed);
            }
        }
    }
}
