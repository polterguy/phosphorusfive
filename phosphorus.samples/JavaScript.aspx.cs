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

    public partial class JavaScript : AjaxPage
    {
        [WebMethod]
        protected void javascript_widget_onclicked (pf.Literal literal, EventArgs e)
        {
            literal.innerValue = Page.Request.Params ["custom_data"] + "your server says; 'hello'. ";
        }
    }
}

