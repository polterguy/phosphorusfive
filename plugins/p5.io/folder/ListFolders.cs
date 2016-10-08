/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
using p5.core;
using p5.io.common;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to help list all folders within folder
    /// </summary>
    public static class ListFolders
    {
        /// <summary>
        ///     List all folders in folder
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-folders")]
        public static void p5_folder_list_folders (ApplicationContext context, ActiveEventArgs e)
        {
            // Getting root folder
            var rootFolder = Common.GetRootFolder (context);

            ObjectIterator.Iterate (context, e.Args, true, "read-folder", delegate (string foldername, string fullpath) {
                foreach (var idxFolder in Directory.GetDirectories (rootFolder + foldername)) {
                    var folderName = idxFolder.Replace ("\\", "/");
                    folderName = folderName.Replace (rootFolder, "");
                    e.Args.Add (folderName.TrimEnd ('/') + "/");
                }
            });
        }
    }
}
