/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
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