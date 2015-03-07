/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
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
    ///     Class to help figure out if a folder exists on disc.
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Checks to see if a folder exists on disc or not, and returns true if folder exists.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.folder.exists")]
        private static void pf_folder_exists (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder first
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each folder the caller requests knowledge about
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                // appending whether or not the folder exists back to caller
                e.Args.Add (new Node (idx, Directory.Exists (rootFolder + idx)));
            }
        }
    }
}