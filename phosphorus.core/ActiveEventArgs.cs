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
        /*
         * initializes a new instance of this class
         */
        internal ActiveEventArgs (string name, Node args, ActiveEventArgs baseEvent = null)
        {
            Args = args;
            Name = name;
            Base = baseEvent;
        }

        /// <summary>
        /// arguments passed in and returned from Active Events
        /// </summary>
        /// <value>the arguments</value>
        public Node Args {
            get;
            set;
        }

        /// <summary>
        /// name of the Active Event
        /// </summary>
        /// <value>the name</value>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// returns the base Active Event for the currently raised Active Event, if any
        /// </summary>
        /// <value>the base Active Event</value>
        public ActiveEventArgs Base {
            get;
            private set;
        }
    }
}

