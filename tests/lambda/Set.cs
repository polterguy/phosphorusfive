/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using p5.core;
using p5.exp;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [set] lambda keyword
    /// </summary>
    [TestFixture]
    public class Set : TestBase
    {
        public Set ()
            : base ("p5.lambda", "p5.hyperlisp", "p5.types", "p5.math") { }

        /// <summary>
        ///     verifies [set] works with a static constant source
        /// </summary>
        [Test]
        public void Set01 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("set", Expression.Create ("/-?value", Context)).LastChild
                    .Add ("src", "success").Root;
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
                .Add ("set", Expression.Create ("/-2?value", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-?name", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?{0}", Context)).LastChild
                    .Add (string.Empty, "value")
                    .Add ("src", "success").Root;
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
                .Add ("set", Expression.Create ("/-?value", Context)).LastChild
                    .Add ("src", "{0}{1}{2}").LastChild
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
                .Add ("set", Expression.Create ("/-2?value", Context)).LastChild
                    .Add ("src", Expression.Create ("{0}?name", Context)).LastChild
                        .Add (string.Empty, "/./-").Root;
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
                .Add ("set", Expression.Create ("/-?name", Context)).LastChild
                    .Add ("src", "success").Root;
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
                .Add ("set", Expression.Create ("/-2", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-", Context)).Root;
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
                .Add ("set", Expression.Create ("/-", Context)).LastChild
                    .Add ("src").LastChild
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
                .Add ("set", Expression.Create ("/-", Context)).LastChild
                    .Add ("src", new Node ("_data2", "success")).Root;
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
                .Add ("set", Expression.Create ("/-?value", Context)).LastChild
                    .Add ("src", 5).Root;
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
                .Add ("set", Expression.Create ("/-?name", Context)).LastChild
                    .Add ("src", 5).Root;
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
                .Add ("set", Expression.Create ("/-?value", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?name", Context)).Root;
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
                .Add ("set", Expression.Create ("/-", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?value", Context)).LastChild
                    .Add ("src", Expression.Create ("/../**?count", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?name", Context)).LastChild
                    .Add ("src", Expression.Create ("/../**?count", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?value", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-/*?name", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?value", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-/*?value", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?name", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-/*?value", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?value", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-?node", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?name", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-", Context)).Root;
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
                .Add ("set", Expression.Create ("/-?value", Context)).LastChild
                    .Add ("src", Expression.Create ("/mumbo?value", Context)).Root;
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
                .Add ("set", Expression.Create ("/-/*?name", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("?value", Context)).Root;
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
                .Add ("set", Expression.Create ("/-/*?value", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("?name", Context)).Root;
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
                .Add ("set", Expression.Create ("/-/*", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("/0", Context)).Root;
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
                .Add ("set", Expression.Create ("/-/*", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("/{0}/{1}", Context)).LastChild
                        .Add (string.Empty, "*")
                        .Add (string.Empty, Expression.Create ("?name", Context)).Root;
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
                .Add ("set", Expression.Create ("/{0}/*", Context)).LastChild
                    .Add (string.Empty, "-")
                    .Add ("rel-src", Expression.Create ("/{0}{1}", Context)).LastChild
                        .Add (string.Empty, "*")
                        .Add (string.Empty, Expression.Create ("?{0}", Context)).LastChild
                            .Add (string.Empty, "value").Root; // recursive formatting expression
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("success1", node [0] [0].Name);
            Assert.IsNull (node [0] [0].Value);
            Assert.AreEqual (0, node [0] [0].Count);
            Assert.AreEqual ("success2", node [0] [1].Name);
            Assert.IsNull (node [0] [1].Value);
            Assert.AreEqual (0, node [0] [1].Count);
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
                .Add ("set", Expression.Create ("/-/*/*", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("/.", Context)).Root;
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
                .Add ("set", Expression.Create ("/-/*", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("/0?name", Context)).Root;
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
                .Add ("set", Expression.Create ("/-/*?name", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("", Context)).Root;
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
                .Add ("set", Expression.Create ("/-/**?value", Context)).LastChild
                    .Add ("src", "success").Root;
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
                .Add ("set", Expression.Create ("/-/*?value", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("{0}", Context)).LastChild
                        .Add ("", "?name").Root;
            Context.Raise ("set", node [1]);

            // verifying [set] works as it should
            Assert.AreEqual ("_1", node [0] [0].Value);
            Assert.AreEqual ("_2", node [0] [1].Value);
        }

        [Test]
        public void Set37 ()
        {
            var node = ExecuteLambda (@"_result
set:x:/-
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
set:x:/-2?value
  src:x:/./-
set:x:/-3/#?value
  src:foo");
            Assert.AreEqual ("foo", node [1].Value);
        }

        /// <summary>
        ///     verifies that using [src] instead of [source] works
        /// </summary>
        [Test]
        public void Set41 ()
        {
            var node = ExecuteLambda (@"_result
set:x:/-?value
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
set:x:/-?value
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
set:x:/-?name
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
set:x:/-2?value
  src:x:/./-/*");
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
            var node = CreateNode (@"_source
_destination
set:x:/-?value
  src:x:/./-2?value");

            // discarding previous execution, setting a node to string[], for then to re-execute again, after renaming our 
            // [_set] node to actually do something
            node [0].Value = new[] {"howdy", "world"};

            // executing again
            Context.Raise ("lambda", node);
            Assert.AreEqual (2, node [1].Get<string[]> (Context).Length);
            Assert.AreEqual ("howdy", node [1].Get<string[]> (Context) [0]);
            Assert.AreEqual ("world", node [1].Get<string[]> (Context) [1]);
        }

        [ActiveEvent (Name = "test.set46")]
        private static void test_set46 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "howdy world";
        }
        
        /// <summary>
        ///     Verifies that setting a 'value' to the results of an Active Event works correctly.
        /// </summary>
        [Test]
        public void Set46 ()
        {
            // easy way to create a node
            var node = ExecuteLambda (@"_destination
set:x:/-?value
  test.set46");

            Assert.AreEqual ("howdy world", node [0].Value);
        }
    }
}
