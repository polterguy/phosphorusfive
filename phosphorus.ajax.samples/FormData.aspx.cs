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

    public partial class FormData : AjaxPage
    {
        protected pf.Literal lbl;

        protected void txt_onchange (pf.Widget sender, EventArgs e)
        {
            lbl.innerHTML = "value of textbox was; '" + sender ["value"] + "'";
        }
    }
}

