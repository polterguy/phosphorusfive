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
        ///     creates a simple event, and retrieves it
        /// </summary>
        [Test]
        public void CreateAndGetEvent ()
        {
            var result = ExecuteLambda (@"
set-event:events.test1
  foo:bar
insert-before:x:/../0
  get-events:events.test1");
            Assert.AreEqual (@"events.test1
  foo:bar", Utilities.Convert<string> (result [0], Context));
        }
        
        /// <summary>
        ///     creates a simple event, deletes it, and try to retrieve it
        /// </summary>
        [Test]
        public void CreateRemoveAndGetEvent ()
        {
            var result = ExecuteLambda (@"set-event:events.test1
  foo:bar
delete-events:events.test1
insert-before:x:/../0
  get-events:events.test1");
            Assert.AreEqual (0, result.Count);
        }

        /// <summary>
        ///     creates a simple event, and invokes it, returning a value
        /// </summary>
        [Test]
        public void CreateAndInvokeEventReturningValue ()
        {
            var result = ExecuteLambda (@"
set-event:events.test1
  set:x:/..?value
    src:success
insert-before:x:/../0
  events.test1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("success", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
        
        /// <summary>
        ///     creates a simple event, and invokes it, returning nodes
        /// </summary>
        [Test]
        public void CreateAndInvokeEventReturningNodes ()
        {
            var result = ExecuteLambda (@"
set-event:events.test1
  add:x:/..
    src
      _foo1:bar1
insert-before:x:/../0
  events.test1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }

        /// <summary>
        ///     creates a simple event, and invokes it with an argument
        /// </summary>
        [Test]
        public void CreateAndInvokeEventWithArgument ()
        {
            var result = ExecuteLambda (@"
set-event:events.test1
  set:x:/..?value
    src:x:/../*/_input?value
insert-before:x:/../0
  events.test1
    _input:success");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual (0, result [0].Count);
            Assert.AreEqual ("success", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
    }
}
