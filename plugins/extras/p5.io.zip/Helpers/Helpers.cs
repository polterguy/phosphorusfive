/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.exp.exceptions;

namespace p5.io.zip.helpers
{
    /// <summary>
    ///     Class containing common methods for p5.io namespace
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        ///     Returns the root folder of application pool back to caller
        /// </summary>
        /// <returns>the root folder</returns>
        /// <param name="context">application context</param>
        public static string GetBaseFolder (ApplicationContext context)
        {
            return context.Raise (".p5.core.application-folder").Get<string> (context);
        }

        /*
         * Verifies destination file does not exist, and if it does, returns a new path to caller, 
         * and verifies user is authorized to writing to new path
         */
        public static string GetLegalDestinationFilename (
            ApplicationContext context, 
            Node args, 
            string rootFolder, 
            string destinationFile)
        {
            // Checking if file exist
            if (File.Exists (rootFolder + destinationFile)) {

                // Destination file exist from before, creating a new unique destination filename
                destinationFile = Helpers.CreateNewUniqueFileName (context, destinationFile);
            }

            // Verifying user is authorized to writing to destination
            context.Raise ("p5.io.authorize.modify-file", new Node ("", destinationFile).Add ("args", args));

            // Returning possibly new path to caller
            return destinationFile;
        }

        /*
         * Verifies source file or folder can be legally read from
         */
        public static void VerfifySourceFileFolderCanBeReadFrom (
            ApplicationContext context, 
            Node args, 
            string rootFolder,
            string destinationFile, 
            string sourceFileFolder)
        {
            // Verify path is correctly ending with a trailing slash "/" if object is a folder
            if (Directory.Exists (rootFolder + sourceFileFolder)) {

                // Verifies user is authorised to reading folder
                context.Raise ("p5.io.authorize.read-folder", new Node ("", sourceFileFolder).Add ("args", args));
            } else {

                // Verifies user is authorised to reading file
                context.Raise ("p5.io.authorize.read-file", new Node ("", sourceFileFolder).Add ("args", args));
            }

            // Checking that destination is not underneath source
            if (destinationFile.IndexOf (sourceFileFolder) == 0 && destinationFile.Length != sourceFileFolder.Length + 4)
                throw new LambdaException (
                    string.Format ("Destination file '{0}' is beneath source '{1}', which is logically impossible to perform", destinationFile, sourceFileFolder), 
                    args, 
                    context);
        }

        /*
         * Creates a new unique filename, and returns it to caller
         */
        private static string CreateNewUniqueFileName (ApplicationContext context, string destination)
        {
            string basepath = GetBaseFolder (context);
            string[] parts = destination.Split ('.');

            // Getting suffix of filename
            string suffix = parts.Length == 1 ? "" : parts [parts.Length - 1];

            // Getting folder
            string folder = destination.Substring (0, destination.LastIndexOf ("/") + 1);

            // Getting filename, and removing suffix from filename
            string filename = destination.Substring (folder.Length);
            if (!string.IsNullOrEmpty (suffix))
                filename = filename.Substring (0, filename.Length - (suffix.Length + 1));
            suffix = "." + suffix;

            int idxNo = 1;
            while (File.Exists (basepath + folder + string.Format ("{0} copy {1}{2}", filename, idxNo, suffix))) {
                idxNo += 1;
            }
            return string.Format (folder + "{0} copy {1}{2}", filename, idxNo, suffix);
        }
    }
}
