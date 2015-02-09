
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    /// <summary>
    /// unit tests for testing the [lambda.xxx] lambda keyword
    /// </summary>
    [TestFixture]
    public class Lambda : TestBase
    {
        public Lambda ()
            : base ("phosphorus.lambda", "phosphorus.types", "phosphorus.hyperlisp")
        { }

        /// <summary>
        /// verifies that [lambda] is mutable
        /// </summary>
        [Test]
        public void Lambda1 ()
        {
            Node result = ExecuteLambda (@"_data
set:@/-/?value
  source:success", "lambda");
            Assert.AreEqual ("success", result [0].Value);
        }
        
        /// <summary>
        /// verifies that [lambda.immutable] is not mutable
        /// </summary>
        [Test]
        public void Lambda2 ()
        {
            Node result = ExecuteLambda (@"_data:success
set:@/-/?value
  source:error", "lambda.immutable");
            Assert.AreEqual ("success", result [0].Value);
        }
        
        /// <summary>
        /// verifies that [lambda.copy] is not mutable
        /// </summary>
        [Test]
        public void Lambda3 ()
        {
            Node result = ExecuteLambda (@"_data:success
set:@/-/?value
  source:error", "lambda.copy");
            Assert.AreEqual ("success", result [0].Value);
        }
        
        /// <summary>
        /// verifies that [lambda.copy] does not have access to nodes outside if itself
        /// </summary>
        [Test]
        public void Lambda4 ()
        {
            Node result = ExecuteLambda (@"_data:success
lambda.copy
  set:@/../*/_data/?value
    source:error", "lambda");
            Assert.AreEqual ("success", result [0].Value);
        }
        
        /// <summary>
        /// verifies that [lambda.immutable] has access to nodes outside if itself
        /// </summary>
        [Test]
        public void Lambda5 ()
        {
            Node result = ExecuteLambda (@"_data:error
lambda.immutable
  set:@/../*/_data/?value
    source:success", "lambda");
            Assert.AreEqual ("success", result [0].Value);
        }
    }
}
