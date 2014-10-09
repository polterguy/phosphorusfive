/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.ajax.core.internals
{
    /// <summary>
    /// class encapsulating one attribute for widgets
    /// </summary>
    public class Attribute
    {
        public Attribute ()
        { }

        public Attribute (string name)
            : this ()
        {
            Name = name;
        }

        public Attribute (string name, string value)
            : this (name)
        {
            Value = value;
        }

        public string Name {
            get;
            set;
        }

        public string Value {
            get;
            set;
        }
    }
}

