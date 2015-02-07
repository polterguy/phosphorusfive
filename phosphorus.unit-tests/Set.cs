
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
        public void Set1 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "success").Root;
            _context.Raise ("set", node [1]);
            Assert.AreEqual ("success", node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works with a static expression source
        /// </summary>
        [Test]
        public void Set2 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add("success")
                .Add ("set", "@/-2/?value").LastChild
                    .Add ("source", "@/./-/?name").Root;
            _context.Raise ("set", node [2]);
            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        /// verifies [set] works with a formatted destination and a static constant source
        /// </summary>
        [Test]
        public void Set3 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?{0}").LastChild
                    .Add(string.Empty, "value")
                    .Add ("source", "success").Root;
            _context.Raise ("set", node [1]);
            Assert.AreEqual ("success", node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works with a formatted static constant source
        /// </summary>
        [Test]
        public void Set4 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add ("set", "@/-/?value").LastChild
                    .Add ("source", "{0}{1}{2}").LastChild
                        .Add (string.Empty, "su")
                        .Add (string.Empty, "cc")
                        .Add (string.Empty, "ess").Root;
            _context.Raise ("set", node [1]);
            Assert.AreEqual ("success", node [0].Value);
        }
        
        /// <summary>
        /// verifies [set] works with a formatted static expression source
        /// </summary>
        [Test]
        public void Set5 ()
        {
            Node node = new Node ()
                .Add ("_data")
                .Add("success")
                .Add ("set", "@/-2/?value").LastChild
                    .Add ("source", "@{0}?name").LastChild
                        .Add (string.Empty, "/./-/").Root;
            _context.Raise ("set", node [2]);
            Assert.AreEqual ("success", node [0].Value);
        }
    }
}
