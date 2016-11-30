/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

/*
 * This console program contains a reference to the project called "p5.active-event-sample-plugin", but this
 * is only for our convenience, since we don't want to physically copy and paste our plugin DLL into the bin
 * folder for this console program, every time we compile our project. If we wanted to, we wouldn't need this 
 * reference at all in fact, but could completely rely upon dynamically loading our plugin DLL into our AppDomain.
 * 
 * However, there are no dependencies between our console program (this program) and our plugin in any ways.
 * Still our console program is perfectly able to invoke Active Events (methods) in our plugin assembly.
 */

using p5.core;

namespace p5_active_event_sample
{
    public static class Program
    {
        public static void Main (string[] args)
        {
            // First we must load our plugin assembly. This must be done, even if the assembly is linked in as a reference, to
            /// initialize the assembly's Active Event handlers and such.
            System.Console.WriteLine ("First we load our plugin");
            Loader.Instance.LoadAssembly ("p5.active-event-sample-plugin");

            // Then we must create our "ApplicationContext" object, which among other things, allows us to raise Active Events
            System.Console.WriteLine ("Then we create an ApplicationContext and raise our 'foo' Active Event");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();

            // Then we raise our Active Event, which will invoke our "foo" Active Event in our plugin.
            var retVal = context.Raise ("foo");

            // Then we write the returned value from our plugin Active Event to the console
            System.Console.WriteLine ("Value from plugin was; " + retVal.Value);

            // Then we invoke another Active Event, which is statically handled in our plugin, but which creates another handler
            // for our "foo" Active Event, being an "instance handler".
            System.Console.WriteLine ("Then we register an 'instance handler', and re-raise our 'foo' event, which then should have *TWO* handlers");
            context.Raise ("create-instance-handler");

            // When we now raise our "foo" Active Event, we will have two handlers for it
            context.Raise ("foo");

            // Then we uregister our instance event handler
            System.Console.WriteLine ("Then we unregister our instance handler, before we re-raise our 'foo' event yet again");

            // Then we invoke an Active Event which "unregisters" our instance handler
            context.Raise ("unregister-instance-handler");

            // Before we invoke our foo Active Event for the last time, proving our instance handler is now "out of the picture"
            context.Raise ("foo");

            // Before finally making sure our console window stays open until the user hits carriage return
            System.Console.ReadLine ();
        }
    }
}
