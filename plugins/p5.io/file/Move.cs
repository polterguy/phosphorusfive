/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using p5.core;
using p5.exp;

namespace p5.file.file
{
    /// <summary>
    ///     Class to help rename and/or move files.
    /// </summary>
    public static class Move
    {
        /// <summary>
        ///     Moves or renames a file
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "move-file")]
        private static void move_file (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[move-file] needs both a value and a [to] node.");

            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // getting file to move
                string sourceFile = XUtil.Single<string> (context, e.Args);

                // Getting new filename of file
                string destinationFile = XUtil.Single<string> (context, e.Args ["to"]);

                if (sourceFile == destinationFile)
                    return; // Nothing to do here

                // Getting new filename of file, if needed
                if (File.Exists (rootFolder + destinationFile)) {

                    // Destination file exist from before, creating a new unique destination filename
                    destinationFile = Common.CreateNewUniqueFileName (context, destinationFile);
                }

                // Actually moving (or renaming) file
                File.Move (rootFolder + sourceFile, rootFolder + destinationFile);

                // Returning actual destination filename used to caller
                e.Args.Value = destinationFile;
            }
        }
    }
}
