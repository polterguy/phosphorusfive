/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using p5.core;
using p5.exp.exceptions;

namespace p5.io.zip.helpers
{
    /// <summary>
    ///     Class for creating a ZIP file and/or stream
    /// </summary>
    public class ZipCreator : IDisposable
    {
        // Used for tracking if instance is disposed
        private bool _disposed;

        // AES key size, 128 or 256
        private int _keySize;

        // Actual Zip stream
        ZipOutputStream _zipStream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.io.zip.helpers.ZipCreator"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="stream">Stream to zip towards</param>
        /// <param name="compressionLevel">Compression level, 1 through 9</param>
        /// <param name="password">Password, to use for AES encryption of file. Null equals "no encryption"</param>
        /// <param name="keySize">Key size to use for AES encryption. 128 or 256. 256 is significantly stronger, but spends twice as much resources</param>
        public ZipCreator (
            ApplicationContext context,
            Stream stream,
            int compressionLevel,
            string password,
            int keySize)
        {
            Context = context;
            _zipStream = new ZipOutputStream (stream);
            _zipStream.IsStreamOwner = true;
            _zipStream.SetLevel (compressionLevel);
            _zipStream.Password = password;
            _keySize = keySize;
        }

        private ApplicationContext Context {
            get;
            set;
        }

        /*
         * Adds the given file to archive
         */
        public void AddToArchive (string fileFolderName, Node args, string asFileName)
        {
            // We don't zip Linux hidden files or backup files
            if (fileFolderName.Contains ("/.") || fileFolderName.Contains ("~/"))
                return;

            // Figuring out folder of file and file, and separating these two
            fileFolderName = fileFolderName.TrimEnd ('/');
            var rootFolder = fileFolderName.Substring (0, fileFolderName.LastIndexOf ("/") + 1);
            fileFolderName = fileFolderName.Substring (fileFolderName.LastIndexOf ("/") + 1);

            // Now we have separated folder and file name, and can call internal implementation
            AddToArchive (fileFolderName, rootFolder, args, asFileName);
        }

        /*
         * Adds given file/folder from root folder into archive
         */
        private void AddToArchive (string fileFolderName, string rootFolder, Node args, string asFileName)
        {
            // Checking if this is directory
            if (Directory.Exists (rootFolder + fileFolderName)) {

                // Traversing each file within directory
                foreach (var idxOSFileName in Directory.GetFiles (rootFolder + fileFolderName)) {

                    // Normalizing filename
                    var idxFile = idxOSFileName.Replace ("\\", "/").Substring (rootFolder.Length);

                    // Verifying file-/folder name is not a Linux backup or hidden file
                    if (!Path.GetFileName (idxFile).StartsWith (".") && !Path.GetFileName (idxFile).EndsWith ("~")) {

                        // Puts the currently iterated file to the archive
                        PutFileToArchive (
                            rootFolder + idxFile,
                            idxFile,
                            args,
                            asFileName);
                    }
                }

                // Traversing each folder within directory, recursively invoking "self"
                foreach (var idxOSFolderName in Directory.GetDirectories (rootFolder + fileFolderName)) {

                    // Normalizing filename
                    var idxFolder = idxOSFolderName.Replace ("\\", "/").Substring (rootFolder.Length);

                    // Verifying file-/folder name is not Linux backup file/folder or hidden file/folder
                    if (!idxFolder.Contains ("/.") && !idxFolder.EndsWith ("~"))
                        AddToArchive (idxFolder, rootFolder, args, asFileName);
                }
            } else {

                // Puts the file to the archive
                PutFileToArchive (
                    rootFolder + fileFolderName,
                    fileFolderName,
                    args,
                    asFileName);
            }
        }

        /*
         * Puts the given file to the ZIP archive
         */
        private void PutFileToArchive (string fullFileName, string relativeFileName, Node args, string asFileName)
        {
            // Creating entry, and setting properties of new entry
            FileInfo fileInfo = new FileInfo (fullFileName);
            ZipEntry entry = new ZipEntry (asFileName == null ? relativeFileName : asFileName) {
                DateTime = fileInfo.LastWriteTime,
                Size = fileInfo.Length
            };

            // Only setting AES key size if password was supplied to archive
            if (!string.IsNullOrEmpty (_zipStream.Password)) {
                entry.AESKeySize = _keySize;
                if (fileInfo.Length == 0)
                    throw new LambdaException ("You cannot zip and encrypt an empty file", args, Context);
            }

            // Putting entry to archive
            _zipStream.PutNextEntry (entry);

            // Writing contents
            using (FileStream streamReader = File.OpenRead (fullFileName)) {
                streamReader.CopyTo (_zipStream);
            }

            // Closing entry
            _zipStream.CloseEntry ();
        }

        /*
         * Private implementation of IDisposable interface
         */
        void IDisposable.Dispose ()
        {
            Dispose (true);
        }

        /*
         * Actual implementation of Dispose method
         */
        private void Dispose (bool disposing)
        {
            if (!_disposed && disposing) {
                _disposed = true;
                _zipStream.Dispose ();
            }
        }
    }
}
