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
    ///     Class to help create folders on disc.
    /// </summary>
    public static class Create
    {
        /// <summary>
        ///     Creates zero or more folders on disc. If folder exists from before, then false is returned.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.folder.create")]
        private static void pf_folder_create (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each folder caller wants to create
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                // checking to see if folder already exists, and if it does, return "false" to caller
                if (Directory.Exists (rootFolder + idx)) {
                    // folder already exists, returning that fact back to caller
                    e.Args.Add (new Node (idx, false));
                } else {
                    // folder didn't exist, creating it, and returning "true" back to caller, meaning "success"
                    Directory.CreateDirectory (rootFolder + idx);
                    e.Args.Add (new Node (idx, true));
                }
            }
        }
    }
}