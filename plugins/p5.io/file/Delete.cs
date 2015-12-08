/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help remove files
    /// </summary>
    public static class Delete
    {
        /// <summary>
        ///     Removes files from disc
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-file", Protection = EventProtection.LambdaClosed)]
        private static void delete_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Iterating through each path given
                foreach (var idxFile in Common.GetSource (e.Args, context)) {

                    // Verifying user is authorized to writing to destination file
                    context.RaiseNative ("p5.io.authorize.modify-file", new Node ("p5.io.authorize.modify-file", idxFile).Add ("args", e.Args));

                    // Verify path is correct according to conventions
                    if (!idxFile.StartsWith ("/"))
                        throw new LambdaException (
                            string.Format ("Filename '{0}' was not a valid filename", idxFile),
                            e.Args,
                            context);

                    // Checking if file exist
                    if (File.Exists (rootFolder + idxFile)) {

                        // File exists, removing file
                        File.Delete (rootFolder + idxFile);
                    } else {

                        // Oops, file didn't exist, throwing an exception
                        throw new LambdaException  (string.Format ("Tried to delete non-existing file - '{0}'", idxFile), e.Args, context);
                    }
                }
            }
        }
    }
}
