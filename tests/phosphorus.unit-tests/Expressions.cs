
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.unittests
{
    /// <summary>
    /// expressions unit tests, tests all sorts of different expressions
    /// and verify they work as expected
    /// </summary>
    [TestFixture]
    public class Expressions : TestBase
    {
        public Expressions ()
            : base ("phosphorus.types", "phosphorus.hyperlisp", "phosphorus.lambda")
        { }

        /// <summary>
        /// verifies that expressions are defined as such correctly
        /// </summary>
        /// <returns><c>true</c> if this instance is expression; otherwise, <c>false</c>.</returns>
        [Test]
        public void IsExpression ()
        {
            bool notExp = XUtil.IsExpression ("mumbo jumbo");
            bool isExp = XUtil.IsExpression ("@/x/?value");
            Assert.AreEqual (false, notExp);
            Assert.AreEqual (true, isExp);
        }

        /// <summary>
        /// verifies a simple 'value' expression works correctly
        /// </summary>
        [Test]
        public void ValueExpression ()
        {
            Node node = new Node ("root", "success");
            var match = Expression.Create ("@?value").Evaluate (node, _context);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }

        /// <summary>
        /// verifies a simple 'name' expression works correctly
        /// </summary>
        [Test]
        public void NameExpression ()
        {
            Node node = new Node ("success");
            var match = Expression.Create ("@?name").Evaluate (node, _context);
            Assert.AreEqual (match.Count, 1);
            Assert.AreEqual ("success", match [0].Value);
        }

        /// <summary>
        /// verifies a simple 'count' expression works correctly
        /// </summary>
        [Test]
        public void CountExpression ()
        {
            Node node = new Node ("root");
            var match = Expression.Create ("@?count").Evaluate (node, _context);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.count, match.TypeOfMatch);
        }

        /// <summary>
        /// verifies a simple 'path' expression works correctly
        /// </summary>
        [Test]
        public void PathExpression ()
        {
            Node node = new Node ("root");
            var match = Expression.Create ("@?path").Evaluate (node, _context);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (new Node.DNA (""), match [0].Value);
        }

        /// <summary>
        /// verifies a simple 'node' expression works correctly
        /// </summary>
        [Test]
        public void NodeExpression ()
        {
            Node node = new Node ("root");
            var match = Expression.Create ("@?node").Evaluate (node, _context);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (node, match [0].Value);
        }

        /// <summary>
        /// verifies IsFormatted from XUtil works corectly
        /// </summary>
        /// <returns><c>true</c> if this instance is formatted; otherwise, <c>false</c>.</returns>
        [Test]
        public void IsFormatted ()
        {
            Node node = new Node ("root", "{0}{1}")
                .Add ("", "su")
                .Add ("x", "error")
                .Add ("", "ccess");
            Assert.AreEqual (true, XUtil.IsFormatted (node));

            node = new Node ("root", "{0}{1}")
                .Add ("x")
                .Add ("y", "error")
                .Add ("z");
            Assert.AreEqual (false, XUtil.IsFormatted (node));
        }

        /// <summary>
        /// verifies formatting a node using XUtil works correctly
        /// </summary>
        [Test]
        public void Format1 ()
        {
            Node node = new Node ("root", "{0}{1}")
                .Add ("", "su")
                .Add ("x", "error")
                .Add ("", "ccess");
            string value = XUtil.FormatNode (node, _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies formatting a node with an explicit data source node using XUtil works correctly
        /// </summary>
        [Test]
        public void Format2 ()
        {
            Node node = new Node ("root", "{0}cc{1}")
                .Add ("", "@/*/_first/?value")
                .Add ("", "@/*/_second/?value")
                .Add ("_source").LastChild
                    .Add ("_first", "su")
                    .Add ("x", "error")
                    .Add ("_second", "ess").Root;

            // notice that data source node and formatting nodes are different here ...
            string value = XUtil.FormatNode (node, node [2], _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single01 ()
        {
            Node node = new Node ("root", "success");
            string value = XUtil.Single<string> (node, _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single02 ()
        {
            Node node = new Node ("root")
                .Add ("", "su")
                .Add ("", "cc")
                .Add ("", "ess");
            string value = XUtil.Single<string> ("@/*/?value", node, _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single03 ()
        {
            Node node = new Node ("root", "@/*/?value")
                .Add ("", "su")
                .Add ("", "cc")
                .Add ("", "ess");
            string value = XUtil.Single<string> (node, _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single04 ()
        {
            Node node = new Node ("root", "{0}")
                .Add ("", "success");
            string value = XUtil.Single<string> (node, _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single05 ()
        {
            Node node = new Node ("root", "{0}")
                .Add ("", "@/0/?name").LastChild
                    .Add ("success").Root;
            string value = XUtil.Single<string> (node, _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single06 ()
        {
            Node node = new Node ("root", "{0}")
                .Add ("", "@/0/?name").LastChild
                    .Add ("success").Root;
            string value = XUtil.Single<string> (node, node [0], _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single07 ()
        {
            Node node = new Node ("root", "success")
                .Add ("", "error");

            // making sure first node is used for finding single value
            string value = XUtil.Single<string> (node, node [0], _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single08 ()
        {
            Node node = new Node ("root")
                .Add ("1")
                .Add ("2")
                .Add ("3");
            int value = XUtil.Single<int> ("@/*/?name", node, _context);
            Assert.AreEqual (123, value);
        }

        /// <summary>
        /// verifies Single from XUtil works correctly
        /// </summary>
        [Test]
        public void Single09 ()
        {
            Node node = new Node ("root")
                .Add ("", 1)
                .Add ("", 2)
                .Add ("", 3);
            int value = XUtil.Single<int> ("@/*/?value", node, _context);
            Assert.AreEqual (123, value);
        }

        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate01 ()
        {
            Node node = new Node ("root")
                .Add ("", "su")
                .Add ("", "cc")
                .Add ("", "ess");
            string value = null;
            foreach (var idx in XUtil.Iterate<string> ("@/*/?value", node, _context)) {
                value += idx;
            }
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate02 ()
        {
            Node node = new Node ("root", "@/*/?value")
                .Add ("", "su")
                .Add ("", "cc")
                .Add ("", "ess");
            string value = null;
            foreach (var idx in XUtil.Iterate<string> (node, _context)) {
                value += idx;
            }
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate03 ()
        {
            Node node = new Node ("root", "success")
                .Add ("", "error");
            string value = null;

            // making sure first node is used for evaluating iteration
            foreach (var idx in XUtil.Iterate<string> (node, node [0], _context)) {
                value += idx;
            }
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate04 ()
        {
            Node node = new Node ("root", "@{0}/?value")
                .Add ("", "@/0/?name").LastChild
                    .Add ("/0", "success").Root;
            string value = null;

            // making sure second node is used as data source
            foreach (var idx in XUtil.Iterate<string> (node, node [0], _context)) {
                value += idx;
            }
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate05 ()
        {
            Node node = new Node ("root", "@{0}/?value")
                .Add ("", "@/0/?name").LastChild
                    .Add ("/0/0", "success").Root;
            string value = null;

            // making sure first node is used as data source
            foreach (var idx in XUtil.Iterate<string> (node, _context)) {
                value += idx;
            }
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate06 ()
        {
            Node node = new Node ("root")
                .Add ("", 1)
                .Add ("", 2)
                .Add ("", 3);
            string value = null;
            foreach (var idx in XUtil.Iterate<string> ("@/*/?value", node, _context)) {
                value += idx;
            }
            Assert.AreEqual ("123", value);
        }

        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate07 ()
        {
            Node node = new Node ("root")
                .Add ("", 1)
                .Add ("", 2)
                .Add ("", 3);
            int value = 0;
            foreach (var idx in XUtil.Iterate ("@/*/?value", node, _context)) {
                value += Utilities.Convert<int> (idx.Value, _context);
            }
            Assert.AreEqual (6, value);
        }
        
        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate08 ()
        {
            Node node = new Node ("root")
                .Add ("", 1)
                .Add ("", 2)
                .Add ("", 3);
            int value = 0;
            foreach (var idx in XUtil.Iterate<int> (node, _context)) {
                value += idx;
            }
            Assert.AreEqual (6, value);
        }
        
        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate09 ()
        {
            Node node = new Node ("root")
                .Add ("", 1)
                .Add ("", 2)
                .Add ("", 3);
            int value = 0;
            foreach (var idx in XUtil.Iterate<Node> (node, _context)) {
                value += idx.Get<int> (_context);
            }
            Assert.AreEqual (6, value);
        }
        
        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate10 ()
        {
            Node node = new Node ("root", "succ{0}")
                .Add (string.Empty, "ess");
            string value = "";
            foreach (var idx in XUtil.Iterate<string> (node, _context)) {
                value += idx;
            }
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies Iterate from XUtil works correctly
        /// </summary>
        [Test]
        public void Iterate11 ()
        {
            Node node = new Node ("root", "succ{0}")
                .Add (string.Empty, "ess:int:5");
            string value = "";

            // string should be converted into a Node
            foreach (var idx in XUtil.Iterate<Node> (node, _context)) {
                value += Utilities.Convert<string> (idx, _context);
                Assert.AreEqual (5, idx.Value);
            }
            Assert.AreEqual ("success:int:5", value);
        }

        /// <summary>
        /// verifies root expressions works correctly
        /// </summary>
        [Test]
        public void RootExpression()
        {
            Node node = new Node ("success")
                .Add ("");
            string value = XUtil.Single<string> ("@/../?name", node [0], _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies children retrieval expressions works correctly
        /// </summary>
        [Test]
        public void ChildrenExpression()
        {
            Node node = new Node ("")
                .Add ("su")
                .Add ("ccess");
            string value = XUtil.Single<string> ("@/*/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies descendants retrieval expressions works correctly
        /// </summary>
        [Test]
        public void DescendantsExpression()
        {
            Node node = new Node ("")
                .Add ("su").LastChild
                    .Add ("cc").LastChild
                        .Add ("es").Root.FirstChild
                .Add ("s");
            string value = XUtil.Single<string> ("@/**/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies retrieve ancestor expressions works correctly
        /// </summary>
        [Test]
        public void AncestorExpression()
        {
            Node node = new Node ("")
                .Add ("_start").LastChild
                    .Add ("su").LastChild
                        .Add ("cc").LastChild
                            .Add ("es").Root.FirstChild
                        .Add ("s").Root
                .Add ("error");
            string value = XUtil.Single<string> ("@/.._start/*/**/?name", node [0] [0], _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies retrieve sibling expressions works correctly
        /// </summary>
        [Test]
        public void SiblingExpressions()
        {
            Node node = new Node ("")
                .Add ("success")
                .Add ("error");
            string value = XUtil.Single<string> ("@/-/?name", node [1], _context);
            Assert.AreEqual ("success", value);

            node = new Node ("")
                .Add ("error")
                .Add ("success");
            value = XUtil.Single<string> ("@/+/?name", node [0], _context);
            Assert.AreEqual ("success", value);
            
            node = new Node ("")
                .Add ("error")
                .Add ("error")
                .Add ("success");
            value = XUtil.Single<string> ("@/+2/?name", node [0], _context);
            Assert.AreEqual ("success", value);
            
            node = new Node ("")
                .Add ("success")
                .Add ("error")
                .Add ("error");
            value = XUtil.Single<string> ("@/-2/?name", node [2], _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies named expressions works correctly
        /// </summary>
        [Test]
        public void NameEqualsExpression ()
        {
            Node node = new Node ("root")
                .Add ("success")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/success/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies valued expressions works correctly
        /// </summary>
        [Test]
        public void ValueEqualsExpression ()
        {
            Node node = new Node ("root")
                .Add ("success", "query");
            string value = XUtil.Single<string> ("@/*/=query/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies valued expressions works correctly with types
        /// </summary>
        [Test]
        public void ValueTypeExpression1 ()
        {
            Node node = new Node ("root")
                .Add ("success", 5);
            string value = XUtil.Single<string> ("@/*/=:int:5/?name", node, _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies valued expressions works correctly with types
        /// </summary>
        [Test]
        public void ValueTypeExpression2 ()
        {
            Node node = new Node ("root")
                .Add ("error", "5");
            string value = XUtil.Single<string> ("@/*/=:int:5/?name", node, _context);
            Assert.IsNull (value);
        }

        /// <summary>
        /// verifies valued expressions works correctly with types
        /// </summary>
        [Test]
        public void ValueTypeExpression3 ()
        {
            Node node = new Node ("root")
                .Add ("error", 5);
            string value = XUtil.Single<string> ("@/*/=5/?name", node, _context);
            Assert.IsNull (value);
        }

        /// <summary>
        /// verifies numbered child expressions works correctly
        /// </summary>
        [Test]
        public void NumberedExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("success");
            string value = XUtil.Single<string> ("@/1/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies range expressions works correctly
        /// </summary>
        [Test]
        public void RangeExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("su")
                .Add ("cc")
                .Add ("e")
                .Add ("ss")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/[1,5]/?name", node, _context);
            Assert.AreEqual ("success", value);

            node = new Node ("root")
                .Add ("error")
                .Add ("su")
                .Add ("cc")
                .Add ("e")
                .Add ("ss");
            value = XUtil.Single<string> ("@/*/[1,]/?name", node, _context);
            Assert.AreEqual ("success", value);
            
            node = new Node ("root")
                .Add ("su")
                .Add ("cc")
                .Add ("e")
                .Add ("ss")
                .Add ("error");
            value = XUtil.Single<string> ("@/*/[,4]/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies retrieve reference node expressions works correctly
        /// </summary>
        [Test]
        public void ReferenceExpression ()
        {
            Node node = new Node ("root")
                .Add ("_1")
                .Add ("_2", new Node ("_value", "success"));
            string value = XUtil.Single<string> ("@/1/#/?value", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies retrieve parent expressions works correctly
        /// </summary>
        [Test]
        public void ParentExpression ()
        {
            Node node = new Node ("success")
                .Add ("error")
                .Add ("error");
            string value = XUtil.Single<string> ("@/./?name", node [1], _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies named regex expressions works correctly
        /// </summary>
        [Test]
        public void NamedRegexExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("success");
            string value = XUtil.Single<string> (@"@/*/""/s/""/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies valued regex expressions works correctly
        /// </summary>
        [Test]
        public void ValuedRegexExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("success", "val");
            string value = XUtil.Single<string> (@"@/*/=""/val/""/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies modulo expressions works correctly
        /// </summary>
        [Test]
        public void ModuloExpression ()
        {
            Node node = new Node ("root")
                .Add ("succ")
                .Add ("error")
                .Add ("ess");
            string value = XUtil.Single<string> ("@/*/%2/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies left shift expressions works correctly
        /// </summary>
        [Test]
        public void LeftShiftExpression ()
        {
            Node node = new Node ("succ")
                .Add ("error")
                .Add ("ess")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/%2/</?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies right shift expressions works correctly
        /// </summary>
        [Test]
        public void RightShiftExpression ()
        {
            Node node = new Node ("root")
                .Add ("success")
                .Add ("error");
            string value = XUtil.Single<string> ("@/>/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies logical or expressions works correctly
        /// </summary>
        [Test]
        public void OrExpression ()
        {
            Node node = new Node ("root")
                .Add ("succ")
                .Add ("ess");
            string value = XUtil.Single<string> ("@/0/|/1/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies logical and expressions works correctly
        /// </summary>
        [Test]
        public void AndExpression ()
        {
            Node node = new Node ("root")
                .Add ("value1", "error")
                .Add ("value2", "success");
            string value = XUtil.Single<string> ("@/*/&/*/value2/?value", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies logical xor expressions works correctly
        /// </summary>
        [Test]
        public void XorExpression ()
        {
            Node node = new Node ("root")
                .Add ("succ")
                .Add ("ess")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/((/succ/|/error/)^(/ess/|/error/))/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies logical not expressions works correctly
        /// </summary>
        [Test]
        public void NotExpression ()
        {
            // verifying simple not works
            Node node = new Node ("root")
                .Add ("succ")
                .Add ("ess")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/!/2/?name", node, _context);
            Assert.AreEqual ("success", value);

            // verifying grouped not works
            node = new Node ("root")
                .Add ("succ")
                .Add ("ess")
                .Add ("error");
            value = XUtil.Single<string> ("@/*/(!/error/)?name", node, _context);
            Assert.AreEqual ("success", value);
            
            // verifying logical is using last "root"
            node = new Node ("root")
                .Add ("succ")
                .Add ("ess")
                .Add ("error");
            value = XUtil.Single<string> ("@/*/!/error/?name", node, _context);
            Assert.AreEqual ("successerror", value); // this one is not supposed to return "success"
        }
        
        /// <summary>
        /// verifies expressions handles precedence correctly
        /// </summary>
        [Test]
        public void PrecedenceExpression ()
        {
            Node node = new Node ("root")
                .Add ("_1", "error")
                .Add ("_2", "error")
                .Add ("_3", "success");
            string value = XUtil.Single<string> ("@/*/(/_1/|/_2/|/_3/&/_3/)?value", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies expressions handles orders of logical components correctly
        /// </summary>
        [Test]
        public void OrderedExpression ()
        {
            Node node = new Node ("root")
                .Add ("ess")
                .Add ("succ")
                .Add ("error");
            string value = XUtil.Single<string> ("@/1/|/0/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies it is possible to create a multiline expression
        /// </summary>
        [Test]
        public void MultilineExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("success", "x\r\ny")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/=@\"x\r\ny\"/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies creating expressions referencing other expressions works correctly
        /// </summary>
        [Test]
        public void ReferencedExpression1 ()
        {
            Node node = new Node ("root")
                .Add ("error", "@/+/?name")
                .Add ("success")
                .Add ("error");
            string value = XUtil.Single<string> ("@@/0/?value", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        /// <summary>
        /// verifies creating expressions referencing other expressions recursively works correctly
        /// </summary>
        [Test]
        public void ReferencedExpression2 ()
        {
            Node node = new Node ("root")
                .Add ("error", "@@/+2/?value")
                .Add ("success")
                .Add ("exp", "@/-/?name");
            string value = XUtil.Single<string> ("@@/0/?value", node, _context);
            Assert.AreEqual ("success", value);
        }

        /// <summary>
        /// verifies creating expressions referencing other expressions and constants
        /// intermingled with each other works correctly
        /// </summary>
        [Test]
        public void ReferencedExpression3 ()
        {
            Node node = ExecuteLambda (@"_data
  _1:su
  cc:@?name
  _3:ess
set:@/../?value
  source:@@/../*/_data/*/?value");
            Assert.AreEqual ("success", node.Value);
        }
        
        /// <summary>
        /// verifies creating expressions converting the result from string to integer works
        /// </summary>
        [Test]
        public void ConvertExpression1 ()
        {
            Node node = ExecuteLambda (@"_data:567
set:@/../?value
  source:@/../*/_data/?value.int");
            Assert.AreEqual (567, node.Value);
        }
        
        /// <summary>
        /// verifies creating expressions converting the result from integer to string works
        /// </summary>
        [Test]
        public void ConvertExpression2 ()
        {
            Node node = ExecuteLambda (@"_data:int:567
set:@/../?value
  source:@/../*/_data/?value.string");
            Assert.AreEqual ("567", node.Value);
        }

        /// <summary>
        /// converts a Node to its string representation
        /// </summary>
        [Test]
        public void Convert1 ()
        {
            Node node = new Node ("root")
                .Add ("foo", 5);
            string value = Utilities.Convert<string> (node, _context);
            Assert.AreEqual ("root\r\n  foo:int:5", value);
        }

        /// <summary>
        /// converts a DateTime to its Hyperlisp string representation
        /// </summary>
        [Test]
        public void Convert2 ()
        {
            DateTime date = new DateTime (2015, 01, 22, 23, 59, 59);
            string value = Utilities.Convert<string> (date, _context);
            Assert.AreEqual ("2015-01-22T23:59:59", value);
        }

        /// <summary>
        /// converts a Node without "phosphorus.types" loaded into the ApplicationContext
        /// </summary>
        [Test]
        public void Convert3 ()
        {
            Loader.Instance.UnloadAssembly ("phosphorus.types");
            _context = Loader.Instance.CreateApplicationContext ();
            try {
                Node node = new Node ("root")
                    .Add ("foo", 5);
                string value = Utilities.Convert<string> (node, _context);
                Assert.AreEqual ("Name=root, Count=1", value);
            }
            finally {
                Loader.Instance.LoadAssembly ("phosphorus.types");
                _context = Loader.Instance.CreateApplicationContext ();
            }
        }
        
        /// <summary>
        /// verifies escaped iterators works
        /// </summary>
        [Test]
        public void EscapedIterators ()
        {
            Node node = new Node ("_data")
                .Add ("*").LastChild
                    .Add ("..").LastChild
                        .Add (".").LastChild
                            .Add ("/").LastChild
                                .Add ("\\").LastChild
                                    .Add ("success").Root;
            string value = XUtil.Single<string> ("@/*/\\*/*/\\../*/\\./*/\"\\\\/\"/*/\\\\/*/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
    }
}
