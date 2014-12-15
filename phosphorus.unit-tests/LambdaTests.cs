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
    public class LambdaTests
    {
        [Test]
        public void InvokeEmptyLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        public void InvokeSimpleSetLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :howdy";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeSimpleSetCopyLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :howdy";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda.copy", tmp);
            Assert.AreNotEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeSimpleSetImmutableLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :howdy";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda.immutable", tmp);
            Assert.AreNotEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
lambda
  set:@/./-/?value
    :howdy";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerImmutableLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
lambda.immutable
  set:@/./-/?value
    :howdy
  set:@/+/?value
    :no-show
  _no_out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerCopyLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
lambda.copy
  set:@/./-/?value
    :howdy
  set:@/+/?value
    :no-show
  _no_out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (null, tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerImmutableExpressionLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_copy
_exe
  set:@/./-/?value
    :howdy
  set:@/+/?value
    :no-show
  _no_out
lambda.immutable:@/-/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerCopyExpressionLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_copy
_exe
  set:@/./-/?value
    :howdy
  set:@/+/?value
    :no-show
  _no_out
lambda.copy:@/-/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (null, tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerTextWithParametersLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_input
set:@/+/*/_input/?value
  :@/./-/?node
lambda.copy:@""
set:@/./*/_input/#/?value
  :howdy
""
  _input";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerNodeWithParametersLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_input
set:@/+/*/_input/?value
  :@/./-/?node
lambda:node:@""_x
  set:@/./*/_input/#/?value
    :howdy
  set:@/?value
    :no-val""
  _input";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("no-val", tmp [2].Get<Node> () [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerNodeCopyWithParametersLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_input
set:@/+/*/_input/?value
  :@/./-/?node
lambda.copy:node:@""_x
  set:@/./*/_input/#/?value
    :howdy
  set:@/?value
    :no-val""
  _input";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreNotEqual ("no-val", tmp [2].Get<Node> () [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerNodeImmutableWithParametersLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_input
set:@/+/*/_input/?value
  :@/./-/?node
lambda.immutable:node:@""_x
  set:@/./*/_input/#/?value
    :howdy
  set:@/?value
    :no-val""
  _input";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreNotEqual ("no-val", tmp [2].Get<Node> () [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerExpressionLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
_x
  set:@/./-/?value
    :howdy
lambda:@/-/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeMultipleExpressionsLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out1
_out2
_x
  set:@/./-/-/?value
    :howdy
_x
  set:@/./-/-/?value
    :world
lambda:@/../*/_x/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeMultipleExpressionsValueWithArgsLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:node:@""_x
  set:@/./*/_arg/#/?value
    :howdy""
_x:@""set:@/./*/_arg/#/?value
  :{0} world
    :@/./././*/_arg/#/?value""
_arg
set:@/+/*/_arg/?value
  :@/./-/?node
lambda:@/../*/_x/?value
  _arg";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("howdy world", tmp [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void PassInReferenceOutputNode ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
set:@/+/#/?value
  :world";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);

            // here we pass in a "reference node" that's being used for retrieving output from lambda
            // basically the Value of one of the execution nodes is a node itself, which we can retrieve after execution, since the
            // immutable parts of "pf.lambda" will not clone the value of nodes, but create a "shallow copy" of the value of all nodes
            // cloned. hence the node will be copied by reference inside the "pf.lambda", meaning the node we pass in, will be accessible
            // from the outside of the "pf.lambda" after execution
            tmp.Add (new Node (string.Empty, new Node ()));

            // executing lambda, now with a "reference node" being used for retrieving value(s) from inside the lambda object
            context.Raise ("lambda.copy", tmp);
            Assert.AreEqual ("world", tmp [1].Get<Node> ().Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [1].Get<Node> ().Name, "wrong value of node after executing lambda object");
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
set:syntax-error";
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
_x
  set:syntax-error
lambda:@/-/?node";
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
lambda
  set:syntax-error";
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
lambda.copy
  set:syntax-error";
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
lambda.immutable
  set:syntax-error";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
lambda.copy:@/+/?node
_x
  set:syntax-error";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
lambda.immutable:@/+/?node
_x
  set:syntax-error";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError8 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
lambda:node:""
_x
  set:syntax-error""";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError9 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
lambda.copy:node:""
_x
  set:syntax-error""";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError10 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
lambda.immutable:node:""
_x
  set:syntax-error""";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
    }
}
