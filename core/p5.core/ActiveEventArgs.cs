/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;

namespace p5.core
{
    /// <summary>
    ///     Active Event arguments
    /// </summary>
    public class ActiveEventArgs : EventArgs
    {
        /*
         * initializes a new instance of this class
         */
        internal ActiveEventArgs (string name, Node args)
        {
            Args = args;
            Name = name;
        }

        /// <summary>
        ///     Arguments passed in and returned from Active Events
        /// </summary>
        /// <value>the arguments</value>
        public Node Args
        {
            get;
            private set;
        }

        /// <summary>
        ///     Name of the Active Event raised
        /// </summary>
        /// <value>the name</value>
        public string Name
        {
            get;
            private set;
        }
    }
}
