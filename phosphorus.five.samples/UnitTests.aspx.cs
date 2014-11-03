/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.samples
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Reflection;
    using System.Collections.Generic;
    using phosphorus.core;
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
        protected void sandbox_invoke_multiple_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML += "x";
            System.Threading.Thread.Sleep (100);
        }
        
        [WebMethod]
        protected void sandbox_invoke_javascript_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = Page.Request.Params ["mumbo"] + " jumbo";
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
        protected void sandbox_invoke_make_container_visible_invisible_child_onclick (pf.Container container, EventArgs e)
        {
            container.Visible = true;
        }

        [WebMethod]
        protected void sandbox_invoke_make_container_visible_child_invisible_1_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> literals = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            literals [0].Visible = false;
        }
        
        [WebMethod]
        protected void sandbox_invoke_make_container_visible_child_invisible_2_onclick (pf.Container container, EventArgs e)
        {
            container.Visible = true;
        }
        
        [WebMethod]
        protected void sandbox_invoke_make_container_visible_child_visible_1_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> literals = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            literals [0].Visible = true;
        }

        [WebMethod]
        protected void sandbox_invoke_make_container_visible_child_visible_2_onclick (pf.Container container, EventArgs e)
        {
            container.Visible = true;
        }
        
        [WebMethod]
        protected void sandbox_invoke_add_child_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 1)
                throw new ApplicationException ("widget disappeared somehow");

            if (existing[0].innerHTML != "foo")
                throw new ApplicationException ("widget had wrong innerHTML");

            pf.Literal literal = container.CreatePersistentControl<pf.Literal> ();
            literal.ElementType = "strong";
            literal.innerHTML = "howdy world";

            existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 2)
                throw new ApplicationException ("widget disappeared somehow after insertion");
            
            if (existing[1].innerHTML != "howdy world")
                throw new ApplicationException ("widget had wrong innerHTML");
        }
        
        [WebMethod]
        protected void sandbox_invoke_insert_child_onclick (pf.Container container, EventArgs e)
        {
            pf.Literal literal = container.CreatePersistentControl<pf.Literal> (null, 0);
            literal.ElementType = "strong";
            literal.innerHTML = "howdy world";

            var existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 2)
                throw new ApplicationException ("widget disappeared somehow after insertion");

            if (existing[0].innerHTML != "howdy world")
                throw new ApplicationException ("widget had wrong innerHTML");
        }

        [WebMethod]
        protected void sandbox_invoke_add_child_check_exist_1_onclick (pf.Container container, EventArgs e)
        {
            pf.Literal literal = container.CreatePersistentControl<pf.Literal> ();
            literal.ElementType = "strong";
            literal.innerHTML = "howdy world";
        }
        
        [WebMethod]
        protected void sandbox_invoke_add_child_check_exist_2_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 2)
                throw new ApplicationException ("widget disappeared somehow");
            
            if (existing[0].innerHTML != "foo")
                throw new ApplicationException ("widget had wrong innerHTML");
            
            if (existing[1].innerHTML != "howdy world")
                throw new ApplicationException ("widget had wrong innerHTML");

            pf.Literal literal = container.CreatePersistentControl<pf.Literal> ();
            literal.ElementType = "strong";
            literal.innerHTML = "howdy world 2";

            existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 3)
                throw new ApplicationException ("widget disappeared somehow after insertion");
            
            if (existing[2].innerHTML != "howdy world 2")
                throw new ApplicationException ("widget had wrong innerHTML");
        }
        
        [WebMethod]
        protected void sandbox_invoke_insert_child_check_exist_1_onclick (pf.Container container, EventArgs e)
        {
            pf.Literal literal = container.CreatePersistentControl<pf.Literal> (null, 0);
            literal.ElementType = "strong";
            literal.innerHTML = "howdy world";
        }

        [WebMethod]
        protected void sandbox_invoke_insert_child_check_exist_2_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 2)
                throw new ApplicationException ("widget disappeared somehow");

            if (existing[0].innerHTML != "howdy world")
                throw new ApplicationException ("widget had wrong innerHTML");

            if (existing[1].innerHTML != "foo")
                throw new ApplicationException ("widget had wrong innerHTML");

            pf.Literal literal = container.CreatePersistentControl<pf.Literal> (null, 1);
            literal.ElementType = "strong";
            literal.innerHTML = "howdy world 2";

            existing = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            if (existing.Count != 3)
                throw new ApplicationException ("widget disappeared somehow after insertion");

            if (existing[1].innerHTML != "howdy world 2")
                throw new ApplicationException ("widget had wrong innerHTML");
        }
        
        [WebMethod]
        protected void sandbox_invoke_remove_child_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> literals = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            container.RemoveControlPersistent (literals [0]);
        }
        
        [WebMethod]
        protected void sandbox_invoke_remove_multiple_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> literals = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            container.RemoveControlPersistent (literals [0]);
            container.RemoveControlPersistent (literals [1]);
        }

        [WebMethod]
        protected void sandbox_invoke_append_remove_onclick (pf.Container container, EventArgs e)
        {
            List<pf.Literal> literals = new List<pf.Literal> (container.GetChildControls<pf.Literal> ());
            container.RemoveControlPersistent (literals [0]);
            pf.Literal literal = container.CreatePersistentControl<pf.Literal> (null, 0);
            literal.ElementType = "strong";
            literal.innerHTML = "howdy world";
        }
        
        [WebMethod]
        protected void sandbox_invoke_remove_many_onclick (pf.Container container, EventArgs e)
        {
            // removing three controls
            container.RemoveControlPersistentAt (1); // sandbox_invoke_remove_many_2
            ((pf.Container)container.Controls [1]).RemoveControlPersistentAt (2); // sandbox_invoke_remove_many_6
            ((pf.Container)((pf.Container)container.Controls [1]).Controls [1]).RemoveControlPersistentAt (1); // sandbox_invoke_remove_many_9

            // creating two new controls

            // parent is sandbox_invoke_remove_many
            pf.Literal lit1 = container.CreatePersistentControl<pf.Literal> (null, 0);
            lit1.ElementType = "strong";
            lit1.innerHTML = "howdy";

            // parent is sandbox_invoke_remove_many_5
            pf.Literal lit2 = ((pf.Container)((pf.Container)container.Controls [2]).Controls [1]).CreatePersistentControl<pf.Literal> ();
            lit2.ElementType = "em";
            lit2.innerHTML = "world";
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
            if (((pf.Literal)container.Controls [0]).innerHTML != "howdy")
                throw new ApplicationException ("control value not correct on postback");
            if (((pf.Literal)container.Controls [0]).ElementType != "strong")
                throw new ApplicationException ("control element not correct on postback");
            if (((pf.Literal)container.Controls [2].Controls [1].Controls [2]).innerHTML != "world")
                throw new ApplicationException ("control value not correct on postback");
            if (((pf.Literal)container.Controls [2].Controls [1].Controls [2]).ElementType != "em")
                throw new ApplicationException ("control element not correct on postback");
        }

        [ActiveEvent(Name="foo")]
        public void sandbox_invoke_raise_page_onclick_event (ApplicationContext sender, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }
        
        [WebMethod]
        protected void sandbox_invoke_raise_page_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Node node = new Node ();
            context.Raise ("foo", node);
            if (!node.Value.Equals ("success"))
                throw new ApplicationException ("active event wasn't handled");
        }
        
        [ActiveEvent(Name="foo2")]
        public void sandbox_invoke_register_twice_onclick_event (ApplicationContext sender, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [WebMethod]
        protected void sandbox_invoke_register_twice_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context.RegisterListeningObject (this);
            Node node = new Node ();
            context.Raise ("foo2", node);
            if (!node.Value.Equals ("successsuccess"))
                throw new ApplicationException ("active event was handled twice");
        }
        
        [ActiveEvent(Name="foo3")]
        public void sandbox_invoke_unregister_onclick_onclick_event (ApplicationContext sender, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [WebMethod]
        protected void sandbox_invoke_unregister_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context.UnregisterListeningObject (this);
            Node node = new Node (null, "success");
            context.Raise ("foo3", node);
            if (!node.Value.Equals ("success"))
                throw new ApplicationException ("active event listener wasn't unregistered");
        }
        
        [ActiveEvent(Name="foo4")]
        public void sandbox_invoke_handle_twice_onclick_1_event (ApplicationContext sender, ActiveEventArgs e)
        {
            e.Args.Value += "x";
        }

        [ActiveEvent(Name="foo4")]
        public void sandbox_invoke_handle_twice_onclick_2_event (ApplicationContext sender, ActiveEventArgs e)
        {
            e.Args.Value += "y";
        }

        [WebMethod]
        protected void sandbox_invoke_handle_twice_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Node node = new Node (null, "");
            context.Raise ("foo4", node);
            if (!node.Value.Equals ("xy") && !node.Value.Equals ("yx"))
                throw new ApplicationException ("active event was not handled twice");
        }

        private class Tmp
        {
            [ActiveEvent(Name="foo5")]
            public void foo (ApplicationContext sender, ActiveEventArgs e)
            {
                e.Args.Value += "tjobing";
            }

            [ActiveEvent(Name="foo6")]
            public static void foo2 (ApplicationContext sender, ActiveEventArgs e)
            {
                e.Args.Value += "tjobing2";
            }
            
            [ActiveEvent(Name="foo7")]
            public static void foo3 (ApplicationContext sender, ActiveEventArgs e)
            {
                e.Args.Value += "qwerty";
            }
            
            [ActiveEvent(Name="foo7")]
            public void foo4 (ApplicationContext sender, ActiveEventArgs e)
            {
                e.Args.Value += "qwerty";
            }
        }
        
        [WebMethod]
        protected void sandbox_invoke_handle_domain_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Tmp tmp = new Tmp ();
            context.RegisterListeningObject (tmp);
            Node node = new Node (null, "");
            context.Raise ("foo5", node);
            if (!node.Value.Equals ("tjobing"))
                throw new ApplicationException ("active event was not handled twice");
        }
        
        [WebMethod]
        protected void sandbox_invoke_handle_static_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node node = new Node (null, "");
            context.Raise ("foo6", node);
            if (!node.Value.Equals ("tjobing2"))
                throw new ApplicationException ("active event was not handled twice");
        }

        [WebMethod]
        protected void sandbox_invoke_handle_twice_domain_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Tmp tmp = new Tmp ();
            context.RegisterListeningObject (tmp);
            Node node = new Node (null, "");
            context.Raise ("foo7", node);
            if (!node.Value.Equals ("qwertyqwerty"))
                throw new ApplicationException ("active event was not handled twice");
        }
        
        [WebMethod]
        protected void sandbox_invoke_handle_trice_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Tmp tmp = new Tmp ();
            context.RegisterListeningObject (tmp);
            Node node = new Node (null, "");
            context.Raise ("foo7", node);
            if (!node.Value.Equals ("qwertyqwerty"))
                throw new ApplicationException ("active event was not handled twice");
            context.UnregisterListeningObject (tmp);
            node = new Node (null, "");
            context.Raise ("foo7", node);
            if (!node.Value.Equals ("qwerty"))
                throw new ApplicationException ("active event was not handled twice");
        }

        class Tmp2
        { }
        
        [WebMethod]
        protected void sandbox_invoke_handle_null_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Tmp2 tmp = new Tmp2 ();
            context.RegisterListeningObject (tmp);
        }
        
        [WebMethod]
        protected void sandbox_invoke_raise_null_onclick (pf.Literal container, EventArgs e)
        {
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node node = new Node (null, "");
            context.Raise ("foo8", node);
            if (!node.Value.Equals (""))
                throw new ApplicationException ("active event was handled");
        }
    }
}

