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
using p5.core;
using p5.io.common;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to check if a folder exists on disc
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Checks to see if a folder exists on disc or not
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "folder-exist")]
        public static void folder_exist (ApplicationContext context, ActiveEventArgs e)
        {
            ObjectIterator.Iterate (context, e.Args, true, "read-folder", delegate (string foldername, string fullpath) {
                e.Args.Add (foldername, Directory.Exists (fullpath));
            });
        }
    }
}
