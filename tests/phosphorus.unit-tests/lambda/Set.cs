/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [set] lambda keyword
    /// </summary>
    [TestFixture]
    public class Set : TestBase
    {
        public Set ()
            : base ("phosphorus.lambda", "phosphorus.hyperlisp", "phosphorus.types", "phosphorus.math") { }

        /// <summary>
        ///     verifies [set] works with a static constant source
        /// </summary>
        [Test]
        public void Set01 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                .Add ("source", "success").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works with a static expression source
        /// </summary>
        [Test]
        public void Set02 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("success")
                .Add ("set", "@/-2/?value").LastChild
                .Add ("source", "@/./-/?name").Root;
            Context.Raise ("set", node [2]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works with a formatted destination and a static constant source
        /// </summary>
        [Test]
        public void Set03 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?{0}").LastChild
                .Add (string.Empty, "value")
                .Add ("source", "success").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works with a formatted static constant source
        /// </summary>
        [Test]
        public void Set04 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                .Add ("source", "{0}{1}{2}").LastChild
                .Add (string.Empty, "su")
                .Add (string.Empty, "cc")
                .Add (string.Empty, "ess").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works with a formatted static expression source
        /// </summary>
        [Test]
        public void Set05 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("success")
                .Add ("set", "@/-2/?value").LastChild
                .Add ("source", "@{0}?name").LastChild
                .Add (string.Empty, "/./-/").Root;
            Context.Raise ("set", node [2]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is 'name'
        /// </summary>
        [Test]
        public void Set06 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?name").LastChild
                .Add ("source", "success").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is 'node' and source is static expression
        /// </summary>
        [Test]
        public void Set07 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("_data2", "success")
                .Add ("set", "@/-2/?node").LastChild
                .Add ("source", "@/./-/?node").Root;
            Context.Raise ("set", node [2]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data2", node [0].Name);
            Assert.AreEqual ("success", node [0].Value);

            // verifying [source] is still around
            Assert.AreEqual ("_data2", node [1].Name);
            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is 'node' and source is static constant
        /// </summary>
        [Test]
        public void Set08 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?node").LastChild
                .Add ("source").LastChild
                .Add ("_data2", "success").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data2", node [0].Name);
            Assert.AreEqual ("success", node [0].Value);

            // verifying [source] is still around
            Assert.AreEqual ("_data2", node [1] [0] [0].Name);
            Assert.AreEqual ("success", node [1] [0] [0].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is 'node' and source is static constant as reference node
        /// </summary>
        [Test]
        public void Set09 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?node").LastChild
                .Add ("source", new Node ("_data2", "success")).Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data2", node [0].Name);
            Assert.AreEqual ("success", node [0].Value);

            // verifying [source] is still around
            Assert.AreEqual ("_data2", node [1] [0].Get<Node> (Context).Name);
            Assert.AreEqual ("success", node [1] [0].Get<Node> (Context).Value);
        }

        /// <summary>
        ///     verifies [set] works when source is static constant integer
        /// </summary>
        [Test]
        public void Set10 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                .Add ("source", 5).Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (5, node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works when source is static constant integer and destination is 'name'
        /// </summary>
        [Test]
        public void Set11 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?name").LastChild
                .Add ("source", 5).Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("5", node [0].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is value and there is no source
        /// </summary>
        [Test]
        public void Set12 ()
        {
            var node = new Node ()
                .Add ("_data", "error")
                .Add ("set", "@/-/?value").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.IsNull (node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is name and there is no source
        /// </summary>
        [Test]
        public void Set13 ()
        {
            var node = new Node ()
                .Add ("error")
                .Add ("set", "@/-/?name").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (string.Empty, node [0].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is node and there is no source
        /// </summary>
        [Test]
        public void Set14 ()
        {
            var node = new Node ()
                .Add ("error")
                .Add ("set", "@/-/?node").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (1, node.Count);
        }

        /// <summary>
        ///     verifies [set] works when destination is value and source is 'count' expression
        /// </summary>
        [Test]
        public void Set18 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                .Add ("source", "@/../**/?count").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (4, node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is name and source is 'count' expression
        /// </summary>
        [Test]
        public void Set19 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?name").LastChild
                .Add ("source", "@/../**/?count").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("4", node [0].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is value,
        ///     and source is static expression yielding multiple values
        /// </summary>
        [Test]
        public void Set20 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("succ")
                .Add ("ess").Parent
                .Add ("set", "@/-/?value").LastChild
                .Add ("source", "@/./-/*/?name").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is value,
        ///     and source is static expression yielding multiple
        ///     values with different types
        /// </summary>
        [Test]
        public void Set21 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add (string.Empty, "succ")
                .Add (string.Empty, 5)
                .Add (string.Empty, "ess").Parent
                .Add ("set", "@/-/?value").LastChild
                .Add ("source", "@/./-/*/?value").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("succ5ess", node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is name,
        ///     and source is static expression yielding multiple
        ///     values with different types
        /// </summary>
        [Test]
        public void Set22 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add (string.Empty, "succ")
                .Add (string.Empty, 5)
                .Add (string.Empty, "ess").Parent
                .Add ("set", "@/-/?name").LastChild
                .Add ("source", "@/./-/*/?value").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("succ5ess", node [0].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is value,
        ///     and source is static expression yielding 'node'
        /// </summary>
        [Test]
        public void Set23 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("succ")
                .Add ("ess").Parent
                .Add ("set", "@/-/?value").LastChild
                .Add ("source", "@/./-/?node").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data", node [0].Get<Node> (Context).Name);
            Assert.AreEqual ("succ", node [0].Get<Node> (Context) [0].Name);
            Assert.AreEqual ("ess", node [0].Get<Node> (Context) [1].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is name,
        ///     and source is static node
        /// </summary>
        [Test]
        public void Set24 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("succ")
                .Add ("ess").Parent
                .Add ("set", "@/-/?name").LastChild
                .Add ("source", "@/./-/?node").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_data\r\n  succ\r\n  ess", node [0].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is value,
        ///     and source is static expression yielding no results
        /// </summary>
        [Test]
        public void Set25 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                .Add ("source", "@/mumbo/?value").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.IsNull (node [0].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is name,
        ///     and source is relative expression
        /// </summary>
        [Test]
        public void Set26 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("_1", "success1")
                .Add ("_2", "success2").Parent
                .Add ("set", "@/-/*/?name").LastChild
                .Add ("rel-source", "@?value").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success1", node [0] [0].Name);
            Assert.AreEqual ("success2", node [0] [1].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is value,
        ///     and source is relative expression
        /// </summary>
        [Test]
        public void Set27 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("success1")
                .Add ("success2").Parent
                .Add ("set", "@/-/*/?value").LastChild
                .Add ("rel-source", "@?name").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is node,
        ///     and source is relative expression, where source is child of destination
        /// </summary>
        [Test]
        public void Set28 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add (string.Empty).LastChild
                .Add ("_1", "success1").Parent
                .Add (string.Empty).LastChild
                .Add ("_2", "success2").Parent.Parent
                .Add ("set", "@/-/*/?node").LastChild
                .Add ("rel-source", "@/0/?node").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (0, node [0] [0].Count);
            Assert.AreEqual ("_1", node [0] [0].Name);
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual (0, node [0] [1].Count);
            Assert.AreEqual ("_2", node [0] [1].Name);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is node,
        ///     and source is relative formatted expression
        /// </summary>
        [Test]
        public void Set29 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("_1").LastChild
                .Add ("_1", "success1").Parent
                .Add ("_2").LastChild
                .Add ("_2", "success2").Parent.Parent
                .Add ("set", "@/-/*/?node").LastChild
                .Add ("rel-source", "@/{0}/{1}/?node").LastChild
                .Add (string.Empty, "*")
                .Add (string.Empty, "@?name").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is formatted node expression,
        ///     and source is relative source, which is a formatted expression,
        ///     and one of sources yields no result
        /// </summary>
        [Test]
        public void Set31 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("_1").LastChild
                .Add ("_1", "success1").Parent
                .Add ("_2").LastChild
                .Add ("_2", "success2").Parent
                .Add ("_3").LastChild
                .Add ("_ERROR2").Parent.Parent // intentionally returns "null" to verify [_3] is deleted
                .Add ("set", "@/{0}/*/?node").LastChild
                .Add (string.Empty, "-")
                .Add ("rel-source", "@/{0}/{1}/?node").LastChild
                .Add (string.Empty, "*")
                .Add (string.Empty, "@?{0}").LastChild
                .Add (string.Empty, "name").Root; // recursive formatting expression
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is node,
        ///     and source is relative expression, where source is parent of destination
        /// </summary>
        [Test]
        public void Set32 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("success1").LastChild
                .Add ("success11").Parent
                .Add ("success2").LastChild
                .Add ("success21").Parent.Parent
                .Add ("set", "@/-/*/*/?node").LastChild
                .Add ("rel-source", "@/./?node").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success1", node [0] [0].Name);
            Assert.AreEqual ("success1", node [0] [0] [0].Name);
            Assert.AreEqual ("success11", node [0] [0] [0] [0].Name);
            Assert.AreEqual ("success2", node [0] [1].Name);
            Assert.AreEqual ("success2", node [0] [1] [0].Name);
            Assert.AreEqual ("success21", node [0] [1] [0] [0].Name);
        }

        /// <summary>
        ///     verifies [set] works when destination is node,
        ///     and source is relative expression, leading to a string, which should
        ///     be converted into a node
        /// </summary>
        [Test]
        public void Set33 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("success1").LastChild
                .Add ("_val1:success1").Parent
                .Add ("success2").LastChild
                .Add ("_val2:success2").Parent.Parent
                .Add ("set", "@/-/*/?node").LastChild
                .Add ("rel-source", "@/0/?name").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual (0, node [0] [0].Count);
            Assert.AreEqual ("_val1", node [0] [0].Name);
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual (0, node [0] [1].Count);
            Assert.AreEqual ("_val2", node [0] [1].Name);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }

        /// <summary>
        ///     verifies [set] works when destination is name,
        ///     and source is relative expression, leading to a node, which should
        ///     be converted into a string
        /// </summary>
        [Test]
        public void Set34 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("success1").LastChild
                .Add ("foo1", 5).Parent // making sure types works
                .Add ("success2").LastChild
                .Add ("foo2", new Node ("bar2", "x")).Parent // making sure recursive nodes works
                .Add ("success3").LastChild
                .Add ("foo3", "test1\r\ntest2").Parent.Parent // making sure CR/LF works
                .Add ("set", "@/-/*/?name").LastChild
                .Add ("rel-source", "@?node").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (3, node [0].Count);
            Assert.AreEqual (1, node [0] [0].Count); // making sure source is still around
            Assert.AreEqual ("success1\r\n  foo1:int:5", node [0] [0].Name);
            Assert.AreEqual (1, node [0] [1].Count); // making sure source is still around
            Assert.AreEqual ("success2\r\n  foo2:node:\"bar2:x\"", node [0] [1].Name);
            Assert.AreEqual (1, node [0] [2].Count); // making sure source is still around
            Assert.AreEqual ("success3\r\n  foo3:@\"test1\r\ntest2\"", node [0] [2].Name);
        }

        /// <summary>
        ///     verifies [set] works when there are more than one destination
        /// </summary>
        [Test]
        public void Set35 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("_1")
                .Add ("_2").Parent
                .Add ("set", "@/-/**/?value").LastChild
                .Add ("source", "success").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("success", node [0].Value);
            Assert.AreEqual ("success", node [0] [0].Value);
            Assert.AreEqual ("success", node [0] [1].Value);
        }

        /// <summary>
        ///     verifies [set] works when [rel-source] is a formatting expression
        /// </summary>
        [Test]
        public void Set36 ()
        {
            var node = new Node ()
                .Add ("_data").LastChild
                .Add ("_1")
                .Add ("_2").Parent
                .Add ("set", "@/-/*/?value").LastChild
                .Add ("rel-source", "{0}").LastChild
                .Add ("", "@?name").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_1", node [0] [0].Value);
            Assert.AreEqual ("_2", node [0] [1].Value);
        }

        [Test]
        public void Set37 ()
        {
            var node = ExecuteLambda (@"_result
set:@/-/?node
  source:@""success-name:success-value""");
            Assert.AreEqual (0, node [0].Count);
            Assert.AreEqual ("success-name", node [0].Name);
            Assert.AreEqual ("success-value", node [0].Value);
        }

        /// <summary>
        ///     Verifies that setting a node's value to another node, does not clone the original node, allowing
        ///     for nodes to be passed by reference.
        /// </summary>
        [Test]
        public void Set38 ()
        {
            var node = ExecuteLambda (@"_result
_out
set:@/-2/?value
  source:@/./-?node
set:@/-3/#?value
  source:foo");
            Assert.AreEqual ("foo", node [1].Value);
        }

        /// <summary>
        ///     verifies that using escaped expressions as constant source works
        /// </summary>
        [Test]
        public void Set40 ()
        {
            var node = ExecuteLambda (@"_result
set:@/-/?value
  source:\@?node");
            Assert.AreEqual (0, node [0].Count);
            Assert.AreEqual ("@?node", node [0].Value);
        }

        /// <summary>
        ///     verifies that using [src] instead of [source] works
        /// </summary>
        [Test]
        public void Set41 ()
        {
            var node = ExecuteLambda (@"_result
set:@/-/?value
  src:success");
            Assert.AreEqual (0, node [0].Count);
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        ///     verifies that setting a 'value' to a bunch of static nodes works
        /// </summary>
        [Test]
        public void Set42 ()
        {
            var node = ExecuteLambda (@"_result
set:@/-/?value
  src
    foo1:bar1
    foo2:bar2");
            Assert.AreEqual (0, node [0].Count);
            Assert.AreEqual (2, node [1] [0].Count);
            Assert.AreEqual ("foo1:bar1\r\nfoo2:bar2", node [0].Value);
        }

        /// <summary>
        ///     verifies that setting a 'name' to a bunch of static nodes works
        /// </summary>
        [Test]
        public void Set43 ()
        {
            var node = ExecuteLambda (@"_result
set:@/-/?name
  src
    foo1:bar1
    foo2:bar2");
            Assert.AreEqual (0, node [0].Count);
            Assert.AreEqual (2, node [1] [0].Count);
            Assert.AreEqual ("foo1:bar1\r\nfoo2:bar2", node [0].Name);
        }

        /// <summary>
        ///     verifies that setting a 'value' to an expression returning multiple nodes works
        /// </summary>
        [Test]
        public void Set44 ()
        {
            var node = ExecuteLambda (@"_result
_data
  foo1:bar1
  foo2:bar2
set:@/-2/?value
  src:@/./-/*?node");
            Assert.AreEqual (0, node [0].Count);
            Assert.AreEqual ("foo1:bar1\r\nfoo2:bar2", node [0].Value);
        }

        /// <summary>
        ///     verifies that setting a 'value' to an expression leading to an IEnumerable single value,
        ///     does not tamper with the original object in any ways
        /// </summary>
        [Test]
        public void Set45 ()
        {
            // easy way to create a node
            var node = ExecuteLambda (@"_source
_destination
_set:@/-/?value
  src:@/./-2?value");

            // discarding previous execution, setting a node to string[], for then to re-execute again, after renaming our 
            // [_set] node to actually do something
            node [0].Value = new[] {"howdy", "world"};
            node [2].Name = "set";

            // executing again
            Context.Raise ("lambda", node);
            Assert.AreEqual (2, node [1].Get<string[]> (Context).Length);
            Assert.AreEqual ("howdy", node [1].Get<string[]> (Context) [0]);
            Assert.AreEqual ("world", node [1].Get<string[]> (Context) [1]);
        }
        
        /// <summary>
        ///     Verifies that setting a 'value' to the results of an Active Event works correctly.
        /// </summary>
        [Test]
        public void Set46 ()
        {
            // easy way to create a node
            var node = ExecuteLambda (@"event:test.set46
  lambda
    set:@/./.?value
      source:howdy world
_destination
set:@/-/?value
  test.set46");

            Assert.AreEqual ("howdy world", node [1].Value);
        }
        
        /// <summary>
        ///     Verifies that setting a 'value' to the results of a math hierarchy works correctly.
        /// </summary>
        [Test]
        public void Set47 ()
        {
            // easy way to create a node
            var node = ExecuteLambda (@"_destination
set:@/-/?value
  +:int:2
    _:2");

            Assert.AreEqual (4, node [0].Value);
        }
    }
}
