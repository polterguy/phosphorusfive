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

using System.IO;
using System.Linq;
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
            using (new ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Helpers.GetBaseFolder (context);

                // Getting destination file
                var destination = Helpers.GetSystemPath (context, e.Args.GetExValue<string> (context));

                // Getting destination path, and verify path can be legally written to
                var destinationFile = Helpers.GetLegalDestinationFilename (
                    context,
                    e.Args,
                    rootFolder,
                    destination);

                // Getting source file(s)
                var source = XUtil.Sources (context, e.Args, "compression-level", "password", "key-size");

                // Making sure we are able to delete zip file, if an exception occurs
                try {

                    // Creating zip file, supplying FileStream as stream to store results into
                    using (ZipCreator creator = new ZipCreator (
                        context,
                        File.Create (rootFolder + destinationFile),
                        e.Args.GetExChildValue ("compression-level", context, 3),
                        e.Args.GetExChildValue<string> ("password", context, null),
                        e.Args.GetExChildValue ("key-size", context, 256))) {

                        // Looping through each input file/folder given
                        foreach (var idxSourceFileFolder in source) {

                            var idxSource = Helpers.GetSystemPath (context, Utilities.Convert<string> (context, idxSourceFileFolder));

                            // Verifies source folder can be read from
                            Helpers.VerfifySourceFileFolderCanBeReadFrom (
                                context,
                                e.Args,
                                rootFolder,
                                destinationFile,
                                idxSource);

                            // Adding currently iterated file/folder to zip file stream
                            creator.AddToArchive (rootFolder + idxSource,
                                                  e.Args,
                                                  e.Args.Children.First (ix => idxSourceFileFolder.Name == (string)ix.Value).GetExChildValue<string> ("as", context, null));
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
