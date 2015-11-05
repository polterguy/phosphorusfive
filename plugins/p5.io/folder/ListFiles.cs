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
    ///     Class to help list all files within folder(s).
    /// 
    ///     Contains [list-files], and its associated helper methods.
    /// </summary>
    public static class ListFiles
    {
        /// <summary>
        ///     List all files in folder(s).
        /// 
        ///     Will list all files within the specified folder(s).
        /// 
        ///     Example that lists all files within root folder of your system;
        /// 
        ///     <pre>list-files:</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "list-files")]
        private static void list_files (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, true)) {

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
}
