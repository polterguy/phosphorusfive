
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
    /// class to help list folders on disc
    /// </summary>
    public static class folder
    {
        /// <summary>
        /// list all the folders in folder given as value of args given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.folder.list-folders")]
        private static void pf_folder_list_folders (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder
            string rootFolder = common.GetRootFolder (context);

            // iterating through each folder passed in by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // iterating all folders in current directory, and returning as nodes beneath args given
                foreach (var idxFolder in Directory.GetDirectories (rootFolder + idx)) {

                    // normalizing file path delimiters for both Linux and Windows
                    string folderName = idxFolder.Replace ("\\", "/");
                    folderName = folderName.Replace (rootFolder, "");
                    e.Args.Add (new Node (string.Empty, folderName));
                }
            }
        }
    }
}
