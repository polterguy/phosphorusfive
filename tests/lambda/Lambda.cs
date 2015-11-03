/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [lambda.xxx] lambda keyword
    /// </summary>
    [TestFixture]
    public class Lambda : TestBase
    {
        public Lambda ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp") { }

        /// <summary>
        ///     verifies that [lambda] is mutable
        /// </summary>
        [Test]
        public void Lambda01 ()
        {
            var result = ExecuteLambda (@"_data
set:x:/-?value
  src:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda.immutable] is not mutable
        /// </summary>
        [Test]
        public void Lambda02 ()
        {
            var result = ExecuteLambda (@"_data:success
set:x:/-?value
  src:error", "lambda.immutable");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda.copy] is not mutable
        /// </summary>
        [Test]
        public void Lambda03 ()
        {
            var result = ExecuteLambda (@"_data:success
set:x:/-?value
  src:error", "lambda.copy");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda.copy] does not have access to nodes outside if itself
        /// </summary>
        [Test]
        public void Lambda04 ()
        {
            var result = ExecuteLambda (@"_data:success
lambda.copy
  set:x:/../*/_data?value
    src:error");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda.immutable] has access to nodes outside if itself
        /// </summary>
        [Test]
        public void Lambda05 ()
        {
            var result = ExecuteLambda (@"_data:error
lambda.immutable
  set:x:/../*/_data?value
    src:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda] can invoke lambda objects through expressions leading to nodes
        /// </summary>
        [Test]
        public void Lambda06 ()
        {
            var result = ExecuteLambda (@"_exe
  set:x:/.?value
    src:success
lambda:x:/-");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda] can invoke reference nodes
        /// </summary>
        [Test]
        public void Lambda07 ()
        {
            var result = ExecuteLambda (@"_exe:node:@""_exe
  set:x:/.?value
    src:success""
lambda:x:/-?value");
            Assert.AreEqual ("success", result [0].Get<Node> (Context).Value);
        }

        /// <summary>
        ///     verifies that [lambda] can pass in arguments when executing results of expressions
        /// </summary>
        [Test]
        public void Lambda08 ()
        {
            var result = ExecuteLambda (@"_exe
  set:x:/.?value
    src:x:/././*/_result?value
lambda:x:/-
  _result:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda] can invoke text objects
        /// </summary>
        [Test]
        public void Lambda09 ()
        {
            var result = ExecuteLambda (@"_exe:@""set:x:/../*/_result/#?value
  src:success""
lambda:x:/-?value
  _result:node:_result"); // passing in reference node, to be able to retrieve values from lambda invocation
            Assert.AreEqual ("success", result [1] [0].Get<Node> (Context).Value);
        }

        /// <summary>
        ///     verifies that [lambda] can execute expression yielding multiple results, and that it does so consecutively in
        ///     order expression returns matches
        /// </summary>
        [Test]
        public void Lambda10 ()
        {
            var result = ExecuteLambda (@"_exe1
  set:x:/..?value
    src:succ
_exe1
  set:x:/..?value
    src:{0}{1}
      :x:/..?value
      :ess
lambda:x:/-2|/-1");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda.single] can execute one single node
        /// </summary>
        [Test]
        public void Lambda11 ()
        {
            var result = ExecuteLambda (@"_exe1
  set:x:/..?value
    src:success
lambda.single:x:/-/0");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda.single] can execute single nodes when given an
        ///     expression that returns multiple results
        /// </summary>
        [Test]
        public void Lambda12 ()
        {
            var result = ExecuteLambda (@"_exe1
  set:x:/..?value
    src:succ
_exe1
  set:x:/..?value
    src:{0}{1}
      :x:/..?value
      :ess
lambda.single:x:/-2/*|/-1/*");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda.single] can execute single nodes when given an
        ///     expression that returns multiple results, where some nodes are NOT'ed away
        /// </summary>
        [Test]
        public void Lambda13 ()
        {
            var result = ExecuteLambda (@"_exe1
  set:x:/..?value
    src:succ
_exe1:error
  set:x:/..?value
    src:error
_exe1
  set:x:/..?value
    src:{0}{1}
      :x:/..?value
      :ess
lambda.single:x:/../*(/_exe1!/=error)/*");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda.single] will not execute anything when given a block of code
        /// </summary>
        [Test]
        public void Lambda14 ()
        {
            var result = ExecuteLambda (@"_exe1:success
  set:x:/.?value
    src:error
lambda.single:x:/-");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda] can do the "delete this" pattern, without messing
        ///     up the execution pointer, making it skip an active event
        /// </summary>
        [Test]
        public void Lambda15 ()
        {
            var result = ExecuteLambda (@"_exe1
  set:x:/.
lambda:x:/-
set:x:/..?value
  src:success");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda] can create a new node, just beneath the currently executed node,
        ///     without making the engine "skip" a node
        /// </summary>
        [Test]
        public void Lambda16 ()
        {
            var result = ExecuteLambda (@"_exe1
  add:x:/..
    src
      set:x:/..?value
        src:success
lambda:x:/-");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda] can invoke text objects as values
        /// </summary>
        [Test]
        public void Lambda17 ()
        {
            var result = ExecuteLambda (@"lambda:@""set:x:/../*/_result/#?value
  src:success""
  _result:node:_result"); // passing in reference node, to be able to retrieve values from lambda invocation
            Assert.AreEqual ("success", result [0] [0].Get<Node> (Context).Value);
        }
    }
}