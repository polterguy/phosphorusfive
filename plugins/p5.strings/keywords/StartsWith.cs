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

using p5.exp;
using p5.core;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [p5.string.starts-with] Active Event.
    /// </summary>
    public static class StartsWith
    {
        /// <summary>
        ///     The [p5.string.starts-with] event, allows you to check if a string starts with another string.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "starts-with")]
        [ActiveEvent (Name = "p5.string.starts-with")]
        public static void p5_string_starts_with (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

				// Retrieving what to look for, and returning early if we have no source.
				var src = XUtil.Source (context, e.Args);
				if (src == null)
					return;

				// Returning to lowers of expression or constant.
                e.Args.Value = XUtil.Single<string> (context, e.Args)?.StartsWithEx (Utilities.Convert<string> (context, src)) ?? false;
            }
        }
    }
}
