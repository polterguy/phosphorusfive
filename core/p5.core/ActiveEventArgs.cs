/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
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
        internal ActiveEventArgs (string name, Node args, ApplicationContext.ContextTicket ticket)
        {
            Args = args;
            Name = name;
            Ticket = ticket;
        }

        /// <summary>
        ///     Arguments passed in and returned from Active Events
        /// </summary>
        /// <value>the arguments</value>
        public Node Args { get; private set; }

        /// <summary>
        ///     Name of the Active Event raised
        /// </summary>
        /// <value>the name</value>
        public string Name { get; private set; }

        public ApplicationContext.ContextTicket Ticket { get; private set; }
    }
}
