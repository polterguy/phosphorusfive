/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.unittests
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        public void SimpleNameExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp:x";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression ("@/../0/?name");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp", match [0].Name, "expression didn't return what we expected");
            Assert.AreEqual ("x", match [0].Value, "expression didn't return what we expected");
            Assert.AreEqual (new Node.DNA ("0"), match [0].Path, "expression didn't return what we expected");
            Assert.AreEqual (Match.MatchType.Name, match.TypeOfMatch, "expression didn't return what we expected");
        }

        [Test]
        public void SimpleValueExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp:x";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression ("@/../0/?value");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp", match [0].Name, "expression didn't return what we expected");
            Assert.AreEqual ("x", match [0].Value, "expression didn't return what we expected");
            Assert.AreEqual (new Node.DNA ("0"), match [0].Path, "expression didn't return what we expected");
            Assert.AreEqual (Match.MatchType.Value, match.TypeOfMatch, "expression didn't return what we expected");
        }
        
        [Test]
        public void SimpleCountExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp:x";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression ("@/../0/?count");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.Count, match.TypeOfMatch, "expression didn't return what we expected");
        }
        
        [Test]
        public void SimplePathExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp:x";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression ("@/../0/?path");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.Path, match.TypeOfMatch, "expression didn't return what we expected");
        }
        
        [Test]
        public void SimpleNodeExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp:x";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression ("@/../0/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.Node, match.TypeOfMatch, "expression didn't return what we expected");
        }
        
        [Test]
        public void AllChildrenExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression ("@/../*/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual (Match.MatchType.Node, match.TypeOfMatch, "expression didn't return what we expected");
            Assert.AreEqual ("_tmp1", match [0].Name);
            Assert.AreEqual ("_tmp2", match [1].Name);
        }
        
        [Test]
        public void AllDescendantsExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression ("@/../*/**/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (3, match.Count);
            Assert.AreEqual (Match.MatchType.Node, match.TypeOfMatch, "expression didn't return what we expected");
            Assert.AreEqual ("_tmp1", match [0].Name);
            Assert.AreEqual ("_tmp3", match [1].Name);
            Assert.AreEqual ("_tmp2", match [2].Name);
        }
        
        [Test]
        public void RootExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression ("@/../?node");
            var match = ex.Evaluate (tmp [0] [0]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (string.Empty, match [0].Name);
            Assert.AreEqual ("_tmp1", match [0] [0].Name);
        }
        
        [Test]
        public void NamedAncestorExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/"".._tmp1""/?node");
            var match = ex.Evaluate (tmp [0] [0]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp1", match [0].Name);
        }
        
        [Test]
        public void PreviousSiblingExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/-/?node");
            var match = ex.Evaluate (tmp [1]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp1", match [0].Name);
        }
        
        [Test]
        public void NextSiblingExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/+/?node");
            var match = ex.Evaluate (tmp [0]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp2", match [0].Name);
        }
        
        [Test]
        public void NamedChildExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/_tmp2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp2", match [0].Name);
        }
        
        [Test]
        public void NamedChildMultipleReturnsExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/_tmp1/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("x2", match [1].Value);
        }
        
        [Test]
        public void ValuedChildExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/=x2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("x2", match [0].Value);
        }
        
        [Test]
        public void NumberedChildExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/1/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("y", match [0].Value);
        }
        
        [Test]
        public void RangedExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/*/**/[0,2]/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("z", match [1].Value);
        }
        
        [Test]
        public void ReferenceExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:node:@""_x:zz""
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/0/0/#/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("zz", match [0].Value);
        }
        
        [Test]
        public void ParentExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
  _tmp3:zz
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/0/0/./?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("x", match [0].Value);
        }
        
        [Test]
        public void NumberedNextSiblingExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/0/+2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("x2", match [0].Value);
        }
        
        [Test]
        public void NumberedPreviousSiblingExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/-2/?node");
            var match = ex.Evaluate (tmp [2]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("x", match [0].Value);
        }
        
        [Test]
        public void NamedRegexExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_xmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/""/_tmp+/""/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("x2", match [1].Value);
        }
        
        [Test]
        public void ValuedRegexExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp1:x2
";
            context.Raise ("pf.code-2-nodes", tmp);
            var ex = new Expression (@"@/=""/x+/""/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("x2", match [1].Value);
        }// TODO; psst, modulo, shift-expressions, logicals and grouping
    }
}

