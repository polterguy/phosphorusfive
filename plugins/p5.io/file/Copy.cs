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
using p5.exp;
using p5.core;
using p5.io.common;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help copy one or more file(s).
    /// </summary>
    public static class Copy
    {
        /// <summary>
        ///     Copies one or more file(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "copy-file")]
        [ActiveEvent (Name = "p5.io.file.copy")]
        public static void p5_io_copy_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking uf user supplied an [overwrite] argument, and using its value to ignore File.Exists, if supplied.
            var overwrite = e.Args.GetExChildValue ("overwrite", context, false);
            e.Args ["overwrite"]?.UnTie ();

            // Using our common helper for actual implementation.
            MoveCopyHelper.CopyMoveFileObject (
                context,
                e.Args,
                "read-file",
                "modify-file",
                delegate (string rootFolder, string source, string destination) {
                    File.Copy (rootFolder + source, rootFolder + destination);
                },
                delegate (string filename) {
                    if (overwrite && File.Exists (filename)) {
                        File.Delete (filename);
                        return false;
                    }
                    return File.Exists (filename);
                });
        }
    }
}
