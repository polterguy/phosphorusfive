
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class Files
    {
        private ApplicationContext _context;

        public Files ()
        {
            Loader.Instance.LoadAssembly (this.GetType ());
            Loader.Instance.LoadAssembly ("phosphorus.file");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        private static string GetBasePath ()
        {
            string retVal = Assembly.GetExecutingAssembly ().Location;
            retVal = retVal.Substring (0, retVal.LastIndexOf ("/") + 1);
            return retVal;
        }

        [ActiveEvent (Name = "pf.core.application-folder")]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = GetBasePath ();
        }

        [Test]
        public void Save ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add (string.Empty, "this is a test");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }
        
        [Test]
        public void Overwrite ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }

            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add (string.Empty, "this is a LONGER test");
            _context.Raise ("pf.file.save", node);

            node = new Node (string.Empty, "test1.txt")
                .Add (string.Empty, "this is a test");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("this is a test", reader.ReadToEnd ());
            }
        }

        [Test]
        public void Remove ()
        {
            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add (string.Empty, "this is a test");
            _context.Raise ("pf.file.save", node);

            // removing file using phosphorus.five
            node = new Node (string.Empty, "test1.txt");
            _context.Raise ("pf.file.remove", node);

            // verifying removal of file was done correctly
            Assert.AreEqual (false, File.Exists (GetBasePath () + "test1.txt"), "file existed");
        }
        
        [Test]
        public void RemoveExpression ()
        {
            // creating files using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add (string.Empty, "this is a test");
            _context.Raise ("pf.file.save", node);

            node = new Node (string.Empty, "test2.txt")
                .Add (string.Empty, "this is a test");
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
                .Add (string.Empty, "this is a test");
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
                .Add (string.Empty, "@/-/?name");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("hello world", reader.ReadToEnd ());
            }
        }
        
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
                .Add (string.Empty, "@/-2/|/-1/?name");
            _context.Raise ("pf.file.save", node);

            // verifying creation of file was done correctly
            Assert.AreEqual (true, File.Exists (GetBasePath () + "test1.txt"), "file didn't exist");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual ("hello world 2.0", reader.ReadToEnd ());
            }
        }

        [Test]
        public void Load ()
        {
            // deleting file if it already exists
            if (File.Exists (GetBasePath () + "test1.txt")) {
                File.Delete (GetBasePath () + "test1.txt");
            }
            
            // creating file using phosphorus.file
            Node node = new Node (string.Empty, "test1.txt")
                .Add (string.Empty, "success");
            _context.Raise ("pf.file.save", node);

            // loading file using phosphorus.five
            node = new Node (string.Empty, "test1.txt");
            _context.Raise ("pf.file.load", node);

            Assert.AreEqual ("success", node.LastChild.Value);
        }
        
        [Test]
        public void LoadExpression ()
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
                .Add (string.Empty, "success1");
            _context.Raise ("pf.file.save", node);

            node = new Node (string.Empty, "test2.txt")
                .Add (string.Empty, "success2");
            _context.Raise ("pf.file.save", node);

            // loading file using phosphorus.five
            node = new Node (string.Empty, "@/0/|/1/?name")
                .Add ("test1.txt")
                .Add ("test2.txt");
            _context.Raise ("pf.file.load", node);

            Assert.AreEqual ("success1", node.LastChild.PreviousNode.Value);
            Assert.AreEqual ("success2", node.LastChild.Value);
        }
    }
}
