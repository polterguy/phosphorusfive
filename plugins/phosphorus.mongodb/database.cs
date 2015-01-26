
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 * MongoDB is licensed as Affero GPL, see the enclosed README.md file for details
 */

using System;
using System.Configuration;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Builders;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.mongodb
{
    /// <summary>
    /// wraps the MongoDB drivers
    /// </summary>
    public static class database
    {
        // contains the connection string to the MongoDB you're using in this application
        private static string _connectionString;
        private static MongoClient _client;
        private static string _db;

        /*
         * static constructor, instantiating all common static values according to settings
         */
        static database ()
        {
            _connectionString = ConfigurationManager.AppSettings ["mongodb-connection-string"];
            _db = ConfigurationManager.AppSettings ["mongodb-database"];
            _client = new MongoClient (_connectionString);
        }

        /// <summary>
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.mongo.select")]
        private static void pf_mongo_select (ApplicationContext context, ActiveEventArgs e)
        {
            var server = _client.GetServer ();
            var db = server.GetDatabase (_db);
            //var collection = db.GetCollection<Entity> ("entities");

            /*var query = Query.And (CreateCriteria (e.Args));
            foreach (var idx in collection.Find (query)) {
                idx.Root.Value = idx.Id;
                e.Args.Add (idx.Root);
            }*/
        }

        /// <summary>
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.mongo.update")]
        private static void pf_mongo_update (ApplicationContext context, ActiveEventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.mongo.insert")]
        private static void pf_mongo_insert (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving nodes to actually insert
            IEnumerable<Node> list = GetInsertNodes (e.Args);

            // inserting nodes
            var server = _client.GetServer ();
            var db = server.GetDatabase (_db);

            foreach (Node idxNode in list) {
                var collection = db.GetCollection<BsonDocument> (idxNode.Name);
                BsonDocument doc = new BsonDocument (true);
                foreach (Node idxInner in idxNode.Children) {
                    doc.Add (new BsonElement (idxInner.Name, GetNextDoc (idxInner)));
                }
                collection.Insert (doc);
                idxNode.Value = doc ["_id"];
            }
        }

        private static BsonValue GetNextDoc (Node node)
        {
            if (node.Value == null && node.Count == 0)
                return BsonNull.Value;
            BsonDocument ret = new BsonDocument ();
            if (node.Value != null) {
                ret.Add (new BsonElement ("value", node.Value == null ? BsonNull.Value : BsonValue.Create (node.Value)));
            }
            if (node.Count > 0) {
                BsonDocument children = new BsonDocument (true);
                foreach (Node idx in node.Children) {
                    children.Add (idx.Name, GetNextDoc (idx));
                }
                ret.Add ("children", children);
            }
            return ret;
        }

        private static IEnumerable<Node> GetInsertNodes (Node node)
        {
            if (!Expression.IsExpression (node.Value)) {
                foreach (Node idx in node.Children) {
                    yield return idx;
                }
                yield break;
            }

            var match = Expression.Create (node.Get<string> ()).Evaluate (node);
            if (match.Count == 0)
                throw new ArgumentException ("expression expected to return 'node' in [pf.mongo.xxx] returned nothing");
            if (match.TypeOfMatch == Match.MatchType.Node) {
                foreach (Node idx in match.Matches) {
                    yield return idx;
                }
                yield break;
            }

            for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                var retVal = match.GetValue (idxNo) as Node;
                if (retVal == null)
                    throw new ArgumentException ("expression for [pf.mongo.xxx] that was expected to return a 'node' value, didn't");
                yield return retVal;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.mongo.delete")]
        private static void pf_mongo_delete (ApplicationContext context, ActiveEventArgs e)
        {
        }
    }
}
