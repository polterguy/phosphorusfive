/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp;
using p5.io.common;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to check if a folder exists on disc
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Checks to see if a folder exists on disc or not
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "folder-exist", Protection = EventProtection.LambdaClosed)]
        private static void folder_exist (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Retrieving source
                var sourceFiles = Common.GetSource (e.Args, context).ToList ();

                // Multiple folder source, returning existence of all folders
                e.Args.Value = false;
                foreach (var idxFolder in sourceFiles) {

                    // Verifying user is authorized to reading from currently iterated folder
                    context.RaiseNative ("p5.io.authorize.load-folder", new Node ("p5.io.authorize.load-folder", idxFolder).Add ("args", e.Args));

                    // Letting caller know whether or not this file exists
                    if (!Directory.Exists (rootFolder + idxFolder)) {

                        // Folder didn't exist, letting caller know, and aborting early
                        e.Args.Value = false;
                        return;
                    } else {

                        // Folder existed
                        e.Args.Value = true;
                    }
                }
            }
        }
    }
}
