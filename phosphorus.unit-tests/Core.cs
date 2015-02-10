
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class Core
    {
        /// <summary>
        /// verifies loading an assembly works
        /// </summary>
        [Test]
        public void LoadAssembly1 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
        }

        /// <summary>
        /// verifies loading an assembly with extension works
        /// </summary>
        [Test]
        public void LoadAssembly2 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.lambda.dll");
        }

        /// <summary>
        /// verifying loading the same assembly twice works
        /// </summary>
        [Test]
        public void LoadAssembly3 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");

        }

        /// <summary>
        /// verifying loading two asssemblies works
        /// </summary>
        [Test]
        public void LoadAssembly4 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
        }

        /// <summary>
        /// verifying loading the currently executing assembly works
        /// </summary>
        [Test]
        public void LoadAssembly5 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
        }

        /// <summary>
        /// verifyi loading an assembly by type works
        /// </summary>
        [Test]
        public void LoadAssembly6 ()
        {
            Loader.Instance.LoadAssembly (typeof (Core));
        }

        /// <summary>
        /// verify creating an application context works
        /// </summary>
        [Test]
        public void ApplicationContext1 ()
        {
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Assert.IsNotNull (context);
        }

        [ActiveEvent (Name = "foo")]
        private static void foo (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }

        /// <summary>
        /// verifying raising an Active Event with a single static event handler works
        /// </summary>
        [Test]
        public void ApplicationContext2 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            context.Raise ("foo", tmp);
            Assert.AreEqual ("success", tmp.Value, "Active Event in current assembly did not execute when expected");
        }
        
        [ActiveEvent (Name = "foo2")]
        private static void foo2_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "foo2")]
        private static void foo2_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        /// verifying raising an Active Event with two static event handlers works
        /// </summary>
        [Test]
        public void ApplicationContext3 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo2", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value, "Active Event in current assembly did not execute when expected");
        }
        
        [ActiveEvent (Name = "foo3")]
        private void foo3 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }

        /// <summary>
        /// verifying raising an Active Event with a single instance event handler works
        /// </summary>
        [Test]
        public void ApplicationContext4 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Node tmp = new Node ();
            context.Raise ("foo3", tmp);
            Assert.AreEqual ("success", tmp.Value, "Active Event in current assembly did not execute when expected");
        }
        
        [ActiveEvent (Name = "foo4")]
        private void foo4_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "foo4")]
        private void foo4_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        /// verifying raising an Active Event with two instance event handlers works
        /// </summary>
        [Test]
        public void ApplicationContext5 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo4", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value, "Active Event in current assembly did not execute when expected");
        }
        
        [ActiveEvent (Name = "foo5")]
        private void foo5_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "foo5")]
        private static void foo5_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        /// verifying raising an Active Event with one static event handler, and another instance event handler,
        /// for the same Active Event works
        /// </summary>
        [Test]
        public void ApplicationContext6 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo5", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value, "Active Event in current assembly did not execute when expected");
        }
        
        [ActiveEvent (Name = "foo6")]
        private static void foo6 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        /// verifying that when discarding an Application Context and creating a new, no "garbage" from previous
        /// context passes into new context
        /// </summary>
        [Test]
        public void ApplicationContext7 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // intentionally throwing away previous context
            context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // raising Active Event on new context, making sure our event handler is only invoked once
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo6", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }
        
        [ActiveEvent (Name = "foo7")]
        private static void foo7 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        /// <summary>
        /// verifying that registering the same object as listener twice in application context
        /// does not have effect
        /// </summary>
        [Test]
        public void ApplicationContext8 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context.RegisterListeningObject (this);
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo7", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }
        
        [ActiveEvent (Name = "foo8")]
        private void foo8 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext9 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context.UnregisterListeningObject (this);
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo8", tmp);
            Assert.AreEqual (string.Empty, tmp.Value, "context contained previously registered instance listener");
        }
        
        [ActiveEvent (Name = "foo9")]
        private void foo9_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "foo9")]
        private static void foo9_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext10 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context.UnregisterListeningObject (this);
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo9", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }
        
        [ActiveEvent (Name = "foo10")]
        private static void foo10 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext11 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.UnloadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo10", tmp);
            Assert.AreEqual (string.Empty, tmp.Value, "assembly didn't unload correctly");
        }
        
        [ActiveEvent (Name = "foo11")]
        private void foo11 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext12 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Loader.Instance.UnloadAssembly ("phosphorus.unit-tests");
            context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo11", tmp);
            Assert.AreEqual (string.Empty, tmp.Value, "assembly didn't unload correctly");
        }
        
        [Test]
        public void InvokeNullEventHandler ()
        {
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("non-existing", tmp);
        }

        [ActiveEvent (Name = "foo12")]
        private static void foo12 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }
        
        [ActiveEvent (Name = "foo13", Overrides = "foo12")]
        private static void foo13 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext13 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();
            Node node = new Node ();
            context.Raise ("foo12", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo14")]
        private void foo14 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo15", Overrides = "foo14")]
        private void foo15 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext14 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Node node = new Node ();
            context.Raise ("foo14", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo16")]
        private static void foo16 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo17", Overrides = "foo16")]
        private void foo17 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext15 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();
            Node node = new Node ();

            // raising before registering override, to ensure override is not registered before
            // instance listener is registered
            context.Raise ("foo16", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");

            // then registering instance listened to make sure our override then is invoked,
            // and not our base Active Event
            context.RegisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo16", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");

            // then unregistering instance listener, making sure base Active Event is now invoked
            context.UnregisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo16", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo19", Overrides = "foo18")]
        private static void foo19 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext16 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();
            Node node = new Node ();
            context.Raise ("foo18", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");
        }

        [ActiveEvent (Name = "foo21", Overrides = "foo20")]
        private void foo21 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext17 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Node node = new Node ();
            context.Raise ("foo20", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo22")]
        private static void foo22 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo23_1", Overrides = "foo22")]
        private static void foo23_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success1";
        }

        [ActiveEvent (Name = "foo23_2", Overrides = "foo22")]
        private static void foo23_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success2";
        }

        [Test]
        public void ApplicationContext18 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();
            Node node = new Node ();
            context.Raise ("foo22", node);
            Assert.IsTrue (node.Get<string> (context) == "success1success2" || node.Get<string> (context) == "success2success1", "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo24")]
        private void foo24 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        private class foo25
        {
            [ActiveEvent (Name = "foo25_1", Overrides = "foo24")]
            private void foo25_1 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success1";
            }

            [ActiveEvent (Name = "foo25_2", Overrides = "foo24")]
            private void foo25_2 (ApplicationContext context, ActiveEventArgs e)
            {
                e.Args.Value += "success2";
            }
        }

        [Test]
        public void ApplicationContext19 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising without this registered as listener first
            Node node = new Node ();
            context.Raise ("foo24", node);
            Assert.AreEqual (null, node.Value, "active event wasn't correctly overridden");

            // then re-raising with this as listener
            context.RegisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo24", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");

            // then re-raising with foo25 instance registeredd ass listener
            foo25 tmp = new foo25 ();
            context.RegisterListeningObject (tmp);
            node = new Node ();
            context.Raise ("foo24", node);
            Assert.IsTrue (node.Get<string> (context) == "success1success2" || node.Get<string> (context) == "success2success1", "active event wasn't correctly overridden");

            // then re-raising with foo25 instance removed as listener
            context.UnregisterListeningObject (tmp);
            node = new Node ();
            context.Raise ("foo24", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");
            
            // then re-raising with this removed as listener
            context.UnregisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo24", node);
            Assert.AreEqual (null, node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo26")]
        private void foo26 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo27_1", Overrides = "foo26")]
        private void foo27_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "foo27_2", Overrides = "foo26")]
        private static void foo27_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext20 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising without registering this as listener first
            Node node = new Node ();
            context.Raise ("foo26", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");

            // then raising when this is registered as listener
            context.RegisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo26", node);
            Assert.AreEqual ("successsuccess", node.Value, "active event wasn't correctly overridden");

            // then unregistering this, and raising again
            context.UnregisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo26", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo28")]
        private static void foo28 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "failure";
        }

        [ActiveEvent (Name = "foo29")]
        private static void foo29 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext21 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising without overriding Active Event
            Node node = new Node ();
            context.Raise ("foo28", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");

            // raising when Active event is overridden
            context.Override ("foo28", "foo29");
            node = new Node ();
            context.Raise ("foo28", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");

            // then removing override and re-raising
            context.RemoveOverride ("foo28", "foo29");
            node = new Node ();
            context.Raise ("foo28", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");
        }
        
        [Test]
        public void ApplicationContext22 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising without overriding Active Event
            Node node = new Node ();
            context.Raise ("foo28", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");

            // raising when Active event is overridden
            context.Override ("foo28", "foo29");
            node = new Node ();
            context.Raise ("foo28", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");

            // discarding previous application context to make sure new context does NOT have override from previous context
            context = Loader.Instance.CreateApplicationContext ();

            // then re-raising Active Event
            node = new Node ();
            context.Raise ("foo28", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");
        }

        [ActiveEvent (Name = "foo30")]
        private void foo30 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "failure";
        }

        [ActiveEvent (Name = "foo31")]
        private void foo31 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext23 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);

            // raising without overriding Active Event
            Node node = new Node ();
            context.Raise ("foo30", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");

            // raising when Active event is overridden
            context.Override ("foo30", "foo31");
            node = new Node ();
            context.Raise ("foo30", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");

            // then removing override and re-raising
            context.RemoveOverride ("foo30", "foo31");
            node = new Node ();
            context.Raise ("foo30", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo32")]
        private static void foo32 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "failure";
        }

        [ActiveEvent (Name = "foo33")]
        private void foo33 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext24 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising without overriding Active Event
            Node node = new Node ();
            context.Raise ("foo32", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");

            // raising when Active event is overridden, but instance is not registered
            context.Override ("foo32", "foo33");
            node = new Node ();
            context.Raise ("foo32", node);
            Assert.AreEqual (null, node.Value, "active event wasn't correctly overridden");

            // registering instance, and raising Active Event
            context.RegisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo32", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");

            // then unregistering listener, and re-raising
            context.UnregisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo32", node);
            Assert.AreEqual (null, node.Value, "active event wasn't correctly overridden");

            // then removing override and re-raising
            context.RemoveOverride ("foo32", "foo33");
            node = new Node ();
            context.Raise ("foo32", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo34")]
        private static void foo34 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo35", Overrides = "foo34")]
        private static void foo35 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext25 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising without overriding Active Event
            Node node = new Node ();
            context.Raise ("foo35", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo36")]
        private static void foo36 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo37", Overrides = "foo36")]
        private static void foo37 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "foo38", Overrides = "foo37")]
        private static void foo38 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success2";
        }

        [Test]
        public void ApplicationContext26 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising without overriding Active Event
            Node node = new Node ();
            context.Raise ("foo36", node);
            Assert.AreEqual ("success2", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo39")]
        private static void foo39 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo40", Overrides = "foo39")]
        private void foo40 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [ActiveEvent (Name = "foo41")]
        private void foo41 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success2";
        }

        [Test]
        public void ApplicationContext27 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising without overriding Active Event
            Node node = new Node ();
            context.Raise ("foo39", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");

            // registering "this" as listener, expecting override to kick in
            context.RegisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo39", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");

            // overriding 41 by hand, expecting 41 to kick in on 39
            context.Override ("foo40", "foo41");
            node = new Node ();
            context.Raise ("foo39", node);
            Assert.AreEqual ("success2", node.Value, "active event wasn't correctly overridden");

            // removing override, expecting 40 to kick in
            context.RemoveOverride ("foo40", "foo41");
            node = new Node ();
            context.Raise ("foo39", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");

            // removing this as listener, expecting 39 to kick in
            context.UnregisterListeningObject (this);
            node = new Node ();
            context.Raise ("foo39", node);
            Assert.AreEqual ("failure", node.Value, "active event wasn't correctly overridden");

            // registering this again, and register 41 as override to 39, expecting both to kick in,
            // though in undefined order
            context.RegisterListeningObject (this);
            context.Override ("foo39", "foo41");
            node = new Node ();
            context.Raise ("foo39", node);
            Assert.IsTrue (node.Get<string> (context) == "successsuccess2" || node.Get<string> (context) == "success2success", "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo42")]
        private static void foo42 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "failure";
        }

        [ActiveEvent (Name = "foo43", Overrides = "foo42")]
        private static void foo43 (ApplicationContext context, ActiveEventArgs e)
        {
            context.CallBase (e);
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext28 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising Active Event
            Node node = new Node ();
            context.Raise ("foo42", node);
            Assert.AreEqual ("failuresuccess", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo44")]
        private static void foo44 (ApplicationContext context, ActiveEventArgs e)
        {
            Assert.AreEqual (null, e.Base);
            e.Args.Value += "failure";
        }

        [ActiveEvent (Name = "foo45", Overrides = "foo44")]
        private static void foo45 (ApplicationContext context, ActiveEventArgs e)
        {
            Assert.AreEqual ("foo44", e.Base.Name);
            e.Args.Value = "success";
            context.CallBase (e);
        }

        [Test]
        public void ApplicationContext29 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising Active Event
            Node node = new Node ();
            context.Raise ("foo44", node);
            Assert.AreEqual ("successfailure", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo46")]
        private static void foo46 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "failure1";
        }

        [ActiveEvent (Name = "foo47")]
        private static void foo47 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "failure2";
        }

        [ActiveEvent (Name = "foo48", Overrides = "foo46")]
        [ActiveEvent (Name = "foo49", Overrides = "foo47")]
        private static void foo48_49 (ApplicationContext context, ActiveEventArgs e)
        {
            Assert.IsTrue (e.Name == "foo48" || e.Name == "foo49");
            if (e.Base != null)
                Assert.IsTrue (e.Base.Name == "foo46" || e.Base.Name == "foo47");
            context.CallBase (e);
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext30 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising first Active Event
            Node node = new Node ();
            context.Raise ("foo46", node);
            Assert.AreEqual ("failure1success", node.Value, "active event wasn't correctly overridden");
            
            // raising second Active Event
            node = new Node ();
            context.Raise ("foo47", node);
            Assert.AreEqual ("failure2success", node.Value, "active event wasn't correctly overridden");
            
            // raising Active Event directly, twice, once for each attribute
            node = new Node ();
            context.Raise ("foo48", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");
            node = new Node ();
            context.Raise ("foo49", node);
            Assert.AreEqual ("success", node.Value, "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo50")]
        private static void foo50 (ApplicationContext context, ActiveEventArgs e)
        {
            context.CallBase (e);
            e.Args.Value += "failure1";
        }
        
        [ActiveEvent (Name = "foo51", Overrides = "foo50")]
        private static void foo51 (ApplicationContext context, ActiveEventArgs e)
        {
            context.CallBase (e);
            e.Args.Value += "failure2";
        }
        
        [ActiveEvent (Name = "foo52", Overrides = "foo50")]
        private static void foo52 (ApplicationContext context, ActiveEventArgs e)
        {
            context.CallBase (e);
            e.Args.Value += "failure3";
        }
        
        [ActiveEvent (Name = "foo53_1", Overrides = "foo51")]
        [ActiveEvent (Name = "foo53_2", Overrides = "foo52")]
        private static void foo53 (ApplicationContext context, ActiveEventArgs e)
        {
            context.CallBase (e);
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext31 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            var context = Loader.Instance.CreateApplicationContext ();

            // raising first Active Event
            Node node = new Node ();
            context.Raise ("foo50", node);
            Assert.IsTrue (node.Get<string> (context) == "failure1failure2successfailure1failure3success" || 
                           node.Get<string> (context) == "failure1failure3successfailure1failure2success", "active event wasn't correctly overridden");
        }
        
        [ActiveEvent (Name = "foo54")]
        [ActiveEvent (Name = "foo54")]
        private static void foo54 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void ApplicationContext32 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            context.Raise ("foo54", tmp);
            Assert.AreEqual ("successsuccess", tmp.Value, "Active Event in current assembly did not execute as expected");
        }
    }
}
