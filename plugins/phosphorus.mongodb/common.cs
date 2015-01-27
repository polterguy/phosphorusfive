
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 * MongoDB is licensed as Affero GPL, see the enclosed README.md file for details, 
 * since this file is linking towards the MongoDB .net driver, it hence needs to
 * be redistrivuted according to the Affero GPL license
 */

using System;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using phosphorus.core;

namespace phosphorus.mongodb
{
    /// <summary>
    /// common functionality for the MongoDB drivers
    /// </summary>
    public static class common
    {
        // contains the connection string to the MongoDB you're using in this application
        private static string _connectionString;
        private static MongoClient _client;
        private static string _db;

        /*
         * static constructor, instantiating all common static values according to settings
         */
        static common ()
        {
            _connectionString = ConfigurationManager.AppSettings ["mongodb-connection-string"] ?? "mongodb://localhost";
            _db = ConfigurationManager.AppSettings ["mongodb-database"] ?? "phosphorus";
            _client = new MongoClient (_connectionString);
        }
        
        /// <summary>
        /// returns the connection string for our MongoDB
        /// </summary>
        /// <value>the connection string</value>
        public static string ConnectionString {
            get {
                return _connectionString;
            }
        }

        /// <summary>
        /// returns the database name for the MongoDB
        /// </summary>
        /// <value>the database name</value>
        public static string DataBaseName {
            get {
                return _db;
            }
        }

        /// <summary>
        /// returns a MongoDB client
        /// </summary>
        /// <value>the MongoDB client</value>
        public static MongoClient Client {
            get {
                return _client;
            }
        }

        /// <summary>
        /// returns the database the MongoClient
        /// </summary>
        /// <value>the database</value>
        public static MongoDatabase DataBase {
            get {
                return _client.GetServer ().GetDatabase (_db);
            }
        }
        
        /*
         * creates a Query document from the given node
         */
        public static QueryDocument CreateQueryDocumentFromNode (Node node)
        {
            // loops through all children nodes, adding as BsonElements to returned QueryDocument
            QueryDocument retVal = new QueryDocument ();
            foreach (Node idx in node.Children) {
                retVal.Add (CreateQueryElementFromNode (idx));
            }
            return retVal;
        }

        /*
         * creates a query element from the given node
         */
        private static BsonElement CreateQueryElementFromNode (Node node)
        {
            BsonElement retVal;
            if (node.Name == "_id") {

                // ID criteria
                retVal = new BsonElement ("_id", BsonValue.Create (node.Value));
            } else if (node.Count == 0) {

                if (node.Value == null && !node.Name.StartsWith ("$")) {

                    // simple "exists" syntactic helper
                    retVal = new BsonElement (node.Name, new QueryDocument ("$exists", true));
                } else if (node.Name.StartsWith ("$") && node.Value != null) {

                    // MongoDB "operator"
                    retVal = new BsonElement (node.Name, BsonValue.Create (node.Value));
                } else {

                    // simple equality comparison, appending '.value' automatically
                    retVal = new BsonElement (node.Name + ".value", BsonValue.Create (node.Value));
                }
            } else {

                // this is a list of nodes, handing over to "list builder"
                retVal = CreateQueryElementFromNodeList (node);
            }
            return retVal;
        }

        /*
         * expects a node with children and returns a list of query element BsonElements
         */
        private static BsonElement CreateQueryElementFromNodeList (Node node)
        {
            BsonValue bValue;
            if (node.Count > 1) {

                // this is an "array of items"
                BsonArray array = new BsonArray ();
                foreach (Node idx in node.Children) {
                    array.Add (CreateQueryValueFromNode (idx));
                }
                bValue = array;
            } else {

                // this is a "dictionary of ONE item", being a document by itself
                QueryDocument doc = new QueryDocument ();
                doc.Add (CreateQueryElementFromNode (node[0]));
                bValue = doc;
            }
            if (node.Name.StartsWith ("$")) {

                // MongoDB operator
                return new BsonElement (node.Name, bValue);
            } else {

                // automatically appending the '.value' parts
                return new BsonElement (node.Name + ".value", bValue);
            }
        }

        /*
         * used for creating values from arrays of items in the above method
         */
        private static BsonValue CreateQueryValueFromNode (Node node)
        {
            if (node.Name == string.Empty) {

                // simple "array" construct
                return BsonValue.Create (node.Value);
            } else {

                // value is itself a Querydocument
                QueryDocument doc = new QueryDocument ();
                doc.Add (CreateQueryElementFromNode (node));
                return doc;
            }
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
