
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
        // we don't need to register instances of this class, or even create instance of the class since
        // the only Active Event handler in this class is "static", and hence will be invoked every time
        // the "foo" Active Event is raised, as long as the Assembly containing the type "StaticListener"
        // is loaded using the "Loader.Instance.LoadAssembly" method
        public class StaticListener
        {
            [ActiveEvent (Name = "foo")]
            protected static void foo (ApplicationContext sender, ActiveEventArgs e)
            {
                e.Args.Value += " - StaticListener was invoked - ";
            }
        }

        // this class needs to create an instance, and then register that instance as a "listening object"
        // in addition to having its assembly registered as the above static method, since its Active Event
        // handler is an instance method
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

            // raising an active event, and showing the return value as the innerValue of our literal being clicked
            // please notice that BOTH ActiveEvent handlers will at this point be invoked, since static event listeners are
            // automatically mapped and invoked as long as the assembly containing the type where they're defined is loaded
            Node node = new Node ();
            context.Raise ("foo", node);
            literal.innerValue = node.Value.ToString ();
        }
    }
}
