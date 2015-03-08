/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

/// <summary>
///     Main namespace for all [pf.file.xxx] Active Events.
/// 
///     Contains all Active Events within the [pf.file.xxx] namespace, allowing for creation, deletion, and manipulation
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
namespace phosphorus.file.file
{
    /// <summary>
    ///     Class to help check if a file exists.
    /// 
    ///     Contains the [pf.file.exists] Active Event, and its associated helper methods.
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Returns true if file(s) exists.
        /// 
        ///     Will return the value "true" if file(s) exists, otherwise false.
        /// 
        ///     Example;
        ///     <pre>pf.file.exists:foo.txt</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.file.exists")]
        private static void pf_file_exists (ApplicationContext context, ActiveEventArgs e)
        {
            // finding root folder
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each filepath given
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                // letting caller know whether or not this file exists
                e.Args.Add (new Node (idx, File.Exists (rootFolder + idx)));
            }
        }
    }
}
