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
    ///     Class to help copy a file
    /// </summary>
    public static class Copy
    {
        /// <summary>
        ///     Copies a file
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "copy-file")]
        private static void copy_file (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[copy-file] needs both a value and a [to] node.");

            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Getting source
                string sourceFile = XUtil.Single<string> (context, e.Args);

                // Getting destination
                string destinationFile = XUtil.Single<string> (context, e.Args ["to"]);

                // Getting new filename of file, if needed
                if (File.Exists (rootFolder + destinationFile)) {

                    // Destination file exist from before, creating a new unique destination filename
                    destinationFile = Common.CreateNewUniqueFileName (context, destinationFile);
                }

                // Actually moving (or renaming) file
                File.Copy (rootFolder + sourceFile, rootFolder + destinationFile);

                // Returning actual destination filename used to caller
                e.Args.Value = destinationFile;
            }
        }
    }
}
