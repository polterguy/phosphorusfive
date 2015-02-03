
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
    public class SetTests : TestBase
    {
        public SetTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void SetValueFromConstantString ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:y");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetValueFromConstantInteger ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:int:5");
            Assert.AreEqual (5, tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameFromConstantString ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?name
  source:y");
            Assert.AreEqual ("y", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameFromConstantInteger ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?name
  source:int:5");
            Assert.AreEqual ("5", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNodeFromConstant ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?node
  source
    x:y");
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetValueFromExpressionString ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:@/./+/?value
_source:x");
            Assert.AreEqual ("x", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetValueFromExpressionInteger ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:@/./+/?value
_source:int:5");
            Assert.AreEqual (5, tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameFromExpressionString ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?name
  source:@/./+/?value
_source:x");
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetNameFromExpressionInteger ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?name
  source:@/./+/?value
_source:int:5");
            Assert.AreEqual ("5", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNodeFromExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?node
  source:@/./+/?node
x:y");
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetValueFromPathExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:@/./+/?path
_source:x");
            Assert.AreEqual (new Node.DNA ("2"), tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetValueFromCountExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:@/../*/**/?count");
            Assert.AreEqual (3, tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetValueFromNodeExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:@/./+/?node
x:y");
            Assert.AreEqual ("x", tmp [0].Get<Node> ().Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Get<Node> ().Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameFromPathExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?name
  source:@/./+/?path
_source:x");
            Assert.AreEqual ("2", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameFromCountExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?name
  source:@/../*/**/?count");
            Assert.AreEqual ("3", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetMultipleValuesFromConstant ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
set:@/-/|/-/-/?value
  source:x");
            Assert.AreEqual ("x", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetMultipleValuesFromExpression ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
set:@/-/|/-/-/?value
  source:@/./+/?value
_source:x");
            Assert.AreEqual ("x", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleNodesFromConstant ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
set:@/-/|/-/-/?node
  source
    x:y");
            Assert.AreEqual ("x", tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleNodesFromExpression ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
set:@/-/|/-/-/?node
  source:@/./+/?node
_x:y");
            Assert.AreEqual ("_x", tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x", tmp [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetNameToNull ()
        {
            Node tmp = ExecuteLambda (@"_out1
set:@/-/?name");
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetValueToNull ()
        {
            Node tmp = ExecuteLambda (@"_out1:x
set:@/-/?value");
            Assert.AreEqual (null, tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetNodeToNull ()
        {
            Node tmp = ExecuteLambda (@"_out1:x
set:@/-/?node");
            Assert.AreEqual (1, tmp.Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("set", tmp [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SetMultipleNamesToNull ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out1
set:@/-/|/-/-/?name");
            Assert.AreEqual (string.Empty, tmp [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [1].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetMultipleValuesToNull ()
        {
            Node tmp = ExecuteLambda (@"_out1:x
_out1:x
set:@/-/|/-/-/?value");
            Assert.AreEqual (null, tmp [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SetMultipleNodesToNull ()
        {
            Node tmp = ExecuteLambda (@"_out1:x
_out2:x
set:@/-/|/-/-/?node");
            Assert.AreEqual (1, tmp.Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("set", tmp [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SourceIsFormattingExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:y{0}
    :x");
            Assert.AreEqual ("yx", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void DestinationIsFormattingExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/{0}/?value
  :-
  source:y");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void DestinationIsRecursiveFormattingExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/../*/{0}/?value
  :{0}{1}
    :_o
    :ut
  source:y");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void RelativeSource ()
        {
            Node tmp = ExecuteLambda (@"_out
  _value:success
set:@/../*/_out/?value
  rel-source:@/*/_value/?value");
            Assert.AreEqual ("success", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void RelativeFormattedSource ()
        {
            Node tmp = ExecuteLambda (@"_out
  _value:success
set:@/../*/_out/?value
  rel-source:@/*/{0}/?value
    :_value");
            Assert.AreEqual ("success", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void RelativeFormattedSourceExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
  _value:success
set:@/../*/_out/?value
  rel-source:@/*/{0}/?value
    :@/../*/_out/0/?name");
            Assert.AreEqual ("success", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void RelativeFormattedSourceExpressionAndFormattedDestination ()
        {
            Node tmp = ExecuteLambda (@"_out
  _value:success
set:@/{0}/*/_out/?value
  :..
  rel-source:@/*/{0}/?value
    :@/../*/_out/0/?name");
            Assert.AreEqual ("success", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SourceIsEscapedExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
set:@/-/?value
  source:\@/../?value");
            Assert.AreEqual ("@/../?value", tmp [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            ExecuteLambda (@"_out1
set:@/-/?node
  source:@/./+/|/./+/+/?node
_x1:f
_x2:g");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            ExecuteLambda (@"_out1
set:@/-/?path");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            ExecuteLambda (@"_out1
set:@/-/?count");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            ExecuteLambda (@"_out1
set:@/-/?value
  source:@/./+/|/./+/+/?value
_x1:f
_x2:g");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError5 ()
        {
            ExecuteLambda (@"_out1
set:@/-/?name
  source:@/./+/|/./+/+/?value
_x1:f
_x2:g");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError6 ()
        {
            ExecuteLambda (@"_out1
set:@/-/?node
  source:@/./+/|/./+/+/?value
_x1:f
_x2:g");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError7 ()
        {
            ExecuteLambda (@"_out1
set:@/-/?value
  source:@/./+/?value
  source:@/./+/?value
_x1:f");
        }
    }
}
