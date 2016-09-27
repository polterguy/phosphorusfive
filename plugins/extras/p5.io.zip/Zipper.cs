/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.zip.helpers;

namespace p5.io.zip
{
    /// <summary>
    ///     Class to help zip folders and files
    /// </summary>
    public static class Zipper
    {
        /// <summary>
        ///     Zips folders and files
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "zip")]
        public static void zip (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * Notice, since zip might zip another file than requested by the user, we return that filepath
             * as value of main args
             */
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Helpers.GetBaseFolder (context);

                // Getting destination path, and verify path can be legally written to
                var destinationFile = Helpers.GetLegalDestinationFilename (
                    context, 
                    e.Args, 
                    rootFolder, 
                    XUtil.Single<string> (context, e.Args ["to"], true));

                // Making sure we are able to delete zip file, if an exception occurs
                try {

                    // Creating zip file, supplying FileStream as stream to store results into
                    using (ZipCreator creator = new ZipCreator(
                        context, 
                        File.Create (rootFolder + destinationFile), 
                        e.Args.GetExChildValue<int> ("compression-level", context, 3),
                        e.Args.GetExChildValue<string> ("password", context, null),
                        e.Args.GetExChildValue<int> ("key-size", context, 256))) {

                        // Looping through each input file/folder given
                        foreach (var idxSourceFileFolder in XUtil.Iterate<string> (context, e.Args, true)) {

                            // Verifies source folder can be read from
                            Helpers.VerfifySourceFileFolderCanBeReadFrom (
                                context, 
                                e.Args, 
                                rootFolder,
                                destinationFile,
                                idxSourceFileFolder);

                            // Adding currently iterated file/folder to zip file stream
                            creator.AddToArchive (rootFolder + idxSourceFileFolder, e.Args);
                        }
                    }

                    // Making sure we return actual destination to caller
                    e.Args.Value = destinationFile;
                } catch {

                    // Checking if destination file exist, and if so, delete it, before we rethrow exception
                    if (File.Exists (rootFolder + destinationFile))
                        File.Delete (rootFolder + destinationFile);
                    throw;
                }
            }
        }
    }
}
