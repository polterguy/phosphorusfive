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

namespace p5.ajax.core
{
    /// <summary>
    ///     Utility class, containing useful helpers for p5.ajax
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Determines whether the given name is a legal name for a clr method
        /// </summary>
        /// <returns><c>true</c> if name is legal method name for clr; otherwise, <c>false</c></returns>
        /// <param name="name">Name</param>
        public static bool IsLegalMethodName (string name)
        {
            return name.All (idxChar => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_".IndexOf (idxChar) != -1);
        }
    }
}