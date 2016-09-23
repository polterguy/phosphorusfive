/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
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
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-files", Protection = EventProtection.LambdaClosed)]
        public static void list_files (ApplicationContext context, ActiveEventArgs e)
        {
            // Getting root folder
            var rootFolder = Common.GetRootFolder (context);

            // Checking if we've got a filter
            string filter = e.Args.GetExChildValue ("filter", context, "");

            QueryHelper.Run (context, e.Args, true, "read-folder", delegate (string foldername, string fullpath) {
                foreach (var idxFile in Directory.GetFiles (fullpath)) {
                    if (filter == "" || idxFile.EndsWith ("." + filter)) {
                        var fileName = idxFile.Replace ("\\", "/");
                        fileName = fileName.Replace (rootFolder, "");
                        e.Args.Add (fileName);
                    }
                }
                return true;
            });
        }
    }
}
