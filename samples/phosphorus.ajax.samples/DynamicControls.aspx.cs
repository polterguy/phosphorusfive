/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Web.UI;
using phosphorus.ajax.core;

// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace phosphorus.five.samples
{
    using pf = ajax.widgets;

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class DynamicControls : AjaxPage
    {
        private static int _next = 1;
        protected pf.Container List;
        protected pf.Void Txt;
        protected pf.Void Update;

        private string CurrentEdit
        {
            get { return ViewState ["CurrentEdit"] as string; }
            set { ViewState ["CurrentEdit"] = value; }
        }

        protected override void OnPreRender (EventArgs e)
        {
            if (CurrentEdit != null)
                Update.RemoveAttribute ("disabled");
            else
                Update ["disabled"] = null;
            base.OnPreRender (e);
        }

        [WebMethod]
        protected void item_onclick (pf.Literal sender, EventArgs e)
        {
            if (sender.innerValue == "are you sure?") {
                List.RemoveControlPersistent (sender);
                CurrentEdit = null;
            } else {
                Txt ["value"] = sender.innerValue;
                CurrentEdit = sender.ID;
                sender.innerValue = "are you sure?";
            }
        }

        [WebMethod]
        protected void append_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var widget = List.CreatePersistentControl<pf.Literal> ("x" + (_next ++), List.Controls.Count);
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerValue = Txt ["value"];
        }

        [WebMethod]
        protected void insert_top_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var widget = List.CreatePersistentControl<pf.Literal> ("x" + (_next ++), 0);
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerValue = Txt ["value"];
        }

        [WebMethod]
        protected void insert_at_random_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var widget = List.CreatePersistentControl<pf.Literal> ("x" + (_next ++), new Random ().Next (0, List.Controls.Count));
            widget.ElementType = "li";
            widget.RenderType = pf.Widget.RenderingType.NoClose;
            widget ["onclick"] = "item_onclick";
            widget.innerValue = Txt ["value"];
        }

        [WebMethod]
        protected void replace_random_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            if (List.Controls.Count == 0) {
                Txt ["value"] = "nothing to replace!!";
            } else {
                var which = new Random ().Next (0, List.Controls.Count);
                List.RemoveControlPersistentAt (which);

                var widget = List.CreatePersistentControl<pf.Literal> ("x" + (_next ++), which);
                widget.ElementType = "li";
                widget.RenderType = pf.Widget.RenderingType.NoClose;
                widget ["onclick"] = "item_onclick";
                widget.innerValue = Txt ["value"];
            }
        }

        [WebMethod]
        protected void love_bomb_onclick (pf.Void btn, EventArgs e)
        {
            CurrentEdit = null;
            var rnd = new Random ();
            foreach (var idx in List.GetChildControls<pf.Literal> ()) {
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
            var toRemove = List.GetChildControls<pf.Literal> ().Where (idx => idx.innerValue.Contains ("turtles")).Cast<Control> ().ToList ();
            foreach (var idx in toRemove) {
                List.RemoveControlPersistent (idx);
            }
        }

        [WebMethod]
        protected void update_onclick (pf.Void btn, EventArgs e)
        {
            var liter = (pf.Literal) List.FindControl (CurrentEdit);
            liter.innerValue = Txt ["value"];
            CurrentEdit = null;
        }
    }
}