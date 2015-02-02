
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
    public class BranchingTests : TestBase
    {
        public BranchingTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void EmptyIfNoResult ()
        {
            ExecuteLambda (@"if:@/-/?node");
        }

        [Test]
        public void EmptyIfNoResultVerifyElseExecuted ()
        {
            Node tmp = ExecuteLambda (@"if:@/-/?node
else
  set:@/./+/?value
    source:x
_val");
            Assert.AreEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfTrueVerifyElseNotExecuted ()
        {
            Node tmp = ExecuteLambda (@"_x
if:@/-/?node
else
  set:@/./+/?value
    :x
_val");
            Assert.AreNotEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfTrueVerifyElseIfNotExecuted ()
        {
            Node tmp = ExecuteLambda (@"_x
if:@/-/?node
else-if:@/?name
  set:@/./+/?value
    :x
_val");
            Assert.AreNotEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfFalseVerifyElseIfExecuted ()
        {
            Node tmp = ExecuteLambda (@"if:@/-/?node
else-if:@/?name
  set:@/./+/?value
    source:x
_val");
            Assert.AreEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }

        [Test]
        public void IfFalseVerifyElseExecuted ()
        {
            Node tmp = ExecuteLambda (@"if:@/-/?node
else
  set:@/./+/?value
    source:x
_val");
            Assert.AreEqual ("x", tmp [2].Value, "node result after execution was not what was expected");
        }

        [Test]
        public void NestedIf ()
        {
            Node tmp = ExecuteLambda (@"_x
if:@/-/?node
  if:@/?name
    set:@/../*/_x/?value
      source:x");
            Assert.AreEqual ("x", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void ConsecutiveIfStatements ()
        {
            Node tmp = ExecuteLambda (@"_x
if:@/-/?node
if:@/?name
  set:@/../*/_x/?value
    source:x");
            Assert.AreEqual ("x", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfEqualsStatement ()
        {
            Node tmp = ExecuteLambda (@"_x:x
if:@/-/?value
  =:x
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfNotEqualsStatement ()
        {
            Node tmp = ExecuteLambda (@"_x:x
if:@/-/?value
  !=:z
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfMoreThanStatement ()
        {
            Node tmp = ExecuteLambda (@"_x:b
if:@/-/?value
  >:a
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfLessThanStatement ()
        {
            Node tmp = ExecuteLambda (@"_x:a
if:@/-/?value
  <:b
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfMoreThanEqualStatement ()
        {
            Node tmp = ExecuteLambda (@"_x:a
if:@/-/?value
  >=:a
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfLessThanEqualStatement ()
        {
            Node tmp = ExecuteLambda (@"_x:a
if:@/-/?value
  <=:a
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfTypesComparisonStatement ()
        {
            Node tmp = ExecuteLambda (@"_x:int:5
if:@/-/?value
  =:int:5
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfTypesDifferentComparisonStatement ()
        {
            Node tmp = ExecuteLambda (@"_x:5
if:@/-/?value
  =:int:5
  lambda
    set:@/../_x/?value
      :y");
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfOrComparisonStatementYieldsTrue ()
        {
            Node tmp = ExecuteLambda (@"_x:5
if:@/-/?value
  =:4
  or:@/./-/?value
    =:5
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfOrComparisonStatementYieldsFalse ()
        {
            Node tmp = ExecuteLambda (@"_x:5
if:@/-/?value
  =:4
  or:@/./-/?value
    =:6
  lambda
    set:@/../_x/?value
      :y");
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfAndComparisonStatementYieldsTrue ()
        {
            Node tmp = ExecuteLambda (@"_x:5
if:@/-/?value
  =:5
  and:@/./-/?name
    =:_x
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfAndComparisonStatementYieldsFalse ()
        {
            Node tmp = ExecuteLambda (@"_x:5
if:@/-/?value
  =:5
  and:@/./-/?name
    =:_y
  lambda
    set:@/../_x/?value
      :y");
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementAndPrecedence1 ()
        {
            Node tmp = ExecuteLambda (@"_x:5
if:@/-/?value
  =:4
  or:@/./-/?value
    =:5
  and:@/./-/?name
    =:_y
  lambda
    set:@/../_x/?value
      :y");
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementAndPrecedence2 ()
        {
            Node tmp = ExecuteLambda (@"_x:5
if:@/-/?value
  =:5
  and:@/./-/?name
    =:_y
  or:@/./-/?name
    =:_x
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementNestedConditions ()
        {
            Node tmp = ExecuteLambda (@"_x:5
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
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementNestedConditionsFalseElseIfTrue ()
        {
            Node tmp = ExecuteLambda (@"_x:6
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
      source:y
else-if:@/-2/?value
  =:6
  lambda
    set:@/../*/_x/?value
      source:q");
            Assert.AreEqual ("q", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementCompareNodesYieldsTrue ()
        {
            Node tmp = ExecuteLambda (@"_x
  y:5
  z:int:6
_x
  y:5
  z:int:6
if:@/-2/?node
  =:@/./-1/?node
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementCompareNodesYieldsFalse ()
        {
            Node tmp = ExecuteLambda (@"_x
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
      :y");
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementComparePathsYieldsTrue ()
        {
            Node tmp = ExecuteLambda (@"_x
if:@/-1/?path
  =:@/./-1/?path
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementComparePathsYieldsFalse ()
        {
            Node tmp = ExecuteLambda (@"_x
if:@/?path
  =:@/./-1/?path
  lambda
    set:@/../_x/?value
      :y");
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparePathsLessThanYieldsTrue ()
        {
            Node tmp = ExecuteLambda (@"_x
if:@/-/?path
  <:@/./?path
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparePathsLessThanYieldsFalse ()
        {
            Node tmp = ExecuteLambda (@"_x
if:@/?path
  <:@/./-/?path
  lambda
    set:@/../_x/?value
      :y");
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementCompareCountYieldsTrue ()
        {
            Node tmp = ExecuteLambda (@"_x
  _x
  _x
_x
  _x
  _x
if:@/-2/**/?count
  =:@/./-1/**/?count
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonStatementCompareCountYieldsFalse ()
        {
            Node tmp = ExecuteLambda (@"_x
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
      :y");
            Assert.AreNotEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonIntLessThan ()
        {
            Node tmp = ExecuteLambda (@"_x:int:5
if:@/-/?value
  <:int:6
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonIntMoreThan ()
        {
            Node tmp = ExecuteLambda (@"_x:int:5
if:@/-/?value
  >:int:4
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonMultipleLambdas ()
        {
            Node tmp = ExecuteLambda (@"_x:int:5
if:@/-/?value
  =:int:5
  lambda
    set:@/../*/_x/?value
      source:y
  lambda
    set:@/../*/_x/?name
      source:_y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
            Assert.AreEqual ("_y", tmp [0].Name, "node result after execution was not what was expected");
        }
        
        [Test]
        public void IfComparisonLambdaExpression ()
        {
            Node tmp = ExecuteLambda (@"_x:int:5
if:@/-/?value
  =:int:5
  lambda:@/../*/_exe/?node
_exe
  set:@/../*/_x/?value
    source:y");
            Assert.AreEqual ("y", tmp [0].Value, "node result after execution was not what was expected");
        }
    } // TODO: Constants on "left-hand-side", constant "exist" expressions, expressions against expressions, and "syntax error tests"
}
