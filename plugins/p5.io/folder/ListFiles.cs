/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using System.Linq;
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

            // Access rights invocation, to remove folders user doesn't have access to reading.
            Node access = null;
            if (context.Ticket.Role != "root") {
                access = new Node ();
                context.RaiseEvent ("p5.auth.access.list", access);

                // Eclusing everything not relevant to our current operation.
                foreach (var idx in access.Children.ToList ()) {
                    if (idx.Name != context.Ticket.Role || idx ["read-folder-deny"] == null)
                        idx.UnTie ();
                }

                // Checking if there's anything left in our access right object, and if not, deleting it entirely.
                if (access.Count == 0)
                    access = null;
            }

            // Checking if we've got a filter
            string filter = e.Args.GetExChildValue ("filter", context, "") ?? "";

            ObjectIterator.Iterate (context, e.Args, true, "read-folder", delegate (string foldername, string fullpath) {
                foreach (var idxFile in Directory.GetFiles (fullpath)) {
                    if (filter == "" || MatchFilter (idxFile, filter)) {

                        var fileName = idxFile.Replace ("\\", "/");
                        fileName = fileName.Replace (rootFolder, "");

                        // Checking if user has read access to folder's content.
                        var doAdd = true;
                        if (access != null) {
                            foreach (var idx in access [context.Ticket.Role].Children) {
                                if (idx.Name == "read-folder-deny" && fileName.StartsWithEx (idx.Get<string> (context))) {
                                    doAdd = false;
                                }
                            }
                        }
                        if (doAdd)
                            e.Args.Add (fileName);
                    }
                }
            });
        }

        /*
         * Helper for above.
         * Verifies the filename somehow matches one of the filters, defaulting to true, if filter is empty.
         */
        static bool MatchFilter (string filename, string filter)
        {
            bool retVal = filter == "";
            foreach (var idxFilter in filter.Split ('|')) {
                if (idxFilter.StartsWithEx (".")) {

                    // Filter is a file extension type of filter.
                    if (Path.GetExtension (filename).ToLower () == idxFilter.ToLower ()) {
                        retVal = true;
                        break;
                    }
                } else {

                    // Filter is anything but a file extension filter.
                    if (idxFilter.StartsWithEx ("~")) {

                        // Filter is a "like" type of filter.
                        if (Path.GetFileName (filename).Contains (idxFilter.Substring (1))) {
                            retVal = true;
                            break;
                        }
                    } else {

                        // Filter is an "exact match" type of filter.
                        if (Path.GetFileName (filename) == idxFilter) {
                            retVal = true;
                            break;
                        }
                    }
                }
            }
            return retVal;
        }
    }
}
