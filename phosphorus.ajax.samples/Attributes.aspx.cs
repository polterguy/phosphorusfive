/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed readme.me file for details
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

        protected void toggleClass_onclick (pf.Literal btn, EventArgs e)
        {
            if (literal.HasAttribute ("class"))
                literal.RemoveAttribute ("class");
            else
                literal ["class"] = "green";
        }
        
        protected void changeTag_onclick (pf.Literal btn, EventArgs e)
        {
            if (literal.Tag == "p")
                literal.Tag = "div";
            else
                literal.Tag = "p";
        }
        
        protected void toggleVisibility_onclick (pf.Literal btn, EventArgs e)
        {
            if (literal.Visible)
                literal.Visible = false;
            else
                literal.Visible = true;
        }
    }
}

