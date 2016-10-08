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
    ///     Class wrapping the MongoDB find
    /// </summary>
    public static class Find
    {
        /// <summary>
        ///     Finds documents from your MongoDB database
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mongo.find")]
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
                    DocumentParser.ParseDocument (context, idxNode, idxDoc, "_id");

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
    }
}

