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
        protected pf.Void viewState;

        protected override void OnInit (EventArgs e)
        {
            content.EnableViewState = viewState.HasAttribute ("checked");
            txt.EnableViewState = viewState.HasAttribute ("checked");
            base.OnInit (e);
        }

        [WebMethod]
        protected void submit_onclick (pf.Void btn, EventArgs e)
        {
            content.innerHTML = txt.innerHTML.Replace (".  ", ".&nbsp;&nbsp;");
        }
        
        [WebMethod]
        protected void changeClass_onclick (pf.Void btn, EventArgs e)
        {
            content ["class"] = "green";
        }
        
        [WebMethod]
        protected void makeInVisible_onclick (pf.Void btn, EventArgs e)
        {
            content.Visible = false;
        }
        
        [WebMethod]
        protected void makeVisible_onclick (pf.Void btn, EventArgs e)
        {
            content.Visible = true;
        }
        
        [WebMethod]
        protected void addOne_onclick (pf.Void btn, EventArgs e)
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

