/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace phosphorus.file.folder
{
    /// <summary>
    ///     Class to help list all folders within folder(s).
    /// 
    ///     Contains [pf.folder.list-folders], and its associated helper methods.
    /// </summary>
    public static class ListFolders
    {
        /// <summary>
        ///     List all folders in folder(s).
        /// 
        ///     Will list all folders within the specified folder(s).
        /// 
        ///     Example that lists all folders within root folder of your system;
        /// 
        ///     <pre>pf.folder.list-folders:</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.folder.list-folders")]
        private static void pf_folder_list_folders (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each folder passed in by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                // iterating all folders in current directory, and returning as nodes beneath args given
                foreach (var idxFolder in Directory.GetDirectories (rootFolder + idx)) {
                    // normalizing file path delimiters for both Linux and Windows
                    var folderName = idxFolder.Replace ("\\", "/");
                    folderName = folderName.Replace (rootFolder, "");
                    e.Args.Add (new Node (string.Empty, folderName));
                }
            }
        }
    }
}
