
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
    public class Folders : TestBase
    {
        public Folders ()
            : base ("phosphorus.file")
        { }

        /// <summary>
        /// verifies [pf.folder.create] works correctly
        /// </summary>
        [Test]
        public void Create ()
        {
            // deleting folder if it already exists
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1");
            }

            // creating folder using "phosphorus.file"
            Node node = new Node (string.Empty, "test1");
            _context.Raise ("pf.folder.create", node);
            
            // verifying create functioned as is should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
        }
        
        /// <summary>
        /// verifies [pf.folder.create] works correctly
        /// </summary>
        [Test]
        public void CreateExpression1 ()
        {
            // deleting folder if it already exists
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1");
            }
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2");
            }

            // creating folder using "phosphorus.file"
            Node node = new Node (string.Empty, "@/*/?name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.create", node);
            
            // verifying create functioned as is should
            Assert.AreEqual ("test1", node [2].Name);
            Assert.AreEqual (true, node [2].Value);
            Assert.AreEqual ("test2", node [3].Name);
            Assert.AreEqual (true, node [3].Value);
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test2"));
        }
        
        /// <summary>
        /// verifies [pf.folder.create] works correctly
        /// </summary>
        [Test]
        public void CreateExpression2 ()
        {
            // deleting folder if it already exists
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1");
            }
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2");
            }
            
            // creating folder using "phosphorus.file"
            Node node = new Node (string.Empty, "@/*/!/0/?{0}")
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.create", node);

            // verifying create functioned as is should
            Assert.AreEqual ("test1", node [3].Name);
            Assert.AreEqual (true, node [3].Value);
            Assert.AreEqual ("test2", node [4].Name);
            Assert.AreEqual (true, node [4].Value);
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test2"));
        }
        
        /// <summary>
        /// verifies [pf.folder.create] works correctly
        /// </summary>
        [Test]
        public void CreateExpression3 ()
        {
            // deleting folder if it already exists
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1");
            }
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2");
            }

            // making sure one of our folders exists from before, to verify create returns false for this bugger
            if (!Directory.Exists (GetBasePath () + "test3")) {
                Directory.CreateDirectory (GetBasePath () + "test3");
            }

            // creating folder using "phosphorus.file"
            Node node = new Node (string.Empty, "@/*/?name")
                .Add ("test1")
                .Add ("test2")
                .Add ("test3");
            _context.Raise ("pf.folder.create", node);

            // verifying create functioned as is should
            Assert.AreEqual ("test1", node [3].Name);
            Assert.AreEqual (true, node [3].Value);
            Assert.AreEqual ("test2", node [4].Name);
            Assert.AreEqual (true, node [4].Value);
            Assert.AreEqual ("test3", node [5].Name);
            Assert.AreEqual (false, node [5].Value); // supposed to return false!
        }

        /// <summary>
        /// verifies [pf.folder.exists] works correctly
        /// </summary>
        [Test]
        public void Exists ()
        {
            // creating folder if it doesn't already exists
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // checking to see if folder exists using "phosphorus.file"
            Node node = new Node (string.Empty, "test1");
            _context.Raise ("pf.folder.exists", node);

            // verifying exists returned true as it should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.exists] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression1 ()
        {
            // creating folder if it doesn't already exists
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }
            if (!Directory.Exists (GetBasePath () + "test2")) {
                Directory.CreateDirectory (GetBasePath () + "test2");
            }

            // checking to see if folder exists using "phosphorus.file"
            Node node = new Node (string.Empty, "@/*/?name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.exists", node);

            // verifying exists returned true as it should
            Assert.AreEqual ("test1", node [2].Name);
            Assert.AreEqual (true, node [2].Value);
            Assert.AreEqual ("test2", node [3].Name);
            Assert.AreEqual (true, node [3].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.exists] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression2 ()
        {
            // creating folder if it doesn't already exists
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }
            if (!Directory.Exists (GetBasePath () + "test2")) {
                Directory.CreateDirectory (GetBasePath () + "test2");
            }

            // checking to see if folder exists using "phosphorus.file"
            Node node = new Node (string.Empty, "@/*/!/0/?{0}")
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.exists", node);

            // verifying exists returned true as it should
            Assert.AreEqual ("test1", node [3].Name);
            Assert.AreEqual (true, node [3].Value);
            Assert.AreEqual ("test2", node [4].Name);
            Assert.AreEqual (true, node [4].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.exists] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression3 ()
        {
            // creating folder if it doesn't already exists
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }
            if (!Directory.Exists (GetBasePath () + "test2")) {
                Directory.CreateDirectory (GetBasePath () + "test2");
            }
            if (Directory.Exists (GetBasePath () + "test3")) {
                Directory.Delete (GetBasePath () + "test3");
            }

            // checking to see if folder exists using "phosphorus.file"
            Node node = new Node (string.Empty, "@/*/?name")
                .Add ("test1")
                .Add ("test2")
                .Add ("test3");
            _context.Raise ("pf.folder.exists", node);

            // verifying exists returned true as it should
            Assert.AreEqual ("test1", node [3].Name);
            Assert.AreEqual (true, node [3].Value);
            Assert.AreEqual ("test2", node [4].Name);
            Assert.AreEqual (true, node [4].Value);
            Assert.AreEqual ("test3", node [5].Name);
            Assert.AreEqual (false, node [5].Value); // this bugger doesn't exist
        }
    }
}
