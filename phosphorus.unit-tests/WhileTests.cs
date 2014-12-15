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
    public class WhileTests
    {
        [Test]
        public void WhileFalseNoContent ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
while:@/-/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }

        [Test]
        public void WhileFalseVerifyNoExecute ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
while:@/-/*/?node
  set:@/../_x/?value
    :y";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreNotEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileTrueVerifyExecute ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
while:!
  :@/./-/?value
  lambda
    set:@/../*/_x/?value
      :y";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileTrueAndTrueVerifyExecute ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
while:!
  :@/./-/?value
  and:!
    :@/../*/_y/?value
  lambda
    set:@/../*/_x/?value
      :y";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileTrueOrTrueVerifyExecuteTwice ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
while:!
  :@/./-/?value
  or:!
    :@/../*/_y/?value
  lambda
    if:!
      :@/../*/_x/?value
      lambda
        add:@/../?node
          _y:val
    else
      set:@/../*/_y/?value
  lambda
    set:@/../*/_x/?value
      :y
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("y", tmp [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesCountExist ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  :a
  :b
  :c
while:@/../*/_x/*//?count
  set:@/../*/_x/*//[,1]/?name
    :_y
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("_y", tmp [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_y", tmp [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_y", tmp [0] [2].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesExist ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  :a
  :b
  :c
while:@/../*/_x/*/?node
  set:@/../*/_x/0/?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (0, tmp [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesCountMoreThan1 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  :a
  :b
  :c
while:@/../*/_x/*/?count
  >:int:1
  lambda
    set:@/../*/_x/0/?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesCountMoreEqualsString3 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  :a
  :b
  :c
while:@/../_x/*/?count
  =:3
  lambda
    set:@/../_x/0/?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (3, tmp [0].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void WhileNodesCountMoreEqualsInt3 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  :a
  :b
  :c
while:@/../*/_x/*/?count
  =:int:3
  lambda
    set:@/../*/_x/0/?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (2, tmp [0].Count, "wrong value of node after executing lambda object");
        }
    }
}

