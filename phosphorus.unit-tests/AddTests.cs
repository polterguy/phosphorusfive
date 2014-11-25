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
    public class AddTests
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
        public void AddNodes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  x:y
    z:q
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("x", _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", _executionNodes [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", _executionNodes [0] [0] [0].Value, "wrong value of node after executing lambda object");

            // making sure source nodes are still around, and that add adds a copy of the nodes to add
            Assert.AreEqual ("x", _executionNodes [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", _executionNodes [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", _executionNodes [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", _executionNodes [1] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodesFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  :@/./+/*/?node
_x
  x:y
    z:q
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("x", _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", _executionNodes [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", _executionNodes [0] [0] [0].Value, "wrong value of node after executing lambda object");

            // making sure source nodes are still around, and that add adds a copy of the nodes to add
            Assert.AreEqual ("x", _executionNodes [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", _executionNodes [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", _executionNodes [2] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", _executionNodes [2] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodeValuesFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  :@/./+/*/?value
_x
  x:y
    z:q
  x2:y2
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual (string.Empty, _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, _executionNodes [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", _executionNodes [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (2, _executionNodes [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, _executionNodes [0] [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodeNamesFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  :@/./+/*/**/?name
_x
  x:y
    z:q
  x2:y2
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual (string.Empty, _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, _executionNodes [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", _executionNodes [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, _executionNodes [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x2", _executionNodes [0] [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (3, _executionNodes [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, _executionNodes [0] [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodePathsFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  :@/./+/*/**/?path
_x
  x:y
    z:q
  x2:y2
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual (string.Empty, _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (new Node.DNA ("2-0"), _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, _executionNodes [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (new Node.DNA ("2-0-0"), _executionNodes [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, _executionNodes [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (new Node.DNA ("2-1"), _executionNodes [0] [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (3, _executionNodes [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, _executionNodes [0] [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodeCountFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  :@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual (string.Empty, _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (3, _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, _executionNodes [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, _executionNodes [0] [0].Count, "wrong value of node after executing lambda object");
        }

        [Test]
        public void AlmostExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  :@./+/*/**/?count
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual (string.Empty, _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("@./+/*/**/?count", _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, _executionNodes [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void MultipleDestinationsWithExpressionSource ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
add:@/-/|/-/-/?node
  :@/./+/?value
:dfg
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("dfg", _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("dfg", _executionNodes [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", _executionNodes [1] [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void MultipleDestinationsWithStaticSource ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
add:@/-/|/-/-/?node
  :dfg
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("dfg", _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("dfg", _executionNodes [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", _executionNodes [1] [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddMultipleSourceNodes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
add:@/-/|/-/-/?node
  :@/./+/|/./+/+/?node
_x1:y1
_x2:y2
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("_x1", _executionNodes [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", _executionNodes [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", _executionNodes [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", _executionNodes [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x1", _executionNodes [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", _executionNodes [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", _executionNodes [1] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", _executionNodes [1] [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?value
  :@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?name
  :@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?count
  :@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?path
  :@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError5 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
        }
    }
}

