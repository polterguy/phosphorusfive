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
    using phosphorus.core;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class ActiveEvents : AjaxPage
    {
        public class StaticListener
        {
            [ActiveEvent (Name = "foo")]
            protected static void foo (ApplicationContext sender, ActiveEventArgs e)
            {
                e.Args.Value += " - StaticListener was invoked - ";
            }
        }

        public class InstanceListener
        {
            [ActiveEvent (Name = "foo")]
            protected void foo (ApplicationContext sender, ActiveEventArgs e)
            {
                e.Args.Value += " - InstanceListener was invoked - ";
            }
        }

        [WebMethod]
        protected void element_onclick (pf.Literal literal, EventArgs e)
        {
            // creating our application context consisting of the assembly currently being executed
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();

            // registering an instance listener, static listeners will automatically be wired up
            InstanceListener instance = new InstanceListener ();
            context.RegisterListeningObject (instance);

            // raising an active event, and showing the return value as the innerHTML of our literal being clicked
            // please notice that BOTH ActiveEvent handlers will at this point be invoked, since static event listeners are
            // automatically mapped
            Node node = new Node ();
            context.Raise ("foo", node);
            literal.innerHTML = node.Value.ToString ();
        }
    }
}

