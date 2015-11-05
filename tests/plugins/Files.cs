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
    ///     unit tests for testing the [p5.file.xxx] namespace
    /// </summary>
    [TestFixture]
    public class Files : TestBase
    {
        public Files ()
            : base ("p5.io", "p5.hyperlisp", "p5.lambda", "p5.types")
        { }

        /*
         * necessary to return "root folder" of executing Assembly
         */
        [ActiveEvent (Name = "p5.core.application-folder")]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = GetBasePath ();
        }

        /// <summary>
        ///     verifies [file-exist] works correctly
        /// </summary>
        [Test]
        public void Exists ()
        {
            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // checking to see if file exists
            node = new Node (string.Empty, "test1.txt");
            Context.Raise ("file-exist", node);

            // verifying [file-exist] returned valid values
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
        }

        /// <summary>
        ///     verifies [file-exist] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression1 ()
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
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
            Assert.AreEqual ("test2.txt", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
        }

        /// <summary>
        ///     verifies [file-exist] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression2 ()
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
        ///     verifies [file-exist] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression3 ()
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
        ///     verifies [delete-file] works correctly
        /// </summary>
        [Test]
        public void Remove ()
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
        }

        /// <summary>
        ///     verifies [delete-file] works correctly when given an expression
        /// </summary>
        [Test]
        public void RemoveExpression1 ()
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
        }

        /// <summary>
        ///     verifies [delete-file] works correctly when given an expression
        /// </summary>
        [Test]
        public void RemoveExpression2 ()
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
        }

        /// <summary>
        ///     verifies [delete-file] works correctly when given an expression
        /// </summary>
        [Test]
        public void RemoveExpression3 ()
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
        }

        /// <summary>
        ///     verifies [save-file] works correctly
        /// </summary>
        [Test]
        public void Save ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("save-file", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [save-file] works correctly when given an expression, leading to
        ///     multiple different files being saved at the same time, with the same static content
        /// </summary>
        [Test]
        public void SaveExpression01 ()
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
        ///     verifies [save-file] works correctly when given a formatted expression as path, leading
        ///     to multiple file names, where files contains same static content
        /// </summary>
        [Test]
        public void SaveExpression02 ()
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
        ///     verifies [save-file] works correctly when given a formatted expression, which is
        ///     not a p5.lambda expression, saving one file, with static content
        /// </summary>
        [Test]
        public void SaveExpression03 ()
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
        ///     verifies [save-file] works correctly when given an expression, where [source] is an expression,
        ///     pointing to one 'name'
        /// </summary>
        [Test]
        public void SaveExpression04 ()
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
        ///     verifies [save-file] works correctly when given an expression, where [source] is an expression,
        ///     pointing to two 'name' values
        /// </summary>
        [Test]
        public void SaveExpression05 ()
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
        ///     verifies [save-file] works correctly when given a formatted expression as path,
        ///     which is not a p5.lambda expression, and [source] is an expression, leading to one 'name'
        /// </summary>
        [Test]
        public void SaveExpression06 ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node (string.Empty, "te{0}1.txt")
                .Add (string.Empty, "st")
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
        ///     verifies [save-file] works correctly when given a constant as file name, and [source] is
        ///     a formatted p5.lambda expression, pointing to one 'name'
        /// </summary>
        [Test]
        public void SaveExpression07 ()
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
        ///     making sure [save-file] works when given a constant as a filepath, and an expression as
        ///     a [source], where the expression is of type 'node', and points to multiple nodes
        /// </summary>
        [Test]
        public void SaveExpression11 ()
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
        ///     making sure [save-file] works when given a constant as a filepath, and an expression as
        ///     a [source], where the expression is of type 'node', and points to one node
        /// </summary>
        [Test]
        public void SaveExpression12 ()
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
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void SaveActiveEvent01 ()
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
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
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
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void SaveActiveEvent03 ()
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
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void SaveActiveEvent04 ()
        {
            ExecuteLambda (@"save-file:test1.txt
  test.save.av4_1
  test.save.av4_2");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [save-file] works correctly when overwriting an existing file
        /// </summary>
        [Test]
        public void Overwrite ()
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
        ///     verifies [load-file] works correctly
        /// </summary>
        [Test]
        public void Load ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "success");
            Context.Raise ("save-file", node);

            // loading file using Phosphorus Five
            node = new Node (string.Empty, "test1.txt");
            Context.Raise ("load-file", node);

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }

        /// <summary>
        ///     verifies [load-file] works correctly when given an expression
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
        ///     verifies [load-file] works correctly when given an expression
        /// </summary>
        [Test]
        public void LoadExpression2 ()
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

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }

        /// <summary>
        ///     verifies [load-file] works correctly when given an expression
        /// </summary>
        [Test]
        public void LoadExpression3 ()
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

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }
        
        /// <summary>
        ///     verifies [load-file] works correctly when loading Hyperlisp
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

            Assert.AreEqual ("foo1", node.LastChild [0].Name);
            Assert.AreEqual ("bar1", node.LastChild [0].Value);
            Assert.AreEqual ("foo2", node.LastChild [1].Name);
            Assert.AreEqual ("bar2", node.LastChild [1].Value);
        }
    }
}