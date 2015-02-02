
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
    public class AddTests : TestBase
    {
        public AddTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void AddStaticNodes ()
        {
            Node tmp = ExecuteLambda (@"_out
add:@/-/?node
  source
    x:y
      z:q");
            Assert.AreEqual ("x", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [0] [0] [0].Value, "wrong value of node after executing lambda object");

            // making sure source nodes are still around, and that add adds a copy of the nodes to add
            Assert.AreEqual ("x", tmp [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [1] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [1] [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [1] [0] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void AddNodesFromExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
add:@/-/?node
  source:@/./+/*/?node
_x
  x:y
    z:q");
            Assert.AreEqual ("x", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [0] [0] [0].Value, "wrong value of node after executing lambda object");

            // making sure source nodes are still around, and that add adds a copy of the nodes to add
            Assert.AreEqual ("x", tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [2] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [2] [0] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void AddNodesFromFormattedSourceExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
add:@/-/?node
  source:@/{0}/{1}/*/?node
    :.
    :+
_x
  x:y
    z:q");
            Assert.AreEqual ("x", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [0] [0] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void AddNodesFromFormatedDestinationExpression ()
        {
            Node tmp = ExecuteLambda (@"_out
add:@/{0}/?node
  :-
  source:@/./+/*/?node
_x
  x:y
    z:q");
            Assert.AreEqual ("x", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("z", tmp [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("q", tmp [0] [0] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void MultipleDestinationsWithExpressionSource ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
add:@/-/|/-/-/?node
  source:@/./+/?node
:dfg");
            Assert.AreEqual ("dfg", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("dfg", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", tmp [1] [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void MultipleDestinationsWithStaticSource ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
add:@/-/|/-/-/?node
  source
    :dfg");
            Assert.AreEqual ("dfg", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("dfg", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("", tmp [1] [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void MultipleStaticSourceNodesWithMultipleDestinations ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
add:@/-/|/-/-/?node
  source
    _x1:y1
    _x2:y2");
            Assert.AreEqual ("_x1", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x1", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [1] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [1] [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void MultipleSourceNodesExpressionWithMultipleDestinations ()
        {
            Node tmp = ExecuteLambda (@"_out1
_out2
add:@/-/|/-/-/?node
  source:@/./+/|/./+/+/?node
_x1:y1
_x2:y2");
            Assert.AreEqual ("_x1", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x1", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [1] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [1] [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SourceIsAlsoDestination ()
        {
            Node tmp = ExecuteLambda (@"add:@/+/|/+/+/?node
  source:@/./+/|/./+/+/?node
_x1:y1
_x2:y2");

            // asserting old nodes is unchanged
            Assert.AreEqual ("_x1", tmp [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (2, tmp [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [2].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (2, tmp [2].Count, "wrong value of node after executing lambda object");

            // asserting new nodes are appended correctly into children's collection
            Assert.AreEqual ("_x1", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [1] [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [1] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [1] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [1] [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x1", tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y1", tmp [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [2] [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_x2", tmp [2] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y2", tmp [2] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [2] [1].Count, "wrong value of node after executing lambda object");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            ExecuteLambda (@"_out
add:@/-/?value
  source:@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            ExecuteLambda (@"_out
add:@/-/?name
  source:@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            ExecuteLambda (@"_out
add:@/-/?count
  source:@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            ExecuteLambda (@"_out
add:@/-/?path
  source:@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError5 ()
        {
            ExecuteLambda (@"_out
add:@/-/?node");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError6 ()
        {
            ExecuteLambda (@"_out
add:@/-/?node
  source:@/./+/*/?value
_x
  x:y
    z:q
  x2:y2");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError7 ()
        {
            ExecuteLambda (@"_out
add:@/-/?node
  source:@/./+/*/**/?name
_x
  x:y
    z:q
  x2:y2");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError8 ()
        {
            ExecuteLambda (@"_out
add:@/-/?node
  source:@/./+/*/**/?path
_x
  x:y
    z:q
  x2:y2");
        }

        [Test]
        [ExpectedException]
        public void SyntaxError9 ()
        {
            ExecuteLambda (@"_out
add:@/-/?node
  source:@/./+/*/**/?count
_x
  x:y
    z:q
  x2:y2");
        }
    }
}
