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

using System.Text.RegularExpressions;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [replace] Active Event.
    /// </summary>
    public static class Replace
    {
        /// <summary>
        ///     The [replace] event, allows you to replace occurrences of a string or regular expression with another strings.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "replace")]
        [ActiveEvent (Name = "p5.string.replace")]
        public static void p5_string__replace (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Figuring out source value for [replace], and returning early if there is no source.
                string source = XUtil.Single<string> (context, e.Args);
                if (source == null)
                    return;

                // Retrieving what to replace it with.
                var with = Utilities.Convert (context, XUtil.Source (context, e.Args, "src"), "");

                // Checking what type of object we're searching for, and doing some basic sanity check.
                var what = XUtil.Source (context, e.Args, "dest");
                if (what == null)
                    throw new LambdaException ("[replace] requires something to search for", e.Args, context);
                if (what is Regex)
                    e.Args.Value = (what as Regex).Replace (source, with);
                else
                    e.Args.Value = source.Replace (Utilities.Convert<string> (context, what), with);
            }
        }
    }
}
