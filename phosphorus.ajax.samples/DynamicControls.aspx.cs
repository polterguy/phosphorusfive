/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.ajax.samples
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Collections.Generic;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class DynamicControls : AjaxPage
    {
        protected pf.Container list;
        protected pf.Void txt;

        [WebMethod]
        protected void item_onclick (pf.Literal literal, EventArgs e)
        {
            if (literal.innerHTML == "are you sure?") {
                literal.Parent.Controls.Remove (literal);
            } else {
                literal.innerHTML = "are you sure?";
            }
        }
        
        [WebMethod]
        protected void append_onclick (pf.Void btn, EventArgs e)
        {
            // to make sure our new control does not get the same ID as other existing controls, we explicitly create one
            string id = Guid.NewGuid ().ToString ().Replace ("-", "");
            pf.Literal widget = list.CreatePersistentControl<pf.Literal> (id);
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
        }
        
        [WebMethod]
        protected void insert_top_onclick (pf.Void btn, EventArgs e)
        {
            // to make sure our new control does not get the same ID as other existing controls, we explicitly create one
            string id = Guid.NewGuid ().ToString ().Replace ("-", "");
            pf.Literal widget = list.CreatePersistentControl<pf.Literal> (id, 0);
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
        }
        
        [WebMethod]
        protected void insert_at_random_onclick (pf.Void btn, EventArgs e)
        {
            // to make sure our new control does not get the same ID as other existing controls, we explicitly create one
            string id = Guid.NewGuid ().ToString ().Replace ("-", "");
            pf.Literal widget = list.CreatePersistentControl<pf.Literal> (id, new Random ().Next (0, list.Controls.Count));
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
        }
        
        [WebMethod]
        protected void replace_random_onclick (pf.Void btn, EventArgs e)
        {
            if (list.Controls.Count == 0) {
                txt ["value"] += " - could not replace, nothing to replace. appended instead";
                append_onclick (btn, e);
            } else {
                int which = new Random ().Next (0, list.Controls.Count - 1);
                list.Controls.RemoveAt (which);

                // to make sure our new control does not get the same ID as other existing controls, we explicitly create one
                string id = Guid.NewGuid ().ToString ().Replace ("-", "");
                pf.Literal widget = list.CreatePersistentControl<pf.Literal> (id, which);
                widget.ElementType = "li";
                widget.RenderType = pf.Widget.RenderingType.NoClose;
                widget ["onclick"] = "item_onclick";
                widget.innerHTML = txt ["value"];
            }
        }
        
        [WebMethod]
        protected void love_bomb_onclick (pf.Void btn, EventArgs e)
        {
            Random rnd = new Random ();
            foreach (Control idx in list.Controls) {
                pf.Literal lit = idx as pf.Literal;
                if (lit != null && rnd.Next (0, 3) == 1) {
                    lit.innerHTML = "i like turtles!";
                    lit ["class"] = "turtles";
                }
            }
        }
        
        [WebMethod]
        protected void harvest_love_onclick (pf.Void btn, EventArgs e)
        {
            List<Control> toRemove = new List<Control> ();
            foreach (Control idx in list.Controls) {
                pf.Literal lit = idx as pf.Literal;
                if (lit.innerHTML.Contains ("turtles")) {
                    toRemove.Add (lit);
                }
            }
            foreach (Control idx in toRemove) {
                idx.Parent.Controls.Remove (idx);
            }
        }
    }
}

