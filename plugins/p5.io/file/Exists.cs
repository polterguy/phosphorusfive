/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for all [p5.file.xxx] Active Events.
/// 
///     Contains all Active Events within the [p5.file.xxx] namespace, allowing for creation, deletion, and manipulation
///     of files, on your local file system.
/// 
///     All of these Active Events can take <see cref="phosphorus.expressions.Expression">Expressions</see> as their
///     values. This means you can for instance load, remove or save multiple files at the same time, with one single
///     Active Event invocation.
/// 
///     These Active Events also returns a node hierarchy, containing the file/folder names as the name of the nodes, and
///     whatever result the Active Event returns beneath that node. This is because since these Active Events can take multiple
///     arguments, through expressions, they must be able to return values, relative to their specifically handled argument.
/// </summary>
namespace p5.file.file
{
    /// <summary>
    ///     Class to help check if a file exists.
    /// 
    ///     Contains the [Value] Active Event, and its associated helper methods.
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Returns true if file(s) exists.
        /// 
        ///     Will return the value "true" if file(s) exists, otherwise false.
        /// 
        ///     Example;
        ///     <pre>Value:foo.txt</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "file-exist")]
        private static void file_exist (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args)) {

                // finding root folder
                var rootFolder = Common.GetRootFolder (context);

                // iterating through each filepath given
                foreach (var idx in Common.GetSource (e.Args, context)) {

                    // letting caller know whether or not this file exists
                    e.Args.Add (new Node (idx, File.Exists (rootFolder + idx)));
                }
            }
        }
    }
}
