/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp;

namespace p5.file.folder
{
    /// <summary>
    ///     Class to help copy a folder
    /// </summary>
    public static class Copy
    {
        /// <summary>
        ///     Copies a folder
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "copy-folder")]
        private static void copy_folder (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[copy-folder] needs both a value and a [to] node.");

            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // getting folder to copy
                string sourceFolder = XUtil.Single<string> (context, e.Args).Trim ('/') + "/";

                // Gettting new path of folder
                string destinationFolder = XUtil.Single<string> (context, e.Args ["to"]).Trim ('/') + "/";

                // Getting new foldername for folder, if needed
                if (Directory.Exists (rootFolder + destinationFolder)) {

                    // Destination folder exist from before, creating a new unique destination foldername
                    destinationFolder = Common.CreateNewUniqueFolderName (context, destinationFolder);
                }

                // Actually copying folder, getting source first, in case copying implies copy one
                // folder inside of itself, directly, or indirectly, which would create a never ending
                // recursive loop, unless we retrieve all source objects first
                CopyFolder (GetSourceFileObjects (rootFolder + sourceFolder, ""), rootFolder + sourceFolder, rootFolder + destinationFolder);

                // Returning actual destination foldername used to caller
                e.Args.Value = destinationFolder;
            }
        }

        /*
         * Helper for above, recursively traverses a folder, and copies it
         */
        private static void CopyFolder (List<Tuple<string, bool>> sourceFileObjects, string source, string destination)
        {
            // Verifying currently traversed source folder exist
            var sourceFolder = new DirectoryInfo (source);
            if (!sourceFolder.Exists)
                throw new DirectoryNotFoundException (
                    "Folder to copy could not be found: "
                    + source);

            // Creating destination folder, if necessary
            var destinationFolder = new DirectoryInfo(destination);
            if (!destinationFolder.Exists) // Makes "merge" operations possible
                Directory.CreateDirectory (destinationFolder.FullName);

            // Looping through each folder and file, creating as we proceed, linearly
            foreach (var idxFileObj in sourceFileObjects) {
                if (idxFileObj.Item2) {

                    // Folder, verifying each path up to current exist
                    string[] entities = idxFileObj.Item1.Split (new char [] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                    string fullPath = "";
                    foreach (var idxFolder in entities) {

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
         * Helper for above, returns a list of folders and files (folders are "true" in Item2 of Tuple)
         */
        private static List<Tuple<string, bool>> GetSourceFileObjects (string rootFolder, string source)
        {
            List<Tuple<string, bool>> retVal = new List<Tuple<string, bool>> ();

            // Looping through all files in current directory, and appending to return value
            foreach (FileInfo subdir in new DirectoryInfo (rootFolder + source).GetFiles ()) {
                retVal.Add (new Tuple<string, bool> (subdir.FullName.Replace (rootFolder, ""), false));
            }

            // Looping through all folders in current directory, and appending to return value
            foreach (DirectoryInfo subdir in new DirectoryInfo (rootFolder + source).GetDirectories ()) {
                retVal.Add (new Tuple<string, bool> (subdir.FullName.Replace (rootFolder, "").Trim ('/') + "/", true));

                // Recursively invoking "self"
                retVal.AddRange (GetSourceFileObjects (rootFolder, subdir.FullName.Replace (rootFolder, "").Trim ('/') + "/"));
            }

            // Returning list of files and folders
            return retVal;
        }
    }
}
