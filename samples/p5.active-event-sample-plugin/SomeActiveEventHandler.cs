/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

 /*
  * This is a "plugin assembly", consumed in our "p5.active-event-sample" console program. Neither contains
  * any references to the other in any ways, still our console program is able to invoke Active Events in this assembly,
  * as if they were methods in a library.
  */

using p5.core;

namespace p5_active_event_sample_plugins
{
    public class SomeActiveEventHandler
    {
        // Here we register a *STATIC* Active Event handler, which will be invoked every time
        // the "foo" Active Event is raised
        [ActiveEvent (Name = "foo")]
        public static void foo (ApplicationContext context, ActiveEventArgs e)
        {
            // Simply outputting some text to the console, proving we were here
            System.Console.WriteLine ("Hello from plugin!");

            // Before showing how we can return values from our Active Event handlers.
            e.Args.Value = "Phosphorus Five";
        }

        // This Active Event will create an instance handler, showing this feature of P5
        // Notice, after this Active Event has been invoked, we will have *TWO* methods handling our
        // "foo" Active Event.
        [ActiveEvent (Name = "create-instance-handler")]
        public static void create_instance_handler (ApplicationContext context, ActiveEventArgs e)
        {
            var fooInstance = new SomeActiveEventHandler ();
            context.RegisterListeningObject (fooInstance);
        }

        // Notice how this event handler, handles the same ("foo") Active Event, but is *NOT*a static method
        // Hence it is an "instance handler", which will only kick in, for each registered instance of this class
        [ActiveEvent (Name = "foo")]
        public void foo_instance (ApplicationContext context, ActiveEventArgs e)
        {
            // Simply outputting some text to the console, proving we were here
            System.Console.WriteLine ("ANOTHER Hello from plugin!");
        }

        // This Active Event will create an instance handler, showing this feature of P5
        [ActiveEvent (Name = "unregister-instance-handler")]
        public void unregister_instance_handler (ApplicationContext context, ActiveEventArgs e)
        {
            context.UnregisterListeningObject (this);
        }
    }
}
