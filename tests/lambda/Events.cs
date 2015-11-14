/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using NUnit.Framework;
using p5.core;
using p5.exp;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     Class encapsulating unit tests for dynamically created Active Events
    /// </summary>
    [TestFixture]
    public class Events : TestBase
    {
        public Events ()
            : base ("p5.hyperlisp", "p5.lambda", "p5.types")
        { }

        /// <summary>
        ///     Creates a simple event, and retrieves it
        /// </summary>
        [Test]
        public void CreateAndGetEvent ()
        {
            var result = ExecuteLambda (@"set-event:events.test1
  foo:bar
insert-before:x:/../0
  get-events:events.test1");
            Assert.AreEqual (@"events.test1
  foo:bar", Utilities.Convert<string> (result [0], Context));
        }
        
        /// <summary>
        ///     Creates a simple event, and retrieves it using an expression
        /// </summary>
        [Test]
        public void CreateAndGetEventExpression ()
        {
            var result = ExecuteLambda (@"set-event:events.test2
  foo:bar
_evt:events.test2
insert-before:x:/../0
  get-events:x:/./-?value");
            Assert.AreEqual (@"events.test2
  foo:bar", Utilities.Convert<string> (result [0], Context));
        }
        
        /// <summary>
        ///     Creates two events, and retrieves them both using an expression
        /// </summary>
        [Test]
        public void CreateAndGetMultipleEventsExpression ()
        {
            var result = ExecuteLambda (@"set-event:events.test3
  foo1:bar1
set-event:events.test4
  foo2:bar2
_evt
  events.test3
  events.test4
insert-before:x:/../0
  get-events:x:/./-/*?name");
            string evts = Utilities.Convert<string> (result, Context);
            Assert.AreEqual (@"""""
  events.test3
    foo1:bar1
  events.test4
    foo2:bar2", evts);
        }
        
        /// <summary>
        ///     Creates two events, and retrieves them both using a formatted expression
        /// </summary>
        [Test]
        public void CreateAndGetMultipleEventsFormattedExpression ()
        {
            var result = ExecuteLambda (@"_val:-
set-event:events.test3
  foo1:bar1
set-event:events.test4
  foo2:bar2
_evt
  events.test3
  events.test4
insert-before:x:/../0
  get-events:x:/./{0}/*?name
    :x:/../*/_val?value");
            string evts = Utilities.Convert<string> (result, Context);
            Assert.AreEqual (@"""""
  events.test3
    foo1:bar1
  events.test4
    foo2:bar2", evts);
        }

        /// <summary>
        ///     Creates a simple event, deletes it, and tries to retrieve it
        /// </summary>
        [Test]
        public void CreateRemoveAndGetEvent ()
        {
            var result = ExecuteLambda (@"set-event:events.test5
  foo:bar
delete-events:events.test5
insert-before:x:/../0
  get-events:events.test5");
            Assert.AreEqual (0, result.Count);
        }

        /// <summary>
        ///     Creates a simple event, and invokes it, returning a string value
        /// </summary>
        [Test]
        public void CreateAndInvokeEventReturningStringValue ()
        {
            var result = ExecuteLambda (@"set-event:events.test5
  set:x:/..?value
    src:success
insert-before:x:/../0
  events.test5");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("success", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
        
        /// <summary>
        ///     Creates a simple event, and invokes it, returning a node value
        /// </summary>
        [Test]
        public void CreateAndInvokeEventReturningNodeValue ()
        {
            var result = ExecuteLambda (@"set-event:events.test6
  set:x:/..?value
    src:node:""success:foo-bar""
insert-before:x:/../0
  events.test6");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("success", result [0].Name);
            Assert.AreEqual ("foo-bar", result [0].Value);
        }

        /// <summary>
        ///     Creates a simple event, and invokes it, returning an integer value
        /// </summary>
        [Test]
        public void CreateAndInvokeEventReturningIntegerValue ()
        {
            var result = ExecuteLambda (@"set-event:events.test7
  set:x:/..?value
    src:int:5
events.test7
insert-before:x:/../0
  src:foo
set:x:/../0?value
  src:x:/./-2?value");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("foo", result [0].Name);
            Assert.AreEqual (5, result [0].Value);
        }
        
        /// <summary>
        ///     Creates a simple event, and invokes it, returning an expression value
        /// </summary>
        [Test]
        public void CreateAndInvokeEventReturningExpressionValue ()
        {
            var result = ExecuteLambda (@"set-event:events.test8
  set:x:/..?value
    src:x:/./+?value.x
  _x:/foo?value
events.test8
insert-before:x:/../0
  src:foo
set:x:/../0?value
  src:x:/./-2?value");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("foo", result [0].Name);
            Assert.IsTrue (result [0].Value is Expression);
            Assert.AreEqual ("/foo?value", result [0].Get<string> (Context).ToString ());
        }

        /// <summary>
        ///     Creates a simple event, and invokes it, returning nodes
        /// </summary>
        [Test]
        public void CreateAndInvokeEventReturningNodes ()
        {
            var result = ExecuteLambda (@"set-event:events.test9
  add:x:/..
    src
      _foo1:bar1
insert-before:x:/../0
  events.test9");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }

        /// <summary>
        ///     Creates a simple event, and invokes it, returning multiple nodes
        /// </summary>
        [Test]
        public void CreateAndInvokeEventReturningMultipleNodes ()
        {
            var result = ExecuteLambda (@"set-event:events.test10
  add:x:/..
    src
      _foo1:bar1
      _foo2:bar2
insert-before:x:/../0
  events.test10");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual (0, result [1].Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }

        /// <summary>
        ///     Creates a simple event, and invokes it with an argument as a node child
        /// </summary>
        [Test]
        public void CreateAndInvokeEventWithNodeArgument ()
        {
            var result = ExecuteLambda (@"set-event:events.test11
  set:x:/..?value
    src:x:/../*/_input?value
insert-before:x:/../0
  events.test11
    _input:success");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("success", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
        
        /// <summary>
        ///     Creates a simple event, and invokes it with multiple arguments as children nodes
        /// </summary>
        [Test]
        public void CreateAndInvokeEventWithMultipleNodeArguments ()
        {
            var result = ExecuteLambda (@"set-event:events.test12
  set:x:/..?value
    src:x:/../*(/_input1|/_input2)?value
insert-before:x:/../0
  events.test12
    _input1:succ
    _input2:ess");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("success", result [0].Name);
            Assert.IsNull (result [0].Value);
        }

        /// <summary>
        ///     Creates a simple event, and invokes it with an argument in value
        /// </summary>
        [Test]
        public void CreateAndInvokeEventWithArgumentInValue ()
        {
            var result = ExecuteLambda (@"set-event:events.test13
  add:x:/..
    src:x:/../*/_arg?value
insert-before:x:/../0
  events.test13:success");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("success", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
        
        /// <summary>
        ///     Creates a simple event, and invokes it with an argument in value, where argument is a formatted string
        /// </summary>
        [Test]
        public void CreateAndInvokeEventWithFormattedArgumentInValue ()
        {
            var result = ExecuteLambda (@"set-event:events.test13_2
  add:x:/..
    src:x:/..?value
  add:x:/..
    src:x:/../*/_arg?value
insert-before:x:/../0
  events.test13_2:succ{0}
    :ess");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("success", result [0].Name);
            Assert.IsNull (result [0].Value);
        }

        /// <summary>
        ///     Creates a simple event, and invokes it with an expression in value as argument, yielding
        ///     single node value
        /// </summary>
        [Test]
        public void CreateInvokeWithExpYieldingNode ()
        {
            var result = ExecuteLambda (@"set-event:events.test14
  insert-before:x:/../0
    src:x:/../*/_arg/*/_x/*
_x
  _foo1:bar1
  _foo2:bar2
insert-before:x:/../0
  events.test14:x:/./-");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual (0, result [1].Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Creates a simple event, and invokes it with an expression in value as argument, yielding
        ///     single string value
        /// </summary>
        [Test]
        public void CreateInvokeWithExpYieldingString ()
        {
            var result = ExecuteLambda (@"set-event:events.test14_3
  insert-before:x:/../0
    src:x:/../*/_arg?value
  set:x:/..?value
_x:_foo1
insert-before:x:/../0
  events.test14_3:x:/./-?value");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.IsNull (result [0].Value);
        }

        /// <summary>
        ///     Creates a simple event, and invokes it with a formatted expression in value as argument, yielding
        ///     single node value
        /// </summary>
        [Test]
        public void CreateInvokeWithFormExpYieldingNode ()
        {
            var result = ExecuteLambda (@"set-event:events.test14_2
  insert-before:x:/../0
    src:x:/../*/_arg/*/_x/*
_x
  _foo1:bar1
  _foo2:bar2
insert-before:x:/../0
  events.test14_2:x:/{0}/-
    :.");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual (0, result [1].Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }

        /// <summary>
        ///     Creates a simple event, and invokes it with an expression in value as argument, yielding
        ///     multiple node values
        /// </summary>
        [Test]
        public void CreateInvokeWithExpArgYieldingManyNodes ()
        {
            var result = ExecuteLambda (@"set-event:events.test15
  insert-before:x:/../0
    src:x:/../*/_arg/*/_x/*
_x
  _foo1:bar1
  _foo2:bar2
_x
  _foo3:bar3
  _foo4:bar4
insert-before:x:/../0
  events.test15:x:/./-2|/./-");
            Assert.AreEqual (4, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual (0, result [1].Count);
            Assert.AreEqual (0, result [2].Count);
            Assert.AreEqual (0, result [3].Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
            Assert.AreEqual ("_foo3", result [2].Name);
            Assert.AreEqual ("bar3", result [2].Value);
            Assert.AreEqual ("_foo4", result [3].Name);
            Assert.AreEqual ("bar4", result [3].Value);
        }
        
        /// <summary>
        ///     Creates a simple event, and invokes it with an expression in value as argument, yielding
        ///     multiple string values
        /// </summary>
        [Test]
        public void CreateInvokeWithExpInValueYieldingManyStrings ()
        {
            var result = ExecuteLambda (@"set-event:events.test16
  insert-before:x:/../0
    src:x:/../*/_arg?value
_x
  _foo1:bar1
  _foo2:bar2
_x
  _foo3:bar3
  _foo4:bar4
insert-before:x:/../0
  events.test16:x:/./-2/*|/./-/*?name");
            Assert.AreEqual (4, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual (0, result [1].Count);
            Assert.AreEqual (0, result [2].Count);
            Assert.AreEqual (0, result [3].Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("_foo3", result [2].Name);
            Assert.AreEqual ("_foo4", result [3].Name);
            Assert.IsNull (result [0].Value);
            Assert.IsNull (result [1].Value);
            Assert.IsNull (result [2].Value);
            Assert.IsNull (result [3].Value);
        }
    }
}
