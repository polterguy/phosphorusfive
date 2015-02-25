
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests.plugins
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
                Directory.Delete (GetBasePath () + "test1", true);
            }

            // creating folder using "phosphorus.file"
            var node = new Node (string.Empty, "test1");
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
                Directory.Delete (GetBasePath () + "test1", true);
            }
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2", true);
            }

            // creating folder using "phosphorus.file"
            var node = new Node (string.Empty, "@/*/?name")
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
                Directory.Delete (GetBasePath () + "test1", true);
            }
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2", true);
            }
            
            // creating folder using "phosphorus.file"
            var node = new Node (string.Empty, "@/*/!/0/?{0}")
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
                Directory.Delete (GetBasePath () + "test1", true);
            }
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2", true);
            }

            // making sure one of our folders exists from before, to verify create returns false for this bugger
            if (!Directory.Exists (GetBasePath () + "test3")) {
                Directory.CreateDirectory (GetBasePath () + "test3");
            }

            // creating folder using "phosphorus.file"
            var node = new Node (string.Empty, "@/*/?name")
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
            var node = new Node (string.Empty, "test1");
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
            var node = new Node (string.Empty, "@/*/?name")
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
            var node = new Node (string.Empty, "@/*/!/0/?{0}")
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
                Directory.Delete (GetBasePath () + "test3", true);
            }

            // checking to see if folder exists using "phosphorus.file"
            var node = new Node (string.Empty, "@/*/?name")
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
        
        /// <summary>
        /// verifies [pf.folder.exists] works correctly
        /// </summary>
        [Test]
        public void ListFiles ()
        {
            // creating folder if it doesn't already exists
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // creating files within folder
            var node = new Node (string.Empty, "@/*/!/*/source/?name")
                .Add ("test1/test1.txt")
                .Add ("test1/test2.txt")
                .Add ("test1/test3.txt")
                .Add ("source", "success");
            _context.Raise ("pf.file.save", node);

            // listing files within folder
            node = new Node (string.Empty, "test1");
            _context.Raise ("pf.folder.list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test1.txt", node [0].Value);
            Assert.AreEqual ("test1/test2.txt", node [1].Value);
            Assert.AreEqual ("test1/test3.txt", node [2].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.exists] works correctly
        /// </summary>
        [Test]
        public void ListFilesExpression1 ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test2");

            // creating files within folder
            var node = new Node (string.Empty, "@/*/!/*/source/?name")
                .Add ("test1/test1.txt")
                .Add ("test2/test2.txt")
                .Add ("test1/test3.txt")
                .Add ("source", "success");
            _context.Raise ("pf.file.save", node);

            // listing files within folder
            node = new Node (string.Empty, "@/*/?name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test1.txt", node [2].Value);
            Assert.AreEqual ("test1/test3.txt", node [3].Value);
            Assert.AreEqual ("test2/test2.txt", node [4].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.exists] works correctly
        /// </summary>
        [Test]
        public void ListFilesExpression2 ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test2");

            // creating files within folder
            var node = new Node (string.Empty, "@/*/!/*/source/?name")
                .Add ("test1/test1.txt")
                .Add ("test2/test2.txt")
                .Add ("test1/test3.txt")
                .Add ("source", "success");
            _context.Raise ("pf.file.save", node);

            // listing files within folder
            node = new Node (string.Empty, "@/*/!/*//?{0}")
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test1.txt", node [3].Value);
            Assert.AreEqual ("test1/test3.txt", node [4].Value);
            Assert.AreEqual ("test2/test2.txt", node [5].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.exists] works correctly
        /// </summary>
        [Test]
        public void ListFilesExpression3 ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");

            // creating files within folder
            var node = new Node (string.Empty, "@/*/!/*/source/?name")
                .Add ("test1/test1.txt")
                .Add ("test1/test2.txt")
                .Add ("test1/test3.txt")
                .Add ("source", "success");
            _context.Raise ("pf.file.save", node);

            // listing files within folder
            node = new Node (string.Empty, "te{0}")
                .Add (string.Empty, "st1");
            _context.Raise ("pf.folder.list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test1.txt", node [1].Value);
            Assert.AreEqual ("test1/test2.txt", node [2].Value);
            Assert.AreEqual ("test1/test3.txt", node [3].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.list-folders] works correctly
        /// </summary>
        [Test]
        public void ListFolders ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            Directory.CreateDirectory (GetBasePath () + "test1/xxx");
            Directory.CreateDirectory (GetBasePath () + "test1/yyy");

            // listing folders within folder
            var node = new Node (string.Empty, "test1");
            _context.Raise ("pf.folder.list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/xxx", node [0].Value);
            Assert.AreEqual ("test1/yyy", node [1].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.list-folders] works correctly
        /// </summary>
        [Test]
        public void ListFoldersExpression1 ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            Directory.CreateDirectory (GetBasePath () + "test2");
            Directory.CreateDirectory (GetBasePath () + "test1/xxx");
            Directory.CreateDirectory (GetBasePath () + "test2/yyy");

            // listing folders within folder
            var node = new Node (string.Empty, "@/*/?name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/xxx", node [2].Value);
            Assert.AreEqual ("test2/yyy", node [3].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.list-folders] works correctly
        /// </summary>
        [Test]
        public void ListFoldersExpression2 ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            Directory.CreateDirectory (GetBasePath () + "test2");
            Directory.CreateDirectory (GetBasePath () + "test1/xxx");
            Directory.CreateDirectory (GetBasePath () + "test2/yyy");

            // listing folders within folder
            var node = new Node (string.Empty, "@{0}?name")
                .Add (string.Empty, "/*/!/*//")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/xxx", node [3].Value);
            Assert.AreEqual ("test2/yyy", node [4].Value);
        }
        
        /// <summary>
        /// verifies [pf.folder.list-folders] works correctly
        /// </summary>
        [Test]
        public void ListFoldersExpression3 ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            Directory.CreateDirectory (GetBasePath () + "test1/xxx");
            Directory.CreateDirectory (GetBasePath () + "test1/yyy");

            // listing folders within folder
            var node = new Node (string.Empty, "te{0}")
                .Add (string.Empty, "st1");
            _context.Raise ("pf.folder.list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/xxx", node [1].Value);
            Assert.AreEqual ("test1/yyy", node [2].Value);
        }

        /// <summary>
        /// verifies [pf.folder.remove] works as it should
        /// </summary>
        [Test]
        public void Remove1 ()
        {
            // creating directory to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "test1");
            _context.Raise ("pf.folder.remove", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test1", node [0].Name);
        }

        /// <summary>
        /// verifies [pf.folder.remove] works as it should
        /// </summary>
        [Test]
        public void Remove2 ()
        {
            // creating directory to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");

                // creating a file within directory, to verify remove removes recursively
                var createFile = new Node (string.Empty, "test1/test1.txt")
                    .Add ("source", "this is a test");
                _context.Raise ("pf.file.save", createFile);
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "test1");
            _context.Raise ("pf.folder.remove", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test1", node [0].Name);
        }
        
        /// <summary>
        /// verifies [pf.folder.remove] works as it should
        /// </summary>
        [Test]
        public void RemoveExpression1 ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }
            if (!Directory.Exists (GetBasePath () + "test2")) {
                Directory.CreateDirectory (GetBasePath () + "test2");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "@/*/?name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.remove", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test2"));
            Assert.AreEqual (true, node [2].Value);
            Assert.AreEqual ("test1", node [2].Name);
            Assert.AreEqual (true, node [3].Value);
            Assert.AreEqual ("test2", node [3].Name);
        }
        
        /// <summary>
        /// verifies [pf.folder.remove] works as it should
        /// </summary>
        [Test]
        public void RemoveExpression2 ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }
            if (!Directory.Exists (GetBasePath () + "test2")) {
                Directory.CreateDirectory (GetBasePath () + "test2");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "@/*/!/*//?{0}")
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add ("test2");
            _context.Raise ("pf.folder.remove", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test2"));
            Assert.AreEqual (true, node [3].Value);
            Assert.AreEqual ("test1", node [3].Name);
            Assert.AreEqual (true, node [4].Value);
            Assert.AreEqual ("test2", node [4].Name);
        }
        
        /// <summary>
        /// verifies [pf.folder.remove] works as it should
        /// </summary>
        [Test]
        public void RemoveExpression3 ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "te{0}")
                .Add (string.Empty, "st1");
            _context.Raise ("pf.folder.remove", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual ("test1", node [1].Name);
        }
    }
}
