/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
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
            return XUtil.Single<T> (context, node [name], node [name], false, defaultValue);
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
            return XUtil.Single<T> (context, node, node, false, defaultValue);
        }
    }
}
