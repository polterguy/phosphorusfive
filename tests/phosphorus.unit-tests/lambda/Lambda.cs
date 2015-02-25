/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [lambda.xxx] lambda keyword
    /// </summary>
    [TestFixture]
    public class Lambda : TestBase
    {
        public Lambda ()
            : base ("phosphorus.lambda", "phosphorus.types", "phosphorus.hyperlisp") { }

        /// <summary>
        ///     verifies that [lambda] is mutable
        /// </summary>
        [Test]
        public void Lambda01 ()
        {
            var result = ExecuteLambda (@"_data
set:@/-/?value
  source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda.immutable] is not mutable
        /// </summary>
        [Test]
        public void Lambda02 ()
        {
            var result = ExecuteLambda (@"_data:success
set:@/-/?value
  source:error", "lambda.immutable");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda.copy] is not mutable
        /// </summary>
        [Test]
        public void Lambda03 ()
        {
            var result = ExecuteLambda (@"_data:success
set:@/-/?value
  source:error", "lambda.copy");
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
  set:@/../*/_data/?value
    source:error");
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
  set:@/../*/_data/?value
    source:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda] can invoke lambda objects through expressions leading to nodes
        /// </summary>
        [Test]
        public void Lambda06 ()
        {
            var result = ExecuteLambda (@"_exe
  set:@/./?value
    source:success
lambda:@/-/?node");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda] can invoke reference nodes
        /// </summary>
        [Test]
        public void Lambda07 ()
        {
            var result = ExecuteLambda (@"_exe:node:@""_exe
  set:@/./?value
    source:success""
lambda:@/-/?value");
            Assert.AreEqual ("success", result [0].Get<Node> (Context).Value);
        }

        /// <summary>
        ///     verifies that [lambda] can pass in arguments when executing results of expressions
        /// </summary>
        [Test]
        public void Lambda08 ()
        {
            var result = ExecuteLambda (@"_exe
  set:@/./?value
    source:@/././*/_result/?value
lambda:@/-/?node
  _result:success");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     verifies that [lambda] can invoke text objects
        /// </summary>
        [Test]
        public void Lambda09 ()
        {
            var result = ExecuteLambda (@"_exe:@""set:@/../*/_result/#/?value
  source:success""
lambda:@/-/?value
  _result:node:_result"); // passing in reference node, to be able to retrieve values from lambda invocation
            Assert.AreEqual ("success", result [1] [0].Get<Node> (Context).Value);
        }

        /// <summary>
        ///     verifies that [lambda] can execute expression yielding multiple results
        /// </summary>
        [Test]
        public void Lambda10 ()
        {
            var result = ExecuteLambda (@"_exe1
  set:@/../?value
    source:succ
_exe1
  set:@/../?value
    source:{0}{1}
      :@/../?value
      :ess
lambda:@/-2/|/-1/?node");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda.single] can execute one single node
        /// </summary>
        [Test]
        public void Lambda11 ()
        {
            var result = ExecuteLambda (@"_exe1
  set:@/../?value
    source:success
lambda.single:@/-/0/?node");
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
  set:@/../?value
    source:succ
_exe1
  set:@/../?value
    source:{0}{1}
      :@/../?value
      :ess
lambda.single:@/-2/*/|/-1/*/?node");
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
  set:@/../?value
    source:succ
_exe1:error
  set:@/../?value
    source:error
_exe1
  set:@/../?value
    source:{0}{1}
      :@/../?value
      :ess
lambda.single:@/../*/(/_exe1/!/=error/)/*/?node");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda.single] will not execute anything when given a block of code
        /// </summary>
        [Test]
        public void Lambda14 ()
        {
            var result = ExecuteLambda (@"_exe1:success
  set:@/./?value
    source:error
lambda.single:@/-/?node");
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
  set:@/./?node
lambda:@/-/?node
set:@/../?value
  source:success");
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
  append:@/../?node
    source
      set:@/../?value
        source:success
lambda:@/-/?node");
            Assert.AreEqual ("success", result.Value);
        }

        /// <summary>
        ///     verifies that [lambda] can invoke text objects as values
        /// </summary>
        [Test]
        public void Lambda17 ()
        {
            var result = ExecuteLambda (@"lambda:@""set:@/../*/_result/#/?value
  source:success""
  _result:node:_result"); // passing in reference node, to be able to retrieve values from lambda invocation
            Assert.AreEqual ("success", result [0] [0].Get<Node> (Context).Value);
        }
    }
}