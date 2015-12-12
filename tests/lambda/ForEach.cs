/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.exp;
using p5.core;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     Unit tests for testing the [for-each] lambda keyword
    /// </summary>
    [TestFixture]
    public class ForEach : TestBase
    {
        public ForEach ()
        : base ("p5.lambda", "p5.types", "p5.hyperlisp") { }

        /// <summary>
        ///     Verifies that [for-each] works when expression is of type 'node'
        /// </summary>
        [Test]
        public void ForEachNode ()
        {
            var result = ExecuteLambda (@"_data
  _foo1:bar1
  _foo2:bar2
for-each:x:/-/*
  add:x:/..
    src:x:/././*/__dp/#");
            Assert.AreEqual (2, result.Children.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Verifies that [for-each] works when expression is of type 'name'
        /// </summary>
        [Test]
        public void ForEachName ()
        {
            var result = ExecuteLambda (@"_data
  _foo1:bar1
  _foo2:bar2
for-each:x:/-/*?name
  add:x:/..
    src:x:/././*/__dp?value");
            Assert.AreEqual (2, result.Children.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.IsNull (result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.IsNull (result [1].Value);
        }
        
        /// <summary>
        ///     Verifies that [for-each] works when expression is of type 'value'
        /// </summary>
        [Test]
        public void ForEachValue ()
        {
            var result = ExecuteLambda (@"_data
  _foo1:bar1
  _foo2:bar2
for-each:x:/-/*?value
  add:x:/..
    src:x:/././*/__dp?value");
            Assert.AreEqual (2, result.Children.Count);
            Assert.AreEqual ("bar1", result [0].Name);
            Assert.IsNull (result [0].Value);
            Assert.AreEqual ("bar2", result [1].Name);
            Assert.IsNull (result [1].Value);
        }
        
        /// <summary>
        ///     Verifies that [for-each] works when given a constant node with two children
        /// </summary>
        [Test]
        public void ForEachValueIsNode ()
        {
            var result = ExecuteLambda (@"for-each:node:@""_data
  _foo1:bar1
  _foo2:bar2""
  add:x:/..
    src:x:/././*/__dp/#");
            Assert.AreEqual (2, result.Children.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Verifies that [for-each] works when given a constant string that turns into two nodes
        /// </summary>
        [Test]
        public void ForEachValueIsString ()
        {
            var result = ExecuteLambda (@"for-each:@""_foo1:bar1
_foo2:bar2""
  add:x:/..
    src:x:/././*/__dp/#");
            Assert.AreEqual (2, result.Children.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        [ActiveEvent (Name = "for-each.test1", Protection = EventProtection.LambdaClosed)]
        private static void for_each_test1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("_foo1", "bar1");
            e.Args.Add ("_foo2", "bar2");
        }

        /// <summary>
        ///     Verifies that [for-each] works when given a source that is an Active Event invocation
        /// </summary>
        [Test]
        public void ForEachSourceIsActiveEvent ()
        {
            var result = ExecuteLambda (@"for-each
  for-each.test1
  add:x:/..
    src:x:/././*/__dp/#");
            Assert.AreEqual (2, result.Children.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Verifies that [for-each] works when given a source that is a dynamically created Active Event invocation
        /// </summary>
        [Test]
        public void ForEachSourceIsDynamicActiveEvent ()
        {
            var result = ExecuteLambda (@"set-event:for-each.test2
  add:x:/..
    src
      _foo1:bar1
      _foo2:bar2
for-each
  for-each.test2
  add:x:/..
    src:x:/././*/__dp/#");
            Assert.AreEqual (2, result.Children.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Verifies that [for-each] works when given a source that is a dynamically created Active Event invocation
        ///     that returns a string as value
        /// </summary>
        [Test]
        public void ForEachSourceIsActiveEventReturningString ()
        {
            var result = ExecuteLambda (@"set-event:for-each.test3
  set:x:/..?value
    src:@""_foo1:bar1
_foo2:bar2""
for-each
  for-each.test3
  add:x:/..
    src:x:/././*/__dp?value");
            Assert.AreEqual (2, result.Children.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Verifies that [for-each] has access to nodes outside of itself
        /// </summary>
        [Test]
        public void ForEachIsNotRoot ()
        {
            var result = ExecuteLambda (@"_data
  foo1
  foo2
for-each:x:/-/*?name
  set:x:/./-?value
    src:{0}{1}
      :x:/../0?value
      :x:/..for-each/*/__dp?value
add:x:/..
  src:x:/../0?value");
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("foo1foo2", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
    }
}
