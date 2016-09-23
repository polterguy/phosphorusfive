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
    ///     Class to help check the size of a file
    /// </summary>
    public static class Size
    {
        /// <summary>
        ///     Returns the size of the specified file(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "file-size", Protection = EventProtection.LambdaClosed)]
        public static void file_size (ApplicationContext context, ActiveEventArgs e)
        {
            QueryHelper.Run (context, e.Args, delegate (string filename, string fullpath) {
                FileInfo info = new FileInfo (fullpath);
                e.Args.Add (filename, info.Length);
            });
        }
    }
}
