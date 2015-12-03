/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

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
        ///     verifies loading an assembly works
        /// </summary>
        [Test]
        public void LoadAssembly1 ()
        {
            Loader.Instance.LoadAssembly ("p5.lambda");
        }

        /// <summary>
        ///     verifies loading an assembly with extension works
        /// </summary>
        [Test]
        public void LoadAssembly2 ()
        {
            Loader.Instance.LoadAssembly ("p5.lambda.dll");
        }

        /// <summary>
        ///     verifying loading the same assembly twice works
        /// </summary>
        [Test]
        public void LoadAssembly3 ()
        {
            Loader.Instance.LoadAssembly ("p5.lambda");
            Loader.Instance.LoadAssembly ("p5.lambda");
        }

        /// <summary>
        ///     verifying loading two asssemblies works
        /// </summary>
        [Test]
        public void LoadAssembly4 ()
        {
            Loader.Instance.LoadAssembly ("p5.lambda");
            Loader.Instance.LoadAssembly ("p5.hyperlisp");
        }

        /// <summary>
        ///     verifying loading the currently executing assembly works
        /// </summary>
        [Test]
        public void LoadAssembly5 ()
        {
            Loader.Instance.LoadAssembly (GetType ());
        }

        /// <summary>
        ///     verifyi loading an assembly by type works
        /// </summary>
        [Test]
        public void LoadAssembly6 ()
        {
            Loader.Instance.LoadAssembly (typeof (Core));
        }

        /// <summary>
        ///     verify creating an application context works
        /// </summary>
        [Test]
        public void ApplicationContext1 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            Assert.IsNotNull (context);
        }

        [ActiveEvent (Name = "foo", Protection = EntranceProtection.Lambda)]
        private static void Foo (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }

        /// <summary>
        ///     verifying raising an Active Event with a single static event handler works
        /// </summary>
        [Test]
        public void ApplicationContext2 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ();
            context.Raise ("foo", tmp);
            Assert.AreEqual ("success", tmp.Value, "Active Event in current assembly did not execute when expected");
        }

        /// <summary>
        ///     used in ApplicationContext3 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo2", Protection = EntranceProtection.LambdaVirtual)]
        private static void foo2_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     used in ApplicationContext3 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo2", Protection = EntranceProtection.LambdaVirtual)]
        private static void foo2_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying raising an Active Event with two static event handlers works
        /// </summary>
        [Test]
        public void ApplicationContext3 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo2", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value, "Active Event in current assembly did not execute when expected");
        }

        /// <summary>
        ///     used in ApplicationContext4 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo3", Protection = EntranceProtection.LambdaVirtual)]
        private void Foo3 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }

        /// <summary>
        ///     verifying raising an Active Event with a single instance event handler works
        /// </summary>
        [Test]
        public void ApplicationContext4 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            var tmp = new Node ();
            context.Raise ("foo3", tmp);
            Assert.AreEqual ("success", tmp.Value, "Active Event in current assembly did not execute when expected");
        }

        /// <summary>
        ///     used in ApplicationContext5 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo4", Protection = EntranceProtection.LambdaVirtual)]
        private void foo4_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     used in ApplicationContext5 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo4", Protection = EntranceProtection.LambdaVirtual)]
        private void foo4_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying raising an Active Event with two instance event handlers works
        /// </summary>
        [Test]
        public void ApplicationContext5 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo4", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value, "Active Event in current assembly did not execute when expected");
        }

        /// <summary>
        ///     used in ApplicationContext6 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo5", Protection = EntranceProtection.LambdaVirtual)]
        private void foo5_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     used in ApplicationContext6 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo5", Protection = EntranceProtection.LambdaVirtual)]
        private static void foo5_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying raising an Active Event with one static event handler, and another instance event handler,
        ///     for the same Active Event works
        /// </summary>
        [Test]
        public void ApplicationContext6 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo5", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value, "Active Event in current assembly did not execute when expected");
        }

        /// <summary>
        ///     used in ApplicationContext7 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo6", Protection = EntranceProtection.Lambda)]
        private static void Foo6 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying that when discarding an Application Context and creating a new, no "garbage" from previous
        ///     context passes into new context
        /// </summary>
        [Test]
        public void ApplicationContext7 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // intentionally throwing away previous context
            context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // raising Active Event on new context, making sure our event handler is only invoked once
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo6", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }

        /// <summary>
        ///     used in ApplicationContext8 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo7", Protection = EntranceProtection.Lambda)]
        private static void Foo7 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying that registering the same object as instance listener twice
        ///     in application context does not have any effect
        /// </summary>
        [Test]
        public void ApplicationContext8 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context.RegisterListeningObject (this);
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo7", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }

        /// <summary>
        ///     used in ApplicationContext9 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo8", Protection = EntranceProtection.LambdaVirtual)]
        private void Foo8 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying that unregistering in instance listener, removes the event handler
        /// </summary>
        [Test]
        public void ApplicationContext9 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // unregistering instance listener before we raise event
            context.UnregisterListeningObject (this);
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo8", tmp);
            Assert.AreEqual (string.Empty, tmp.Value, "context contained previously registered instance listener");
        }

        /// <summary>
        ///     used in ApplicationContext10 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo9", Protection = EntranceProtection.LambdaVirtual)]
        private void foo9_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     used in ApplicationContext10 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo9", Protection = EntranceProtection.LambdaVirtual)]
        private static void foo9_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying that unregistering an instance listener, still kicks in a static listener
        /// </summary>
        [Test]
        public void ApplicationContext10 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context.UnregisterListeningObject (this);
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo9", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }

        /// <summary>
        ///     used in ApplicationContext11 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo10", Protection = EntranceProtection.Lambda)]
        private static void Foo10 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying that unloading a dll, before creating application context, removes the static
        ///     listener from the dll
        /// </summary>
        [Test]
        public void ApplicationContext11 ()
        {
            Loader.Instance.LoadAssembly (GetType ());
            Loader.Instance.UnloadAssembly ("p5.tests");
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo10", tmp);
            Loader.Instance.LoadAssembly (GetType ());
            Assert.AreEqual (string.Empty, tmp.Value, "assembly didn't unload correctly");
        }

        /// <summary>
        ///     used in ApplicationContext12 Unit Test
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "foo11", Protection = EntranceProtection.LambdaVirtual)]
        private void Foo11 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        ///     verifying unloading a dll and then re-create application context, removes instance listener
        /// </summary>
        [Test]
        public void ApplicationContext12 ()
        {
            Loader.Instance.LoadAssembly (this.GetType ());
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // intentionally unloading assembly, before re-creating context
            Loader.Instance.UnloadAssembly ("p5.tests");
            context = Loader.Instance.CreateApplicationContext ();
            Loader.Instance.LoadAssembly (this.GetType ());
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo11", tmp);
            Assert.AreEqual (string.Empty, tmp.Value, "assembly didn't unload correctly");
        }

        /// <summary>
        ///     verifying that invoking a non-existing Active Event does not trigger any errors
        /// </summary>
        [Test]
        public void InvokeNullEventHandler ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node (string.Empty, string.Empty);
            context.Raise ("non-existing", tmp);
        }

        [ActiveEvent (Name = "foo24", Protection = EntranceProtection.LambdaVirtual)]
        private void Foo24 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo54", Protection = EntranceProtection.LambdaVirtual)]
        [ActiveEvent (Name = "foo54", Protection = EntranceProtection.LambdaVirtual)]
        private static void Foo54 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext32 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ();
            context.Raise ("foo54", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value, "Active Event in current assembly did not execute as expected");
        }

        private class Foo55
        {
            [ActiveEvent (Name = "foo55", Protection = EntranceProtection.Lambda)]
            private void Foo54 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }
        }

        [Test]
        [ExpectedException]
        public void ApplicationContext33 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (new Foo55 ());
            context.RegisterListeningObject (new Foo55 ());
        }

        private class Foo56
        {
            [ActiveEvent (Name = "foo56", Protection = EntranceProtection.Lambda)]
            private static void Foo54 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }
        }

        [Test]
        public void ApplicationContext34 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            var tmp = new Node ();
            context.Raise ("foo56", tmp);
            Assert.AreEqual ("success", tmp.Value, "Active Event in current assembly did not execute as expected");
        }

        private class Foo57
        {
            [ActiveEvent (Name = "foo57", Protection = EntranceProtection.Lambda)]
            private static void Foo54 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }

            [ActiveEvent (Name = "foo57", Protection = EntranceProtection.Lambda)]
            private void Foo54_2 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }
        }

        [Test]
        [ExpectedException]
        public void ApplicationContext35 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (new Foo57 ());
        }

        private class Foo58
        {
            [ActiveEvent (Name = "foo58", Protection = EntranceProtection.Lambda)]
            private static void Foo54 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }

            [ActiveEvent (Name = "foo58", Protection = EntranceProtection.Lambda)]
            private void Foo54_2 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success";
            }
        }

        [Test]
        [ExpectedException]
        public void ApplicationContext36 ()
        {
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (new Foo58 ());
        }
    }
}