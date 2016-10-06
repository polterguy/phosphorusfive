/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to help rename and/or move folders
    /// </summary>
    public static class Move
    {
        /// <summary>
        ///     Moves or renames a folder
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "move-folder")]
        [ActiveEvent (Name = "rename-folder")]
        public static void move_rename_folder (ApplicationContext context, ActiveEventArgs e)
        {
            // Using our common helper for actual implementation
            MoveCopyHelper.CopyMoveFileObject (
                context, 
                e.Args, 
                "modify-folder", 
                "modify-folder", 
                delegate (string rootFolder, string source, string destination) {

                // Actually moving (or renaming) folder
                Directory.Move (rootFolder + source, rootFolder + destination);
            }, delegate (string destination) {
                return Directory.Exists (destination);
            });
        }
    }
}
