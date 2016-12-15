/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to help list all files within one or more folder(s).
    /// </summary>
    public static class ListFiles
    {
        /// <summary>
        ///     List all files in one or more folder(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-files")]
        [ActiveEvent (Name = "p5.io.folder.list-files")]
        public static void p5_io_folder_list_files (ApplicationContext context, ActiveEventArgs e)
        {
            // Getting root folder
            var rootFolder = Common.GetRootFolder (context);

            // Checking if we've got a filter
            string filter = e.Args.GetExChildValue ("filter", context, "") ?? "";

            ObjectIterator.Iterate (context, e.Args, true, "read-folder", delegate (string foldername, string fullpath) {
                foreach (var idxFile in Directory.GetFiles (fullpath)) {
                    if (filter == "" || MatchFilter (idxFile, filter)) {
                        var fileName = idxFile.Replace ("\\", "/");
                        fileName = fileName.Replace (rootFolder, "");
                        e.Args.Add (fileName);
                    }
                }
            });
        }

        /*
         * Helper for above
         */
        private static bool MatchFilter (string filename, string filter)
        {
            if (filter.StartsWith ("."))
                return Path.GetExtension (filename).ToLower () == filter.ToLower ();
            return Path.GetFileName (filename).Contains (filter);
        }
    }
}
