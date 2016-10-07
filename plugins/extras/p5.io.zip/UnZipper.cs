/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
using System.Linq;
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
            if (e.Args.Value == null)
                throw new LambdaException (
                    "[unzip] needs both a destination as its value",
                    e.Args,
                    context);

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Helpers.GetBaseFolder (context);

                // Getting destination folder
                var destFolder = GetDestinationFolder (context, e);

                // Getting source files
                var source = XUtil.Source (context, e.Args, e.Args, "src", new string[] { "password" }.ToList ());

                // Looping through each source zip file given
                foreach (var idxZipFilePath in source) {

                    // Unzips currently iterated file
                    UnzipFile (
                        context, 
                        e.Args,
                        rootFolder,
                        Helpers.GetSystemPath (context, Utilities.Convert<string> (context, idxZipFilePath)),
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
            var destFolder = Helpers.GetSystemPath (context, e.Args.GetExValue<string> (context));

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
