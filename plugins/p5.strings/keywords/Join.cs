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

using System.Text;
using p5.exp;
using p5.core;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [p5.string.join] Active Event.
    /// </summary>
    public static class Join
    {
        /// <summary>
        ///     The [p5.string.join] event, allows you to join multiple strings into a single string, performing automatic conversion
        ///     to strings of the specified expression values if necessary.
        ///     Optionally, pass in a [sep] and [wrap] argument, which separates and 'wraps' each entities joined.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "join")]
        [ActiveEvent (Name = "p5.string.join")]
        public static void p5_string_join (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving separator character(s).
                var sep = e.Args.GetExChildValue<string> ("sep", context, null);

                // Retrieving wrapper character(s).
                var wrap = e.Args.GetExChildValue<string> ("wrap", context, null);

                // Used as buffer
                StringBuilder result = new StringBuilder ();

                // Looping through each value
                foreach (var idx in XUtil.Iterate<string> (context, e.Args)) {

                    // Checking if this is first instance, and if not, we add separator value, if there is a [sep] value defined.
                    if (sep != null && result.Length != 0)
                        result.Append (sep);

                    // Adding currently iterated result, possibly adding [wrap] on each side of string.
                    if (wrap != null)
                        result.Append (wrap + idx + wrap);
                    else
                        result.Append (idx);
                }

                // Returning result.
                e.Args.Value = result.ToString ();
            }
        }
    }
}
