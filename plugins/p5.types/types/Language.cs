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
using System.Text;
using p5.exp;
using p5.core;

namespace p5.types.types
{
    /// <summary>
    ///     Class helps to handle locale and globalization from a general idea.
    /// </summary>
    public static class Language
    {
        /// <summary>
        ///     Creates a byte array from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.globalization.get-language-name")]
        static void p5_globalization_get_language_name (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = System.Globalization.CultureInfo.GetCultureInfo (e.Args.GetExValue<string> (context)).DisplayName;
        }
    }
}
