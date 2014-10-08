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
        protected pf.Container position;
        protected pf.Literal before;
        protected pf.Literal after;
        protected pf.Container child;
        protected pf.Void txt;

        protected override void OnLoad (EventArgs e)
        {
            int idxNo = 0;
            foreach (Control idx in list.Controls) {
                pf.Literal lit = idx as pf.Literal;
                if (lit != null) {
                    idxNo += 1;

                    // dynamically creating list item to decide where to insert new control
                    pf.Literal li = new pf.Literal ();
                    li.Tag = "option";
                    li["value"] = lit.ID;
                    li.innerHTML = idxNo.ToString () + " - (" + lit.innerHTML + ")";
                    child.Controls.Add (li);
                }
            }
            base.OnLoad (e);
        }

        [WebMethod]
        protected void item_onclick (pf.Literal literal, EventArgs e)
        {
            list.Controls.Remove (literal);
        }
        
        [WebMethod]
        protected void insert_onclick (pf.Void literal, EventArgs e)
        {
            bool isBefore = before.HasAttribute ("selected");
            string childId = null;
            int idxNo = 0;
            bool hasListItems = false;
            foreach (Control idxChild in child.Controls) {
                pf.Literal childLi = idxChild as pf.Literal;
                idxNo += 1;
                if (childLi != null) {
                    hasListItems = true;
                    if (childLi.HasAttribute ("selected")) {
                        childId = childLi.ID;
                        break;
                    }
                }
            }

            // we now know where to insert the control [before] and at which position [childId] we should insert out control
            pf.Literal widget = new pf.Literal();
            widget.Tag = "li";
            widget.HasEndTag = false;
            widget ["onclick"] = "item_onclick";
            widget.innerHTML = txt ["value"];
            if (hasListItems && isBefore) {
                idxNo -= 1;
            }
            list.Controls.AddAt (idxNo, widget);
        }
    }
}

