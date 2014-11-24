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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseStringLiteral ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
x:""y""";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            Loader.Instance.LoadAssembly ("phosphorus.execute");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_string:string:@""""
_string:string:
_int:int:5
_single:single:10.55
_double:double:10.54
_node:node:@""x:z""
_string:""string:""
_bool:bool:true
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
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
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            Assert.AreEqual ("_tmp1\r\nthomas", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("howdy", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("_tmp2", tmp [1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("howdy22", tmp [1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("_tmp3", tmp [1][0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("value", tmp [1][0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("_tmp4\r\nis cool", tmp [1][1].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("value\r\n   value", tmp [1][1].Value, "wrong value of node after parsing of hyperlisp");
        }
    }
}

