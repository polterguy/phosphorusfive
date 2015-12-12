/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Security;
using NUnit.Framework;
using p5.core;

namespace p5.unittests
{
    /// <summary>
    ///     unit tests for phosphorus.core project
    /// </summary>
    [TestFixture]
    public class Core : TestBase
    {
        /// <summary>
        ///     Verifies loading an assembly works
        /// </summary>
        [Test]
        public void LoadAssembly ()
        {
            Loader.Instance.LoadAssembly ("p5.lambda");
        }

        /// <summary>
        ///     Verifies loading an assembly with extension works
        /// </summary>
        [Test]
        public void LoadAssemblyWithExtension ()
        {
            Loader.Instance.LoadAssembly ("p5.lambda.dll");
        }

        /// <summary>
        ///     Verifies loading the same assembly twice works
        /// </summary>
        [Test]
        public void LoadAssemblyTwice ()
        {
            Loader.Instance.LoadAssembly ("p5.lambda");
            Loader.Instance.LoadAssembly ("p5.lambda");
        }

        /// <summary>
        ///     Verifies loading two asssemblies works
        /// </summary>
        [Test]
        public void LoadTwoAssemblies ()
        {
            Loader.Instance.LoadAssembly ("p5.lambda");
            Loader.Instance.LoadAssembly ("p5.hyperlisp");
        }

        /// <summary>
        ///     Verifies loading the currently executing assembly by type works
        /// </summary>
        [Test]
        public void LoadExecutingAssemblyByType ()
        {
            Loader.Instance.LoadAssembly (GetType ());
        }

        /// <summary>
        ///     Verifies loading an assembly by type works
        /// </summary>
        [Test]
        public void LoadAssemblyByType ()
        {
            Loader.Instance.LoadAssembly (typeof (Core));
        }

        /// <summary>
        ///     Verifies creating an application context works
        /// </summary>
        [Test]
        public void CreateApplicationContext ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            Assert.IsNotNull (context);
        }

        /*
         * Now some more "advanced" unit tests, 
         * that are actually testing it is possible to raise Active Events
         */

        [ActiveEvent (Name = "core.foo", Protection = EventProtection.NativeClosed)]
        private static void core_foo (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }

        /// <summary>
        ///     Verifies raising an Active Event with a single static event handler works
        /// </summary>
        [Test]
        public void RaiseStatic ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ();
            context.RaiseNative ("core.foo", tmp);
            Assert.AreEqual ("success", tmp.Value);
        }

        /// <summary>
        ///     Verifies raising an Active Event with a single static event handler does not work, if
        ///     Active Event is raised from lambda, and event is protected, such that only native code can
        ///     raise it
        /// </summary>
        [Test]
        public void RaiseStaticNativeClosedFromLambda ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ();

            // Throws
            Assert.Throws<SecurityException> (delegate {
                context.RaiseLambda ("core.foo", tmp);
            });
        }

        private class FooClass
        {
            [ActiveEvent (Name = "core.foo", Protection = EventProtection.NativeOpen)]
            private void core_foo (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value = "success";
            }
        }

        /// <summary>
        ///     Verifies instantiating and registering an event listener does not work, if existing
        ///     event is registered as NativeClosed
        /// </summary>
        [Test]
        public void RegisterListenerNativeClosed ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var x = new FooClass ();

            // Should throw!
            Assert.Throws<SecurityException> (delegate {
                context.RegisterListeningObject (x);
            });
        }

        [ActiveEvent (Name = "core.foo2", Protection = EventProtection.NativeOpen)]
        private static void core_foo2_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "core.foo2", Protection = EventProtection.NativeOpen)]
        private static void core_foo2_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifies raising an Active Event with two static event handlers works
        /// </summary>
        [Test]
        public void RaiseTwoStaticHandlers ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ("", "");
            context.RaiseNative ("core.foo2", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value);
        }

        [ActiveEvent (Name = "core.foo3", Protection = EventProtection.NativeOpen)]
        private void core_foo3 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }

        /// <summary>
        ///     Verifies raising an Active Event with a single instance event handler works
        /// </summary>
        [Test]
        public void RaiseSingleInstanceHandler ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            var tmp = new Node ();
            context.RaiseNative ("core.foo3", tmp);
            Assert.AreEqual ("success", tmp.Value);
        }

        [ActiveEvent (Name = "core.foo4", Protection = EventProtection.NativeOpen)]
        private void core_foo4_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     used in ApplicationContext5 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "core.foo4", Protection = EventProtection.NativeOpen)]
        private void core_foo4_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifying raising an Active Event with two instance event handlers works
        /// </summary>
        [Test]
        public void RaiseTwoInstanceHandlers ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            var tmp = new Node ("", "");
            context.RaiseNative ("core.foo4", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value);
        }

        /// <summary>
        ///     Verifying raising an Active Event with two instance event handlers works, 
        ///     when instance is registered twice
        /// </summary>
        [Test]
        public void RaiseTwoInstanceHandlersInstanceRegisteredTwice ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // Adding same object twice, which SHOULDN'T make a difference ...
            context.RegisterListeningObject (this);

            var tmp = new Node ("", "");
            context.RaiseNative ("core.foo4", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value);
        }

        [ActiveEvent (Name = "core.foo5", Protection = EventProtection.NativeOpen)]
        private void core_foo5_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "core.foo5", Protection = EventProtection.NativeOpen)]
        private static void core_foo5_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifies raising an Active Event with one static event handler, and another instance event handler,
        ///     for the same Active Event works
        /// </summary>
        [Test]
        public void RaiseOneInstanceOneStaticHandler ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            var tmp = new Node ("", "");
            context.RaiseNative ("core.foo5", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value);
        }

        [ActiveEvent (Name = "core.foo6", Protection = EventProtection.NativeOpen)]
        private void core_foo6 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifies that when discarding an Application Context, and creating a new, no "garbage" from previous
        ///     context passes into new context
        /// </summary>
        [Test]
        public void DiscardAppContextCreateNew ()
        {
            // Creating App Context that we will immediately throw away, to verify no garbage is passed from
            // one context to another
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // Intentionally throwing away previous context
            context = Loader.Instance.CreateApplicationContext ();

            // Registering this once more
            context.RegisterListeningObject (this);

            // Raising Active Event on new context, making sure our event handler is only invoked once
            var tmp = new Node ("", "");
            context.RaiseNative ("core.foo6", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }

        [ActiveEvent (Name = "core.foo7", Protection = EventProtection.LambdaClosed)]
        private static void core_foo7 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifies that registering the same object as instance listener twice in 
        ///     application context does not have any effect when having static listener
        /// </summary>
        [Test]
        public void RegisterSameListenerTwiceStaticHandler ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // SHOULDN'T make a difference
            context.RegisterListeningObject (this);
            var tmp = new Node ("", "");
            context.RaiseNative ("core.foo7", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }

        [ActiveEvent (Name = "core.foo8", Protection = EventProtection.NativeOpen)]
        private void core_foo8 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifies that unregistering in instance listener, removes the event handler
        /// </summary>
        [Test]
        public void UnregisterListener ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // Unregistering instance listener before we raise event, which SHOULD remove handler
            context.UnregisterListeningObject (this);

            var tmp = new Node ("", "");
            context.RaiseNative ("core.foo8", tmp);
            Assert.AreEqual ("", tmp.Value, "context contained previously registered instance listener");
        }

        [ActiveEvent (Name = "core.foo9", Protection = EventProtection.NativeOpen)]
        private void core_foo9_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "foo9", Protection = EventProtection.NativeOpen)]
        private static void foo9_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifies that unregistering an instance listener, still kicks in a static listener
        /// </summary>
        [Test]
        public void UnregisterListenerWithStaticHandler ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context.UnregisterListeningObject (this);
            var tmp = new Node ("", "");
            context.RaiseNative ("foo9", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }

        [ActiveEvent (Name = "core.foo10", Protection = EventProtection.LambdaClosed)]
        private static void core_foo10 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifies that unloading a dll, before creating application context, removes the static
        ///     listener from the dll
        /// </summary>
        [Test]
        public void UnloadAssembly ()
        {
            Loader.Instance.LoadAssembly (GetType ());

            // SHOULD remove static listener above
            Loader.Instance.UnloadAssembly ("p5.tests");

            // Creating context after unloading assembly
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ("", "");
            context.RaiseNative ("core.foo10", tmp);

            // Making sure we reload assembly, for next test
            Loader.Instance.LoadAssembly (GetType ());
            Assert.AreEqual ("", tmp.Value);
        }

        [ActiveEvent (Name = "core.foo11", Protection = EventProtection.NativeOpen)]
        private void core_foo11 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     Verifies that unloading a dll, and then re-create application context, removes instance listener
        /// </summary>
        [Test]
        public void RegisterAssemblyAfterContextCreation ()
        {
            Loader.Instance.LoadAssembly (this.GetType ());
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // Intentionally unloading assembly, before re-creating context
            Loader.Instance.UnloadAssembly ("p5.tests");

            // Creating new context
            context = Loader.Instance.CreateApplicationContext ();

            // Registering type of this as event handler, which is TOO LATE to change above created context
            Loader.Instance.LoadAssembly (this.GetType ());
            var tmp = new Node ("", "");

            // Even though we reloaded this assembly, we did so after creating context, 
            // which means context should NOT have [core.foo11] as handler
            context.RaiseNative ("core.foo11", tmp);
            Assert.AreEqual ("", tmp.Value);
        }

        /// <summary>
        ///     Verifies that invoking a non-existing Active Event does not do anything
        /// </summary>
        [Test]
        public void InvokeNullEventHandler ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ("", "");
            context.RaiseNative ("core.non-existing", tmp);
            Assert.AreEqual ("", tmp.Name);
            Assert.AreEqual ("", tmp.Value);
        }

        private class Foo12
        {
            [ActiveEvent (Name = "core.foo12", Protection = EventProtection.NativeClosed)]
            private void core_foo12 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }
        }

        /// <summary>
        ///     Verifies that registering same listener twice creates a security exception when event is closed
        /// </summary>
        [Test]
        public void ExtendingClosedEventThrows ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (new Foo12 ());

            Assert.Throws<SecurityException> (delegate {
                context.RegisterListeningObject (new Foo12 ());
            });
        }

        private class Foo13
        {
            [ActiveEvent (Name = "core.foo13", Protection = EventProtection.LambdaClosed)]
            private static void core_foo13 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }
        }

        /// <summary>
        ///     Verifies that a static event handler in an inner class is working correctly
        /// </summary>
        [Test]
        public void StaticHandlerInInnerClass ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ();
            context.RaiseNative ("core.foo13", tmp);
            Assert.AreEqual ("success", tmp.Value);
        }

        private class Foo14
        {
            [ActiveEvent (Name = "core.foo14", Protection = EventProtection.NativeClosed)]
            private static void core_foo14_1 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }

            [ActiveEvent (Name = "core.foo14", Protection = EventProtection.NativeOpen)]
            private void core_foo14_2 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }
        }

        /// <summary>
        ///     Verifies that a registering an instance handler where a closed static handler exist from before throws
        /// </summary>
        [Test]
        public void ExtendingClosedStaticWithInstanceThrows ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            Assert.AreEqual ("success", context.RaiseLambda ("core.foo14").Value);

            Assert.Throws<SecurityException> (delegate {
                context.RegisterListeningObject (new Foo14 ());
            });
        }

        private class Foo15
        {
            [ActiveEvent (Name = "core.foo15", Protection = EventProtection.NativeOpen)]
            private static void core_foo15_1 (ApplicationContext context, ActiveEventArgs e)
            { }

            [ActiveEvent (Name = "core.foo15", Protection = EventProtection.NativeClosed)]
            private void core_foo15_2 (ApplicationContext context, ActiveEventArgs e)
            { }
        }

        /// <summary>
        ///     Verifies that a registering a closed instance handler where an open static handler exist from before throws
        /// </summary>
        [Test]
        public void ExtendingOpenStaticWithClosedInstanceThrows ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            Assert.AreEqual ("success", context.RaiseLambda ("core.foo15").Value);

            Assert.Throws<SecurityException> (delegate {
                context.RegisterListeningObject (new Foo15 ());
            });
        }
    }
}