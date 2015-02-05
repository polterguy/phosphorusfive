
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
    public class Hyperlisp
    {
        private ApplicationContext _context;

        public Hyperlisp ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.types");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void ParseHyperlisp1 ()
        {
            Node tmp = new Node (string.Empty, "x:y");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp2 ()
        {
            Node tmp = new Node (string.Empty, @"

x:y

");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (1, tmp.Count, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp3 ()
        {
            Node tmp = new Node (string.Empty, ":y");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseHyperlisp4 ()
        {
            Node tmp = new Node (string.Empty, @"

:y");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp5 ()
        {
            Node tmp = new Node ();
            tmp.Value = @"x:""y""";
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp6 ()
        {
            Node tmp = new Node ();
            tmp.Value = @"x:""\ny\\\r\n\""""";
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("\r\ny\\\r\n\"", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp7 ()
        {
            Node tmp = new Node ();
            tmp.Value = @"x:@""y""";
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp8 ()
        {
            Node tmp = new Node ();
            tmp.Value = string.Format (@"x:@""mumbo
jumbo""""howdy\r\n{0}""", "\n");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("mumbo\r\njumbo\"howdy\\r\\n\r\n", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp9 ()
        {
            Node tmp = new Node ();
            tmp.Value = @"x:""""";
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp10 ()
        {
            Node tmp = new Node ();
            tmp.Value = @"x:@""""";
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseHyperlisp11 ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_string:string:@""""
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
_time:time:""15.23:57:53.567""";
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual ("", tmp [0].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("", tmp [1].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (5, tmp [2].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (10.55F, tmp [3].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof(float), tmp [3].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (10.54D, tmp [4].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof(double), tmp [4].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (typeof(Node), tmp [5].Value.GetType (), "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("x", tmp [5].Get<Node> (_context).Name, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual ("z", tmp [5].Get<Node> (_context).Value, "wrong value of node after parsing of hyperlisp");
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
            Assert.AreEqual (new DateTime (2012, 12, 21), tmp [18].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new DateTime (2012, 12, 21, 23, 59, 59), tmp [19].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new DateTime (2012, 12, 21, 23, 59, 59, 987), tmp [20].Value, "wrong value of node after parsing of hyperlisp");
            Assert.AreEqual (new TimeSpan (15, 23, 57, 53, 567), tmp [21].Value, "wrong value of node after parsing of hyperlisp");
        }
        
        [Test]
        public void ParseHyperlisp12 ()
        {
            Node tmp = new Node ();
            tmp.Add (new Node ("_blob", new byte [] { 134, 254, 12 }));
            _context.Raise ("pf.hyperlisp.lambda2hyperlisp", tmp);
            Assert.AreEqual ("_blob:blob:hv4M", tmp.Value, "wrong value of node after parsing of hyperlisp");
            tmp = new Node (string.Empty, tmp.Value);
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (new byte [] { 134, 254, 12 }, tmp [0].Value, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            Node tmp = new Node (string.Empty, " x:y"); // one space before token
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            Node tmp = new Node (string.Empty, @"x:y
 z:q"); // only one space when opening children collection
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            Node tmp = new Node (string.Empty, @"x:y
   z:q"); // three spaces when opening children collection
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }

        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            Node tmp = new Node (string.Empty, "z:\"howdy"); // open string literal
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError5 ()
        {
            Node tmp = new Node (string.Empty, @"z:"""); // empty and open string literal
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError6 ()
        {
            Node tmp = new Node (string.Empty, @"z:@"""); // empty and open multiline string literal
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError7 ()
        {
            Node tmp = new Node (string.Empty, @"z:@""howdy"); // open multiline string literal
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError8 ()
        {
            Node tmp = new Node (string.Empty, @"z:@""howdy
qwertyuiop
                    "); // open multiline string literal
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError9 ()
        {
            Node tmp = new Node (string.Empty, @"z:node:@""howdy:x
 f:g"""); // syntax error in hyperlisp node content, only one space while opening child collection of "howdy" node
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError10 ()
        {
            Node tmp = new Node (string.Empty, @"z:node:@""howdy:x
f:g"""); // logical error in hyperlisp node content, multiple "root" nodes
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
        }

        [Test]
        public void ParseComment1 ()
        {
            Node tmp = new Node (string.Empty, "//");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseComment2 ()
        {
            Node tmp = new Node (string.Empty, "// comment");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseComment3 ()
        {
            Node tmp = new Node (string.Empty, "/**/");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseComment4 ()
        {
            Node tmp = new Node (string.Empty, "/*comment*/");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseComment5 ()
        {
            Node tmp = new Node (string.Empty, @"/*
comment

*/");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (0, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }

        [Test]
        public void ParseComment6 ()
        {
            Node tmp = new Node (string.Empty, @"// comment
jo:dude
/*comment */
hello");
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            Assert.AreEqual (2, tmp.Count, "wrong value of node after parsing of hyperlisp");
        }
    }
}
