/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for everything related to folders
/// </summary>
namespace p5.io.folder
{
    /// <summary>
    ///     Class to help create folders on disc
    /// </summary>
    public static class Create
    {
        /// <summary>
        ///     Creates folders on disc
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-folder", Protection = EventProtection.LambdaClosed)]
        private static void create_folder (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving root folder
                var rootFolder = Common.GetRootFolder (context);

                // Iterating through each folder caller wants to create
                foreach (var idxFolder in Common.GetSource (e.Args, context)) {

                    // Verify foldername is a valid foldername according to conventions
                    if (!idxFolder.StartsWith ("/") || !idxFolder.EndsWith ("/"))
                        throw new LambdaException (
                            string.Format ("Foldername '{0}' was not a valid foldername", idxFolder),
                            e.Args,
                            context);

                    // Verifying user is authorized to writing to destination
                    context.RaiseNative ("p5.io.authorize.save-folder", new Node ("p5.io.authorize.save-folder", idxFolder).Add ("args", e.Args));

                    // Checking to see if folder already exists
                    if (Directory.Exists (rootFolder + idxFolder)) {

                        // Oops, folder exist from before
                        throw new LambdaException (string.Format ("Folder '{0}' exist from before", idxFolder), e.Args, context);
                    } else {

                        // Folder didn't exist
                        Directory.CreateDirectory (rootFolder + idxFolder);
                    }
                }
            }
        }
    }
}
