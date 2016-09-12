/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Drawing;
using p5.core;

namespace p5.imaging.helpers
{
    /// <summary>
    ///     Class containing common methods for p5.imaging namespace
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        ///     Returns the root folder of application pool back to caller
        /// </summary>
        /// <returns>the root folder</returns>
        /// <param name="context">application context</param>
        public static string GetBaseFolder (ApplicationContext context)
        {
            return context.RaiseNative ("p5.core.application-folder").Get<string> (context);
        }

        /// <summary>
        ///     Returns a Color from given string representation
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="color">Color.</param>
        public static Color GetColor (string color)
        {
            return ColorTranslator.FromHtml (color);
        }
    }
}
