/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.file.folder
{
    /// <summary>
    ///     Class to help remove folders from disc.
    /// </summary>
    public static class Remove
    {
        /// <summary>
        ///     Removes zero or more folders on disc.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.folder.remove")]
        private static void pf_folder_remove (ApplicationContext context, ActiveEventArgs e)
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