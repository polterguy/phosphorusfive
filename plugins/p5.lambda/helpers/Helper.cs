/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

using p5.exp;
using p5.core;
using p5.exp.exceptions;
using System.Collections.Generic;
using System;

/// <summary>
///     Contains helper classes for lambda Active Events.
/// </summary>
namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping commonalities for lambda Active Events
    /// </summary>
    public class Helper
    {
        /// <summary>
        ///     Verifies node's value is an expression, and returns that expression to caller.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expNode"></param>
        /// <param name="activeEventName"></param>
        /// <returns></returns>
        static public Expression GetDestinationExpression (ApplicationContext context, Node expNode, string activeEventName)
        {
            // Asserting destination is expression.
            var exp = expNode.Value as Expression;
            if (exp == null)
                throw new LambdaException (
                    string.Format ("Not a valid destination expression given to [{0}], value was '{1}', expected expression", activeEventName, expNode.Value),
                    expNode,
                    context);
            return exp;
        }

        /// <summary>
        ///     Retrieves a node match object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expNode"></param>
        /// <param name="activeEventName"></param>
        /// <returns></returns>
        static public Match GetDestinationNodeMatch (ApplicationContext context, Node expNode, string activeEventName)
        {
            var ex = GetDestinationExpression (context, expNode, activeEventName);
            var match = ex.Evaluate (context, expNode, expNode);
            if (match.TypeOfMatch != Match.MatchType.node)
                throw new LambdaException (string.Format ("Destination for [{0}] was not a node type of expression", activeEventName), expNode, context);
            return match;
        }

        /// <summary>
        ///     Returns source value for an Active Event.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public object GetSourceValue (ApplicationContext context, Node args)
        {
            var srcNode = args.Children.Find (ix => ix.Name != "" && !ix.Name.StartsWith (".") && !ix.Name.StartsWith ("_"));
            if (srcNode == null)
                return null;
            context.Raise (srcNode.Name, srcNode);
            return srcNode.Value ?? srcNode.FirstChild;
        }

        /// <summary>
        ///     Returns the source nodes for an  Active Event invocation.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public List<Node> GetSourceNodes (ApplicationContext context, Node args)
        {
            // Retrieving first node with a legal Active Event name.
            var srcNode = args.Children.Find (ix => ix.Name != "" && !ix.Name.StartsWith (".") && !ix.Name.StartsWith ("_"));

            // If no source node was given, we assume no source was given.
            if (srcNode == null)
                return null;

            // Raising source Active Event and returning results.
            context.Raise (srcNode.Name, srcNode);

            // We prioritize value if it is existing after source Active Event invocation.
            // Fallback is children of invocation node.
            // Notice, if value alread is a node, we add it as it is, otherwise we convert it, and adds its children, since conversion will create a root node for us.
            return new List<Node> (srcNode.Value != null 
                ? (srcNode.Value is Node 
                    ? new List<Node> (new Node[] { srcNode.Value as Node }) 
                    : srcNode.Get<Node> (context).Children) 
                : srcNode.Children);
        }
    }
}
