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
    ///     unit tests for testing the [while] lambda keyword
    /// </summary>
    [TestFixture]
    public class While : TestBase
    {
        public While ()
            : base ("p5.lambda", "p5.hyperlisp", "p5.types") { }

        /// <summary>
        ///     verifies that [while] works when source is 'node' expression, and condition
        ///     is 'exists'
        /// </summary>
        [Test]
        public void While1 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess").Parent
                .Add ("_result")
                .Add ("while", Expression.Create ("/-2/*", Context)).LastChild
                    .Add ("set", Expression.Create ("/./-?value", Context)).LastChild
                        .Add ("src", "{0}{1}").LastChild
                            .Add (string.Empty, Expression.Create ("/../*/_result?value", Context))
                            .Add (string.Empty, Expression.Create ("/../*/_data/0?name", Context)).Parent.Parent
                    .Add ("set", Expression.Create ("/../*/_data/0", Context)).Root;
            Context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     verifies that [while] works when source is 'node' expression, comparison is 'more-than',
        ///     and comparison runs towards a 'count' expression
        /// </summary>
        [Test]
        public void While2 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess")
                    .Add ("error").Parent
                .Add ("_result")
                .Add ("while", Expression.Create ("/-2/*?count", Context)).LastChild
                    .Add (">", 1)
                    .Add ("lambda").LastChild
                        .Add ("set", Expression.Create ("/././-?value", Context)).LastChild
                            .Add ("src", "{0}{1}").LastChild
                                .Add (string.Empty, Expression.Create ("/../*/_result?value", Context))
                                .Add (string.Empty, Expression.Create ("/../*/_data/0?name", Context)).Parent.Parent
                        .Add ("set", Expression.Create ("/../*/_data/0", Context)).Root;
            Context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     verifies that [while] works when source is 'name' expression, comparison is 'not-equals',
        ///     and comparison runs towards a node expression of type 'name'
        /// </summary>
        [Test]
        public void While3 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess")
                    .Add ("error").Parent
                .Add ("_result")
                .Add ("while", Expression.Create ("/-2/0?name", Context)).LastChild
                    .Add ("!=", "error")
                    .Add ("lambda").LastChild
                        .Add ("set", Expression.Create ("/././-?value", Context)).LastChild
                            .Add ("src", "{0}{1}").LastChild
                                .Add (string.Empty, Expression.Create ("/../*/_result?value", Context))
                                .Add (string.Empty, Expression.Create ("/../*/_data/0?name", Context)).Parent.Parent
                        .Add ("set", Expression.Create ("/../*/_data/0", Context)).Root;
            Context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     verifies that [while] works with formatting expressions, doing a 'not-equals' on 'value',
        ///     where rhs is of type integer
        /// </summary>
        [Test]
        public void While4 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess")
                    .Add ("error", 5).Parent
                .Add ("_result")
                .Add ("while", Expression.Create ("/{0}/0?value", Context)).LastChild
                    .Add (string.Empty, "-2")
                    .Add ("!=", 5)
                    .Add ("lambda").LastChild
                        .Add ("set", Expression.Create ("/././-?value", Context)).LastChild
                            .Add ("src", "{0}{1}").LastChild
                                .Add (string.Empty, Expression.Create ("/../*/_result?value", Context))
                                .Add (string.Empty, Expression.Create ("/../*/_data/0?name", Context)).Parent.Parent
                        .Add ("set", Expression.Create ("/../*/_data/0", Context)).Root;
            Context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     verifies that [while] is immutable by default
        /// </summary>
        [Test]
        public void While5 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add ("success", "foo")
                    .Add ("error").Parent
                .Add ("_result")
                .Add ("while", Expression.Create ("/-2/0?value", Context)).LastChild
                    .Add ("set", Expression.Create ("/./-?value", Context)).LastChild
                        .Add ("src", Expression.Create ("/../*/_data/0?name", Context)).Parent
                    .Add ("set", Expression.Create ("/../*/_data/0", Context))
                    .Add ("set", Expression.Create ("", Context)).Root; // this node should be deleted, and reinserted afterwards again
            Context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
            Assert.AreEqual ("set", node [2] [2].Name);
        }

        /// <summary>
        ///     verifies that [while] is not immutable, if it is overridden with "lambda" child
        /// </summary>
        [Test]
        public void While6 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add ("success", "foo")
                    .Add ("error").Parent
                .Add ("_result")
                .Add ("while", Expression.Create ("/-2/0?value", Context)).LastChild
                    .Add ("lambda").LastChild
                        .Add ("set", Expression.Create ("/././-?value", Context)).LastChild
                            .Add ("src", Expression.Create ("/../*/_data/0?name", Context)).Parent
                        .Add ("set", Expression.Create ("/../*/_data/0", Context))
                        .Add ("set", Expression.Create ("", Context)).Root; // this node should be deleted, and never reinserted
            Context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
            Assert.AreEqual (2, node [2] [0].Count);
        }

        /// <summary>
        ///     verifies that [while] condition can be manipulated from inside the [while] itself, if [while]
        ///     if not of type "immutable"
        /// </summary>
        [Test]
        public void While7 ()
        {
            var node = new Node ()
                .Add ("_result")
                .Add ("while", "tjobing").LastChild
                    .Add ("lambda").LastChild
                        .Add ("set", Expression.Create ("/././-?value", Context)).LastChild
                            .Add ("src", "success").Parent
                        .Add ("set", Expression.Create ("/../*/while?value", Context)).Root; // at this point we [set] [while]'s value to null
            Context.Raise ("while", node [1]);
            Assert.AreEqual ("success", node [0].Value);
        }
    }
}