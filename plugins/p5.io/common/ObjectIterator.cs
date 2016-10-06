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
    ///     Class to help iterate files and folders
    /// </summary>
    public static class ObjectIterator
    {
        // Callback for iterating files and folders. Unless you return "true", iteration will stop.
        public delegate void ObjectIteratorDelegate (string filename, string fullpath);

        /// <summary>
        ///     Allows you to iterate files and folders for querying them
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Parameters passed into Active Event</param>
        /// <param name="removeArgsValue">If true, will remove args.Value after evaluation</param>
        /// <param name="authorizeEvent">Name of [.p5.io.authorize] Active Event to authorize operation</param>
        public static void Iterate (
            ApplicationContext context, 
            Node args, 
            bool removeArgsValue, 
            string authorizeEvent,
            ObjectIteratorDelegate functor)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (args, removeArgsValue)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Multiple filename source, returning existence of all files
                foreach (var idxFileObject in XUtil.Iterate<string> (context, args, true)) {

                    // Retrieving actual system path
                    var fileObjectName = Common.GetSystemPath (context, idxFileObject);

                    // Verifying user is authorized to reading from currently iterated file
                    switch (authorizeEvent) {
                        case "read-file":
                        case "read-folder":
                        case "modify-file":
                        case "modify-folder":
                            context.Raise (".p5.io.authorize." + authorizeEvent, new Node ("", fileObjectName).Add ("args", args));
                            break;
                        default:
                            throw new LambdaException ("Unknown authorization event in " + args.Name, args, context);
                    }

                    // Invoking callback delegate
                    functor (fileObjectName, rootFolder + fileObjectName);
                }
            }
        }
    }
}
