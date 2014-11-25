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
    public class ContextAndLoaderTests
    {
        [Test]
        public void LoadAssembly ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
        }

        [Test]
        public void LoadAssemblyWithExtension ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.lambda.dll");
        }

        [Test]
        public void LoadAssemblyTwice ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
        }
        
        [Test]
        public void LoadTwoAssemblies ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
        }
        
        [Test]
        public void LoadExecutingAssembly ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
        }

        [Test]
        public void CreateApplicationContext ()
        {
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Assert.IsNotNull (context);
        }

        [ActiveEvent (Name = "foo")]
        private static void foo (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }
        
        [Test]
        public void RaiseActiveEventInCurrentAssembly ()
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

        [Test]
        public void RaiseActiveEventWithTwoHandlers ()
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

        [Test]
        public void RaiseInstanceActiveEvent ()
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

        [Test]
        public void RaiseInstanceActiveEventWithTwoListeners ()
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

        [Test]
        public void RaiseActiveEventWithBothStaticAndInstanceListener ()
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

        [Test]
        public void CreateContextRegisterInstanceDiscardAndReCreateContext ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            context = Loader.Instance.CreateApplicationContext ();
            context.RegisterListeningObject (this);
            Node tmp = new Node (string.Empty, string.Empty);
            context.Raise ("foo6", tmp);
            Assert.AreEqual ("success", tmp.Value, "context contained previously registered instance listener");
        }
        
        [ActiveEvent (Name = "foo7")]
        private static void foo7 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "success";
        }

        [Test]
        public void RegisterListenerObjectTwice ()
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
        public void UnregisterListeningObject ()
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
        public void UnregisterListeningObjectVerifyStaticHandlerStillExecutes ()
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
        public void UnloadAssemblyVerifyStaticEventHandlerIsNotInvoked ()
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
        public void UnloadAssemblyVerifyInstanceEventHandlerIsNotInvoked ()
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
    }
}

