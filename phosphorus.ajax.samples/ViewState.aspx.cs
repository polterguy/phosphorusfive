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

    public partial class ViewState : AjaxPage
    {
        protected pf.Literal content;
        protected pf.Literal txt;
        protected pf.Literal viewState;

        protected override void OnInit (EventArgs e)
        {
            content.EnableViewState = viewState.HasAttribute ("checked");
            txt.EnableViewState = viewState.HasAttribute ("checked");
            base.OnInit (e);
        }

        protected void submit_onclick (pf.Literal btn, EventArgs e)
        {
            content.innerHTML = txt.innerHTML.Replace (".  ", ".&nbsp;&nbsp;");
        }
        
        protected void changeClass_onclick (pf.Literal btn, EventArgs e)
        {
            content ["class"] = "green";
        }
        
        protected void makeInVisible_onclick (pf.Literal btn, EventArgs e)
        {
            content.Visible = false;
        }
        
        protected void makeVisible_onclick (pf.Literal btn, EventArgs e)
        {
            content.Visible = true;
        }
        
        protected void addOne_onclick (pf.Literal btn, EventArgs e)
        {
            if (!content.EnableViewState) {
                content.innerHTML = "this only works with viewstate enabled!!";
                content ["class"] = "red";
            } else {
                content.innerHTML += "X";
            }
        }
    }
}

