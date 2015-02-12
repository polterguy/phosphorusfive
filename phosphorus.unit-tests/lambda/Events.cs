
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests.lambda
{
    [TestFixture]
    public class Events : TestBase
    {
        public Events ()
            : base ("phosphorus.hyperlisp", "phosphorus.lambda")
        { }

        /// <summary>
        /// creates a simple event, and invokes it, to verify events works as they should
        /// </summary>
        [Test]
        public void Events01 ()
        {
            Node node = ExecuteLambda (@"event:test.foo1
  lambda
    set:@/././*/_out/?value
      source:success
test.foo1
  _out");
            Assert.AreEqual ("success", node [1] [0].Value);
        }
        
        /// <summary>
        /// creates a simple event, and invokes it with an expression as root argument, consuming this expression
        /// from inside the event as a referenced expression, to verify events can take parameters
        /// as values when invoked
        /// </summary>
        [Test]
        public void Events02 ()
        {
            Node node = ExecuteLambda (@"event:test.foo2
  lambda
    set:@@/././?value
      source:success
test.foo2:@/+/?value
_out");
            Assert.AreEqual ("success", node [2].Value);
        }
        
        /// <summary>
        /// creates an event with multiple [lambda] objects, making sure events can take multiple lambda statements,
        /// and execute them in order declared
        /// </summary>
        [Test]
        public void Events03 ()
        {
            Node node = ExecuteLambda (@"event:test.foo3
  lambda
    set:@/././*/_out/?value
      source:succ
  lambda
    set:@/././*/_out/?value
      source:{0}{1}
        :@/././././*/_out/?value
        :ess
test.foo3
  _out");
            Assert.AreEqual ("success", node [1] [0].Value);
        }
        
        /// <summary>
        /// creates an event with a [lambda.copy] statement as child, making sure lambda objects are
        /// invoket correctly from events
        /// </summary>
        [Test]
        public void Events04 ()
        {
            Node node = ExecuteLambda (@"event:test.foo4
  lambda.copy
    set:@/././*/_out/?value
      source:error
test.foo4
  _out:success");
            Assert.AreEqual ("success", node [1] [0].Value);
        }
        
        /// <summary>
        /// creates an event in one Application Context, to invoke it in a different, making sure
        /// events works the way they should
        /// </summary>
        [Test]
        public void Events05 ()
        {
            ExecuteLambda (@"event:test.foo5
  lambda
    set:@/././*/_out/?value
      source:success");
            Node node = ExecuteLambda (@"test.foo5
  _out");
            Assert.AreEqual ("success", node [0] [0].Value);
        }
        
        /// <summary>
        /// creates an event twice, to make sure both invocations are invoked, in order of creation
        /// </summary>
        [Test]
        public void Events06 ()
        {
            Node node = ExecuteLambda (@"event:test.foo6
  lambda
    set:@/././*/_out/?value
      source:succ
event:test.foo6
  lambda
    set:@/././*/_out/?value
      source:{0}{1}
        :@/././././*/_out/?value
        :ess
test.foo6
  _out");
            Assert.AreEqual ("success", node [2] [0].Value);
        }
        
        /// <summary>
        /// creates an event using a formatting expression, making sure events works as they should
        /// </summary>
        [Test]
        public void Events07 ()
        {
            ExecuteLambda (@"event:test.f{0}
  :oo7
  lambda
    set:@/././*/_out/?value
      source:success");
            Node node = ExecuteLambda (@"test.foo7
  _out");
            Assert.AreEqual ("success", node [0] [0].Value);
        }
        
        /// <summary>
        /// creates an event, for then to delete it, making sure deletion is successful
        /// </summary>
        [Test]
        public void Events08 ()
        {
            Node node = ExecuteLambda (@"event:test.foo8
  lambda
    set:@/././*/_out/?value
      source:error
delete-event:test.foo8
test.foo8
  _out:success");
            Assert.AreEqual ("success", node [2] [0].Value);
        }
        
        /// <summary>
        /// creates an event, for then to delete it, using a formatting expression,
        /// making sure deletion is successful
        /// </summary>
        [Test]
        public void Events09 ()
        {
            Node node = ExecuteLambda (@"event:test.foo9
  lambda
    set:@/././*/_out/?value
      source:error
delete-event:test.{0}
  :foo9
test.foo9
  _out:success");
            Assert.AreEqual ("success", node [2] [0].Value);
        }
        
        /// <summary>
        /// creates two events, for then to delete them both, using expressions,
        /// making sure deletion is successful
        /// </summary>
        [Test]
        public void Events10 ()
        {
            Node node = ExecuteLambda (@"event:test.foo10
  lambda
    set:@/././*/_out/?value
      source:error
event:test.foo11
  lambda
    set:@/././*/_out/?value
      source:error
delete-event:@/../*/""/test/""/?name
test.foo10
  _out:success
test.foo11
  _out:success");
            Assert.AreEqual ("success", node [3] [0].Value);
            Assert.AreEqual ("success", node [4] [0].Value);
        }














        //[Test]
        public void CreateAndExecuteLambdaEventInDifferentContext ()
        {
            Node node = ExecuteLambda (@"
event:test.foo2
  set:@/./*/_out/#/?value
    :success");

            // then executing our lambda event in another context
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            node = new Node ();
            node.Value = @"
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo2
  _out";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", node);
            context.Raise ("lambda", node);
            Assert.AreEqual ("success", node [0].Value);
        }

        //[Test]
        public void CreateAndExecuteLambdaEventWithTwoHandlers ()
        {
            Node node = ExecuteLambda (@"
event:test.foo3
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
event:test.foo3
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo3
  _out");
            Assert.AreEqual ("successsuccess", node [2].Value);
        }
        
        //[Test]
        public void CreateAndExecuteLambdaEventWithTwoHandlersInDifferentContexts ()
        {
            Node node = ExecuteLambda (@"
event:test.foo4
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
event:test.foo4
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success");

            // then invoking active events in different context
            node = new Node ();
            node.Value = @"
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo4
  _out";
            _context = Loader.Instance.CreateApplicationContext ();
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", node);
            _context.Raise ("lambda", node);
            Assert.AreEqual ("successsuccess", node [0].Value);
        }
        
        //[Test]
        public void VerifyEventExecutionIsImmutable ()
        {
            Node node = ExecuteLambda (@"
event:test.foo5
  set:@/./*/_out/#/?value
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
  _out");
            Assert.AreEqual ("success", node [1].Value);
            Assert.AreEqual ("success", node [4].Value);
        }
        
        //[Test]
        public void VerifyLambdaIsRoot ()
        {
            Node node = ExecuteLambda (@"
event:test.foo6
  if:@/./?name
    =:test.foo6
    and:@/../?value
      =:howdy
    lambda
      set:@/""..test.foo6""/*/_out/#/?value
        :{0}{1}
          :@/./././*/_out/#/?value
          :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo6:howdy
  _out");
            Assert.AreEqual ("success", node [1].Value);
        }
        
        //[Test]
        public void DeleteEvent ()
        {
            Node node = ExecuteLambda (@"
event:test.foo7
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
delete-event:test.foo7
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo7
  _out");
            Assert.AreEqual (null, node [2].Value);
        }
        
        //[Test]
        public void DeleteMultipleEventsWithSameName ()
        {
            Node node = ExecuteLambda (@"
event:test.foo8
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
event:test.foo8
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
delete-event:test.foo8
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo8
  _out");
            Assert.AreEqual (null, node [3].Value);
        }
        
        //[Test]
        public void CreateOverriddenEvent ()
        {
            Node node = ExecuteLambda (@"
event:test.foo9
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo10
  overrides:test.foo9
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo9
  _out");
            Assert.AreEqual ("success", node [2].Value);
        }
        
        //[Test]
        public void CreateMultipleOverriddenEvents ()
        {
            Node node = ExecuteLambda (@"
event:test.foo11
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo12
  overrides:test.foo11
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo13
  overrides:test.foo11
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo11
  _out");
            Assert.IsTrue (node [3].Get<string> (_context) == "success2success1" || 
                           node [3].Get<string> (_context) == "success1success2");
        }
        
        //[Test]
        public void CreateMultipleEventsOneOverridden ()
        {
            Node node = ExecuteLambda (@"
event:test.foo14
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo15
  overrides:test.foo14
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo15
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo14
  _out");
            Assert.IsTrue (node [3].Get<string> (_context) == "success1success2" || 
                           node [3].Get<string> (_context) == "success2success1");
        }
        
        //[Test]
        public void OverrideEventThatIsOverridingEvent ()
        {
            Node node = ExecuteLambda (@"
event:test.foo16
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo17
  overrides:test.foo16
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo18
  overrides:test.foo17
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo16
  _out");
            Assert.AreEqual ("success", node [3].Value);
        }
        
        //[Test]
        public void InvokeEventOverriddenInChainWithTwoImplementations ()
        {
            Node node = ExecuteLambda (@"
event:test.foo19
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo20
  overrides:test.foo19
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo21
  overrides:test.foo20
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo21
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo19
  _out");
            Assert.IsTrue (node [4].Get<string> (_context) == "success1success2" || 
                           node [4].Get<string> (_context) == "success2success1");
        }
        
        //[Test]
        public void InvokeMiddleEventInChainOfOverridesWithMultipleImplementations ()
        {
            Node node = ExecuteLambda (@"
event:test.foo22
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo23
  overrides:test.foo22
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo24
  overrides:test.foo23
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo24
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo23
  _out");
            Assert.IsTrue (node [4].Get<string> (_context) == "success1success2" || 
                           node [4].Get<string> (_context) == "success2success1");
        }
        
        //[Test]
        public void CreateOneEventOverridingMultipleBaseEvents ()
        {
            Node node = ExecuteLambda (@"
event:test.foo25
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo26
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo27
  overrides
    :test.foo25
    :test.foo26
  set:@/./*/_out/#/?value
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
  _out");
            Assert.AreEqual ("success", node [3].Value);
            Assert.AreEqual ("success", node [6].Value);
        }
        
        //[Test]
        public void CallBaseFromOverridingEvent ()
        {
            Node node = ExecuteLambda (@"
event:test.foo28
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo29
  overrides:test.foo28
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo28
  _out");
            Assert.AreEqual ("failuresuccess", node [2].Value);
        }
        
        //[Test]
        public void CallBaseFromOverridingEventAfterExecutionOfSelf ()
        {
            Node node = ExecuteLambda (@"
event:test.bar1
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.bar2
  overrides:test.bar1
  set:@/./*/_out/#/?value
    :success
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.bar1
  _out");
            Assert.AreEqual ("successfailure", node [2].Value);
        }

        //[Test]
        public void CallBaseForEventsOverridingOverriddenEvents ()
        {
            Node node = ExecuteLambda (@"
event:test.foo30
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo31
  overrides:test.foo30
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
event:test.foo32
  overrides:test.foo31
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success3
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo30
  _out");
            Assert.AreEqual ("success1success2success3", node [3].Value);
        }
        
        //[Test]
        public void CallBaseForEventOverriddenMultipleTimes ()
        {
            Node node = ExecuteLambda (@"
event:test.foo33
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo34
  overrides:test.foo33
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
event:test.foo35
  overrides:test.foo33
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success3
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo33
  _out");
            Assert.IsTrue (node [3].Get<string> (_context) == "success1success2success1success3" || 
                           node [3].Get<string> (_context) == "success1success3success1success2");
        }
        
        //[Test]
        public void CallBaseForEventOverridingMultipleEvents ()
        {
            Node node = ExecuteLambda (@"
event:test.foo36
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo37
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
event:test.foo38
  overrides
    :test.foo36
    :test.foo37
  set:@/+/*/_out/?value
    :@/././*/_out/?value
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
  _out");
            Assert.AreEqual ("success1success3", node [3].Value);
            Assert.AreEqual ("success2success3", node [6].Value);
        }
        
        //[Test]
        public void CallBaseForEventNotOverriding ()
        {
            Node node = ExecuteLambda (@"
event:test.foo39
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo39
  _out");
            Assert.AreEqual ("success", node [1].Value);
        }
        
        //[Test]
        public void CallOverridingEventDirectlyWithCallBase ()
        {
            Node node = ExecuteLambda (@"
event:test.foo40
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo41
  overrides:test.foo40
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo41
  _out");
            Assert.AreEqual ("success", node [2].Value);
        }
        
        //[Test]
        public void DynamicallyOverrideEventAfterCreation ()
        {
            Node node = ExecuteLambda (@"
event:test.foo42
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo43
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
override:test.foo42
  with:test.foo43
set:@/+/*/_out/?value
  :@/./-/-/?node
test.foo42
  _out");
            Assert.AreEqual ("success", node [2].Value);
        }
        
        //[Test]
        public void DynamicallyOverrideEventAfterCreationMultipleTimes ()
        {
            Node node = ExecuteLambda (@"
event:test.foo44
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo45
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo46
  set:@/./*/_out/#/?value
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
  _out");
            Assert.IsTrue (node [3].Get<string> (_context) == "success1success2" || 
                           node [2].Get<string> (_context) == "success2success1");
        }

        //[Test]
        public void DynamicallyOverrideEventAfterCreationMultipleTimesCallBase ()
        {
            Node node = ExecuteLambda (@"
event:test.foo47
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo48
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo49
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
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
  _out");
            Assert.IsTrue (node [3].Get<string> (_context) == "failuresuccess1failuresuccess2" || 
                           node [2].Get<string> (_context) == "failuresuccess2failuresuccess1");
        }
        
        //[Test]
        public void DynamicallyOverrideEventAfterCreationMultipleOverridesCallBase ()
        {
            Node node = ExecuteLambda (@"
event:test.foo50
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo51
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
event:test.foo52
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
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
  _out");
            Assert.AreEqual ("success2success1success3", node [3].Value);
        }
        
        //[Test]
        public void DynamicallyOverrideEventMultipleTimesEventsCallBase ()
        {
            Node node = ExecuteLambda (@"
event:test.foo53
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo54
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo55
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
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
  _out");
            Assert.IsTrue (node [3].Get<string> (_context) == "failuresuccess1failuresuccess2" || 
                           node [2].Get<string> (_context) == "failuresuccess2failuresuccess1");
        }
        
        //[Test]
        public void DynamicallyOverrideMultipleEventsCallBase ()
        {
            Node node = ExecuteLambda (@"
event:test.foo56
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success1
event:test.foo57
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success2
event:test.foo58
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
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
  _out");
            Assert.AreEqual ("success1success3", node [3].Value);
            Assert.AreEqual ("success2success3", node [8].Value);
        }
        
        //[Test]
        public void DynamicallyOverrideEventCallDirectly ()
        {
            Node node = ExecuteLambda (@"
event:test.foo59
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
event:test.foo60
  set:@/+/*/_out/?value
    :@/././*/_out/?value
  call-base
    _out
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
override:test.foo59
  with:test.foo60
set:@/+/*/_out/?value
  :@/./-2/?node
test.foo60
  _out");
            Assert.AreEqual ("success", node [2].Value);
        }
        
        //[Test]
        public void CallEmptyBase ()
        {
            Node node = ExecuteLambda (@"
event:test.foo61
  bar
event:test.foo62
  overrides:test.foo61
  call-base
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
_out
set:@/+/*/_out/?value
  :@/./-/?node
test.foo61
  _out");
            Assert.AreEqual ("success", node [2].Value);
        }

        //[Test]
        public void DynamicallyDeleteOverride ()
        {
            Node node = ExecuteLambda (@"
event:test.foo63
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :success
event:test.foo64
  set:@/./*/_out/#/?value
    :{0}{1}
      :@/./././*/_out/#/?value
      :failure
_out
override:test.foo63
  with:test.foo64
delete-override:test.foo63
  :test.foo64
set:@/+/*/_out/?value
  :@/./-3/?node
test.foo63
  _out");
            Assert.AreEqual ("success", node [2].Value);
        }
    }
}
