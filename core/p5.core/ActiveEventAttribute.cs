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
        ///     Only native code (C#) can invoke this Event, and there can only be one handler
        /// </summary>
        Native,

        /// <summary>
        ///     Only native code (C#) can invoke this Event, and Event can be handled multiple times
        /// </summary>
        NativeOpen,

        /// <summary>
        ///     Both p5.lambda and C# code can invoke this Active, but event cannot be handled multiple times
        /// </summary>
        Lambda,

        /// <summary>
        ///     Both p5.lambda and C# code can invoke and handle this Active, and event can be handled multiple times
        /// </summary>
        LambdaOpen,

        /// <summary>
        ///     No protection at all
        /// </summary>
        None
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
            Protection = EventProtection.Native;
        }

        /// <summary>
        ///     The name of your Active Event
        /// </summary>
        /// <value>the name</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets the security entrance associated with Active Event
        /// </summary>
        /// <value>The entrance.</value>
        public EventProtection Protection {
            get;
            set;
        }
    }
}
