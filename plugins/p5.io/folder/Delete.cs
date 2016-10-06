/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to help remove folders from disc
    /// </summary>
    public static class Remove
    {
        /// <summary>
        ///     Removes folders from disc
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-folder")]
        public static void delete_folder (ApplicationContext context, ActiveEventArgs e)
        {
            QueryHelper.Iterate (context, e.Args, true, "modify-folder", delegate (string foldername, string fullpath) {
                if (Directory.Exists (fullpath)) {
                    Directory.Delete (fullpath, true);
                } else {
                    throw new LambdaException (string.Format ("Tried to delete non-existing folder - '{0}'", foldername), e.Args, context);
                }
            });
        }
    }
}
