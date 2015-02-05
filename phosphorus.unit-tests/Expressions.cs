
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
    [TestFixture]
    public class Expressions
    {
        private ApplicationContext _context;

        public Expressions ()
        {
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void IsExpression ()
        {
            bool notExp = XUtil.IsExpression ("mumbo jumbo");
            bool isExp = XUtil.IsExpression ("@/x/?value");
            Assert.AreEqual (false, notExp);
            Assert.AreEqual (true, isExp);
        }

        [Test]
        public void ValueExpression ()
        {
            Node node = new Node ("root")
                .Add ("x", "success")
                .Add ("y");
            var match = Expression.Create ("@/*/x/?value").Evaluate (node, _context);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }

        [Test]
        public void NameExpression ()
        {
            Node node = new Node ("root")
                .Add ("success")
                .Add ("y");
            var match = Expression.Create ("@/0/?name").Evaluate (node, _context);
            Assert.AreEqual (match.Count, 1);
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void CountExpression ()
        {
            Node node = new Node ("root")
                .Add ("x")
                .Add ("y");
            var match = Expression.Create ("@/*/?count").Evaluate (node, _context);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual (Match.MatchType.count, match.TypeOfMatch);
        }

        [Test]
        public void PathExpression ()
        {
            Node node = new Node ("root")
                .Add ("success")
                .Add ("y");
            var match = Expression.Create ("@/0/?path").Evaluate (node, _context);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (new Node.DNA ("0"), match [0].Value);
        }
        
        [Test]
        public void NodeExpression ()
        {
            Node node = new Node ("root")
                .Add ("success")
                .Add ("y");
            var match = Expression.Create ("@/0/?node").Evaluate (node, _context);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (node [0], match [0].Value);
        }
        
        [Test]
        public void IsFormatted ()
        {
            Node node = new Node ("root", "{0}{1}")
                .Add ("", "su")
                .Add ("x", "error")
                .Add ("", "ccess");
            bool value = XUtil.IsFormatted (node);
            Assert.AreEqual (true, value);

            node = new Node ("root", "{0}{1}")
                .Add ("x")
                .Add ("y", "error")
                .Add ("z");
            value = XUtil.IsFormatted (node);
            Assert.AreEqual (false, value);
        }
        
        [Test]
        public void Format()
        {
            Node node = new Node ("root", "{0}{1}")
                .Add ("", "su")
                .Add ("x", "error")
                .Add ("", "ccess");
            string value = XUtil.FormatNode (node, node, _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void FormatWithDataSource ()
        {
            Node node = new Node ("root", "{0}{1}")
                .Add ("", "@/*/_first/?value")
                .Add ("", "@/*/_second/?value")
                .Add ("_source").LastChild
                    .Add ("_first", "su")
                    .Add ("x", "error")
                    .Add ("_second", "ccess").Root;

            // notice that data source node and formatting nodes are different here ...
            string value = XUtil.FormatNode (node, node [2], _context);
            Assert.AreEqual ("success", value);
        }

        [Test]
        public void Single ()
        {
            Node node = new Node ("root")
                .Add ("", "su")
                .Add ("", "ccess");
            string value = XUtil.Single<string> ("@/*/?value", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void Iterate ()
        {
            Node node = new Node ("root")
                .Add ("", "su")
                .Add ("", "cc")
                .Add ("", "ess");
            string value = null;
            XUtil.Iterate<string> ("@/*/?value", node, _context, 
            delegate (string idx) {
                value += idx;
            });
            Assert.AreEqual ("success", value);
        }

        [Test]
        public void RootExpression()
        {
            Node node = new Node ("success")
                .Add ("");
            string value = XUtil.Single<string> ("@/../?name", node [0], _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void ChildrenExpression()
        {
            Node node = new Node ("")
                .Add ("su")
                .Add ("ccess");
            string value = XUtil.Single<string> ("@/*/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
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
        
        [Test]
        public void NameEqualsExpression ()
        {
            Node node = new Node ("root")
                .Add ("success")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/success/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void ValueEqualsExpression ()
        {
            Node node = new Node ("root")
                .Add ("success", "query")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/=query/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void NumberedExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("success");
            string value = XUtil.Single<string> ("@/1/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
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
        
        [Test]
        public void ReferenceExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("error", new Node ("_value", "success"));
            string value = XUtil.Single<string> ("@/1/#/?value", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void ParentExpression ()
        {
            Node node = new Node ("success")
                .Add ("error")
                .Add ("error");
            string value = XUtil.Single<string> ("@/./?name", node [1], _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void NamedRegexExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("success");
            string value = XUtil.Single<string> (@"@/*/""/s/""/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void ValuedRegexExpression ()
        {
            Node node = new Node ("root")
                .Add ("error")
                .Add ("success", "val");
            string value = XUtil.Single<string> (@"@/*/=""/val/""/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
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
        
        [Test]
        public void RightShiftExpression ()
        {
            Node node = new Node ("root")
                .Add ("success")
                .Add ("error");
            string value = XUtil.Single<string> ("@/>/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void OrExpression ()
        {
            Node node = new Node ("root")
                .Add ("succ")
                .Add ("ess");
            string value = XUtil.Single<string> ("@/0/|/1/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
        [Test]
        public void AndExpression ()
        {
            Node node = new Node ("root")
                .Add ("value1", "error")
                .Add ("value2", "success");
            string value = XUtil.Single<string> ("@/*/&/*/value2/?value", node, _context);
            Assert.AreEqual ("success", value);
        }
        
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
        
        [Test]
        public void NotExpression ()
        {
            Node node = new Node ("root")
                .Add ("succ")
                .Add ("ess")
                .Add ("error");
            string value = XUtil.Single<string> ("@/*/!/2/?name", node, _context);
            Assert.AreEqual ("success", value);
        }
        
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
        
        [Test]
        public void ReferencedExpression ()
        {
            Node node = new Node ("root")
                .Add ("error", "@/+/?name")
                .Add ("success")
                .Add ("error");
            string value = XUtil.Single<string> ("@@/0/?value", node, _context);
            Assert.AreEqual ("success", value);

            node = new Node ("root")
                .Add ("error", "@@/+2/?name")
                .Add ("success")
                .Add ("@/-/?name");
            value = XUtil.Single<string> ("@@/0/?value", node, _context);
            Assert.AreEqual ("success", value);
        }
    }
}
