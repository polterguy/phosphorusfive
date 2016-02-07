/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.mongo.helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace p5.mongo
{
    /// <summary>
    ///     Class wrapping the MongoDB find
    /// </summary>
    public static class Find
    {
        /// <summary>
        ///     Finds documents from your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.find", Protection = EventProtection.LambdaClosed)]
        public static void p5_mongo_find (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {
                
                // Retrieving table name and running sanity check
                var tableName = e.Args.Get<string> (context);
                if (string.IsNullOrEmpty (tableName))
                    throw new LambdaException ("No table name supplied to [p5.mongo.find]", e.Args, context);

                // Retrieving filter, defaulting to empty
                var filter = Filter.CreateFilter (context, e.Args);

                // Retrieving collection
                var collection = Database.Instance.MongoDatabase.GetCollection<BsonDocument> (tableName);

                // Running query
                var cursor = collection.Find (filter);

                // Sorting query
                cursor = SortCursor (context, e.Args, cursor);

                // Checking if we've got a [start] offset
                if (e.Args ["start"] != null)
                    cursor.Skip (e.Args.GetChildValue ("start", context, 0));

                // Applying [count]
                int count = e.Args.GetChildValue ("count", context, -1);

                // Looping through result set until [count] is reached
                int idxCount = 0;
                foreach (var idxDoc in cursor.ToEnumerable ()) {

                    // Making sure we return currently iterated document to caller
                    var id = BsonTypeMapper.MapToDotNetValue (idxDoc.Elements.First (ix => ix.Name == "_id").Value);
                    var idxNode = e.Args.Add (tableName, id).LastChild;

                    // Parsing document, and stuffing results into idxNode, making sure we skip the "_id" element for main document
                    ParseDocument (context, idxNode, idxDoc, "_id");

                    // Checking if we've reached [count]
                    if (++idxCount == count)
                        break;
                }
            }
        }

        /*
         * Sorts a cursor according to the given args
         */
        private static IFindFluent<BsonDocument, BsonDocument> SortCursor (
            ApplicationContext context,
            Node args,
            IFindFluent<BsonDocument, BsonDocument> cursor)
        {
            // Checking if caller supplied [sort]
            if (args ["sort"] != null && args ["sort"].Children.Count > 0) {

                // Sorting in order of elements supplied by caller
                var sortBuilder = Builders<BsonDocument>.Sort;
                SortDefinition<BsonDocument> sort = null;
                foreach (var idxSort in args ["sort"].Children) {
                    var direction = idxSort.Get (context, "ascending");
                    if (direction == "ascending") {

                        // Ascending sort
                        sort = sortBuilder.Ascending (idxSort.Name);
                    } else if (direction == "descending") {

                        // Descending sort
                        sort = sortBuilder.Descending (idxSort.Name);
                    } else {

                        // Oops...!
                        throw new LambdaException ("Sorry, don't know how to sort in that direction!", idxSort, context);
                    }
                }
                cursor = cursor.Sort (sort);
            }
            return cursor;
        }

        /*
         * Parses a BsonDocument and returns it to caller in args
         */
        public static void ParseDocument (
            ApplicationContext context, 
            Node docNode, 
            BsonDocument doc,
            string skipID)
        {
            // Looping through each element in BsonDocument
            foreach (var idxEl in doc.Elements.Where (ix => ix.Name != skipID)) {

                // Adding currently iterated element
                var idxNode = docNode.Add (idxEl.Name).LastChild;
                ParseElementValue (context, idxNode, idxEl.Value);
            }
        }

        /*
         * Parses an element's value and returns to caller in elNode
         */
        private static void ParseElementValue (
            ApplicationContext context, 
            Node elNode, 
            BsonValue value)
        {

            // Figuring out type of element
            switch (value.BsonType) {
            case BsonType.Array:
                foreach (var idxValue in value.AsBsonArray) {
                    var childNode = elNode.Add ("").LastChild;
                    ParseElementValue (context, childNode, idxValue);
                }
                break;
            case BsonType.Binary:
                elNode.Value = value.AsByteArray;
                break;
            case BsonType.Boolean:
                elNode.Value = value.AsBoolean;
                break;
            case BsonType.DateTime:
                elNode.Value = value.ToUniversalTime ();
                break;
            case BsonType.Document:
                ParseDocument (context, elNode, value.AsBsonDocument, null);
                break;
            case BsonType.Double:
                elNode.Value = value.AsDouble;
                break;
            case BsonType.Int32:
                elNode.Value = value.AsInt32;
                break;
            case BsonType.Int64:
                elNode.Value = value.AsInt64;
                break;
            case BsonType.JavaScript:
                elNode.Value = value.ToString ();
                break;
            case BsonType.JavaScriptWithScope:
                elNode.Value = value.ToString ();
                break;
            case BsonType.Null:
                elNode.Value = null;
                break;
            case BsonType.ObjectId:
                elNode.Value = value.ToString ();
                break;
            case BsonType.RegularExpression:
                elNode.Value = value.ToString ();
                break;
            case BsonType.String:
                elNode.Value = value.AsString;
                break;
            case BsonType.Symbol:
                elNode.Value = value.ToString ();
                break;
            case BsonType.Timestamp:
                elNode.Value = value.ToString ();
                break;
            case BsonType.Undefined:
                elNode.Value = value.ToString ();
                break;
            }
        }
    }
}

