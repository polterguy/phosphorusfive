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

namespace p5.exp
{
    /// <summary>
    ///     Contains extension methods for string, to make sure we by default have culture-awareness sane methods for most common operations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Invokes string.IndexOf with invariant culture.
        /// </summary>
        /// <returns>The index of the what string, if found, otherwise -1</returns>
        /// <param name="self">The "this" parameter</param>
        /// <param name="what">What to look for</param>
        /// <param name="startIndex">The start index of where to start looking</param>
        public static int IndexOfEx (this string self, string what, int startIndex = 0)
        {
            return self.IndexOf (what, startIndex, System.StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Invokes string.LastIndexOf with invariant culture.
        /// </summary>
        /// <returns>The last index of the what string, if found, otherwise -1</returns>
        /// <param name="self">The "this" parameter</param>
        /// <param name="what">What to look for</param>
        /// <param name="startIndex">The start index of where to start looking</param>
        public static int LastIndexOfEx (this string self, string what)
        {
            return self.LastIndexOf (what, System.StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Returns true if string starts with the given string.
        /// </summary>
        /// <returns><c>true</c>, if this starts with what, <c>false</c> otherwise.</returns>
        /// <param name="self">The this parameter</param>
        /// <param name="what">What to check for</param>
        public static bool StartsWithEx (this string self, string what)
        {
            return self.StartsWith (what, System.StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Returns true if string ends with the given string.
        /// </summary>
        /// <returns><c>true</c>, if this ends with what, <c>false</c> otherwise.</returns>
        /// <param name="self">The this parameter</param>
        /// <param name="what">What to check for</param>
        public static bool EndsWithEx (this string self, string what)
        {
            return self.EndsWith (what, System.StringComparison.InvariantCulture);
        }
    }
}
