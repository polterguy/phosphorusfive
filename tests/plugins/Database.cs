/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using p5.core;

namespace p5.unittests.plugins
{
    /// <summary>
    ///     [p5.data.xxx] unit tests
    /// </summary>
    [TestFixture]
    public class Database : TestBase
    {
        public Database ()
            : base ("p5.types", "p5.hyperlisp", "p5.lambda", "p5.io", "p5.data")
        { }

        /*
         * runs before every unit test, deletes all documents from "unit_tests"
         */

        [SetUp]
        public void SetUp ()
        {
            // deleting entire database, in case there already are items in it
            ExecuteLambda (@"delete-data:x:/*/*");
        }

        /*
         * necessary to return "root folder" of executing Assembly
         */

        [ActiveEvent (Name = "p5.core.application-folder")]
        private static void GetRootFolder (ApplicationContext context, ActiveEventArgs e) { e.Args.Value = GetBasePath (); }

        /// <summary>
        ///     selects non-existing objects from database, making sure nothing is returned
        /// </summary>
        [Test]
        public void Select01 ()
        {
            var tmp = ExecuteLambda (@"select-data:x:/*/*/_mumbo_field/=jumbo_value");
            Assert.AreEqual (0, tmp [0].Count);
        }

        /// <summary>
        ///     inserts node, then selects, for then to delete and select again, making
        ///     sure both insert, select and delete works as it should
        /// </summary>
        [Test]
        public void Select02 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test1
    howdy:world
select-data:x:/*/*/_test1
delete-data:x:/*/*/_test1
select-data:x:/*/*/_test1");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual ("_test1", tmp [1] [0].Name);
            Assert.AreEqual (typeof (Guid), tmp [1] [0].Value.GetType ());
            Assert.AreEqual ("howdy", tmp [1] [0] [0].Name);
            Assert.AreEqual ("world", tmp [1] [0] [0].Value);
            Assert.AreEqual (0, tmp [3].Count);
        }

        /// <summary>
        ///     inserts into database for then to select 'name', to verify select works as it should
        /// </summary>
        [Test]
        public void Select03 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy:world
select-data:x:/*/*/_testX/0?name");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.AreEqual ("howdy", tmp [1] [0].Value);
        }

        /// <summary>
        ///     inserts into database for then to select 'value', to verify select works as it should
        /// </summary>
        [Test]
        public void Select04 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy:world
select-data:x:/*/*/_testX/0?value");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.AreEqual ("world", tmp [1] [0].Value);
        }

        /// <summary>
        ///     inserts into database, for then to select 'count' to verify select works as it should
        /// </summary>
        [Test]
        public void Select05 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy:world
select-data:x:/*/*/_testX/0?count");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.AreEqual (1, tmp [1] [0].Value);
        }

        /// <summary>
        ///     inserts into database, for then to select 'path' to verify select works as it should
        /// </summary>
        [Test]
        public void Select06 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy:world
select-data:x:/*/*/_testX/0?path");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.IsTrue (tmp [1] [0].Value is Node.Dna);
        }

        /// <summary>
        ///     inserts multiple objects into database, for then to select 'name', having multiple return values, to
        ///     verify select and insert works as it should
        /// </summary>
        [Test]
        public void Select07 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy1:world1
  _testX
    howdy2:world2
select-data:x:/*/*/_testX/0?name");
            Assert.AreEqual (2, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.AreEqual ("howdy1", tmp [1] [0].Value);
            Assert.AreEqual (string.Empty, tmp [1] [1].Name);
            Assert.AreEqual ("howdy2", tmp [1] [1].Value);
        }

        /// <summary>
        ///     inserts multiple objects into database, for then to select 'value', having multiple return values, to
        ///     verify select and insert works as it should
        /// </summary>
        [Test]
        public void Select08 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy1:world1
  _testX
    howdy2:world2
select-data:x:/*/*/_testX/0?value");
            Assert.AreEqual (2, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.AreEqual ("world1", tmp [1] [0].Value);
            Assert.AreEqual (string.Empty, tmp [1] [1].Name);
            Assert.AreEqual ("world2", tmp [1] [1].Value);
        }

        /// <summary>
        ///     inserts multiple objects into database, for then to select 'path', having multiple return values, to
        ///     verify select and insert works as it should
        /// </summary>
        [Test]
        public void Select09 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy1:world1
  _testX
    howdy2:world2
select-data:x:/*/*/_testX/0?path");
            Assert.AreEqual (2, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.AreEqual (string.Empty, tmp [1] [1].Name);
            Assert.IsTrue (tmp [1] [0].Value is Node.Dna);
            Assert.IsTrue (tmp [1] [1].Value is Node.Dna);
        }

        /// <summary>
        ///     inserts multiple objects into database, for then to select 'count', having multiple return values, to
        ///     verify select and insert works as it should
        /// </summary>
        [Test]
        public void Select10 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy1:world1
  _testX
    howdy2:world2
select-data:x:/*/*/_testX?count");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.AreEqual (2, tmp [1] [0].Value);
        }

        /// <summary>
        ///     inserts a couple of nodes, for then to select 'deep' from database, making
        ///     sure select and insert works as it should
        /// </summary>
        [Test]
        public void Select11 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test4
    howdy:world
  _test4
    howdy:world2
    query_field
      x:y
  _test4
    howdy:world3
select-data:x:/*/*/_test4/*/query_field/*/x/=y/./.");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual ("_test4", tmp [1] [0].Name);
            Assert.AreEqual ("world2", tmp [1] [0] [0].Value);
            Assert.AreEqual ("x", tmp [1] [0] [1] [0].Name);
            Assert.AreEqual ("y", tmp [1] [0] [1] [0].Value);
        }

        /// <summary>
        ///     inserts one node, for then to select the node, using a formatting expression, containing
        ///     an expression as one of its formatting parameters
        /// </summary>
        [Test]
        public void Select12 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test4
    howdy:world
select-data:x:/*/*/{0}
  :x:/../0/0?name");
            Assert.AreEqual (2, tmp [1].Count);
            Assert.AreEqual ("_test4", tmp [1] [1].Name);
            Assert.AreEqual ("howdy", tmp [1] [1] [0].Name);
            Assert.AreEqual ("world", tmp [1] [1] [0].Value);
        }
        
        /// <summary>
        ///     inserts into database, for then to select 'count' to verify select works as it should
        ///     when counting items that there are zero matches of
        /// </summary>
        [Test]
        public void Select13 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
    howdy:world
select-data:x:/*/*/_testY?count");
            Assert.AreEqual (0, tmp [1] [0].Value);
        }

        /// <summary>
        ///     inserts one node, for then to delete the node, using a formatting expression,
        ///     where one of the formatting parameters are an expression in itself
        /// </summary>
        [Test]
        public void Delete01 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test4
    howdy:world
delete-data:x:/*/*/{0}
  :x:/../0/0?name
select-data:x:/*/*/_test4");
            Assert.AreEqual (0, tmp [2].Count);
        }

        /// <summary>
        ///     inserts from an expression source, making sure insert can handle expressions
        /// </summary>
        [Test]
        public void Insert01 ()
        {
            var tmp = ExecuteLambda (@"insert-data:x:/+
_testX
  howdy:world
select-data:x:/*/*/_testX");
            Assert.AreEqual (1, tmp [2].Count);
            Assert.AreEqual ("_testX", tmp [2] [0].Name);
            Assert.AreEqual (typeof (Guid), tmp [2] [0].Value.GetType ());
            Assert.AreEqual ("howdy", tmp [2] [0] [0].Name);
            Assert.AreEqual ("world", tmp [2] [0] [0].Value);
        }

        /// <summary>
        ///     inserts multiple items from an expression source, making sure insert works as it should
        /// </summary>
        [Test]
        public void Insert02 ()
        {
            var tmp = ExecuteLambda (@"insert-data:x:/+|/+/+
_testX
  howdy:world
_testX
  howdy:world
select-data:x:/*/*/_testX");
            Assert.AreEqual (2, tmp [3].Count);
            Assert.AreEqual ("_testX", tmp [3] [0].Name);
            Assert.AreEqual (typeof (Guid), tmp [3] [0].Value.GetType ());
            Assert.AreEqual ("howdy", tmp [3] [0] [0].Name);
            Assert.AreEqual ("world", tmp [3] [0] [0].Value);
            Assert.AreEqual ("_testX", tmp [3] [1].Name);
            Assert.AreEqual (typeof (Guid), tmp [3] [1].Value.GetType ());
            Assert.AreEqual ("howdy", tmp [3] [1] [0].Name);
            Assert.AreEqual ("world", tmp [3] [1] [0].Value);
        }

        /// <summary>
        ///     inserts one item into database where item is "string" type, making sure insert can corectly convert
        ///     from string to Node
        /// </summary>
        [Test]
        public void Insert03 ()
        {
            var tmp = ExecuteLambda (@"insert-data:node:@""_testX
  howdy:world""
select-data:x:/*/*/_testX");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual ("_testX", tmp [1] [0].Name);
            Assert.AreEqual (typeof (Guid), tmp [1] [0].Value.GetType ());
            Assert.AreEqual ("howdy", tmp [1] [0] [0].Name);
            Assert.AreEqual ("world", tmp [1] [0] [0].Value);
        }

        /// <summary>
        ///     inserts two items into database, where items are "strings", making sure insert can corectly convert
        ///     from string to Node(s)
        /// </summary>
        [Test]
        public void Insert04 ()
        {
            var tmp = ExecuteLambda (@"insert-data:@""_testX
  howdy:world
_testX
  howdy:world""
select-data:x:/*/*/_testX");
            Assert.AreEqual (2, tmp [1].Count);
            Assert.AreEqual ("_testX", tmp [1] [0].Name);
            Assert.AreEqual (typeof (Guid), tmp [1] [0].Value.GetType ());
            Assert.AreEqual ("howdy", tmp [1] [0] [0].Name);
            Assert.AreEqual ("world", tmp [1] [0] [0].Value);
        }

        /// <summary>
        ///     inserts an item into the database which is nothing but a "simple value" type of item
        ///     from string to Node
        /// </summary>
        [Test]
        public void Insert05 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  foo:bar
select-data:x:/*/*/foo?value");
            Assert.AreEqual (1, tmp [1].Count);
            Assert.AreEqual (string.Empty, tmp [1] [0].Name);
            Assert.AreEqual ("bar", tmp [1] [0].Value);
        }

        /// <summary>
        ///     inserts two items with the same ID, expecting an exception
        /// </summary>
        [Test]
        [ExpectedException]
        public void InsertError01 ()
        {
            ExecuteLambda (@"insert-data
  foo1:bar_x
  foo2:bar_x");
        }

        /// <summary>
        ///     inserts a couple of items into database, for then to perform a 'deep' update, making
        ///     sure update works as it should
        /// </summary>
        [Test]
        public void Update01 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test5
    howdy:world
  _test5
    howdy:world2
    query_field
      x:y
  _test5
    howdy:world3
update-data:x:/*/*/_test5/*/query_field
  src
    query_field2
      x:zz
select-data:x:/*/*/_test5/*/query_field2/*/x/=zz/./.");
            Assert.AreEqual (1, tmp [2].Count);
            Assert.AreEqual ("_test5", tmp [2] [0].Name);
            Assert.AreEqual ("world2", tmp [2] [0] [0].Value);
            Assert.AreEqual ("x", tmp [2] [0] [1] [0].Name);
            Assert.AreEqual ("zz", tmp [2] [0] [1] [0].Value);
        }

        /// <summary>
        ///     inserts some items into database, for then to update multiple items at the same time,
        ///     making sure update works as it should
        /// </summary>
        [Test]
        public void Update02 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test6
    howdy:world
  _test6
    howdy:world2
  _test6
    howdy:world3
update-data:x:/*/*/_test6
  src
    _test6_update
      howdy:worldZZ
select-data:x:/*/*/_test6_update
select-data:x:/*/*/_test6");
            Assert.AreEqual (3, tmp [2].Count);
            Assert.AreEqual ("_test6_update", tmp [2] [0].Name);
            Assert.AreEqual ("worldZZ", tmp [2] [0] [0].Value);
            Assert.AreEqual ("worldZZ", tmp [2] [1] [0].Value);
            Assert.AreEqual ("worldZZ", tmp [2] [2] [0].Value);
            Assert.AreEqual (0, tmp [3].Count);
        }

        /// <summary>
        ///     inserts one item into database, for then to update through 'value' expression, making sure update works
        ///     as it should
        /// </summary>
        [Test]
        public void Update03 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _testX
update-data:x:/*/*/_testX?value
  src:{0}{1}
    :hello
    :world
select-data:x:/*/*/_testX");
            Assert.AreEqual (1, tmp [2].Count);
            Assert.AreEqual ("_testX", tmp [2] [0].Name);
            Assert.AreEqual ("helloworld", tmp [2] [0].Value);
        }

        /// <summary>
        ///     inserts one item into database, for then to update root item through use of expressions,
        ///     making sure update works as it should
        /// </summary>
        [Test]
        public void Update04 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test7
    howdy:world
update-data:x:/*/*/_test7
  src:x:/./+
_test7_update
  howdy2:world2
select-data:x:/*/*/_test7_update
select-data:x:/*/*/_test7");
            Assert.AreEqual (1, tmp [3].Count);
            Assert.AreEqual ("_test7_update", tmp [3] [0].Name);
            Assert.AreEqual ("howdy2", tmp [3] [0] [0].Name);
            Assert.AreEqual ("world2", tmp [3] [0] [0].Value);
            Assert.AreEqual (0, tmp [4].Count);
        }

        /// <summary>
        ///     inserts an item into database, for them to update 'value' of item to become a node, through an expression,
        ///     making sure values in database can store nodes as their values
        /// </summary>
        [Test]
        public void Update05 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test9
    howdy:world
update-data:x:/*/*/_test9/*/howdy?value
  src:x:/./+
_howdy:world
select-data:x:/*/*/_test9");
            Assert.AreEqual (1, tmp [3].Count);
            Assert.AreEqual ("_test9", tmp [3] [0].Name);
            Assert.AreEqual ("howdy", tmp [3] [0] [0].Name);
            Assert.AreEqual ("_howdy", tmp [3] [0] [0].Get<Node> (Context).Name);
            Assert.AreEqual ("world", tmp [3] [0] [0].Get<Node> (Context).Value);
            Assert.AreEqual (0, tmp [3] [0] [0].Get<Node> (Context).Count);
        }

        /// <summary>
        ///     inserts an item into database, for them to update 'value' using [rel-source], making sure update works as
        ///     it should
        /// </summary>
        [Test]
        public void Update06 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test9
    howdy:world
update-data:x:/*/*/_test9/*/howdy?value
  rel-src:x:/.?name
select-data:x:/*/*/_test9");
            Assert.AreEqual (1, tmp [2].Count);
            Assert.AreEqual ("_test9", tmp [2] [0].Name);
            Assert.AreEqual ("howdy", tmp [2] [0] [0].Name);
            Assert.AreEqual ("_test9", tmp [2] [0] [0].Value);
        }

        /// <summary>
        ///     inserts an item into database, for them to update 'value' using [rel-source], where [rel-source]
        ///     is a formatting expression, making sure update works as it should
        /// </summary>
        [Test]
        public void Update07 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test9
    howdy:world
update-data:x:/*/*/_test9/*/howdy?value
  rel-src:{0}{1}
    :x:/.?name
    :x:?value
select-data:x:/*/*/_test9");
            Assert.AreEqual (1, tmp [2].Count);
            Assert.AreEqual ("_test9", tmp [2] [0].Name);
            Assert.AreEqual ("howdy", tmp [2] [0] [0].Name);
            Assert.AreEqual ("_test9world", tmp [2] [0] [0].Value);
        }

        /// <summary>
        ///     inserts an item into database, for them to update 'value' using [rel-source], where [rel-source]
        ///     points to multiple node's values
        /// </summary>
        [Test]
        public void Update08 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  _test9
    _1:""howdy ""
    _2:world
    _dest
update-data:x:/*/*/_test9/*/_dest?value
  rel-src:x:/./*(/_1|/_2)?value
select-data:x:/*/*/_test9");
            Assert.AreEqual (1, tmp [2].Count);
            Assert.AreEqual ("_test9", tmp [2] [0].Name);
            Assert.AreEqual ("howdy ", tmp [2] [0] [0].Value);
            Assert.AreEqual ("world", tmp [2] [0] [1].Value);
            Assert.AreEqual ("_dest", tmp [2] [0] [2].Name);
            Assert.AreEqual ("howdy world", tmp [2] [0] [2].Value);
        }

        /// <summary>
        ///     inserts an item into database, for them to update item, making sure the item gets a new ID when one
        ///     is explicitly given
        /// </summary>
        [Test]
        public void Update09 ()
        {
            var tmp = ExecuteLambda (@"insert-data
  howdy
update-data:x:/*/*/howdy
  src
    howdy2:foo
select-data:x:/*/*/howdy2");
            Assert.AreEqual (typeof (Guid), tmp [0] [0].Value.GetType ());
            Assert.AreEqual ("foo", tmp [2] [0].Value);
        }

        /// <summary>
        ///     tries to insert one item into database with no name
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError01 () { ExecuteLambda (@"insert-data
  :bar1"); }

        /// <summary>
        ///     tries to update an item without submitting a [source] or [rel-source]
        /// </summary>
        [Test]
        [ExpectedException]
        public void SyntaxError02 ()
        {
            ExecuteLambda (@"insert-data
  foo1:bar1");
            ExecuteLambda (@"update-data:@/*/*/foo1/?value
  bar2");
        }
    }
}