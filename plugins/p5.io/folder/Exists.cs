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
    ///     Class to check if a folder exists on disc.
    /// 
    ///     Encapsulates the [folder-exist] Active Event, and its associated helper methods.
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Checks to see if a folder exists on disc or not.
        /// 
        ///     Will return "true" if folder exists, otherwise false.
        /// 
        ///     Example;
        ///     <pre>folder-exist:foo</pre>
        /// 
        ///     Example checking for existence of multiple folders, "foo1" and "foo2";
        /// 
        ///     <pre>_data
        ///   foo1
        ///   foo2
        /// folder-exist:@/-?name</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "folder-exist")]
        private static void folder_exist (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, true)) {

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
}
