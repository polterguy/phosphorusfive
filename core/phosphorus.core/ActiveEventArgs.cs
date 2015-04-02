/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;

namespace phosphorus.core
{
    public class ActiveEventArgs : EventArgs
    {
        internal ActiveEventArgs (string name, Node args, ActiveEventArgs baseEvent = null)
        {
            Args = args;
            Name = name;
            Base = baseEvent;
        }

        public Node Args { get; private set; }

        public string Name { get; private set; }

        public ActiveEventArgs Base { get; private set; }
    }
}
