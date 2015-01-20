/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class FileTests
    {
        [Test]
        public void SaveAndLoadFile ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.save:mumbo.txt
  :""this is mumbo""
pf.file.load:mumbo.txt
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("this is mumbo", tmp [1] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void OverwriteLongFileWithShort ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.save:mumbo.txt
  :""this is mumbo, and this is a long text""
pf.file.save:mumbo.txt
  :""this is mumbo, shorter""
pf.file.load:mumbo.txt
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("this is mumbo, shorter", tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SaveAndRemoveFile ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.save:mumbo.txt
  :""this is mumbo""
pf.file.exists:mumbo.txt
pf.file.remove:mumbo.txt
pf.file.exists:mumbo.txt
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [3] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void CreateAndRemoveFolder ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.create-folder:mumbo
pf.file.folder-exists:mumbo
pf.file.remove-folder:mumbo
pf.file.folder-exists:mumbo
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (true, tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (false, tmp [3] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void ListFiles ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.create-folder:list-files
pf.file.save:list-files/mumbo1.txt
  :""mumbo1""
pf.file.save:list-files/mumbo2.txt
  :""mumbo2""
pf.file.save:list-files/mumbo3.txt
  :""mumbo3""
pf.file.list-files:list-files
pf.file.remove-folder:list-files
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (3, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-files/mumbo1.txt", tmp [4] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-files/mumbo2.txt", tmp [4] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-files/mumbo3.txt", tmp [4] [2].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void ListFolders ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.file.create-folder:list-folders
pf.file.create-folder:list-folders/mumbo1
pf.file.create-folder:list-folders/mumbo2
pf.file.create-folder:list-folders/mumbo3
pf.file.list-folders:list-folders
pf.file.remove-folder:list-folders
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (3, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-folders/mumbo1", tmp [4] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-folders/mumbo2", tmp [4] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("list-folders/mumbo3", tmp [4] [2].Value, "wrong value of node after executing lambda object");
        }
    }
}

