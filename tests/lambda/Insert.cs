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
    ///     unit tests for testing the [insert-before] and [insert-after] lambda keywords
    /// </summary>
    [TestFixture]
    public class Insert : TestBase
    {
        public Insert ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp")
        { }

        [Test]
        public void InsertBefore ()
        {
            var result = ExecuteLambda (@"insert-before:x:/../0
  src
    foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
        
        [Test]
        public void InsertAfter ()
        {
            var result = ExecuteLambda (@"insert-after:x:/../0
  src
    _foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
    }
}
