
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests.lambda
{
    /// <summary>
    /// unit tests for testing the [for-each] lambda keyword
    /// </summary>
    [TestFixture]
    public class ForEach : TestBase
    {
        public ForEach ()
            : base ("phosphorus.lambda")
        { }

        /// <summary>
        /// verifies that [for-each] works when expression is of type 'name'
        /// </summary>
        [Test]
        public void ForEach1 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add ("su")
                    .Add ("cc")
                    .Add ("ess").Parent
                .Add ("_result")
                .Add ("for-each", "@/-2/*/?name").LastChild
                    .Add ("set", "@/./-/?value").LastChild
                        .Add ("source", "{0}{1}").LastChild
                            .Add (string.Empty, "@/../*/_result/?value")
                            .Add (string.Empty, "@/./././*/__dp/?value").Root;
            _context.Raise ("for-each", node [2]);
            Assert.AreEqual ("success", node [1].Value);
        }
        
        /// <summary>
        /// verifies that [for-each] works when expression is of type 'value',
        /// and there are different types in some nodes
        /// </summary>
        [Test]
        public void ForEach2 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add (string.Empty, "succ")
                    .Add (string.Empty, 5)
                    .Add (string.Empty, "ess").Parent
                .Add ("_result")
                .Add ("for-each", "@/-2/*/?value").LastChild
                    .Add ("set", "@/./-/?value").LastChild
                        .Add ("source", "{0}{1}").LastChild
                            .Add (string.Empty, "@/../*/_result/?value")
                            .Add (string.Empty, "@/..for-each/*/__dp/?value").Root;
            _context.Raise ("for-each", node [2]);
            Assert.AreEqual ("succ5ess", node [1].Value);
        }
        
        /// <summary>
        /// verifies that [for-each] works when expression is of type 'node',
        /// and there are different types in some nodes
        /// </summary>
        [Test]
        public void ForEach3 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                    .Add (string.Empty, "succ")
                    .Add (string.Empty, 5)
                    .Add (string.Empty, "ess").Parent
                .Add ("_result")
                .Add ("for-each", "@/-2/*/?node").LastChild
                    .Add ("set", "@/./-/?value").LastChild
                        .Add ("source", "{0}{1}").LastChild
                            .Add (string.Empty, "@/../*/_result/?value")
                            .Add (string.Empty, "@/..for-each/*/__dp/#/?value").Root;
            _context.Raise ("for-each", node [2]);
            Assert.AreEqual ("succ5ess", node [1].Value);
        }
        
        /// <summary>
        /// verifies that [for-each] is immutable by default
        /// </summary>
        [Test]
        public void ForEach4 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("for-each", "@/-/?node").LastChild
                    .Add ("set", "@?node").Root;
            _context.Raise ("for-each", node [1]);
            Assert.AreEqual ("@?node", node [1] [0].Value);
        }
        
        /// <summary>
        /// verifies that [for-each] is not immutable, if overridden with lambda child
        /// </summary>
        [Test]
        public void ForEach5 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("for-each", "@/-/?node").LastChild
                    .Add ("lambda").LastChild
                        .Add ("set", "@?node").Root;
            _context.Raise ("for-each", node [1]);
            Assert.AreEqual (0, node [1] [0].Count);
        }
    }
}
