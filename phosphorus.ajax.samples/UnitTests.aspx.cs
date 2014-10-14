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

    public partial class UnitTests : AjaxPage
    {
        [WebMethod]
        protected void sandbox_invoke_empty_onclick (pf.Literal literal, EventArgs e)
        {
            if (literal.ID != "sandbox_invoke_empty")
                throw new ApplicationException ("wrong id of element on server");
        }

        [WebMethod]
        protected void sandbox_invoke_exception_onclick (pf.Literal literal, EventArgs e)
        {
            throw new ApplicationException ("this is an intentional error");
        }
        
        [WebMethod]
        protected void sandbox_invoke_change_content_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = "new value";
        }
        
        [WebMethod]
        protected void sandbox_invoke_change_two_properties_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] = "new value 1";
            literal.innerHTML = "new value 2";
        }
        
        [WebMethod]
        protected void sandbox_invoke_add_remove_1_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] = "new value 1";
        }
        
        [WebMethod]
        protected void sandbox_invoke_add_remove_2_onclick (pf.Literal literal, EventArgs e)
        {
            literal.RemoveAttribute ("class");
        }
        
        [WebMethod]
        protected void sandbox_invoke_add_remove_same_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] = "mumbo-jumbo";
            literal.RemoveAttribute ("class");
        }
        
        [WebMethod]
        protected void sandbox_invoke_change_twice_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] = "mumbo";
            literal ["class"] = "jumbo";
        }
    }
}

