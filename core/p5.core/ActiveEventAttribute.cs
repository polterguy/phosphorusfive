/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;

/// <summary>
///     Main namespace for core Active Event features of Phosphorus Five
/// </summary>
namespace p5.core
{
    /// <summary>
    ///     Security association for Active Event
    /// </summary>
    public enum EventProtection
    {
        /// <summary>
        ///     Only native code can invoke this Event, and there can only be one handler
        /// </summary>
        NativeClosed,

        /// <summary>
        ///     Only native code can invoke this Event, and Event can be handled multiple times
        /// </summary>
        NativeOpen,

        /// <summary>
        ///     Both p5.lambda and native code can invoke this Active, but event cannot be handled multiple times
        /// </summary>
        LambdaClosed,

        /// <summary>
        ///     Both p5.lambda and native code can invoke this event, and there can exist multiple handlers for it, but only in native code
        /// </summary>
        LambdaClosedNativeOpen,

        /// <summary>
        ///     Both p5.lambda and native code can invoke and handle this Active, and event can be handled multiple times, also from lambda
        /// </summary>
        LambdaOpen
    }

    /// <summary>
    ///     Attribute used for marking your methods as Active Events
    /// </summary>
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
    public class ActiveEventAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.core.ActiveEventAttribute"/> class
        /// </summary>
        public ActiveEventAttribute()
        {
            // Creating strongest possible protection by default
            Protection = EventProtection.NativeClosed;
        }

        /// <summary>
        ///     The name of your Active Event
        /// </summary>
        /// <value>the name</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets the security entrance associated with Active Event
        /// </summary>
        /// <value>The entrance</value>
        public EventProtection Protection {
            get;
            set;
        }
    }
}
