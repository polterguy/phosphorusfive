/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.io.common;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to help list all files within folder
    /// </summary>
    public static class ListFiles
    {
        /// <summary>
        ///     List all files in folder
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-files", Protection = EventProtection.LambdaClosed)]
        private static void list_files (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving root folder
                var rootFolder = Common.GetRootFolder (context);

                // Checking if we've got a filter
                List<string> filters;
                if (e.Args ["filter"] != null) {

                    // We are given filter(s)
                    filters = XUtil.Iterate<string> (context, e.Args ["filter"]).ToList ();
                } else {

                    // No filters
                    filters = new List<string> ();
                }

                // Iterating through each folder supplied by caller
                foreach (var idxFolder in Common.GetSource (e.Args, context)) {

                    // Verifying user is authorized to reading from currently iterated folder
                    context.RaiseNative ("_authorize-load-folder", new Node ("_authorize-load-folder", idxFolder).Add ("args", e.Args));

                    // Iterating all files in current directory, and returning as nodes beneath args given
                    foreach (var idxFile in Directory.GetFiles (rootFolder + idxFolder)) {

                        // Intentionally dropping "invisible linux 'backup' files" and "invisible system files" on MAC
                        string[] splits = idxFile.Split (new char[] {'/'});
                        if (!idxFile.EndsWith ("~") && !splits[splits.Length - 1].StartsWith (".")) {

                            // File is not a backup file on Linux
                            // Normalizing file path delimiters for both Linux and Windows, before we return it 
                            // back to caller, but first verifying file matches filter given
                            if (filters.Count == 0 || filters.Where (ix => idxFile.EndsWith ("." + ix)).GetEnumerator ().MoveNext ()) {

                                // Returning filename back to caller
                                var fileName = idxFile.Replace ("\\", "/");
                                fileName = fileName.Replace (rootFolder, "").TrimStart ('/');
                                e.Args.Add (new Node (fileName));
                            }
                        }
                    }
                }
            }
        }
    }
}
