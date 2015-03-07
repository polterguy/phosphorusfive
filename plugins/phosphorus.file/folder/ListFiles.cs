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
    ///     Class to help list all files within folder.
    /// </summary>
    public static class ListFiles
    {
        /// <summary>
        ///     List all files in folder(s).
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.folder.list-files")]
        private static void pf_folder_list_files (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each folder given by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                // iterating all files in current directory, and returning as nodes beneath args given
                foreach (var idxFile in Directory.GetFiles (rootFolder + idx)) {
                    // intentionally dropping "invisible linux 'backup' files"
                    if (!idxFile.EndsWith ("~")) {
                        // file is not a backup file on Linux
                        // normalizing file path delimiters for both Linux and Windows, before we return it 
                        // back to caller
                        var fileName = idxFile.Replace ("\\", "/");
                        fileName = fileName.Replace (rootFolder, "");
                        e.Args.Add (new Node (string.Empty, fileName));
                    }
                }
            }
        }
    }
}