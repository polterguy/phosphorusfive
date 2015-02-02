
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class DatabaseTests : TestBase
    {
        public DatabaseTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.types");
            Loader.Instance.LoadAssembly ("phosphorus.unit-tests");
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
            Loader.Instance.LoadAssembly ("phosphorus.data");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        /*
         * runs before every unit test, deletes all documents from "unit_tests"
         */
        [SetUp]
        public void SetUp ()
        {
            // deleting entire database, in case there already are items in it
            ExecuteLambda (@"pf.data.delete:@/*/*/?node");
        }

        [ActiveEvent (Name = "pf.core.application-folder")]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e)
        {
            string asmPath = Assembly.GetExecutingAssembly ().Location;
            asmPath = asmPath.Substring (0, asmPath.LastIndexOf ("/") + 1);
            e.Args.Value = asmPath;
        }

        [Test]
        public void SelectNonExisting ()
        {
            Node tmp = ExecuteLambda (@"pf.data.select:@/*/*/_mumbo_field/=jumbo_value/?node");
            Assert.AreEqual (0, tmp [0].Count, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InsertSelectAndDelete ()
        {
            Node tmp = ExecuteLambda (@"pf.data.insert
  _test1
    howdy:world
pf.data.select:@/*/*/_test1/?node
pf.data.delete:@/*/*/_test1/?node
pf.data.select:@/*/*/_test1/?node");
            Assert.AreEqual (1, tmp [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test1", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [1] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [3].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectSingleName ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
    howdy:world
pf.data.select:@/*/*/_testX/0/?name");
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectSingleValue ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
    howdy:world
pf.data.select:@/*/*/_testX/0/?value");
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectSingleCount ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
    howdy:world
pf.data.select:@/*/*/_testX/0/?count");
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectSinglePath ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
    howdy:world
pf.data.select:@/*/*/_testX/0/?path");
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [2] [0].Value is Node.DNA, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectMultipleName ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
    howdy1:world1
  _testX
    howdy2:world2
pf.data.select:@/*/*/_testX/0/?name");
            Assert.AreEqual (2, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy1", tmp [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy2", tmp [2] [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SelectMultipleValue ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
    howdy1:world1
  _testX
    howdy2:world2
pf.data.select:@/*/*/_testX/0/?value");
            Assert.AreEqual (2, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world1", tmp [2] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [2] [1].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SelectMultiplePath ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
    howdy1:world1
  _testX
    howdy2:world2
pf.data.select:@/*/*/_testX/0/?path");
            Assert.AreEqual (2, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [1].Name, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [2] [0].Value is Node.DNA, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [2] [1].Value is Node.DNA, "wrong value of node after executing lambda object");
        }

        [Test]
        public void SelectMultipleCount ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
    howdy1:world1
  _testX
    howdy2:world2
pf.data.select:@/*/*/_testX/?count");
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (string.Empty, tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (2, tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertFromExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert:@/+/?node
_testX
  howdy:world
pf.data.select:@/*/*/_testX/?node");
            Assert.AreEqual (1, tmp [3].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_testX", tmp [3] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [3] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [3] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [3] [0] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InsertMultipleFromExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert:@/+/|/+/+/?node
_testX
  howdy:world
_testX
  howdy:world
pf.data.select:@/*/*/_testX/?node");
            Assert.AreEqual (2, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_testX", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [4] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [4] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [4] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_testX", tmp [4] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [4] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [4] [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [4] [1] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void InsertAndCount ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test2/?node
pf.data.insert
  _test2
    howdy:world
pf.data.select:@/*/*/_test2/?count");
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertSelectAndCountMultipleNodes ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test3/?node
pf.data.insert
  _test3
    howdy:world
  _test3
    howdy:world2
  _test3
    howdy:world3
pf.data.select:@/*/*/_test3/?count
pf.data.select:@/*/*/_test3/?node");
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
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test4/?node
pf.data.insert
  _test4
    howdy:world
  _test4
    howdy:world2
    query_field
      x:y
  _test4
    howdy:world3
pf.data.select:@/*/*/_test4/*/query_field/*/x/=y/././?node");
            Assert.AreEqual (1, tmp [2].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test4", tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [2] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [2] [0] [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("y", tmp [2] [0] [1] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateSingleNode ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test5/?node
pf.data.insert
  _test5
    howdy:world
  _test5
    howdy:world2
    query_field
      x:y
  _test5
    howdy:world3
pf.data.update:@/*/*/_test5/*/query_field/?node
  query_field2
    x:zz
pf.data.select:@/*/*/_test5/*/query_field2/*/x/=zz/././?node");
            Assert.AreEqual (1, tmp [3].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test5", tmp [3] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [3] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("x", tmp [3] [0] [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("zz", tmp [3] [0] [1] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateMultipleNodes ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test6/?node
pf.data.delete:@/*/*/_test6_update/?node
pf.data.insert
  _test6
    howdy:world
  _test6
    howdy:world2
  _test6
    howdy:world3
pf.data.update:@/*/*/_test6/?node
  _test6_update
    howdy:worldZZ
pf.data.select:@/*/*/_test6_update/?node
pf.data.select:@/*/*/_test6/?node");
            Assert.AreEqual (3, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [5].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test6_update", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("worldZZ", tmp [4] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("worldZZ", tmp [4] [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("worldZZ", tmp [4] [2] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateFromFormattedExpressionSource ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testX/?node
pf.data.insert
  _testX
pf.data.update:@/*/*/_testX/?value
  :{0}{1}
    :hello
    :world
pf.data.select:@/*/*/_testX/?node");
            Assert.AreEqual (1, tmp [3].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_testX", tmp [3] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("helloworld", tmp [3] [0].Value, "wrong value of node after executing lambda object");
        }

        [Test]
        public void UpdateExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/(/_test7/|/_test7_update/)?node
pf.data.insert
  _test7
    howdy:world
pf.data.update:@/*/*/_test7/?node
  :@/./+/?node
_test7_update
  howdy2:world2
pf.data.select:@/*/*/_test7_update/?node
pf.data.select:@/*/*/_test7/?node");
            Assert.AreEqual (1, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [5].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test7_update", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy2", tmp [4] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [4] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateValueFromValueExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test8/?node
pf.data.insert
  _test8
    howdy:world
pf.data.update:@/*/*/_test8/*/howdy/?value
  :@/./+/?value
:world2
pf.data.select:@/*/*/_test8/?node");
            Assert.AreEqual (1, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test8", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [4] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateValueFromNameExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test8/?node
pf.data.insert
  _test8
    howdy:world
pf.data.update:@/*/*/_test8/*/howdy/?value
  :@/./+/?name
world2
pf.data.select:@/*/*/_test8/?node");
            Assert.AreEqual (1, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test8", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [4] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateNameFromValueExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test9/?node
pf.data.insert
  _test9
    howdy:world
pf.data.update:@/*/*/_test9/*/howdy/?name
  :@/./+/?value
:howdy2
pf.data.select:@/*/*/_test9/?node");
            Assert.AreEqual (1, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test9", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy2", tmp [4] [0] [0].Name, "wrong value of node after executing lambda object");
        }

        [Test]
        public void UpdateNameFromNameExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test9/?node
pf.data.insert
  _test9
    howdy:world
pf.data.update:@/*/*/_test9/*/howdy/?name
  :@/./+/?name
howdy2
pf.data.select:@/*/*/_test9/?node");
            Assert.AreEqual (1, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test9", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy2", tmp [4] [0] [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateNodeFromNodeExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test9/?node
pf.data.insert
  _test9
    howdy:world
pf.data.update:@/*/*/_test9/*/howdy/?node
  :@/./+/?node
howdy2:world2
pf.data.select:@/*/*/_test9/?node");
            Assert.AreEqual (1, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test9", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy2", tmp [4] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world2", tmp [4] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateValueFromCountExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test9/?node
pf.data.insert
  _test9
    howdy:world
pf.data.update:@/*/*/_test9/*/howdy/?value
  :@/../*/?count
pf.data.select:@/*/*/_test9/?node");
            Assert.AreEqual (1, tmp [3].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test9", tmp [3] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (4, tmp [3] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateNameFromCountExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test9/?node
pf.data.insert
  _test9
    howdy:world
pf.data.update:@/*/*/_test9/*/howdy/?name
  :@/../*/?count
pf.data.select:@/*/*/_test9/?node");
            Assert.AreEqual (1, tmp [3].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test9", tmp [3] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("4", tmp [3] [0] [0].Name, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void UpdateValueFromNodeExpression ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_test9/?node
pf.data.insert
  _test9
    howdy:world
pf.data.update:@/*/*/_test9/*/howdy/?value
  :@/./+/?node
_howdy:world
pf.data.select:@/*/*/_test9/?node");
            Assert.AreEqual (1, tmp [4].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_test9", tmp [4] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy", tmp [4] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_howdy", tmp [4] [0] [0].Get<Node> ().Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("world", tmp [4] [0] [0].Get<Node> ().Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (0, tmp [4] [0] [0].Get<Node> ().Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertVeryManyValues ()
        {
            Node tmp = ExecuteLambda (@"pf.data.delete:@/*/*/_testMany/?node
for-each:@/0/*/**/?node
  pf.data.insert
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
    _testMany
      howdy:world
pf.data.select:@/*/*/_testMany/?count");
            Assert.AreEqual (800, tmp [2] [0].Value, "wrong value of node after executing lambda object");
        }
    }
}
