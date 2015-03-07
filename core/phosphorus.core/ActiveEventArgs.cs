/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.core
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
        internal ActiveEventArgs (string name, Node args, ActiveEventArgs baseEvent = null)
        {
            Args = args;
            Name = name;
            Base = baseEvent;
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

        /// <summary>
        ///     Returns the base Active Event for the currently raised Active Event.
        /// 
        ///     If your Active Event is overriding another Active Event, then this is the EventArgs for the base event
        ///     implementation. If you wish to invoke your base Active Event, you can do so using ApplicationContent.CallBase,
        ///     passing in this property.
        /// </summary>
        /// <value>the base Active Event</value>
        public ActiveEventArgs Base { get; private set; }
    }
}
