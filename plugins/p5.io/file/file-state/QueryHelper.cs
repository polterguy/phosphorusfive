/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;

namespace p5.io.file.file_state
{
    /// <summary>
    ///     Class to help query files
    /// </summary>
    public static class QueryHelper
    {
        // Callback for iterating files
        public delegate void QueryHelperDelegate (string filename, string fullpath);

        /// <summary>
        ///     Allows you to iterate files for querying them
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Parameters passed into Active Event</param>
        public static void Run (ApplicationContext context, Node args, QueryHelperDelegate functor)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (args, true)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Multiple filename source, returning existence of all files
                foreach (var idxFile in XUtil.Iterate<string> (context, args, true)) {

                    // Retrieving actual system path
                    var filename = Common.GetSystemPath (context, idxFile);

                    // Verifying user is authorized to reading from currently iterated file
                    context.RaiseNative ("p5.io.authorize.read-file", new Node ("", filename).Add ("args", args));

                    // Invoking callback delegate
                    functor (filename, rootFolder + filename);
                }
            }
        }
    }
}
