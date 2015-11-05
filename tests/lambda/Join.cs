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
        ///     verifies [join] works without a separator
        /// </summary>
        [Test]
        public void Join01 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
join:x:/-/*?name");
            Assert.AreEqual (0, node [1].Count);
            Assert.AreEqual ("foo1foo2", node [1].Value);
        }

        /// <summary>
        ///     verifies [join] works with a separator
        /// </summary>
        [Test]
        public void Join02 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
join:x:/-/*?name
  =:,");
            Assert.AreEqual (0, node [1].Count);
            Assert.AreEqual ("foo1,foo2", node [1].Value);
        }
        
        /// <summary>
        ///     verifies [join] works with both separators when selecting nodes
        /// </summary>
        [Test]
        public void Join03 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
join:x:/-/*
  =:,
  ==:-");
            Assert.AreEqual (0, node [1].Count);
            Assert.AreEqual ("foo1-bar1,foo2-bar2", node [1].Value);
        }
        
        /// <summary>
        ///     verifies [join] works with trimming where trimming is an expression yielding true
        /// </summary>
        [Test]
        public void Join04 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:"" bar1""
  foo2:bar2
join:x:/-/*
  =:,
  ==:-
  trim:x:/./+?value
_trim:true");
            Assert.AreEqual (0, node [1].Count);
            Assert.AreEqual ("foo1-bar1,foo2-bar2", node [1].Value);
        }

        /// <summary>
        ///     verifies [join] works with trimming where trimming is an expression yielding false
        /// </summary>
        [Test]
        public void Join05 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:"" bar1""
  foo2:bar2
join:x:/-/*
  =:,
  ==:-
  trim:x:/./+?value
_trim:false");
            Assert.AreEqual (0, node [1].Count);
            Assert.AreEqual ("foo1- bar1,foo2-bar2", node [1].Value);
        }
        
        /// <summary>
        ///     verifies [join] works with both separators when selecting nodes, and one node has null value
        /// </summary>
        [Test]
        public void Join06 ()
        {
            var node = ExecuteLambda (@"_data
  foo1
  foo2:bar2
join:x:/-/*
  =:,
  ==:-");
            Assert.AreEqual (0, node [1].Count);
            Assert.AreEqual ("foo1,foo2-bar2", node [1].Value);
        }
    }
}
