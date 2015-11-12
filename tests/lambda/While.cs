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
            : base ("p5.lambda", "p5.hyperlisp", "p5.types", "p5.math") { }

        /// <summary>
        ///     verifies that [while] works when source is a constant string
        /// </summary>
        [Test]
        public void WhileConstantString ()
        {
            var result = ExecuteLambda (@"while:foo
  _foo:bool:false
  add:x:/..
    src:_foo1
  set:x:/.?value");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
        
        /// <summary>
        ///     verifies that [while] works when source is an expression which leads to a constant integer decremented to zero
        /// </summary>
        [Test]
        public void WhileExpressionInteger ()
        {
            var result = ExecuteLambda (@"_data:int:1
while:x:/-?value
  add:x:/..
    src:_foo1
  set:x:/./-?value
    -:x:/././-?value
      _:1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
        
        /// <summary>
        ///     verifies that [while] works when source is a constant integer decremented to zero
        /// </summary>
        [Test]
        public void WhileConstantInteger ()
        {
            var result = ExecuteLambda (@"while:int:1
  add:x:/..
    src:_foo1
  set:x:/.?value
    -:x:/./.?value
      _:1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
        
        /// <summary>
        ///     verifies that [while] works when source is a constant boolean yielding false
        /// </summary>
        [Test]
        public void WhileConstantBooleanFalse ()
        {
            var result = ExecuteLambda (@"while:bool:false
  add:x:/..
    src:_foo1");
            Assert.AreEqual (0, result.Count);
        }
        
        /// <summary>
        ///     verifies that [while] works when source is a constant boolean yielding true
        /// </summary>
        [Test]
        public void WhileConstantBooleanTrue ()
        {
            var result = ExecuteLambda (@"while:bool:true
  add:x:/..
    src:_foo1
  set:x:/.?value
    src:bool:false");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.IsNull (result [0].Value);
        }
        
        /// <summary>
        ///     verifies that [while] works when source is a constant null value, 
        ///     and first child is evaluated instead
        /// </summary>
        [Test]
        public void WhileConstantNull ()
        {
            var result = ExecuteLambda (@"while
  _foo:bool:false
  add:x:/..
    src:_foo1
  set:x:/.?value
    src:bool:false");
            Assert.AreEqual (0, result.Count);
        }
    }
}
