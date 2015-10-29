/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for everything related to folders.
/// 
///     The Active Events within this namespace, allows you to create, remove, view, and change, your folders, and the contents of the folders, 
///     within your Phosphorus Five installation.
/// </summary>
namespace p5.file.folder
{
    /// <summary>
    ///     Class to help create folders on disc.
    /// 
    ///     Encapsulates the [p5.folder.create] Active Event, and its associated helper methods.
    /// </summary>
    public static class Create
    {
        /// <summary>
        ///     Creates zero or more folders on disc.
        /// 
        ///     If folder exists from before, then false is returned.
        /// 
        ///     Example that creates a "foo" folder, on root of your application;
        /// 
        ///     <pre>p5.folder.create:foo</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.folder.create")]
        private static void p5_folder_create (ApplicationContext context, ActiveEventArgs e)
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
