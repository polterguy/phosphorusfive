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

/// <summary>
///     Main namespace for all files and folders Active Events
/// </summary>
namespace p5.io.file
{
    /// <summary>
    ///     Class to help check if a file exists.
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Returns true if file(s) exists
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "file-exist", Protection = EventProtection.LambdaClosed)]
        private static void file_exist (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Retrieving source
                var sourceFiles = Common.GetSource (e.Args, context).ToList ();

                // Multiple filename source, returning existence of all files
                e.Args.Value = false;
                foreach (var idxFile in sourceFiles) {

                    // Verifying user is authorized to reading from currently iterated file
                    context.RaiseNative ("p5.io.authorize.load-file", new Node ("p5.io.authorize.load-file", idxFile).Add ("args", e.Args));

                    // Letting caller know whether or not this file exists
                    if (!File.Exists (rootFolder + idxFile)) {

                        // File didn't exist, letting caller know, and aborting early
                        e.Args.Value = false;
                        return;
                    } else {

                        // File existed
                        e.Args.Value = true;
                    }
                }
            }
        }
    }
}
