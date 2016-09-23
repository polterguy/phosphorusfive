/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help check if a file exists.
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Returns true if file(s) exists
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "file-exist", Protection = EventProtection.LambdaClosed)]
        public static void file_exist (ApplicationContext context, ActiveEventArgs e)
        {
            QueryHelper.Run (context, e.Args, false, "read-file", delegate (string filename, string fullpath) {
                if (!File.Exists (fullpath)) {
                    e.Args.Value = false;
                    return false;
                } else {
                    e.Args.Value = true;
                    return true;
                }
            });

            // In case expressions yields nothing, it should still be in value of node
            if (XUtil.IsExpression (e.Args.Value))
                e.Args.Value = false;
        }
    }
}
