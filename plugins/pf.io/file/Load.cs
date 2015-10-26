/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using pf.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.file.file
{
    /// <summary>
    ///     Class to help load files.
    /// 
    ///     Contains [pf.file.load], [pf.file.binary.load], and their associated helper methods.
    /// </summary>
    public static class Load
    {
        /// <summary>
        ///     Loads zero or more text files from disc.
        /// 
        ///     If file does not exist, false will be returned for file path.
        /// 
        ///     Example that loads the file "foo.txt" from your "phosphorus.application-folder" if you run it through the main
        ///     web application;
        /// 
        ///     <pre>pf.file.load:foo.txt</pre>
        /// 
        ///     Example that loads the file "foo1.txt" and "foo2.txt" from main folder;
        /// 
        ///     <pre>_data
        ///   foo1.txt
        ///   foo2.txt
        /// pf.file.load:@/-/*name</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.file.load")]
        private static void pf_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder of app
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each file path given
            foreach (var idxFilename in XUtil.Iterate<string> (e.Args, context)) {
                // checking to see if file exists
                if (File.Exists (rootFolder + idxFilename)) {
                    // file exists, loading it as text file, and appending text into node
                    // with filename as name, and content as value
                    using (TextReader reader = File.OpenText (rootFolder + idxFilename)) {
                        e.Args.Add (new Node (idxFilename, reader.ReadToEnd ()));
                    }
                } else {
                    // file didn't exist, making sure we signal caller, by return a "false" node,
                    // where name of node is filename, and value is boolean false
                    e.Args.Add (new Node (idxFilename, false));
                }
            }
        }

        /// <summary>
        ///     Loads zero or more binary files from disc.
        /// 
        ///     If file does not exist, false will be returned for file path.
        /// 
        ///     Example that loads the file "foo.zip" from your "phosphorus.application-folder" if you run it through the main
        ///     web application;
        /// 
        ///     <pre>pf.file.binary.load:foo.zip</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.file.binary.load")]
        private static void pf_file_binary_load (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving root folder of app
            var rootFolder = Common.GetRootFolder (context);

            // iterating through each file path given
            foreach (var idxFilename in XUtil.Iterate<string> (e.Args, context)) {
                // checking to see if file exists
                if (File.Exists (rootFolder + idxFilename)) {
                    // file exists, loading it as text file, and appending text into node
                    // with filename as name, and content as value
                    using (FileStream stream = File.OpenRead (rootFolder + idxFilename)) {
                        byte [] buffer = new byte [stream.Length];
                        stream.Read (buffer, 0, buffer.Length);
                        e.Args.Add (new Node (idxFilename, buffer));
                    }
                } else {
                    // file didn't exist, making sure we signal caller, by return a "false" node,
                    // where name of node is filename, and value is boolean false
                    e.Args.Add (new Node (idxFilename, false));
                }
            }
        }
    }
}
