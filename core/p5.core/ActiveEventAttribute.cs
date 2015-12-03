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
    public enum EntranceProtection
    {
        /// <summary>
        ///     Only native code (C#) can invoke this Active Event
        /// </summary>
        NativeOnly,

        /// <summary>
        ///     Only native code (C#) can invoke this Active Event, and Event can be handled multiple times
        /// </summary>
        NativeOnlyVirtual,

        /// <summary>
        ///     Both p5.lambda and C# code can invoke this Active Event
        /// </summary>
        Lambda,

        /// <summary>
        ///     Both p5.lambda and C# code can invoke this Active Event, and Event can be handled multiple times and/or reset
        /// </summary>
        LambdaVirtual
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
            Protection = EntranceProtection.NativeOnly;
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
        public EntranceProtection Protection {
            get;
            set;
        }
    }
}
