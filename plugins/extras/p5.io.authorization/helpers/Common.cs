/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Collections.Generic;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for helpers for p5.io.authorization plugin
/// </summary>
namespace p5.io.authorization.helpers
{
    /// <summary>
    ///     Class containing common methods for p5.io namespace
    /// </summary>
    internal static class Common
    {
        /*
         * Returns the filename of our "auth" file
         */
        public static string GetAuthFile (ApplicationContext context)
        {
            return context.RaiseNative ("_p5.security.get-auth-file").Get<string> (context).Replace ("~/", "");
        }

        /*
         * Normalizes a folder name
         */
        public static string NormalizeFolderName (string folder)
        {
            if (folder.StartsWith ("~/"))
                folder = folder.Substring (2);
            return "/" + folder.Trim ('/') + "/";
        }

        /*
         * Normalizes a file name
         */
        public static string NormalizeFileName (string file)
        {
            return file.TrimStart ('/').TrimEnd ('.');
        }
    }
}
