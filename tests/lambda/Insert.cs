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
    ///     unit tests for testing the [add] lambda keyword
    /// </summary>
    [TestFixture]
    public class Insert : TestBase
    {
        public Insert ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp")
        { }

        [Test]
        public void Insert01 ()
        {
            var node = ExecuteLambda (@"_result
  _x1
insert-before:x:/-/0
  src
    foo:bar");

            Assert.AreEqual ("foo", node [0] [0].Name);
            Assert.AreEqual ("bar", node [0] [0].Value);
        }
        
        [Test]
        public void Insert02 ()
        {
            var node = ExecuteLambda (@"_result
insert-after:x:/-
  src
    foo:bar");

            Assert.AreEqual ("foo", node [1].Name);
            Assert.AreEqual ("bar", node [1].Value);
        }
        
        [Test]
        public void Insert03 ()
        {
            var node = ExecuteLambda (@"_result
insert-after:x:
  src
    set:x:/../0?value
      src:success");

            Assert.AreEqual ("success", node [0].Value);
        }
        
        [Test]
        public void Insert04 ()
        {
            var node = ExecuteLambda (@"_result:success
insert-before:x:
  src
    set:x:/../0?value
      src:error");

            Assert.AreEqual ("success", node [0].Value);
            Assert.AreEqual ("set", node [1].Name);
        }
    }
}
