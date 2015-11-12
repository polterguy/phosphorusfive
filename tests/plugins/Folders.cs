/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using NUnit.Framework;
using p5.core;
using p5.exp;

namespace p5.unittests.plugins
{
    /// <summary>
    ///     unit tests for testing the p5.io folders logic
    /// </summary>
    [TestFixture]
    public class Folders : TestBase
    {
        public Folders ()
            : base ("p5.io", "p5.hyperlisp", "p5.lambda", "p5.types") { }

        /// <summary>
        ///     verifies [create-folder] works correctly
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
            Context.Raise ("create-folder", node);

            // verifying create functioned as is should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
        }

        /// <summary>
        ///     verifies [create-folder] works correctly
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
            var node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("create-folder", node);

            // verifying create functioned as is should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test2"));
        }

        /// <summary>
        ///     verifies [create-folder] works correctly
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
            var node = new Node (string.Empty, Expression.Create ("/*!/0?{0}", Context))
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("create-folder", node);

            // verifying create functioned as is should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test2"));
        }

        /// <summary>
        ///     verifies [create-folder] works correctly
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
            var node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1")
                .Add ("test2")
                .Add ("test3");
            Context.Raise ("create-folder", node);

            // verifying create functioned as is should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual ("test3", node [2].Name);
            Assert.AreEqual (false, node [2].Value); // supposed to return false!
        }

        /// <summary>
        ///     verifies [folder-exist] works correctly
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
            Context.Raise ("folder-exist", node);

            // verifying exists returned true as it should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     verifies [folder-exist] works correctly
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
            var node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("folder-exist", node);

            // verifying exists returned true as it should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
        }

        /// <summary>
        ///     verifies [folder-exist] works correctly
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
            var node = new Node (string.Empty, Expression.Create ("/*!/0?{0}", Context))
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("folder-exist", node);

            // verifying exists returned true as it should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
        }

        /// <summary>
        ///     verifies [folder-exist] works correctly
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
            var node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1")
                .Add ("test2")
                .Add ("test3");
            Context.Raise ("folder-exist", node);

            // verifying exists returned true as it should
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual ("test3", node [2].Name);
            Assert.AreEqual (false, node [2].Value); // this bugger doesn't exist
        }

        /// <summary>
        ///     verifies [folder-exist] works correctly
        /// </summary>
        [Test]
        public void ListFiles ()
        {
            // creating folder if it doesn't already exists
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // creating files within folder
            var node = new Node (string.Empty, Expression.Create ("/0?name", Context))
                .Add ("test1/test1.txt")
                    .Add ("src", "success");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, Expression.Create ("/0?name", Context))
                .Add ("test1/test2.txt")
                    .Add ("src", "success");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, Expression.Create ("/0?name", Context))
                .Add ("test1/test3.txt")
                    .Add ("src", "success");
            Context.Raise ("save-file", node);

            // listing files within folder
            node = new Node (string.Empty, "test1");
            Context.Raise ("list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test1.txt", node [0].Value);
            Assert.AreEqual ("test1/test2.txt", node [1].Value);
            Assert.AreEqual ("test1/test3.txt", node [2].Value);
        }

        /// <summary>
        ///     verifies [folder-exist] works correctly
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
            var node = new Node (string.Empty, "test1/test1.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, "test2/test2.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, "test1/test3.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);

            // listing files within folder
            node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test1.txt", node [0].Value);
            Assert.AreEqual ("test1/test3.txt", node [1].Value);
            Assert.AreEqual ("test2/test2.txt", node [2].Value);
        }

        /// <summary>
        ///     verifies [folder-exist] works correctly
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
            var node = new Node (string.Empty, "test1/test1.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, "test2/test2.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, "test1/test3.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);

            // listing files within folder
            node = new Node (string.Empty, Expression.Create ("/*!/*/?{0}", Context))
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test1.txt", node [0].Value);
            Assert.AreEqual ("test1/test3.txt", node [1].Value);
            Assert.AreEqual ("test2/test2.txt", node [2].Value);
        }

        /// <summary>
        ///     verifies [folder-exist] works correctly
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
            var node = new Node (string.Empty, "test1/test1.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, "test1/test2.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, "test1/test3.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);

            // listing files within folder
            node = new Node (string.Empty, "te{0}")
                .Add (string.Empty, "st1");
            Context.Raise ("list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test1.txt", node [0].Value);
            Assert.AreEqual ("test1/test2.txt", node [1].Value);
            Assert.AreEqual ("test1/test3.txt", node [2].Value);
        }

        /// <summary>
        ///     verifies [list-folders] works correctly
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
            Context.Raise ("list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/xxx", node [0].Value);
            Assert.AreEqual ("test1/yyy", node [1].Value);
        }

        /// <summary>
        ///     verifies [list-folders] works correctly
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
            var node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/xxx", node [0].Value);
            Assert.AreEqual ("test2/yyy", node [1].Value);
        }

        /// <summary>
        ///     verifies [list-folders] works correctly
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
            var node = new Node (string.Empty, Expression.Create ("{0}?name", Context))
                .Add (string.Empty, "/*!/*/")
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/xxx", node [0].Value);
            Assert.AreEqual ("test2/yyy", node [1].Value);
        }

        /// <summary>
        ///     verifies [list-folders] works correctly
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
            Context.Raise ("list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/xxx", node [0].Value);
            Assert.AreEqual ("test1/yyy", node [1].Value);
        }

        [ActiveEvent (Name = "test.list-folders-1")]
        private static void ListFolderEvent (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "test1";
        }
        
        /// <summary>
        ///     verifies [list-folders] works correctly
        /// </summary>
        [Test]
        public void ListFoldersExpression4 ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            Directory.CreateDirectory (GetBasePath () + "test1/test-folder");

            // listing folders within folder
            var node = ExecuteLambda (@"list-folders
  test.list-folders-1
insert-before:x:/../0
  src:x:/../*");

            // verifying list-files returned true as it should
            Assert.AreEqual ("test1/test-folder", node [0] [0].Value);
        }

        /// <summary>
        ///     verifies [delete-folder] works as it should
        /// </summary>
        [Test]
        public void RemoveFolder1 ()
        {
            // creating directory to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "test1");
            Context.Raise ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test1", node [0].Name);
        }

        /// <summary>
        ///     verifies [delete-folder] works as it should
        /// </summary>
        [Test]
        public void RemoveFolder2 ()
        {
            // creating directory to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");

                // creating a file within directory, to verify remove removes recursively
                var createFile = new Node (string.Empty, "test1/test1.txt")
                    .Add ("src", "this is a test");
                Context.Raise ("save-file", createFile);
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "test1");
            Context.Raise ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test1", node [0].Name);
        }

        /// <summary>
        ///     verifies [delete-folder] works as it should
        /// </summary>
        [Test]
        public void RemoveFolder3 ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }
            if (!Directory.Exists (GetBasePath () + "test2")) {
                Directory.CreateDirectory (GetBasePath () + "test2");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test2"));
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual ("test2", node [1].Name);
        }

        /// <summary>
        ///     verifies [delete-folder] works as it should
        /// </summary>
        [Test]
        public void RemoveFolder42 ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }
            if (!Directory.Exists (GetBasePath () + "test2")) {
                Directory.CreateDirectory (GetBasePath () + "test2");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, Expression.Create ("/*!/*/?{0}", Context))
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add ("test2");
            Context.Raise ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test2"));
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test1", node [0].Name);
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual ("test2", node [1].Name);
        }

        /// <summary>
        ///     verifies [delete-folder] works as it should
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
            Context.Raise ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test1", node [0].Name);
        }
    }
}