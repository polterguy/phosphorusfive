/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for all IO file operations in Phosphorus Five
/// </summary>
namespace p5.io.file
{
    /// <summary>
    ///     Class to help copy a file
    /// </summary>
    public static class Copy
    {
        /// <summary>
        ///     Copies a file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "copy-file", Protection = EventProtection.LambdaClosed)]
        private static void copy_file (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * We do not remove value of arguments here, since it is used for returning value of 
             * new filename, since it might not necessarily be the same as the one caller requested, 
             * if file exist from before
             */

            // Basic syntax checking
            if (e.Args.Value == null || e.Args.LastChild == null || e.Args.LastChild.Name != "to")
                throw new ArgumentException ("[copy-file] needs both a value and a [to] node.");

            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Getting source and verify path is correct according to conventions
                string sourceFile = XUtil.Single<string> (context, e.Args);
                if (!sourceFile.StartsWith ("/"))
                    throw new LambdaException (
                        string.Format ("Source file '{0}' was not a valid filename", sourceFile),
                        e.Args,
                        context);

                // Getting destination and verify path is correct according to conventions
                string destinationFile = XUtil.Single<string> (context, e.Args ["to"]);
                if (!destinationFile.StartsWith ("/"))
                    throw new LambdaException (
                        string.Format ("Destination file '{0}' was not a valid filename", destinationFile),
                        e.Args,
                        context);

                // Verifying user is authorized to both reading from source, and writing to destination
                context.RaiseNative ("p5.io.authorize.read-file", new Node ("p5.io.authorize.read-file", sourceFile).Add ("args", e.Args));
                context.RaiseNative ("p5.io.authorize.modify-file", new Node ("p5.io.authorize.modify-file", destinationFile).Add ("args", e.Args));

                // Getting new filename of file, if needed
                if (File.Exists (rootFolder + destinationFile)) {

                    // Destination file exist from before, creating a new unique destination filename
                    destinationFile = Common.CreateNewUniqueFileName (context, destinationFile);

                    // Checking again if user is authorized to writing to new destination filename
                    context.RaiseNative ("p5.io.authorize.modify-file", new Node ("p5.io.authorize.modify-file", destinationFile).Add ("args", e.Args));
                }

                // Actually moving (or renaming) file
                File.Copy (rootFolder + sourceFile, rootFolder + destinationFile);

                // Returning actual destination filename used to caller
                e.Args.Value = destinationFile;
            }
        }
    }
}
