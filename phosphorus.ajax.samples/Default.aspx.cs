/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed license.txt file for details
 */

namespace phosphorus.ajax.samples
{
    using System;
    using System.Web;
    using System.Web.UI;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class Default : AjaxPage
    {
        protected void hello_onclick (pf.Literal sender, EventArgs e)
        {
            if (sender.innerHTML.Trim () == "click me") {
                sender.innerHTML = "hello world";
            } else {
                sender.innerHTML += ", hello world";
            }
        }
    }
}

