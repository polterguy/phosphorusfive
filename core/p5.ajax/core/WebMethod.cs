/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;

namespace p5.ajax.core
{
    /// <summary>
    ///     Attribute for marking methods as web methods
    /// 
    ///     By marking your methods with this attribute, it becomes possible to invoke your methods 
    ///     from JavaScript, from the client-side, through Ajax.
    /// </summary>
    [AttributeUsage (AttributeTargets.Method)]
    public class WebMethod : Attribute {}
}