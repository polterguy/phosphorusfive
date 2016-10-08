/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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

using System;
using System.IO;
using System.Collections.Generic;
using p5.core;
using p5.exp.exceptions;
using p5.io.common;

/// <summary>
///     Main namespace for all IO folder operations in Phosphorus Five
/// </summary>
namespace p5.io.folder
{
    /// <summary>
    ///     Class to help copy a folder
    /// </summary>
    public static class Copy
    {
        /// <summary>
        ///     Copies a folder
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "copy-folder")]
        public static void copy_folder (ApplicationContext context, ActiveEventArgs e)
        {
            // Using our common helper for actual implementation
            MoveCopyHelper.CopyMoveFileObject (
                context, 
                e.Args, 
                "read-folder", 
                "modify-folder", 
                delegate (string rootFolder, string source, string destination) {

                // Actually moving (or renaming) folder
                CopyFolder (context, e.Args, GetSourceFileObjects (rootFolder + source, ""), rootFolder + source, rootFolder + destination);
            }, delegate (string destination) {
                return Directory.Exists (destination);
            });
        }

        /*
         * Helper for above, recursively traverses a folder, and copies it
         */
        private static void CopyFolder (
            ApplicationContext context,
            Node args,
            List<Tuple<string, bool>> sourceFileObjects, 
            string source, 
            string destination)
        {
            // Verifying currently traversed source folder exist
            var sourceFolder = new DirectoryInfo (source);
            if (!sourceFolder.Exists)
                throw new LambdaException (string.Format ("Source folder '{0}' could not be found", source), args, context);

            // Creating destination folder, if necessary
            var destinationFolder = new DirectoryInfo (destination);

            // Makes "merge" operations possible
            if (!destinationFolder.Exists)
                Directory.CreateDirectory (destinationFolder.FullName);

            // Looping through each folder and file, creating as we proceed, linearly
            foreach (var idxFileObj in sourceFileObjects) {
                if (idxFileObj.Item2) {

                    // Folder, verifying each path up to current exist
                    string[] entities = idxFileObj.Item1.Split (new char [] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                    string fullPath = "";
                    foreach (var idxFolder in entities) {

                        // Checking if currently iterated folder part exist
                        fullPath += idxFolder + "/";
                        if (!Directory.Exists (destination + fullPath))
                            Directory.CreateDirectory (destination + fullPath);
                    }
                } else {

                    // File, simply copying
                    File.Copy (source + idxFileObj.Item1, destination + idxFileObj.Item1);
                }
            }
        }

        /*
         * Helper for above, returns a list of folders and files. 
         * Folders are "true" in Item2 of Tuple
         */
        private static List<Tuple<string, bool>> GetSourceFileObjects (
            string rootFolder, 
            string source)
        {
            // Return value
            var retVal = new List<Tuple<string, bool>> ();

            // Looping through all files in current directory, and appending to return value
            foreach (FileInfo subdir in new DirectoryInfo (rootFolder + source).GetFiles ()) {

                // Adding currently iterated file to return value
                retVal.Add (new Tuple<string, bool> (subdir.FullName.Replace ("\\", "/").Replace (rootFolder, ""), false /* FILE */));
            }

            // Looping through all folders in current directory, and appending to return value
            foreach (DirectoryInfo subdir in new DirectoryInfo (rootFolder + source).GetDirectories ()) {

                // Adding currently iterated folder to return value
                retVal.Add (new Tuple<string, bool> (subdir.FullName.Replace ("\\", "/").Replace (rootFolder, "").Trim ('/') + "/", true /* FOLDER */));

                // Recursively invoking "self"
                retVal.AddRange (GetSourceFileObjects (rootFolder, subdir.FullName.Replace ("\\", "/").Replace (rootFolder, "").Trim ('/') + "/"));
            }

            // Returning list of files and folders
            return retVal;
        }
    }
}
