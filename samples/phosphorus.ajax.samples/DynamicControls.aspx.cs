
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.samples
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
        protected pf.Void update;
        private static int _next = 1;

        private string CurrentEdit {
            get { return ViewState ["CurrentEdit"] as string; }
            set { ViewState ["CurrentEdit"] = value; }
        }

        protected override void OnPreInit (EventArgs e)
        {
            base.OnPreInit (e);
        }

        protected override void OnPreRender (EventArgs e)
        {
            if (CurrentEdit != null)
                update.RemoveAttribute ("disabled");
            else
                update ["disabled"] = null;
            base.OnPreRender (e);
        }

        [WebMethod]
        protected void item_onclick (pf.Literal sender, EventArgs e)
        {
            if (sender.innerValue == "are you sure?") {
                list.RemoveControlPersistent (sender);
                CurrentEdit = null;
            } else {
                txt ["value"] = sender.innerValue;
                CurrentEdit = sender.ID;
                sender.innerValue = "are you sure?";
            }
        }
        
        [WebMethod]
        protected void append_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var widget = list.CreatePersistentControl<pf.Literal> ("x" + (_next ++), list.Controls.Count);
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerValue = txt ["value"];
        }
        
        [WebMethod]
        protected void insert_top_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var widget = list.CreatePersistentControl<pf.Literal> ("x" + (_next ++), 0);
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerValue = txt ["value"];
        }
        
        [WebMethod]
        protected void insert_at_random_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var widget = list.CreatePersistentControl<pf.Literal> ("x" + (_next ++), new Random ().Next (0, list.Controls.Count));
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerValue = txt ["value"];
        }
        
        [WebMethod]
        protected void replace_random_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            if (list.Controls.Count == 0) {
                txt ["value"] = "nothing to replace!!";
            } else {
                var which = new Random ().Next (0, list.Controls.Count);
                list.RemoveControlPersistentAt (which);

                var widget = list.CreatePersistentControl<pf.Literal> ("x" + (_next ++), which);
                widget.ElementType = "li";
                widget.RenderType = pf.Widget.RenderingType.NoClose;
                widget ["onclick"] = "item_onclick";
                widget.innerValue = txt ["value"];
            }
        }
        
        [WebMethod]
        protected void love_bomb_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var rnd = new Random ();
            foreach (var idx in list.GetChildControls<pf.Literal> ()) {
                if (rnd.Next (0, 3) == 1) {
                    idx.innerValue = "i like turtles!";
                    idx ["class"] = "turtles";
                }
            }
        }
        
        [WebMethod]
        protected void harvest_love_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var toRemove = new List<Control> ();
            foreach (var idx in list.GetChildControls<pf.Literal> ()) {
                if (idx.innerValue.Contains ("turtles")) {
                    toRemove.Add (idx);
                }
            }
            foreach (var idx in toRemove) {
                list.RemoveControlPersistent (idx);
            }
        }
        
        [WebMethod]
        protected void update_onclick (pf.Void btn, EventArgs e)
        {
            var liter = list.FindControl (CurrentEdit) as pf.Literal;
            liter.innerValue = txt ["value"];
            CurrentEdit = null;
        }
    }
}
