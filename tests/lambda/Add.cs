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
    ///     unit tests for testing the [add] lambda keyword
    /// </summary>
    [TestFixture]
    public class Add : TestBase
    {
        public Add ()
            : base ("p5.lambda", "p5.types", "p5.hyperlisp")
        { }

        /// <summary>
        ///     appends a static constant source node to destination
        /// </summary>
        [Test]
        public void Add01 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("add", Expression.Create ("/-?node", Context)).LastChild
                    .Add ("src").LastChild
                        .Add ("foo1", "success1")
                        .Add ("foo2", "success2").Root;
            Context.Raise ("add", node [1]);

            // verifying [add] works as it should
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("success1", node [0] [0].Value);
            Assert.AreEqual ("foo2", node [0] [1].Name);
            Assert.AreEqual ("success2", node [0] [1].Value);
        }

        /// <summary>
        ///     appends a static expression source node to destination
        /// </summary>
        [Test]
        public void Add02 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("source").LastChild
                    .Add ("foo", "success").Parent
                .Add ("add", Expression.Create ("/-2?node", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-?node", Context)).Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("source", node [0] [0].Name);
            Assert.IsNull (node [0] [0].Value);
            Assert.AreEqual ("foo", node [0] [0] [0].Name);
            Assert.AreEqual ("success", node [0] [0] [0].Value);
        }

        /// <summary>
        ///     appends a static expression source node to destination where both destination and source
        ///     expressions have formatting values
        /// </summary>
        [Test]
        public void Add03 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("source").LastChild
                    .Add ("foo", "success").Parent
                .Add ("add", Expression.Create ("/{0}?node", Context)).LastChild
                    .Add (string.Empty, "-2")
                    .Add ("src", Expression.Create ("/./{0}?node", Context)).LastChild
                        .Add (string.Empty, "-").Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("source", node [0] [0].Name);
            Assert.IsNull (node [0] [0].Value);
            Assert.AreEqual ("foo", node [0] [0] [0].Name);
            Assert.AreEqual ("success", node [0] [0] [0].Value);
        }

        /// <summary>
        ///     appends a static string source, which is converted into node
        /// </summary>
        [Test]
        public void Add04 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("add", Expression.Create ("/-?node", Context)).LastChild
                    .Add ("src", "foo1:success\r\n  bar1:int:5").Root;
            Context.Raise ("add", node [1]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("success", node [0] [0].Value);
            Assert.AreEqual ("bar1", node [0] [0] [0].Name);
            Assert.AreEqual (5, node [0] [0] [0].Value);
        }

        /// <summary>
        ///     appends an expression result, yielding string, which is converted into node
        /// </summary>
        [Test]
        public void Add05 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("_source", "success-name:success-value")
                .Add ("add", Expression.Create ("/-2?node", Context)).LastChild
                    .Add ("src", Expression.Create ("/../*/_source?value", Context)).Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("success-name", node [0] [0].Name);
            Assert.AreEqual ("success-value", node [0] [0].Value);
        }

        /// <summary>
        ///     appends an expression result, being 'value' expression, where value is a node
        /// </summary>
        [Test]
        public void Add06 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("_source", new Node ("success", 5))
                .Add ("add", Expression.Create ("/-2?node", Context)).LastChild
                    .Add ("src", Expression.Create ("/../*/_source?value", Context)).Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("success", node [0] [0].Name);
            Assert.AreEqual (5, node [0] [0].Value);
        }

        /// <summary>
        ///     appends a static source, where source is a node itself
        /// </summary>
        [Test]
        public void Add07 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("add", Expression.Create ("/-?node", Context)).LastChild
                    .Add ("src", new Node ("success", 5)).Root;
            Context.Raise ("add", node [1]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("success", node [0] [0].Name);
            Assert.AreEqual (5, node [0] [0].Value);
        }

        /// <summary>
        ///     appends an expression source, where destination is a child of source
        /// </summary>
        [Test]
        public void Add08 ()
        {
            var node = new Node ()
                .Add ("_source").LastChild
                    .Add ("_destination").Parent
                .Add ("add", Expression.Create ("/-/0?node", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-?node", Context)).Root;
            Context.Raise ("add", node [1]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual (1, node [0] [0].Count);
            Assert.AreEqual ("_source", node [0] [0] [0].Name);
            Assert.AreEqual (1, node [0] [0] [0].Count);
            Assert.AreEqual ("_destination", node [0] [0] [0] [0].Name);
        }

        /// <summary>
        ///     appends an expression source, where source is a child of destination
        /// </summary>
        [Test]
        public void Add09 ()
        {
            var node = new Node ()
                .Add ("_destination").LastChild
                    .Add ("_source").Parent
                .Add ("add", Expression.Create ("/-?node", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-/0?node", Context)).Root;
            Context.Raise ("add", node [1]);

            // verifying [add] works as it should
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual (0, node [0] [0].Count);
            Assert.AreEqual ("_source", node [0] [0].Name);
            Assert.AreEqual (0, node [0] [1].Count);
            Assert.AreEqual ("_source", node [0] [1].Name);
        }

        /// <summary>
        ///     appends a relative source, where source is child of destination
        /// </summary>
        [Test]
        public void Add10 ()
        {
            var node = new Node ()
                .Add ("_destination1").LastChild
                    .Add ("_source1").Parent
                .Add ("_destination2").LastChild
                    .Add ("_source2").Parent
                .Add ("add", Expression.Create ("/-1|/-2?node", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("/0?node", Context)).Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("_source1", node [0] [0].Name);
            Assert.AreEqual ("_source1", node [0] [1].Name);
            Assert.AreEqual (2, node [1].Count);
            Assert.AreEqual ("_source2", node [1] [0].Name);
            Assert.AreEqual ("_source2", node [1] [1].Name);
        }

        /// <summary>
        ///     appends a relative source, where source is parent of destination
        /// </summary>
        [Test]
        public void Add11 ()
        {
            var node = new Node ()
                .Add ("_source1").LastChild
                    .Add ("_destination1").Parent
                .Add ("_source2").LastChild
                    .Add ("_destination2").Parent
                .Add ("add", Expression.Create ("/-1/*|/-2/*?node", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("/.?node", Context)).Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("_destination1", node [0] [0].Name);
            Assert.AreEqual (1, node [0] [0].Count);
            Assert.AreEqual ("_source1", node [0] [0] [0].Name);
            Assert.AreEqual (1, node [0] [0] [0].Count);
            Assert.AreEqual ("_destination1", node [0] [0] [0] [0].Name);
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual ("_destination2", node [1] [0].Name);
            Assert.AreEqual (1, node [1] [0].Count);
            Assert.AreEqual ("_source2", node [1] [0] [0].Name);
            Assert.AreEqual (1, node [1] [0] [0].Count);
            Assert.AreEqual ("_destination2", node [1] [0] [0] [0].Name);
        }

        /// <summary>
        ///     appends a relative source, where destination equals source
        /// </summary>
        [Test]
        public void Add12 ()
        {
            var node = new Node ()
                .Add ("_source1")
                .Add ("_source2")
                .Add ("add", Expression.Create ("/-1|/-2?node", Context)).LastChild
                    .Add ("rel-src", Expression.Create ("?node", Context)).Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual (0, node [0] [0].Count);
            Assert.AreEqual ("_source1", node [0] [0].Name);
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual (0, node [1] [0].Count);
            Assert.AreEqual ("_source2", node [1] [0].Name);
        }

        /// <summary>
        ///     appends a a static source, where source is expression of type 'value',
        ///     where value is a reference node
        /// </summary>
        [Test]
        public void Add13 ()
        {
            var node = new Node ()
                .Add ("_source", new Node ("success", 5))
                .Add ("_destination")
                .Add ("add", Expression.Create ("/-?node", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-2?value", Context)).Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual ("success", node [1] [0].Name);
            Assert.AreEqual (5, node [1] [0].Value);
        }

        /// <summary>
        ///     appends a a static source, where destination is expression of type 'value',
        ///     where value is a reference node
        /// </summary>
        [Test]
        public void Add14 ()
        {
            var node = new Node ()
                .Add ("_destination-parent", new Node ("_destination"))
                .Add ("success", 5)
                .Add ("add", Expression.Create ("/-2?value", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-?node", Context)).Root;
            Context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Get<Node> (Context).Count);
            Assert.AreEqual ("success", node [0].Get<Node> (Context) [0].Name);
            Assert.AreEqual (5, node [0].Get<Node> (Context) [0].Value);
        }

        /// <summary>
        ///     appends a a static source, where source has no result
        /// </summary>
        [Test]
        public void Add15 ()
        {
            var node = new Node ()
                .Add ("_destination")
                .Add ("add", Expression.Create ("/-?node", Context)).LastChild
                    .Add ("src", Expression.Create ("/mumbo?node", Context)).Root;
            Context.Raise ("add", node [1]);

            // verifying [add] works as it should
            Assert.AreEqual (0, node [0].Count);
        }

        /// <summary>
        ///     appends a a static source, where source has no values
        ///     and value is a reference node
        /// </summary>
        [Test]
        public void Add16 ()
        {
            var node = ExecuteLambda (@"_data
  foo:bar
_exp:x:/-?node
add:x:/+?node
  src:x:@/./-?value
_out");
            // verifying [add] works as it should
            Assert.AreEqual (1, node [3].Count);
            Assert.AreEqual ("_data", node [3] [0].Name);
            Assert.AreEqual ("foo", node [3] [0] [0].Name);
            Assert.AreEqual ("bar", node [3] [0] [0].Value);
        }

        /// <summary>
        ///     appends an integer value, making sure [append] works as it should
        ///     where value is a reference node
        /// </summary>
        [Test]
        public void Add17 ()
        {
            var node = ExecuteLambda (@"add:x:/+?node
  src:int:500
_out");
            // verifying [add] works as it should
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual (string.Empty, node [1] [0].Name);
            Assert.AreEqual (500, node [1] [0].Value);
        }

        /// <summary>
        ///     appends a static string source, which is converted into node,
        ///     where conversion yields multiple result nodes
        /// </summary>
        [Test]
        public void Add18 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("add", Expression.Create ("/-?node", Context)).LastChild
                    .Add ("src", "foo1:success\r\nbar1:int:5").Root;
            Context.Raise ("add", node [1]);

            // verifying [add] works as it should
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("success", node [0] [0].Value);
            Assert.AreEqual ("bar1", node [0] [1].Name);
            Assert.AreEqual (5, node [0] [1].Value);
        }

        /// <summary>
        ///     appends a string value, using [src] instead of [source], making
        ///     sure [append] works as it should
        /// </summary>
        [Test]
        public void Add19 ()
        {
            var node = ExecuteLambda (@"add:x:/+?node
  src:success
_out");
            // verifying [add] works as it should
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual ("success", node [1] [0].Name);
        }
        
        [ActiveEvent (Name = "test.add.20")]
        private static void test_add_20 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("foo", "bar");
        }

        /// <summary>
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void Add20 ()
        {
            var node = ExecuteLambda (@"_result
add:x:/-
  test.add.20");

            Assert.AreEqual ("foo", node [0] [0].Name);
            Assert.AreEqual ("bar", node [0] [0].Value);
        }
        
        [ActiveEvent (Name = "test.add.21")]
        private static void test_add_21 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "foo:bar";
        }

        /// <summary>
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void Add21 ()
        {
            var node = ExecuteLambda (@"_result
add:x:/-
  test.add.21");

            Assert.AreEqual ("foo", node [0] [0].Name);
            Assert.AreEqual ("bar", node [0] [0].Value);
        }
        
        [ActiveEvent (Name = "test.add.22")]
        private static void test_add_22 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = new Node ("foo", "bar");
        }

        /// <summary>
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void Add22 ()
        {
            var node = ExecuteLambda (@"_result
add:x:/-
  test.add.22");

            Assert.AreEqual ("foo", node [0] [0].Name);
            Assert.AreEqual ("bar", node [0] [0].Value);
        }
        
        /// <summary>
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void Add23 ()
        {
            var node = ExecuteLambda (@"_result
add:x:/-
  test.add.22:error"); // making sure "children" has presedence unless "value" is changed!

            Assert.AreEqual ("foo", node [0] [0].Name);
            Assert.AreEqual ("bar", node [0] [0].Value);
        }

        /// <summary>
        ///     tries to add into 'value' destination, where value is not a Node
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            var node = new Node ()
                .Add ("_destination", "foo")
                .Add ("error")
                .Add ("add", Expression.Create ("/-2?value", Context)).LastChild
                    .Add ("src", Expression.Create ("/./-?node", Context)).Root;
            Context.Raise ("add", node [2]);
        }

        /// <summary>
        ///     tries to append into 'value' destination, where value is not a Node with a relative source
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError2 ()
        {
            var node = new Node ()
                .Add ("_destination", "foo")
                .Add ("error")
                .Add ("add", Expression.Create ("/-2?value", Context)).LastChild
                    .Add ("rel-src", "@/./-?node").Root;
            Context.Raise ("add", node [2]);
        }

        /// <summary>
        ///     tries to append into 'value' destination, where value is null, with a relative source
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError3 ()
        {
            var node = new Node ()
                .Add ("_destination")
                .Add ("error")
                .Add ("add", Expression.Create ("/-2?value", Context)).LastChild
                    .Add ("rel-src", "@/./-?node").Root;
            Context.Raise ("add", node [2]);
        }
        
        /// <summary>
        ///     tries to use something that's not an expression in destination
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError4 ()
        {
            var node = new Node ()
                .Add ("_destination")
                .Add ("error")
                .Add ("add", "@/-2/?node").LastChild
                    .Add ("src", "@/./-?node").Root;
            Context.Raise ("add", node [2]);
        }
    }
}
