
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
  lambda
    set:@/../*/_out/#/?value
      :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
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
  lambda
    set:@/../*/_out/#/?value
      :success";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);

            // then executing our lambda event in another context
            context = Loader.Instance.CreateApplicationContext ();
            tmp = new Node ();
            tmp.Value = @"
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo2
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
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
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
event:test.foo3
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo3
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
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
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
event:test.foo4
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);

            // then invoking active events in different context
            tmp = new Node ();
            tmp.Value = @"
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo4
  _out";
            context = Loader.Instance.CreateApplicationContext ();
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
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
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
    set:@/-/?name
      :mumbo
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo5
  _out
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo5
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("success", tmp [4].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void VerifyLambdaIsRoot ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo6
  lambda
    if:@/../?name
      =:test.foo6
      and:@/../?value
        =:howdy
      lambda
        set:@/../*/_out/#/?value
          :{0}{1}
            :@/./././*/_out/#/?value
            :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo6:howdy
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
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
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
delete-event:test.foo7
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo7
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (null, tmp [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DeleteMultipleEventsWithSameName ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo8
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
event:test.foo8
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
delete-event:test.foo8
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo8
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (null, tmp [3].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CreateOverriddenEvent ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo9
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo10
  overrides:test.foo9
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo9
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CreateMultipleOverriddenEvents ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo11
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo12
  overrides:test.foo11
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo13
  overrides:test.foo11
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo11
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [3].Get<string> () == "success2success1" || tmp [3].Get<string> () == "success1success2", "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CreateMultipleEventsOneOverridden ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo14
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo15
  overrides:test.foo14
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo15
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo14
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [3].Get<string> () == "success1success2" || tmp [3].Get<string> () == "success2success1", "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void OverrideEventThatIsOverridingEvent ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo16
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo17
  overrides:test.foo16
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo18
  overrides:test.foo17
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo16
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [3].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeEventOverriddenInChainWithTwoImplementations ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo19
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo20
  overrides:test.foo19
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo21
  overrides:test.foo20
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo21
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo19
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [4].Get<string> () == "success1success2" || tmp [4].Get<string> () == "success2success1", "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InvokeMiddleEventInChainOfOverridesWithMultipleImplementations ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo22
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo23
  overrides:test.foo22
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo24
  overrides:test.foo23
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo24
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo23
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [4].Get<string> () == "success1success2" || tmp [4].Get<string> () == "success2success1", "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CreateOneEventOverridingMultipleBaseEvents ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo25
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo26
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo27
  overrides
    :test.foo25
    :test.foo26
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo25
  _out
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo26
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [3].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("success", tmp [6].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CallBaseFromOverridingEvent ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo28
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo29
  overrides:test.foo28
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo28
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("failuresuccess", tmp [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CallBaseFromOverridingEventAfterExecutionOfSelf ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.bar1
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.bar2
  overrides:test.bar1
  lambda
    set:@/../*/_out/#/?value
      :success
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.bar1
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("successfailure", tmp [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void CallBaseForEventsOverridingOverriddenEvents ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo30
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo31
  overrides:test.foo30
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
event:test.foo32
  overrides:test.foo31
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success3
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo30
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success1success2success3", tmp [3].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CallBaseForEventOverriddenMultipleTimes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo33
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo34
  overrides:test.foo33
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
event:test.foo35
  overrides:test.foo33
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success3
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo33
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [3].Get<string> () == "success1success2success1success3" || tmp [3].Get<string> () == "success1success3success1success2", "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CallBaseForEventOverridingMultipleEvents ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo36
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo37
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
event:test.foo38
  overrides
    :test.foo36
    :test.foo37
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success3
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo36
  _out
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo37
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success1success3", tmp [3].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("success2success3", tmp [6].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CallBaseForEventNotOverriding ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo39
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo39
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CallOverridingEventDirectlyWithCallBase ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo40
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo41
  overrides:test.foo40
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo41
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DynamicallyOverrideEventAfterCreation ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo42
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo43
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
override:test.foo42
  with:test.foo43
set:@/+/*/_out/?value
  :@/./-/-/?node
test.foo42
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DynamicallyOverrideEventAfterCreationMultipleTimes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo44
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo45
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo46
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
_out
override:test.foo44
  with
    :test.foo45
    :test.foo46
set:@/+/*/_out/?value
  :@/./-/-/?node
test.foo44
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [3].Get<string> () == "success1success2" || tmp [2].Get<string> () == "success2success1", "wrong value of node after executing lambda object");
        }

        [Test]
        public void DynamicallyOverrideEventAfterCreationMultipleTimesCallBase ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo47
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo48
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo49
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
_out
override:test.foo47
  with
    :test.foo48
    :test.foo49
set:@/+/*/_out/?value
  :@/./-/-/?node
test.foo47
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [3].Get<string> () == "failuresuccess1failuresuccess2" || tmp [2].Get<string> () == "failuresuccess2failuresuccess1", "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DynamicallyOverrideEventAfterCreationMultipleOverridesCallBase ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo50
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo51
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
event:test.foo52
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success3
_out
override:test.foo50
  with:test.foo51
override:test.foo51
  with:test.foo52
set:@/+/*/_out/?value
  :@/./-3/?node
test.foo50
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success2success1success3", tmp [3].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DynamicallyOverrideEventMultipleTimesEventsCallBase ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo53
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo54
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo55
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
_out
override:test.foo53
  with:test.foo54
override:test.foo53
  with:test.foo55
set:@/+/*/_out/?value
  :@/./-3/?node
test.foo53
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [3].Get<string> () == "failuresuccess1failuresuccess2" || tmp [2].Get<string> () == "failuresuccess2failuresuccess1", "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DynamicallyOverrideMultipleEventsCallBase ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo56
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success1
event:test.foo57
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success2
event:test.foo58
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success3
_out
override:test.foo56
  with:test.foo58
override:test.foo57
  with:test.foo58
set:@/+/*/_out/?value
  :@/./-3/?node
test.foo56
  _out
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo57
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success1success3", tmp [3].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("success2success3", tmp [8].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DynamicallyOverrideEventCallDirectly ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo59
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :failure
event:test.foo60
  lambda
    set:@/+/*/_out/?value
      :@/../*/_out/?value
    call-base
      _out
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
override:test.foo59
  with:test.foo60
set:@/+/*/_out/?value
  :@/./-2/?node
test.foo60
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CallEmptyBase ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo61
  lambda
    bar
event:test.foo62
  overrides:test.foo61
  lambda
    call-base
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/./././*/_out/#/?value
        :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo61
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [2].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void DynamicallyDeleteOverride ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
event:test.foo63
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/../*/_out/#/?value
        :success
event:test.foo64
  lambda
    set:@/../*/_out/#/?value
      :{0}{1}
        :@/../*/_out/#/?value
        :failure
_out
override:test.foo63
  with:test.foo64
delete-override:test.foo63
  :test.foo64
set:@/+/*/_out/?value
  :@/./-3/?node
test.foo63
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("success", tmp [2].Value, "wrong value of node after executing lambda object");
        }
    }
}
