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

using System.Linq;
using p5.exp;
using p5.core;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [p5.string.trim] Active Event.
    /// </summary>
    public static class Trim
    {
        /// <summary>
        ///     The [p5.string.trim] event, allows you to trim occurrencies of characters in a string.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "trim")]
        [ActiveEvent (Name = "trim-left")]
        [ActiveEvent (Name = "trim-right")]
        [ActiveEvent (Name = "p5.string.trim")]
        [ActiveEvent (Name = "p5.string.trim-left")]
        [ActiveEvent (Name = "p5.string.trim-right")]
        public static void p5_string_trim (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting trim characters, defaulting to whitespace characters.
                var characters = e.Args.GetExChildValue ("chars", context, " \r\n\t");

                // Returning length of constant or expression, converted to string if necessary.
                var source = XUtil.Single<string> (context, e.Args);
                switch (e.Name) {
                    case "trim":
                    case "p5.string.trim":
                        e.Args.Value = source.Trim (characters.ToArray ());
                        break;
                    case "trim-left":
                    case "p5.string.trim-left":
                        e.Args.Value = source.TrimStart (characters.ToArray ());
                        break;
                    case "trim-right":
                    case "p5.string.trim-right":
                        e.Args.Value = source.TrimEnd (characters.ToArray ());
                        break;
                }
            }
        }
    }
}
