/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
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
    ///     Class to help unzip zipfiles
    /// </summary>
    public static class UnZipper
    {
        /// <summary>
        ///     Unzips zipfiles
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "unzip")]
        public static void unzip (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving password, if there is one, and untying it, 
            // to make sure it never leaves method, in case of an exception, etc
            string password = e.Args.GetExChildValue<string>("password", context, null);
            e.Args.FindOrCreate ("password").UnTie (); // Making sure password NEVER LEAVES METHOD!!

            // Basic syntax checking
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args ["to"] == null)
                throw new LambdaException (
                    "[unzip] needs both a value and a [to] node",
                    e.Args,
                    context);

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Helpers.GetBaseFolder (context);

                // Getting destination folder
                var destFolder = GetDestinationFolder (context, e);

                // Looping through each source zip file given
                foreach (var idxZipFilePath in XUtil.Iterate<string> (context, e.Args)) {

                    // Unzips currently iterated file
                    UnzipFile (
                        context, 
                        e.Args,
                        rootFolder,
                        idxZipFilePath,
                        destFolder,
                        password);
                }

                // Returning folder path of where files where unzipped to caller
                e.Args.Value = destFolder;
            }
        }

        /*
         * Retrieves destination folder, and verifies user has write access to it
         */
        private static string GetDestinationFolder (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving detination folder
            var destFolder = e.Args.GetExChildValue<string> ("to", context);

            // Verifying user is authorized to writing to destination folder
            context.Raise (".p5.io.authorize.modify-folder", new Node ("", destFolder).Add ("args", e.Args));
            return destFolder;
        }

        /*
         * Unzips a single file
         */
        static void UnzipFile (
            ApplicationContext context, 
            Node args,
            string rootFolder,
            string zipFilePath,
            string destPath,
            string password)
        {
            // Verifying user is allowed to read from zipfile given
            context.Raise (".p5.io.authorize.read-file", new Node ("", zipFilePath).Add ("args", args));

            // Creating ZipFile, wrapping a file stream, denoting the path to physical file on disc
            using (var zipFile = new ZipFile (File.OpenRead (rootFolder + zipFilePath)) {
                IsStreamOwner = true,
                Password = password
            }) {
                // Looping through entries in zip file
                foreach (ZipEntry idxZipEntry in zipFile) {

                    // Making sure entry is a file
                    if (idxZipEntry.IsFile) {

                        // Getting full path of currently iterated file/folder
                        var idxDestPath = destPath + idxZipEntry.Name.TrimStart ('/');

                        // Entry is file, making sure "full path" exist
                        EnsureFolderExist (
                            context, 
                            args, 
                            rootFolder,
                            idxDestPath);

                        // Serialise file to stream
                        using (var outputStream = File.Create (rootFolder + idxDestPath)) {

                            // Serialising zipfile to stream
                            zipFile.GetInputStream (idxZipEntry).CopyTo (outputStream);
                        }
                    }
                }
            }
        }

        /*
         * Ensures that the given file's folder exist, and creates it if necessary
         */
        static void EnsureFolderExist (
            ApplicationContext context, 
            Node args, 
            string rootFolder,
            string fileNameFullPath)
        {
            // Splitting filename path on "/" to create folder entities
            var splits = new List<string> ((fileNameFullPath).Split ('/'));

            // Removing filename
            splits.RemoveAt (splits.Count - 1);
            var curPath = "/";
            foreach (var idxSplit in splits) {

                // Adding currently iterated folder
                curPath += idxSplit + "/";

                // Verifies user is authorized to writing to currently iterated destination folder
                context.Raise (".p5.io.authorize.modify-folder", new Node ("", curPath).Add ("args", args));

                // Verifies folder exist, and if not, creates it
                if (!Directory.Exists (rootFolder + curPath)) {

                    // Folder did not exist, creating it
                    Directory.CreateDirectory (rootFolder + curPath);
                }
            }
        }
    }
}
