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
    ///     unit tests for testing the [set] lambda keyword
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
        public void Split01 ()
        {
            var node = ExecuteLambda (@"_data:foo bar
split:x:/-?value
  =:"" """);
            Assert.AreEqual (2, node [1].Count);
            Assert.AreEqual ("foo", node [1] [0].Name);
            Assert.AreEqual ("bar", node [1] [1].Name);
        }
        
        /// <summary>
        ///     verifies [split] works with a constant source
        /// </summary>
        [Test]
        public void Split02 ()
        {
            var node = ExecuteLambda (@"split:""foo bar""
  =:"" """);
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo", node [0] [0].Name);
            Assert.AreEqual ("bar", node [0] [1].Name);
        }
        
        /// <summary>
        ///     verifies [split] works when forced to convert from node to string
        /// </summary>
        [Test]
        public void Split03 ()
        {
            var node = ExecuteLambda (@"split:node:@""foo
  bar""
  =:""\r\n""");
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo", node [0] [0].Name);
            Assert.AreEqual ("  bar", node [0] [1].Name);
        }
        
        /// <summary>
        ///     verifies [split] works when forced to convert from node to string through an expression
        ///     where expression yields node result set
        /// </summary>
        [Test]
        public void Split05 ()
        {
            var node = ExecuteLambda (@"_data
  foo
    bar
split:x:/-/*
  =:""\r\n""");
            Assert.AreEqual (2, node [1].Count);
            Assert.AreEqual ("foo", node [1] [0].Name);
            Assert.AreEqual ("  bar", node [1] [1].Name);
        }
        
        /// <summary>
        ///     verifies [split] works when splitting into name/value nodes, without trimming
        /// </summary>
        [Test]
        public void Split06 ()
        {
            var node = ExecuteLambda (@"_data:foo1-bar1, foo2-bar2
split:x:/-?value
  =:,
  ==:-");
            Assert.AreEqual (2, node [1].Count);
            Assert.AreEqual ("foo1", node [1] [0].Name);
            Assert.AreEqual ("bar1", node [1] [0].Value);
            Assert.AreEqual (" foo2", node [1] [1].Name);
            Assert.AreEqual ("bar2", node [1] [1].Value);
        }
        
        /// <summary>
        ///     verifies [split] works when splitting into name/value nodes, with trimming
        /// </summary>
        [Test]
        public void Split07 ()
        {
            var node = ExecuteLambda (@"_data:foo1-bar1, foo2-bar2
split:x:/-?value
  =:,
  ==:-
  trim:true");
            Assert.AreEqual (2, node [1].Count);
            Assert.AreEqual ("foo1", node [1] [0].Name);
            Assert.AreEqual ("bar1", node [1] [0].Value);
            Assert.AreEqual ("foo2", node [1] [1].Name);
            Assert.AreEqual ("bar2", node [1] [1].Value);
        }
    }
}
