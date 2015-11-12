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
    ///     unit tests for testing the [join] lambda keyword
    /// </summary>
    [TestFixture]
    public class Join : TestBase
    {
        public Join ()
            : base ("p5.lambda", "p5.hyperlisp", "p5.types") { }

        /// <summary>
        ///     verifies [join] works without a separator when joining name
        /// </summary>
        [Test]
        public void JoinNameWithoutSeparator ()
        {
            var result = ExecuteLambda (@"_data
  foo1
  foo2
insert-before:x:/../0
  join:x:/./-/*?name");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("foo1foo2", result [0].Name);
        }
        
        /// <summary>
        ///     verifies [join] works with a separator when joining name
        /// </summary>
        [Test]
        public void JoinNameWithSeparator ()
        {
            var result = ExecuteLambda (@"_data
  foo1
  foo2
insert-before:x:/../0
  join:x:/./-/*?name
    =:,");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("foo1,foo2", result [0].Name);
        }
        
        /// <summary>
        ///     verifies [join] works without a separator when joining nodes
        /// </summary>
        [Test]
        public void JoinNodesWithoutSeparator ()
        {
            var result = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
insert-before:x:/../0
  join:x:/./-/*");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("foo1bar1foo2bar2", result [0].Name);
        }
        
        /// <summary>
        ///     verifies [join] works with one separator
        /// </summary>
        [Test]
        public void JoinNodesWithOneSeparator ()
        {
            var result = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
insert-before:x:/../0
  join:x:/./-/*
    =:,");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("foo1bar1,foo2bar2", result [0].Name);
        }
        
        /// <summary>
        ///     verifies [join] works with two separator
        /// </summary>
        [Test]
        public void JoinNodesWithTwoSeparators ()
        {
            var result = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
insert-before:x:/../0
  join:x:/./-/*
    =:,
    ==:-");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("foo1-bar1,foo2-bar2", result [0].Name);
        }
    }
}
