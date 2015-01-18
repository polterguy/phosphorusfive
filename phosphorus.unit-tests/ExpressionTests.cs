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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?name");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?value");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?count");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?path");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.Path, match.TypeOfMatch, "expression didn't return what we expected");
            Assert.AreEqual (new Node.DNA ("0"), match.GetValue (0));
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.Node, match.TypeOfMatch, "expression didn't return what we expected");
            Assert.AreSame (tmp [0], match.GetValue (0));
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../*/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../*/**/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/"".._tmp1""/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/-/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/+/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp2/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("x2", match [1].Value);
        }
        
        [Test]
        public void ValuedExpression ()
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/=x2/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/1/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/**/[1,3]/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("z", match [0].Value);
            Assert.AreEqual ("y", match [1].Value);
        }
        
        [Test]
        public void RangedOnlyStartExpression ()
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/**/[2,]/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("y", match [0].Value);
            Assert.AreEqual ("x2", match [1].Value);
        }
        
        [Test]
        public void RangedOnlyEndExpression ()
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/**/[,2]/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/0/0/#/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/0/0/./?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/0/+2/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/-2/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/""/_tmp+/""/?node");
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
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/=""/x+/""/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("x2", match [1].Value);
        }
        
        [Test]
        public void ModuloExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/%2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match [0].Name);
            Assert.AreEqual ("_tmp3", match [1].Name);
        }
        
        [Test]
        public void LeftShiftExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ("foo");
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/%2/</?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("foo", match [0].Name);
            Assert.AreEqual ("_tmp2", match [1].Name);
        }
        
        [Test]
        public void RightShiftExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/%2/>/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp2", match [0].Name);
        }
        
        [Test]
        public void OrExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/|/*/_tmp2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match [0].Name);
            Assert.AreEqual ("_tmp2", match [1].Name);
        }
        
        [Test]
        public void AndExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/&/*/=x/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp1", match [0].Name);
        }
        
        [Test]
        public void XorExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/(/*/_tmp1/|/*/_tmp3/)^(/*/_tmp2/|/*/_tmp3/)?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match [0].Name);
            Assert.AreEqual ("_tmp2", match [1].Name);
        }
        
        [Test]
        public void NotExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp1:x2
_tmp1:y
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/!/*/=x2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match [0].Name);
            Assert.AreEqual ("_tmp1", match [1].Name);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("y", match [1].Value);
        }
        
        [Test]
        public void NoPrecedenceExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/|/*/_tmp2/&/*/=y/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp2", match [0].Name);
        }
        
        [Test]
        public void OrderedExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp2/|/*/_tmp1/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp2", match [0].Name);
            Assert.AreEqual ("_tmp1", match [1].Name);
        }

        [Test]
        public void GroupExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_tmp1:x
_tmp2:y
_tmp3:x2
";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/|(/*/_tmp2/&/*/=y/)?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match [0].Name);
            Assert.AreEqual ("_tmp2", match [1].Name);
        }
        
        [Test]
        public void MultilineExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
""_tmp\n1"":x";
            context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/@""_tmp
1""/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp\r\n1", match [0].Name);
        }
    }
}

