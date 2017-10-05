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

using System;
using System.IO;
using System.Collections.Generic;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to help copy one or more folder(s).
    /// </summary>
    public static class Copy
    {
        /// <summary>
        ///     Copies one or more folder(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "copy-folder")]
        [ActiveEvent (Name = "p5.io.folder.copy")]
        public static void p5_io_folder_copy (ApplicationContext context, ActiveEventArgs e)
        {
            // Using our common helper for actual implementation.
            MoveCopyHelper.CopyMoveFileObject (
                context,
                e.Args,
                "read-folder",
                "modify-folder",
                delegate (string rootFolder, string source, string destination) {
                    CopyFolder (
                        context,
                        e.Args,
                        GetSourceFileObjects (rootFolder + source, ""),
                        rootFolder + source,
                        rootFolder + destination);
                }, Directory.Exists);
        }

        /*
         * Helper for above, creates all folders, and copies all files.
         * When this method is invoked, our "sourceFileObjects" list will contain all folders and all files we should copy.
         */
        static void CopyFolder (
            ApplicationContext context,
            Node args,
            List<Tuple<string, bool>> sourceFileObjects,
            string sourceFolder,
            string destinationFolder)
        {
            // Verifying currently traversed source folder exists.
            if (!new DirectoryInfo (sourceFolder).Exists)
                throw new LambdaException (string.Format ("Source folder '{0}' could not be found", sourceFolder), args, context);

            // Creating destination folder, if necessary.
            if (!new DirectoryInfo (destinationFolder).Exists)
                Directory.CreateDirectory (destinationFolder);

            // Looping through each folder and file, creating destination folders if necessary, as we proceed.
            foreach (var idxFileObj in sourceFileObjects) {
                if (idxFileObj.Item2) {

                    // Currently iterated object is a folder, verifying each folder up to current exist.
                    string [] entities = idxFileObj.Item1.Split (new char [] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    string fullPath = "";
                    foreach (var idxFolder in entities) {

                        // Checking if currently iterated folder part exist.
                        fullPath += idxFolder + "/";
                        if (!Directory.Exists (destinationFolder + fullPath))
                            Directory.CreateDirectory (destinationFolder + fullPath);
                    }
                } else {

                    // File, simply copying into destination.
                    File.Copy (sourceFolder + idxFileObj.Item1, destinationFolder + idxFileObj.Item1);
                }
            }
        }

        /*
         * Helper for above, returns a list of folders and files. 
         * Folders are "true" in Item2 of Tuple.
         */
        static List<Tuple<string, bool>> GetSourceFileObjects (
            string rootFolder,
            string source)
        {
            // Creating our return value.
            var retVal = new List<Tuple<string, bool>> ();

            // Looping through all files in current directory, and appending to return value.
            foreach (FileInfo subdir in new DirectoryInfo (rootFolder + source).GetFiles ()) {
                retVal.Add (new Tuple<string, bool> (subdir.FullName.Replace ("\\", "/").Replace (rootFolder, ""), false /* FILE */));
            }

            // Looping through all folders in current directory, and appending to return value, making sure we exchange "\" with "/".
            foreach (DirectoryInfo subdir in new DirectoryInfo (rootFolder + source).GetDirectories ()) {

                // Adding currently iterated folder to return value, exchanging "\" with "/".
                retVal.Add (new Tuple<string, bool> (subdir.FullName.Replace ("\\", "/").Replace (rootFolder, "").Trim ('/') + "/", true /* FOLDER */));

                // Recursively invoking "self", to get all folders and files inside of currently iterated folder.
                retVal.AddRange (GetSourceFileObjects (rootFolder, subdir.FullName.Replace ("\\", "/").Replace (rootFolder, "").Trim ('/') + "/"));
            }

            // Returning list of files and folders, which now should contain all folders and files recursively from what we started out with.
            return retVal;
        }
    }
}
