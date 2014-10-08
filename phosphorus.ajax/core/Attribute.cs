/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.ajax.core
{
    /// <summary>
    /// class encapsulating one attribute for widgets
    /// </summary>
    public class Attribute
    {
        // helper for rendering attributes back to client correctly
        internal enum AttributeState
        {
            Dirty = 1, // meaning, updates needs to be sent to client
            Removed = 2, // meaning it was removed
            ExistedBeforeViewstate = 4 // helps determine if attribute existed before viewstate was finished loading
        };

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Attribute"/> class
        /// </summary>
        public Attribute ()
        {
            State = 0;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Attribute"/> class
        /// </summary>
        /// <param name="name">name of attribute</param>
        public Attribute (string name)
            : this ()
        {
            Name = name;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Attribute"/> class
        /// </summary>
        /// <param name="name">name of attribute</param>
        /// <param name="value">value of attribute</param>
        public Attribute (string name, string value)
            : this (name)
        {
            Value = value;
        }

        /// <summary>
        /// gets or sets the name
        /// </summary>
        /// <value>the name</value>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// gets or sets the value
        /// </summary>
        /// <value>the value</value>
        public string Value {
            get;
            set;
        }

        internal string OldValue {
            get;
            set;
        }

        internal string ValueBeforeTracking {
            get;
            set;
        }

        internal int State {
            get;
            set;
        }
    }
}

