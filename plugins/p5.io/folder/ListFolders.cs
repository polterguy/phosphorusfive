/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.exp;

namespace p5.file.folder
{
    /// <summary>
    ///     Class to help list all folders within folder(s).
    /// </summary>
    public static class ListFolders
    {
        /// <summary>
        ///     List all folders in folder(s).
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "list-folders")]
        private static void p5_folder_list_folders (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // retrieving root folder
                var rootFolder = Common.GetRootFolder (context);

                // iterating through each folder passed in by caller
                foreach (var idx in Common.GetSource (e.Args, context)) {

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
}
