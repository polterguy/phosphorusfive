/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the [append] lambda keyword
    /// </summary>
    [TestFixture]
    public class Append : TestBase
    {
        public Append ()
            : base ("phosphorus.lambda", "phosphorus.types", "phosphorus.hyperlisp") { }

        /// <summary>
        ///     appends a static constant source node to destination
        /// </summary>
        [Test]
        public void Append01 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("append", "@/-/?node").LastChild
                .Add ("source").LastChild
                .Add ("foo1", "success1")
                .Add ("foo2", "success2").Root;
            Context.Raise ("append", node [1]);

            // verifying [append] works as it should
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
        public void Append02 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("source").LastChild
                .Add ("foo", "success").Parent
                .Add ("append", "@/-2/?node").LastChild
                .Add ("source", "@/./-/?node").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
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
        public void Append03 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("source").LastChild
                .Add ("foo", "success").Parent
                .Add ("append", "@/{0}/?node").LastChild
                .Add (string.Empty, "-2")
                .Add ("source", "@/./{0}/?node").LastChild
                .Add (string.Empty, "-").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
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
        public void Append04 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("append", "@/-/?node").LastChild
                .Add ("source", "foo1:success\r\n  bar1:int:5").Root;
            Context.Raise ("append", node [1]);

            // verifying [append] works as it should
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
        public void Append05 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("_source", "success-name:success-value")
                .Add ("append", "@/-2/?node").LastChild
                .Add ("source", "@/../*/_source/?value").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("success-name", node [0] [0].Name);
            Assert.AreEqual ("success-value", node [0] [0].Value);
        }

        /// <summary>
        ///     appends an expression result, being 'value' expression, where value is a node
        /// </summary>
        [Test]
        public void Append06 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("_source", new Node ("success", 5))
                .Add ("append", "@/-2/?node").LastChild
                .Add ("source", "@/../*/_source/?value").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("success", node [0] [0].Name);
            Assert.AreEqual (5, node [0] [0].Value);
        }

        /// <summary>
        ///     appends a static source, where source is a node itself
        /// </summary>
        [Test]
        public void Append07 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("append", "@/-/?node").LastChild
                .Add ("source", new Node ("success", 5)).Root;
            Context.Raise ("append", node [1]);

            // verifying [append] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("success", node [0] [0].Name);
            Assert.AreEqual (5, node [0] [0].Value);
        }

        /// <summary>
        ///     appends an expression source, where destination is a child of source
        /// </summary>
        [Test]
        public void Append08 ()
        {
            var node = new Node ()
                .Add ("_source").LastChild
                .Add ("_destination").Parent
                .Add ("append", "@/-/0/?node").LastChild
                .Add ("source", "@/./-/?node").Root;
            Context.Raise ("append", node [1]);

            // verifying [append] works as it should
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
        public void Append09 ()
        {
            var node = new Node ()
                .Add ("_destination").LastChild
                .Add ("_source").Parent
                .Add ("append", "@/-/?node").LastChild
                .Add ("source", "@/./-/0/?node").Root;
            Context.Raise ("append", node [1]);

            // verifying [append] works as it should
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
        public void Append10 ()
        {
            var node = new Node ()
                .Add ("_destination1").LastChild
                .Add ("_source1").Parent
                .Add ("_destination2").LastChild
                .Add ("_source2").Parent
                .Add ("append", "@/-1/|/-2/?node").LastChild
                .Add ("rel-source", "@/0/?node").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
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
        public void Append11 ()
        {
            var node = new Node ()
                .Add ("_source1").LastChild
                .Add ("_destination1").Parent
                .Add ("_source2").LastChild
                .Add ("_destination2").Parent
                .Add ("append", "@/-1/*/|/-2/*/?node").LastChild
                .Add ("rel-source", "@/./?node").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
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
        public void Append12 ()
        {
            var node = new Node ()
                .Add ("_source1")
                .Add ("_source2")
                .Add ("append", "@/-1/|/-2/?node").LastChild
                .Add ("rel-source", "@?node").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
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
        public void Append13 ()
        {
            var node = new Node ()
                .Add ("_source", new Node ("success", 5))
                .Add ("_destination")
                .Add ("append", "@/-/?node").LastChild
                .Add ("source", "@/./-2/?value").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual ("success", node [1] [0].Name);
            Assert.AreEqual (5, node [1] [0].Value);
        }

        /// <summary>
        ///     appends a a static source, where destination is expression of type 'value',
        ///     where value is a reference node
        /// </summary>
        [Test]
        public void Append14 ()
        {
            var node = new Node ()
                .Add ("_destination-parent", new Node ("_destination"))
                .Add ("success", 5)
                .Add ("append", "@/-2/?value").LastChild
                .Add ("source", "@/./-/?node").Root;
            Context.Raise ("append", node [2]);

            // verifying [append] works as it should
            Assert.AreEqual (1, node [0].Get<Node> (Context).Count);
            Assert.AreEqual ("success", node [0].Get<Node> (Context) [0].Name);
            Assert.AreEqual (5, node [0].Get<Node> (Context) [0].Value);
        }

        /// <summary>
        ///     appends a a static source, where source has no result
        /// </summary>
        [Test]
        public void Append15 ()
        {
            var node = new Node ()
                .Add ("_destination")
                .Add ("append", "@/-/?node").LastChild
                .Add ("source", "@/mumbo/?node").Root;
            Context.Raise ("append", node [1]);

            // verifying [append] works as it should
            Assert.AreEqual (0, node [0].Count);
        }

        /// <summary>
        ///     appends a a static source, where source has no values
        ///     where value is a reference node
        /// </summary>
        [Test]
        public void Append16 ()
        {
            var node = ExecuteLambda (@"_data
  foo:bar
_exp:@/-/?node
append:@/+/?node
  source:@@/./-/?value
_out");
            // verifying [append] works as it should
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
        public void Append17 ()
        {
            var node = ExecuteLambda (@"append:@/+/?node
  source:int:500
_out");
            // verifying [append] works as it should
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual (string.Empty, node [1] [0].Name);
            Assert.AreEqual (500, node [1] [0].Value);
        }

        /// <summary>
        ///     appends a static string source, which is converted into node,
        ///     where conversion yields multiple result nodes
        /// </summary>
        [Test]
        public void Append18 ()
        {
            var node = new Node ()
                .Add ("_data")
                .Add ("append", "@/-/?node").LastChild
                .Add ("source", "foo1:success\r\nbar1:int:5").Root;
            Context.Raise ("append", node [1]);

            // verifying [append] works as it should
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
        public void Append19 ()
        {
            var node = ExecuteLambda (@"append:@/+/?node
  src:success
_out");
            // verifying [append] works as it should
            Assert.AreEqual (1, node [1].Count);
            Assert.AreEqual ("success", node [1] [0].Name);
        }

        /// <summary>
        ///     tries to append into 'value' destination, where value is not a Node
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError1 ()
        {
            var node = new Node ()
                .Add ("_destination", "foo")
                .Add ("error")
                .Add ("append", "@/-2/?value").LastChild
                .Add ("source", "@/./-/?node").Root;
            Context.Raise ("append", node [2]);
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
                .Add ("append", "@/-2/?value").LastChild
                .Add ("rel-source", "@/../*/error/?node").Root;
            Context.Raise ("append", node [2]);
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
                .Add ("append", "@/-2/?value").LastChild
                .Add ("rel-source", "@/../*/error/?node").Root;
            Context.Raise ("append", node [2]);
        }
    }
}