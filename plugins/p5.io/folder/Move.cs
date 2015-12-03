/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using p5.core;
using p5.exp;

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
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "move-folder", Protection = EventProtection.Lambda)]
        private static void move_folder (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[move-folder] needs both a value and a [to] node.");

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Getting folder to move
                string sourceFolder = "/" + XUtil.Single<string> (context, e.Args).Trim ('/') + "/";

                // Getting new destination of folder
                string destinationFolder = "/" + XUtil.Single<string> (context, e.Args ["to"]).Trim ('/') + "/";

                // Verifying user is authorized to both reading from source, and writing to destination
                context.RaiseNative ("_authorize-load-folder", new Node ("_authorize-load-folder", sourceFolder).Add ("args", e.Args));
                context.RaiseNative ("_authorize-save-folder", new Node ("_authorize-save-folder", destinationFolder).Add ("args", e.Args));

                // Aborting early if there's nothing to do here ...
                if (sourceFolder == destinationFolder)
                    return;

                // Getting new filename of file, if needed
                if (Directory.Exists (rootFolder + destinationFolder)) {

                    // Destination folder exist from before, creating a new unique destination foldername
                    destinationFolder = Common.CreateNewUniqueFolderName (context, destinationFolder);

                    // Authorizing for new folder name
                    context.RaiseNative ("_authorize-save-folder", new Node ("_authorize-save-folder", destinationFolder).Add ("args", e.Args));
                }

                // Actually moving (or renaming) folder
                Directory.Move (rootFolder + sourceFolder.TrimStart('/'), rootFolder + destinationFolder.TrimStart('/'));

                // Returning actual destination foldername used to caller
                e.Args.Value = destinationFolder;
            }
        }
    }
}
