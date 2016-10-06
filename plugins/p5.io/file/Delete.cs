/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help delete files
    /// </summary>
    public static class Delete
    {
        /// <summary>
        ///     Delete files from disc
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-file")]
        public static void delete_file (ApplicationContext context, ActiveEventArgs e)
        {
            ObjectIterator.Iterate (context, e.Args, true, "modify-file", delegate (string filename, string fullpath) {
                if (File.Exists (fullpath)) {

                    // File exists, removing file
                    File.Delete (fullpath);
                } else {

                    // Oops, file didn't exist, throwing an exception
                    throw new LambdaException (
                        string.Format ("Tried to delete non-existing file '{0}'", filename),
                        e.Args,
                        context);
                }
            });
        }
    }
}
