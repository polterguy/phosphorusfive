/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed readme.me file for details
 */

using System;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// class encapsulating one attribute for widgets
    /// </summary>
    [Serializable]
    public class Attribute
    {
        [NonSerialized]
        private bool _dirty;

        [NonSerialized]
        private bool _inViewState;

        [NonSerialized]
        private string _oldValue;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Attribute"/> class
        /// </summary>
        public Attribute ()
        { }

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
        
        internal bool Dirty {
            get { return _dirty; }
            set { _dirty = value; }
        }
        
        internal bool InViewState {
            get { return _inViewState; }
            set { _inViewState = value; }
        }

        internal string OldValue {
            get { return _oldValue; }
            set { _oldValue = value; }
        }
    }
}

