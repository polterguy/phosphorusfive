/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [if], [else-if] and [else] lambda keyword
    /// </summary>
    [TestFixture]
    public class Branching : TestBase
    {
        public Branching ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp") { }

        /// <summary>
        ///     verifies [if] works when given constant string
        /// </summary>
        [Test]
        public void IfConstantStringTrue ()
        {
            var result = ExecuteLambda (@"if:foo
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
        
        /// <summary>
        ///     verifies [if] works when given constant integer
        /// </summary>
        [Test]
        public void IfConstantIntegerTrue ()
        {
            var result = ExecuteLambda (@"if:int:5
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
        
        /// <summary>
        ///     verifies [if] works when given two constant strings to compare
        /// </summary>
        [Test]
        public void IfConstantCompareTwoString ()
        {
            var result = ExecuteLambda (@"if:foo
  equals:foo
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
        
        /// <summary>
        ///     verifies [if] works when given two constants of different types to compare
        /// </summary>
        [Test]
        public void IfConstantTwoDifferentTypes ()
        {
            var result = ExecuteLambda (@"if:5
  equals:int:5
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (0, result.Count);
        }
        
        /// <summary>
        ///     verifies [if] works when given two expressions to compare
        /// </summary>
        [Test]
        public void IfExpressionsTwoString ()
        {
            var result = ExecuteLambda (@"_foo1:foo
_foo2:foo
if:x:/-2?value
  equals:x:/./-?value
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
        
        /// <summary>
        ///     verifies [if] works when given two expressions returning different types to compare
        /// </summary>
        [Test]
        public void IfExpressionsTwoDifferentTypes ()
        {
            var result = ExecuteLambda (@"_foo1:int:5
_foo2:5
if:x:/-2?value
  equals:x:/./-?value
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (0, result.Count);
        }
        
        /// <summary>
        ///     verifies [if] works when anding results of one comparison
        /// </summary>
        [Test]
        public void IfAndingResults ()
        {
            var result = ExecuteLambda (@"_foo1:int:5
_foo2:5
if:x:/-2?value
  equals:int:5
  and:x:/./-?value
    =:5
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
        
        /// <summary>
        ///     verifies [if] works when oring results of one comparison
        /// </summary>
        [Test]
        public void IfOringResults ()
        {
            var result = ExecuteLambda (@"_foo1:int:5
_foo2:5
if:x:/-2?value
  equals:int:6
  or:x:/./-?value
    =:5
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
        
        /// <summary>
        ///     verifies [if] works when anding yielding false, for then to or results of another comparison
        /// </summary>
        [Test]
        public void IfOringAndedResults ()
        {
            var result = ExecuteLambda (@"_foo1:int:5
_foo2:5
if:x:/-2?value
  equals:int:6
  and:x:/./-?value
    equals:6
  or:x:/./-?value
    =:5
    and:x:/././-2?value
      equals:int:5
  add:x:/..
    src
      _foo1:bar1");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("_foo1", result [0].Name);
            Assert.AreEqual ("bar1", result [0].Value);
        }
    }
}
