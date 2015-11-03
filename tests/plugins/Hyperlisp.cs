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
    ///     unit tests for tesing Hyperlisp parser
    /// </summary>
    [TestFixture]
    public class Hyperlisp : TestBase
    {
        public Hyperlisp ()
            : base ("p5.hyperlisp", "p5.types", "p5.lambda")
        { }

        /// <summary>
        ///     parses simple name/value Hyperlisp
        /// </summary>
        [Test]
        public void ParseHyperlisp01 ()
        {
            var tmp = new Node (string.Empty, "x:y");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses name/value Hyperlisp with lots of spacing in front and at end
        /// </summary>
        [Test]
        public void ParseHyperlisp02 ()
        {
            var tmp = new Node (string.Empty, @"

x:y

");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (1, tmp.Count, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where there's no name but only value
        /// </summary>
        [Test]
        public void ParseHyperlisp03 ()
        {
            var tmp = new Node (string.Empty, ":y");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where there's no name but only value prepended by some spacing
        /// </summary>
        [Test]
        public void ParseHyperlisp04 ()
        {
            var tmp = new Node (string.Empty, @"

:y");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where value is a string literal
        /// </summary>
        [Test]
        public void ParseHyperlisp05 ()
        {
            var tmp = new Node {Value = @"x:""y"""};
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where value is a string literal containing CR/LF
        /// </summary>
        [Test]
        public void ParseHyperlisp06 ()
        {
            var tmp = new Node {Value = @"x:""\ny\\\r\n\"""""};
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("\r\ny\\\r\n\"", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where value is a multiline string literal
        /// </summary>
        [Test]
        public void ParseHyperlisp07 ()
        {
            var tmp = new Node {Value = @"x:@""y"""};
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where value is a multilinbe string literal containing CR/LF, CR and
        ///     so on in different configurations
        /// </summary>
        [Test]
        public void ParseHyperlisp08 ()
        {
            var tmp = new Node {Value = string.Format (@"x:@""mumbo
jumbo""""howdy\r\n{0}""", "\n")};
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("mumbo\r\njumbo\"howdy\\r\\n\r\n", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where value is an empty string literal
        /// </summary>
        [Test]
        public void ParseHyperlisp09 ()
        {
            var tmp = new Node {Value = @"x:"""""};
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where value is an empty multiline string literal
        /// </summary>
        [Test]
        public void ParseHyperlisp10 ()
        {
            var tmp = new Node {Value = @"x:@"""""};
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp containing a lot of different types
        /// </summary>
        [Test]
        public void ParseHyperlisp11 ()
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
_dna:path:0-1
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
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("", tmp [1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (5, tmp [2].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (10.55F, tmp [3].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof (float), tmp [3].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (10.54D, tmp [4].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof (double), tmp [4].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof (Node), tmp [5].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("x", tmp [5].Get<Node> (Context).Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("z", tmp [5].Get<Node> (Context).Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("string:", tmp [6].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (true, tmp [7].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (Guid.Parse ("E5A53FC9-A306-4609-89E5-9CC2964DA0AC"), tmp [8].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new Node.Dna ("0-1"), tmp [9].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (-9223372036854775808L, tmp [10].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (18446744073709551615L, tmp [11].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (4294967295, tmp [12].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (-32768, tmp [13].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (456.89M, tmp [14].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (255, tmp [15].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (-128, tmp [16].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ('x', tmp [17].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new DateTime (2012, 12, 21), tmp [18].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new DateTime (2012, 12, 21, 23, 59, 59), tmp [19].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new DateTime (2012, 12, 21, 23, 59, 59, 987), tmp [20].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new TimeSpan (15, 23, 57, 53, 567), tmp [21].Value, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp where value is a blob (byte[])
        /// </summary>
        [Test]
        public void ParseHyperlisp12 ()
        {
            var tmp = new Node ();
            tmp.Add (new Node ("_blob", new byte[] {134, 254, 12}));
            Context.Raise ("p5.hyperlisp.lambda2hyperlisp", tmp);
            Assert.AreEqual ("_blob:blob:hv4M", tmp.Value, "wrong value of node after parsing of hyperlisp");
            tmp = new Node (string.Empty, tmp.Value);
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (new byte[] {134, 254, 12}, tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        /// <summary>
        ///     parses Hyperlisp where parsing is done through an expression
        /// </summary>
        [Test]
        public void ParseHyperlisp13 ()
        {
            Node node = ExecuteLambda (@"_exe
  foo:bar
p5.hyperlisp.lambda2hyperlisp:x:/-");
            Assert.AreEqual (@"_exe
  foo:bar", node [1].Value);
        }
        
        /// <summary>
        ///     parses Hyperlisp where parsing is done through an expression leading to several nodes
        /// </summary>
        [Test]
        public void ParseHyperlisp14 ()
        {
            Node node = ExecuteLambda (@"_exe1
  foo1:bar1
_exe2
  foo2:bar2
p5.hyperlisp.lambda2hyperlisp:x:/-2|/-");
            Assert.AreEqual (@"_exe1
  foo1:bar1
_exe2
  foo2:bar2", node [2].Value);
        }
        
        /// <summary>
        ///     parses Hyperlisp where parsing is done through an expression leading to several nodes and
        ///     expression returns nodes in "reverse mode"
        /// </summary>
        [Test]
        public void ParseHyperlisp15 ()
        {
            Node node = ExecuteLambda (@"_exe1
  foo1:bar1
_exe2
  foo2:bar2
p5.hyperlisp.lambda2hyperlisp:x:/-|/-2");
            Assert.AreEqual (@"_exe2
  foo2:bar2
_exe1
  foo1:bar1", node [2].Value);
        }

        /// <summary>
        ///     parses Hyperlisp containing one empty comment and no nodes
        /// </summary>
        [Test]
        public void ParseComment1 ()
        {
            var tmp = new Node (string.Empty, "//");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp containing one comment and no nodes
        /// </summary>
        [Test]
        public void ParseComment2 ()
        {
            var tmp = new Node (string.Empty, "// comment");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp containing one empty multi line comment and no nodes
        /// </summary>
        [Test]
        public void ParseComment3 ()
        {
            var tmp = new Node (string.Empty, "/**/");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp containing one multi line comment and no nodes
        /// </summary>
        [Test]
        public void ParseComment4 ()
        {
            var tmp = new Node (string.Empty, "/*comment*/");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp containing one multi line comment with multiple lines and no nodes
        /// </summary>
        [Test]
        public void ParseComment5 ()
        {
            var tmp = new Node (string.Empty, @"/*
comment

*/");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        /// <summary>
        ///     parses Hyperlisp containing several different types of comments and some nodes
        /// </summary>
        [Test]
        public void ParseComment6 ()
        {
            var tmp = new Node (string.Empty, @"// comment
jo:dude
/*comment */
hello");
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (2, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }
        
        /// <summary>
        ///     parses Hyperlisp where parsing is done through an expression leading to several nodes and
        ///     expression returns nodes in "reverse mode"
        /// </summary>
        [Test]
        public void Hyperlisp2Lambda01 ()
        {
            Node node = ExecuteLambda (@"_res:@""_exe1
  foo1:bar1
_exe2
  foo2:bar2""
p5.hyperlisp.hyperlisp2lambda:x:/-?value");
            Assert.AreEqual (2, node [1].Count);
            Assert.AreEqual ("_exe1", node [1] [0].Name);
            Assert.AreEqual (1, node [1] [0].Count);
            Assert.AreEqual ("bar1", node [1] [0] [0].Value);
            Assert.AreEqual ("_exe2", node [1] [1].Name);
            Assert.AreEqual (1, node [1] [1].Count);
            Assert.AreEqual ("bar2", node [1] [1] [0].Value);
        }
        
        /// <summary>
        ///     parses Hyperlisp where parsing is done through an expression leading to several nodes z
        /// </summary>
        [Test]
        public void Hyperlisp2Lambda02 ()
        {
            Node node = ExecuteLambda (@"_res1:@""_exe1
  foo1:bar1""
_res2:@""
_exe2
  foo2:bar2""
p5.hyperlisp.hyperlisp2lambda:x:/-2|/-?value");
            Assert.AreEqual (2, node [2].Count);
            Assert.AreEqual ("_exe1", node [2] [0].Name);
            Assert.AreEqual (1, node [2] [0].Count);
            Assert.AreEqual ("bar1", node [2] [0] [0].Value);
            Assert.AreEqual ("_exe2", node [2] [1].Name);
            Assert.AreEqual (1, node [2] [1].Count);
            Assert.AreEqual ("bar2", node [2] [1] [0].Value);
        }
        
        /// <summary>
        ///     parses Hyperlisp where parsing is done through an expression leading to several nodes and
        ///     expression returns nodes in "reverse mode"
        /// </summary>
        [Test]
        public void Hyperlisp2Lambda03 ()
        {
            Node node = ExecuteLambda (@"_res2:@""_exe2
  foo2:bar2""
_res1:@""
_exe1
  foo1:bar1""
p5.hyperlisp.hyperlisp2lambda:x:/-|/-2?value");
            Assert.AreEqual (2, node [2].Count);
            Assert.AreEqual ("_exe1", node [2] [0].Name);
            Assert.AreEqual (1, node [2] [0].Count);
            Assert.AreEqual ("bar1", node [2] [0] [0].Value);
            Assert.AreEqual ("_exe2", node [2] [1].Name);
            Assert.AreEqual (1, node [2] [1].Count);
            Assert.AreEqual ("bar2", node [2] [1] [0].Value);
        }

        /// <summary>
        ///     parses Hyperlisp where there is one space too much in front of name
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError01 ()
        {
            var tmp = new Node (string.Empty, " x:y"); // one space before token
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }

        /// <summary>
        ///     parses Hyperlisp where there is one space too much in front of name
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError02 ()
        {
            var tmp = new Node (string.Empty, @"x:y
 z:q"); // only one space when opening children collection
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }

        /// <summary>
        ///     parses Hyperlisp where there is three spaces too much in front of name
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError03 ()
        {
            var tmp = new Node (string.Empty, @"x:y
   z:q"); // three spaces when opening children collection
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }

        /// <summary>
        ///     parses Hyperlisp where there is an open string literal
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError04 ()
        {
            var tmp = new Node (string.Empty, "z:\"howdy"); // open string literal
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }

        /// <summary>
        ///     parses Hyperlisp where there is an empty open string literal
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError05 ()
        {
            var tmp = new Node (string.Empty, @"z:"""); // empty and open string literal
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }

        /// <summary>
        ///     parses Hyperlisp where there is an empty open multiline string literal
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError06 ()
        {
            var tmp = new Node (string.Empty, @"z:@"""); // empty and open multiline string literal
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }

        /// <summary>
        ///     parses Hyperlisp where there is an open multiline string literal
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError07 ()
        {
            var tmp = new Node (string.Empty, @"z:@""howdy"); // open multiline string literal
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }

        /// <summary>
        ///     parses Hyperlisp where there is an open multiline string literal on multiple lines
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError08 ()
        {
            var tmp = new Node (string.Empty, @"z:@""howdy
qwertyuiop
                    "); // open multiline string literal
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }

        /// <summary>
        ///     parses Hyperlisp where there is a space too much in reference node value
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError09 ()
        {
            var tmp = new Node (string.Empty, @"z:node:@""howdy:x
 f:g"""); // syntax error in hyperlisp node content, only one space while opening child collection of "howdy" node
            Context.Raise ("p5.hyperlisp.hyperlisp2lambda", tmp);
        }
    }
}