/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Collections.Generic;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for all file operations in Phosphorus Five
/// </summary>
namespace p5.io.common
{
    /// <summary>
    ///     Class containing common methods for p5.io namespace
    /// </summary>
    internal static class Common
    {
        /*
         * Contains our root folder
         */
        private static string _rootFolder;

        /// <summary>
        ///     Returns the root folder of application pool back to caller
        /// </summary>
        /// <returns>the root folder</returns>
        /// <param name="context">application context</param>
        public static string GetRootFolder (ApplicationContext context)
        {
            if (_rootFolder == null) {

                // This is the first time we invoke this guy
                _rootFolder = context.RaiseNative ("p5.core.application-folder").Get<string> (context);

                // Making sure we normalize folder separators, to have uniform folder structure on different operating systems
                _rootFolder = _rootFolder.Replace ("\\", "/").TrimEnd ('/');
            }
            return _rootFolder;
        }

        /*
         * Creates a new unique filename, and returns it to caller
         */
        public static string CreateNewUniqueFileName (ApplicationContext context, string destination)
        {
            string basepath = GetRootFolder (context);
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

            int idxNo = 2;
            while (File.Exists (basepath + folder + string.Format ("{0} copy {1}{2}", filename, idxNo, suffix))) {
                idxNo += 1;
            }
            return string.Format (folder + "{0} copy {1}{2}", filename, idxNo, suffix);
        }

        /*
         * Creates a new unique foldername, and returns it to caller
         */
        public static string CreateNewUniqueFolderName (ApplicationContext context, string destination)
        {
            string basepath = GetRootFolder (context);

            // Getting folder
            destination = destination.TrimEnd ('/');
            string baseFolder = destination.Substring (0, destination.LastIndexOf ("/") + 1);

            // Getting filename, and removing suffix from filename
            string newFolderName = destination.Substring (baseFolder.Length);

            int idxNo = 2;
            while (Directory.Exists (basepath + baseFolder + string.Format ("{0} copy {1}/", newFolderName, idxNo))) {
                idxNo += 1;
            }
            return string.Format (baseFolder + "{0} copy {1}/", newFolderName, idxNo);
        }

        /*
         * Used to retrieve source for operations such as [file-exist], [load-file] etc ...
         */
        public static IEnumerable<string> GetSource (Node args, ApplicationContext context)
        {
            if (args.Value != null) {
                foreach (var idx in XUtil.Iterate<string> (context, args)) {

                    yield return idx;
                }
                yield break;
            } else {
                var objRetVal = XUtil.SourceSingle (context, args);
                if (objRetVal is Node) {

                    // Converting to a single string
                    objRetVal = ((Node)objRetVal).Get<string> (context);
                } else if (objRetVal is IEnumerable<Node>) {

                    // Converting to a single string
                    foreach (var idx in objRetVal as IEnumerable<Node>) {
                    
                        yield return idx.Get<string> (context);
                    }
                    yield break;
                }
                yield return Utilities.Convert<string> (context, objRetVal) ?? "";
            }
        }
    }
}
