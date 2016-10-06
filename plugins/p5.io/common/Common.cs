/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;

/// <summary>
///     Main namespace for all file operations in Phosphorus Five
/// </summary>
namespace p5.io.common
{
    /// <summary>
    ///     Class containing common methods for p5.io namespace
    /// </summary>
    internal static class Common
    {
        /// <summary>
        ///     Returns the root folder of application pool back to caller
        /// </summary>
        /// <returns>the root folder</returns>
        /// <param name="context">application context</param>
        public static string GetRootFolder (ApplicationContext context)
        {
            return context.Raise (".p5.core.application-folder").Get<string> (context);
        }

        /*
         * Returns the actual file/folder path
         */
         public static string GetSystemPath (ApplicationContext context, string path)
        {
            if (path.StartsWith ("~")) {
                return "/users/" + context.Ticket.Username + path.Substring (1);
            }
            return path;
        }
    }
}
