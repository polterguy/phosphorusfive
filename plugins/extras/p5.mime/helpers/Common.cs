/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.exp;
using p5.core;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Common helper class for mime features of Phosphorus Five
    /// </summary>
    internal static class Common
    {
        /// <summary>
        ///     Returns base folder for application
        /// </summary>
        /// <returns>The base folder</returns>
        /// <param name="context">Application Context</param>
        public static string GetRootFolder (ApplicationContext context)
        {
            return context.RaiseNative ("p5.core.application-folder").Get<string> (context);
        }
    }
}

