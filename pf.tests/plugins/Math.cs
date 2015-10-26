/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using pf.core;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace phosphorus.unittests.plugins
{
    /// <summary>
    ///     Unit tests for testing the phosphorus.math Active events ('+', '-', '*' and '/')
    /// </summary>
    [TestFixture]
    public class Math : TestBase
    {
        public Math ()
            : base ("pf.hyperlisp", "pf.types", "pf.lambda", "pf.math")
        { }

        /// <summary>
        ///     Verifies two string can be added correctly.
        /// </summary>
        [Test]
        public void Add01 ()
        {
            Node result = ExecuteLambda (@"+:2
  _:2");
            Assert.AreEqual ("22", result [0].Value);
        }
        
        /// <summary>
        ///     Verifies two integers can be added correctly.
        /// </summary>
        [Test]
        public void Add02 ()
        {
            Node result = ExecuteLambda (@"+:int:2
  _:int:2");
            Assert.AreEqual (4, result [0].Value);
        }
        
        /// <summary>
        ///     Verifies one integer and one string can be added correctly, and that string is converted to an integer.
        /// </summary>
        [Test]
        public void Add03 ()
        {
            Node result = ExecuteLambda (@"+:int:2
  _:2");
            Assert.AreEqual (4, result [0].Value);
        }
        
        /// <summary>
        ///     Verifies one decimal and one string can be added correctly, and that string is converted to a decimal.
        /// </summary>
        [Test]
        public void Add04 ()
        {
            Node result = ExecuteLambda (@"+:decimal:2
  _:2");
            Assert.AreEqual (4M, result [0].Value);
            Assert.AreEqual (typeof (decimal), result [0].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies to floats can be multiplied together.
        /// </summary>
        [Test]
        public void Multiply01 ()
        {
            Node result = ExecuteLambda (@"*:float:2
  _:float:3");
            Assert.AreEqual (6F, result [0].Value);
            Assert.AreEqual (typeof (float), result [0].Value.GetType ());
        }

        /// <summary>
        ///     Verifies one double can be divided by a string.
        /// </summary>
        [Test]
        public void Divide01 ()
        {
            Node result = ExecuteLambda (@"/:double:14
  _:2");
            Assert.AreEqual (7D, result [0].Value);
            Assert.AreEqual (typeof (double), result [0].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies one integer can calculate the modulo from a float.
        /// </summary>
        [Test]
        public void Modulo01 ()
        {
            Node result = ExecuteLambda (@"%:int:15
  _:7");
            Assert.AreEqual (1, result [0].Value);
            Assert.AreEqual (typeof (int), result [0].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies precedence of multiple operators.
        /// </summary>
        [Test]
        public void Precedence01 ()
        {
            Node result = ExecuteLambda (@"*:int:2
  +:int:5
    _:10
  _:2");
            Assert.AreEqual (60, result [0].Value);
            Assert.AreEqual (typeof (int), result [0].Value.GetType ());
        }
    }
}
