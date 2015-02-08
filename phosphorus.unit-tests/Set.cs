
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    /// <summary>
    /// unit tests for testing the [set] lambda keyword
    /// </summary>
    [TestFixture]
    public class Set : TestBase
    {
        public Set ()
            : base ("phosphorus.lambda")
        { }

        /// <summary>
        /// verifies [set] works with a static constant source
        /// </summary>
        [Test]
        public void Set01 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "success").Root;
            _context.Raise ("set", node [1]);
            
            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works with a static expression source
        /// </summary>
        [Test]
        public void Set02 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add("success")
                .Add ("set", "@/-2/?value").LastChild
                    .Add ("source", "@/./-/?name").Root;
            _context.Raise ("set", node [2]);
            
            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        /// verifies [set] works with a formatted destination and a static constant source
        /// </summary>
        [Test]
        public void Set03 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?{0}").LastChild
                    .Add(string.Empty, "value")
                    .Add ("source", "success").Root;
            _context.Raise ("set", node [1]);
            
            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works with a formatted static constant source
        /// </summary>
        [Test]
        public void Set04 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "{0}{1}{2}").LastChild
                        .Add (string.Empty, "su")
                        .Add (string.Empty, "cc")
                        .Add (string.Empty, "ess").Root;
            _context.Raise ("set", node [1]);
            
            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works with a formatted static expression source
        /// </summary>
        [Test]
        public void Set05 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add("success")
                .Add ("set", "@/-2/?value").LastChild
                    .Add ("source", "@{0}?name").LastChild
                        .Add (string.Empty, "/./-/").Root;
            _context.Raise ("set", node [2]);
            
            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        /// verifies [set] works when destination is 'name'
        /// </summary>
        [Test]
        public void Set06 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?name").LastChild
                    .Add ("source", "success").Root;
            _context.Raise ("set", node [1]);
            
            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Name);
        }

        /// <summary>
        /// verifies [set] works when destination is 'node' and source is static expression
        /// </summary>
        [Test]
        public void Set07 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add("_data2", "success")
                .Add ("set", "@/-2/?node").LastChild
                    .Add ("source", "@/./-/?node").Root;
            _context.Raise ("set", node [2]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data2", node [0].Name);
            Assert.AreEqual ("success", node [0].Value);

            // verifying [source] is still around
            Assert.AreEqual ("_data2", node [1].Name);
            Assert.AreEqual ("success", node [1].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is 'node' and source is static constant
        /// </summary>
        [Test]
        public void Set08 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?node").LastChild
                    .Add ("source").LastChild
                        .Add ("_data2", "success").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data2", node [0].Name);
            Assert.AreEqual ("success", node [0].Value);

            // verifying [source] is still around
            Assert.AreEqual ("_data2", node [1] [0] [0].Name);
            Assert.AreEqual ("success", node [1] [0] [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is 'node' and source is static constant as reference node
        /// </summary>
        [Test]
        public void Set09 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?node").LastChild
                    .Add ("source", new Node ("_data2", "success")).Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data2", node [0].Name);
            Assert.AreEqual ("success", node [0].Value);

            // verifying [source] is still around
            Assert.AreEqual ("_data2", node [1] [0].Get<Node> (_context).Name);
            Assert.AreEqual ("success", node [1] [0].Get<Node> (_context).Value);
        }
        
        /// <summary>
        /// verifies [set] works when source is static constant integer
        /// </summary>
        [Test]
        public void Set10 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", 5).Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (5, node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works when source is static constant integer and destination is 'name'
        /// </summary>
        [Test]
        public void Set11 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?name").LastChild
                    .Add ("source", 5).Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("5", node [0].Name);
        }
        
        /// <summary>
        /// verifies [set] works when destination is value and there is no source
        /// </summary>
        [Test]
        public void Set12 ()
        {
            Node node = new Node ()
                .Add ("_data", "error")
                .Add ("set", "@/-/?value").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.IsNull (node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is name and there is no source
        /// </summary>
        [Test]
        public void Set13 ()
        {
            Node node = new Node ()
                .Add ("error")
                .Add ("set", "@/-/?name").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (string.Empty, node [0].Name);
        }
        
        /// <summary>
        /// verifies [set] works when destination is node and there is no source
        /// </summary>
        [Test]
        public void Set14 ()
        {
            Node node = new Node ()
                .Add ("error")
                .Add ("set", "@/-/?node").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (1, node.Count);
        }
        
        /// <summary>
        /// verifies [set] works when destination is value and source is null
        /// </summary>
        [Test]
        public void Set15 ()
        {
            Node node = new Node ()
                .Add ("_data", "error")
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.IsNull (node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is name and source is null
        /// </summary>
        [Test]
        public void Set16 ()
        {
            Node node = new Node ()
                .Add ("error")
                .Add ("set", "@/-/?name").LastChild
                    .Add ("source").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (string.Empty, node [0].Name);
        }

        /// <summary>
        /// verifies [set] works when destination is node and source is null
        /// </summary>
        [Test]
        public void Set17 ()
        {
            Node node = new Node ()
                .Add ("error")
                .Add ("set", "@/-/?node").LastChild
                    .Add ("source").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (1, node.Count);
        }
        
        /// <summary>
        /// verifies [set] works when destination is value and source is 'count' expression
        /// </summary>
        [Test]
        public void Set18 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "@/../**/?count").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (4, node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is name and source is 'count' expression
        /// </summary>
        [Test]
        public void Set19 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?name").LastChild
                    .Add ("source", "@/../**/?count").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("4", node [0].Name);
        }
        
        /// <summary>
        /// verifies [set] works when destination is value,
        /// and source is static expression yielding multiple values
        /// </summary>
        [Test]
        public void Set20 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("succ")
                    .Add ("ess").Parent
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "@/./-/*/?name").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is value,
        /// and source is static expression yielding multiple
        /// values with different types
        /// </summary>
        [Test]
        public void Set21 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add (string.Empty, "succ")
                    .Add (string.Empty, 5)
                    .Add (string.Empty, "ess").Parent
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "@/./-/*/?value").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("succ5ess", node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is name,
        /// and source is static expression yielding multiple
        /// values with different types
        /// </summary>
        [Test]
        public void Set22 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add (string.Empty, "succ")
                    .Add (string.Empty, 5)
                    .Add (string.Empty, "ess").Parent
                .Add ("set", "@/-/?name").LastChild
                    .Add ("source", "@/./-/*/?value").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("succ5ess", node [0].Name);
        }
        
        /// <summary>
        /// verifies [set] works when destination is value,
        /// and source is static expression yielding 'node'
        /// </summary>
        [Test]
        public void Set23 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("succ")
                    .Add ("ess").Parent
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "@/./-/?node").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data", node [0].Get<Node> (_context).Name);
            Assert.AreEqual ("succ", node [0].Get<Node> (_context) [0].Name);
            Assert.AreEqual ("ess", node [0].Get<Node> (_context) [1].Name);
        }
        
        /// <summary>
        /// verifies [set] works when destination is name,
        /// and source is static node
        /// </summary>
        [Test]
        public void Set24 ()
        {
            // since we're dependent upon Node to string conversion here, we'll need
            // a couple of additional assemblies
            Loader.Instance.LoadAssembly ("phosphorus.types");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            _context = Loader.Instance.CreateApplicationContext ();

            try {
                Node node = new Node ()
                    .Add ("_data").LastChild
                        .Add ("succ")
                        .Add ("ess").Parent
                    .Add ("set", "@/-/?name").LastChild
                        .Add ("source", "@/./-/?node").Root;
                _context.Raise ("set", node [1]);

                // verifying [set] works as it should
                Assert.AreEqual ("_data\r\n  succ\r\n  ess", node [0].Name);
            } finally {

                // making sure we "unload" our extra assemblies here
                Loader.Instance.UnloadAssembly ("phosphorus.types");
                Loader.Instance.UnloadAssembly ("phosphorus.hyperlisp");
                _context = Loader.Instance.CreateApplicationContext ();
            }
        }
        
        /// <summary>
        /// verifies [set] works when destination is value,
        /// and source is static expression yielding no results
        /// </summary>
        [Test]
        public void Set25 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "@/mumbo/?value").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.IsNull (node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is name,
        /// and source is relative expression
        /// </summary>
        [Test]
        public void Set26 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add("_1", "success1")
                    .Add("_2", "success2").Parent
                .Add ("set", "@/-/*/?name").LastChild
                    .Add ("rel-source", "@?value").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success1", node [0] [0].Name);
            Assert.AreEqual ("success2", node [0] [1].Name);
        }
        
        /// <summary>
        /// verifies [set] works when destination is value,
        /// and source is relative expression
        /// </summary>
        [Test]
        public void Set27 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add("success1")
                    .Add("success2").Parent
                .Add ("set", "@/-/*/?value").LastChild
                    .Add ("rel-source", "@?name").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is node,
        /// and source is relative expression, where source is child of destination
        /// </summary>
        [Test]
        public void Set28 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add(string.Empty).LastChild
                        .Add("_1", "success1").Parent
                    .Add(string.Empty).LastChild
                        .Add("_2", "success2").Parent.Parent
                .Add ("set", "@/-/*/?node").LastChild
                    .Add ("rel-source", "@/0/?node").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (0, node [0] [0].Count);
            Assert.AreEqual ("_1", node [0] [0].Name);
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual (0, node [0] [1].Count);
            Assert.AreEqual ("_2", node [0] [1].Name);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is node,
        /// and source is relative formatted expression
        /// </summary>
        [Test]
        public void Set29 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add("_1").LastChild
                        .Add("_1", "success1").Parent
                    .Add("_2").LastChild
                        .Add("_2", "success2").Parent.Parent
                .Add ("set", "@/-/*/?node").LastChild
                    .Add ("rel-source", "@/{0}/{1}/?node").LastChild
                        .Add(string.Empty, "*")
                        .Add(string.Empty, "@?name").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is formatted node expression,
        /// and source is relative formatted expression, and one of sources yields
        /// no result
        /// </summary>
        [Test]
        public void Set31 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add("_1").LastChild
                        .Add("_1", "success1").Parent
                    .Add("_2").LastChild
                        .Add("_2", "success2").Parent
                    .Add("_3").LastChild
                        .Add("_ERROR2").Parent.Parent // intentionally returns "null" to verify [_3] is deleted
                .Add ("set", "@/{0}/*/?node").LastChild
                    .Add(string.Empty, "-")
                    .Add ("rel-source", "@/{0}/{1}/?node").LastChild
                        .Add(string.Empty, "*")
                        .Add(string.Empty, "@?{0}").LastChild
                            .Add(string.Empty, "name").Root; // recursive formatting expression
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }
        
        /// <summary>
        /// verifies [set] works when destination is node,
        /// and source is relative expression, where source is parent of destination
        /// </summary>
        [Test]
        public void Set32 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add("success1").LastChild
                        .Add("success11").Parent
                    .Add("success2").LastChild
                        .Add("success21").Parent.Parent
                .Add ("set", "@/-/*/*/?node").LastChild
                    .Add ("rel-source", "@/./?node").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success1", node [0] [0].Name);
            Assert.AreEqual ("success1", node [0] [0] [0].Name);
            Assert.AreEqual ("success11", node [0] [0] [0] [0].Name);
            Assert.AreEqual ("success2", node [0] [1].Name);
            Assert.AreEqual ("success2", node [0] [1] [0].Name);
            Assert.AreEqual ("success21", node [0] [1] [0] [0].Name);
        }
        
        /// <summary>
        /// verifies [set] works when destination is node,
        /// and source is relative expression, leading to a string, which should
        /// be converted into a node
        /// </summary>
        [Test]
        public void Set33 ()
        {
            // since we're converting from string to node, we'll need these buggers
            Loader.Instance.LoadAssembly ("phosphorus.types");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            _context = Loader.Instance.CreateApplicationContext ();

            try {
                Node node = new Node ()
                    .Add ("_data").LastChild
                        .Add("success1").LastChild
                            .Add("_val1:success1").Parent
                        .Add("success2").LastChild
                            .Add("_val2:success2").Parent.Parent
                    .Add ("set", "@/-/*/?node").LastChild
                        .Add ("rel-source", "@/0/?name").Root;
                _context.Raise ("set", node [1]);

                // verifying [set] works as it should
                Assert.AreEqual (2, node [0].Count);
                Assert.AreEqual (0, node [0] [0].Count);
                Assert.AreEqual ("_val1", node [0] [0].Name);
                Assert.AreEqual ("success1", node [0] [0].Value);
                Assert.AreEqual (0, node [0] [1].Count);
                Assert.AreEqual ("_val2", node [0] [1].Name);
                Assert.AreEqual ("success2", node [0] [1].Value);
            } finally {

                // making sure we "unload" our extra assemblies here
                Loader.Instance.UnloadAssembly ("phosphorus.types");
                Loader.Instance.UnloadAssembly ("phosphorus.hyperlisp");
                _context = Loader.Instance.CreateApplicationContext ();
            }
        }
        
        /// <summary>
        /// verifies [set] works when destination is name,
        /// and source is relative expression, leading to a node, which should
        /// be converted into a string
        /// </summary>
        [Test]
        public void Set34 ()
        {
            // since we're converting from node to string, we'll need these buggers
            Loader.Instance.LoadAssembly ("phosphorus.types");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            _context = Loader.Instance.CreateApplicationContext ();

            try {
                Node node = new Node ()
                    .Add ("_data").LastChild
                        .Add("success1").LastChild
                            .Add("foo1", 5).Parent // making sure types works
                        .Add("success2").LastChild
                            .Add("foo2", new Node ("bar2", "x")).Parent // making sure recursive nodes works
                        .Add("success3").LastChild
                            .Add("foo3", "test1\r\ntest2").Parent.Parent // making sure CR/LF works
                    .Add ("set", "@/-/*/?name").LastChild
                        .Add ("rel-source", "@?node").Root;
                _context.Raise ("set", node [1]);

                // verifying [set] works as it should
                Assert.AreEqual (3, node [0].Count);
                Assert.AreEqual (1, node [0] [0].Count); // making sure source is still around
                Assert.AreEqual ("success1\r\n  foo1:int:5", node [0] [0].Name);
                Assert.AreEqual (1, node [0] [1].Count); // making sure source is still around
                Assert.AreEqual ("success2\r\n  foo2:node:\"bar2:x\"", node [0] [1].Name);
                Assert.AreEqual (1, node [0] [2].Count); // making sure source is still around
                Assert.AreEqual ("success3\r\n  foo3:@\"test1\r\ntest2\"", node [0] [2].Name);
            } finally {

                // making sure we "unload" our extra assemblies here
                Loader.Instance.UnloadAssembly ("phosphorus.types");
                Loader.Instance.UnloadAssembly ("phosphorus.hyperlisp");
                _context = Loader.Instance.CreateApplicationContext ();
            }
        }
        
        /// <summary>
        /// verifies [set] works when there are more than one destination
        /// </summary>
        [Test]
        public void Set35 ()
        {
            Node node = new Node ()
                .Add ("_data").LastChild
                    .Add ("_1")
                    .Add ("_2").Parent
                .Add ("set", "@/-/**/?value").LastChild
                    .Add ("source", "success").Root;
            _context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
            Assert.AreEqual ("success", node [0] [0].Value);
            Assert.AreEqual ("success", node [0] [1].Value);
        }
    }
}
