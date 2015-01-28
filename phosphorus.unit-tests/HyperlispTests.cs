
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class HyperlispTests
    {
        [Test]
        public void ParseSimpleHyperlisp ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:y";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseEmptyRootName ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
:y";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseEmptySingleLineCommentToken ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"//";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseSingleLineCommentToken ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
// comment";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseEmptyMultiLineCommentToken ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"/**/";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseMultiLineCommentToken ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"/*
comment */";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseNodesWithCommentTokens ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
// comment
jo:dude
/*comment */
hello";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (2, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseStringLiteral ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:""y""";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseStringLiteralWithEscapeCharacters ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:""\ny\\\r\n\""""";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("\r\ny\\\r\n\"", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseMultilineStringLiteral ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:@""y""";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseMultilineStringLiteralWithEscapeCharacters ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = string.Format (@"
x:@""mumbo
jumbo""""howdy\r\n{0}""", "\n");
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("mumbo\r\njumbo\"howdy\\r\\n\r\n", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseEmptyStringLiteral ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:""""";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseEmptyMultilineStringLiteral ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:@""""";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseMultipleNodes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:@""""
x2
  x3
  :x4
  :int:4
  :string:@""
howdy world
""
y:z
  z:0
    q:1
  w:2
    v:12
t:h";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("x2", tmp [1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("x3", tmp [1][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (null, tmp [1][0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (string.Empty, tmp [1][1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("x4", tmp [1][1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (string.Empty, tmp [1][2].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (4, tmp [1][2].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof(int), tmp [1][2].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (string.Empty, tmp [1][3].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("\r\nhowdy world\r\n", tmp [1][3].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (null, tmp [1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [2].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("z", tmp [2].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("z", tmp [2][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("0", tmp [2][0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("q", tmp [2][0][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("1", tmp [2][0][0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("w", tmp [2][1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("2", tmp [2][1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("v", tmp [2][1][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("12", tmp [2][1][0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("t", tmp [3].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("h", tmp [3].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseTypes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_string:string:@""""
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
_time:time:""15.23:57:53.567""
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("", tmp [1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (5, tmp [2].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (10.55F, tmp [3].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof(float), tmp [3].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (10.54D, tmp [4].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof(double), tmp [4].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof(Node), tmp [5].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("x", tmp [5].Get<Node> ().Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("z", tmp [5].Get<Node> ().Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("string:", tmp [6].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (true, tmp [7].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (Guid.Parse ("E5A53FC9-A306-4609-89E5-9CC2964DA0AC"), tmp [8].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new Node.DNA ("0-1"), tmp [9].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (-9223372036854775808L, tmp [10].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (18446744073709551615L, tmp [11].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (4294967295, tmp [12].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (-32768, tmp [13].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (456.89M, tmp [14].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (255, tmp [15].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (-128, tmp [16].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ('x', tmp [17].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new DateTime (2012, 12, 21).ToUniversalTime(), tmp [18].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new DateTime (2012, 12, 21, 23, 59, 59).ToUniversalTime(), tmp [19].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new DateTime (2012, 12, 21, 23, 59, 59, 987).ToUniversalTime(), tmp [20].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new TimeSpan (15, 23, 57, 53, 567), tmp [21].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ComplexNamesAndNonExistentType ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
""_tmp1\nthomas"":howdy
@""_tmp2"":howdy22
  @""_tmp3"":""mumbo-jumbo-type"":@""value""
  @""_tmp4
is cool"":@""mumbo-
jumbo-type"":@""value
   value""
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("_tmp1\r\nthomas", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("_tmp2", tmp [1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("howdy22", tmp [1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("_tmp3", tmp [1][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("value", tmp [1][0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("_tmp4\r\nis cool", tmp [1][1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("value\r\n   value", tmp [1][1].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseUsingExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_data:@""_foo
  tmp1
  tmp2:howdy world""
code2lambda:@/-/?value
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("_foo", tmp [1][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (1, tmp [1].Count, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (2, tmp [1][0].Count, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("tmp1", tmp [1][0][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("tmp2", tmp [1][0][1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("howdy world", tmp [1][0][1].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseUsingExpressionYieldingMultipleResults ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_data:@""_foo
  tmp1
  tmp2:howdy world""
_data:@""_foo2
  tmp12
  tmp22:howdy world2""
code2lambda:@/../*/_data/?value
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (2, tmp [2].Count, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (2, tmp [2][0].Count, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (2, tmp [2][1].Count, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("_foo", tmp [2][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("tmp1", tmp [2][0][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("tmp2", tmp [2][0][1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("howdy world", tmp [2][0][1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("_foo2", tmp [2][1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("tmp12", tmp [2][1][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("tmp22", tmp [2][1][1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("howdy world2", tmp [2][1][1].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void CreateHyperlispFromNodes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.hyperlisp.lambda2hyperlisp
  _data
    tmp1
    tmp2:howdy world
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("_data\r\n  tmp1\r\n  tmp2:howdy world", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void CreateHyperlispFromNodesUsingExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_data
  tmp1
  tmp2:howdy world
pf.hyperlisp.lambda2hyperlisp:@/-/?node
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("_data\r\n  tmp1\r\n  tmp2:howdy world", tmp [1].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void CreateHyperlispFromNodesUsingExpressionYieldingMultipleResults ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_data
  tmp1
  tmp2:howdy world
_data
  tmp12
  tmp22:howdy world2
pf.hyperlisp.lambda2hyperlisp:@/../*/_data/?node
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("_data\r\n  tmp1\r\n  tmp2:howdy world\r\n_data\r\n  tmp12\r\n  tmp22:howdy world2", tmp [2].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
 x:y"; // one space before token
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:y
 z:q"; // only one space when opening children collection
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:y
   z:q"; // three spaces when opening children collection
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }

        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:y
  z:""howdy"; // open string literal
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError5 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:y
  z:"""; // empty and open string literal
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError6 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:y
  z:@"""; // empty and open multiline string literal
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError7 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:y
  z:@""howdy"; // open multiline string literal
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError8 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:y
  z:@""howdy

"; // open multiline string literal
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError9 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
z:node:@""howdy:x
 f:g"""; // syntax error in hyperlisp node content, only one space while opening child collection of "howdy" node
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError10 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
z:node:@""howdy:x
f:g"""; // logical error in hyperlisp node content, multiple "root" nodes
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
    }
}
