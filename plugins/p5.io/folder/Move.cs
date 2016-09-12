/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;

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
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "move-folder", Protection = EventProtection.LambdaClosed)]
        public static void move_folder (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * We do not remove value of arguments here, since it is used for returning value of 
             * new foldername, since it might not necessarily be the same as the one caller requested, 
             * if folder exist from before
             */

            // Basic syntax checking
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[move-folder] needs both a value and a [to] node.");

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

                // Aborting early if there's nothing to do here ...
                if (sourceFolder == destinationFolder)
                    return;

                // Getting new filename of file, if needed
                if (Directory.Exists (rootFolder + destinationFolder)) {

                    // Destination folder exist from before, creating a new unique destination foldername
                    destinationFolder = Common.CreateNewUniqueFolderName (context, destinationFolder);

                    // Authorizing for new folder name
                    context.RaiseNative ("p5.io.authorize.modify-folder", new Node ("", destinationFolder).Add ("args", e.Args));
                }

                // Actually moving (or renaming) folder
                Directory.Move (rootFolder + sourceFolder, rootFolder + destinationFolder);

                // Returning actual destination foldername used to caller
                e.Args.Value = destinationFolder;
            }
        }
    }
}
