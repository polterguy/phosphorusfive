
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
    public class MongoDBTests
    {
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
    simple_integer:int:54
pf.mongo.select:unit_tests
";
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
            context.Raise ("lambda", tmp);
            Assert.AreEqual ("result", tmp [1] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual ("unit_tests", tmp [1] [0] [0].Name, "wrong value of node after executing lambda object");
            Assert.AreEqual (1, tmp [1] [0].Count, "wrong value of node after executing lambda object");
            Assert.IsTrue (tmp [1] [0] [0].Value is MongoDB.Bson.BsonObjectId, "wrong value of node after executing lambda object");
            Assert.AreEqual ("howdy world", tmp [1] [0] [0] [0].Value, "wrong value of node after executing lambda object");
            Assert.AreEqual (54, tmp [1] [0] [0] [1].Value, "wrong value of node after executing lambda object");
        }
    }
}
