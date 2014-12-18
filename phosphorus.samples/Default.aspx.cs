/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.samples
{
    using System;
    using System.Web;
    using System.Web.UI;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class Default : AjaxPage
    {
        [WebMethod]
        protected void hello_onclick (pf.Literal sender, EventArgs e)
        {
            if (sender.innerValue == "click me") {
                sender.innerValue = "hello world";
                sender ["class"] = "change-is-the-only-constant";
            } else {
                sender.innerValue = "click me";
                sender.RemoveAttribute ("class");
            }
        }
    }
}

