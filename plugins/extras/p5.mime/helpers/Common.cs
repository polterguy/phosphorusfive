/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using p5.core;

namespace p5.mime.helpers
{
    /// <summary>
    ///     Common helper class for mime features of Phosphorus Five
    /// </summary>
    internal static class Common
    {
        /// <summary>
        ///     Returns base folder for application
        /// </summary>
        /// <returns>The base folder</returns>
        /// <param name="context">Application Context</param>
        public static string GetRootFolder (ApplicationContext context)
        {
            return context.Raise (".p5.core.application-folder").Get<string> (context);
        }
    }
}

