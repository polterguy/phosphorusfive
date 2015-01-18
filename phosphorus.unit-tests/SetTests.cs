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
        [Test]
        public void SetValueFromConstant ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :y";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetNameFromConstant ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?name
  :y";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNodeFromConstant ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?node
  x:y";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetValueFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :@/./+/?value
_source:x";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetNameFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?name
  :@/./+/?value
_source:x";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetNodeFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?node
  :@/./+/?node
x:y";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetValueFromPathExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :@/./+/?path
_source:x";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (new Node.DNA ("2"), tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetValueFromCountExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :@/../*/**/?count";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (3, tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetValueFromNodeExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :@/./+/?node
x:y";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Get<Node> ().Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Get<Node> ().Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameFromPathExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?name
  :@/./+/?path
_source:x";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("2", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameFromCountExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?name
  :@/../*/**/?count";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("3", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameFromNodeExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?name
  :@/./+/?node
x:y";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x:y", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetMultipleValuesFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
set:@/-/|/-/-/?value
  :@/./+/?value
_source:x";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleValuesFromConstant ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
set:@/-/|/-/-/?value
  :x";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleNodesFromConstant ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
set:@/-/|/-/-/?node
  x:y";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleNodesFromExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
set:@/-/|/-/-/?node
  :@/./+/?node
_x:y";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("_x", tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x", tmp [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameToNull ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
set:@/-/?name";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetValueToNull ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1:x
set:@/-/?value";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (null, tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetNodeToNull ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1:x
set:@/-/?node";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp.Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("set", tmp [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleNamesToNull ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out1
set:@/-/|/-/-/?name";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [1].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetMultipleValuesToNull ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1:x
_out1:x
set:@/-/|/-/-/?value";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (null, tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetMultipleNodesToNull ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1:x
_out2:x
set:@/-/|/-/-/?node";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp.Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("set", tmp [0].Name, "wrong value of node after executing lambda object");
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
_out1
set:@/-/?node
  _x1:y
  _x2:y";
            context.Raise ("hyperlisp2lambda", tmp);
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
_out1
set:@/-/?node
  :@/./+/|/./+/+/?node
_x1:f
_x2:g";
            context.Raise ("hyperlisp2lambda", tmp);
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
_out1
set:@/-/?path";
            context.Raise ("hyperlisp2lambda", tmp);
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
_out1
set:@/-/?count";
            context.Raise ("hyperlisp2lambda", tmp);
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
_out1
set:@/-/?value
  :@/./+/|/./+/+/?value
_x1:f
_x2:g";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
        }

        [Test]
        [ExpectedException]
        public void SyntaxError6 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
set:@/-/?name
  :@/./+/|/./+/+/?value
_x1:f
_x2:g";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError7 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
set:@/-/?node
  :@/./+/|/./+/+/?value
_x1:f
_x2:g";
            context.Raise ("hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
        }
    }
}

