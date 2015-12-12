/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     Unit tests for testing the [retrieve] lambda keyword
    /// </summary>
    [TestFixture]
    public class Retrieve : TestBase
    {
        public Retrieve ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp")
        { }

        /// <summary>
        ///     Retrieves one static source
        /// </summary>
        [Test]
        public void RetrieveStaticSource ()
        {
            var result = ExecuteLambda (@"_result
add:x:/-
  retrieve
    src:x:/+/*
    _tmp
      _foo:bar", "eval-mutable");
            Assert.AreEqual ("_foo", result [0] [0].Name);
            Assert.AreEqual ("bar", result [0] [0].Value);
            Assert.AreEqual (0, result [0] [0].Children.Count);
        }

        /// <summary>
        ///     Retrieves one static source
        /// </summary>
        [Test]
        public void RetrieveStaticSourceYieldingMultiple ()
        {
            var result = ExecuteLambda (@"_result
add:x:/-
  retrieve
    src:x:/+/*
    _tmp
      _foo1:bar1
      _foo2:bar2", "eval-mutable");
            Assert.AreEqual (2, result [0].Children.Count);
            Assert.AreEqual ("_foo1", result [0] [0].Name);
            Assert.AreEqual ("bar1", result [0] [0].Value);
            Assert.AreEqual (0, result [0] [0].Children.Count);
            Assert.AreEqual ("_foo2", result [0] [1].Name);
            Assert.AreEqual ("bar2", result [0] [1].Value);
            Assert.AreEqual (0, result [0] [1].Children.Count);
        }

        /// <summary>
        ///     Retrieves two static sources
        /// </summary>
        [Test]
        public void RetrieveTwoStaticSources ()
        {
            var result = ExecuteLambda (@"_result
add:x:/-
  retrieve
    src:x:/+/*
    _tmp
      _foo1:bar1
    src:x:/+/*
    _tmp
      _foo2:bar2", "eval-mutable");
            Assert.AreEqual (2, result [0].Children.Count);
            Assert.AreEqual ("_foo1", result [0] [0].Name);
            Assert.AreEqual ("bar1", result [0] [0].Value);
            Assert.AreEqual (0, result [0] [0].Children.Count);
            Assert.AreEqual ("_foo2", result [0] [1].Name);
            Assert.AreEqual ("bar2", result [0] [1].Value);
            Assert.AreEqual (0, result [0] [1].Children.Count);
        }

        [ActiveEvent (Name = "retrieve.test1", Protection = EventProtection.LambdaClosedNativeOpen)]
        private static void retrieve_test1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("_ret");
            e.Args.LastChild.Add ("_foo1", "bar1");
            e.Args.LastChild.Add ("_foo2", "bar2");
        }

        /// <summary>
        ///     Retrieves results from Active Event invocation
        /// </summary>
        [Test]
        public void RetrieveFromEvent ()
        {
            var result = ExecuteLambda (@"_result
add:x:/-
  retrieve
    src:x:/+/*/*
    retrieve.test1", "eval-mutable");
            Assert.AreEqual (2, result [0].Children.Count);
            Assert.AreEqual ("_foo1", result [0] [0].Name);
            Assert.AreEqual ("bar1", result [0] [0].Value);
            Assert.AreEqual (0, result [0] [0].Children.Count);
            Assert.AreEqual ("_foo2", result [0] [1].Name);
            Assert.AreEqual ("bar2", result [0] [1].Value);
            Assert.AreEqual (0, result [0] [1].Children.Count);
        }
    }
}
