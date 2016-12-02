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

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

/// <summary>
///     Contains helper classes for lambda Active Events.
/// </summary>
namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping commonalities for lambda Active Events
    /// </summary>
    public class SourceHelper
    {
        /// <summary>
        ///     Returns a node match object, optionally restricted to node type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expressionNode"></param>
        /// <param name="activeEventName"></param>
        /// <returns></returns>
        static public Match GetDestinationMatch (ApplicationContext context, Node expressionNode, bool mustBeNodeTypeExpression = false)
        {
            var ex = GetDestinationExpression (context, expressionNode);
            var match = ex.Evaluate (context, expressionNode, expressionNode);

            // Checking if caller retricted type of expression, and if so, verifying it conforms.
            if (mustBeNodeTypeExpression && match.TypeOfMatch != Match.MatchType.node)
                throw new LambdaException (string.Format ("Destination for [{0}] was not a node type of expression", expressionNode.Name), expressionNode, context);

            // Success, returning match object to caller.
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
            var srcNode = args.Children.Where (ix => ix.Name != "" && !ix.Name.StartsWith (".") && !ix.Name.StartsWith ("_")).ToList ();

            // Sanity check.
            if (srcNode.Count > 1)
                throw new LambdaException ("Multiple source found for [" + args.Name + "]", args, context);

            // Checking if there was any source.
            if (srcNode.Count == 0)
                return null;

            // Raising source Active Event, and returning results.
            context.Raise (srcNode[0].Name, srcNode[0]);

            // Sanity check
            if (srcNode[0].Value == null && srcNode[0].Children.Count > 1)
                throw new LambdaException ("Source Active Event returned multiple source", args, context);

            // Returning value
            return srcNode[0].Value ?? srcNode[0].FirstChild;
        }

        /// <summary>
        ///     Returns the source nodes for an  Active Event invocation.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public List<Node> GetSourceNodes (
            ApplicationContext context, 
            Node args, 
            params string[] avoidNodes)
        {
            // Retrieving all source nodes with a legal Active Event name.
            var srcNodes = args.Children.Where (ix => ix.Name != "" && !ix.Name.StartsWith (".") && !ix.Name.StartsWith ("_")).ToList ();
            if (avoidNodes != null)
                srcNodes.RemoveAll (ix => avoidNodes.Contains (ix.Name));

            // If no source nodes was given, we return no source early.
            if (srcNodes.Count == 0)
                return null;

            // Looping through all source nodes, invoking Active Events, and adding into return value.
            var retVal = new List<Node> ();
            foreach (var idxSrc in srcNodes) {

                // Raising source Active Event.
                context.Raise (idxSrc.Name, idxSrc);

                // We prioritize value if it is existing after source Active Event invocation.
                // Fallback is children of invocation node.
                // Notice, if value alread is a node, we add it as it is, otherwise we convert it, and adds its children, since conversion will create a root node for us.
                if (idxSrc.Value != null) {
                    if (idxSrc.Value is Node) {
                        retVal.Add (idxSrc.Value as Node);
                    } else {
                        retVal.AddRange (idxSrc.Get<Node> (context).Children);
                    }
                } else {
                    retVal.AddRange (idxSrc.Children);
                }
            }
            return retVal.Count > 0 ? retVal : null;
        }

        /// <summary>
        ///     Verifies node's value is an expression, and returns that expression to caller.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expressionNode"></param>
        /// <param name="activeEventName"></param>
        /// <returns></returns>
        static private Expression GetDestinationExpression (ApplicationContext context, Node expressionNode)
        {
            // Asserting destination is expression.
            var ex = expressionNode.Value as Expression;
            if (ex == null)
                throw new LambdaException (
                    string.Format ("Not a valid destination expression given to [{0}], value was '{1}', expected expression", expressionNode.Name, expressionNode.Value),
                    expressionNode,
                    context);
            return ex;
        }
    }
}
