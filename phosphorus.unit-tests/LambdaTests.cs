
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
    public class LambdaTests : TestBase
    {
        public LambdaTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void InvokeEmptyLambda ()
        {
            ExecuteLambda ("");
        }
        
        [Test]
        public void InvokeSimpleSetLambda ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:howdy");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeSimpleSetCopyLambda ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  :howdy");
            Assert.AreNotEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeSimpleSetImmutableLambda ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  :howdy");
            Assert.AreNotEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerLambda ()
        {
            Node tmp = ExecuteLambda (@"_out
lambda
  set:@/./-/?value
    source:howdy");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerImmutableLambda ()
        {
            Node tmp = ExecuteLambda (@"_out
lambda.immutable
  set:@/./-/?value
    source:howdy
  set:@/+/?value
    source:no-show
  _no_out");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerCopyLambda ()
        {
            Node tmp = ExecuteLambda (@"_out
lambda.copy
  set:@/./-/?value
    :howdy
  set:@/+/?value
    :no-show
  _no_out");
            Assert.AreEqual (null, tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerImmutableExpressionLambda ()
        {
            Node tmp = ExecuteLambda (@"_copy
_exe
  set:@/./-/?value
    source:howdy
  set:@/+/?value
    source:no-show
  _no_out
lambda.immutable:@/-/?node");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerCopyExpressionLambda ()
        {
            Node tmp = ExecuteLambda (@"_copy
_exe
  set:@/./-/?value
    :howdy
  set:@/+/?value
    :no-show
  _no_out
lambda.copy:@/-/?node");
            Assert.AreEqual (null, tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerTextWithParametersLambda ()
        {
            Node tmp = ExecuteLambda (@"_input
set:@/+/*/_input/?value
  source:@/./-/?node
lambda.copy:@""
set:@/./*/_input/#/?value
  source:howdy
""
  _input");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerNodeWithParametersLambda ()
        {
            Node tmp = ExecuteLambda (@"_input
set:@/+/*/_input/?value
  source:@/./-/?node
lambda:node:@""_x
  set:@/./*/_input/#/?value
    source:howdy
  set:@/?value
    source:no-val""
  _input");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("no-val", tmp [2].Get<Node> () [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerNodeCopyWithParametersLambda ()
        {
            Node tmp = ExecuteLambda (@"_input
set:@/+/*/_input/?value
  source:@/./-/?node
lambda.copy:node:@""_x
  set:@/./*/_input/#/?value
    source:howdy
  set:@/?value
    source:no-val""
  _input");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreNotEqual ("no-val", tmp [2].Get<Node> () [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerNodeImmutableWithParametersLambda ()
        {
            Node tmp = ExecuteLambda (@"_input
set:@/+/*/_input/?value
  source:@/./-/?node
lambda.immutable:node:@""_x
  set:@/./*/_input/#/?value
    source:howdy
  set:@/?value
    source:no-val""
  _input");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreNotEqual ("no-val", tmp [2].Get<Node> () [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InvokeInnerExpressionLambda ()
        {
            Node tmp = ExecuteLambda (@"_out
_x
  set:@/./-/?value
    source:howdy
lambda:@/-/?node");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeMultipleExpressionsLambda ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
_x
  set:@/./-/-/?value
    source:howdy
_x
  set:@/./-/-/?value
    source:world
lambda:@/../*/_x/?node");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeMultipleExpressionsValueWithArgsLambda ()
        {
            Node tmp = ExecuteLambda (@"_x:node:@""_x
  set:@/./*/_arg/#/?value
    source:howdy""
_x:@""set:@/./*/_arg/#/?value
  source:{0} world
    :@/./././*/_arg/#/?value""
_arg
set:@/+/*/_arg/?value
  source:@/./-/?node
lambda:@/../*/_x/?value
  _arg");
            Assert.AreEqual ("howdy world", tmp [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void PassInReferenceOutputNode ()
        {
            Node tmp = new Node ();
            tmp.Value = @"set:@/+/#/?value
  source:world";
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);

            // here we pass in a "reference node" that's being used for retrieving output from lambda
            // basically the Value of one of the execution nodes is a node itself, which we can retrieve after execution, since the
            // immutable parts of "pf.lambda" will not clone the value of nodes, but create a "shallow copy" of the value of all nodes
            // cloned. hence the node will be copied by reference inside the "pf.lambda", meaning the node we pass in, will be accessible
            // from the outside of the "pf.lambda" after execution
            tmp.Add (new Node (string.Empty, new Node ()));

            // executing lambda, now with a "reference node" being used for retrieving value(s) from inside the lambda object
            _context.Raise ("lambda.copy", tmp);
            Assert.AreEqual ("world", tmp [1].Get<Node> ().Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [1].Get<Node> ().Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            ExecuteLambda (@"set:syntax-error");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            ExecuteLambda (@"_x
  set:syntax-error
lambda:@/-/?node");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            ExecuteLambda (@"lambda
  set:syntax-error");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            ExecuteLambda (@"lambda.copy
  set:syntax-error");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError5 ()
        {
            ExecuteLambda (@"lambda.immutable
  set:syntax-error");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError6 ()
        {
            ExecuteLambda (@"lambda.copy:@/+/?node
_x
  set:syntax-error");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError7 ()
        {
            ExecuteLambda (@"lambda.immutable:@/+/?node
_x
  set:syntax-error");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError8 ()
        {
            ExecuteLambda (@"lambda:node:""
_x
  set:syntax-error""");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError9 ()
        {
            ExecuteLambda (@"lambda.copy:node:""
_x
  set:syntax-error""");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError10 ()
        {
            ExecuteLambda (@"lambda.immutable:node:""
_x
  set:syntax-error""");
        }
    }
}
