/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.file.file
{
    /// <summary>
    ///     Class to help remove files.
    /// 
    ///     Contains the [pf.file.remove] Active Event, and its associated helper methods.
    /// </summary>
    public static class Remove
    {
        /// <summary>
        ///     Removes zero or more files from disc.
        /// 
        ///     If file is successfully removed, true will be returned, otherwise false is returned.
        /// 
        ///     Example;
        /// 
        ///     <pre>pf.file.remove:foo.txt</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.file.remove")]
        private static void pf_file_remove (ApplicationContext context, ActiveEventArgs e)
        {
            // getting root folder
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each path given
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                if (File.Exists (rootFolder + idx)) {
                    // file exists, removing file and signaling caller
                    File.Delete (rootFolder + idx);
                    e.Args.Add (new Node (idx, true));
                } else {
                    // file does not exist, signaling caller
                    e.Args.Add (new Node (idx, false));
                }
            }
        }
    }
}
