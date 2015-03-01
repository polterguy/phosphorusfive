/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 * MongoDB is licensed as Affero GPL, see the enclosed README.md file for details, 
 * since this file is linking towards the MongoDB .net driver, it hence needs to
 * be redistrivuted according to the Affero GPL license
 */

using System;
using MongoDB.Bson;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.mongodb.helpers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.mongodb
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
        [ActiveEvent (Name = "pf.mongo.delete")]
        private static void pf_mongo_delete (ApplicationContext context, ActiveEventArgs e)
        {
            var table = e.Args.Get<string> (context);
            if (string.IsNullOrEmpty (table)) // no table name given
                throw new ArgumentException ("[pf.mongo.delete] needs the table name as the value of its node, either through an expression or a constant");

            if (XUtil.IsExpression (table)) {
                // table name is given as an expression
                var match = Expression.Create (table).Evaluate (e.Args, context);
                table = match [0].Value as string;
                //if (!match.IsSingleLiteral || string.IsNullOrEmpty (table))
                //    throw new ArgumentException ("if [pf.mongo.delete] is given an expression, the expression needs to return only one value that can be converted into a string somehow");
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