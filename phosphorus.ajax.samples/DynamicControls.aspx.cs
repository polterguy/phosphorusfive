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

    public partial class DynamicControls : AjaxPage
    {
        protected pf.Container list;
        protected pf.Void txt;

        [WebMethod]
        protected void item_onclick (pf.Literal literal, EventArgs e)
        {
            literal.Parent.Controls.Remove (literal);
        }
        
        [WebMethod]
        protected void insert_onclick (pf.Void btn, EventArgs e)
        {
            pf.Literal widget = list.CreatePersistentControl<pf.Literal> (null);
            widget.Tag = "li";
            widget.HasEndTag = false;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
        }
    }
}

