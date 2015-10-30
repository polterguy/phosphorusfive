/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.exp;

namespace p5.file.folder
{
    /// <summary>
    ///     Class to help remove folders from disc.
    /// 
    ///     Contains [p5.folder.remove], and its associated helper methods.
    /// </summary>
    public static class Remove
    {
        /// <summary>
        ///     Removes zero or more folders from disc.
        /// 
        ///     Will recursively remove folder, and all of its contents. If removal operation is successful, this
        ///     Active Event will return "true" for the successfully removed folder(s), otherwise "false".
        /// 
        ///     Example that removes the folder "foo" from root;
        /// 
        ///     <pre>p5.folder.remove:foo</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.folder.remove")]
        private static void p5_folder_remove (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each folder caller wants to create
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                if (Directory.Exists (rootFolder + idx)) {
                    // folder exists, removing it recursively,
                    // and returning success back to caller
                    Directory.Delete (rootFolder + idx, true);
                    e.Args.Add (new Node (idx, true));
                } else {
                    // folder didn't exist, returning that fact back to caller
                    e.Args.Add (new Node (idx, false));
                }
            }
        }
    }
}
