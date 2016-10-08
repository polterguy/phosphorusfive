/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Linq;
using p5.core;
using p5.mongo.helpers;
using p5.exp.exceptions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace p5.mongo {
    /// <summary>
    ///     Class wrapping the MongoDB index
    /// </summary>
    public static class Index
    {
        /// <summary>
        ///     Creates an index in your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.create-index")]
        public static void p5_mongo_create_index (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check
            if (e.Args.Children.Count == 0)
                throw new LambdaException ("No column supplied to [p5.mongo.create-index]", e.Args, context);
            
            // Retrieving table name and running sanity check
            var tableName = e.Args.Get<string> (context);
            if (string.IsNullOrEmpty (tableName))
                throw new LambdaException ("No table name supplied to [p5.mongo.create-index]", e.Args, context);

            // Retrieving collection
            var collection = Database.Instance.MongoDatabase.GetCollection<BsonDocument> (tableName);

            // Creating first index
            var builder = Builders<BsonDocument>.IndexKeys;
            var curIndex = e.Args [0].Get<string> (context, "ascending") == "ascending" ? 
                builder.Ascending (e.Args [0].Name) : 
                builder.Descending (e.Args [0].Name);
            e.Args [0].UnTie ();

            // Looping through the rest of the indexes supplied, creating compound index combined of all supplied fields
            foreach (var idxIndex in e.Args.Children) {
                curIndex = idxIndex.Get<string> (context, "ascending") == "ascending" ? 
                    curIndex.Ascending (idxIndex.Name) :
                    curIndex.Descending (idxIndex.Name);
            }

            // Cleaning up
            e.Args.Clear ();

            // Creating index on table
            collection.Indexes.CreateOne (curIndex);
        }

        /// <summary>
        ///     Creates an index in your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.create-text-index")]
        public static void p5_mongo_create_text_index (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check
            if (e.Args.Children.Count == 0)
                throw new LambdaException ("No column supplied to [p5.mongo.create-text-index]", e.Args, context);

            // Retrieving table name and running sanity check
            var tableName = e.Args.Get<string> (context);
            if (string.IsNullOrEmpty (tableName))
                throw new LambdaException ("No table name supplied to [p5.mongo.create-text-index]", e.Args, context);

            // Retrieving collection
            var collection = Database.Instance.MongoDatabase.GetCollection<BsonDocument> (tableName);

            // Creating text index
            var builder = Builders<BsonDocument>.IndexKeys;
            var index = builder.Text (e.Args.FirstChild.Name);

            e.Args.Clear ();
            collection.Indexes.CreateOne (index);
        }

        /// <summary>
        ///     Lists all indexes in your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.list-indexes")]
        public static void p5_mongo_list_indexes (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving table name and running sanity check
            var tableName = e.Args.Get<string> (context);
            if (string.IsNullOrEmpty (tableName))
                throw new LambdaException ("No table name supplied to [p5.mongo.list-indexes]", e.Args, context);

            // Retrieving collection
            var collection = Database.Instance.MongoDatabase.GetCollection<BsonDocument> (tableName);

            // Looping through each index, and returning to caller
            foreach (var idxIndexCursor in collection.Indexes.List ().ToEnumerable ()){

                // Making sure we return currently iterated document to caller
                var id = BsonTypeMapper.MapToDotNetValue (idxIndexCursor.Elements.First (ix => ix.Name == "name").Value).ToString ();
                var idxNode = e.Args.Add (id).LastChild;

                DocumentParser.ParseDocument (context, idxNode, idxIndexCursor, "name");
            }
        }

        /// <summary>
        ///     Drops one or more indexes from your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.drop-indexes")]
        public static void p5_mongo_drop_indexes (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving table name and running sanity check
            var tableName = e.Args.Get<string> (context);
            if (string.IsNullOrEmpty (tableName))
                throw new LambdaException ("No table name supplied to [p5.mongo.drop-indexes]", e.Args, context);

            // Retrieving collection
            var collection = Database.Instance.MongoDatabase.GetCollection<BsonDocument> (tableName);

            // Looping through each index caller wants to drop
            foreach (var idxIndexNode in e.Args.Children) {

                // Drops currently iterated index
                collection.Indexes.DropOne (idxIndexNode.Name);
            }
        }
    }
}

