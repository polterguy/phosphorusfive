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
    public class DatabaseTests
    {
        [Test]
        public void SelectNonExisting ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            Loader.Instance.LoadAssembly ("phosphorus.data");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.data.select:@/*/*/_mumbo_field/=jumbo_value/?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (0, tmp [0].Count, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InsertSelectAndDelete ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            Loader.Instance.LoadAssembly ("phosphorus.data");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.data.insert
  _test1
    howdy:world
pf.data.select:@/*/*/_test1/?node
pf.data.delete:@/*/*/_test1/?node
pf.data.select:@/*/*/_test1/?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test1", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [1] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [3].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertAndCount ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            Loader.Instance.LoadAssembly ("phosphorus.data");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.data.delete:@/*/*/_test2/?node
pf.data.insert
  _test2
    howdy:world
pf.data.count:@/*/*/_test2/?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertSelectAndCountMultipleNodes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            Loader.Instance.LoadAssembly ("phosphorus.data");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.data.delete:@/*/*/_test3/?node
pf.data.insert
  _test3
    howdy:world
  _test3
    howdy:world2
  _test3
    howdy:world3
pf.data.count:@/*/*/_test3/?node
pf.data.select:@/*/*/_test3/?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (3, tmp [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (3, tmp [3].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test3", tmp [3] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [3] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [3] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test3", tmp [3] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [3] [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [3] [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test3", tmp [3] [2].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [3] [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world3", tmp [3] [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertAndSelectInnerNodes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            Loader.Instance.LoadAssembly ("phosphorus.data");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.data.delete:@/*/*/_test4/?node
pf.data.insert
  _test4
    howdy:world
  _test4
    howdy:world2
    query_field
      x:y
  _test4
    howdy:world3
pf.data.select:@/*/*/_test4/*/query_field/*/x/=y/././?node
";
            context.Raise ("pf.hyperlisp-2-nodes", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test4", tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [2] [0] [0].Value, "wrong value of node after executing lambda object");
        }
    }
}

