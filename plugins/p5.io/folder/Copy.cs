/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
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
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
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
                string sourceFolder = XUtil.Single<string> (e.Args, context);

                // Gettting new path of folder
                string destinationFolder = XUtil.Single<string> (e.Args ["to"], context);

                // Getting new foldername for folder, if needed
                if (Directory.Exists (rootFolder + destinationFolder)) {

                    // Destination folder exist from before, creating a new unique destination foldername
                    destinationFolder = Common.CreateNewUniqueFolderName (context, destinationFolder);
                }

                // Actually copying folder
                CopyFolder (rootFolder + sourceFolder, destinationFolder);

                // Returning actual destination foldername used to caller
                e.Args.Value = destinationFolder;
            }
        }

        /*
         * Helper for above, recursively traverses a folder, and copies it
         */
        private static void CopyFolder (string source, string destination)
        {
            DirectoryInfo sourceFolder = new DirectoryInfo (source);

            if (!sourceFolder.Exists)
                throw new DirectoryNotFoundException (
                    "Directory to copy could not be found: "
                    + source);

            if (!Directory.Exists (destination))
                Directory.CreateDirectory (destination);

            // Copying all files from currently traversed source 
            // folder, into currently traversed destination folder
            foreach (FileInfo file in sourceFolder.GetFiles ()) {
                file.CopyTo (Path.Combine (destination, file.Name), true);
            }

            // Looping through each sub folder in source folder, and copying it
            // to the destination
            foreach (DirectoryInfo subdir in sourceFolder.GetDirectories ()) {
                CopyFolder (subdir.FullName, Path.Combine (destination, subdir.Name));
            }
        }
    }
}
