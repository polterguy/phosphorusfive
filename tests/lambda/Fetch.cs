/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     Unit tests for testing the [fetch] lambda keyword
    /// </summary>
    [TestFixture]
    public class Fetch : TestBase
    {
        public Fetch ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp")
        { }

        /// <summary>
        ///     Fetches from a static source
        /// </summary>
        [Test]
        public void FetchStaticSource ()
        {
            var result = ExecuteLambda (@"_result
set:x:/-?value
  fetch
    src:x:/+/*?name
    _tmp
      _foo:bar", "eval-mutable");
            Assert.AreEqual ("_foo", result [0].Value);
        }

        /// <summary>
        ///     Fetches from the result of an executed lambda block, 
        ///     to verify lambda is evaluated before fetch [src] is evaluated
        /// </summary>
        [Test]
        public void FetchFromEvaledCode ()
        {
            var result = ExecuteLambda (@"_result
set:x:/-?value
  fetch
    src:x:/+?value
    _res
    if:x:/../0?name
      =:_result
      set:x:/..fetch/*/_res?value
        src:success", "eval-mutable");
            Assert.AreEqual ("success", result [0].Value);
        }

        /// <summary>
        ///     Fetches from the result of an Active Event
        /// </summary>
        [Test]
        public void FetchFromActiveEvent ()
        {
            var result = ExecuteLambda (@"_result
set-event:fetch.test1
  set:x:/..?value
    src:success
set:x:/../*/_result?value
  fetch
    src:x:/+?value
    fetch.test1", "eval-mutable");
            Assert.AreEqual ("success", result [0].Value);
        }
    }
}
