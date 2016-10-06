/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help load files
    /// </summary>
    public static class Load
    {
        /// <summary>
        ///     Loads files from disc
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "load-file")]
        public static void file_load (ApplicationContext context, ActiveEventArgs e)
        {
            ObjectIterator.Iterate (context, e.Args, true, "read-file", delegate (string filename, string fullpath) {
                if (File.Exists (fullpath)) {
                    if (IsTextFile (filename)) {
                        LoadTextFile (context, e.Args, fullpath, filename);
                    } else {
                        LoadBinaryFile (e.Args, fullpath, filename);
                    }
                } else {
                    throw new LambdaException (
                        string.Format ("Couldn't find file '{0}'", filename),
                        e.Args,
                        context);
                }
            });
        }

        /*
         * Determines if file is text according to file extension
         */
        private static bool IsTextFile (string fileName)
        {
            switch (Path.GetExtension (fileName)) {
                case ".txt":
                case ".md":
                case ".css":
                case ".js":
                case ".html":
                case ".htm":
                case ".hl":
                case ".xml":
                case ".csv":
                    return true;
                default:
                    return false;
            }
        }

        /*
         * Loads specified as text and appends into args
         */
        private static void LoadTextFile (
            ApplicationContext context, 
            Node args, 
            string fullpath,
            string fileName)
        {
            using (TextReader reader = File.OpenText (fullpath)) {

                // Reading file content
                string fileContent = reader.ReadToEnd ();

                // Checking if we should automatically convert file content to lambda
                if (fileName.EndsWith (".hl") && args.GetExChildValue ("convert", context, true)) {

                    // Automatically converting to Hyperlambda before returning
                    args.Add (fileName, null, Utilities.Convert<Node> (context, fileContent).Children);
                }
                else {

                    // Adding file content as string
                    args.Add (fileName, fileContent);
                }
            }
        }

        /*
         * Loads a binary file and appends as blob/byte[] into args
         */
        private static void LoadBinaryFile (
            Node args, 
            string rootFolder,
            string fileName)
        {
            using (FileStream stream = File.OpenRead (rootFolder + fileName)) {

                // Reading file content
                var buffer = new byte [stream.Length];
                stream.Read (buffer, 0, buffer.Length);
                args.Add (fileName, buffer);
            }
        }
    }
}
