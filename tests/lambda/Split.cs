/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;
using p5.exp;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [split] lambda keyword
    /// </summary>
    [TestFixture]
    public class Split : TestBase
    {
        public Split ()
            : base ("p5.lambda", "p5.hyperlisp", "p5.types") { }
        
        /// <summary>
        ///     verifies [split] works with an expression source
        /// </summary>
        [Test]
        public void SplitExpressionResults ()
        {
            var node = ExecuteLambda (@"_data:foo bar
insert-before:x:/../0
  split:x:/./-?value
    =:"" """);
            Assert.AreEqual (2, node.Children.Count);
            Assert.AreEqual ("foo", node [0].Name);
            Assert.AreEqual ("bar", node [1].Name);
        }
        
        /// <summary>
        ///     verifies [split] works with an expression source
        /// </summary>
        [Test]
        public void SplitExpressionYieldingTwoResults ()
        {
            var node = ExecuteLambda (@"_data1:""foo1 bar1 ""
_data2:foo2 bar2
insert-before:x:/../0
  split:x:/./-2|/./-?value
    =:"" """);
            Assert.AreEqual (4, node.Children.Count);
            Assert.AreEqual ("foo1", node [0].Name);
            Assert.AreEqual ("bar1", node [1].Name);
            Assert.AreEqual ("foo2", node [2].Name);
            Assert.AreEqual ("bar2", node [3].Name);
        }
    }
}
