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
        private static Node _executionNodes;

        // since pf.lambda executes its nodes "immutable", we need a mechanism to store the execution nodes
        // before the pf.lambda returns. this event handler stores the nodes in a static variable, such
        // that we have something to compare when comparing the "output" after execution after execution of
        // lambda objects
        [ActiveEvent (Name = "tests.store-nodes")]
        private static void StoreExecutionNodes (ApplicationContext context, ActiveEventArgs e)
        {
            _executionNodes = e.Args.Root.Clone ();
        }

        [Test]
        public void InvokeSimpleLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
        }
        
        [Test]
        public void InvokeSimpleStatementLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
set:@/-/?value
  :howdy
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("howdy", _executionNodes [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
lambda
  set:@/./-/?value
    :howdy
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("howdy", _executionNodes [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerExpressionLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
_x
  set:@/./-/?value
    :howdy
pf.lambda:@/-/?node
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("howdy", _executionNodes [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeMultipleInnerExpressionsLambda ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
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
pf.lambda:@/../_x/?node
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("howdy", _executionNodes [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", _executionNodes [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerExpressionLambdaWithParameters ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_out
_x
  set:@/./-/?value
    :@/././_val/#/?value
pf.lambda:@/-/?node
  _val:howdy
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("howdy", _executionNodes [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerExpressionLambdaReturnValue ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  set:@/./_val/#/?value
    :world
pf.lambda:@/-/?node
  _val:howdy
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("world", _executionNodes [1][0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerExpressionLambdaReturnNodes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  add:@/./_val/#/?node
    x:hello
  add:@/./_val/#/?node
    y:world
lambda:@/-/?node
  _val
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("x", _executionNodes [1][0][0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("hello", _executionNodes [1][0][0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", _executionNodes [1][0][1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", _executionNodes [1][0][1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerExpressionReturningText ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
@""set:@/+/#/?value
  :hello""
lambda:@/-/?name
  _val
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("hello", _executionNodes [1][0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeInnerMultipleExpressionReturningText ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:@""add:@/+/#/?node
  x:hello""
lambda:@/../_x/?value
  _val
_x:@""add:@/+/#/?node
  y:world""
_x:node:@""add:@/+/#/?node
  z:2.0""
tests.store-nodes";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("pf.lambda", tmp);
            Assert.AreEqual ("x", _executionNodes [1][0][0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("hello", _executionNodes [1][0][0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", _executionNodes [1][0][1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", _executionNodes [1][0][1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", _executionNodes [1][0][2].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("2.0", _executionNodes [1][0][2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("add", _executionNodes [3].Get<Node> ().Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, _executionNodes [3].Get<Node> ().Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, _executionNodes [3].Get<Node> ().Parent, "wrong value of node after executing lambda object");
        }
    }
}

