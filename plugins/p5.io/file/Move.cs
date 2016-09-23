/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.io.common;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help rename and/or move files
    /// </summary>
    public static class Move
    {
        /// <summary>
        ///     Moves or renames a file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "move-file", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "rename-file", Protection = EventProtection.LambdaClosed)]
        public static void move_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Using our common helper for actual implementation
            MoveCopyHelper.CopyMoveFileObject (context, e, delegate (string rootFolder, string source, string destination) {

                // Verifying user is authorized to both modify source, and modify destination
                context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", source).Add ("args", e.Args));
                context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", destination).Add ("args", e.Args));

                // Actually moving (or renaming) file
                File.Move (rootFolder + source, rootFolder + destination);

                // Making sure we return the filename as the value of root node, in case a new filename was created
                e.Args.Value = destination;
            }, delegate (string destination) {
                return Common.CreateNewUniqueFileName (context, destination);
            }, delegate (string destination) {
                return File.Exists (destination);
            });
        }
    }
}
