
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
            string table = e.Args.Get<string> ();
            if (string.IsNullOrEmpty (table)) {

                // no table name given
                throw new ArgumentException ("[pf.mongo.select] needs the table name as the value of its node, either through an expression or a constant");
            }
            if (Expression.IsExpression (table)) {

                // table name is given as an expression
                var match = Expression.Create (table).Evaluate (e.Args);
                if (!match.IsSingleLiteral) {
                    throw new ArgumentException ("if [pf.mongo.select] is given an expression, the expression needs to return only one value that can be converted into a string somehow");
                }
                table = match.GetValue (0) as string;
                if (string.IsNullOrEmpty (table)) {
                    throw new ArgumentException ("if [pf.mongo.select] is given an expression, the expression needs to return only one value that can be converted into a string somehow");
                }
            }
            var server = _client.GetServer ();
            var db = server.GetDatabase (_db);
            var collection = db.GetCollection<BsonDocument> (table);

            // converting the current node structure to a Bson Document, used as criteria for our select
            QueryDocument query = CreateQueryDocumentFromNode (context, e.Args);
            var result = collection.Find (query);
            if (result.Count () > 0) {
                Node resultNode = new Node ("result");
                e.Args.Add (resultNode);
                foreach (var idxMatch in result) {
                    resultNode.Add (CreateNodeFromQueryMatch (idxMatch, table));
                }
            }
        }

        /*
         * creates a Query document from a node
         */
        private static QueryDocument CreateQueryDocumentFromNode (ApplicationContext context, Node node)
        {
            QueryDocument retVal = new QueryDocument ();
            foreach (Node idx in node.Children) {
                retVal.Add (CreateQueryElementFromNode (context, idx));
            }
            return retVal;
        }

        private static BsonElement CreateQueryElementFromNode (ApplicationContext context, Node node)
        {
            BsonElement retVal;
            if (node.Name == "_id") {
                retVal = new BsonElement ("_id", BsonValue.Create (node.Value));
            } else if (node.Count == 0) {
                if (node.Value == null && !node.Name.StartsWith ("$")) {
                    retVal = new BsonElement (node.Name, new QueryDocument ("$exists", true));
                } else if (node.Name.StartsWith ("$") && node.Value != null) {
                    retVal = new BsonElement (node.Name, BsonValue.Create (node.Value));
                } else {
                    retVal = new BsonElement (node.Name + ".value", BsonValue.Create (node.Value));
                }
            } else {
                BsonValue bValue;
                if (node.Count > 1) {
                    BsonArray array = new BsonArray ();
                    foreach (Node idx in node.Children) {
                        array.Add (CreateQueryValueFromNode (context, idx));
                    }
                    bValue = array;
                } else {
                    QueryDocument doc = new QueryDocument ();
                    foreach (Node idx in node.Children) {
                        doc.Add (CreateQueryElementFromNode (context, idx));
                    }
                    bValue = doc;
                }
                if (node.Name.StartsWith ("$")) {
                    retVal = new BsonElement (node.Name, bValue);
                } else {
                    retVal = new BsonElement (node.Name + ".value", bValue);
                }
            }
            return retVal;
        }
        
        private static BsonValue CreateQueryValueFromNode (ApplicationContext context, Node node)
        {
            BsonValue retVal;
            if (node.Name == string.Empty) {
                retVal = BsonValue.Create (node.Value);
            } else {
                QueryDocument doc = new QueryDocument ();
                doc.Add (CreateQueryElementFromNode (context, node));
                retVal = doc;
            }
            return retVal;
        }

        /*
         * creates a Node from a query match
         */
        private static Node CreateNodeFromQueryMatch (BsonDocument cursor, string table)
        {
            Node retVal = new Node (table, cursor ["_id"]);
            foreach (var idx in cursor) {
                if (idx.Name == "_id")
                    continue;
                retVal.Add (CreateNodeFromElement (idx));

            }
            return retVal;
        }

        /*
         * helper for above, recursively fills out node structure from given BsonElement
         */
        private static Node CreateNodeFromElement (BsonElement element)
        {
            Node retVal = new Node (element.Name);
            BsonDocument inner = element.Value.AsBsonDocument;
            if (inner.Contains ("value")) {
                retVal.Value = BsonTypeMapper.MapToDotNetValue (inner ["value"]);
            }
            if (inner.Contains ("*")) {
                foreach (var idx in inner ["*"].AsBsonDocument) {
                    retVal.Add (CreateNodeFromElement (idx));
                }
            }
            return retVal;
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
        /// inserts a node hierarchy into MongoDB instance, the nodes to insert can either be given as children nodes of [pf.mongo.insert],
        /// or as an expression or formatting expression in the value of [pf.mongo.insert]
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

            // looping through all nodes to insert
            foreach (Node idxNode in list) {

                // verifying node's value is "null", since anything else would be a bug
                // this ensures we can use the root node's value to return the ID of the BsonDocument back to caller, in 
                // addition to that it ensures we won't insert the same nodes twice
                if (idxNode.Value != null)
                    throw new ArgumentException ("root node's value to insert must be 'null'");

                // using the node name as the "database table name"
                var collection = db.GetCollection<BsonDocument> (idxNode.Name);

                // creating a Bson document from the currently iterated Node
                BsonDocument doc = CreateBsonDocumentFromNode (context, idxNode);

                // doing actually insert into database, and returning the "ID" created by MongoDB as "value"
                // of currently node inserted
                collection.Insert (doc);
                idxNode.Value = doc ["_id"];
            }
        }

        /*
         * creates a Bson document from the given node
         */
        private static BsonDocument CreateBsonDocumentFromNode (ApplicationContext context, Node node)
        {
            // looping through all properties on node recursively, adding them to bson document
            BsonDocument doc = new BsonDocument ();
            foreach (Node idxInner in node.Children) {
                doc.Add (idxInner.Name, CreateBsonValueFromNode (context, idxInner));
            }
            return doc;
        }

        /*
         * creates a BsonValue from the given node and returns to caller
         */
        private static BsonValue CreateBsonValueFromNode (ApplicationContext context, Node node)
        {
            // if both "value" and "children" are null/empty, we return null as value of node
            if (node.Value == null && node.Count == 0)
                return BsonNull.Value;

            // document to contain both "value" and "children" nodes
            BsonDocument retVal = new BsonDocument ();

            // we only return "value" node if value is not null
            if (node.Value != null) {
                if (node.Value is Node) {
                    Node convert = new Node (string.Empty, node.Value);
                    context.Raise ("lambda2code", convert);
                    retVal.Add ("value", BsonValue.Create (convert.Value));
                } else {
                    retVal.Add ("value", BsonValue.Create (node.Value));
                }
            }

            // we only return "children" node, if there are any children
            if (node.Count > 0) {

                // recursively iterating through children to add them up into "children" doc of current node
                BsonDocument childrenDoc = new BsonDocument ();
                foreach (Node idx in node.Children) {
                    childrenDoc.Add (idx.Name, CreateBsonValueFromNode (context, idx));
                }
                retVal.Add ("*", childrenDoc);
            }
            return retVal;
        }

        /*
         * returns the nodes to insert for an insert operation
         */
        private static IEnumerable<Node> GetInsertNodes (Node node)
        {
            // if node's value is not an expression, we assume the nodes the user wants to insert
            // are the children nodes of the given node
            if (!Expression.IsExpression (node.Value)) {
                if (node.Count == 0)
                    throw new ArgumentException ("not data given to [pf.mongo.xxx] Active Event when data was expected");
                foreach (Node idx in node.Children) {
                    yield return idx;
                }
                yield break;
            }

            // here we have an expression, returning the results of the expression, supporting "formatting expressions",
            // in addition to static expressions
            string expression = node.Get<string> ();
            if (node.Count > 0)
                expression = Expression.FormatNode (node);
            var match = Expression.Create (expression).Evaluate (node);
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
        
        /// <summary>
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.objectid")]
        private static void pf_hyperlist_get_object_value_objectid (ApplicationContext context, ActiveEventArgs e)
        {
            string strValue = e.Args.Get<string> ();
            e.Args.Value = ObjectId.Parse (strValue);
        }
        
        /// <summary>
        /// returns "objectid" for using as type information for the MongoDB.Bson.ObjectId type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.MongoDB.Bson.ObjectId")]
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.MongoDB.Bson.BsonObjectId")]
        private static void pf_hyperlist_get_type_name_MongoDB_Bson_ObjectId (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "objectid";
        }
    }
}
