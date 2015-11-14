/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;

namespace p5.file.folder
{
    /// <summary>
    ///     Class to help list all files within folder(s).
    /// </summary>
    public static class ListFiles
    {
        /// <summary>
        ///     List all files in folder(s).
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

                // checking if we've got a filter
                List<string> filters;
                if (e.Args ["filter"] != null) {

                    // we're given filters
                    filters = new List<string> (new List<string> (XUtil.Iterate<string> (e.Args ["filter"], context)));
                } else {

                    // no filters
                    filters = new List<string> ();
                }

                // iterating through each folder given by caller
                foreach (var idx in Common.GetSource (e.Args, context)) {

                    // iterating all files in current directory, and returning as nodes beneath args given
                    foreach (var idxFile in Directory.GetFiles (rootFolder + idx)) {

                        // intentionally dropping "invisible linux 'backup' files"
                        if (!idxFile.EndsWith ("~") && !idxFile.StartsWith (".")) {

                            // file is not a backup file on Linux
                            // normalizing file path delimiters for both Linux and Windows, before we return it 
                            // back to caller, but first verifying file matches filter given
                            if (filters.Count == 0 || filters.Where (ix => idxFile.EndsWith ("." + ix)).GetEnumerator ().MoveNext ()) {

                                // returning filename back to caller
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
}
