/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using NUnit.Framework;
using p5.core;

namespace p5.unittests.lambda
{
    [TestFixture]
    public class Events : TestBase
    {
        public Events ()
            : base ("p5.hyperlisp", "p5.lambda", "p5.types")
        { }

        /// <summary>
        ///     creates a simple event, and invokes it, to verify events works as they should
        /// </summary>
        [Test]
        public void Events01 ()
        {
            var node = ExecuteLambda (@"set-event:test.foo1
  set:x:/./*/_out?value
    src:success
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
            var node = ExecuteLambda (@"set-event:test.foo2
  set:x:@/.?value
    src:success
test.foo2:x:/+?value
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
set-event:test.foo3
  lambda
    set:x:/././*/_out?value
      src:{0}{1}
        :succ
        :x:/././././*/_out?value
  lambda
    set:x:/././*/_out?value
      src:{0}{1}
        :x:/././././*/_out?value
        :ess
test.foo3
  _out");
            Assert.AreEqual ("success", node [2] [0].Value);
        }

        /// <summary>
        ///     creates an event in one Application Context, to invoke it in a different, making sure
        ///     events works the way they should
        /// </summary>
        [Test]
        public void Events05 ()
        {
            ExecuteLambda (@"set-event:test.foo5
  set:x:/./*/_out?value
    src:success");

            // creating new Application Context
            Context = Loader.Instance.CreateApplicationContext ();
            var node = ExecuteLambda (@"test.foo5
  _out");
            Assert.AreEqual ("success", node [0] [0].Value);
        }

        /// <summary>
        ///     creates an event using a formatting expression, making sure events works as they should
        /// </summary>
        [Test]
        public void Events07 ()
        {
            ExecuteLambda (@"set-event:test.f{0}
  :oo7
  set:x:/./*/_out?value
    src:success");
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
            var node = ExecuteLambda (@"set-event:test.foo8
  set:@/./*/_out/?value
    src:error
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
            var node = ExecuteLambda (@"set-event:test.foo9
  set:@/./*/_out/?value
    src:error
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
            var node = ExecuteLambda (@"set-event:test.foo10
  set:x:/./*/_out?value
    src:error
set-event:test.foo11
  set:x:/./*/_out?value
    src:error
remove-event:x:/../*(/test.foo10|test.foo11)?name
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
set-event:x:/-/*?name
  set:x:/./*/_out?value
    src:success
test.foo12
  _out
test.foo13
  _out");
            Assert.AreEqual ("success", node [2] [0].Value);
            Assert.AreEqual ("success", node [3] [0].Value);
        }

        [ActiveEvent (Name = "test.hardcoded")]
        private static void test_hardcoded (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value += "succ";
        }

        /// <summary>
        ///     creates an Active Event that already exists as a C# Active Event, verifying both are called,
        ///     and that "dynamically created" event is invoked last
        /// </summary>
        [Test]
        public void Events12 ()
        {
            var node = ExecuteLambda (@"set-event:test.hardcoded
  set:x:/.?value
    src:{0}ess
      :x:/././.?value
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
            var node = ExecuteLambda (@"set-event:test.foo14
  set:x:/.?value
    src:x:/./+/#?name
  _foo:node:
  set:x:/-/#?name
    src:success
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
        ///     verifies that [p5.meta.list-event] works correctly when having no filter, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events14 ()
        {
            var node = ExecuteLambda (@"list-events");
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [p5.meta.list-event] works correctly when having a string filter, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events15 ()
        {
            var node = ExecuteLambda (@"list-events:test.static.event-");
            Assert.AreEqual (2, node [0].Count);
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [p5.meta.list-event] works correctly when having a filter being a 'value' expression, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events16 ()
        {
            var node = ExecuteLambda (@"_filter:test.static.event-
list-events:x:/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [p5.meta.list-event] works correctly when having a filter being a 'value' expression,
        ///     and value is a list of reference node, which is supposed to be converted into string, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events17 ()
        {
            var node = ExecuteLambda (@"_filter:node:""test.static.event-""
list-events:x:/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [p5.meta.list-event] works correctly when having a filter, which is a reference expression,
        ///     leading another expression, leading to multiple nodes, where each node's value is an exact match, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events18 ()
        {
            var node = ExecuteLambda (@"_filter:x:/*?value
  :test.static.event-1
  :test.static.event-2
list-events:x:@/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [p5.meta.list-event] works correctly when having a filter, which leads to nothing, 
        ///     and Active Event is supposed to return nothing
        /// </summary>
        [Test]
        public void Events19 ()
        {
            var node = ExecuteLambda (@"list-events:@/-?value");
            Assert.AreEqual (0, node [0].Count);
        }

        /// <summary>
        ///     verifies that [p5.meta.list-event] works correctly when having a filter, which is a reference expression,
        ///     leading another expression, leading to a single node, where that node's value is an exact match, and Active Event
        ///     is supposed to return two statically created Active Events
        /// </summary>
        [Test]
        public void Events20 ()
        {
            var node = ExecuteLambda (@"_filter:x:/*?value
  :test.static.event-
list-events:x:@/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-2".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [p5.meta.list-event] works correctly when having a filter, which is a reference expression,
        ///     leading another expression, leading to a single node, where that node's value is an exact match, and Active Event
        ///     is supposed to return one statically created Active Events
        /// </summary>
        [Test]
        public void Events21 ()
        {
            var node = ExecuteLambda (@"_filter:x:/*?value
  :test.static.event-1
list-events:x:@/-?value");
            Assert.AreEqual (1, node [1].Count);
            Assert.IsTrue (node [1].Children.SingleOrDefault (idx => "test.static.event-1".Equals (idx.Value)) != null);
        }

        /// <summary>
        ///     verifies that [p5.meta.list-event] works correctly and returns static events as [static] node names
        /// </summary>
        [Test]
        public void Events22 ()
        {
            var node = ExecuteLambda (@"list-events:test.static.event-1");
            Assert.AreEqual (1, node [0].Count);
            Assert.IsTrue (node [0].Children.SingleOrDefault (idx => "static".Equals (idx.Name)) != null);
        }

        /// <summary>
        ///     creates an event that returns a new node by using [add], verifying events work as they should
        /// </summary>
        [Test]
        public void Events23 ()
        {
            var node = ExecuteLambda (@"set-event:test.foo15
  add:x:/.?node
    src
      _result:success
test.foo15");
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual ("_result", node [1] [0].Name);
            Assert.AreEqual ("success", node [1] [0].Value);
        }
    }
}