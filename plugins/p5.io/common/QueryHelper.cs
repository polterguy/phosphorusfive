/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.exp;
using p5.core;

namespace p5.io.common
{
    /// <summary>
    ///     Class to help query files and folders
    /// </summary>
    public static class QueryHelper
    {
        // Callback for iterating files and folders. Unless you return "true", iteration will stop.
        public delegate bool QueryHelperDelegate (string filename, string fullpath);

        /// <summary>
        ///     Allows you to iterate files and folders for querying them
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Parameters passed into Active Event</param>
        public static void Run (
            ApplicationContext context, 
            Node args, 
            bool removeArgsValue, 
            string authorizeEvent,
            QueryHelperDelegate functor)
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
                    context.Raise ("p5.io.authorize." + authorizeEvent, new Node ("", fileObjectName).Add ("args", args));

                    // Invoking callback delegate
                    if (!functor (fileObjectName, rootFolder + fileObjectName))
                        return;
                }
            }
        }
    }
}
