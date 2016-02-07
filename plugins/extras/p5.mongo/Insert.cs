/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.mongo.helpers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace p5.mongo
{
    /// <summary>
    ///     Class wrapping the MongoDB inserts
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     Inserts documents into your MongoDB instance
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.insert", Protection = EventProtection.LambdaClosed)]
        public static void p5_mongo_insert (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each document to insert
            foreach (var idx in XUtil.Iterate<Node> (context, e.Args, false, false, true)) {

                // Finding table name to insert document into
                var tableName = idx.Name;

                // Populating document
                var document = CreateDocument (context, idx);

                // Checking if we've got an explicit ID
                if (idx.Value != null) {
                    document.Add ("_id", BsonValue.Create (idx.Value));
                }

                // Inserting document into database instance
                var collection = Database.Instance.MongoDatabase.GetCollection<BsonDocument>(tableName);
                collection.InsertOne (document);
            }
        }

        /*
         * Created a BSON document according to given docNode
         */
        private static BsonDocument CreateDocument (ApplicationContext context, Node docNode)
        {
            var retVal = new BsonDocument (true);
            foreach (var idxChild in docNode.Children) {
                retVal.Add (CreateElement (context, idxChild));
            }
            return retVal;
        }

        /*
         * Creates a BsonElement from given elNode. Possible return values are document, simple name/value element or BsonArray
         */
        private static BsonElement CreateElement (ApplicationContext context, Node elNode)
        {
            // Sanity check!
            if (elNode.Children.Count > 0 && elNode.Value != null)
                throw new LambdaException ("Either supply value or children for node, not both", elNode, context);

            // Returning BsonElement according to node given
            if (elNode.Children.Count == 0) {

                // Simple value type
                return new BsonElement (elNode.Name, BsonValue.Create (elNode.Value));
            } else {

                // Complex type, either document or array, checking which according to names of children
                if (elNode.Children.Count == elNode.Children.Count (ix => ix.Name == "")) {

                    // Array type, since all children are "nameless"
                    var retVal = new BsonArray();
                    foreach (var idxChild in elNode.Children) {
                        retVal.Add (BsonValue.Create (idxChild.Value));
                    }
                    return new BsonElement (elNode.Name, retVal);
                } else {

                    // Complex inner document type, since children have names
                    var doc = new BsonDocument (true);
                    foreach (var idxChild in elNode.Children) {
                        doc.Add (CreateElement (context, idxChild));
                    }
                    return new BsonElement (elNode.Name, doc);
                }
            }
        }
    }
}

