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
        [Test]
        public void AddNodes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  x:y
    z:q";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [0] [0] [0].Value, "wrong value of node after executing lambda object");

            // making sure source nodes are still around, and that add adds a copy of the nodes to add
            Assert.AreEqual ("x", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [1] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodesFromExpression ()
        {
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
    z:q";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [0] [0] [0].Value, "wrong value of node after executing lambda object");

            // making sure source nodes are still around, and that add adds a copy of the nodes to add
            Assert.AreEqual ("x", tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [2] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [2] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodeValuesFromExpression ()
        {
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
  x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (2, tmp [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [0] [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodeNamesFromExpression ()
        {
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
  x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x2", tmp [0] [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (3, tmp [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [0] [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodePathsFromExpression ()
        {
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
  x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (new Node.DNA ("2-0"), tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (new Node.DNA ("2-0-0"), tmp [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (new Node.DNA ("2-1"), tmp [0] [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (3, tmp [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [0] [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodeCountFromExpression ()
        {
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
  x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (3, tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [0] [0].Count, "wrong value of node after executing lambda object");
        }

        [Test]
        public void AlmostExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node
  :"" @./+/*/**/?count""";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (" @./+/*/**/?count", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void MultipleDestinationsWithExpressionSource ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
add:@/-/|/-/-/?node
  :@/./+/?value
:dfg";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("dfg", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("dfg", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", tmp [1] [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void MultipleDestinationsWithStaticSource ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
add:@/-/|/-/-/?node
  :dfg";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("dfg", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("dfg", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", tmp [1] [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddMultipleSourceNodes ()
        {
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
_x2:y2";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("_x1", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x1", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [1] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [1] [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
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
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
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
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
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
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
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
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError5 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
add:@/-/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
    }
}

