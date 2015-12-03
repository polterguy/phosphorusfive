/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [add] lambda keyword
    /// </summary>
    [TestFixture]
    public class Add : TestBase
    {
        public Add ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp")
        { }

        /// <summary>
        ///     appends a static constant source node to destination
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
        ///     appends the results of an expression to destination
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
        
        [ActiveEvent (Name = "add.test1", Protection = EntranceProtection.Lambda)]
        private static void add_test1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("_foo1", "bar1");
            e.Args.Add ("_foo2", "bar2");
        }

        /// <summary>
        ///     appends the result of an Active Event returning nodes to destination
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
        
        [ActiveEvent (Name = "add.test2", Protection = EntranceProtection.Lambda)]
        private static void add_test2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = @"_foo1:bar1
_foo2:bar2";
        }

        /// <summary>
        ///     appends the result of an Active Event returning string to destination
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
        ///     appends a static constant string source to destination
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
        ///     appends a the results of an expression returning strings to destination
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
    }
}
