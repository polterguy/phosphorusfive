
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.file
{
    /// <summary>
    /// class to help remove files
    /// </summary>
    public static class file_remove
    {
        /// <summary>
        /// removes one or more files from the path given as value of args, which might be a constant, or
        /// an expression. all files that are successfully removed, will be returned as children nodes, 
        /// with path of file being name, and value being true. if file is not successfully removed, return
        /// value for that file will be false
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.remove")]
        private static void pf_file_remove (ApplicationContext context, ActiveEventArgs e)
        {
            // getting root folder
            string rootFolder = common.GetRootFolder (context);

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
