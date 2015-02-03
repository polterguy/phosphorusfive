
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
    public class ForEachTests : TestBase
    {
        public ForEachTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.types");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [Test]
        public void EmptyForEachNoResult ()
        {
            ExecuteLambda (@"for-each:@/-/?node");
        }

        [Test]
        public void ForEachOneResult ()
        {
            Node tmp = ExecuteLambda (@"_x
for-each:@/-/?node
  set:@/./*/__dp/#/?node");
            Assert.AreEqual (1, tmp.Count);
        }
        
        [Test]
        public void ForEachMultipleResults ()
        {
            Node tmp = ExecuteLambda (@"_x
  :x
  :y
for-each:@/-/*/?node
  set:@/./*/__dp/#/?value
    source:z");
            Assert.AreEqual ("z", tmp [0] [0].Value);
            Assert.AreEqual ("z", tmp [0] [1].Value);
        }
        
        [Test]
        public void NestedForEach ()
        {
            Node tmp = ExecuteLambda (@"_x
  :x
    :f
    :g
  :y
    :h
    :j
for-each:@/-/*/?node
  for-each:@/./*/__dp/#/*/?node
    set:@/./*/__dp/#/?value
      source:q
    set:@/././*/__dp/#/?value
      source:z");
            Assert.AreEqual ("z", tmp [0] [0].Value);
            Assert.AreEqual ("z", tmp [0] [1].Value);
            Assert.AreEqual ("q", tmp [0] [0] [0].Value);
            Assert.AreEqual ("q", tmp [0] [0] [1].Value);
            Assert.AreEqual ("z", tmp [0] [1].Value);
            Assert.AreEqual ("q", tmp [0] [1] [0].Value);
            Assert.AreEqual ("q", tmp [0] [1] [0].Value);
        }
        
        [Test]
        public void ForEachingNodeValueBeingNode ()
        {
            Node tmp = ExecuteLambda (@"_data:node:""_foo""
for-each:@/-/?value
  set:@/./*/__dp/#/?value
    source:bar");
            Assert.AreEqual ("bar", tmp [0].Get<Node> ().Value);
        }

        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            ExecuteLambda (@"_data:foo
for-each:@/-/?value");
        }
        
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            ExecuteLambda (@"for-each:mumbo-jumbo");
        }
    }
}
