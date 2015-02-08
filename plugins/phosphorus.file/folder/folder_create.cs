
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Reflection;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.file
{
    /// <summary>
    /// class to help create folders on disc
    /// </summary>
    public static class folder_create
    {
        /// <summary>
        /// creates one or more folder on disc from the path given as value of args, 
        /// which might be a constant, or an expression. all folders that are successfully created will
        /// be returned as children nodes, having the path to the folder as name, and its value being true.
        /// if a folder already exists from before, the return value will be false
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.folder.create")]
        private static void pf_folder_create (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder
            string rootFolder = common.GetRootFolder (context);

            // iterating through each folder caller wants to create
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                if (Directory.Exists (rootFolder + idx)) {

                    // folder already exists, returning that fact back to caller
                    e.Args.Add (new Node (idx, false));
                } else {

                    // folder didn't exist, creating it and returning success back to caller
                    Directory.CreateDirectory (rootFolder + idx);
                    e.Args.Add (new Node (idx, true));
                }
            }
        }
    }
}
