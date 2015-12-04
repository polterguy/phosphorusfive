/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     Unit tests for testing the [add] lambda keyword
    /// </summary>
    [TestFixture]
    public class Add : TestBase
    {
        public Add ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp")
        { }

        /// <summary>
        ///     Appends a static constant source with two nodes to destination
        /// </summary>
        [Test]
        public void AddStaticSource ()
        {
            var result = ExecuteLambda (@"add:x:/..
  src
    _foo1:bar1
    _foo2:bar2");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Appends the results of an expression yielding two nodes to destination
        /// </summary>
        [Test]
        public void AddExpressionSource ()
        {
            var result = ExecuteLambda (@"_foos
  _foo1:bar1
  _foo2:bar2
add:x:/..
  src:x:/./-/*");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        [ActiveEvent (Name = "add.test1", Protection = EventProtection.LambdaClosed)]
        private static void add_test1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("_foo1", "bar1");
            e.Args.Add ("_foo2", "bar2");
        }

        /// <summary>
        ///     Appends the result of an Active Event returning two nodes to destination
        /// </summary>
        [Test]
        public void AddActiveEventSource ()
        {
            var result = ExecuteLambda (@"add:x:/..
  add.test1");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        [ActiveEvent (Name = "add.test2", Protection = EventProtection.LambdaClosed)]
        private static void add_test2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = @"_foo1:bar1
_foo2:bar2";
        }

        /// <summary>
        ///     Appends the result of an Active Event returning string as value to destination
        /// </summary>
        [Test]
        public void AddActiveEventStringSource ()
        {
            var result = ExecuteLambda (@"add:x:/..
  add.test2");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Appends a static constant string source to destination
        /// </summary>
        [Test]
        public void AddStaticStringSource ()
        {
            var result = ExecuteLambda (@"add:x:/..
  src:@""_foo1:bar1
_foo2:bar2""");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }
        
        /// <summary>
        ///     Appends a the results of an expression returning strings to destination
        /// </summary>
        [Test]
        public void AddExpressionReturningStringSource ()
        {
            var result = ExecuteLambda (@"_foos:@""_foo1:bar1
_foo2:bar2""
add:x:/..
  src:x:/./-?value");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }

        /// <summary>
        ///     Appends a the results of an expression returning node from value to destination
        /// </summary>
        [Test]
        public void AddExpressionReturningNodeFromValue ()
        {
            var result = ExecuteLambda (@"_foos:node:@""_wrapper
  _foo1:bar1
  _foo2:bar2""
add:x:/..
  src:x:/./-?value");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_wrapper", result [0].Name);
            Assert.AreEqual ("_foo1", result [0] [0].Name);
            Assert.AreEqual ("bar1", result [0] [0].Value);
            Assert.AreEqual ("_foo2", result [0][1].Name);
            Assert.AreEqual ("bar2", result [0][1].Value);
        }

        /// <summary>
        ///     Appends the results of an expression returning reference node's children from value to destination
        /// </summary>
        [Test]
        public void AddExpressionReturningReferenceNodeFromValue ()
        {
            var result = ExecuteLambda (@"_foos:node:@""_wrapper
  _foo1:bar1
  _foo2:bar2""
add:x:/..
  src:x:/./-/#/*");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }

        /// <summary>
        ///     Appends two [src] nodes with static children into destination
        /// </summary>
        [Test]
        public void AddWithMultipleStaticSources ()
        {
            var result = ExecuteLambda (@"add:x:/..
  src
    _foo1:bar1
  src
    _foo2:bar2");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }

        /// <summary>
        ///     Appends two [src] nodes with expressions in both
        /// </summary>
        [Test]
        public void AddWithMultipleExpressionSources ()
        {
            var result = ExecuteLambda (@"_x1
  _foo1:bar1
_x2
  _foo2:bar2
add:x:/..
  src:x:/../*/_x1/*
  src:x:/../*/_x2/*");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }

        /// <summary>
        ///     Appends two [src] nodes where one has a static source and the other has an expression source
        /// </summary>
        [Test]
        public void AddWithMultipleMixedSources ()
        {
            var result = ExecuteLambda (@"_x1
  _foo1:bar1
add:x:/..
  src:x:/../*/_x1/*
  src
    _foo2:bar2");
            Assert.AreEqual (2, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
            Assert.AreEqual ("_foo2", result [1].Name);
            Assert.AreEqual ("bar2", result [1].Value);
        }

        /// <summary>
        ///     Verifies mixing [src] and [rel-src] in same [add] operation throws an exception
        /// </summary>
        [Test]
        [ExpectedException(typeof(p5.exp.exceptions.LambdaException))]
        public void AddWithRelSrcAndSrcThrows ()
        {
            ExecuteLambda (@"add:x:/..
  src
    x
  rel-src:x:/..");
        }
    }
}
