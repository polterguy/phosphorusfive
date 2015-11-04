/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 * MongoDB is licensed as Affero GPL, see the enclosed README.md file for details, 
 * since this file is linking towards the MongoDB .net driver, it hence needs to
 * be redistrivuted according to the Affero GPL license
 */

using System;
using MongoDB.Bson;
using p5.core;
using p5.exp;
using p5.mongodb.helpers;

namespace p5.mongodb
{
    /// <summary>
    ///     wraps the MongoDB "delete" statement
    /// </summary>
    public static class Delete
    {
        /// <summary>
        ///     deletes items from the MongoDB database according to the criteria given
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mongo.delete")]
        private static void p5_mongo_delete (ApplicationContext context, ActiveEventArgs e)
        {
            var table = e.Args.Get<string> (context);
            if (string.IsNullOrEmpty (table)) // no table name given
                throw new ArgumentException ("[p5.mongo.delete] needs the table name as the value of its node, either through an expression or a constant");

            if (XUtil.IsExpression (table)) {
                // table name is given as an expression
                var match = Expression.Create (table, context).Evaluate (e.Args, context);
                table = match [0].Value as string;
                //if (!match.IsSingleLiteral || string.IsNullOrEmpty (table))
                //    throw new ArgumentException ("if [p5.mongo.delete] is given an expression, the expression needs to return only one value that can be converted into a string somehow");
            }

            // getting collection according to "table name"
            var collection = Common.DataBase.GetCollection<BsonDocument> (table);

            // converting the current node structure to a Bson Document, used as criteria for our select
            var query = Common.CreateQueryDocumentFromNode (e.Args);

            // running the query, removing all items matching criteria from database
            collection.Remove (query);
        }
    }
}