/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.expressions
{
    /// <summary>
    ///     Contains extension methods for the Node class.
    /// 
    ///     Contains useful extension methods for Nodes
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        ///     Returns the evaluated value of the given node's child node.
        /// 
        ///     Evaluates node's speecified children node's value with XUtil.Single and returns the result.
        /// </summary>
        /// <returns>The evaluated result.</returns>
        /// <param name="node">Node.</param>
        /// <param name="name">Name of child node to evaluate.</param>
        /// <param name="context">Application context.</param>
        /// <param name="defaultValue">Default value to return.</param>
        /// <typeparam name="T">The type to return the evaluated result as.</typeparam>
        public static T GetExChildValue<T> (this Node node, string name, ApplicationContext context, T defaultValue = default(T))
        {
            if (node [name] == null || node [name].Value == null)
                return defaultValue;
            return XUtil.Single<T> (node.Value, node, context, defaultValue);
        }

        /// <summary>
        ///     Returns the evaluated value of the given node.
        /// 
        ///     Evaluates the given node's value with XUtil.Single, and returns the result.
        /// </summary>
        /// <returns>The evaluated result of the node's value.</returns>
        /// <param name="node">Node to evaluate.</param>
        /// <param name="context">Application context.</param>
        /// <param name="defaultValue">Default value to return.</param>
        /// <typeparam name="T">The type to return the evaluated result as.</typeparam>
        public static T GetExValue<T> (this Node node, ApplicationContext context, T defaultValue = default(T))
        {
            return XUtil.Single<T> (node.Value, node, context, defaultValue);
        }
    }
}
