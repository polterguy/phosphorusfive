
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
    /// unit tests for testing the [append] lambda keyword
    /// </summary>
    [TestFixture]
    public class Branching : TestBase
    {
        public Branching ()
            : base ("phosphorus.lambda", "phosphorus.types", "phosphorus.hyperlisp")
        { }

        /// <summary>
        /// verifies [if] works when given constant
        /// </summary>
        [Test]
        public void If1 ()
        {
            Node result = ExecuteLambda (@"if:foo
  set:@/../?value
    source:success");
            Assert.AreEqual ("success", result.Value);
        }
    }
}
