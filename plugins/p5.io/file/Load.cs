/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.file
{
    /// <summary>
    ///     Loads one or more file(s).
    /// </summary>
    public static class Load
    {
        /// <summary>
        ///     Loads one or more file(s) from local disc.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "load-file")]
        [ActiveEvent (Name = "p5.io.file.load")]
        public static void p5_io_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            ObjectIterator.Iterate (
                context, 
                e.Args, 
                true, 
                "read-file", 
                delegate (string filename, string fullpath) {
                    if (File.Exists (fullpath)) {

                        // Text files and binary files are loaded differently.
                        // Text file might for instance be converted automatically.
                        if (IsTextFile (filename))
                            LoadTextFile (context, e.Args, fullpath, filename);
                        else
                            LoadBinaryFile (e.Args, fullpath, filename);
                    } else {

                        // Oops, file didn't exist.
                        throw new LambdaException (
                            string.Format ("Couldn't find file '{0}'", filename),
                            e.Args,
                            context);
                    }
            });
        }

        /*
         * Determines if file is text according to the most common file extensions
         */
        static bool IsTextFile (string fileName)
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
         * Loads specified file as text and appends into args, possibly converting into lambda.
         */
        static void LoadTextFile (
            ApplicationContext context, 
            Node args, 
            string fullpath,
            string fileName)
        {
            using (TextReader reader = File.OpenText (fullpath)) {

                // Reading file content.
                string fileContent = reader.ReadToEnd ();

                // Checking if we should automatically convert file content to lambda.
                if (fileName.EndsWithEx (".hl") && args.GetExChildValue ("convert", context, true)) {

                    // Automatically converting to lambda before returning.
                    args.Add (fileName, null, Utilities.Convert<Node> (context, fileContent).Children);

                } else if (fileName.EndsWithEx (".csv") && args.GetExChildValue ("convert", context, true)) {

                    // Automatically converting to lambda before returning.
                    var csvLambda = new Node ("", fileContent);
                    context.RaiseEvent ("p5.csv.csv2lambda", csvLambda);
                    args.Add (fileName, null, csvLambda["result"].Children);

                } else {

                    // Adding file content as string.
                    args.Add (fileName, fileContent);
                }
            }
        }

        /*
         * Loads a binary file and appends as blob/byte[] into args.
         */
        static void LoadBinaryFile (
            Node args,
            string fullpath,
            string filename)
        {
            using (FileStream stream = File.OpenRead (fullpath)) {

                // Reading file content
                var buffer = new byte [stream.Length];
                stream.Read (buffer, 0, buffer.Length);
                args.Add (filename, buffer);
            }
        }
    }
}
