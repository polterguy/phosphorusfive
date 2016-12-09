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

namespace p5.io.file
{
    /// <summary>
    ///     Moves or renames one or more file(s).
    /// </summary>
    public static class Move
    {
        /// <summary>
        ///     Moves or renames one or more file(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.io.file.move")]
        [ActiveEvent (Name = "p5.io.file.rename")]
        public static void p5_io_file_move_rename (ApplicationContext context, ActiveEventArgs e)
        {
            // Using our common helper for actual implementation.
            MoveCopyHelper.CopyMoveFileObject (
                context, 
                e.Args, 
                "modify-file", 
                "modify-file", 
                delegate (string rootFolder, string source, string destination) {

                // Actually moving (or renaming) file.
                File.Move (rootFolder + source, rootFolder + destination);
            }, delegate (string destination) {
                return File.Exists (destination);
            });
        }
    }
}
