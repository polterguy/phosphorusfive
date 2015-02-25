
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.file
{
    /// <summary>
    /// class to help create, traverse, modify and delete folders on disc
    /// </summary>
    public static class folder_remove
    {
        /// <summary>
        /// removes one or more folders from the path given as value of args, which might be a constant, or
        /// an expression. all folders that are successfully removed will be returned as children nodes,
        /// having the path to the folder as name, and its value being true. if a folder does not exist,
        /// the return value will be false for that folder
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.folder.remove")]
        private static void pf_folder_remove (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder
            var rootFolder = common.GetRootFolder (context);

            // iterating through each folder caller wants to create
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                if (Directory.Exists (rootFolder + idx)) {

                    // folder exists, removing it recursively,
                    // and returning success back to caller
                    Directory.Delete (rootFolder + idx, true);
                    e.Args.Add (new Node (idx, true));
                } else {

                    // folder didn't exist, returning that fact back to caller
                    e.Args.Add (new Node (idx, false));
                }
            }
        }
    }
}
