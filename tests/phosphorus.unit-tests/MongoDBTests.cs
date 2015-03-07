
/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using MongoDB.Bson;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class MongoDBTests : TestBase
    {
        public MongoDBTests ()
        {
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.types");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.mongodb");
            _context = Loader.Instance.CreateApplicationContext ();
        }

        [SetUp]
        public void SetUp ()
        {
            ExecuteLambda (@"pf.mongo.delete:unit_tests
pf.mongo.delete:unit_tests2");
        }

        [Test]
        public void SimpleInsertAndSelect ()
        {
            Node tmp = ExecuteLambda (@"pf.mongo.insert
  unit_tests
    simple_string:howdy world
pf.mongo.select:unit_tests");
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
            Node tmp = ExecuteLambda (@"pf.mongo.insert
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
pf.mongo.select:unit_tests");
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
            Node tmp = ExecuteLambda (@"pf.mongo.insert
  unit_tests
    simple_string:howdy world
  unit_tests2
    simple_string2:howdy world2
pf.mongo.select:unit_tests
pf.mongo.select:unit_tests2");
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
            Node tmp = ExecuteLambda (@"pf.mongo.insert
  unit_tests
    simple_string:howdy world
  unit_tests
    simple_string:howdy world2
pf.mongo.select:unit_tests
  where
    simple_string:howdy world");
            Assert.AreEqual (1, tmp [1] [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("simple_string", tmp [1] [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy world", tmp [1] [1] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectWithCriteriaInt ()
        {
            Node tmp = ExecuteLambda (@"pf.mongo.insert
  unit_tests
    value:int:5
  unit_tests
    value:int:6
pf.mongo.select:unit_tests
  where
    value:int:5");
            Assert.AreEqual (1, tmp [1] [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("value", tmp [1] [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (5, tmp [1] [1] [0] [0].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectWithCriteriaNoMatch ()
        {
            Node tmp = ExecuteLambda (@"pf.mongo.insert
  unit_tests
    value:int:5
  unit_tests
    value:int:6
pf.mongo.select:unit_tests
  where
    value:int:7");
            Assert.AreEqual (1, tmp [1].Count, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void SelectWithMultipleCriteria ()
        {
            Node tmp = ExecuteLambda (@"pf.mongo.insert
  unit_tests
    value:int:5
    name:john doe
  unit_tests
    value:int:6
    name:john doe
pf.mongo.select:unit_tests
  where
    value:int:5
    name:john doe");
            Assert.AreEqual (1, tmp [1] [1].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("value", tmp [1] [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (5, tmp [1] [1] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("name", tmp [1] [1] [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("john doe", tmp [1] [1] [0] [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertWithChildrenNodes ()
        {
            Node tmp = ExecuteLambda (@"pf.mongo.insert
  unit_tests
    name:john doe
    address
      zip:98765
      street:Dunbar Rd.
pf.mongo.select:unit_tests");
            Assert.AreEqual (1, tmp [1] [0].Count, "wrong value of node after executing lambda object");
            Assert.AreEqual ("name", tmp [1] [0] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("john doe", tmp [1] [0] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("address", tmp [1] [0] [0] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (null, tmp [1] [0] [0] [1].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("zip", tmp [1] [0] [0] [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("98765", tmp [1] [0] [0] [1] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual ("street", tmp [1] [0] [0] [1] [1].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("Dunbar Rd.", tmp [1] [0] [0] [1] [1].Value, "wrong value of node after executing lambda object");
        }
        
        [Test]
        public void InsertDeepHierarchyMultipleTypes ()
        {
            ExecuteLambda (@"pf.mongo.insert
  unit_tests
    name:john doe
    title:CEO
    department:California
      sub-department-id:int:12
      date-created:date:""2012-12-21T23:44:56.123""
      _x:@""sdfpih sdfih sdf
sdfijj dsfihj sdf""
    address
      zip:int:98765
      street:Dunbar Rd.
      pho_no_id:guid:E5A53FC9-A306-4609-89E5-9CC2964DA0ac
pf.mongo.select:unit_tests
set:@/../**/unit_tests/?value
if:@/../*/""pf.mongo.insert""/0/?node
  !=:@/../*/""pf.mongo.select""/0/result/*/?node
  lambda
    _set:faking exception due to expression being parsed late, and only if equality fails");
        }
    }
}
