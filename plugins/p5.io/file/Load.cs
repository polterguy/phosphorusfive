/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Collections.Generic;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help load files
    /// </summary>
    public static class Load
    {
        /// <summary>
        ///     Loads text files from disc
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "load-file")]
        [ActiveEvent (Name = "load-text-file")]
        private static void file_text_load (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving root folder of app
                var rootFolder = Common.GetRootFolder (context);

                // Iterating through each file path given
                foreach (var idxFilename in Common.GetSource (e.Args, context)) {

                    // Verifying user is authorized to reading from currently iterated file
                    context.Raise ("_authorize-load-file", new Node ("_authorize-load-file", idxFilename).Add ("args", e.Args));

                    // Checking to see if file exists
                    if (File.Exists (rootFolder + idxFilename)) {

                        // File exists, loading it as text file, and appending text into node,
                        // with filename as name, and content as value
                        using (TextReader reader = File.OpenText (rootFolder + idxFilename)) {

                            // Reading file content
                            string fileContent = reader.ReadToEnd ();
                            if (idxFilename.EndsWith (".hl") && e.Args.GetExChildValue ("convert", context, true)) {

                                // Automatically converting to Hyperlisp before returning
                                e.Args.Add (new Node (idxFilename, null, Utilities.Convert<Node> (context, fileContent).Children));
                            } else {

                                // Adding file content as string
                                e.Args.Add (new Node (idxFilename, fileContent));
                            }
                        }
                    } else {

                        // File didn't exist
                        throw new LambdaException (string.Format ("Couldn't find file '{0}'", idxFilename), e.Args, context);
                    }
                }
            }
        }

        /// <summary>
        ///     Loads zero or more binary files from disc
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "load-binary-file")]
        private static void load_binary_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving root folder of app
                var rootFolder = Common.GetRootFolder (context);

                // Iterating through each file path given
                foreach (var idxFilename in XUtil.Iterate<string> (context, e.Args)) {

                    // Verifying user is authorized to reading from currently iterated file
                    context.Raise ("_authorize-load-file", new Node ("_authorize-load-file", idxFilename).Add ("args", e.Args));

                    // Checking to see if file exists
                    if (File.Exists (rootFolder + idxFilename)) {

                        // File exists, loading it as text file, and appending text into node
                        // with filename as name, and content as value
                        using (FileStream stream = File.OpenRead (rootFolder + idxFilename)) {

                            // Loading binary content
                            byte[] buffer = new byte [stream.Length];
                            stream.Read (buffer, 0, buffer.Length);
                            e.Args.Add (new Node (idxFilename, buffer));
                        }
                    } else {

                        // File didn't exist
                        throw new LambdaException (string.Format ("Couldn't find file '{0}'", idxFilename), e.Args, context);
                    }
                }
            }
        }
    }
}
