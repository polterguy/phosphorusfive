/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [append] lambda keyword
    /// </summary>
    [TestFixture]
    public class Branching : TestBase
    {
        public Branching ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp") { }

        /// <summary>
        ///     verifies [if] works when given constant
        /// </summary>
        [Test]
        public void If01 ()
        {
            var result = ExecuteLambda (@"if:foo
  set:@/../?value
    source:success");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two constant strings that are equal
        /// </summary>
        [Test]
        public void If02 ()
        {
            var result = ExecuteLambda (@"if:foo
  =:foo
  lambda
    set:@/../?value
      source:success");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two constant strings that are not equal
        /// </summary>
        [Test]
        public void If03 ()
        {
            var result = ExecuteLambda (@"_result:success
if:foo
  =:bar
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing one constant to one expression,
        ///     and result should yield true
        /// </summary>
        [Test]
        public void If04 ()
        {
            var result = ExecuteLambda (@"_result:foo
if:@/../*/_result/?value
  =:foo
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing one constant to one expression,
        ///     and result should yield false
        /// </summary>
        [Test]
        public void If05 ()
        {
            var result = ExecuteLambda (@"_result:success
if:@/../*/_result/?value
  =:foo
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing one constant to one expression,
        ///     and result should yield true, and expression is on right hand side
        ///     of comparison
        /// </summary>
        [Test]
        public void If06 ()
        {
            var result = ExecuteLambda (@"_result:error
if:error
  =:@/../*/_result/?value
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing one constant to one expression,
        ///     and result should yield false, and expression is on right hand side
        ///     of comparison
        /// </summary>
        [Test]
        public void If07 ()
        {
            var result = ExecuteLambda (@"_result:success
if:foo
  =:@/../*/_result/?value
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     and result should yield true
        /// </summary>
        [Test]
        public void If08 ()
        {
            var result = ExecuteLambda (@"_result1:foo
_result2:foo
if:@/../*/_result1/?value
  =:@/../*/_result2/?value
  lambda
    set:@/../*/_result1/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     and result should yield false
        /// </summary>
        [Test]
        public void If09 ()
        {
            var result = ExecuteLambda (@"_result1:success
_result2:foo
if:@/../*/_result1/?value
  =:@/../*/_result2/?value
  lambda
    set:@/../*/_result1/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions with different types returned,
        ///     and result should yield false
        /// </summary>
        [Test]
        public void If10 ()
        {
            var result = ExecuteLambda (@"_result1:int:5
_result2:5
if:@/../*/_result1/?value
  =:@/../*/_result2/?value
  lambda
    set:@/../*/_result1/?value
      source:error");
            Assert.AreEqual (5, result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions returning integer values,
        ///     and result should yield true
        /// </summary>
        [Test]
        public void If11 ()
        {
            var result = ExecuteLambda (@"_result1:int:5
_result2:int:5
if:@/../*/_result1/?value
  =:@/../*/_result2/?value
  lambda
    set:@/../*/_result1/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions returning integer values,
        ///     and result should yield false
        /// </summary>
        [Test]
        public void If12 ()
        {
            var result = ExecuteLambda (@"_result1:int:5
_result2:int:6
if:@/../*/_result1/?value
  =:@/../*/_result2/?value
  lambda
    set:@/../*/_result1/?value
      source:error");
            Assert.AreEqual (5, result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     returning multiple results, and result should yield true
        /// </summary>
        [Test]
        public void If13 ()
        {
            var result = ExecuteLambda (@"_result1
  foo1:bar1
  foo2:bar2
_result2
  foo1:bar1
  foo2:bar2
if:@/../*/_result1/*/?value
  =:@/../*/_result2/*/?value
  lambda
    set:@/../*/_result1/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     returning multiple results, and result should yield false
        /// </summary>
        [Test]
        public void If14 ()
        {
            var result = ExecuteLambda (@"_result1:success
  foo1:bar1
  foo2:bar2
_result2
  foo1:bar1
  foo2:ERROR
if:@/../*/_result1/*/?value
  =:@/../*/_result2/*/?value
  lambda
    set:@/../*/_result1/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     returning multiple results, with multiple different types,
        ///     and result should yield true
        /// </summary>
        [Test]
        public void If15 ()
        {
            var result = ExecuteLambda (@"_result1
  foo1:bar1
  foo2:int:5
_result2
  foo1:bar1
  foo2:int:5
if:@/../*/_result1/*/?value
  =:@/../*/_result2/*/?value
  lambda
    set:@/../*/_result1/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     returning multiple results, with multiple different types,
        ///     and result should yield false
        /// </summary>
        [Test]
        public void If16 ()
        {
            var result = ExecuteLambda (@"_result1:success
  foo1:bar1
  foo2:int:5
_result2
  foo1:bar1
  foo2:int:6
if:@/../*/_result1/*/?value
  =:@/../*/_result2/*/?value
  lambda
    set:@/../*/_result1/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     returning multiple results, where node graph is different
        ///     in expression results
        /// </summary>
        [Test]
        public void If17 ()
        {
            var result = ExecuteLambda (@"_result1:success
  foo1:bar1
  foo2:int:5
_result2
  foo1:bar1
  foo2:int:5
    error
if:@/../*/_result1/*/?node
  =:@/../*/_result2/*/?node
  lambda
    set:@/../*/_result1/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     returning multiple results, where node graph is different
        ///     in expression results, yet results should be similar anyway
        /// </summary>
        [Test]
        public void If18 ()
        {
            var result = ExecuteLambda (@"_result1:error
  foo1:bar1
  foo2:int:5
_result2
  foo1:bar1
  foo2:int:5
    error
if:@/../*/_result1/*/?value
  =:@/../*/_result2/*/?value
  lambda
    set:@/../*/_result1/?value
      source:success");

            // note, this comparison should yield true, since we're comparing the node's 'values',
            // which should be similar, since our [error] node above in [_result2] has no value
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions,
        ///     returning multiple results, where node graph is different
        ///     in expression results, yet results should be similar anyway
        /// </summary>
        [Test]
        public void If19 ()
        {
            var result = ExecuteLambda (@"_result1:error
  foo1:bar1
  foo2:5
_result2
  foo1:bar1
  empty:
  foo2:int:5
    error:
if:@/../*/_result1/*/?value
  =:@/../*/_result2/*/?value
  lambda
    set:@/../*/_result1/?value
      source:success");

            // note, this comparison should yield true, since we're comparing the node's 'values',
            // which should be similar, since our [error] node above in [_result2] has no value
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing one count expression to a constant
        /// </summary>
        [Test]
        public void If20 ()
        {
            var result = ExecuteLambda (@"_result
  foo1:bar1
  foo2:int:5
if:@/../*/_result/*/?count
  =:int:2
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing an expression returning a node,
        ///     to a constant node
        /// </summary>
        [Test]
        public void If21 ()
        {
            var result = ExecuteLambda (@"_result
  foo1:bar1
  foo2:int:5
if:@/../*/_result/?node
  =:node:@""_result
  foo1:bar1
  foo2:int:5""
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing an expression with 'null'
        /// </summary>
        [Test]
        public void If22 ()
        {
            var result = ExecuteLambda (@"_result:success
if:@/../*/_result/?value
  =
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing a constant with 'null'
        /// </summary>
        [Test]
        public void If23 ()
        {
            var result = ExecuteLambda (@"_result:success
if:foo
  =
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two constants with formatting expressions
        /// </summary>
        [Test]
        public void If24 ()
        {
            var result = ExecuteLambda (@"_result:error
if:{0}o
  :fo
  =:{0}o
    :fo
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two expressions with formatting expressions
        /// </summary>
        [Test]
        public void If25 ()
        {
            var result = ExecuteLambda (@"_result:error
if:@/../{0}/?value
  :0
  =:@/../{0}/?value
    :*/{0}
      :@/../0/?name
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two integers with >= and comparison should yield true
        /// </summary>
        [Test]
        public void If26 ()
        {
            var result = ExecuteLambda (@"_result:error
if:int:5
  >=:int:4
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two integers with >= and comparison should yield false
        /// </summary>
        [Test]
        public void If27 ()
        {
            var result = ExecuteLambda (@"_result:success
if:int:4
  >=:int:5
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two strings with >= and comparison should yield true
        /// </summary>
        [Test]
        public void If28 ()
        {
            var result = ExecuteLambda (@"_result:error
if:b
  >=:a
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two strings with >= and comparison should yield false
        /// </summary>
        [Test]
        public void If29 ()
        {
            var result = ExecuteLambda (@"_result:success
if:a
  >=:b
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two strings with &lt;= and comparison should yield true
        /// </summary>
        [Test]
        public void If30 ()
        {
            var result = ExecuteLambda (@"_result:error
if:a
  <=:b
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two strings with &lt;= and comparison should yield false
        /// </summary>
        [Test]
        public void If31 ()
        {
            var result = ExecuteLambda (@"_result:success
if:b
  <=:a
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two strings with != and comparison should yield true
        /// </summary>
        [Test]
        public void If32 ()
        {
            var result = ExecuteLambda (@"_result:error
if:abba
  !=:abca
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two strings with != and comparison should yield false
        /// </summary>
        [Test]
        public void If33 ()
        {
            var result = ExecuteLambda (@"_result:success
if:abba
  !=:abba
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two strings with > and comparison should yield true
        /// </summary>
        [Test]
        public void If34 ()
        {
            var result = ExecuteLambda (@"_result:error
if:b
  >:abba
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when comparing two strings with &lt; and comparison should yield true
        /// </summary>
        [Test]
        public void If35 ()
        {
            var result = ExecuteLambda (@"_result:error
if:abba
  <:bce
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when the operator is ! (NOT) and statement should yield true
        /// </summary>
        [Test]
        public void If36 ()
        {
            var result = ExecuteLambda (@"_result:error
if:!
  :@/../*/foo/?node
  lambda
    set:@/../*/_result/?value
      source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [if] works when the operator is ! (NOT) and statement should yield false
        /// </summary>
        [Test]
        public void If37 ()
        {
            var result = ExecuteLambda (@"_result:success
if:!
  :@/../*/_result/?node
  lambda
    set:@/../*/_result/?value
      source:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [else] kicks in when [if] evaluates to false
        /// </summary>
        [Test]
        public void If38 ()
        {
            var result = ExecuteLambda (@"_result
if:!
  :@/../*/_result/?node
  lambda
    set:@/../*/_result/?value
      source:error
else
  set:@/../*/_result/?value
    source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [else-if] kicks in when [if] evaluates to false
        /// </summary>
        [Test]
        public void If39 ()
        {
            var result = ExecuteLambda (@"_result
if:!
  :@/../*/_result/?node
  lambda
    set:@/../*/_result/?value
      source:error
else-if:@/../*/_result/?node
  set:@/../*/_result/?value
    source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies [else] kicks in when [if] and [else-if] evaluates to false
        /// </summary>
        [Test]
        public void If40 ()
        {
            var result = ExecuteLambda (@"_result
if:!
  :@/../*/_result/?node
  lambda
    set:@/../*/_result/?value
      source:error
else-if:@/../*/_resultXX/?node
  set:@/../*/_result/?value
    source:error
else
  set:@/../*/_result/?value
    source:success");
            Assert.AreEqual ("success", result [0].Value);
        }
    }
}