
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
    public class ExpressionTests : TestBase
    {
        public ExpressionTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.types");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void SimpleNameExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp:x";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?name");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp", match.GetNode (0).Name, "expression didn't return what we expected");
            Assert.AreEqual ("x", match.GetNode (0).Value, "expression didn't return what we expected");
            Assert.AreEqual (new Node.DNA ("0"), match.GetNode (0).Path, "expression didn't return what we expected");
            Assert.AreEqual (Match.MatchType.Name, match.TypeOfMatch, "expression didn't return what we expected");
        }

        [Test]
        public void SimpleValueExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp:x";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?value");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp", match.GetNode (0).Name, "expression didn't return what we expected");
            Assert.AreEqual ("x", match.GetNode (0).Value, "expression didn't return what we expected");
            Assert.AreEqual (new Node.DNA ("0"), match.GetNode (0).Path, "expression didn't return what we expected");
            Assert.AreEqual (Match.MatchType.Value, match.TypeOfMatch, "expression didn't return what we expected");
        }
        
        [Test]
        public void SimpleCountExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp:x";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?count");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.Count, match.TypeOfMatch, "expression didn't return what we expected");
        }
        
        [Test]
        public void SimplePathExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp:x";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?path");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.Path, match.TypeOfMatch, "expression didn't return what we expected");
            Assert.AreEqual (new Node.DNA ("0"), match.GetValue (0));
        }
        
        [Test]
        public void SimpleNodeExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp:x";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../0/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (Match.MatchType.Node, match.TypeOfMatch, "expression didn't return what we expected");
            Assert.AreSame (tmp [0], match.GetValue (0));
        }
        
        [Test]
        public void AllChildrenExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../*/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual (Match.MatchType.Node, match.TypeOfMatch, "expression didn't return what we expected");
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp2", match.GetNode (1).Name);
        }
        
        [Test]
        public void AllDescendantsExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../*/**/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (3, match.Count);
            Assert.AreEqual (Match.MatchType.Node, match.TypeOfMatch, "expression didn't return what we expected");
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp3", match.GetNode (1).Name);
            Assert.AreEqual ("_tmp2", match.GetNode (2).Name);
        }
        
        [Test]
        public void RootExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create ("@/../?node");
            var match = ex.Evaluate (tmp [0] [0]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual (string.Empty, match.GetNode (0).Name);
            Assert.AreEqual ("_tmp1", match.GetNode (0) [0].Name);
        }
        
        [Test]
        public void NamedAncestorExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/"".._tmp1""/?node");
            var match = ex.Evaluate (tmp [0] [0]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
        }
        
        [Test]
        public void PreviousSiblingExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/-/?node");
            var match = ex.Evaluate (tmp [1]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
        }
        
        [Test]
        public void NextSiblingExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/+/?node");
            var match = ex.Evaluate (tmp [0]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp2", match.GetNode (0).Name);
        }
        
        [Test]
        public void NamedChildExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp2", match.GetNode (0).Name);
        }
        
        [Test]
        public void NamedChildMultipleReturnsExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2
";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match.GetNode (0).Value);
            Assert.AreEqual ("x2", match.GetNode (1).Value);
        }
        
        [Test]
        public void ValuedExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/=x2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("x2", match.GetNode (0).Value);
        }
        
        [Test]
        public void NumberedChildExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/1/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("y", match.GetNode (0).Value);
        }
        
        [Test]
        public void RangedExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/**/[1,3]/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("z", match.GetNode (0).Value);
            Assert.AreEqual ("y", match.GetNode (1).Value);
        }
        
        [Test]
        public void RangedOnlyStartExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/**/[2,]/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("y", match.GetNode (0).Value);
            Assert.AreEqual ("x2", match.GetNode (1).Value);
        }
        
        [Test]
        public void RangedOnlyEndExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:z
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/**/[,2]/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match.GetNode (0).Value);
            Assert.AreEqual ("z", match.GetNode (1).Value);
        }

        [Test]
        public void ReferenceNode ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:node:@""_x:zz""
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/0/0/#/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("zz", match.GetNode (0).Value);
        }
        
        [Test]
        public void ParentExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
  _tmp3:zz
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/0/0/./?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("x", match.GetNode (0).Value);
        }
        
        [Test]
        public void NumberedNextSiblingExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/0/+2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("x2", match.GetNode (0).Value);
        }
        
        [Test]
        public void NumberedPreviousSiblingExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/-2/?node");
            var match = ex.Evaluate (tmp [2]);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("x", match.GetNode (0).Value);
        }
        
        [Test]
        public void NamedRegexExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_xmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/""/_tmp+/""/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match.GetNode (0).Value);
            Assert.AreEqual ("x2", match.GetNode (1).Value);
        }
        
        [Test]
        public void ValuedRegexExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp1:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/=""/x+/""/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match.GetNode (0).Value);
            Assert.AreEqual ("x2", match.GetNode (1).Value);
        }
        
        [Test]
        public void ModuloExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/%2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp3", match.GetNode (1).Name);
        }
        
        [Test]
        public void LeftShiftExpression ()
        {
            Node tmp = new Node ("foo");
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/%2/</?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("foo", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp2", match.GetNode (1).Name);
        }
        
        [Test]
        public void RightShiftExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/%2/>/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp2", match.GetNode (0).Name);
        }
        
        [Test]
        public void OrExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/|/*/_tmp2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp2", match.GetNode (1).Name);
        }
        
        [Test]
        public void AndExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/&/*/=x/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
        }
        
        [Test]
        public void XorExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/(/*/_tmp1/|/*/_tmp3/)^(/*/_tmp2/|/*/_tmp3/)?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp2", match.GetNode (1).Name);
        }
        
        [Test]
        public void NotExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp1:x2
_tmp1:y";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/!/*/=x2/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp1", match.GetNode (1).Name);
            Assert.AreEqual ("x", match.GetNode (0).Value);
            Assert.AreEqual ("y", match.GetNode (1).Value);
        }
        
        [Test]
        public void NoPrecedenceExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/|/*/_tmp2/&/*/=y/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp2", match.GetNode (0).Name);
        }
        
        [Test]
        public void OrderedExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp2/|/*/_tmp1/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp2", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp1", match.GetNode (1).Name);
        }

        [Test]
        public void GroupExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"_tmp1:x
_tmp2:y
_tmp3:x2";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/_tmp1/|(/*/_tmp2/&/*/=y/)?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("_tmp1", match.GetNode (0).Name);
            Assert.AreEqual ("_tmp2", match.GetNode (1).Name);
        }
        
        [Test]
        public void MultilineExpression ()
        {
            Node tmp = new Node ();
            tmp.Value = @"""_tmp\n1"":x";
            _context.Raise ("code2lambda", tmp);
            var ex = Expression.Create (@"@/*/@""_tmp
1""/?node");
            var match = ex.Evaluate (tmp);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_tmp\r\n1", match.GetNode (0).Name);
        }
        
        [Test]
        public void FormatNodeWithTypeConverters ()
        {
            _context.Raise ("pf.core.application-start");
            Node tmp = new Node ();
            tmp.Value = @"_tmp:{0}{1}{2}{3}{4}
  :x{0}
    :y{0}
      :int:5
  :int:5
  :date:""2012-11-23T22:59:57""
  :path:0-2
  :node:""_x:y""";
            _context.Raise ("code2lambda", tmp);
            object result = XUtil.FormatNode (tmp [0]);
            Assert.AreEqual ("xy55" + new DateTime (2012, 11, 23, 22, 59, 57).ToString ("yyyy-MM-ddTHH:mm:ss") + "0-2" + "_x:y", result, "wrong result in assert");
        }
        
        [Test]
        public void ReferenceExpression ()
        {
            Node tmp = ExecuteLambda (@"_data:success
_exp:@/-/?value
set:@/+/?value
  source:@@/./-/?value
_result");
            Assert.AreEqual ("success", tmp [3].Value, "wrong result in assert");
        }
        
        [Test]
        public void ReferencedReferenceExpression ()
        {
            Node tmp = ExecuteLambda (@"_data:success
_exp1:@/-/?value
_exp2:@@/-/?value
set:@/+/?value
  source:@@/./-/?value
_result");
            Assert.AreEqual ("success", tmp [4].Value, "wrong result in assert");
        }
        
        [Test]
        public void ReferenceExpressionReturningMultipleInnerExpressions ()
        {
            Node tmp = ExecuteLambda (@"_exp1:@/../*/_result1/?value
_exp1:@/../*/_result2/?value
set:@@/../*/_exp1/?value
  source:success
_result1
_result2");
            Assert.AreEqual ("success", tmp [3].Value, "wrong result in assert");
            Assert.AreEqual ("success", tmp [4].Value, "wrong result in assert");
        }
        
        [Test]
        public void FormatedReferenceExpression ()
        {
            Node tmp = ExecuteLambda (@"
_source:_exp
_exp:@/../*/_result/?value
set:@@/../*/{0}/?value
  :@/../*/_source/?value
  source:success
_result");
            Assert.AreEqual ("success", tmp [3].Value, "wrong result in assert");
        }
        
        [Test]
        public void ReferencedReferenceExpressionReturningDifferentType ()
        {
            Node tmp = ExecuteLambda (@"_data:_success
_exp1:@/-/?value
set:@/+/?name
  source:@@/./-/?value
_result");
            Assert.AreEqual ("_success", tmp [3].Name, "wrong result in assert");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            ExecuteLambda (@"_exp1:@/-/?value
_exp1:@/-/?name
set:@/+/?name
  source:@@/../*/_exp1/?value
_result");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            ExecuteLambda (@"_exp1:@/-/?value
_exp1:@/-/?value
set:@/+/?name
  source:@@@/../*/_exp1/?value
_result");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            ExecuteLambda (@"_exp1:/-/?value
_exp1:@/-/?value
set:@/+/?name
  source:@@/../*/_exp1/?value
_result");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            ExecuteLambda (@"_exp1:@/-/?value
_exp1:@/-/?value
set:@/+/?value
  source:@@/../*/_exp1/?name
_result");
        }
    }
}
