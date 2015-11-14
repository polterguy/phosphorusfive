/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Collections.Generic;
using p5.core;
using p5.exp;

namespace p5.file.file
{
    /// <summary>
    ///     Class to help load files.
    /// </summary>
    public static class Load
    {
        /// <summary>
        ///     Loads text-files from disc.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "load-file")]
        [ActiveEvent (Name = "load-text-file")]
        private static void file_load (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, false)) {

                // retrieving root folder of app
                var rootFolder = Common.GetRootFolder (context);

                // iterating through each file path given
                List<string> source = new List<string> (Common.GetSource (e.Args, context));
                if (source.Count == 0)
                    e.Args.Value = false;
                foreach (var idxFilename in source) {

                    // checking to see if file exists
                    if (File.Exists (rootFolder + idxFilename)) {

                        // file exists, loading it as text file, and appending text into node
                        // with filename as name, and content as value
                        using (TextReader reader = File.OpenText (rootFolder + idxFilename)) {

                            // reading file content
                            string fileContent = reader.ReadToEnd ();
                            if (idxFilename.EndsWith (".hl") && e.Args.GetExChildValue ("convert", context, true)) {

                                // automatically converting to Hyperlisp before returning
                                e.Args.Add (new Node (idxFilename, null, Utilities.Convert<Node> (fileContent, context).Children));
                                e.Args.Value = null;
                            } else {

                                // adding file content as string
                                e.Args.Add (new Node (idxFilename, fileContent));
                                e.Args.Value = null;
                            }
                        }
                    } else {

                        // file didn't exist, making sure we signal caller
                        e.Args.Add (new Node (idxFilename, false));
                    }
                }
            }
        }

        /// <summary>
        ///     Loads zero or more binary files from disc.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "load-binary-file")]
        private static void load_binary_file (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover args = new Utilities.ArgsRemover (e.Args, true)) {

                // retrieving root folder of app
                var rootFolder = Common.GetRootFolder (context);

                // iterating through each file path given
                foreach (var idxFilename in XUtil.Iterate<string> (e.Args, context)) {

                    // checking to see if file exists
                    if (File.Exists (rootFolder + idxFilename)) {

                        // file exists, loading it as text file, and appending text into node
                        // with filename as name, and content as value
                        using (FileStream stream = File.OpenRead (rootFolder + idxFilename)) {
                            byte[] buffer = new byte [stream.Length];
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
}
