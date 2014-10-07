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

    public partial class Events : AjaxPage
    {
        protected pf.Literal li4_span;

        [WebMethod]
        protected void li1_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = "widget was clicked";
        }
        
        [WebMethod]
        protected void li2_onmouseover (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = "widget has mouse over it";
        }
        
        [WebMethod]
        protected void li2_onmouseout (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = "widget had mouse moved away from it";
        }
        
        [WebMethod]
        protected void li3_onmousedown (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = "widget has mouse clicking it";
        }
        
        [WebMethod]
        protected void li3_onmouseup (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = "widget had mouse release button";
        }
        
        [WebMethod]
        protected void li4_txt_onchange (pf.Literal literal, EventArgs e)
        {
            li4_span.innerHTML = "new text: '" + literal ["value"] + "'";
        }
    }
}

