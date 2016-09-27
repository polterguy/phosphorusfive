/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.io.common
{
    /// <summary>
    ///     Class to help copy, rename and/or move files
    /// </summary>
    public static class MoveCopyHelper
    {
        /// Delegate for actual implementation for handling one single file
        public delegate void MoveCopyDelegate (string rootFolder, string source, string destination);

        /// Delegate for creating a new filename/foldername
        public delegate string CreateNewFileObjectNameDelegate (string destination);

        /// Delegate for checking if fileobject exists
        public delegate bool FileObjectExistDelegate (string destination);

        /// <summary>
        ///     Common helper for iterating files/folders for move/copy/rename operation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        /// <param name="functor"></param>
        public static void CopyMoveFileObject (
            ApplicationContext context, 
            ActiveEventArgs e, 
            MoveCopyDelegate functor, 
            CreateNewFileObjectNameDelegate functor2,
            FileObjectExistDelegate functor3)
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
                        var dest = XUtil.Source (context, e.Args, idxSource.Node, "dest");

                        // Making sure source yields only one value
                        if (dest.Count != 1)
                            throw new LambdaException ("The destination for your [move-file] operation must be one string", e.Args, context);
                        CopyMoveFileObjectImplementation (
                            context, 
                            e.Args, 
                            rootFolder, 
                            Common.GetSystemPath (context, Utilities.Convert<string> (context, idxSource.Value)),
                            Common.GetSystemPath (context, Utilities.Convert<string> (context, dest[0])),
                            functor,
                            functor2,
                            functor3);
                    }
                } else {

                    // Destination is a constant
                    // Retrieving destination, possibly relative to currently iterated expression result
                    var dest = XUtil.Source (context, e.Args, e.Args, "dest");

                    // Making sure source yields only one value
                    if (dest.Count != 1)
                        throw new LambdaException ("The destination for your [move-file] operation must be one string", e.Args, context);
                    CopyMoveFileObjectImplementation (
                        context,
                        e.Args,
                        rootFolder,
                        Common.GetSystemPath (context, e.Args.Get<string> (context)),
                        Common.GetSystemPath (context, Utilities.Convert<string> (context, dest[0])),
                        functor,
                        functor2,
                        functor3);
                }
            }
        }

        /*
         * Helper for above
         */
        private static void CopyMoveFileObjectImplementation (
            ApplicationContext context, 
            Node args, 
            string rootFolder, 
            string sourceFile, 
            string destinationFile,
            MoveCopyDelegate functor,
            CreateNewFileObjectNameDelegate functor2,
            FileObjectExistDelegate functor3)
        {
            // Getting new filename of file, if needed
            if (functor3 (rootFolder + destinationFile)) {

                // Destination file exist from before, creating a new unique destination filename
                destinationFile = functor2 (destinationFile);

                // Verifying user is allowed to save to updated destination filename
                context.Raise ("p5.io.authorize.modify-file", new Node ("", destinationFile).Add ("args", args));
            }

            functor (rootFolder, sourceFile, destinationFile);
        }
    }
}
