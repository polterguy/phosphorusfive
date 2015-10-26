/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;

namespace pf.core
{
    /// <summary>
    ///     Active Event arguments.
    /// 
    ///     This is the type of EventArgs passed into all of your Active Events. To retrieve parameters passed into, or return values
    ///     from your Active Events, use the Args property.
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
        ///     Arguments passed in and returned from Active Events.
        /// 
        ///     Use this property to return values, or access parameters passed into your Active events.
        /// </summary>
        /// <value>the arguments</value>
        public Node Args { get; private set; }

        /// <summary>
        ///     Name of the Active Event raised.
        /// 
        ///     This is the name of the Active Event that was being raised.
        /// </summary>
        /// <value>the name</value>
        public string Name { get; private set; }
    }
}
