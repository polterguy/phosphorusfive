/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
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
using p5.exp.exceptions;
using p5.mongo.helpers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace p5.mongo
{
    /// <summary>
    ///     Class wrapping the MongoDB aggregate features
    /// </summary>
    public static class Aggregate
    {
        /// <summary>
        ///     Aggregates documents from your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.aggregate")]
        public static void p5_mongo_aggregate (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new Utilities.ArgsRemover (e.Args, true)) {
                
                // Retrieving table name and running sanity check
                var tableName = e.Args.Get<string> (context);
                if (string.IsNullOrEmpty (tableName))
                    throw new LambdaException ("No table name supplied to [p5.mongo.find]", e.Args, context);

                // Retrieving collection
                var collection = Database.Instance.MongoDatabase.GetCollection<BsonDocument> (tableName);

                // Retrieving grouping clauses and creating BsonDocument for Group method
                BsonDocument doc = new BsonDocument ();
                doc.Add ("_id", BsonValue.Create (e.Args ["_id"].Value));
                foreach (var idxChild in e.Args.Children.Where (ix => ix.Name != "_id" && ix.Name != "where")) {
                    doc.Add (idxChild.Name, new BsonDocument (idxChild.Get<string> (context), 1));
                }

                // Retrieving filter criteria
                FilterDefinition<BsonDocument> filter = null;
                if (e.Args ["where"] != null && e.Args ["where"].Children.Count > 0) {
                    filter = Filter.CreateFilter (context, e.Args);
                }

                // Running query
                var aggregate = filter == null ? 
                    collection.Aggregate().Group (doc) :
                    collection.Aggregate().Match (filter).Group (doc);

                // Looping through each result
                foreach (var idxDoc in aggregate.ToEnumerable ()) {

                    // Making sure we return currently iterated document to caller
                    var id = BsonTypeMapper.MapToDotNetValue (idxDoc.Elements.First (ix => ix.Name == "_id").Value);
                    var idxNode = e.Args.Add (tableName, id).LastChild;

                    // Parsing document, and stuffing results into idxNode, making sure we skip the "_id" element for main document
                    DocumentParser.ParseDocument (context, idxNode, idxDoc, "_id");
                }
            }
        }
    }
}

