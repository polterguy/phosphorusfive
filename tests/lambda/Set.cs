/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;
using p5.exp.exceptions;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     Unit tests for testing the [set] lambda keyword
    /// </summary>
    [TestFixture]
    public class Set : TestBase
    {
        public Set ()
            : base ("p5.lambda", "p5.hyperlisp", "p5.types", "p5.math") { }

        /// <summary>
        ///     Verifies [set] works with a static constant source
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
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("success", result [0].Value);
        }
        
        /// <summary>
        ///     Verifies [set] works with an expression source
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
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual ("success", result [0].Value);
        }
        
        [ActiveEvent (Name = "set.test1", Protection = EventProtection.LambdaClosed)]
        private static void set_test1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }
        
        /// <summary>
        ///     Verifies [set] works with an Active Event source
        /// </summary>
        [Test]
        public void SetActiveEventSource ()
        {
            var result = ExecuteLambda (@"_dest
set:x:/-?value
  set.test1
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual ("success", result [0].Value);
        }
        
        [ActiveEvent (Name = "set.test2", Protection = EventProtection.LambdaClosed)]
        private static void set_test2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add (new Node ("_foo1", "bar1"));
            e.Args.Add (new Node ("_foo2", "bar2"));
        }

        /// <summary>
        ///     Verifies [set] works with an Active Event source that returns nodes
        /// </summary>
        [Test]
        public void SetActiveEventSourceReturningNodes ()
        {
            var result = ExecuteLambda (@"_dest
set:x:/-?value
  set.test2
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual ("", result[0].Get<Node> (Context).Name);
            Assert.IsNull (result[0].Get<Node> (Context).Value);
            Assert.AreEqual ("_foo1", result[0].Get<Node> (Context)[0].Name);
            Assert.AreEqual ("bar1", result[0].Get<Node> (Context)[0].Value);
            Assert.AreEqual ("_foo2", result[0].Get<Node> (Context)[1].Name);
            Assert.AreEqual ("bar2", result[0].Get<Node> (Context)[1].Value);
        }
        
        [ActiveEvent (Name = "set.test3", Protection = EventProtection.LambdaClosed)]
        private static void set_test3 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add (new Node ("_foo1", "bar1"));
            e.Args [0].Add ("_foo2", "bar2");
        }

        /// <summary>
        ///     Verifies [set] works with an Active Event source that returns a single node
        /// </summary>
        [Test]
        public void SetActiveEventSourceReturningSingleNode ()
        {
            var result = ExecuteLambda (@"_dest
set:x:/-?value
  set.test3
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.IsTrue (result [0].Value is Node);
            Assert.AreEqual ("_foo1", result [0].Get<Node> (Context).Name);
            Assert.AreEqual ("bar1", result [0].Get<Node> (Context).Value);
            Assert.AreEqual ("_foo2", result [0].Get<Node> (Context) [0].Name);
            Assert.AreEqual ("bar2", result [0].Get<Node> (Context) [0].Value);
        }
        
        /// <summary>
        ///     Verifies [set] works with [src] being math results
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
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual (10, result [0].Value);
        }
        
        /// <summary>
        ///     Verifies [set] works with src being math results containing expressions
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
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.AreEqual (10, result [0].Value);
        }

        [ActiveEvent (Name = "set.test4", Protection = EventProtection.LambdaClosed)]
        private static void set_test4 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "succ";
        }

        [ActiveEvent (Name = "set.test5", Protection = EventProtection.LambdaClosed)]
        private static void set_test5 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "ess";
        }

        /// <summary>
        ///     Verifies [set] works with multiple active event invocations as source
        /// </summary>
        [Test]
        public void SetSrcIsTwoActiveEvents ()
        {
            var result = ExecuteLambda (@"set:x:/..?value
  set.test4
  set.test5");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     Verifies [set] works with two Active Event source invocations that returns a single node each
        /// </summary>
        [Test]
        public void SetTwoActiveEventSourceInvocationsReturningSingleNodeEach ()
        {
            var result = ExecuteLambda (@"_dest
set:x:/-?value
  set.test3
  set.test3
add:x:/..
  src:x:/./-2");
            Assert.AreEqual (1, result.Children.Count);
            Assert.AreEqual ("_dest", result [0].Name);
            Assert.IsTrue (result [0].Value is Node);
            Assert.AreEqual ("", result [0].Get<Node> (Context).Name);
            Assert.IsNull (result [0].Get<Node> (Context).Value);
            Assert.AreEqual (2, result [0].Get<Node> (Context).Children.Count);

            Assert.AreEqual ("_foo1", result [0].Get<Node> (Context)[0].Name);
            Assert.AreEqual ("bar1", result [0].Get<Node> (Context)[0].Value);
            Assert.AreEqual ("_foo2", result [0].Get<Node> (Context)[0] [0].Name);
            Assert.AreEqual ("bar2", result [0].Get<Node> (Context)[0] [0].Value);
            Assert.AreEqual ("_foo1", result [0].Get<Node> (Context)[1].Name);
            Assert.AreEqual ("bar1", result [0].Get<Node> (Context)[1].Value);
            Assert.AreEqual ("_foo2", result [0].Get<Node> (Context)[1] [0].Name);
            Assert.AreEqual ("bar2", result [0].Get<Node> (Context)[1] [0].Value);
        }

        /// <summary>
        ///     Verifies [set] works with multiple [src] children
        /// </summary>
        [Test]
        public void SetTwoStaticSrc ()
        {
            var result = ExecuteLambda (@"set:x:/..?value
  src:succ
  src:ess");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     Verifies [set] works with multiple [src] children, where one has a static source, 
        ///     and the other an expression
        /// </summary>
        [Test]
        public void SetTwoSrcMixed ()
        {
            var result = ExecuteLambda (@"_ess:ess
set:x:/..?value
  src:succ
  src:x:/../*/_ess?value");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     Verifies [set] works with multiple [src] children, where one has a static string source, 
        ///     and the other a node value
        /// </summary>
        [Test]
        public void SetTwoSrcStaticAndNode ()
        {
            var result = ExecuteLambda (@"set:x:/..?value
  src:succ
  src:node:""ess""");
            Assert.AreEqual ("succ\r\ness", result.Value);
        }
    }
}
