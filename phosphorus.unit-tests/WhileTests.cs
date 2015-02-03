
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
    public class WhileTests : TestBase
    {
        public WhileTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.types");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void WhileFalseNoContent ()
        {
            ExecuteLambda (@"while:@/-/?node");
        }

        [Test]
        public void WhileFalseVerifyNoExecute ()
        {
            Node tmp = ExecuteLambda (@"_x
while:@/-/*/?node
  set:@/../_x/?value
    source:y");
            Assert.AreNotEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileTrueVerifyExecute ()
        {
            Node tmp = ExecuteLambda (@"_x
while:!
  :@/./-/?value
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileTrueAndTrueVerifyExecute ()
        {
            Node tmp = ExecuteLambda (@"_x
while:!
  :@/./-/?value
  and:!
    :@/../*/_y/?value
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileTrueOrTrueVerifyExecuteTwice ()
        {
            Node tmp = ExecuteLambda (@"_x
while:!
  :@/./-/?value
  or:!
    :@/../*/_y/?value
  lambda
    if:!
      :@/../*/_x/?value
      lambda
        add:@/../?node
          source
            _y:val
    else
      set:@/../*/_y/?value
  lambda
    set:@/../*/_x/?value
      source:y");
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesCountExist ()
        {
            Node tmp = ExecuteLambda (@"_x
  :a
  :b
  :c
while:@/../*/_x/*//?count
  set:@/../*/_x/*//[,1]/?name
    source:_y");
            Assert.AreEqual ("_y", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_y", tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_y", tmp [0] [2].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesExist ()
        {
            Node tmp = ExecuteLambda (@"_x
  :a
  :b
  :c
while:@/../*/_x/*/?node
  set:@/../*/_x/0/?node");
            Assert.AreEqual (0, tmp [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesCountMoreThan1 ()
        {
            Node tmp = ExecuteLambda (@"_x
  :a
  :b
  :c
while:@/../*/_x/*/?count
  >:int:1
  lambda
    set:@/../*/_x/0/?node");
            Assert.AreEqual (1, tmp [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesCountMoreEqualsString3 ()
        {
            Node tmp = ExecuteLambda (@"_x
  :a
  :b
  :c
while:@/../_x/*/?count
  =:3
  lambda
    set:@/../_x/0/?node");
            Assert.AreEqual (3, tmp [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesCountMoreEqualsInt3 ()
        {
            Node tmp = ExecuteLambda (@"_x
  :a
  :b
  :c
while:@/../*/_x/*/?count
  =:int:3
  lambda
    set:@/../*/_x/0/?node");
            Assert.AreEqual (2, tmp [0].Count, "wrong value of node after executing lambda object");
        }
    }
}
