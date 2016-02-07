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
    ///     Class wrapping the MongoDB delete
    /// </summary>
    public static class Delete
    {
        /// <summary>
        ///     Finds documents from your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.delete", Protection = EventProtection.LambdaClosed)]
        public static void p5_mongo_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // Hous cleaning
            using (new Utilities.ArgsRemover (e.Args)) {
                
                // Retrieving table name and running sanity check
                var tableName = e.Args.Get<string> (context);
                if (string.IsNullOrEmpty (tableName))
                    throw new LambdaException ("No table name supplied to [p5.mongo.delete]", e.Args, context);

                // Retrieving filter, defaulting to empty
                var filter = Filter.CreateFilter (context, e.Args);

                // Retrieving collection
                var collection = Database.Instance.MongoDatabase.GetCollection<BsonDocument> (tableName);

                // Running query, looping through each result, and returning to caller
                var result = collection.DeleteMany (filter);

                // Returning result of operation
                e.Args.Value = result.DeletedCount;
            }
        }
    }
}

