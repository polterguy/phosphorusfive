/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.core
{
    /// <summary>
    /// Active Event arguments
    /// </summary>
    public class ActiveEventArgs : EventArgs
    {
        internal ActiveEventArgs (Node args, string name)
        {
            Args = args;
            Name = name;
        }

        /// <summary>
        /// Arguments passed in and returned from Active Events
        /// </summary>
        /// <value>the arguments</value>
        public Node Args {
            get;
            private set;
        }

        public string Name {
            get;
            private set;
        }
    }
}

