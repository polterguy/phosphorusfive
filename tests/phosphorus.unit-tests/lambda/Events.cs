/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Linq;
using NUnit.Framework;
using phosphorus.core;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace phosphorus.unittests.lambda
{
    [TestFixture]
    public class Events : TestBase
    {
        public Events ()
            : base ("phosphorus.hyperlisp", "phosphorus.lambda", "phosphorus.types", "phosphorus.meta") { }

        /// <summary>
        ///     creates a simple event, and invokes it, to verify events works as they should
        /// </summary>
        [Test]
        public void Events01 ()
        {
            var node = ExecuteLambda (@"event:test.foo1
  lambda
    set:@/././*/_out/?value
      source:success
test.foo1
  _out");
            Assert.AreEqual ("success", node [1] [0].Value);
        }

        /// <summary>
        ///     creates a simple event, and invokes it with an expression as root argument, consuming this expression
        ///     from inside the event as a referenced expression, to verify events can take parameters
        ///     as values when invoked
        /// </summary>
        [Test]
        public void Events02 ()
        {
            var node = ExecuteLambda (@"event:test.foo2
  lambda
    set:@@/././?value
      source:success
test.foo2:@/+/?value
_out");
            Assert.AreEqual ("success", node [2].Value);
        }

        /// <summary>
        ///     creates an event with multiple [lambda] objects, making sure events can take multiple lambda statements,
        ///     and execute them in order declared
        /// </summary>
        [Test]
        public void Events03 ()
        {
            var node = ExecuteLambda (@"remove-event:test.foo3
event:test.foo3
  lambda
    set:@/././*/_out/?value
      source:{0}{1}
        :succ
        :@/././././*/_out/?value
  lambda
    set:@/././*/_out/?value
      source:{0}{1}
        :@/././././*/_out/?value
        :ess
test.foo3
  _out");
            Assert.AreEqual ("success", node [2] [0].Value);
        }

        /// <summary>
        ///     creates an event with a [lambda.copy] statement as child, making sure lambda objects are
        ///     invoket correctly from events
        /// </summary>
        [Test]
        public void Events04 ()
        {
            var node = ExecuteLambda (@"event:test.foo4
  lambda.copy
    set:@/././*/_out/?value
      source:error
test.foo4
  _out:success");
            Assert.AreEqual ("success", node [1] [0].Value);
        }

        /// <summary>
        ///     creates an event in one Application Context, to invoke it in a different, making sure
        ///     events works the way they should
        /// </summary>
        [Test]
        public void Events05 ()
        {
            ExecuteLambda (@"event:test.foo5
  lambda
    set:@/././*/_out/?value
      source:success");

            // creating new Application Context
            Context = Loader.Instance.CreateApplicationContext ();
            var node = ExecuteLambda (@"test.foo5
  _out");
            Assert.AreEqual ("success", node [0] [0].Value);
        }

        /// <summary>
        ///     creates an event twice, to make sure both invocations are invoked, in order of creation
        /// </summary>
        [Test]
        public void Events06 ()
        {
            var node = ExecuteLambda (@"event:test.foo6
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
        ///     creates an event using a formatting expression, making sure events works as they should
        /// </summary>
        [Test]
        public void Events07 ()
        {
            ExecuteLambda (@"event:test.f{0}
  :oo7
  lambda
    set:@/././*/_out/?value
      source:success");
            var node = ExecuteLambda (@"test.foo7
  _out");
            Assert.AreEqual ("success", node [0] [0].Value);
        }

        /// <summary>
        ///     creates an event, for then to delete it, making sure deletion is successful
        /// </summary>
        [Test]
        public void Events08 ()
        {
            var node = ExecuteLambda (@"event:test.foo8
  lambda
    set:@/././*/_out/?value
      source:error
remove-event:test.foo8
test.foo8
  _out:success");
            Assert.AreEqual ("success", node [2] [0].Value);
        }

        /// <summary>
        ///     creates an event, for then to delete it, using a formatting expression,
        ///     making sure deletion is successful
        /// </summary>
        [Test]
        public void Events09 ()
        {
            var node = ExecuteLambda (@"event:test.foo9
  lambda
    set:@/././*/_out/?value
      source:error
remove-event:test.{0}
  :foo9
test.foo9
  _out:success");
            Assert.AreEqual ("success", node [2] [0].Value);
        }

        /// <summary>
        ///     creates two events, for then to delete them both, using expressions,
        ///     making sure deletion is successful
        /// </summary>
        [Test]
        public void Events10 ()
        {
            var node = ExecuteLambda (@"event:test.foo10
  lambda
    set:@/././*/_out/?value
      source:error
event:test.foo11
  lambda
    set:@/././*/_out/?value
      source:error
remove-event:@/../*/""/test/""/?name
test.foo10
  _out:success
test.foo11
  _out:success");
            Assert.AreEqual ("success", node [3] [0].Value);
            Assert.AreEqual ("success", node [4] [0].Value);
        }

        /// <summary>
        ///     creates two events with one [event] statement
        /// </summary>
        [Test]
        public void Events11 ()
        {
            var node = ExecuteLambda (@"_evts
  test.foo12
  test.foo13
event:@/-/*/?name
  lambda
    set:@/././*/_out/?value
      source:success
test.foo12
  _out
test.foo13
  _out");
            Assert.AreEqual ("success", node [2] [0].Value);
            Assert.AreEqual ("success", node [3] [0].Value);
        }

        [ActiveEvent (Name = "test.hardcoded")]
        private static void test_hardcoded (ApplicationContext context, ActiveEventArgs e) { e.Args.Value += "ess"; }

        /// <summary>
        ///     creates an Active Event that already exists as a C# Active Event, verifying both are called,
        ///     and that "dynamically created" event is invoked first
        /// </summary>
        [Test]
        public void Events12 ()
        {
            var node = ExecuteLambda (@"event:test.hardcoded
  lambda
    set:@/././?value
      source:succ
test.hardcoded");
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     creates an event that contains "persistent data" in a mutable data node, making sure
        ///     dynamically created Active Events can contain "mutable data segments"
        /// </summary>
        [Test]
        public void Events13 ()
        {
            var node = ExecuteLambda (@"event:test.foo14
  lambda
    set:@/././?value
      source:@/./+/#/?name
    _foo:node:
    set:@/-/#/?name
      source:success
test.foo14
test.foo14");
            Assert.AreEqual ("", node [1].Value);
            Assert.AreEqual ("success", node [2].Value);
        }

        [ActiveEvent (Name = "test.static.event-1")]
        [ActiveEvent (Name = "test.static.event-2")]
        private static void test_static_event_1 (ApplicationContext context, ActiveEventArgs e)
        { }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly when having no filter, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events14 ()
        {
            var node = ExecuteLambda (@"pf.meta.list-events");
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly when having a string filter, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events15 ()
        {
            var node = ExecuteLambda (@"pf.meta.list-events:test.static.event-");
            Assert.AreEqual (2, node [0].Count);
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly when having a filter being a 'value' expression, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events16 ()
        {
            var node = ExecuteLambda (@"_filter:test.static.event-
pf.meta.list-events:@/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly when having a filter being a 'value' expression,
        ///     and value is a list of reference node, which is supposed to be converted into string, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events17 ()
        {
            var node = ExecuteLambda (@"_filter:node:""test.static.event-""
pf.meta.list-events:@/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly when having a filter, which is a reference expression,
        ///     leading another expression, leading to multiple nodes, where each node's value is an exact match, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events18 ()
        {
            var node = ExecuteLambda (@"_filter:@/*?value
  :test.static.event-1
  :test.static.event-2
pf.meta.list-events:@@/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly when having a filter, which leads to nothing, 
        ///     and Active Event is supposed to return nothing
        /// </summary>
        [Test]
        public void Events19 ()
        {
            var node = ExecuteLambda (@"pf.meta.list-events:@/-?value");
            Assert.AreEqual (0, node [0].Count);
        }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly when having a filter, which is a reference expression,
        ///     leading another expression, leading to a single node, where that node's value is an exact match, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events20 ()
        {
            var node = ExecuteLambda (@"_filter:@/*?value
  :test.static.event-
pf.meta.list-events:@@/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly when having a filter, which is a reference expression,
        ///     leading another expression, leading to a single node, where that node's value is an exact match, and Active Event
        ///     is supposed to return one statically created Active Events
        /// </summary>
        [Test]
        public void Events21 ()
        {
            var node = ExecuteLambda (@"_filter:@/*?value
  :test.static.event-1
pf.meta.list-events:@@/-?value");
            Assert.AreEqual (1, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [pf.meta.list-event] works correctly and returns static events as [static] node names
        /// </summary>
        [Test]
        public void Events22 ()
        {
            var node = ExecuteLambda (@"pf.meta.list-events:test.static.event-1");
            Assert.AreEqual (1, node [0].Count);
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "static".Equals (idx.Name)) != null);
        }

        /// <summary>
        ///     creates an event that is overridden, making sure the overridden event is invoked,
        ///     and not the original event
        /// </summary>
        [Test]
        public void Override01 ()
        {
            var node = ExecuteLambda (@"remove-override:test.foo15
  super:test.foo16
remove-event:test.foo15
remove-event:test.foo16
event:test.foo15
  lambda
    set:@/../?value
      source:error
event:test.foo16
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :success
override:test.foo15
  super:test.foo16
test.foo15");
            Assert.AreEqual ("success", node.Value);
        }

        /// <summary>
        ///     creates an event that is overridden, and invoked on a different context,
        ///     making sure the overridden event is invoked, and not the original event
        /// </summary>
        [Test]
        public void Override02 ()
        {
            ExecuteLambda (@"remove-event:test.foo17
remove-event:test.foo18
remove-override:test.foo17
  super:test.foo18
event:test.foo17
  lambda
    set:@/../?value
      source:error
event:test.foo18
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :success
override:test.foo17
  super:test.foo18");

            // creating new application context, to make sure override is re-mapped on consecutive context objects
            Context = Loader.Instance.CreateApplicationContext ();
            var node = ExecuteLambda (@"test.foo17");
            Assert.AreEqual ("success", node.Value);
        }

        /// <summary>
        ///     creates an event that is overridden twice, making sure the overridden
        ///     events are invoked, and not the original event
        /// </summary>
        [Test]
        public void Override03 ()
        {
            var node = ExecuteLambda (@"remove-event:test.foo19
remove-event:test.foo20
remove-event:test.foo21
remove-override:test.foo19
  super:test.foo20
remove-override:test.foo19
  super:test.foo21
event:test.foo19
  lambda
    set:@/../?value
      source:error
event:test.foo20
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :succ
event:test.foo21
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :ess
override:test.foo19
  super:test.foo20
override:test.foo19
  super:test.foo21
test.foo19");
            Assert.AreEqual ("success", node.Value);
        }

        /// <summary>
        ///     creates an event that is overridden twice, using expressions, making sure the overridden
        ///     events are invoked, and not the original event
        /// </summary>
        [Test]
        public void Override04 ()
        {
            var node = ExecuteLambda (@"remove-event:test.foo22
remove-event:test.foo23
remove-event:test.foo24
remove-override:test.foo22
  super:@/../*/(/=test.foo23/[0,1]|/=test.foo24/[0,1])/?value
event:test.foo22
  lambda
    set:@/../?value
      source:error
event:test.foo23
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :succ
event:test.foo24
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :ess
override:test.foo22
  super:@/../*/(/=test.foo23/[0,1]|/=test.foo24/[0,1])/?value
test.foo22");
            Assert.AreEqual ("success", node.Value);
        }

        /// <summary>
        ///     creates two events that are both overridden, making sure the overridden
        ///     events are invoked, and not the original event
        /// </summary>
        [Test]
        public void Override05 ()
        {
            var node = ExecuteLambda (@"remove-event:test.foo25
remove-event:test.foo26
remove-event:test.foo27
remove-override:test.foo25
  super:test.foo27
remove-override:test.foo26
  super:test.foo27
event:test.foo25
  lambda
    set:@/../?value
      source:error
event:test.foo26
  lambda
    set:@/../?value
      source:error
event:test.foo27
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :success
override:test.foo25
  super:test.foo27
override:test.foo26
  super:test.foo27
test.foo25
test.foo26
");
            Assert.AreEqual ("successsuccess", node.Value);
        }

        /// <summary>
        ///     creates two events that are both overridden towards same super, with expressions,
        ///     making sure the overridden events are invoked, and not the original event
        /// </summary>
        [Test]
        public void Override06 ()
        {
            var node = ExecuteLambda (@"remove-event:test.foo28
remove-event:test.foo29
remove-event:test.foo30
remove-override:@/../*/(/=test.foo28/[0,1]|/=test.foo29/[0,1])?value
  super:test.foo30
event:test.foo28
  lambda
    set:@/../?value
      source:error
event:test.foo29
  lambda
    set:@/../?value
      source:error
event:test.foo30
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :success
override:@/../*/(/=test.foo28/[0,1]|/=test.foo29/[0,1])?value
  super:test.foo30
test.foo28
test.foo29
");
            Assert.AreEqual ("successsuccess", node.Value);
        }

        /// <summary>
        ///     creates two events, and overrides one with the other, for then to delete the override,
        ///     making sure the original event is invoked
        /// </summary>
        [Test]
        public void Override07 ()
        {
            var node = ExecuteLambda (@"remove-event:test.foo31
remove-event:test.foo32
remove-override:test.foo31
  super:test.foo32
event:test.foo31
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :success
event:test.foo32
  lambda
    set:@/../?value
      source:error
override:test.foo31
  super:test.foo32
remove-override:test.foo31
  super:test.foo32
test.foo31");
            Assert.AreEqual ("success", node.Value);
        }

        /// <summary>
        ///     creates one active event, which it overrides, for then to use [lambda.invoke] to directly invoke
        ///     base event, making sure override is not invoked
        /// </summary>
        [Test]
        public void Override08 ()
        {
            var node = ExecuteLambda (@"remove-event:test.foo33
remove-event:test.foo34
remove-override:test.foo33
  super:test.foo34
event:test.foo33
  lambda
    set:@/../?value
      source:{0}{1}
        :@/../?value
        :success
event:test.foo34
  lambda
    set:@/../?value
      source:error
override:test.foo33
  super:test.foo34
lambda.invoke
  test.foo33");
            Assert.AreEqual ("success", node.Value);
        }
    }
}