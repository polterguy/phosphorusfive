/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

namespace p5.types.types
{
    /// <summary>
    ///     Class contains helper methods for string type
    /// </summary>
    public static class Base64
    {
        /// <summary>
        ///     Creates a bool from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.string.decode-base64")]
        private static void p5_string_decode_base64 (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves.
            using (new ArgsRemover (e.Args)) {

                // Decoding given value from base64 and returning to caller.
                e.Args.Value = Convert.FromBase64String (XUtil.Single<string> (context, e.Args));
            }
        }
    }
}
