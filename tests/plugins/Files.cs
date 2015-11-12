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
    ///     unit tests for testing the p5.io file logic
    /// </summary>
    [TestFixture]
    public class Files : TestBase
    {
        public Files ()
            : base ("p5.io", "p5.hyperlisp", "p5.lambda", "p5.types")
        { }

        /*
         * necessary to return "root folder" of executing Assembly
         * p5.io relies on this Active Event as a sink
         */
        [ActiveEvent (Name = "p5.core.application-folder")]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = GetBasePath ();
        }

        /// <summary>
        ///     saves a file for then to check if it exist
        /// </summary>
        [Test]
        public void SaveFileFileExist ()
        {
            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // checking to see if file exists
            node = new Node (string.Empty, "test1.txt");
            Context.Raise ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual (true, node.Value);
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
        }

        /// <summary>
        ///     saves two files, for the to check if they both exist
        /// </summary>
        [Test]
        public void SaveTwoFilesFileExist ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 1");
            Context.Raise ("save-file", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("src", "this is test 2");
            Context.Raise ("save-file", node);

            // checking to see if files exists
            node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual (true, node.Value);
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2.txt", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
        }

        /// <summary>
        ///     saves two files, for then to invoke file-exist with a formatted expression
        /// </summary>
        [Test]
        public void SaveTwoFilesFormattedExpressionExist ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 1");
            Context.Raise ("save-file", node);
            node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 2");
            Context.Raise ("save-file", node);

            // checking to see if files exists
            node = new Node (string.Empty, Expression.Create ("/*!/0?{0}", Context))
                .Add (string.Empty, "name")
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2.txt", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
        }

        /// <summary>
        ///     saves one file, for then to check if it exist using a formatted string
        /// </summary>
        [Test]
        public void SaveFileFormattedStringExist ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // checking to see if files exists
            node = new Node (string.Empty, "te{0}.txt")
                .Add (string.Empty, "st1");
            Context.Raise ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
        }

        /// <summary>
        ///     saves a file, for then to delete it
        /// </summary>
        [Test]
        public void DeleteFile ()
        {
            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // removing file using Phosphorus Five
            node = new Node (string.Empty, "test1.txt");
            Context.Raise ("delete-file", node);

            // verifying removal of file was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
            Assert.AreEqual (true, node.Value);
        }

        /// <summary>
        ///     verifies [delete-file] works correctly when given an expression
        /// </summary>
        [Test]
        public void DeleteTwoFilesExpression ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 1");
            Context.Raise ("save-file", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("src", "this is test 2");
            Context.Raise ("save-file", node);

            // removing files using Phosphorus Five
            node = new Node (string.Empty, Expression.Create ("/0|/1?name", Context))
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("delete-file", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test2.txt"));
            Assert.AreEqual (true, node.Value);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual ("test2.txt", node [1].Name);
        }

        /// <summary>
        ///     saves two files, for the to use file-exist with a formatted expression
        /// </summary>
        [Test]
        public void DeleteTwoFilesFormattedExpression ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 1");
            Context.Raise ("save-file", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("src", "this is test 2");
            Context.Raise ("save-file", node);

            // removing files using Phosphorus Five
            node = new Node (string.Empty, Expression.Create ("/1|{0}?name", Context))
                .Add (string.Empty, "/2")
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("delete-file", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test2.txt"));
            Assert.AreEqual (true, node.Value);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual (true, node [1].Value);
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual ("test2.txt", node [1].Name);
        }

        /// <summary>
        ///     saves one file, then deletes it with a formatted string
        /// </summary>
        [Test]
        public void DeleteFileFormattedString ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // removing files using Phosphorus Five
            node = new Node (string.Empty, "te{0}")
                .Add (string.Empty, "st1.txt");
            Context.Raise ("delete-file", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
            Assert.AreEqual (true, node.Value);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test1.txt", node [0].Name);
        }

        /// <summary>
        ///     Saves a file using a formatted file path expression
        /// </summary>
        [Test]
        public void SaveExpressionFilePath ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node (string.Empty, Expression.Create ("/0?name", Context))
                .Add ("test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Saves a file using a formatted expression, leading to multiple results, building a path
        /// </summary>
        [Test]
        public void SaveFormattedExpressionFilePathYieldingMultipleValues ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node (string.Empty, Expression.Create ("/1|/2?{0}", Context))
                .Add (string.Empty, "name")
                .Add ("test1")
                .Add (".txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
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
            var node = new Node (string.Empty, "te{0}")
                .Add (string.Empty, "st1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Saves a file where src is an expression yielding a single result
        /// </summary>
        [Test]
        public void SaveSrcIsExpression ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("success")
                .Add ("src", Expression.Create ("/-?name", Context));
            Context.Raise ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Saves a file where src is an expression yielding multiple results
        /// </summary>
        [Test]
        public void SaveSrcIsExpressionMultiResult ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("succ")
                .Add ("ess")
                .Add ("src", Expression.Create ("/-2|/-1?name", Context));
            Context.Raise ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Saves a file using a formatted expression as src
        /// </summary>
        [Test]
        public void SaveSrcIsFormattedExpression ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("success")
                .Add ("src", Expression.Create ("/{0}?name", Context)).LastChild
                    .Add (string.Empty, "-").Root;
            Context.Raise ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
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
save-file:test1.txt
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
save-file:test1.txt
  src:x:/../*/_data");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("_data\r\n  foo1:bar1\r\n  foo2:bar2", reader.ReadToEnd ());
            }
        }

        [ActiveEvent (Name = "test.save.av1")]
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
            ExecuteLambda (@"save-file:test1.txt
  test.save.av1");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }
        
        [ActiveEvent (Name = "test.save.av2")]
        private static void test_save_av2 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("foo1", "bar1");
            e.Args.Add ("foo2", "bar2");
        }

        /// <summary>
        ///     Saves a file where src is an Active Event invocation returning multiple nodes
        /// </summary>
        [Test]
        public void SaveActiveEvent02 ()
        {
            ExecuteLambda (@"save-file:test1.txt
  test.save.av2");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("foo1:bar1\r\nfoo2:bar2", reader.ReadToEnd ());
            }
        }
        
        [ActiveEvent (Name = "test.save.av3_1")]
        private static void test_save_av3_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("foo1", "bar1");
        }

        [ActiveEvent (Name = "test.save.av3_2")]
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
            ExecuteLambda (@"save-file:test1.txt
  test.save.av3_1
  test.save.av3_2");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("foo1:bar1\r\nfoo2:bar2", reader.ReadToEnd ());
            }
        }
        
        [ActiveEvent (Name = "test.save.av4_1")]
        private static void test_save_av4_1 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "succ";
        }

        [ActiveEvent (Name = "test.save.av4_2")]
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
            ExecuteLambda (@"save-file:test1.txt
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
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a LONGER test");
            Context.Raise ("save-file", node);

            node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

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
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "success1");
            Context.Raise ("save-file", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("src", "success2");
            Context.Raise ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node (string.Empty, Expression.Create ("/0|/1?name", Context))
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("load-file", node);

            Assert.AreEqual ("success1", node.LastChild.PreviousNode.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.PreviousNode.Name);
            Assert.AreEqual ("success2", node.LastChild.Value);
            Assert.AreEqual ("test2.txt", node.LastChild.Name);
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
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node (string.Empty, "te{0}1.txt")
                .Add (string.Empty, "st");
            Context.Raise ("load-file", node);

            Assert.AreEqual ("success", node.Value);
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
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node (string.Empty, Expression.Create ("/{0}?name", Context))
                .Add (string.Empty, "1")
                .Add ("test1.txt");
            Context.Raise ("load-file", node);

            Assert.AreEqual ("success", node.Value);
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
            var node = new Node (string.Empty, "test1.hl")
                .Add ("src", @"foo1:bar1
foo2:bar2");
            Context.Raise ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node ("load-file", "test1.hl");
            Context.Raise ("load-file", node);

            Assert.AreEqual ("foo1", node [0].Name);
            Assert.AreEqual ("bar1", node [0].Value);
            Assert.AreEqual ("foo2", node [1].Name);
            Assert.AreEqual ("bar2", node [1].Value);
        }
    }
}