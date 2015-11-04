/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.exp;

namespace p5.file.file
{
    /// <summary>
    ///     Class to help load files.
    /// 
    ///     Contains [load-file], [p5.file.binary.load], and their associated helper methods.
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
        ///     <pre>load-file:foo.txt</pre>
        /// 
        ///     Example that loads the file "foo1.txt" and "foo2.txt" from main folder;
        /// 
        ///     <pre>_data
        ///   foo1.txt
        ///   foo2.txt
        /// load-file:@/-/*name</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "load-file")]
        private static void file_load (ApplicationContext context, ActiveEventArgs e)
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
                        string fileContent = reader.ReadToEnd ();
                        if (idxFilename.EndsWith (".hl")) {

                            // automatically converting to Hyperlisp before returning
                            var convertNode = new Node (string.Empty, fileContent);
                            e.Args.Add (new Node (idxFilename, null, context.Raise ("lisp2lambda", convertNode).Children));
                        } else {

                            // adding file content as string
                            e.Args.Add (new Node (idxFilename, fileContent));
                        }
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
        ///     <pre>p5.file.binary.load:foo.zip</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "load-binary-file")]
        private static void load_binary_file (ApplicationContext context, ActiveEventArgs e)
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
