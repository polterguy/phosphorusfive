/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Collections.Generic;
using p5.core;
using p5.exp;

namespace p5.file.folder
{
    /// <summary>
    ///     Class to check if a folder exists on disc.
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Checks to see if a folder exists on disc or not.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "folder-exist")]
        private static void folder_exist (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // retrieving root folder first
                var rootFolder = Common.GetRootFolder (context);

                // retrieving source
                var source = new List<string> (Common.GetSource (e.Args, context));
                if (source.Count > 0) {

                    // multiple filename source, returning existence of each file as children nodes of args
                    // plus existence of all files as value of args
                    bool seenFalse = false;
                    foreach (var idx in source) {

                        // letting caller know whether or not this file exists
                        var fileExist = Directory.Exists (rootFolder + idx);
                        if (!fileExist)
                            seenFalse = true;
                        e.Args.Add (new Node (idx, fileExist));
                    }
                    e.Args.Value = !seenFalse;
                } else {

                    // probably expression leading into oblivian
                    e.Args.Value = false;
                }
            }
        }
    }
}
