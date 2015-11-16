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
    ///     Class to help rename and/or move folders.
    /// </summary>
    public static class Move
    {
        /// <summary>
        ///     Moves or renames a folder
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "move-folder")]
        private static void move_folder (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[move-file] needs both a value and a [to] node.");

            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // getting file to move
                string fromFolder = XUtil.Single<string> (e.Args, context);

                // Gettting new filename of file
                string toFolder = XUtil.Single<string> (e.Args ["to"], context);

                // Actually moving (or renaming) file
                Directory.Move (rootFolder + fromFolder, rootFolder + toFolder);
            }
        }
    }
}
