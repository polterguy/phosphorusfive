/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;

/// <summary>
///     Main namespace for core Active Event features of Phosphorus Five
/// </summary>
namespace p5.core
{
    /// <summary>
    ///     Attribute used for marking your methods as Active Events
    /// </summary>
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
    public class ActiveEventAttribute : Attribute
    {
        /// <summary>
        ///     The name of your Active Event
        /// </summary>
        /// <value>the name</value>
        public string Name { get; set; }
    }
}
