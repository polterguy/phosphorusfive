/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
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
    ///     Encapsulates the [p5.folder.exists] Active Event, and its associated helper methods.
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Checks to see if a folder exists on disc or not.
        /// 
        ///     Will return "true" if folder exists, otherwise false.
        /// 
        ///     Example;
        ///     <pre>p5.folder.exists:foo</pre>
        /// 
        ///     Example checking for existence of multiple folders, "foo1" and "foo2";
        /// 
        ///     <pre>_data
        ///   foo1
        ///   foo2
        /// p5.folder.exists:@/-?name</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.folder.exists")]
        private static void p5_folder_exists (ApplicationContext context, ActiveEventArgs e)
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
