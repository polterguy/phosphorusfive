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

    public partial class Attributes : AjaxPage
    {
        protected pf.Literal literal;

        protected override void OnLoad (EventArgs e)
        {
            if (!IsPostBack)
                literal.Tag = "p";
            base.OnLoad (e);
        }

        protected void addClass_onclick (pf.Literal btn, EventArgs e)
        {
            if (btn ["value"] == "add") {
                literal ["class"] = "green";
                btn ["value"] = "remove";
            } else {
                literal.RemoveAttribute ("class");
                btn ["value"] = "add";
            }
        }
        
        protected void changeTag_onclick (pf.Literal btn, EventArgs e)
        {
            if (btn ["value"] == "change") {
                literal.Tag = "div";
                btn ["value"] = "change back";
            } else {
                literal.Tag = "p";
                btn ["value"] = "change";
            }
        }
        
        protected void visibleChange_onclick (pf.Literal btn, EventArgs e)
        {
            if (btn ["value"] == "make element invisible") {
                literal.Visible = false;
                btn ["value"] = "make element visible";
            } else {
                literal.Visible = true;
                btn ["value"] = "make element invisible";
            }
        }
    }
}

