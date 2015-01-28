
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 * MongoDB is licensed as Affero GPL, see the enclosed README.md file for details, 
 * since this file is linking towards the MongoDB .net driver, it hence needs to
 * be redistrivuted according to the Affero GPL license
 */

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.mongodb
{
    /// <summary>
    /// wraps the MongoDB "select" statement
    /// </summary>
    public static class mongo_select
    {
        /// <summary>
        /// selects items from the MongoDB database according to criteria
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.mongo.select")]
        private static void pf_mongo_select (ApplicationContext context, ActiveEventArgs e)
        {
            string table = e.Args.Get<string> ();
            if (string.IsNullOrEmpty (table)) // no table name given
                throw new ArgumentException ("[pf.mongo.select] needs the table name as the value of its node, either through an expression, or a constant");

            if (Expression.IsExpression (table)) {

                // table name is given as an expression
                var match = Expression.Create (table).Evaluate (e.Args);
                table = match.GetValue (0) as string;
                if (!match.IsSingleLiteral || string.IsNullOrEmpty (table))
                    throw new ArgumentException ("if [pf.mongo.select] is given an expression, the expression needs to return only one value that can be converted into a string somehow");
            }

            // getting collection according to "table name"
            var collection = common.DataBase.GetCollection<BsonDocument> (table);

            // converting the current node structure to a Bson Document, used as criteria for our select
            QueryDocument query = common.CreateQueryDocumentFromNode (e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "where";
                }));

            // running the query, and putting results into [result] return node
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
         * creates a Node from a query match, or a cursor on a "find" operation
         */
        private static Node CreateNodeFromQueryMatch (BsonDocument cursor, string table)
        {
            // making sure we return the "_id" as the value of our main node
            Node retVal = new Node (table, cursor ["_id"]);

            // looping through all children elements in match, decorating our Node list
            foreach (var idx in cursor) {
                if (idx.Name == "_id")
                    continue; // skipping, since "_id" is special case, and signifies "ID" of item from database
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

                // item as "value"
                retVal.Value = BsonTypeMapper.MapToDotNetValue (inner ["value"]);
            }
            if (inner.Contains ("*")) {

                // item has "Children", recursively calling self to decorate the rest of the Node returned
                foreach (var idx in inner ["*"].AsBsonDocument) {
                    retVal.Add (CreateNodeFromElement (idx));
                }
            }
            return retVal;
        }
    }
}
