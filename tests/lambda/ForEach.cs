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
    ///     unit tests for testing the [for-each] lambda keyword
    /// </summary>
    [TestFixture]
    public class ForEach : TestBase
    {
        public ForEach ()
        : base ("p5.lambda", "p5.types", "p5.hyperlisp") { }

        /// <summary>
        ///     verifies that [for-each] works when expression is of type 'name'
        /// </summary>
        [Test]
        public void ForEach01 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess").Parent
                .Add ("_result")
                .Add ("for-each", Expression.Create ("/-2/*?name", Context)).LastChild
                    .Add ("set", Expression.Create ("/./-?value", Context)).LastChild
                        .Add ("src", "{0}{1}").LastChild
                            .Add (string.Empty, Expression.Create ("/../*/_result?value", Context))
                            .Add (string.Empty, Expression.Create ("/./././*/__dp?value", Context)).Root;
            Context.Raise ("for-each", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     verifies that [for-each] works when expression is of type 'value',
        ///     and there are different types in some nodes
        /// </summary>
        [Test]
        public void ForEach02 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add (string.Empty, "succ")
                    .Add (string.Empty, 5)
                    .Add (string.Empty, "ess").Parent
                .Add ("_result")
                .Add ("for-each", Expression.Create ("/-2/*?value", Context)).LastChild
                    .Add ("set", Expression.Create ("/./-?value", Context)).LastChild
                        .Add ("src", "{0}{1}").LastChild
                            .Add (string.Empty, Expression.Create ("/../*/_result?value", Context))
                            .Add (string.Empty, Expression.Create ("/..for-each/*/__dp?value", Context)).Root;
            Context.Raise ("for-each", node [2]);
            Assert.AreEqual ("succ5ess", node [1].Value);
        }

        /// <summary>
        ///     verifies that [for-each] works when expression is of type 'node',
        ///     and there are different types in some nodes
        /// </summary>
        [Test]
        public void ForEach03 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add (string.Empty, "succ")
                    .Add (string.Empty, 5)
                    .Add (string.Empty, "ess").Parent
                .Add ("_result")
                .Add ("for-each", Expression.Create ("/-2/*", Context)).LastChild
                    .Add ("set", Expression.Create ("/./-?value", Context)).LastChild
                        .Add ("src", "{0}{1}").LastChild
                            .Add (string.Empty, Expression.Create ("/../*/_result?value", Context))
                            .Add (string.Empty, Expression.Create ("/..for-each/*/__dp/#?value", Context)).Root;
            Context.Raise ("for-each", node [2]);
            Assert.AreEqual ("succ5ess", node [1].Value);
        }

        /// <summary>
        ///     verifies that [for-each] is immutable by default
        /// </summary>
        [Test]
        public void ForEach04 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("for-each", Expression.Create ("/-", Context)).LastChild
                    .Add ("set", Expression.Create ("", Context)).Root;
            Context.Raise ("for-each", node [1]);
            Assert.AreEqual ("set", node [1] [0].Name);
        }

        /// <summary>
        ///     verifies that [for-each] is not immutable, if overridden with lambda child
        /// </summary>
        [Test]
        public void ForEach05 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("for-each", Expression.Create ("/-", Context)).LastChild
                    .Add ("lambda").LastChild
                        .Add ("set", Expression.Create ("", Context)).Root;
            Context.Raise ("for-each", node [1]);
            Assert.AreEqual (0, node [1] [0].Count);
        }
        
        /// <summary>
        ///     Verifies that [for-each] works with value being Node instead of expression
        /// </summary>
        [Test]
        public void ForEach06 ()
        {
            var result = ExecuteLambda (@"_data
for-each:node:@""_data
  foo:succ
  bar:ess""
  set:x:/../0?value
    src:{0}{1}
      :x:/../0?value
      :x:/./././*/__dp/#?value");
            Assert.AreEqual ("success", result [0].Value);
        }
        
        /// <summary>
        ///     Verifies that [for-each] works with value being Node instead of expression and
        ///     node is modified during execution
        /// </summary>
        [Test]
        public void ForEach07 ()
        {
            var result = ExecuteLambda (@"for-each:node:@""_data
  foo:failure
  bar:failure""
  set:x:/./*/__dp/#?value
    src:success");
            Assert.AreEqual ("success", result [0].Get<Node> (Context) [0].Value);
            Assert.AreEqual ("success", result [0].Get<Node> (Context) [1].Value);
        }
    }
}
