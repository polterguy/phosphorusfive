/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 * MongoDB is licensed as Affero GPL, see the enclosed README.md file for details, 
 * since this file is linking towards the MongoDB .net driver, it hence needs to
 * be redistrivuted according to the Affero GPL license
 */

using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using phosphorus.core;

namespace phosphorus.mongodb.helpers
{
    /// <summary>
    ///     common helper functionality for the MongoDB drivers
    /// </summary>
    public static class Common
    {
        /*
         * static constructor, instantiating all common static values according to settings
         */
        static Common ()
        {
            ConnectionString = ConfigurationManager.AppSettings ["mongodb-connection-string"] ?? "mongodb://localhost";
            DataBaseName = ConfigurationManager.AppSettings ["mongodb-database"] ?? "phosphorus";
            Client = new MongoClient (ConnectionString);
        }

        /// <summary>
        ///     returns the connection string for our MongoDB
        /// </summary>
        /// <value>the connection string</value>
        private static string ConnectionString { get; set; }

        /// <summary>
        ///     returns the database name for the MongoDB
        /// </summary>
        /// <value>the database name</value>
        private static string DataBaseName { get; set; }

        /// <summary>
        ///     returns a MongoDB client
        /// </summary>
        /// <value>the MongoDB client</value>
        private static MongoClient Client { get; set; }

        /// <summary>
        ///     returns the database the MongoClient
        /// </summary>
        /// <value>the database</value>
        public static MongoDatabase DataBase
        {
            get { return Client.GetServer ().GetDatabase (DataBaseName); }
        }

        /// <summary>
        ///     creates a MongoDB query document from a node
        /// </summary>
        /// <returns>the query document</returns>
        /// <param name="node">node to create query document from</param>
        public static QueryDocument CreateQueryDocumentFromNode (Node node)
        {
            // loops through all children nodes, adding as BsonElements to returned QueryDocument
            var retVal = new QueryDocument ();
            if (node != null) {
                foreach (var idx in node.Children) {
                    retVal.Add (CreateQueryElementFromNode (idx));
                }
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
                var array = new BsonArray ();
                foreach (var idx in node.Children) {
                    array.Add (CreateQueryValueFromNode (idx));
                }
                bValue = array;
            } else {
                // this is a "dictionary of ONE item", being a document by itself
                bValue = new QueryDocument {CreateQueryElementFromNode (node [0])};
            }
            if (node.Name.StartsWith ("$")) {
                // MongoDB operator
                return new BsonElement (node.Name, bValue);
            }
            // automatically appending the '.value' parts
            return new BsonElement (node.Name + ".value", bValue);
        }

        /*
         * used for creating values from arrays of items in the above method
         */

        private static BsonValue CreateQueryValueFromNode (Node node)
        {
            if (node.Name == string.Empty) {
                // simple "array" construct
                return BsonValue.Create (node.Value);
            }
            // value is itself a Querydocument
            return new QueryDocument {CreateQueryElementFromNode (node)};
        }
    }
}