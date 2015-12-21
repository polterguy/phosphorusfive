/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using p5.core;

namespace p5.unittests.plugins
{
    /// <summary>
    ///     Unit tests for testing the phosphorus.math Active events ('+', '-', '*' and '/')
    /// </summary>
    [TestFixture]
    public class Math : TestBase
    {
        public Math ()
            : base ("p5.hyperlisp", "p5.types", "p5.lambda", "p5.math")
        { }

        /// <summary>
        ///     Verifies two string can be added correctly.
        /// </summary>
        [Test]
        public void AddTwoStrings ()
        {
            Node result = ExecuteLambda (@"+:2
  _:2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual ("22", result [0].Value);
        }
        
        /// <summary>
        ///     Verifies two integers can be added correctly.
        /// </summary>
        [Test]
        public void AddTwoIntegers ()
        {
            Node result = ExecuteLambda (@"+:int:2
  _:int:2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (4, result [0].Value);
        }
        
        /// <summary>
        ///     Verifies one integer and one string can be added correctly, and that string is converted to an integer.
        /// </summary>
        [Test]
        public void AddIntegerAndString ()
        {
            Node result = ExecuteLambda (@"+:int:2
  _:2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (4, result [0].Value);
        }
        
        /// <summary>
        ///     Verifies one decimal and one string can be added correctly, and that string is converted to a decimal.
        /// </summary>
        [Test]
        public void AddDecimalAndString ()
        {
            Node result = ExecuteLambda (@"+:decimal:2
  _:2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (4M, result [0].Value);
            Assert.AreEqual (typeof (decimal), result [0].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies one string and one decimal can be added correctly, and that string is converted to a decimal.
        /// </summary>
        [Test]
        public void AddStringAndDecimal ()
        {
            Node result = ExecuteLambda (@"_val:2
+:decimal:2
  _:x:/../0?value
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (4M, result [1].Value);
            Assert.AreEqual (typeof (decimal), result [1].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies one decimal and one string can be added correctly using expressions, 
        ///     and that string is converted to a decimal.
        /// </summary>
        [Test]
        public void AddTwoStringsConvertedToDecimalExpressions ()
        {
            Node result = ExecuteLambda (@"_val1:2
_val2:2
+:x:/../0?value.decimal
  _:x:/../1?value.decimal
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (4M, result [2].Value);
            Assert.AreEqual (typeof (decimal), result [2].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies one integer, one string and one decimal can be added correctly, and that result is converted to a integer.
        /// </summary>
        [Test]
        public void AddIntStringDecimal ()
        {
            Node result = ExecuteLambda (@"+:int:1
  _:1
  _:decimal:1
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (3, result [0].Value);
            Assert.AreEqual (typeof (int), result [0].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies one integer, one string and one decimal can be subtracted correctly, 
        ///     and that result becomes integer
        /// </summary>
        [Test]
        public void SubtractIntStringDecimal ()
        {
            Node result = ExecuteLambda (@"-:int:5
  _:1
  _:decimal:2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (2, result [0].Value);
            Assert.AreEqual (typeof (int), result [0].Value.GetType ());
        }

        /// <summary>
        ///     Verifies one string and one integer can be added correctly, and that string is converted to a string
        /// </summary>
        [Test]
        public void AddStringInt ()
        {
            Node result = ExecuteLambda (@"+:1
  _:int:1
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual ("11", result [0].Value);
        }

        /// <summary>
        ///     Verifies to floats can be multiplied together.
        /// </summary>
        [Test]
        public void MultiplyTwoFloats ()
        {
            Node result = ExecuteLambda (@"*:float:2
  _:float:3
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (6F, result [0].Value);
            Assert.AreEqual (typeof (float), result [0].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies one float and one string can be multiplied together.
        /// </summary>
        [Test]
        public void MultiplyFloatString ()
        {
            Node result = ExecuteLambda (@"*:float:2
  _:3
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (6F, result [0].Value);
            Assert.AreEqual (typeof (float), result [0].Value.GetType ());
        }

        /// <summary>
        ///     Verifies one double can be divided by a string.
        /// </summary>
        [Test]
        public void DivideDoubleString ()
        {
            Node result = ExecuteLambda (@"/:double:14
  _:2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (7D, result [0].Value);
            Assert.AreEqual (typeof (double), result [0].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies one double can be divided by a string using an expression
        /// </summary>
        [Test]
        public void DivideDoubleStringExpressions ()
        {
            Node result = ExecuteLambda (@"_x:2
/:double:14
  _:x:/../0?value
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (7D, result [1].Value);
            Assert.AreEqual (typeof (double), result [1].Value.GetType ());
        }

        /// <summary>
        ///     Verifies one integer can calculate the modulo from a string
        /// </summary>
        [Test]
        public void ModuloIntString ()
        {
            Node result = ExecuteLambda (@"%:int:15
  _:7
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (1, result [0].Value);
            Assert.AreEqual (typeof (int), result [0].Value.GetType ());
        }
        
        /// <summary>
        ///     Verifies precedence of multiple operators.
        /// </summary>
        [Test]
        public void PresedenceOfScope ()
        {
            Node result = ExecuteLambda (@"*:int:2
  +:int:5
    _:10
  _:2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (60, result [0].Value);
            Assert.AreEqual (typeof (int), result [0].Value.GetType ());
        }

        [ActiveEvent (Name = "p5.math.test-av", Protection = EventProtection.LambdaClosed)]
        public static void p5_math_test_av (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = 5;
        }
        
        /// <summary>
        ///     Verifies source is Active Event
        /// </summary>
        [Test]
        public void AddWithActiveEventValueResult ()
        {
            Node result = ExecuteLambda (@"*:int:2
  p5.math.test-av
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (10, result [0].Value);
            Assert.AreEqual (typeof (int), result [0].Value.GetType ());
        }
    }
}
