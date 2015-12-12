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
    ///     unit tests for testing the p5.io file logic
    /// </summary>
    [TestFixture]
    public class Files : TestBase
    {
        public Files ()
            : base ("p5.io", "p5.hyperlisp", "p5.lambda", "p5.types", "p5.io.authorization")
        { }

        [OneTimeSetUp]
        public void Setup()
        {
            // Making sure we're root for current operation
            Context.UpdateTicket (new ApplicationContext.ContextTicket ("root", "root", false));
        }

        /// <summary>
        ///     saves a file for then to check if it exist
        /// </summary>
        [Test]
        public void SaveFileFileExist ()
        {
            // creating file using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is a test");
            Context.RaiseNative ("save-file", node);

            // checking to see if file exists
            node = new Node ("", "/test1.txt");
            Context.RaiseNative ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     saves two files, for the to check if they both exist
        /// </summary>
        [Test]
        public void SaveTwoFilesFileExist ()
        {
            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is test 1");
            Context.RaiseNative ("save-file", node);

            node = new Node ("", "/test2.txt")
                .Add ("src", "this is test 2");
            Context.RaiseNative ("save-file", node);

            // checking to see if files exists
            node = new Node ("", Expression.Create ("/*?name", Context))
                .Add ("/test1.txt")
                .Add ("/test2.txt");
            Context.RaiseNative ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     saves two files, for then to invoke file-exist with a formatted expression
        /// </summary>
        [Test]
        public void SaveTwoFilesFormattedExpressionExist ()
        {
            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is test 1");
            Context.RaiseNative ("save-file", node);
            node = new Node ("", "/test1.txt")
                .Add ("src", "this is test 2");
            Context.RaiseNative ("save-file", node);

            // checking to see if files exists
            node = new Node ("", Expression.Create (@"/*!/0?{0}", Context))
                .Add ("", "name")
                .Add ("/test1.txt")
                .Add ("/test2.txt");
            Context.RaiseNative ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     saves one file, for then to check if it exist using a formatted string
        /// </summary>
        [Test]
        public void SaveFileFormattedStringExist ()
        {
            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is a test");
            Context.RaiseNative ("save-file", node);

            // checking to see if files exists
            node = new Node ("", "/te{0}.txt")
                .Add ("", "st1");
            Context.RaiseNative ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     saves a file, for then to delete it
        /// </summary>
        [Test]
        public void DeleteFile ()
        {
            // creating file using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is a test");
            Context.RaiseNative ("save-file", node);

            // removing file using Phosphorus Five
            node = new Node ("", "/test1.txt");
            Context.RaiseNative ("delete-file", node);

            // verifying removal of file was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
        }

        /// <summary>
        ///     verifies [delete-file] works correctly when given an expression
        /// </summary>
        [Test]
        public void DeleteTwoFilesExpression ()
        {
            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is test 1");
            Context.RaiseNative ("save-file", node);

            node = new Node ("", "/test2.txt")
                .Add ("src", "this is test 2");
            Context.RaiseNative ("save-file", node);

            // removing files using Phosphorus Five
            node = new Node ("", Expression.Create ("/0|/1?name", Context))
                .Add ("/test1.txt")
                .Add ("/test2.txt");
            Context.RaiseNative ("delete-file", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test2.txt"));
        }

        /// <summary>
        ///     saves two files, for the to use file-exist with a formatted expression
        /// </summary>
        [Test]
        public void DeleteTwoFilesFormattedExpression ()
        {
            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is test 1");
            Context.RaiseNative ("save-file", node);

            node = new Node ("", "/test2.txt")
                .Add ("src", "this is test 2");
            Context.RaiseNative ("save-file", node);

            // removing files using Phosphorus Five
            node = new Node ("", Expression.Create ("/1|{0}?name", Context))
                .Add ("", "/2")
                .Add ("/test1.txt")
                .Add ("/test2.txt");
            Context.RaiseNative ("delete-file", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test2.txt"));
        }

        /// <summary>
        ///     saves one file, then deletes it with a formatted string
        /// </summary>
        [Test]
        public void DeleteFileFormattedString ()
        {
            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is a test");
            Context.RaiseNative ("save-file", node);

            // removing files using Phosphorus Five
            node = new Node ("", "/te{0}")
                .Add ("", "st1.txt");
            Context.RaiseNative ("delete-file", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
        }

        /// <summary>
        ///     Saves a file using a formatted string as filepath
        /// </summary>
        [Test]
        public void SaveFormattedString ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node ("", "/te{0}")
                .Add ("", "st1.txt")
                .Add ("src", "this is a test");
            Context.RaiseNative ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Saves a file where src is expression yielding multiple nodes
        /// </summary>
        [Test]
        public void SaveExpressionMultipleNodes ()
        {
            ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
save-file:/test1.txt
  src:x:/../*/_data/*");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("foo1:bar1\r\nfoo2:bar2", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Saves a file using an expression as src, yielding a single node result
        /// </summary>
        [Test]
        public void SaveExpressionSingleNodeResult ()
        {
            ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
save-file:/test1.txt
  src:x:/../*/_data");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("_data\r\n  foo1:bar1\r\n  foo2:bar2", reader.ReadToEnd ());
            }
        }

        [ActiveEvent (Name = "test.save.av1", Protection = EventProtection.LambdaClosed)]
        private static void test_save_av1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "success";
        }
        
        /// <summary>
        ///     Saves a file where src is an Active Event invocation returning a single value
        /// </summary>
        [Test]
        public void SaveSrcActiveEventReturningValue ()
        {
            ExecuteLambda (@"save-file:/test1.txt
  test.save.av1");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }
        
        [ActiveEvent (Name = "test.save.av2", Protection = EventProtection.LambdaClosed)]
        private static void test_save_av2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("foo1", "bar1");
            e.Args.Add ("foo2", "bar2");
        }

        /// <summary>
        ///     Saves a file where src is an Active Event invocation returning multiple nodes
        /// </summary>
        [Test]
        public void SaveActiveEvent ()
        {
            ExecuteLambda (@"save-file:/test1.txt
  test.save.av2");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual (@"""""
  foo1:bar1
  foo2:bar2", reader.ReadToEnd ());
            }
        }
        
        [ActiveEvent (Name = "test.save.av3_1", Protection = EventProtection.LambdaClosed)]
        private static void test_save_av3_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("foo1", "bar1");
        }

        [ActiveEvent (Name = "test.save.av3_2", Protection = EventProtection.LambdaClosed)]
        private static void test_save_av3_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("foo2", "bar2");
        }

        /// <summary>
        ///     Saves a file where src is two Active Event invocations, returning a single node each
        /// </summary>
        [Test]
        public void SaveSrcIsTwoActiveEventsReturningNodes ()
        {
            ExecuteLambda (@"save-file:/test1.txt
  test.save.av3_1
  test.save.av3_2");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual (@"""""
  foo1:bar1
  foo2:bar2", reader.ReadToEnd ());
            }
        }
        
        [ActiveEvent (Name = "test.save.av4_1", Protection = EventProtection.LambdaClosed)]
        private static void test_save_av4_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "succ";
        }

        [ActiveEvent (Name = "test.save.av4_2", Protection = EventProtection.LambdaClosed)]
        private static void test_save_av4_2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "ess";
        }

        /// <summary>
        ///     Saves a file where src is two Active Event invocations, returning a single value each
        /// </summary>
        [Test]
        public void SaveSrcIsTwwoActiveEventsReturningSingleValue ()
        {
            ExecuteLambda (@"save-file:/test1.txt
  test.save.av4_1
  test.save.av4_2");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Saves a file twice, second time with shorter text, to verify file is overwritten
        /// </summary>
        [Test]
        public void OverwriteFile ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is a LONGER test");
            Context.RaiseNative ("save-file", node);

            node = new Node ("", "/test1.txt")
                .Add ("src", "this is a test");
            Context.RaiseNative ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Load two files using expressions
        /// </summary>
        [Test]
        public void LoadTwoFilesUsingExpression ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test2.txt")) {
                File.Delete (GetBasePath () + "test2.txt");
            }

            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "success1");
            Context.RaiseNative ("save-file", node);

            node = new Node ("", "/test2.txt")
                .Add ("src", "success2");
            Context.RaiseNative ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node ("", Expression.Create (@"/""0""|/""1""?name", Context))
                .Add ("/test1.txt")
                .Add ("/test2.txt");
            Context.RaiseNative ("load-file", node);

            Assert.AreEqual ("success1", node.LastChild.PreviousNode.Value);
            Assert.AreEqual ("/test1.txt", node.LastChild.PreviousNode.Name);
            Assert.AreEqual ("success2", node.LastChild.Value);
            Assert.AreEqual ("/test2.txt", node.LastChild.Name);
        }

        /// <summary>
        ///     Loads a single file using a formatted string
        /// </summary>
        [Test]
        public void LoadFormattedString ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node ("", "/te{0}1.txt")
                .Add ("", "st");
            Context.RaiseNative ("load-file", node);

            Assert.AreEqual ("success", node [0].Value);
        }

        /// <summary>
        ///     Loads a file using a formatted expression
        /// </summary>
        [Test]
        public void LoadFormattedExpression ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating files using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "success");
            Context.RaiseNative ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node ("", Expression.Create (@"/""{0}""?name", Context))
                .Add ("", "1")
                .Add ("/test1.txt");
            Context.RaiseNative ("load-file", node);

            Assert.AreEqual ("success", node [0].Value);
        }
        
        /// <summary>
        ///     Loads a hyperlisp file
        /// </summary>
        [Test]
        public void LoadHyperlisp ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.hl")) {
                File.Delete (GetBasePath () + "test1.hl");
            }

            // creating files using p5.file
            var node = new Node ("", "/test1.hl")
                .Add ("src", @"foo1:bar1
foo2:bar2");
            Context.RaiseNative ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node ("load-file", "/test1.hl");
            Context.RaiseNative ("load-file", node);

            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("bar1", node [0] [0].Value);
            Assert.AreEqual ("foo2", node [0] [1].Name);
            Assert.AreEqual ("bar2", node [0] [1].Value);
        }

        /// <summary>
        ///     Tries to acces "auth" file
        /// </summary>
        [Test]
        public void ReadAuthFileAsGuest ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                _role = "guest";
                _username = "guest";
                try
                {
                    var node = new Node ("", _auth);
                    Context.RaiseNative ("load-file", node);
                }
                finally 
                {
                    _role = "root";
                    _username = "root";
                }
            });
        }

        /// <summary>
        ///     Tries to acces "auth" file
        /// </summary>
        [Test]
        public void ReadAuthFileAsGuestFileNameUppers ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                _role = "guest";
                _username = "guest";
                try
                {
                    var node = new Node ("", _auth.ToUpper ());
                    Context.RaiseNative ("load-file", node);
                }
                finally 
                {
                    _role = "root";
                    _username = "root";
                }
            });
        }

        /// <summary>
        ///     Tries to acces "auth" file
        /// </summary>
        [Test]
        public void ReadAuthFileAsRoot ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                var node = new Node ("", _auth);
                Context.RaiseNative ("load-file", node);
            });
        }

        /// <summary>
        ///     Tries to acces "auth" file
        /// </summary>
        [Test]
        public void ReadAuthFileWithSlash ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                var node = new Node ("", "/" + _auth);
                Context.RaiseNative ("load-file", node);
            });
        }

        /// <summary>
        ///     Tries to acces "auth" file
        /// </summary>
        [Test]
        public void ReadAuthFileWithDot ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                var node = new Node ("", _auth + ".");
                Context.RaiseNative ("load-file", node);
            });
        }

        /// <summary>
        ///     Tries to write to "auth" file
        /// </summary>
        [Test]
        public void WriteAuthFileAsRoot ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                var node = new Node ("", _auth + ".");
                Context.RaiseNative ("save-file", node);
            });
        }

        /// <summary>
        ///     Tries to acces "auth" file
        /// </summary>
        [Test]
        public void WriteAuthFileUppersAsGuest ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                _role = "guest";
                _username = "guest";
                try
                {
                    var node = new Node ("", _auth.ToUpper ());
                    Context.RaiseNative ("save-file", node);
                }
                finally 
                {
                    _role = "root";
                    _username = "root";
                }
            });
        }

        /// <summary>
        ///     Tries to access another user's data
        /// </summary>
        [Test]
        public void ReadAnotherUsersDataAsRoot ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                var node = new Node ("", "/users/foo/foo.txt");
                Context.RaiseNative ("load-file", node);
            });
        }

        /// <summary>
        ///     Tries to write to another user's data
        /// </summary>
        [Test]
        public void SaveToAnotherUsersDataAsRoot ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                var node = new Node ("", "/users/foo/foo.txt");
                Context.RaiseNative ("save-file", node);
            });
        }

        /// <summary>
        ///     Tries to load a file without having an initial "/"
        /// </summary>
        [Test]
        public void LoadFileWithoutInitialSlash ()
        {
            // Creating file using p5.file
            var node = new Node ("", "/test1.txt")
                .Add ("src", "this is a test");
            Context.RaiseNative ("save-file", node);

            // removing directory using "phosphorus.file"
            node = new Node ("", "test1.txt");
            Assert.Throws<LambdaException> (delegate {
                Context.RaiseNative ("load-file", node);
            });
        }

        /// <summary>
        ///     Tries to save a file without having an initial "/"
        /// </summary>
        [Test]
        public void SaveFileWithoutInitialSlash ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                // Creating file using p5.file
                var node = new Node ("", "test1.txt")
                    .Add ("src", "this is a test");
                Context.RaiseNative ("save-file", node);
            });
        }

        /// <summary>
        ///     Tries to check if a file exist without having an initial "/"
        /// </summary>
        [Test]
        public void FileExistWithoutInitialSlash ()
        {
            Assert.Throws<LambdaSecurityException> (delegate {
                // Creating file using p5.file
                var node = new Node ("", "test1.txt")
                    .Add ("src", "this is a test");
                Context.RaiseNative ("file-exist", node);
            });
        }

        /// <summary>
        ///     Tries to delete a file without having an initial "/"
        /// </summary>
        [Test]
        public void DeleteFileWithoutInitialSlash ()
        {
            // Creating file using p5.file if it does not exist
            if (!File.Exists (GetBasePath () + "test1.txt")) {

                // File didn't exist, creating it
                var node = new Node ("", "/test1.txt")
                    .Add ("src", "this is a test");
                Context.RaiseNative ("save-file", node);
            }

            Assert.Throws<LambdaSecurityException> (delegate {
                var result = new Node ("", "test1.txt");
                Context.RaiseNative ("delete-file", result);
            });
        }

        /// <summary>
        ///     Tries to copy a file without having an initial "/" in source file
        /// </summary>
        [Test]
        public void CopyFileWithoutInitialSlashInSource ()
        {
            // Creating file using p5.file if it does not exist
            if (!File.Exists (GetBasePath () + "test1.txt")) {

                // File didn't exist, creating it
                var node = new Node ("", "/test1.txt")
                    .Add ("src", "this is a test");
                Context.RaiseNative ("save-file", node);
            }

            // Copy file
            var result = new Node ("", "test1.txt")
                .Add ("to", "/test2.txt");
            Assert.Throws<LambdaException> (delegate {
                Context.RaiseNative ("copy-file", result);
            });
        }

        /// <summary>
        ///     Tries to copy a file without having an initial "/" in source file
        /// </summary>
        [Test]
        public void CopyFileWithoutInitialSlashInDestination ()
        {
            // Creating file using p5.file if it does not exist
            if (!File.Exists (GetBasePath () + "test1.txt")) {

                // File didn't exist, creating it
                var node = new Node ("", "/test1.txt")
                    .Add ("src", "this is a test");
                Context.RaiseNative ("save-file", node);
            }

            // Copy file
            var result = new Node ("", "/test1.txt")
                .Add ("to", "test2.txt");
            Assert.Throws<LambdaSecurityException> (delegate {
                Context.RaiseNative ("copy-file", result);
            });
        }

        /// <summary>
        ///     Tries to move a file without having an initial "/" in source file
        /// </summary>
        [Test]
        public void MoveFileWithoutInitialSlashInSource ()
        {
            // Creating file using p5.file if it does not exist
            if (!File.Exists (GetBasePath () + "test1.txt")) {

                // File didn't exist, creating it
                var node = new Node ("", "/test1.txt")
                    .Add ("src", "this is a test");
                Context.RaiseNative ("save-file", node);
            }

            // Copy file
            var result = new Node ("", "test1.txt")
                .Add ("to", "/test2.txt");
            Assert.Throws<LambdaSecurityException> (delegate {
                Context.RaiseNative ("move-file", result);
            });
        }

        /// <summary>
        ///     Tries to move a file without having an initial "/" in source file
        /// </summary>
        [Test]
        public void MoveFileWithoutInitialSlashInDestination ()
        {
            // Creating file using p5.file if it does not exist
            if (!File.Exists (GetBasePath () + "test1.txt")) {

                // File didn't exist, creating it
                var node = new Node ("", "/test1.txt")
                    .Add ("src", "this is a test");
                Context.RaiseNative ("save-file", node);
            }

            // Copy file
            var result = new Node ("", "/test1.txt")
                .Add ("to", "test2.txt");
            Assert.Throws<LambdaSecurityException> (delegate {
                Context.RaiseNative ("move-file", result);
            });
        }
    }
}