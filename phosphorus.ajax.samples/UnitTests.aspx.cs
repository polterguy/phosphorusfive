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
        
        //[WebMethod] intentionally commented out
        protected void sandbox_invoke_no_webmethod_onclick (pf.Literal literal, EventArgs e)
        {
        }

        [WebMethod]
        protected void sandbox_invoke_normal_onclick (pf.Literal literal, EventArgs e)
        {
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
        
        [WebMethod]
        protected void sandbox_invoke_change_markup_attribute_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] = "bar";
        }
        
        [WebMethod]
        protected void sandbox_invoke_remove_markup_attribute_onclick (pf.Literal literal, EventArgs e)
        {
            literal.RemoveAttribute ("class");
        }
        
        [WebMethod]
        protected void sandbox_invoke_remove_add_markup_attribute_1_onclick (pf.Literal literal, EventArgs e)
        {
            literal.RemoveAttribute ("class");
        }
        
        [WebMethod]
        protected void sandbox_invoke_remove_add_markup_attribute_2_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] = "bar";
        }
        
        [WebMethod]
        protected void sandbox_invoke_concatenate_long_attribute_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] += "qwerty";
        }
        
        [WebMethod]
        protected void sandbox_invoke_create_concatenate_long_attribute_1_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] = "x1234567890";
        }
        
        [WebMethod]
        protected void sandbox_invoke_create_concatenate_long_attribute_2_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] += "abcdefghijklmnopqrstuvwxyz";
        }
        
        [WebMethod]
        protected void sandbox_invoke_change_container_child_child_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] += "bar";
        }
        
        [WebMethod]
        protected void sandbox_invoke_make_container_visible_onclick (pf.Container container, EventArgs e)
        {
            container.Visible = true;
        }
        
        [WebMethod]
        protected void sandbox_invoke_make_container_visible_child_invisible_1_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> literals = new List<pf.Literal> (container.GetControls<pf.Literal> ());
            literals [0].Visible = false;
        }
        
        [WebMethod]
        protected void sandbox_invoke_make_container_visible_child_invisible_2_onclick (pf.Container container, EventArgs e)
        {
            container.Visible = true;
        }
    }
}

