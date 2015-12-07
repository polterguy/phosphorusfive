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
using p5.exp;
using p5.core;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for all file operations in Phosphorus Five
/// </summary>
namespace p5.io.zip
{
    /// <summary>
    ///     Class to help GZip folders and files
    /// </summary>
    public static class GZip
    {
        /// <summary>
        ///     GZips folders and files
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "gzip", Protection = EventProtection.LambdaClosed)]
        private static void gzip (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[gzip] needs both a value and a [to] node.");

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Helpers.GetRootFolder (context);

                // Getting destination and verify path is correct according to conventions
                string destinationFile = XUtil.Single<string> (context, e.Args ["to"]);
                if (!destinationFile.StartsWith ("/"))
                    throw new LambdaException (
                        string.Format ("Destination file '{0}' was not a valid filename", destinationFile),
                        e.Args,
                        context);

                // Verifying user is authorized to writing to destination
                context.RaiseNative ("p5.io.authorize.save-file", new Node ("p5.io.authorize.save-file", destinationFile).Add ("args", e.Args));

                // Checking if file exist
                if (File.Exists (rootFolder + destinationFile)) {

                    // Destination file exist from before, creating a new unique destination filename
                    destinationFile = Helpers.CreateNewUniqueFileName (context, destinationFile);

                    // Verifying user is authorized to writing to updated destination
                    context.RaiseNative ("p5.io.authorize.save-file", new Node ("p5.io.authorize.save-file", destinationFile).Add ("args", e.Args));
                }

                // Making sure we are able to delete file, if an exception occurs!
                try
                {
                    // Creating our output stream, which will contain our GZip file
                    using (var output = File.Create (rootFolder + destinationFile)) {

                        // Creating our GZip stream, wrapping the file stream
                        using (var gzStream = new GZipOutputStream (output)) {

                            // Creating TAR archive, wrapping our GZip stream
                            using (var archive = TarArchive.CreateOutputTarArchive (gzStream)) {

                                // Looping through each input directory given
                                foreach (var idxSourceFileFolder in XUtil.Iterate<string> (context, e.Args)) {

                                    // Verify path is correct according to conventions
                                    if (!idxSourceFileFolder.StartsWith ("/"))
                                        throw new LambdaException (
                                            string.Format ("Source file/folder '{0}' was not a valid filename", idxSourceFileFolder),
                                            e.Args,
                                            context);

                                    // Verify path is correctly ending with a trailing slash "/" if object is a folder
                                    if (Directory.Exists (rootFolder + idxSourceFileFolder)) {
                                        if (!idxSourceFileFolder.EndsWith ("/"))
                                            throw new LambdaException (
                                                string.Format ("Source folder '{0}' was not a valid filename", idxSourceFileFolder),
                                                e.Args,
                                                context);
                                    }

                                    // Verifying user is authorized to reading from source
                                    context.RaiseNative ("p5.io.authorize.load-file", new Node ("p5.io.authorize.load-file", idxSourceFileFolder).Add ("args", e.Args));

                                    // Verifying file-/folder name is not a "restricted" type of file
                                    if (idxSourceFileFolder.StartsWith (".") || idxSourceFileFolder.EndsWith ("~"))
                                        continue;

                                    var rootSource = idxSourceFileFolder.Trim ('/');
                                    rootSource = rootSource.Substring (rootSource.LastIndexOf ("/") + 1);

                                    AddFileObjectToArchive (archive, rootFolder + idxSourceFileFolder.TrimEnd ('/'), rootSource);
                                }
                            }
                        }

                        // Returning filepath of ZIP file actually created to caller
                        e.Args.Value = destinationFile;
                    }
                }
                catch
                {
                    // Checking if file exist, and if so, delete it, before we rethrow exception
                    if (File.Exists (rootFolder + destinationFile))
                        File.Delete (rootFolder + destinationFile);
                    throw;
                }
            }
        }

        /// <summary>
        ///     Unzips folders and files
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "gunzip", Protection = EventProtection.LambdaClosed)]
        private static void gunzip (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[gunzip] needs both a value and a [to] node.");

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Helpers.GetRootFolder (context);

                // Getting destination folder
                var destinationFolder = "/" + e.Args ["to"].GetExValue<string> (context).Trim ('/') + "/";

                // Verifying user is authorized to writing to destination folder
                context.RaiseNative ("p5.io.authorize.save-folder", new Node ("p5.io.authorize.save-folder", destinationFolder).Add ("args", e.Args));

                // Looping through each source zip file given
                foreach (var idxZipFile in XUtil.Iterate<string> (context, e.Args)) {

                    // Verifying user is allowed to read from file given
                    context.RaiseNative ("p5.io.authorize.load-file", new Node ("p5.io.authorize.load-file", idxZipFile).Add ("args", e.Args));

                    // Creating our input stream, which wraps the GZip file given
                    using (var input = File.OpenRead (rootFolder + idxZipFile.TrimStart ('/'))) {

                        // Creating our GZip stream, wrapping the file stream
                        using (var gzStream = new GZipInputStream (input)) {

                            // Creating TAR archive, wrapping our GZip stream
                            using (var archive = TarArchive.CreateInputTarArchive (gzStream)) {

                                // Extracting archive to destination folder
                                archive.SetKeepOldFiles (false);
                                archive.ExtractContents (rootFolder + destinationFolder.TrimStart ('/'));
                            }
                        }
                    }

                    // Returning folder path of where files where unzipped to caller
                    e.Args.Value = destinationFolder;
                }
            }
        }

        /*
         * Helper for above [gzip] Active Event
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

                    // Verifying file-/folder name is not a "restricted" type of file
                    if (idxFile.StartsWith (".") || idxFile.EndsWith ("~"))
                        continue;

                    AddFileObjectToArchive (archive, idxFile, rootPath);
                }

                // Traversing each folder within directory, recursively invoking "self"
                foreach (var idxFolder in Directory.GetDirectories (fileFolderPath)) {

                    // Verifying file-/folder name is not a "restricted" type of file
                    if (idxFolder.StartsWith (".") || idxFolder.EndsWith ("~"))
                        continue;

                    AddFileObjectToArchive (archive, idxFolder, rootPath);
                }
            }
        }
    }
}
