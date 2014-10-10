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
        protected void append_onclick (pf.Void btn, EventArgs e)
        {
            pf.Literal widget = list.CreatePersistentControl<pf.Literal> (null);
            widget.Tag = "li";
            widget.HasEndTag = false;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
        }
        
        [WebMethod]
        protected void insert_top_onclick (pf.Void btn, EventArgs e)
        {
            pf.Literal widget = list.CreatePersistentControl<pf.Literal> (null, 0);
            widget.Tag = "li";
            widget.HasEndTag = false;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
        }
        
        [WebMethod]
        protected void insert_at_random_onclick (pf.Void btn, EventArgs e)
        {
            Random rnd = new Random ();
            pf.Literal widget = list.CreatePersistentControl<pf.Literal> (null, rnd.Next (0, list.Controls.Count));
            widget.Tag = "li";
            widget.HasEndTag = false;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
        }
        
        [WebMethod]
        protected void replace_random_onclick (pf.Void btn, EventArgs e)
        {
            Random rnd = new Random ();
            int which = rnd.Next (0, list.Controls.Count - 1);
            list.Controls.RemoveAt (which);

            // to make sure our new control does not get the same ID as other existing controls, we explicitly give 
            // it an ID here, even though we don't need to give it an ID in the other methods
            pf.Literal widget = list.CreatePersistentControl<pf.Literal> (Guid.NewGuid ().ToString ().Replace ("-", ""), which);
            widget.Tag = "li";
            widget.HasEndTag = false;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
        }
    }
}

