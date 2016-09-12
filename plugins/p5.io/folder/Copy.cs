/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Collections.Generic;
using p5.exp;
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
        [ActiveEvent (Name = "copy-folder", Protection = EventProtection.LambdaClosed)]
        public static void copy_folder (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * We do not remove value of arguments here, since it is used for returning value of 
             * new foldername, since it might not necessarily be the same as the one caller requested, 
             * if folder exist from before
             */

            // Basic syntax checking
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[copy-folder] needs both a value and a [to] node.");

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Getting source and verify path is correct according to conventions
                string sourceFolder = XUtil.Single<string> (context, e.Args);

                // Getting destination and verify path is correct according to conventions
                string destinationFolder = XUtil.Single<string> (context, e.Args ["to"]);

                // Verifying user is authorized to both reading from source, and writing to destination
                context.RaiseNative ("p5.io.authorize.read-folder", new Node ("", sourceFolder).Add ("args", e.Args));
                context.RaiseNative ("p5.io.authorize.modify-folder", new Node ("", destinationFolder).Add ("args", e.Args));

                // Getting new foldername for folder, if needed
                if (Directory.Exists (rootFolder + destinationFolder)) {

                    // Destination folder exist from before, creating a new unique destination foldername
                    destinationFolder = Common.CreateNewUniqueFolderName (context, destinationFolder);

                    // Making sure user is authorized to writing to UPDATED folder
                    context.RaiseNative ("p5.io.authorize.modify-folder", new Node ("", destinationFolder).Add ("args", e.Args));
                }

                // Actually copying folder, getting source first, in case copying implies copy one
                // folder inside of itself, directly, or indirectly, which would create a never ending
                // recursive loop, unless we retrieve all source objects first
                CopyFolder (context, e.Args, GetSourceFileObjects (rootFolder + sourceFolder, ""), rootFolder + sourceFolder, rootFolder + destinationFolder);

                // Returning actual destination foldername used to caller
                e.Args.Value = destinationFolder;
            }
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
            var destinationFolder = new DirectoryInfo(destination);

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
         * Helper for above, returns a list of folders and files (folders are "true" in Item2 of Tuple)
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
                retVal.Add (new Tuple<string, bool> (subdir.FullName.Replace (rootFolder, ""), false /* FILE */));
            }

            // Looping through all folders in current directory, and appending to return value
            foreach (DirectoryInfo subdir in new DirectoryInfo (rootFolder + source).GetDirectories ()) {

                // Adding currently iterated folder to return value
                retVal.Add (new Tuple<string, bool> (subdir.FullName.Replace (rootFolder, "").Trim ('/') + "/", true /* FOLDER */));

                // Recursively invoking "self"
                retVal.AddRange (GetSourceFileObjects (rootFolder, subdir.FullName.Replace (rootFolder, "").Trim ('/') + "/"));
            }

            // Returning list of files and folders
            return retVal;
        }
    }
}
