/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using NUnit.Framework;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.unittests.plugins
{
    /// <summary>
    ///     unit tests for testing the p5.io folders logic
    /// </summary>
    [TestFixture]
    public class Folders : TestBase
    {
        public Folders ()
            : base ("p5.io", "p5.hyperlisp", "p5.lambda", "p5.types", "p5.security", "p5.io.authorization")
        { }

        [TestFixtureSetUp]
        public void Setup()
        {
            // Making sure we're root for current operation
            Context.UpdateTicket (new ApplicationContext.ContextTicket ("root", "root", false));
        }

        /// <summary>
        ///     Creates a folder
        /// </summary>
        [Test]
        public void CreateFolder ()
        {
            // deleting folder if it already exists
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }

            // creating folder using "phosphorus.file"
            var node = new Node (string.Empty, "/test1/");
            Context.RaiseNative ("create-folder", node);

            // verifying create functioned as is should
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
        }

        /// <summary>
        ///     Creates two folders using an expression
        /// </summary>
        [Test]
        public void CreateTwoFoldersExpression ()
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
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("create-folder", node);

            // verifying create functioned as is should
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test2"));
        }

        /// <summary>
        ///     Creates two folders using a formatted expression
        /// </summary>
        [Test]
        public void CreateFolderFormattedExpression ()
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
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("create-folder", node);

            // verifying create functioned as is should
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (true, Directory.Exists (GetBasePath () + "test2"));
        }

        /// <summary>
        ///     Creates three folders using an expression, where one folder exist from before
        /// </summary>
        [Test]
        [ExpectedException(typeof(LambdaException))]
        public void CreateThreeFoldersExpressionOneExist ()
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
                .Add ("/test1")
                .Add ("/test2")
                .Add ("/test3");
            Context.RaiseNative ("create-folder", node);
        }

        /// <summary>
        ///     Checks if an existing folder exist using folder-exist
        /// </summary>
        [Test]
        public void FolderExist ()
        {
            // creating folder if it doesn't already exists
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // checking to see if folder exists using "phosphorus.file"
            var node = new Node (string.Empty, "/test1/");
            Context.RaiseNative ("folder-exist", node);

            // verifying exists returned true as it should
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     Checks if two folders exist using an expression
        /// </summary>
        [Test]
        public void TwoFolderExistExpression ()
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
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("folder-exist", node);

            // verifying exists returned true as it should
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     Checks if two folders exist using a formatted expression
        /// </summary>
        [Test]
        public void TwoFoldersExistFormattedExpression ()
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
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("folder-exist", node);

            // verifying exists returned true as it should
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     Checks if three folders exist using an expression, where one does not exist
        /// </summary>
        [Test]
        public void ThreeFolderExistExpressionOneFails ()
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
                .Add ("/test1/")
                .Add ("/test2/")
                .Add ("/test3/");
            Context.RaiseNative ("folder-exist", node);

            // verifying exists returned true as it should
            Assert.AreEqual (false, node.Value); // this bugger doesn't exist
        }

        /// <summary>
        ///     List files within a folder using a filter
        /// </summary>
        [Test]
        public void ListFilesStringFilter ()
        {
            // creating folder if it doesn't already exists
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // creating files within folder
            var node = new Node (string.Empty, Expression.Create ("/0?name", Context))
                .Add ("/test1/test1.txt")
                    .Add ("src", "success");
            Context.RaiseNative ("save-file", node);
            node = new Node (string.Empty, Expression.Create ("/0?name", Context))
                .Add ("/test1/test2.txt")
                    .Add ("src", "success");
            Context.RaiseNative ("save-file", node);
            node = new Node (string.Empty, Expression.Create ("/0?name", Context))
                .Add ("/test1/test3.txt")
                    .Add ("src", "success");
            Context.RaiseNative ("save-file", node);

            // listing files within folder
            node = new Node (string.Empty, "/test1/");
            Context.RaiseNative ("list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("/test1/test1.txt", node [0].Name);
            Assert.AreEqual ("/test1/test2.txt", node [1].Name);
            Assert.AreEqual ("/test1/test3.txt", node [2].Name);
        }

        /// <summary>
        ///     List files using an expression leading to a filter
        /// </summary>
        [Test]
        public void ListFilesExpressionFilter ()
        {
            // Deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            if (Directory.Exists (GetBasePath () + "test2")) {
                Directory.Delete (GetBasePath () + "test2", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test2");

            // creating files within folder
            var node = new Node (string.Empty, "/test1/test1.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);
            node = new Node (string.Empty, "/test2/test2.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);
            node = new Node (string.Empty, "/test1/test3.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);

            // listing files within folder
            node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("/test1/test1.txt", node [0].Name);
            Assert.AreEqual ("/test1/test3.txt", node [1].Name);
            Assert.AreEqual ("/test2/test2.txt", node [2].Name);
        }

        /// <summary>
        ///     List files using a formatted expression
        /// </summary>
        [Test]
        public void ListFilesFormattedExpression ()
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
            var node = new Node (string.Empty, "/test1/test1.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);
            node = new Node (string.Empty, "/test2/test2.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);
            node = new Node (string.Empty, "/test1/test3.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);

            // listing files within folder
            node = new Node (string.Empty, Expression.Create ("/*!/*/?{0}", Context))
                .Add (string.Empty, "name")
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("/test1/test1.txt", node [0].Name);
            Assert.AreEqual ("/test1/test3.txt", node [1].Name);
            Assert.AreEqual ("/test2/test2.txt", node [2].Name);
        }

        /// <summary>
        ///     List files using a formatted string as filter
        /// </summary>
        [Test]
        public void ListFilesFormattedStringFilter ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");

            // creating files within folder
            var node = new Node (string.Empty, "/test1/test1.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);
            node = new Node (string.Empty, "/test1/test2.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);
            node = new Node (string.Empty, "/test1/test3.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);

            // listing files within folder
            node = new Node (string.Empty, "/te{0}")
                .Add (string.Empty, "st1/");
            Context.RaiseNative ("list-files", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("/test1/test1.txt", node [0].Name);
            Assert.AreEqual ("/test1/test2.txt", node [1].Name);
            Assert.AreEqual ("/test1/test3.txt", node [2].Name);
        }

        /// <summary>
        ///     List folders using a constant string as filter
        /// </summary>
        [Test]
        public void ListFoldersStringFilter ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            Directory.CreateDirectory (GetBasePath () + "test1/xxx");
            Directory.CreateDirectory (GetBasePath () + "test1/yyy");

            // listing folders within folder
            var node = new Node (string.Empty, "/test1/");
            Context.RaiseNative ("list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("/test1/xxx/", node [0].Name);
            Assert.AreEqual ("/test1/yyy/", node [1].Name);
        }

        /// <summary>
        ///     List folders using an expression as filter
        /// </summary>
        [Test]
        public void ListFoldersExpressionFilter ()
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
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("/test1/xxx/", node [0].Name);
            Assert.AreEqual ("/test2/yyy/", node [1].Name);
        }

        /// <summary>
        ///     List folders using a formatted expression as filter
        /// </summary>
        [Test]
        public void ListFoldersFormattedExpression ()
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
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("/test1/xxx/", node [0].Name);
            Assert.AreEqual ("/test2/yyy/", node [1].Name);
        }

        /// <summary>
        ///     List folders using a formatted string as filter
        /// </summary>
        [Test]
        public void ListFoldersFormattedStringFilter ()
        {
            // deleting and re-creating folders to make sure they're empty and don't contain "garbage"
            if (Directory.Exists (GetBasePath () + "test1")) {
                Directory.Delete (GetBasePath () + "test1", true);
            }
            Directory.CreateDirectory (GetBasePath () + "test1");
            Directory.CreateDirectory (GetBasePath () + "test1/xxx");
            Directory.CreateDirectory (GetBasePath () + "test1/yyy");

            // listing folders within folder
            var node = new Node (string.Empty, "/te{0}/")
                .Add (string.Empty, "st1");
            Context.RaiseNative ("list-folders", node);

            // verifying list-files returned true as it should
            Assert.AreEqual ("/test1/xxx/", node [0].Name);
            Assert.AreEqual ("/test1/yyy/", node [1].Name);
        }

        [ActiveEvent (Name = "test.list-folders-1", Protection = EventProtection.LambdaClosed)]
        private static void ListFolderEvent (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "/test1/";
        }
        
        /// <summary>
        ///     List folders using as Active Event result as filter
        /// </summary>
        [Test]
        public void ListFoldersActiveEventFilter ()
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
            Assert.AreEqual ("/test1/test-folder/", node [0] [0].Name);
        }

        /// <summary>
        ///     Deletes a single folder
        /// </summary>
        [Test]
        public void DeleteFolder ()
        {
            // creating directory to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "/test1/");
            Context.RaiseNative ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
        }

        /// <summary>
        ///     Deletes a folder with a file inside of it
        /// </summary>
        [Test]
        public void DeleteFolderWithFile ()
        {
            // creating directory to remove
            if (!Directory.Exists (GetBasePath () + "test1"))
                Directory.CreateDirectory (GetBasePath () + "test1");

            // creating a file within directory, to verify remove removes recursively
            var createFile = new Node (string.Empty, "/test1/test1.txt")
                .Add ("src", "this is a test");
            Context.RaiseNative ("save-file", createFile);

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "/test1/");
            Context.RaiseNative ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
        }

        /// <summary>
        ///     Deletes two folder using an expression
        /// </summary>
        [Test]
        public void DeleteTwoFoldersExpression ()
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
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test2"));
        }

        /// <summary>
        ///     Deletes two folders using a formatted expression
        /// </summary>
        [Test]
        public void DeleteTwoFoldersFormattedExpression ()
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
                .Add ("/test1/")
                .Add ("/test2/");
            Context.RaiseNative ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test2"));
        }

        /// <summary>
        ///     Deletes a folder using a formatted string
        /// </summary>
        [Test]
        public void DeleteFolderFormattedString ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "/te{0}")
                .Add (string.Empty, "st1/");
            Context.RaiseNative ("delete-folder", node);

            // verifying remove works as it should
            Assert.AreEqual (false, Directory.Exists (GetBasePath () + "test1"));
        }

        /// <summary>
        ///     Tries to list files in folder without having a trailing "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void ListFilesWithoutTrailingSlash ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "/test1");
            Context.RaiseNative ("list-files", node);
        }

        /// <summary>
        ///     Tries to list folders in folder without having a trailing "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void ListFoldersWithoutTrailingSlash ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "/test1");
            Context.RaiseNative ("list-folders", node);
        }

        /// <summary>
        ///     Tries to list files in folder without having an initial "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void ListFilesWithoutInitialSlash ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "test1/");
            Context.RaiseNative ("list-files", node);
        }

        /// <summary>
        ///     Tries to list folders in folder without having an initial "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void ListFoldersWithoutInitialSlash ()
        {
            // creating directories to remove
            if (!Directory.Exists (GetBasePath () + "test1")) {
                Directory.CreateDirectory (GetBasePath () + "test1");
            }

            // removing directory using "phosphorus.file"
            var node = new Node (string.Empty, "test1/");
            Context.RaiseNative ("list-folders", node);
        }

        /// <summary>
        ///     Tries to check if a folder exist without having an initial "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void FolderExistWithoutInitialSlash ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }

            // removing directory using "phosphorus.file"
            var result = new Node (string.Empty, "test1/");
            Context.RaiseNative ("folder-exist", result);
        }

        /// <summary>
        ///     Tries to check if a folder exist without having a trailing "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void FolderExistWithoutTrailingSlash ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }

            // removing directory using "phosphorus.file"
            var result = new Node (string.Empty, "/test1");
            Context.RaiseNative ("folder-exist", result);
        }

        /// <summary>
        ///     Tries to delete a folder without having an initial "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void DeleteFolderWithoutInitialSlash ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }

            // removing directory using "phosphorus.file"
            var result = new Node (string.Empty, "test1/");
            Context.RaiseNative ("delete-folder", result);
        }

        /// <summary>
        ///     Tries to delete a folder without having a trailing "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void DeleteFolderWithoutTrailingSlash ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }

            // removing directory using "phosphorus.file"
            var result = new Node (string.Empty, "/test1");
            Context.RaiseNative ("delete-folder", result);
        }

        /// <summary>
        ///     Tries to create a folder without having an initial "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void CreateFolderWithoutInitialSlash ()
        {
            if (Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "test1/");
            Context.RaiseNative ("create-folder", result);
        }

        /// <summary>
        ///     Tries to create a folder without having a trailing "/"
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void CreateFolderWithoutTrailingSlash ()
        {
            if (Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "/test1");
            Context.RaiseNative ("create-folder", result);
        }

        /// <summary>
        ///     Tries to copy a folder without having an initial "/" in source
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void CopyFolderWithoutInitialSlashInSource ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }
            if (Directory.Exists ("test2")) {
                var node = new Node (string.Empty, "/test2/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "test1/")
                .Add ("to", "/test2/");
            Context.RaiseNative ("copy-folder", result);
        }

        /// <summary>
        ///     Tries to copy a folder without having aa trailing "/" in source
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void CopyFolderWithoutTrailingSlashInSource ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }
            if (Directory.Exists ("test2")) {
                var node = new Node (string.Empty, "/test2/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "/test1")
                .Add ("to", "/test2/");
            Context.RaiseNative ("copy-folder", result);
        }

        /// <summary>
        ///     Tries to copy a folder without having an initial "/" in destination
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void CopyFolderWithoutInitialSlashInDestinatio  ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }
            if (Directory.Exists ("test2")) {
                var node = new Node (string.Empty, "/test2/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "/test1/")
                .Add ("to", "test2/");
            Context.RaiseNative ("copy-folder", result);
        }

        /// <summary>
        ///     Tries to copy a folder without having a trailing "/" in destination
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void CopyFolderWithoutTrailingSlashInDestination ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }
            if (Directory.Exists ("test2")) {
                var node = new Node (string.Empty, "/test2/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "/test1/")
                .Add ("to", "/test2");
            Context.RaiseNative ("copy-folder", result);
        }

        /// <summary>
        ///     Tries to move a folder without having an initial "/" in source
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void MoveFolderWithoutInitialSlashInSource ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }
            if (Directory.Exists ("test2")) {
                var node = new Node (string.Empty, "/test2/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "test1/")
                .Add ("to", "/test2/");
            Context.RaiseNative ("move-folder", result);
        }

        /// <summary>
        ///     Tries to move a folder without having aa trailing "/" in source
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void MoveFolderWithoutTrailingSlashInSource ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }
            if (Directory.Exists ("test2")) {
                var node = new Node (string.Empty, "/test2/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "/test1")
                .Add ("to", "/test2/");
            Context.RaiseNative ("move-folder", result);
        }

        /// <summary>
        ///     Tries to move a folder without having an initial "/" in destination
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void MoveFolderWithoutInitialSlashInDestinatio  ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }
            if (Directory.Exists ("test2")) {
                var node = new Node (string.Empty, "/test2/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "/test1/")
                .Add ("to", "test2/");
            Context.RaiseNative ("move-folder", result);
        }

        /// <summary>
        ///     Tries to move a folder without having a trailing "/" in destination
        /// </summary>
        [Test]
        [ExpectedException (typeof (LambdaException))]
        public void MoveFolderWithoutTrailingSlashInDestination ()
        {
            if (!Directory.Exists ("test1")) {
                var node = new Node (string.Empty, "/test1/");
                Context.RaiseNative ("create-folder", node);
            }
            if (Directory.Exists ("test2")) {
                var node = new Node (string.Empty, "/test2/");
                Context.RaiseNative ("delete-folder", node);
            }
            var result = new Node (string.Empty, "/test1/")
                .Add ("to", "/test2");
            Context.RaiseNative ("move-folder", result);
        }
    }
}