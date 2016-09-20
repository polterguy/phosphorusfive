/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help copy and/or rename a file
    /// </summary>
    public static class Copy
    {
        /// <summary>
        ///     Copies a file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "copy-file", Protection = EventProtection.LambdaClosed)]
        public static void copy_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Getting source and verify path is correct according to conventions
                string sourceFile = XUtil.Single<string> (context, e.Args);

                // Getting destination and verify path is correct according to conventions
                string destinationFile = XUtil.Single<string> (context, e.Args ["to"]);

                // Verifying user is authorized to both reading from source, and writing to destination
                context.RaiseNative ("p5.io.authorize.read-file", new Node ("", sourceFile).Add ("args", e.Args));
                context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", destinationFile).Add ("args", e.Args));

                // Getting new filename of file, if needed
                if (File.Exists (rootFolder + destinationFile)) {

                    // Destination file exist from before, creating a new unique destination filename
                    destinationFile = Common.CreateNewUniqueFileName (context, destinationFile);

                    // Checking again if user is authorized to writing to new destination filename
                    context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", destinationFile).Add ("args", e.Args));
                }

                // Actually moving (or renaming) file
                File.Copy (rootFolder + sourceFile, rootFolder + destinationFile);

                // Returning actual destination filename used to caller
                e.Args.Value = destinationFile;
            }
        }
    }
}
