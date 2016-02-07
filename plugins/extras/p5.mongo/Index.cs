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
    ///     Class wrapping the MongoDB index
    /// </summary>
    public static class Index
    {
        /// <summary>
        ///     Creates an index in your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.index", Protection = EventProtection.LambdaClosed)]
        public static void p5_mongo_index (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check
            if (e.Args.Children.Count == 0)
                throw new LambdaException ("No column supplied to [p5.mongo.index]", e.Args, context);
            
            // Retrieving table name and running sanity check
            var tableName = e.Args.Get<string> (context);
            if (string.IsNullOrEmpty (tableName))
                throw new LambdaException ("No table name supplied to [p5.mongo.index]", e.Args, context);

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
    }
}

