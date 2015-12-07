/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;

namespace p5.ajax.core
{
    /// <summary>
    ///     Attribute for methods you wish to use as web methods
    /// </summary>
    [AttributeUsage (AttributeTargets.Method)]
    public class WebMethod : Attribute
    { }
}