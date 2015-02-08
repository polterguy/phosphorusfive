
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
    /// unit tests for testing the [add] lambda keyword
    /// </summary>
    [TestFixture]
    public class Add : TestBase
    {
        public Add ()
            : base ("phosphorus.lambda", "phosphorus.types", "phosphorus.hyperlisp")
        { }

        /// <summary>
        /// adds a static constant source node to destination
        /// </summary>
        [Test]
        public void Add01 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("add", "@/-/?node").LastChild
                    .Add ("source").LastChild
                        .Add ("foo", "success").Root;
            _context.Raise ("add", node [1]);
            
            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("foo", node [0] [0].Name);
            Assert.AreEqual ("success", node [0] [0].Value);
        }
        
        /// <summary>
        /// adds a static expression source node to destination
        /// </summary>
        [Test]
        public void Add02 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("source").LastChild
                    .Add("foo", "success").Parent
                .Add ("add", "@/-2/?node").LastChild
                    .Add ("source", "@/./-/?node").Root;
            _context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("source", node [0] [0].Name);
            Assert.IsNull (node [0] [0].Value);
            Assert.AreEqual ("foo", node [0] [0] [0].Name);
            Assert.AreEqual ("success", node [0] [0] [0].Value);
        }
        
        /// <summary>
        /// adds a static expression source node to destination where both destination and source
        /// expressions have formatting values
        /// </summary>
        [Test]
        public void Add03 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("source").LastChild
                    .Add("foo", "success").Parent
                .Add ("add", "@/{0}/?node").LastChild
                    .Add (string.Empty, "-2")
                    .Add ("source", "@/./{0}/?node").LastChild
                        .Add(string.Empty, "-").Root;
            _context.Raise ("add", node [2]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("source", node [0] [0].Name);
            Assert.IsNull (node [0] [0].Value);
            Assert.AreEqual ("foo", node [0] [0] [0].Name);
            Assert.AreEqual ("success", node [0] [0] [0].Value);
        }
        
        /// <summary>
        /// adds a static string source, which is converted into node
        /// </summary>
        [Test]
        public void Add04 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("add", "@/-/?node").LastChild
                    .Add ("source", "foo1:success\r\n  bar1:int:5").Root;
            _context.Raise ("add", node [1]);

            // verifying [add] works as it should
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("success", node [0] [0].Value);
            Assert.AreEqual ("bar1", node [0] [0] [0].Name);
            Assert.AreEqual (5, node [0] [0] [0].Value);
        }
        // rel-source for [add]
        // [add] where source is expression pointing to 'name' and/or 'value', and source becomes converted into Node
        // [add] when source and destination overlaps
        // [add] where source is 'value', where value of node is a reference node itself, both as an expression, and as constant
    }
}
