/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using p5.ajax.core;

namespace p5.samples
{
    using p5 = p5.ajax.widgets;

    public partial class Default : AjaxPage
    {
        [WebMethod]
        protected void hello_onclick (p5.Literal sender, EventArgs e)
        {
            if (sender.innerValue == "Click me for hello world") {
                sender.innerValue = "Click me again, while inspecting the HTTP request using your browser";
                sender ["class"] = "change-is-the-only-constant";
            } else {
                sender.innerValue = "Click me for hello world";
                sender.RemoveAttribute ("class");
            }
        }
    }
}