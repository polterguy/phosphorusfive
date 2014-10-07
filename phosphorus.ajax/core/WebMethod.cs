/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.ajax.core
{
    /// <summary>
    /// attribute for marking methods as web methods, possible to invoke from javascript on client side through ajax
    /// </summary>
    [AttributeUsage (AttributeTargets.Method)]
    public class WebMethod : Attribute
    { }
}

