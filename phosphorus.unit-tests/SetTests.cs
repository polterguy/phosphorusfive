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
    public class SetTests
    {
        private static Node _executionNodes;

        // since pf.lambda executes its nodes "immutable", we need a mechanism to store the execution nodes
        // before the pf.lambda returns. this event handler stores the nodes in a static variable, such
        // that we have something to compare when comparing the "output" after execution after execution of
        // lambda objects. notice we could have used "reference nodes", but this construct kind of creates more
        // beautiful code, and will work as long as all tests are executed on the same thread consecutively,
        // and two tests are never executed at the same time, creating a race condition
        // for another way to retrieve output from "pf.lambda" invocations, see the "PassInReferenceOutputNode"
        // unit tests further down in the "LambdaTests.cs" file, which is a cleaner way for client code to retrieve output values
        [ActiveEvent (Name = "tests.store-nodes")]
        private static void StoreExecutionNodes (ApplicationContext context, ActiveEventArgs e)
        {
            _executionNodes = e.Args.Root.Clone ();
        }

        [Test]
        public void SetValueFromConstant ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :y
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("y", _executionNodes [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetValueFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :@/./+/?value
_source:x
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("x", _executionNodes [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetNameFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?name
  :@/./+/?value
_source:x
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("x", _executionNodes [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetValueFromPathExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :@/./+/?path
_source:x
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual (new Node.DNA ("2"), _executionNodes [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleValuesFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
set:@/-/|/-/-/?value
  :@/./+/?value
_source:x
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("x", _executionNodes [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", _executionNodes [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleValuesFromConstant ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
set:@/-/|/-/-/?value
  :x
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("x", _executionNodes [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", _executionNodes [1].Value, "wrong value of node after executing lambda object");
        }
    }
}

