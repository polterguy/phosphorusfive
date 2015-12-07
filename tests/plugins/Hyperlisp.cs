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
    ///     Unit tests for testing Hyperlisp parser
    /// </summary>
    [TestFixture]
    public class Hyperlisp : TestBase
    {
        public Hyperlisp ()
            : base ("p5.hyperlisp", "p5.types", "p5.lambda")
        { }

        /// <summary>
        ///     Parse simple name/value combination
        /// </summary>
        [Test]
        public void ParseHyperlispNameValue ()
        {
            var tmp = new Node ("", "x:y");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name);
            Assert.AreEqual ("y", tmp [0].Value);
        }

        /// <summary>
        ///     Parses name/value Hyperlisp with lots of spacing at front and at end
        /// </summary>
        [Test]
        public void ParseHyperlispTrimSpaces ()
        {
            var tmp = new Node ("", @"

x:y

");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual (1, tmp.Count);
            Assert.AreEqual ("x", tmp [0].Name);
            Assert.AreEqual ("y", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where there's no name but only value
        /// </summary>
        [Test]
        public void ParseHyperlispNoName ()
        {
            var tmp = new Node ("", ":y");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Name);
            Assert.AreEqual ("y", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where there's no name but only value prepended by some spacing
        /// </summary>
        [Test]
        public void ParseHyperlispNoNameTrim ()
        {
            var tmp = new Node ("", @"

:y");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Name);
            Assert.AreEqual ("y", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where value is a string literal
        /// </summary>
        [Test]
        public void ParseHyperlispValueString ()
        {
            var tmp = new Node {Value = @"x:""y"""};
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where value is a string literal containing CR/LF
        /// </summary>
        [Test]
        public void ParseHyperlispValueStringCRLF ()
        {
            var tmp = new Node {Value = @"x:""\ny\\\r\n\"""""};
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("\r\ny\\\r\n\"", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where value is a multiline string literal
        /// </summary>
        [Test]
        public void ParseHyperlispMultiLineStringValue ()
        {
            var tmp = new Node {Value = @"x:@""y"""};
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where value is a multiline string literal containing CR/LF, CR and
        ///     so on in different configurations
        /// </summary>
        [Test]
        public void ParseHyperlispNameValueMultiLineString ()
        {
            var tmp = new Node {Value = string.Format (@"x:@""mumbo
jumbo""""howdy\r\n{0}""", "\n")};
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("mumbo\r\njumbo\"howdy\\r\\n\r\n", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where value is an empty string literal
        /// </summary>
        [Test]
        public void ParseHyperlispEmptyValueString ()
        {
            var tmp = new Node {Value = @"x:"""""};
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where value is an empty multiline string literal
        /// </summary>
        [Test]
        public void ParseHyperlispEmptyValueMultiLineString ()
        {
            var tmp = new Node {Value = @"x:@"""""};
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp containing a lot of different types
        /// </summary>
        [Test]
        public void ParseHyperlispManyTypes ()
        {
            var tmp = new Node {Value = @"_string:string:@""""
_string:string:
_int:int:5
_float:float:10.55
_double:double:10.54
_node:node:@""x:z""
_string:""string:""
_bool:bool:true
_guid:guid:E5A53FC9-A306-4609-89E5-9CC2964DA0AC
_long:long:-9223372036854775808
_ulong:ulong:18446744073709551615
_uint:uint:4294967295
_short:short:-32768
_decimal:decimal:456.89
_byte:byte:255
_sbyte:sbyte:-128
_char:char:x
_date:date:2012-12-21
_date:date:""2012-12-21T23:59:59""
_date:date:""2012-12-21T23:59:59.987""
_time:time:""15.23:57:53.567"""};
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value);
            Assert.AreEqual ("", tmp [1].Value);
            Assert.AreEqual (5, tmp [2].Value);
            Assert.AreEqual (10.55F, tmp [3].Value);
            Assert.AreEqual (typeof (float), tmp [3].Value.GetType ());
            Assert.AreEqual (10.54D, tmp [4].Value);
            Assert.AreEqual (typeof (double), tmp [4].Value.GetType ());
            Assert.AreEqual (typeof (Node), tmp [5].Value.GetType ());
            Assert.AreEqual ("x", tmp [5].Get<Node> (Context).Name);
            Assert.AreEqual ("z", tmp [5].Get<Node> (Context).Value);
            Assert.AreEqual ("string:", tmp [6].Value);
            Assert.AreEqual (true, tmp [7].Value);
            Assert.AreEqual (Guid.Parse ("E5A53FC9-A306-4609-89E5-9CC2964DA0AC"), tmp [8].Value);
            Assert.AreEqual (-9223372036854775808L, tmp [9].Value);
            Assert.AreEqual (18446744073709551615L, tmp [10].Value);
            Assert.AreEqual (4294967295, tmp [11].Value);
            Assert.AreEqual (-32768, tmp [12].Value);
            Assert.AreEqual (456.89M, tmp [13].Value);
            Assert.AreEqual (255, tmp [14].Value);
            Assert.AreEqual (-128, tmp [15].Value);
            Assert.AreEqual ('x', tmp [16].Value);
            Assert.AreEqual (new DateTime (2012, 12, 21), tmp [17].Value);
            Assert.AreEqual (new DateTime (2012, 12, 21, 23, 59, 59), tmp [18].Value);
            Assert.AreEqual (new DateTime (2012, 12, 21, 23, 59, 59, 987), tmp [19].Value);
            Assert.AreEqual (new TimeSpan (15, 23, 57, 53, 567), tmp [20].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where value is a blob (byte[])
        /// </summary>
        [Test]
        public void ParseHyperlispWithBlob ()
        {
            var tmp = new Node ();
            tmp.Add (new Node ("_blob", new byte[] {134, 254, 12}));
            Context.RaiseNative ("lambda2lisp", tmp);
            Assert.AreEqual ("_blob:blob:hv4M", tmp.Value);
            tmp = new Node ("", tmp.Value);
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual (new byte[] {134, 254, 12}, tmp [0].Value);
        }
        
        /// <summary>
        ///     Parses Hyperlisp where parsing is done through an expression
        /// </summary>
        [Test]
        public void ParseHyperlispExpression ()
        {
            Node node = ExecuteLambda (@"_exe
  foo:bar
lambda2lisp:x:/-
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (@"_exe
  foo:bar", node [1].Value);
        }
        
        /// <summary>
        ///     Parses Hyperlisp where parsing is done through an expression leading to several nodes
        /// </summary>
        [Test]
        public void ParseHyperlispExpressionMultiResults ()
        {
            Node node = ExecuteLambda (@"_exe1
  foo1:bar1
_exe2
  foo2:bar2
lambda2lisp:x:/-2|/-
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (@"_exe1
  foo1:bar1
_exe2
  foo2:bar2", node [2].Value);
        }
        
        /// <summary>
        ///     Parses Hyperlisp where parsing is done through an expression leading to several nodes and
        ///     expression returns nodes in "reverse mode"
        /// </summary>
        [Test]
        public void ParseHyperlispExpressionReverseMultiResult ()
        {
            Node node = ExecuteLambda (@"_exe1
  foo1:bar1
_exe2
  foo2:bar2
lambda2lisp:x:/-|/-2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (@"_exe2
  foo2:bar2
_exe1
  foo1:bar1", node [2].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp containing one empty comment and no nodes
        /// </summary>
        [Test]
        public void ParseHyperlispOnlyEmptyComment ()
        {
            var tmp = new Node ("", "//");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count);
        }

        /// <summary>
        ///     Parses Hyperlisp containing one comment and no nodes
        /// </summary>
        [Test]
        public void ParseHyperlispOnlyComment ()
        {
            var tmp = new Node ("", "// comment");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count);
        }

        /// <summary>
        ///     Parses Hyperlisp containing one empty multi line comment and no nodes
        /// </summary>
        [Test]
        public void ParseHyperlispOnlyEmptyMultiLineComment ()
        {
            var tmp = new Node ("", "/**/");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count);
        }

        /// <summary>
        ///     Parses Hyperlisp containing one multi line comment and no nodes
        /// </summary>
        [Test]
        public void ParseHyperlispOnlyMultiLineComment ()
        {
            var tmp = new Node ("", "/*comment*/");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count);
        }

        /// <summary>
        ///     Parses Hyperlisp containing one multi line comment with multiple lines and no nodes
        /// </summary>
        [Test]
        public void ParseHyperlispOnlyMultiLineCommentSpacing ()
        {
            var tmp = new Node ("", @"/*
comment

*/");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count);
        }

        /// <summary>
        ///     Parses Hyperlisp containing several different types of comments and some nodes
        /// </summary>
        [Test]
        public void ParseHyperlispMultiComments ()
        {
            var tmp = new Node ("", @"// comment
jo:dude
/*comment */
hello
/*
 * foo bar
 */");
            Context.RaiseNative ("lisp2lambda", tmp);
            Assert.AreEqual (2, tmp.Count);
        }

        /// <summary>
        ///     Parses Hyperlisp where parsing is done through an expression leading to several nodes
        /// </summary>
        [Test]
        public void ParseHyperlispExpressionMultiResult ()
        {
            Node node = ExecuteLambda (@"_res1:@""_exe1
  foo1:bar1""
_res2:@""
_exe2
  foo2:bar2""
lisp2lambda:x:/-2|/-?value
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (2, node [2].Count);
            Assert.AreEqual ("_exe1", node [2] [0].Name);
            Assert.AreEqual (1, node [2] [0].Count);
            Assert.AreEqual ("bar1", node [2] [0] [0].Value);
            Assert.AreEqual ("_exe2", node [2] [1].Name);
            Assert.AreEqual (1, node [2] [1].Count);
            Assert.AreEqual ("bar2", node [2] [1] [0].Value);
        }
        
        /// <summary>
        ///     Parses Hyperlisp where parsing is done through an expression leading to several nodes and
        ///     expression returns nodes in "reverse mode"
        /// </summary>
        [Test]
        public void ParseHyperlispExpressionMultiResultsReverse ()
        {
            Node node = ExecuteLambda (@"_res2:@""_exe2
  foo2:bar2""
_res1:@""
_exe1
  foo1:bar1""
lisp2lambda:x:/-|/-2?value
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (2, node [2].Count);
            Assert.AreEqual ("_exe1", node [2] [0].Name);
            Assert.AreEqual (1, node [2] [0].Count);
            Assert.AreEqual ("bar1", node [2] [0] [0].Value);
            Assert.AreEqual ("_exe2", node [2] [1].Name);
            Assert.AreEqual (1, node [2] [1].Count);
            Assert.AreEqual ("bar2", node [2] [1] [0].Value);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is one space too much in front of name
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorWrongSpacing ()
        {
            var tmp = new Node ("", " x:y"); // one space before token
            Context.RaiseNative ("lisp2lambda", tmp);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is one space too much in front of name
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorWrongSpacingOfChildTooLittle ()
        {
            var tmp = new Node ("", @"x:y
 z:q"); // only one space when opening children collection
            Context.RaiseNative ("lisp2lambda", tmp);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is three spaces too much in front of name
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorWrongSpacingOfChildTooMuch ()
        {
            var tmp = new Node ("", @"x:y
   z:q"); // three spaces when opening children collection
            Context.RaiseNative ("lisp2lambda", tmp);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is an open string literal
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorOpenStringLiteral ()
        {
            var tmp = new Node ("", "z:\"howdy"); // open string literal
            Context.RaiseNative ("lisp2lambda", tmp);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is an empty open string literal
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorOpenEmptyStringLiteral ()
        {
            var tmp = new Node ("", @"z:"""); // empty and open string literal
            Context.RaiseNative ("lisp2lambda", tmp);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is an empty open multiline string literal
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorOpenEmptyMultiLineString ()
        {
            var tmp = new Node ("", @"z:@"""); // empty and open multiline string literal
            Context.RaiseNative ("lisp2lambda", tmp);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is an open multiline string literal
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorOpenMultiLineString ()
        {
            var tmp = new Node ("", @"z:@""howdy"); // open multiline string literal
            Context.RaiseNative ("lisp2lambda", tmp);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is an open multiline string literal on multiple lines
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorOpenMultiLineStringMultiLines ()
        {
            var tmp = new Node ("", @"z:@""howdy
qwertyuiop
                    "); // open multiline string literal
            Context.RaiseNative ("lisp2lambda", tmp);
        }

        /// <summary>
        ///     Parses Hyperlisp where there is a space too much in reference node value
        /// </summary>
        [Test]
        [ExpectedException]
        public void ParseErrorNodeValueNotValid ()
        {
            var tmp = new Node ("", @"z:node:@""howdy:x
 f:g"""); // syntax error in hyperlisp node content, only one space while opening child collection of "howdy" node
            Context.RaiseNative ("lisp2lambda", tmp);
        }
    }
}