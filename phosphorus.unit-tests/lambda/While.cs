
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests.lambda
{
    /// <summary>
    /// unit tests for testing the [while] lambda keyword
    /// </summary>
    [TestFixture]
    public class While : TestBase
    {
        public While ()
            : base ("phosphorus.lambda", "phosphorus.hyperlisp", "phosphorus.types")
        { }

        /// <summary>
        /// verifies that [while] works when source is 'node' expression, and condition
        /// is 'exists'
        /// </summary>
        [Test]
        public void While1 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess").Parent
                .Add ("_result")
                .Add ("while", "@/-2/*/?node").LastChild
                    .Add ("set", "@/./-/?value").LastChild
                        .Add ("source", "{0}{1}").LastChild
                            .Add (string.Empty, "@/../*/_result/?value")
                            .Add (string.Empty, "@/../*/_data/0/?name").Parent.Parent
                    .Add ("set", "@/../*/_data/0/?node").Root;
            _context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }
        
        /// <summary>
        /// verifies that [while] works when source is 'node' expression, comparison is 'more-than',
        /// and comparison runs towards a 'count' expression
        /// </summary>
        [Test]
        public void While2 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess")
                    .Add ("error").Parent
                .Add ("_result")
                .Add ("while", "@/-2/*/?count").LastChild
                    .Add (">", 1)
                    .Add ("lambda").LastChild
                        .Add ("set", "@/././-/?value").LastChild
                            .Add ("source", "{0}{1}").LastChild
                                .Add (string.Empty, "@/../*/_result/?value")
                                .Add (string.Empty, "@/../*/_data/0/?name").Parent.Parent
                        .Add ("set", "@/../*/_data/0/?node").Root;
            _context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }
        
        /// <summary>
        /// verifies that [while] works when source is 'name' expression, comparison is 'not-equals',
        /// and comparison runs towards a node expression of type 'name'
        /// </summary>
        [Test]
        public void While3 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess")
                    .Add ("error").Parent
                .Add ("_result")
                .Add ("while", "@/-2/0/?name").LastChild
                    .Add ("!=", "error")
                    .Add ("lambda").LastChild
                        .Add ("set", "@/././-/?value").LastChild
                            .Add ("source", "{0}{1}").LastChild
                                .Add (string.Empty, "@/../*/_result/?value")
                                .Add (string.Empty, "@/../*/_data/0/?name").Parent.Parent
                        .Add ("set", "@/../*/_data/0/?node").Root;
            _context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        /// verifies that [while] works with formatting expressions, doing a 'not-equals' on 'value',
        /// where rhs is of type integer
        /// </summary>
        [Test]
        public void While4 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess")
                    .Add ("error", 5).Parent
                .Add ("_result")
                .Add ("while", "@/{0}/0/?value").LastChild
                    .Add (string.Empty, "-2")
                    .Add ("!=", 5)
                    .Add ("lambda").LastChild
                        .Add ("set", "@/././-/?value").LastChild
                            .Add ("source", "{0}{1}").LastChild
                                .Add (string.Empty, "@/../*/_result/?value")
                                .Add (string.Empty, "@/../*/_data/0/?name").Parent.Parent
                        .Add ("set", "@/../*/_data/0/?node").Root;
            _context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        /// verifies that [while] is immutable by default
        /// </summary>
        [Test]
        public void While5 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("success", "foo")
                    .Add ("error").Parent
                .Add ("_result")
                .Add ("while", "@/-2/0/?value").LastChild
                    .Add ("set", "@/./-/?value").LastChild
                        .Add ("source", "@/../*/_data/0/?name").Parent
                    .Add ("set", "@/../*/_data/0/?node")
                    .Add ("set", "@?node").Root; // this node should be deleted, and reinserted afterwards again
            _context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
            Assert.AreEqual ("@?node", node [2] [2].Value);
        }

        /// <summary>
        /// verifies that [while] is not immutable, if it is overridden with "lambda" child
        /// </summary>
        [Test]
        public void While6 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("success", "foo")
                    .Add ("error").Parent
                .Add ("_result")
                .Add ("while", "@/-2/0/?value").LastChild
                    .Add ("lambda").LastChild
                        .Add ("set", "@/././-/?value").LastChild
                            .Add ("source", "@/../*/_data/0/?name").Parent
                        .Add ("set", "@/../*/_data/0/?node")
                        .Add ("set", "@?node").Root; // this node should be deleted, and never reinserted
            _context.Raise ("while", node [2]);
            Assert.AreEqual ("success", node [1].Value);
            Assert.AreEqual (2, node [2] [0].Count);
        }

        /// <summary>
        /// verifies that [while] condition can be manipulated from inside the [while] itself, if [while]
        /// if not of type "immutable"
        /// </summary>
        [Test]
        public void While7 ()
        {
            Node node = new Node ()
                .Add ("_result")
                .Add ("while", "tjobing").LastChild
                    .Add ("lambda").LastChild
                        .Add ("set", "@/././-/?value").LastChild
                            .Add ("source", "success").Parent
                        .Add ("set", "@/../*/while/?value").Root; // at this point we [set] [while]'s value to null
            _context.Raise ("while", node [1]);
            Assert.AreEqual ("success", node [0].Value);
        }
    }
}
