/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to check if a folder exists on disc
    /// </summary>
    public static class Exists
    {
        /// <summary>
        ///     Checks to see if a folder exists on disc or not
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "folder-exist")]
        public static void folder_exist (ApplicationContext context, ActiveEventArgs e)
        {
            QueryHelper.Run (context, e.Args, false, "read-folder", delegate (string foldername, string fullpath) {
                if (!Directory.Exists (fullpath)) {
                    e.Args.Value = false;
                    return false;
                } else {
                    e.Args.Value = true;
                    return true;
                }
            });

            // In case expression yields no results, it will still be in value of node
            if (XUtil.IsExpression (e.Args.Value))
                e.Args.Value = false;
        }
    }
}
