/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.io.zip.helpers;

/// <summary>
///     Main namespace for all ZIP operations in Phosphorus Five
/// </summary>
namespace p5.io.zip
{
    /// <summary>
    ///     Class to help unzip folders and files
    /// </summary>
    public static class UnZipper
    {
        /// <summary>
        ///     Unzips folders and files
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "unzip", Protection = EventProtection.LambdaClosed)]
        private static void unzip (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args["to"] == null)
                throw new ArgumentException ("[unzip] needs both a value and a [to] node.");

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Helpers.GetBaseFolder (context);

                // Getting destination folder
                var destinationFolder = e.Args ["to"].GetExValue<string> (context);
                if (destinationFolder [0] != '/' || destinationFolder [destinationFolder.Length - 1] != '/')
                    throw new LambdaException ("Destination folder was not a valid foldername", e.Args, context);

                // Verifying user is authorized to writing to destination folder
                context.RaiseNative ("p5.io.authorize.modify-folder", new Node ("", destinationFolder).Add ("args", e.Args));

                // Looping through each source zip file given
                foreach (var idxZipFile in XUtil.Iterate<string> (context, e.Args)) {

                    // Verifying user is allowed to read from file given
                    context.RaiseNative ("p5.io.authorize.read-file", new Node ("", idxZipFile).Add ("args", e.Args));

                    // Creating our input stream, which wraps the GZip file given
                    using (var stream = File.OpenRead (rootFolder + idxZipFile)) {

                        // Unzipping
                        var zipFile = new ZipFile (stream);
                        zipFile.Password = e.Args.GetExChildValue<string> ("password", context, null);

                        // Looping through entries in zip file
                        foreach (ZipEntry idxZipEntry in zipFile) {

                            // Getting full path of currently iterated file/folder
                            var idxDestinationFileFolder = rootFolder + destinationFolder + idxZipEntry.Name.TrimStart ('/');

                            if (idxZipEntry.IsFile) {

                                // Making sure directory exist
                                var splits = new List<string> ((destinationFolder + idxZipEntry.Name.TrimStart ('/')).Split ('/'));
                                splits.RemoveAt (splits.Count - 1);
                                var curPath = "/";
                                foreach (var idxSplit in splits) {

                                    // Adding currently iterated folder
                                    curPath += idxSplit + "/";

                                    // Verifying user is authorized to writing to currently iterated destination folder
                                    context.RaiseNative ("p5.io.authorize.modify-folder", new Node ("", curPath).Add ("args", e.Args));

                                    if (!Directory.Exists (rootFolder + curPath)) {
                                        Directory.CreateDirectory (rootFolder + curPath);
                                    }
                                }

                                // Entry is file, making sure "full path" exist
                                using (var outputStream = File.Create (idxDestinationFileFolder)) {

                                    Stream zipStream = zipFile.GetInputStream(idxZipEntry);
                                    zipStream.CopyTo (outputStream);
                                }
                            }
                        }
                    }

                    // Returning folder path of where files where unzipped to caller
                    e.Args.Value = destinationFolder;
                }
            }
        }
    }
}
