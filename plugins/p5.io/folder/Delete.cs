/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to help remove folders from disc
    /// </summary>
    public static class Remove
    {
        /// <summary>
        ///     Removes folders from disc
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-folder", Protection = EntranceProtection.Lambda)]
        private static void delete_folder (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving root folder
                var rootFolder = Common.GetRootFolder (context);

                // Iterating through each folder caller wants to create
                foreach (var idxFolder in Common.GetSource (e.Args, context)) {

                    // Verifying user is authorized to both reading from source, and writing to destination
                    context.Raise ("_authorize-save-folder", new Node ("_authorize-save-folder", idxFolder).Add ("args", e.Args));

                    // Checking to see if folder already exists
                    if (Directory.Exists (rootFolder + idxFolder)) {

                        // Folder exists, removing it recursively
                        Directory.Delete (rootFolder + idxFolder, true);
                    } else {

                        // Oops, folder didn't exist
                        throw new LambdaException (string.Format ("Tried to delete non-existing folder - '{0}'", idxFolder), e.Args, context);
                    }
                }
            }
        }
    }
}
