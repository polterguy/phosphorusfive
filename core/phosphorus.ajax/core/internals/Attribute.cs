/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

/// <summary>
///     Namespace internally used by Phosphorus.Ajax to among other things keep track of attributes for widgets. This is
///     a place you do not need to fiddle with, or understand, since this is internally used by the framework automatically for you.
/// </summary>
namespace phosphorus.ajax.core.internals
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
