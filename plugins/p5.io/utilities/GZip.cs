/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using p5.core;
using p5.exp;

namespace p5.file.utilities
{
    /// <summary>
    ///     Class to help GZip folder(s) and file(s)
    /// </summary>
    public static class GZip
    {
        /// <summary>
        ///     GZips folder(s) and file(s)
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "gzip")]
        private static void gzip (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[gzip-folder] needs both a value and a [to] node.");

            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Creating our output stream, which will contain our GZip file
                using (var output = File.Create (rootFolder + e.Args ["to"].GetExValue<string> (context))) {

                    // Creating our GZip stream, wrapping the file stream
                    using (var gzStream = new GZipOutputStream (output)) {

                        // Creating TAR archive, wrapping our GZip stream
                        using (var archive = TarArchive.CreateOutputTarArchive (gzStream)) {

                            // Looping through each input directory given
                            foreach (var idxSource in XUtil.Iterate<string> (e.Args, context)) {

                                var rootSource = idxSource.Trim ('/');
                                rootSource = rootSource.Substring (rootSource.LastIndexOf ("/") + 1);

                                AddFileObjectToArchive (archive, rootFolder + idxSource.TrimEnd ('/'), rootSource);
                            }
                        }
                    }

                    // Returning filepath of ZIP file to caller
                    e.Args.Value = e.Args ["to"].GetExValue<string> (context);
                }
            }
        }

        /*
         * Helper for above
         */
        private static void AddFileObjectToArchive (TarArchive archive, string fileFolderPath, string rootPath)
        {
            // Creating TAR entry for main object given
            var tarEntry = TarEntry.CreateEntryFromFile (fileFolderPath);
            tarEntry.Name = tarEntry.Name.Substring (tarEntry.Name.IndexOf (rootPath));
            archive.WriteEntry(tarEntry, false);

            // Checking if this is directory
            if (Directory.Exists (fileFolderPath)) {

                // Traversing each file within directory
                foreach (var idxFile in Directory.GetFiles (fileFolderPath)) {
                    AddFileObjectToArchive (archive, idxFile, rootPath);
                }

                // Traversing each folder within directory, recursively invoking "self"
                foreach (var idxFolder in Directory.GetDirectories (fileFolderPath)) {
                    AddFileObjectToArchive (archive, idxFolder, rootPath);
                }
            }
        }
    }
}
