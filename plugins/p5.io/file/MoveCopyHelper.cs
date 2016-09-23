/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.io.common;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help copy, rename and/or move files
    /// </summary>
    public static class MoveCopyHelper
    {
        // Delegate for actual implementation for handling one single file
        public delegate void MoveCopyDelegate (string rootFolder, string source, string destination);

        /// <summary>
        ///     Common helper for iterating files for move/copy/rename operation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        /// <param name="functor"></param>
        public static void CopyMoveFile (ApplicationContext context, ActiveEventArgs e, MoveCopyDelegate functor)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Checking if we're given an expression as destination, to make sure we support Active Event source
                if (XUtil.IsExpression (e.Args.Value)) {

                    // Destination is an expression, which means we might have an Active Event invocation source
                    var destEx = e.Args.Value as Expression;
                    foreach (var idxSource in destEx.Evaluate (context, e.Args, e.Args)) {

                        // Retrieving destination, possibly relative to currently iterated expression result
                        var dest = XUtil.InvokeSource (context, e.Args, idxSource.Node, "dest");

                        // Making sure source yields only one value
                        if (dest.Count != 1)
                            throw new LambdaException ("The destination for your [move-file] operation must be one string", e.Args, context);
                        MoveFile (
                            context, 
                            e.Args, 
                            rootFolder, 
                            Utilities.Convert<string> (context, idxSource.Value),
                            Utilities.Convert<string> (context, dest[0]),
                            functor);
                    }
                } else {

                    // Destination is a constant
                    // Retrieving destination, possibly relative to currently iterated expression result
                    var dest = XUtil.InvokeSource (context, e.Args, e.Args, "dest");

                    // Making sure source yields only one value
                    if (dest.Count != 1)
                        throw new LambdaException ("The destination for your [move-file] operation must be one string", e.Args, context);
                    MoveFile (
                        context,
                        e.Args,
                        rootFolder,
                        e.Args.Get<string> (context),
                        Utilities.Convert<string> (context, dest[0]),
                        functor);
                }
            }
        }

        /*
         * Helper for above
         */
        private static void MoveFile (
            ApplicationContext context, 
            Node args, 
            string rootFolder, 
            string sourceFile, 
            string destinationFile,
            MoveCopyDelegate functor)
        {
            // Getting new filename of file, if needed
            if (File.Exists (rootFolder + destinationFile)) {

                // Destination file exist from before, creating a new unique destination filename
                destinationFile = Common.CreateNewUniqueFileName (context, destinationFile);

                // Verifying user is allowed to save to updated destination filename
                context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", destinationFile).Add ("args", args));
            }

            functor (rootFolder, sourceFile, destinationFile);
        }
    }
}
