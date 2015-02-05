
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
using phosphorus.expressions;

namespace phosphorus.mongodb
{
    /// <summary>
    /// wraps the MongoDB "insert" statement
    /// </summary>
    public static class mongo_insert
    {
        /// <summary>
        /// inserts a node hierarchy into MongoDB instance, the nodes to insert can either be given as children nodes of [pf.mongo.insert],
        /// or as an expression, or formatting expression, in the value of [pf.mongo.insert]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.mongo.insert")]
        private static void pf_mongo_insert (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving nodes to actually insert
            IEnumerable<Node> list = GetInsertNodes (e.Args, context);

            // inserting nodes
            var db = common.DataBase;

            // looping through all nodes to insert
            foreach (Node idxNode in list) {

                // verifying node's value is "null", since anything else would be a bug
                // this ensures we can use the root node's value to return the ID of the BsonDocument back to caller, in 
                // addition to that it ensures we won't insert the same nodes twice
                if (idxNode.Value != null)
                    throw new ArgumentException ("root node's value to insert must be 'null'");

                // using the node name as the "database table name"
                var collection = db.GetCollection<BsonDocument> (idxNode.Name);

                // creating a Bson document from the currently iterated Node
                BsonDocument doc = CreateBsonDocumentFromNode (context, idxNode);

                // doing actually insert into database, and returning the "ID" created by MongoDB as "value"
                // of currently node inserted
                collection.Insert (doc);
                idxNode.Value = doc ["_id"];
            }
        }

        /*
         * creates a Bson document from the given node
         */
        private static BsonDocument CreateBsonDocumentFromNode (ApplicationContext context, Node node)
        {
            // looping through all properties on node recursively, adding them to bson document
            BsonDocument doc = new BsonDocument ();
            foreach (Node idxInner in node.Children) {
                doc.Add (idxInner.Name, CreateBsonValueFromNode (context, idxInner));
            }
            return doc;
        }

        /*
         * creates a BsonValue from the given node and returns to caller
         */
        private static BsonValue CreateBsonValueFromNode (ApplicationContext context, Node node)
        {
            // if both "value" and "children" are null/empty, we return null as value of node
            if (node.Value == null && node.Count == 0)
                return BsonNull.Value;

            // document to contain both "value" and "children" nodes
            BsonDocument retVal = new BsonDocument ();

            // we only return "value" node if value is not null
            if (node.Value != null) {
                if (node.Value is Node) {
                    Node convert = new Node (string.Empty, node.Value);
                    context.Raise ("pf.hyperlisp.lambda2code", convert);
                    retVal.Add ("value", BsonValue.Create (convert.Value));
                } else {
                    retVal.Add ("value", BsonValue.Create (node.Value));
                }
            }

            // we only return "children" node, if there are any children
            if (node.Count > 0) {

                // recursively iterating through children to add them up into "children" doc of current node
                BsonDocument childrenDoc = new BsonDocument ();
                foreach (Node idx in node.Children) {
                    childrenDoc.Add (idx.Name, CreateBsonValueFromNode (context, idx));
                }
                retVal.Add ("*", childrenDoc);
            }
            return retVal;
        }

        /*
         * returns the nodes to insert for an insert operation
         */
        private static IEnumerable<Node> GetInsertNodes (Node node, ApplicationContext context)
        {
            // if node's value is not an expression, we assume the nodes the user wants to insert
            // are the children nodes of the given node
            if (!XUtil.IsExpression (node.Value)) {
                if (node.Count == 0)
                    throw new ArgumentException ("not data given to [pf.mongo.xxx] Active Event when data was expected");
                foreach (Node idx in node.Children) {
                    yield return idx;
                }
                yield break;
            }

            // here we have an expression, returning the results of the expression, supporting "formatting expressions",
            // in addition to static expressions
            string expression = node.Get<string> (context);
            if (node.Count > 0)
                expression = XUtil.FormatNode (node, node, context) as string;
            var match = Expression.Create (expression).Evaluate (node, context);
            if (match.Count == 0)
                throw new ArgumentException ("expression expected to return 'node' in [pf.mongo.xxx] returned nothing");
            if (match.TypeOfMatch == Match.MatchType.node) {
                foreach (var idx in match) {
                    yield return idx.Node;
                }
                yield break;
            }

            for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                var retVal = match [idxNo].Value as Node;
                if (retVal == null)
                    throw new ArgumentException ("expression for [pf.mongo.xxx] that was expected to return a 'node' value, didn't");
                yield return retVal;
            }
        }
    }
}
