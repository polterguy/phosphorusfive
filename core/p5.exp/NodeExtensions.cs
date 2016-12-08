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

using p5.core;

namespace p5.exp
{
    /// <summary>
    ///     Contains extension methods for the Node class
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        ///     Returns the evaluated value of the given node's child node
        /// </summary>
        /// <returns>The evaluated result</returns>
        /// <param name="node">Node</param>
        /// <param name="name">Name of child node to evaluate</param>
        /// <param name="context">Application Context</param>
        /// <param name="defaultValue">Default value to return</param>
        /// <typeparam name="T">The type to return the evaluated result as</typeparam>
        public static T GetExChildValue<T> (
            this Node node, 
            string name, 
            ApplicationContext context, 
            T defaultValue = default(T))
        {
            if (node [name] == null || node [name].Value == null)
                return defaultValue;
            return XUtil.Single<T> (context, node [name], node [name], defaultValue);
        }

        /// <summary>
        ///     Returns the evaluated value of the given node
        /// </summary>
        /// <returns>The evaluated result of the node's value</returns>
        /// <param name="node">Node to evaluate</param>
        /// <param name="context">Application Context</param>
        /// <param name="defaultValue">Default value to return</param>
        /// <param name="inject">String to inject between entities of expression</param>
        /// <typeparam name="T">The type to return the evaluated result as</typeparam>
        public static T GetExValue<T> (
            this Node node, 
            ApplicationContext context, 
            T defaultValue = default(T))
        {
            if (node.Value == null)
                return defaultValue;
            return XUtil.Single (context, node, node, defaultValue);
        }
    }
}
