/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using phosphorus.ajax.core;

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedMember.Global

namespace phosphorus.five.samples
{
    using pf = ajax.widgets;

    public partial class Default : AjaxPage
    {
        [WebMethod]
        protected void hello_onclick (pf.Literal sender, EventArgs e)
        {
            if (sender.innerValue == "click me for hello world") {
                sender.innerValue = "click me again, while inspecting the HTTP request using your browser";
                sender ["class"] = "change-is-the-only-constant";
            } else {
                sender.innerValue = "click me for hello world";
                sender.RemoveAttribute ("class");
            }
        }
    }
}