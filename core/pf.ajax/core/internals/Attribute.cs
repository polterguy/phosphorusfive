/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

/// <summary>
///     Namespace internally used by pf.ajax.
/// 
///     This is not a place you need to fiddle with, or study to understand, since this is internally used by 
///     the framework, and automatically taken care of for you. However, it contains the logic necessary to serialize
///     attribute for your widgets, and persist their values across HTTP requests.
/// </summary>
namespace pf.ajax.core.internals
{
    /// <summary>
    ///     Class encapsulating one attribute for Ajax widgets. You rarely, if ever, have to fiddle with this class yourself.
    /// </summary>
    internal class Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the Attribute class.
        /// </summary>
        /// <param name="name">Name of your attribute.</param>
        public Attribute (string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Initializes a new instance of the Attribute class.
        /// </summary>
        /// <param name="name">Name of your attribute.</param>
        /// <param value="name">Value of your attribute.</param>
        internal Attribute (string name, string value)
            : this (name) { Value = value; }

        /// <summary>
        ///     Gets the name of your attribute.
        /// </summary>
        /// <value>The name.</value>
        internal string Name { get; private set; }

        /// <summary>
        ///     Gets the value of your attribute.
        /// </summary>
        /// <value>The value.</value>
        internal string Value { get; private set; }
    }
}
