/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

namespace phosphorus.ajax.core.internals
{
    /// <summary>
    ///     class encapsulating one attribute for widgets
    /// </summary>
    public class Attribute
    {
        private Attribute () { }

        public Attribute (string name)
            : this () { Name = name; }

        public Attribute (string name, string value)
            : this (name) { Value = value; }

        public string Name { get; private set; }
        public string Value { get; private set; }
    }
}