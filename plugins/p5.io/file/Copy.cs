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
    ///     Class to help copy and/or rename a file
    /// </summary>
    public static class Copy
    {
        /// <summary>
        ///     Copies a file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "copy-file")]
        public static void copy_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Using our common helper for actual implementation
            MoveCopyHelper.CopyMoveFileObject (context, e, delegate (string rootFolder, string source, string destination) {

                // Verifying user is authorized to both modify source, and modify destination
                context.Raise (".p5.io.authorize.read-file", new Node ("", source).Add ("args", e.Args));
                context.Raise (".p5.io.authorize.modify-file", new Node ("", destination).Add ("args", e.Args));

                // Actually moving (or renaming) file
                File.Copy (rootFolder + source, rootFolder + destination);
            },
            delegate (string destination) {
                return File.Exists (destination);
            });
        }
    }
}
