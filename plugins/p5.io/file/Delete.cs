/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.exp;

namespace p5.file.file
{
    /// <summary>
    ///     Class to help remove files.
    /// </summary>
    public static class Delete
    {
        /// <summary>
        ///     Removes files from disc.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "delete-file")]
        private static void delete_file (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args)) {

                // getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // iterating through each path given
                bool? allDeleted = new bool? (); // used to keep track of whether or not we successfully deleted all files
                foreach (var idx in Common.GetSource (e.Args, context)) {

                    if (!allDeleted.HasValue)
                        allDeleted = true;

                    // checking if file exist
                    if (File.Exists (rootFolder + idx)) {

                        // file exists, removing file and signaling caller
                        File.Delete (rootFolder + idx);
                        e.Args.Add (new Node (idx, true));
                    } else {

                        // file does not exist, signaling caller
                        e.Args.Add (new Node (idx, false));
                        allDeleted = false;
                    }
                }
                if (!allDeleted.HasValue || !allDeleted.Value)
                    e.Args.Value = false;
                else
                    e.Args.Value = true;
            }
        }
    }
}
