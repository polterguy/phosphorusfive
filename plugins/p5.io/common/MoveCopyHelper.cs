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

        /// Delegate for checking if fileobject exists
        public delegate bool FileObjectExistDelegate (string destination);

        /// <summary>
        ///     Common helper for iterating files/folders for move/copy/rename operation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        /// <param name="functorMoveObject"></param>
        public static void CopyMoveFileObject (
            ApplicationContext context, 
            ActiveEventArgs e, 
            MoveCopyDelegate functorMoveObject, 
            FileObjectExistDelegate functorObjectExist)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Checking if we're given an expression as source, to make sure we support Active Event source
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
                            functorMoveObject,
                            functorObjectExist);
                    }
                } else {

                    // Source is a constant
                    // Retrieving destination
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
                        functorMoveObject,
                        functorObjectExist);
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
            MoveCopyDelegate functorMoveCopy,
            FileObjectExistDelegate functorObjectExist)
        {
            // Getting new filename of file, if needed
            if (functorObjectExist (rootFolder + destinationFile)) {

                // Destination file exist from before, throwing an exception
                throw new LambdaException (destinationFile + " already exist from before", args, context);
            }

            functorMoveCopy (rootFolder, sourceFile, destinationFile);
        }
    }
}
