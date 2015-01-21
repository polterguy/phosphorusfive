
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
    public class BranchingTests
    {
        [Test]
        public void EmptyIfNoResult ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
if:@/-/?node";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
        }

        [Test]
        public void EmptyIfNoResultVerifyElseExecuted ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
if:@/-/?node
else
  set:@/./+/?value
    :x
_val";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfTrueVerifyElseNotExecuted ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
if:@/-/?node
else
  set:@/./+/?value
    :x
_val";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfTrueVerifyElseIfNotExecuted ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
if:@/-/?node
else-if:@/?name
  set:@/./+/?value
    :x
_val";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfFalseVerifyElseIfExecuted ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
if:@/-/?node
else-if:@/?name
  set:@/./+/?value
    :x
_val";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }

        [Test]
        public void IfFalseVerifyElseExecuted ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
if:@/-/?node
else
  set:@/./+/?value
    :x
_val";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }

        [Test]
        public void NestedIf ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
if:@/-/?node
  if:@/?name
    set:@/../*/_x/?value
      :x
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void ConsecutiveIfStatements ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
if:@/-/?node
if:@/?name
  set:@/../*/_x/?value
    :x
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("x", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfEqualsStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:x
if:@/-/?value
  =:x
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfNotEqualsStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:x
if:@/-/?value
  !=:z
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfMoreThanStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:b
if:@/-/?value
  >:a
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfLessThanStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:a
if:@/-/?value
  <:b
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfMoreThanEqualStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:a
if:@/-/?value
  >=:a
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfLessThanEqualStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:a
if:@/-/?value
  <=:a
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfTypesComparisonStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:int:5
if:@/-/?value
  =:int:5
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfTypesDifferentComparisonStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:5
if:@/-/?value
  =:int:5
  lambda
    set:@/../_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfOrComparisonStatementYieldsTrue ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:5
if:@/-/?value
  =:4
  or:@/./-/?value
    =:5
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfOrComparisonStatementYieldsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:5
if:@/-/?value
  =:4
  or:@/./-/?value
    =:6
  lambda
    set:@/../_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfAndComparisonStatementYieldsTrue ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:5
if:@/-/?value
  =:5
  and:@/./-/?name
    =:_x
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfAndComparisonStatementYieldsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:5
if:@/-/?value
  =:5
  and:@/./-/?name
    =:_y
  lambda
    set:@/../_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementAndPrecedence1 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:5
if:@/-/?value
  =:4
  or:@/./-/?value
    =:5
  and:@/./-/?name
    =:_y
  lambda
    set:@/../_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementAndPrecedence2 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:5
if:@/-/?value
  =:5
  and:@/./-/?name
    =:_y
  or:@/./-/?name
    =:_x
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementNestedConditions ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:5
if:@/-/?value
  =:0
  or:@/../*/_x/?value
    =:1
    or:@/../*/_x/?value
      =:2
      or:@/../*/_x/?value
        =:3
        or:@/../*/_x/?value
          =:4
          or:@/../*/_x/?value
            =:5
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementNestedConditionsFalseElseIfTrue ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:6
if:@/-/?value
  =:0
  or:@/../*/_x/?value
    =:1
    or:@/../*/_x/?value
      =:2
      or:@/../*/_x/?value
        =:3
        or:@/../*/_x/?value
          =:4
          or:@/../*/_x/?value
            =:5
  lambda
    set:@/../*/_x/?value
      :y
else-if:@/-2/?value
  =:6
  lambda
    set:@/../*/_x/?value
      :q
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("q", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementCompareNodesYieldsTrue ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  y:5
  z:int:6
_x
  y:5
  z:int:6
if:@/-2/?node
  =:@/./-1/?node
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementCompareNodesYieldsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  y:5
  z:int:6
    q:w
_x
  y:5
  z:int:6
    q:g
if:@/-2/?node
  =:@/./-1/?node
  lambda
    set:@/../_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementComparePathsYieldsTrue ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
if:@/-1/?path
  =:@/./-1/?path
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementComparePathsYieldsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
if:@/?path
  =:@/./-1/?path
  lambda
    set:@/../_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparePathsLessThanYieldsTrue ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
if:@/-/?path
  <:@/./?path
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparePathsLessThanYieldsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
if:@/?path
  <:@/./-/?path
  lambda
    set:@/../_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementCompareCountYieldsTrue ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  _x
  _x
_x
  _x
  _x
if:@/-2/**/?count
  =:@/./-1/**/?count
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementCompareCountYieldsFalse ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  _x
  _x
_x
  _x
  _x
  _x
if:@/-2/**/?count
  =:@/./-1/**/?count
  lambda
    set:@/../_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonIntLessThan ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:int:5
if:@/-/?value
  <:int:6
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonIntMoreThan ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:int:5
if:@/-/?value
  >:int:4
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonMultipleLambdas ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:int:5
if:@/-/?value
  =:int:5
  lambda
    set:@/../*/_x/?value
      :y
  lambda
    set:@/../*/_x/?name
      :_y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
            Assert.AreEqual ("_y", tmp [0].Name, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonLambdaExpression ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x:int:5
if:@/-/?value
  =:int:5
  lambda:@/../*/_exe/?node
_exe
  set:@/../*/_x/?value
    :y
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
    } // TODO: Constants on "left-hand-side", constant "exist" expressions, expressions against expressions, and "syntax error tests"
}
