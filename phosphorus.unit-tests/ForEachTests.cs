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
    public class ForEachTests
    {
        [Test]
        public void EmptyForEachNoResult ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
for-each:@/-/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }

        [Test]
        public void ForEachOneResult ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
for-each:@/-/?node
  set:@/./__dp/#/?node";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp.Count);
        }
        
        [Test]
        public void ForEachMultipleResults ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  :x
  :y
for-each:@/-/*/?node
  set:@/./__dp/#/?value
    :z";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("z", tmp [0] [0].Value);
            Assert.AreEqual ("z", tmp [0] [1].Value);
        }
        
        [Test]
        public void NestedForEach ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
_x
  :x
    :f
    :g
  :y
    :h
    :j
for-each:@/-/*/?node
  for-each:@/./__dp/#/*/?node
    set:@/./__dp/#/?value
      :q
    set:@/././__dp/#/?value
      :z";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("z", tmp [0] [0].Value);
            Assert.AreEqual ("z", tmp [0] [1].Value);
            Assert.AreEqual ("q", tmp [0] [0] [0].Value);
            Assert.AreEqual ("q", tmp [0] [0] [1].Value);
            Assert.AreEqual ("z", tmp [0] [1].Value);
            Assert.AreEqual ("q", tmp [0] [1] [0].Value);
            Assert.AreEqual ("q", tmp [0] [1] [0].Value);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"for-each:@/-/*/?value";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"for-each:@/-/*/?path";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"for-each:@/-/*/?count";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"for-each:@/-/*/?name";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError5 ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"for-each:mumbo-jumbo";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
        }
    }
}

