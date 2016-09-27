/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for everything related to folders
/// </summary>
namespace p5.io.folder
{
    /// <summary>
    ///     Class to help create folders on disc
    /// </summary>
    public static class Create
    {
        /// <summary>
        ///     Creates folders on disc
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-folder")]
        public static void create_folder (ApplicationContext context, ActiveEventArgs e)
        {
            QueryHelper.Run (context, e.Args, true, "modify-folder", delegate (string foldername, string fullpath) {
                if (Directory.Exists (fullpath)) {
                    throw new LambdaException (string.Format ("Folder '{0}' exist from before", foldername), e.Args, context);
                } else {
                    Directory.CreateDirectory (fullpath);
                }
                return true;
            });
        }
    }
}
