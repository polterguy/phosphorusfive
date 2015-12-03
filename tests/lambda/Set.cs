/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [set] lambda keyword
    /// </summary>
    [TestFixture]
    public class Set : TestBase
    {
        public Set ()
            : base ("p5.lambda", "p5.hyperlisp", "p5.types", "p5.math") { }

        /// <summary>
        ///     verifies [set] works with a static constant source
        /// </summary>
        [Test]
        public void SetStaticSource ()
        {
            var result = ExecuteLambda (@"_x
  _foo1
set:x:/-/0?value
  src:success
add:x:/..
  src:x:/./-2/*");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("success", result [0].Value);
        }
        
        /// <summary>
        ///     verifies [set] works with an expression source
        /// </summary>
        [Test]
        public void SetExpressionSource ()
        {
            var result = ExecuteLambda (@"_x
  _dest
  _source:success
set:x:/-/0?value
  src:x:/./-/1?value
add:x:/..
  src:x:/./-2/0");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual ("success", result [0].Value);
        }
        
        [ActiveEvent (Name = "set.test1", Protection = EventProtection.Lambda)]
        private static void set_test1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }
        
        /// <summary>
        ///     verifies [set] works with an Active Event source
        /// </summary>
        [Test]
        public void SetActiveEventSource ()
        {
            var result = ExecuteLambda (@"_dest
set:x:/-?value
  set.test1
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual ("success", result [0].Value);
        }
        
        [ActiveEvent (Name = "set.test2", Protection = EventProtection.Lambda)]
        private static void set_test2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add (new Node ("_foo1", "bar1"));
            e.Args.Add (new Node ("_foo2", "bar2"));
        }

        /// <summary>
        ///     verifies [set] works with an Active Event source that returns nodes
        /// </summary>
        [Test]
        public void SetActiveEventSourceReturningNodes ()
        {
            var result = ExecuteLambda (@"_dest
set:x:/-?value
  set.test2
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual ("_foo1:bar1\r\n_foo2:bar2", result [0].Value);
        }
        
        [ActiveEvent (Name = "set.test3", Protection = EventProtection.Lambda)]
        private static void set_test3 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add (new Node ("_foo1", "bar1"));
            e.Args [0].Add ("_foo2", "bar2");
        }

        /// <summary>
        ///     verifies [set] works with an Active Event source that returns a single node
        /// </summary>
        [Test]
        public void SetActiveEventSourceReturningSingleNode ()
        {
            var result = ExecuteLambda (@"_dest
set:x:/-?value
  set.test3
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.IsTrue (result [0].Value is Node);
            Assert.AreEqual ("_foo1", result [0].Get<Node> (Context).Name);
            Assert.AreEqual ("bar1", result [0].Get<Node> (Context).Value);
            Assert.AreEqual ("_foo2", result [0].Get<Node> (Context) [0].Name);
            Assert.AreEqual ("bar2", result [0].Get<Node> (Context) [0].Value);
        }
        
        /// <summary>
        ///     verifies [set] works with src being math results
        /// </summary>
        [Test]
        public void SetMathResult ()
        {
            var result = ExecuteLambda (@"_dest
set:x:/-?value
  +:int:5
    _:5
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual (10, result [0].Value);
        }
        
        /// <summary>
        ///     verifies [set] works with src being math results containing expressions
        /// </summary>
        [Test]
        public void SetMathResultContainingExpressions ()
        {
            var result = ExecuteLambda (@"_src:5
_dest
set:x:/-?value
  +:x:/../0?value.int
    _:5
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual (10, result [0].Value);
        }
    }
}
