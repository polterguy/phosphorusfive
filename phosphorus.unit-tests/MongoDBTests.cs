
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using MongoDB.Bson;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class MongoDBTests
    {
        /*
         * runs before every unit test, deletes all documents from "unit_tests"
         */
        [SetUp]
        public void SetUp ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.mongodb");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.mongo.delete:unit_tests
pf.mongo.delete:unit_tests2
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
        }

        [Test]
        public void SimpleInsertAndSelect ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.mongodb");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.mongo.insert
  unit_tests
    simple_string:howdy world
pf.mongo.select:unit_tests
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [0] [0].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.AreEqual ("result", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("unit_tests", tmp [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [1] [0].Count, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [1] [0] [0].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [1] [0] [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_string", tmp [1] [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy world", tmp [1] [0] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SimpleInsertAndSelectWithTypes ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.mongodb");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.mongo.insert
  unit_tests
    simple_string:howdy world
    simple_integer:int:54
    simple_objectid:objectid:52d89c60f54c910a236aa14d
    simple_long:long:9223372036854775807
    simple_hl:node:""_exe\n  _foo""
    simple_guid:guid:e2a9a534-2ccf-4f65-bc48-1afe2667bc78
    simple_short:short:1234
    simple_single:float:1234.54
    simple_double:double:12332234.544322
    simple_bool:bool:true
    simple_byte:byte:255
    simple_date:date:2015-01-20
pf.mongo.select:unit_tests
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [0] [0].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.AreEqual ("result", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("unit_tests", tmp [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [1] [0].Count, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [1] [0] [0].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.AreEqual (12, tmp [1] [0] [0].Count, "wrong value of node after executing lambda object");

            Assert.AreEqual ("simple_string", tmp [1] [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy world", tmp [1] [0] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_integer", tmp [1] [0] [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (54, tmp [1] [0] [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_objectid", tmp [1] [0] [0] [2].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (ObjectId.Parse ("52d89c60f54c910a236aa14d"), tmp [1] [0] [0] [2].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_long", tmp [1] [0] [0] [3].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (9223372036854775807L, tmp [1] [0] [0] [3].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_hl", tmp [1] [0] [0] [4].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("_exe\r\n  _foo", tmp [1] [0] [0] [4].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_guid", tmp [1] [0] [0] [5].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (Guid.Parse ("e2a9a534-2ccf-4f65-bc48-1afe2667bc78"), tmp [1] [0] [0] [5].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_short", tmp [1] [0] [0] [6].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (1234, tmp [1] [0] [0] [6].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_single", tmp [1] [0] [0] [7].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (1234.54F, tmp [1] [0] [0] [7].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_double", tmp [1] [0] [0] [8].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (12332234.544322D, tmp [1] [0] [0] [8].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_bool", tmp [1] [0] [0] [9].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (true, tmp [1] [0] [0] [9].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_byte", tmp [1] [0] [0] [10].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (255, tmp [1] [0] [0] [10].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_date", tmp [1] [0] [0] [11].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (new DateTime (2015, 1, 20).ToUniversalTime (), tmp [1] [0] [0] [11].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertMultipleTablesInOneInsertStatement ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.mongodb");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.mongo.insert
  unit_tests
    simple_string:howdy world
  unit_tests2
    simple_string2:howdy world2
pf.mongo.select:unit_tests
pf.mongo.select:unit_tests2
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.IsTrue (tmp [0] [0].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [0] [1].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.AreEqual ("result", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("result", tmp [2] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("unit_tests", tmp [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("unit_tests2", tmp [2] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [1] [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [2] [0].Count, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [1] [0] [0].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [2] [0] [0].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [1] [0] [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [2] [0] [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_string", tmp [1] [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_string2", tmp [2] [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy world", tmp [1] [0] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy world2", tmp [2] [0] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectWithCriteria ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.mongodb");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.mongo.insert
  unit_tests
    simple_string:howdy world
  unit_tests
    simple_string:howdy world2
pf.mongo.select:unit_tests
  where
    simple_string:howdy world
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [1] [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_string", tmp [1] [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy world", tmp [1] [1] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectWithCriteriaInt ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.mongodb");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.mongo.insert
  unit_tests
    value:int:5
  unit_tests
    value:int:6
pf.mongo.select:unit_tests
  where
    value:int:5
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [1] [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("value", tmp [1] [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (5, tmp [1] [1] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectWithCriteriaNoMatch ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.mongodb");
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();
            Node tmp = new Node ();
            tmp.Value = @"
pf.mongo.insert
  unit_tests
    value:int:5
  unit_tests
    value:int:6
pf.mongo.select:unit_tests
  where
    value:int:7
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual (1, tmp [1].Count, "wrong value of node after executing lambda object");
        }
    }
}
