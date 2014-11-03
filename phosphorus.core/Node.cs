/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;

namespace phosphorus.core
{
    /// <summary>
    /// arguments passed into and returned from Active Events
    /// </summary>
    [Serializable]
    public class Node
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        public Node ()
        { }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        /// <param name="name">name of node</param>
        public Node (string name)
            : this ()
        {
            Name = name;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.Node"/> class
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="value">value of node</param>
        public Node (string name, string value)
            : this (name)
        {
            Value = value;
        }

        /// <summary>
        /// gets or sets the name of the node
        /// </summary>
        /// <value>the name</value>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// gets or sets the value of the node
        /// </summary>
        /// <value>the value</value>
        public object Value {
            get;
            set;
        }
    }
}

