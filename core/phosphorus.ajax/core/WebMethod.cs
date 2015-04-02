/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;

namespace phosphorus.ajax.core
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