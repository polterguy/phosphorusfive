
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
    /// wraps the MongoDB "delete" statement
    /// </summary>
    public static class update
    {
        /// <summary>
        /// updates items from the MongoDB database according to the criteria given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.mongo.update")]
        private static void pf_mongo_update (ApplicationContext context, ActiveEventArgs e)
        {
            string table = e.Args.Get<string> ();
            if (string.IsNullOrEmpty (table)) // no table name given
                throw new ArgumentException ("[pf.mongo.update] needs the table name as the value of its node, either through an expression or a constant");

            if (Expression.IsExpression (table)) {

                // table name is given as an expression
                var match = Expression.Create (table).Evaluate (e.Args);
                table = match.GetValue (0) as string;
                if (!match.IsSingleLiteral || string.IsNullOrEmpty (table))
                    throw new ArgumentException ("if [pf.mongo.update] is given an expression, the expression needs to return only one value that can be converted into a string somehow");
            }
            
            // getting collection according to "table name"
            var collection = common.DataBase.GetCollection<BsonDocument> (table);

            // converting the current node structure to a Bson Document, used as criteria for our select
            QueryDocument query = common.CreateQueryDocumentFromNode (e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "where";
                }));

            UpdateDocument update = CreateUpdateDocument (e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "$set";
            }));

            // running the query, removing all items matching criteria from database
            var result = collection.Update (query, update);
            e.Args.Add (new Node ("result", result.DocumentsAffected));
        }

        /*
         * creates an UpdateDocument for the above method
         */
        private static UpdateDocument CreateUpdateDocument (Node node)
        {
            UpdateDocument retVal = new UpdateDocument (true);
            foreach (Node idx in node.Children) {
                retVal.Add ("$set", new BsonDocument (idx.Name + ".value", BsonValue.Create (idx.Value)));
            }
            return retVal;
        }
    }
}
