/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using NUnit.Framework;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.unittests
{
    /// <summary>
    ///     Expressions unit tests, tests all sorts of different expressions
    ///     and verify they work as expected
    /// </summary>
    [TestFixture]
    public class Expressions : TestBase
    {
        public Expressions ()
            : base ("p5.types", "p5.hyperlisp", "p5.lambda")
        { }
        
        [Test]
        public void ValueExpression ()
        {
            var exp = Expression.Create ("/foo?value", Context);
            Assert.IsNotNull (exp);
            Assert.AreEqual ("/foo?value", exp.Value);
        }
        
        [Test]
        public void NameExpression ()
        {
            var exp = Expression.Create ("/foo?name", Context);
            Assert.IsNotNull (exp);
            Assert.AreEqual ("/foo?name", exp.Value);
        }
        
        [Test]
        public void CountExpression ()
        {
            var exp = Expression.Create ("/foo?count", Context);
            Assert.IsNotNull (exp);
            Assert.AreEqual ("/foo?count", exp.Value);
        }
        
        [Test]
        public void NodeExpression ()
        {
            var exp = Expression.Create ("/foo?node", Context);
            Assert.IsNotNull (exp);
            Assert.AreEqual ("/foo?node", exp.Value);
        }

        [Test]
        public void NodeIsDefault ()
        {
            var exp = Expression.Create ("/foo", Context);
            Assert.IsNotNull (exp);
            Assert.AreEqual ("/foo", exp.Value);
        }

        [Test]
        public void NonExistingType ()
        {
            Assert.Throws<ExpressionException> (delegate {
                Expression.Create ("/foo?valuXX", Context);
            });
        }
        
        [Test]
        public void CaseSensitiveType ()
        {
            Assert.Throws<ExpressionException> (delegate {
                Expression.Create ("/foo?Value", Context);
            });
        }

        [Test]
        public void MissingIteratorDeclaration ()
        {
            Assert.Throws<ExpressionException> (delegate {
                Expression.Create ("foo?value", Context);
            });
        }
        
        [Test]
        public void IteratorDeclarationAtEnd ()
        {
            var exp = Expression.Create ("/*/_foo/*/?value", Context);
            var node = CreateNode (@"_foo
  :success");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.value, match.TypeOfMatch);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }

        [Test]
        public void IteratorDeclarationBeforeLogical ()
        {
            var exp = Expression.Create ("/*/_foo/*/|/*/_bar/*/?value", Context);
            var node = CreateNode (@"_foo
  :success1
_bar
  :success2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("success1", match [0].Value);
            Assert.AreEqual ("success2", match [1].Value);
        }

        [Test]
        public void IteratorDeclarationBeforeGroup ()
        {
            var exp = Expression.Create ("/*/_foo/*/(!/=error)?value", Context);
            var node = CreateNode (@"_foo
  :success
  :error");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void StartingWithGroupDeclaration ()
        {
            var exp = Expression.Create ("(/../*(!(/_result|/_content)))?value", Context);
            var node = CreateNode (@"_foo:success
_result:error
_content:error");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }

        [Test]
        public void MultipleConsecutiveEmptyNames ()
        {
            var exp = Expression.Create ("/*/_foo/*//*/?value", Context);
            var node = CreateNode (@"_foo
  :error
    :success
  _not-empty:error2
    :error");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }

        [Test]
        public void IdentityExpression ()
        {
            var exp = Expression.Create ("", Context);
            Assert.IsNotNull (exp);
            Assert.AreEqual ("", exp.Value);
        }
        
        [Test]
        public void TypeDeclarationWithoutIterator ()
        {
            var exp = Expression.Create ("?value", Context);
            Assert.IsNotNull (exp);
            Assert.AreEqual ("?value", exp.Value);
        }
        
        [Test]
        public void CastingExpressionToExpression ()
        {
            var exp = Expression.Create ("?value.x", Context);
            Assert.IsNotNull (exp);
            Assert.AreEqual ("?value.x", exp.Value);
        }
        
        [Test]
        public void RetrieveValue ()
        {
            var exp = Expression.Create ("/../0?value", Context);
            var node = CreateNode (@"foo:success");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.value, match.TypeOfMatch);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void RetrieveName ()
        {
            var exp = Expression.Create ("/../0?name", Context);
            var node = CreateNode (@"success");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.name, match.TypeOfMatch);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void RetrieveCount ()
        {
            var exp = Expression.Create ("/../0/*?count", Context);
            var node = CreateNode (@"_foo
  x
  y");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.count, match.TypeOfMatch);
            Assert.AreEqual (2, match.Count);
        }
        
        [Test]
        public void RetrieveNode ()
        {
            var exp = Expression.Create ("/../0/*?node", Context);
            var node = CreateNode (@"_foo
  x:x1
  y:y1");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.node, match.TypeOfMatch);
            Assert.AreEqual (2, match.Count);
            Assert.AreSame (node [0] [0], match [0].Value);
            Assert.AreSame (node [0] [1], match [1].Value);
        }

        [Test]
        public void RetrieveMultipleNames ()
        {
            var exp = Expression.Create ("/../0/*?name", Context);
            var node = CreateNode (@"_foo
  x
  y");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.name, match.TypeOfMatch);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("y", match [1].Value);
        }
        
        [Test]
        public void RetrieveMultipleValues ()
        {
            var exp = Expression.Create ("/../0/*?value", Context);
            var node = CreateNode (@"_foo
  :x
  :y");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.value, match.TypeOfMatch);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("x", match [0].Value);
            Assert.AreEqual ("y", match [1].Value);
        }

        [Test]
        public void RetrieveExpression ()
        {
            var exp = Expression.Create ("/../0?value", Context);
            var node = CreateNode (@"_foo:x:/+?value");
            Assert.AreEqual ("/../0?value", exp.Value);
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.value, match.TypeOfMatch);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("/+?value", (match [0].Value as Expression).Value);
        }
        
        [Test]
        public void ReferenceExpression ()
        {
            var exp = Expression.Create ("@/../0?value", Context);
            var node = CreateNode (@"_foo:x:/+?value
_bar:success");
            Assert.AreEqual ("@/../0?value", exp.Value);
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.value, match.TypeOfMatch);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
            Assert.AreEqual (Match.MatchType.value, match [0].TypeOfMatch);
        }

        [Test]
        public void ReferenceExpressionChangeType ()
        {
            var exp = Expression.Create ("@/../0?value", Context);
            var node = CreateNode (@"_foo:x:/+?name
_success");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.value, match.TypeOfMatch);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("_success", match [0].Value);

            // notice, MatchEntity has DIFFERENT type than Match object!
            Assert.AreEqual (Match.MatchType.name, match [0].TypeOfMatch);
        }
        
        [Test]
        public void ReferenceExpressionYieldingNoExpression ()
        {
            var exp = Expression.Create ("@/../0?value", Context);
            var node = CreateNode (@"_bar:success");
            Assert.AreEqual ("@/../0?value", exp.Value);
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (Match.MatchType.value, match.TypeOfMatch);
            Assert.AreEqual (1, match.Count);

            // Notice, even though the expression retrieving this value was a reference expression,
            // there was no expression in source of expression, hence the 'simple value' of expression
            // result is returned, and not evaluated in any ways!
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void ReferenceExpressionMixed ()
        {
            var exp = Expression.Create ("@/../*/[0,2]?value", Context);
            var node = CreateNode (@"_bar1:success1
_bar2:x:/+?name
_success2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual (Match.MatchType.value, match.TypeOfMatch);

            Assert.AreEqual ("success1", match [0].Value);
            Assert.AreEqual (Match.MatchType.value, match [0].TypeOfMatch);

            Assert.AreEqual ("_success2", match [1].Value);
            Assert.AreEqual (Match.MatchType.name, match [1].TypeOfMatch);
        }
        
        [Test]
        public void ReferenceMultipleExpressions ()
        {
            var exp = Expression.Create ("@/../*/[0,2]?value", Context);
            var node = CreateNode (@"_foo1:x:/+2?value
_foo2:x:/+2?value
_res1:success1
_res2:success2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);

            Assert.AreEqual ("success1", match [0].Value);
            Assert.AreEqual (Match.MatchType.value, match [0].TypeOfMatch);

            Assert.AreEqual ("success2", match [1].Value);
            Assert.AreEqual (Match.MatchType.value, match [1].TypeOfMatch);
        }
        
        [Test]
        public void IterateExpressionResults ()
        {
            var exp = Expression.Create ("@/../*/[0,2]?value", Context);
            var node = CreateNode (@"_foo1:x:/+2?value
_foo2:x:/+2?value
_res1:success1
_res2:success2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            bool first = true;
            foreach (var idxMatch in match) {
                if (first) {
                    Assert.AreEqual ("success1", idxMatch.Value);
                    first = false;
                } else {
                    Assert.AreEqual ("success2", idxMatch.Value);
                }
            }
        }
        
        [Test]
        public void SetExpressionValueResult ()
        {
            var exp = Expression.Create ("@/../*/[0,2]?value", Context);
            var node = CreateNode (@"_foo1:x:/+2?value
_foo2:x:/+2?value
_res1:error1
_res2:error2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            bool first = true;
            foreach (var idxMatch in match) {
                if (first) {
                    idxMatch.Value = "success1";
                    first = false;
                } else {
                    idxMatch.Value = "success2";
                }
            }
            Assert.AreEqual ("success1", node [2].Value);
            Assert.AreEqual ("success2", node [3].Value);
        }
        
        [Test]
        public void SetExpressionValueIntegerResult ()
        {
            var exp = Expression.Create ("@/../*/[0,2]?value", Context);
            var node = CreateNode (@"_foo1:x:/+2?value
_foo2:x:/+2?value
_res1:error1
_res2:error2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            bool first = true;
            foreach (var idxMatch in match) {
                if (first) {
                    idxMatch.Value = 1;
                    first = false;
                } else {
                    idxMatch.Value = 2;
                }
            }
            Assert.AreEqual (1, node [2].Value);
            Assert.AreEqual (2, node [3].Value);
        }
        
        [Test]
        public void SetExpressionNameResult ()
        {
            var exp = Expression.Create ("@/../*/[0,2]?value", Context);
            var node = CreateNode (@"_foo1:x:/+2?name
_foo2:x:/+2?name
_error1
_error2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            bool first = true;
            foreach (var idxMatch in match) {
                if (first) {
                    idxMatch.Value = "_success1";
                    first = false;
                } else {
                    idxMatch.Value = "_success2";
                }
            }
            Assert.AreEqual ("_success1", node [2].Name);
            Assert.AreEqual ("_success2", node [3].Name);
        }
        
        [Test]
        public void SetExpressionNameIntegerResult ()
        {
            var exp = Expression.Create ("@/../*/[0,2]?value", Context);
            var node = CreateNode (@"_foo1:x:/+2?name
_foo2:x:/+2?name
_error1
_error2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            bool first = true;
            foreach (var idxMatch in match) {
                if (first) {
                    idxMatch.Value = 1;
                    first = false;
                } else {
                    idxMatch.Value = 2;
                }
            }
            Assert.AreEqual ("1", node [2].Name);
            Assert.AreEqual ("2", node [3].Name);
        }
        
        [Test]
        public void SetExpressionNodeResult ()
        {
            var exp = Expression.Create ("/../*?node", Context);
            var node = CreateNode (@"_error1
_error2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            bool first = true;
            foreach (var idxMatch in match) {
                if (first) {
                    idxMatch.Value = new Node ("_foo1", "_success1");
                    first = false;
                } else {
                    idxMatch.Value = new Node ("_foo2", "_success2");
                }
            }
            Assert.IsTrue (new Node ("_foo1", "_success1").CompareTo (node [0]) == 0);
            Assert.IsTrue (new Node ("_foo2", "_success2").CompareTo (node [1]) == 0);
        }
        
        [Test]
        public void ConvertExpressionNameResult ()
        {
            var exp = Expression.Create ("/../*?name.int", Context);
            var node = CreateNode (@"1
2");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual (1, match [0].Value);
            Assert.AreEqual (2, match [1].Value);
        }
        
        [Test]
        public void ConvertExpressionValueResult ()
        {
            var exp = Expression.Create ("/../*?value.string", Context);
            var node = CreateNode (@"_x:date:""2012-12-23T21:21:21""
_x:date:""2012-12-23T21:21:22""");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual ("2012-12-23T21:21:21", match [0].Value);
            Assert.AreEqual ("2012-12-23T21:21:22", match [1].Value);
        }
        
        [Test]
        public void DoNotConvertExpressionValueResult ()
        {
            var exp = Expression.Create ("/../*?value", Context);
            var node = CreateNode (@"_x:date:""2012-12-23T21:21:21""
_x:date:""2012-12-23T21:21:22""");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (2, match.Count);
            Assert.AreEqual (new DateTime (2012, 12, 23, 21, 21, 21), match [0].Value);
            Assert.AreEqual (new DateTime (2012, 12, 23, 21, 21, 22), match [1].Value);
        }

        [Test]
        public void FormatExpressionConstant ()
        {
            var exp = Expression.Create ("@/../0?value", Context);
            var node = CreateNode (@"_x:x:/../{0}?value
  :1
_success:success");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void FormatExpressionDynamic ()
        {
            var exp = Expression.Create ("@/../0?value", Context);
            var node = CreateNode (@"_x:x:/../{0}?value
  :x:/../*/_1?value
_success:success
_1:1");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void NestedFormatExpressionDynamic ()
        {
            var exp = Expression.Create ("@/../0?value", Context);
            var node = CreateNode (@"_x:x:/../{0}?value
  :x:/../*/{0}?value
    :x:/../*/_1?name
_success:success
_1:1");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void NestedFormatReferenceExpression ()
        {
            var exp = Expression.Create ("@/../0?value", Context);
            var node = CreateNode (@"_x:x:/../{0}?value
  :x:/../*/{0}?value
    :x:@/../*/_nest?value
_success:success
_1:1
_nest:x:/../*/_1?name");
            var match = exp.Evaluate (Context, node);
            Assert.AreEqual (1, match.Count);
            Assert.AreEqual ("success", match [0].Value);
        }
        
        [Test]
        public void ExpressionMadnessObfuscatedCodeOlympicsWinner ()
        {
            var node = ExecuteLambda (@"_result
set:x:/-?value
  src:x:/{0}/{1}{2}value
    :.{0}
      :x:/../3?{0}
        :val{0}
          :x:{0}..{0}4?name
            :x:@/{0}./{1}?value
              :.
              :5
    :2
    :x:@/../6?value
_x:success
_x:.
ue
_:/
_y:x:/+?value
:?
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual ("success", node [0].Value);
        }
        
        [Test]
        public void IterateStringValues ()
        {
            var node = CreateNode (@"succ
ess:bar2");
            string result = "";
            foreach (var idx in XUtil.Iterate<string> (Context, node)) {
                result += idx;
            }
            Assert.AreEqual ("success", result);
        }
        
        [Test]
        public void IterateNodesIntegerValues ()
        {
            var node = CreateNode (@"err:5
or:2");
            int result = 0;
            foreach (var idx in XUtil.Iterate<int> (Context, node)) {
                result += idx;
            }
            Assert.AreEqual (7, result);
        }
        
        [Test]
        public void SingleNodesStringValue ()
        {
            var node = CreateNode (@"succ:err
ess:or");
            string result = XUtil.Single<string> (Context, node);
            Assert.AreEqual ("success", result);
        }
    }
}
