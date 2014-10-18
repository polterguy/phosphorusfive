/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;

namespace phosphorus.core
{
    [Serializable]
    public class Node
    {
        public Node ()
        {
            Children = new List<Node> ();
        }

        public Node (string name)
            : this ()
        {
            Name = name;
        }

        public Node (string name, string value)
            : this (name)
        {
            Value = value;
        }

        public string Name {
            get;
            set;
        }

        public object Value {
            get;
            set;
        }

        public List<Node> Children {
            get;
            private set;
        }
    }
}

