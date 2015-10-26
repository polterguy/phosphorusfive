/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using pf.ajax.core;

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace phosphorus.five.samples
{
    using pf = pf.ajax.widgets;

    public partial class UnitTests : AjaxPage
    {
        [WebMethod]
        protected void sandbox_invoke_empty_onclick (pf.Literal literal, EventArgs e)
        {
            if (literal.ID != "sandbox_invoke_empty")
                throw new ApplicationException ("wrong id of element on server");
        }

        [WebMethod]
        protected void sandbox_invoke_exception_onclick (pf.Literal literal, EventArgs e) { throw new ApplicationException ("this is an intentional error"); }

        //[WebMethod] intentionally commented out
        protected void sandbox_invoke_no_webmethod_onclick (pf.Literal literal, EventArgs e) { }

        [WebMethod]
        protected void sandbox_invoke_normal_onclick (pf.Literal literal, EventArgs e) { }

        [WebMethod]
        protected void sandbox_invoke_multiple_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerValue += "x";
            Thread.Sleep (100);
        }

        [WebMethod]
        protected void sandbox_invoke_javascript_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerValue = Page.Request.Params ["mumbo"] + " jumbo";
        }

        [WebMethod]
        protected void sandbox_invoke_change_content_onclick (pf.Literal literal, EventArgs e) { literal.innerValue = "new value"; }

        [WebMethod]
        protected void sandbox_invoke_change_two_properties_onclick (pf.Literal literal, EventArgs e)
        {
            literal ["class"] = "new value 1";
            literal.innerValue = "new value 2";
        }

        [WebMethod]
        protected void sandbox_invoke_add_remove_1_onclick (pf.Literal literal, EventArgs e) { literal ["class"] = "new value 1"; }

        [WebMethod]
        protected void sandbox_invoke_add_remove_2_onclick (pf.Literal literal, EventArgs e) { literal.RemoveAttribute ("class"); }

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
        protected void sandbox_invoke_change_markup_attribute_onclick (pf.Literal literal, EventArgs e) { literal ["class"] = "bar"; }

        [WebMethod]
        protected void sandbox_invoke_remove_markup_attribute_onclick (pf.Literal literal, EventArgs e) { literal.RemoveAttribute ("class"); }

        [WebMethod]
        protected void sandbox_invoke_remove_add_markup_attribute_1_onclick (pf.Literal literal, EventArgs e) { literal.RemoveAttribute ("class"); }

        [WebMethod]
        protected void sandbox_invoke_remove_add_markup_attribute_2_onclick (pf.Literal literal, EventArgs e) { literal ["class"] = "bar"; }

        [WebMethod]
        protected void sandbox_invoke_concatenate_long_attribute_onclick (pf.Literal literal, EventArgs e) { literal ["class"] += "qwerty"; }

        [WebMethod]
        protected void sandbox_invoke_create_concatenate_long_attribute_1_onclick (pf.Literal literal, EventArgs e) { literal ["class"] = "x1234567890"; }

        [WebMethod]
        protected void sandbox_invoke_create_concatenate_long_attribute_2_onclick (pf.Literal literal, EventArgs e) { literal ["class"] += "abcdefghijklmnopqrstuvwxyz"; }

        [WebMethod]
        protected void sandbox_invoke_change_container_child_child_onclick (pf.Literal literal, EventArgs e) { literal ["class"] += "bar"; }

        [WebMethod]
        protected void sandbox_invoke_make_container_visible_onclick (pf.Container container, EventArgs e) { container.Visible = true; }

        [WebMethod]
        protected void sandbox_invoke_make_container_visible_invisible_child_onclick (pf.Container container, EventArgs e) { container.Visible = true; }

        [WebMethod]
        protected void sandbox_invoke_make_container_visible_child_invisible_2_onclick (pf.Container container, EventArgs e) { container.Visible = true; }

        [WebMethod]
        protected void sandbox_invoke_make_container_visible_child_visible_2_onclick (pf.Container container, EventArgs e) { container.Visible = true; }

        [WebMethod]
        protected void sandbox_invoke_add_child_onclick (pf.Container container, EventArgs e)
        {
            var existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 1)
                throw new ApplicationException ("widget disappeared somehow");

            if (existing [0].innerValue != "foo")
                throw new ApplicationException ("widget had wrong innerValue");

            var literal = container.CreatePersistentControl<pf.Literal> ();
            literal.ElementType = "strong";
            literal.innerValue = "howdy world";

            existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 2)
                throw new ApplicationException ("widget disappeared somehow after insertion");

            if (existing [1].innerValue != "howdy world")
                throw new ApplicationException ("widget had wrong innerValue");
        }

        [WebMethod]
        protected void sandbox_invoke_insert_child_onclick (pf.Container container, EventArgs e)
        {
            var literal = container.CreatePersistentControl<pf.Literal> (null, 0);
            literal.ElementType = "strong";
            literal.innerValue = "howdy world";

            var existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 2)
                throw new ApplicationException ("widget disappeared somehow after insertion");

            if (existing [0].innerValue != "howdy world")
                throw new ApplicationException ("widget had wrong innerValue");
        }

        [WebMethod]
        protected void sandbox_invoke_add_child_check_exist_1_onclick (pf.Container container, EventArgs e)
        {
            var literal = container.CreatePersistentControl<pf.Literal> ();
            literal.ElementType = "strong";
            literal.innerValue = "howdy world";
        }

        [WebMethod]
        protected void sandbox_invoke_add_child_check_exist_2_onclick (pf.Container container, EventArgs e)
        {
            var existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 2)
                throw new ApplicationException ("widget disappeared somehow");

            if (existing [0].innerValue != "foo")
                throw new ApplicationException ("widget had wrong innerValue");

            if (existing [1].innerValue != "howdy world")
                throw new ApplicationException ("widget had wrong innerValue");

            var literal = container.CreatePersistentControl<pf.Literal> ();
            literal.ElementType = "strong";
            literal.innerValue = "howdy world 2";

            existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 3)
                throw new ApplicationException ("widget disappeared somehow after insertion");

            if (existing [2].innerValue != "howdy world 2")
                throw new ApplicationException ("widget had wrong innerValue");
        }

        [WebMethod]
        protected void sandbox_invoke_insert_child_check_exist_1_onclick (pf.Container container, EventArgs e)
        {
            var literal = container.CreatePersistentControl<pf.Literal> (null, 0);
            literal.ElementType = "strong";
            literal.innerValue = "howdy world";
        }

        [WebMethod]
        protected void sandbox_invoke_insert_child_check_exist_2_onclick (pf.Container container, EventArgs e)
        {
            var existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 2)
                throw new ApplicationException ("widget disappeared somehow");

            if (existing [0].innerValue != "howdy world")
                throw new ApplicationException ("widget had wrong innerValue");

            if (existing [1].innerValue != "foo")
                throw new ApplicationException ("widget had wrong innerValue");

            var literal = container.CreatePersistentControl<pf.Literal> (null, 1);
            literal.ElementType = "strong";
            literal.innerValue = "howdy world 2";

            existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 3)
                throw new ApplicationException ("widget disappeared somehow after insertion");

            if (existing [1].innerValue != "howdy world 2")
                throw new ApplicationException ("widget had wrong innerValue");
        }

        [WebMethod]
        protected void sandbox_invoke_remove_child_onclick (pf.Container container, EventArgs e)
        {
            var literals = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            container.RemoveControlPersistent (literals [0]);
        }

        [WebMethod]
        protected void sandbox_invoke_remove_multiple_onclick (pf.Container container, EventArgs e)
        {
            var literals = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            container.RemoveControlPersistent (literals [0]);
            container.RemoveControlPersistent (literals [1]);
        }

        [WebMethod]
        protected void sandbox_invoke_append_remove_onclick (pf.Container container, EventArgs e)
        {
            var literals = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            container.RemoveControlPersistent (literals [0]);
            var literal = container.CreatePersistentControl<pf.Literal> (null, 0);
            literal.ElementType = "strong";
            literal.innerValue = "howdy world";
        }

        [WebMethod]
        protected void sandbox_invoke_remove_many_onclick (pf.Container container, EventArgs e)
        {
            // removing three controls
            container.RemoveControlPersistentAt (1); // sandbox_invoke_remove_many_2
            ((pf.Container) container.Controls [1]).RemoveControlPersistentAt (2); // sandbox_invoke_remove_many_6
            ((pf.Container) ((pf.Container) container.Controls [1]).Controls [1]).RemoveControlPersistentAt (1); // sandbox_invoke_remove_many_9

            // creating two new controls

            // parent is sandbox_invoke_remove_many
            var lit1 = container.CreatePersistentControl<pf.Literal> (null, 0);
            lit1.ElementType = "strong";
            lit1.innerValue = "howdy";

            // parent is sandbox_invoke_remove_many_5
            var lit2 = ((pf.Container) ((pf.Container) container.Controls [2]).Controls [1]).CreatePersistentControl<pf.Literal> ();
            lit2.ElementType = "em";
            lit2.innerValue = "world";
        }

        [WebMethod]
        protected void sandbox_invoke_remove_many_verify_onclick (pf.Container container, EventArgs e)
        {
            if (container.Controls.Count != 3)
                throw new ApplicationException ("control count not correct on postback");
            if (container.Controls [2].Controls.Count != 2)
                throw new ApplicationException ("control count not correct on postback");
            if (container.Controls [2].Controls [1].Controls.Count != 3)
                throw new ApplicationException ("control count not correct on postback");
            if (((pf.Literal) container.Controls [0]).innerValue != "howdy")
                throw new ApplicationException ("control value not correct on postback");
            if (((pf.Literal) container.Controls [0]).ElementType != "strong")
                throw new ApplicationException ("control element not correct on postback");
            if (((pf.Literal) container.Controls [2].Controls [1].Controls [2]).innerValue != "world")
                throw new ApplicationException ("control value not correct on postback");
            if (((pf.Literal) container.Controls [2].Controls [1].Controls [2]).ElementType != "em")
                throw new ApplicationException ("control element not correct on postback");
        }
    }
}