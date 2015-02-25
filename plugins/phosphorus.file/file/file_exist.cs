
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.file
{
    /// <summary>
    /// class to help load and save files
    /// </summary>
    public static class file_exist
    {
        /// <summary>
        /// returns true for each file in constant or expression of path given as args that exists
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.exists")]
        private static void pf_file_exists (ApplicationContext context, ActiveEventArgs e)
        {
            // finding root folder
            var rootFolder = common.GetRootFolder (context);

            // iterating through each filepath given
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // letting caller know whether or not this file exists
                e.Args.Add (new Node (idx, File.Exists (rootFolder + idx)));
            }
        }
    }
}
