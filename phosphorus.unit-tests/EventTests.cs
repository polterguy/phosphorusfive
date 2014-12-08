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
    public class EventTests
    {
        [Test]
        public void CreateAndExecuteSimpleLambdaEvent ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo
  code
    set:@/../_out/#/?value
      :success
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo
  _out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void CreateAndExecuteLambdaEventInDifferentContext ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");

            // first creating our event within one context
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo2
  code
    set:@/../_out/#/?value
      :success";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);

            // then executing our lambda event in another context
            context = Loader.Instance.CreateApplicationContext ();
            tmp = new Node ();
            tmp.Value = @"
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo2
  _out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void CreateAndExecuteLambdaEventWithTwoHandlers ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo3
  code
    set:@/../_out/#/?value
      :{0}{1}
        :@/./././_out/#/?value
        :success
event:test.foo3
  code
    set:@/../_out/#/?value
      :{0}{1}
        :@/./././_out/#/?value
        :success
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo3
  _out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("successsuccess", tmp [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CreateAndExecuteLambdaEventWithTwoHandlersInDifferentContexts ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();

            // first creating events in one context
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo4
  code
    set:@/../_out/#/?value
      :{0}{1}
        :@/./././_out/#/?value
        :success
event:test.foo4
  code
    set:@/../_out/#/?value
      :{0}{1}
        :@/./././_out/#/?value
        :success";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);

            // then invoking active events in different context
            tmp = new Node ();
            tmp.Value = @"
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo4
  _out";
            context = Loader.Instance.CreateApplicationContext ();
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("successsuccess", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void VerifyEventExecutionIsImmutable ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo5
  code
    set:@/../_out/#/?value
      :{0}{1}
        :@/./././_out/#/?value
        :success
    set:@/-/?name
      :mumbo
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo5
  _out
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo5
  _out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("success", tmp [4].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void VerifyCodeIsRoot ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo6
  code
    if:@/../?name
      =:code
      lambda
        set:@/../_out/#/?value
          :{0}{1}
            :@/./././_out/#/?value
            :success
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo6
  _out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DeleteEvent ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo7
  code
    set:@/../_out/#/?value
      :{0}{1}
        :@/./././_out/#/?value
        :success
delete-event:test.foo7
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo7
  _out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("success", tmp [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DeleteMultipleEvents ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo8
  code
    set:@/../_out/#/?value
      :{0}{1}
        :@/./././_out/#/?value
        :success
event:test.foo8
  code
    set:@/../_out/#/?value
      :{0}{1}
        :@/./././_out/#/?value
        :success
delete-event:test.foo8
_out
set:@/+/_out/?value
  :@/./-/?node
test.foo8
  _out";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("success", tmp [3].Value, "wrong value of node after executing lambda object");
        }
    }
}

