
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
    /// unit tests for testing the [pf.file.xxx] namespace
    /// </summary>
    [TestFixture]
    public class Files : TestBase
    {
        public Files ()
            : base ("phosphorus.file")
        { }

        /// <summary>
        /// verifies [pf.file.remove] works correctly
        /// </summary>
        [Test]
        public void Remove ()
        {
            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "this is a test");
            _context.Raise ("pf.file.save", node);

            // removing file using phosphorus.five
            node = new Node (string.Empty, "test1.txt");
            _context.Raise ("pf.file.remove", node);

            // verifying removal of file was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"), "file existed");
        }
        
        /// <summary>
        /// verifies [pf.file.remove] works correctly when given an expression
        /// </summary>
        [Test]
        public void RemoveExpression1 ()
        {
            // creating files using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "this is a test");
            _context.Raise ("pf.file.save", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("source", "this is a test");
            _context.Raise ("pf.file.save", node);

            // removing files using phosphorus.five
            node = new Node (string.Empty, "@/0/|/1/?name")
                .Add ("test1.txt")
                .Add ("test2.txt");
            _context.Raise ("pf.file.remove", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"), "file existed");
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test2.txt"), "file existed");
        }

        /// <summary>
        /// verifies [pf.file.remove] works correctly when given an expression
        /// </summary>
        [Test]
        public void RemoveExpression2 ()
        {
            // creating files using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "this is a test");
            _context.Raise ("pf.file.save", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("source", "this is a test");
            _context.Raise ("pf.file.save", node);

            // removing files using phosphorus.five
            node = new Node (string.Empty, "@/1/|{0}?name")
                .Add ("", "/2/")
                .Add ("test1.txt")
                .Add ("test2.txt");
            _context.Raise ("pf.file.remove", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"), "file existed");
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test2.txt"), "file existed");
        }

        /// <summary>
        /// verifies [pf.file.save] works correctly
        /// </summary>
        [Test]
        public void Save ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "this is a test");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        /// verifies [pf.file.save] works correctly when given an expression
        /// </summary>
        [Test]
        public void SaveExpression1 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }
            if (File.Exists (GetBasePath () + "test2.txt")) {
                File.Delete (GetBasePath () + "test2.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "@/0/|/1/?name")
                .Add ("test1.txt")
                .Add ("test2.txt")
                .Add ("source", "this is a test");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test2.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test2.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }
        
        /// <summary>
        /// verifies [pf.file.save] works correctly when given an expression
        /// </summary>
        [Test]
        public void SaveExpression2 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("hello world")
                .Add ("source", "@/-/?name");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("hello world", reader.ReadToEnd ());
            }
        }
        
        /// <summary>
        /// verifies [pf.file.save] works correctly when given an expression
        /// </summary>
        [Test]
        public void SaveExpression3 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("hello")
                .Add (" world 2.0")
                .Add ("source", "@/-2/|/-1/?name");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("hello world 2.0", reader.ReadToEnd ());
            }
        }
        
        /// <summary>
        /// verifies [pf.file.save] works correctly when given an expression
        /// </summary>
        [Test]
        public void SaveExpression4 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "te{0}1.txt")
                .Add ("", "st")
                .Add ("hello world 2.0")
                .Add ("source", "@/-/?name");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("hello world 2.0", reader.ReadToEnd ());
            }
        }
        
        /// <summary>
        /// verifies [pf.file.save] works correctly when given an expression
        /// </summary>
        [Test]
        public void SaveExpression5 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("success")
                .Add ("source", "@/{0}/?name").LastChild
                    .Add ("", "-").Root;
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }
        
        /// <summary>
        /// verifies [pf.file.save] works correctly when given a relative source
        /// </summary>
        [Test]
        public void SaveExpression6 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }
            if (File.Exists (GetBasePath () + "test2.txt")) {
                File.Delete (GetBasePath () + "test2.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "@/0/|/1/?name")
                .Add ("test1.txt").LastChild
                    .Add ("success1").Parent
                .Add ("test2.txt").LastChild
                    .Add ("success2").Parent
                .Add ("rel-source", "@/0/?name");
            _context.Raise ("pf.file.save", node);

            // verifying creation of files was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success1", reader.ReadToEnd ());
            }
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test2.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test2.txt")) {
                Assert.AreEqual ("success2", reader.ReadToEnd ());
            }
        }
        
        /// <summary>
        /// verifies [pf.file.save] works correctly when given a relative source
        /// </summary>
        [Test]
        public void SaveExpression7 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }
            if (File.Exists (GetBasePath () + "test2.txt")) {
                File.Delete (GetBasePath () + "test2.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "@/0/|/1/?name")
                .Add ("test1.txt").LastChild
                    .Add ("success1").Parent
                .Add ("test2.txt").LastChild
                    .Add ("success2").Parent
                .Add ("rel-source", "@/0/?{0}").LastChild
                    .Add ("", "name").Parent;
            _context.Raise ("pf.file.save", node);

            // verifying creation of files was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success1", reader.ReadToEnd ());
            }
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test2.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test2.txt")) {
                Assert.AreEqual ("success2", reader.ReadToEnd ());
            }
        }
        
        /// <summary>
        /// verifies [pf.file.save] works correctly when given a relative source
        /// </summary>
        [Test]
        public void SaveExpression8 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }
            if (File.Exists (GetBasePath () + "test2.txt")) {
                File.Delete (GetBasePath () + "test2.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "@/0/|/1/?name")
                .Add ("test1.txt").LastChild
                    .Add ("success1", "name").Parent
                .Add ("test2.txt").LastChild
                    .Add ("success2", "name").Parent
                .Add ("rel-source", "@/0/?{0}").LastChild
                    .Add ("", "@/0/?value").Root;
            _context.Raise ("pf.file.save", node);

            // verifying creation of files was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success1", reader.ReadToEnd ());
            }
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test2.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test2.txt")) {
                Assert.AreEqual ("success2", reader.ReadToEnd ());
            }
        }

        /// <summary>
        /// verifies [pf.file.save] works correctly when overwriting an existing file
        /// </summary>
        [Test]
        public void Overwrite ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "this is a LONGER test");
            _context.Raise ("pf.file.save", node);

            node = new Node (string.Empty, "test1.txt")
                .Add ("source", "this is a test");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        /// verifies [pf.file.load] works correctly
        /// </summary>
        [Test]
        public void Load ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }
            
            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "success");
            _context.Raise ("pf.file.save", node);

            // loading file using phosphorus.five
            node = new Node (string.Empty, "test1.txt");
            _context.Raise ("pf.file.load", node);

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }
        
        /// <summary>
        /// verifies [pf.file.load] works correctly when given an expression
        /// </summary>
        [Test]
        public void LoadExpression1 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test2.txt")) {
                File.Delete (GetBasePath () + "test2.txt");
            }

            // creating files using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "success1");
            _context.Raise ("pf.file.save", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("source", "success2");
            _context.Raise ("pf.file.save", node);

            // loading file using phosphorus.five
            node = new Node (string.Empty, "@/0/|/1/?name")
                .Add ("test1.txt")
                .Add ("test2.txt");
            _context.Raise ("pf.file.load", node);

            Assert.AreEqual ("success1", node.LastChild.PreviousNode.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.PreviousNode.Name);
            Assert.AreEqual ("success2", node.LastChild.Value);
            Assert.AreEqual ("test2.txt", node.LastChild.Name);
        }
        
        /// <summary>
        /// verifies [pf.file.load] works correctly when given an expression
        /// </summary>
        [Test]
        public void LoadExpression2 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating files using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "success");
            _context.Raise ("pf.file.save", node);

            // loading file using phosphorus.five
            node = new Node (string.Empty, "te{0}1.txt")
                .Add ("", "st");
            _context.Raise ("pf.file.load", node);

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }
        
        /// <summary>
        /// verifies [pf.file.load] works correctly when given an expression
        /// </summary>
        [Test]
        public void LoadExpression3 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating files using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add ("source", "success");
            _context.Raise ("pf.file.save", node);

            // loading file using phosphorus.five
            node = new Node (string.Empty, "@/{0}/?name")
                .Add ("", "1")
                .Add ("test1.txt");
            _context.Raise ("pf.file.load", node);

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }
    }
}
