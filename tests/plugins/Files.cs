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
        ///     verifies [p5.file.exists] works correctly
        /// </summary>
        [Test]
        public void Exists ()
        {
            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("p5.file.save", node);

            // checking to see if file exists
            node = new Node (string.Empty, "test1.txt");
            Context.Raise ("p5.file.exists", node);

            // verifying [p5.file.exists] returned valid values
            Assert.AreEqual ("test1.txt", node [0].Name);
            Assert.AreEqual (true, node [0].Value);
        }

        /// <summary>
        ///     verifies [p5.file.exists] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression1 ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 1");
            Context.Raise ("p5.file.save", node);
            node = new Node (string.Empty, "test2.txt")
                .Add ("src", "this is test 2");
            Context.Raise ("p5.file.save", node);

            // checking to see if files exists
            node = new Node (string.Empty, Expression.Create ("/*?name", Context))
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("p5.file.exists", node);

            // verifying [p5.file.exists] returned valid values
            Assert.AreEqual ("test1.txt", node [2].Name);
            Assert.AreEqual (true, node [2].Value);
            Assert.AreEqual ("test2.txt", node [3].Name);
            Assert.AreEqual (true, node [3].Value);
        }

        /// <summary>
        ///     verifies [p5.file.exists] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression2 ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 1");
            Context.Raise ("p5.file.save", node);
            node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 2");
            Context.Raise ("p5.file.save", node);

            // checking to see if files exists
            node = new Node (string.Empty, Expression.Create ("/*!/0?{0}", Context))
                .Add (string.Empty, "name")
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("p5.file.exists", node);

            // verifying [p5.file.exists] returned valid values
            Assert.AreEqual ("test1.txt", node [3].Name);
            Assert.AreEqual (true, node [3].Value);
            Assert.AreEqual ("test2.txt", node [4].Name);
            Assert.AreEqual (true, node [4].Value);
        }

        /// <summary>
        ///     verifies [p5.file.exists] works correctly
        /// </summary>
        [Test]
        public void ExistsExpression3 ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("p5.file.save", node);

            // checking to see if files exists
            node = new Node (string.Empty, "te{0}.txt")
                .Add (string.Empty, "st1");
            Context.Raise ("p5.file.exists", node);

            // verifying [p5.file.exists] returned valid values
            Assert.AreEqual ("test1.txt", node [1].Name);
            Assert.AreEqual (true, node [1].Value);
        }

        /// <summary>
        ///     verifies [p5.file.remove] works correctly
        /// </summary>
        [Test]
        public void Remove ()
        {
            // creating file using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("p5.file.save", node);

            // removing file using Phosphorus Five
            node = new Node (string.Empty, "test1.txt");
            Context.Raise ("p5.file.remove", node);

            // verifying removal of file was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
        }

        /// <summary>
        ///     verifies [p5.file.remove] works correctly when given an expression
        /// </summary>
        [Test]
        public void RemoveExpression1 ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 1");
            Context.Raise ("p5.file.save", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("src", "this is test 2");
            Context.Raise ("p5.file.save", node);

            // removing files using Phosphorus Five
            node = new Node (string.Empty, Expression.Create ("/0|/1?name", Context))
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("p5.file.remove", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test2.txt"));
        }

        /// <summary>
        ///     verifies [p5.file.remove] works correctly when given an expression
        /// </summary>
        [Test]
        public void RemoveExpression2 ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is test 1");
            Context.Raise ("p5.file.save", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("src", "this is test 2");
            Context.Raise ("p5.file.save", node);

            // removing files using Phosphorus Five
            node = new Node (string.Empty, Expression.Create ("/1|{0}?name", Context))
                .Add (string.Empty, "/2")
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("p5.file.remove", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test2.txt"));
        }

        /// <summary>
        ///     verifies [p5.file.remove] works correctly when given an expression
        /// </summary>
        [Test]
        public void RemoveExpression3 ()
        {
            // creating files using p5.file
            var node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("p5.file.save", node);

            // removing files using Phosphorus Five
            node = new Node (string.Empty, "te{0}")
                .Add (string.Empty, "st1.txt");
            Context.Raise ("p5.file.remove", node);

            // verifying removal of files was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"));
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly
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
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly when given an expression, leading to
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
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly when given a formatted expression as path, leading
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
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly when given a formatted expression, which is
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
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly when given an expression, where [source] is an expression,
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
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly when given an expression, where [source] is an expression,
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
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly when given a formatted expression as path,
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
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly when given a constant as file name, and [source] is
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
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("success", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     making sure [p5.file.save] works when given a constant as a filepath, and an expression as
        ///     a [source], where the expression is of type 'node', and points to multiple nodes
        /// </summary>
        [Test]
        public void SaveExpression11 ()
        {
            ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
p5.file.save:test1.txt
  src:x:/../*/_data/*");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("foo1:bar1\r\nfoo2:bar2", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     making sure [p5.file.save] works when given a constant as a filepath, and an expression as
        ///     a [source], where the expression is of type 'node', and points to one node
        /// </summary>
        [Test]
        public void SaveExpression12 ()
        {
            ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
p5.file.save:test1.txt
  src:x:/../*/_data");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("_data\r\n  foo1:bar1\r\n  foo2:bar2", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.save] works correctly when overwriting an existing file
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
            Context.Raise ("p5.file.save", node);

            node = new Node (string.Empty, "test1.txt")
                .Add ("src", "this is a test");
            Context.Raise ("p5.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"));
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     verifies [p5.file.load] works correctly
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
            Context.Raise ("p5.file.save", node);

            // loading file using Phosphorus Five
            node = new Node (string.Empty, "test1.txt");
            Context.Raise ("p5.file.load", node);

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }

        /// <summary>
        ///     verifies [p5.file.load] works correctly when given an expression
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
            Context.Raise ("p5.file.save", node);

            node = new Node (string.Empty, "test2.txt")
                .Add ("src", "success2");
            Context.Raise ("p5.file.save", node);

            // loading file using Phosphorus Five
            node = new Node (string.Empty, Expression.Create ("/0|/1?name", Context))
                .Add ("test1.txt")
                .Add ("test2.txt");
            Context.Raise ("p5.file.load", node);

            Assert.AreEqual ("success1", node.LastChild.PreviousNode.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.PreviousNode.Name);
            Assert.AreEqual ("success2", node.LastChild.Value);
            Assert.AreEqual ("test2.txt", node.LastChild.Name);
        }

        /// <summary>
        ///     verifies [p5.file.load] works correctly when given an expression
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
            Context.Raise ("p5.file.save", node);

            // loading file using Phosphorus Five
            node = new Node (string.Empty, "te{0}1.txt")
                .Add (string.Empty, "st");
            Context.Raise ("p5.file.load", node);

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }

        /// <summary>
        ///     verifies [p5.file.load] works correctly when given an expression
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
            Context.Raise ("p5.file.save", node);

            // loading file using Phosphorus Five
            node = new Node (string.Empty, Expression.Create ("/{0}?name", Context))
                .Add (string.Empty, "1")
                .Add ("test1.txt");
            Context.Raise ("p5.file.load", node);

            Assert.AreEqual ("success", node.LastChild.Value);
            Assert.AreEqual ("test1.txt", node.LastChild.Name);
        }
    }
}