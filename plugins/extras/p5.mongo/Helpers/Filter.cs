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
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Linq;
using p5.core;
using p5.exp.exceptions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace p5.mongo.helpers
{
    /// <summary>
    ///     Class wrapping the MongoDB database filter creation
    /// </summary>
    public static class Filter
    {
        /*
         * Creates a filter according to [where] children of filterNode, and returns to caller
         */
        public static FilterDefinition<BsonDocument> CreateFilter (ApplicationContext context, Node filterNode)
        {
            // Creating return value, defaulting to empty filter
            var retVal = FilterDefinition<BsonDocument>.Empty;

            // Checking if a filter was supplied
            if (filterNode ["where"] != null) {

                // Looping through all filters supplied
                foreach (var idxFilter in filterNode ["where"].Children) {

                    // Updating filter according to current filter node
                    retVal = CreateFilterInternal (context, idxFilter, retVal);
                }
            }

            // Returning filter to caller
            return retVal;
        }

        /*
         * Creates a filter and adds to existing filter
         */
        private static FilterDefinition<BsonDocument> CreateFilterInternal (
            ApplicationContext context, 
            Node filterNode,
            FilterDefinition<BsonDocument> previous)
        {
            // Checking type of boolean operator, if any
            switch (filterNode.Name) {
            case "and":
            case "or":
                var original = previous;
                foreach (var idxChildNode in filterNode.Children) {
                    previous = CreateFilterInternal (context, idxChildNode, previous);
                }
                return filterNode.Name == "and" ? 
                    Builders<BsonDocument>.Filter.And (new [] {original, previous}) : 
                    Builders<BsonDocument>.Filter.Or (new [] {original, previous});
            case "not":
                foreach (var idxChildNode in filterNode.Children) {
                    previous = CreateFilterInternal (context, idxChildNode, previous);
                }
                return Builders<BsonDocument>.Filter.Not (previous);
            case "text":
                return Builders<BsonDocument>.Filter.Text (filterNode.Get<string> (context));
            default:
                return CreateSingleCriteria (context, filterNode, previous);
            }
        }

        /*
         * Creates a single criteria according to given filterNode
         */
        private static FilterDefinition<BsonDocument> CreateSingleCriteria (
            ApplicationContext context,
            Node filterNode,
            FilterDefinition<BsonDocument> previous)
        {
            // Extracting operator, lhs and rhs
            var critOperator = filterNode.FirstChild.Name;
            var column = filterNode.Name;
            var value = filterNode.FirstChild.Value;

            // Checking type of operator
            switch (critOperator) {
            case "=":
                return Builders<BsonDocument>.Filter.Eq (column, value);
            case "!=":
                return Builders<BsonDocument>.Filter.Ne (column, value);
            case ">":
                return Builders<BsonDocument>.Filter.Gt (column, value);
            case "<":
                return Builders<BsonDocument>.Filter.Lt (column, value);
            case ">=":
                return Builders<BsonDocument>.Filter.Gte (column, value);
            case "<=":
                return Builders<BsonDocument>.Filter.Lte (column, value);
            case "regex":
                return Builders<BsonDocument>.Filter.Regex (column, new BsonRegularExpression (Utilities.Convert<string> (context, value)));
            case "exist":
                return Builders<BsonDocument>.Filter.Exists (column, value == null ? true : Utilities.Convert<bool> (context, value));
            case "in":
                return Builders<BsonDocument>.Filter.In (column, filterNode.FirstChild.Children.Select (ix => ix.Value).ToArray ());
            case "not-in":
                return Builders<BsonDocument>.Filter.Nin (column, filterNode.FirstChild.Children.Select (ix => ix.Value).ToArray ());
            default:
                throw new LambdaException ("Unknown operator supplied to filter", filterNode.FirstChild, context);
            }
        }
    }
}

