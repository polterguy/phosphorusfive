/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.ajax.samples
{
    using System;
    using System.Web;
    using System.Web.UI;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class FormData : AjaxPage
    {
        protected pf.Literal lbl;
        protected pf.Void txt;

        [WebMethod]
        protected void txt_onchange (pf.Widget sender, EventArgs e)
        {
            lbl.innerHTML = "value of textbox was; '" + sender ["value"] + "'";
        }
        
        [WebMethod]
        protected void btn_onclick (pf.Widget sender, EventArgs e)
        {
            txt ["value"] = "some new text";
            txt_onchange (txt, e);
        }
    }
}

