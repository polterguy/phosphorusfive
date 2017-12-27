/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using p5.exp;
using p5.core;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Helper to retrieve platform type (Linux/Mac, etc)
    /// </summary>
    public static class Platform
    {
        /// <summary>
        ///     Helper to retrieve platform type (Linux/Mac, etc)
        ///     Useful to figure out line endings for special files, etc.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.system.platform")]
        public static void p5_system_platform (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new ArgsRemover (e.Args, false)) {
                e.Args.Value = Environment.OSVersion.Platform.ToString ();
            }
        }

        /// <summary>
        ///     Replaces CR/LF sequences with platform specific version.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.system.platform.normalize-string")]
        public static void p5_system_platform_normalize_string (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new ArgsRemover (e.Args, false)) {
                var content = e.Args.GetExValue (context, "");
                if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix) {
                    e.Args.Value = content.Replace ("\r\n", "\n");
                }
            }
        }
    }
}
