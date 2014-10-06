/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed readme.me file for details
 */

namespace phosphorus.ajax.samples
{
    using System;
    using System.Web;
    using System.Web.UI;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class JavaScript : AjaxPage
    {
        protected void javascript_widget_onclicked (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = Page.Request.Params ["custom_data"] + ", your server says; 'hi dude'";
        }
    }
}

