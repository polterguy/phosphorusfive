/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using System.Collections;
using System.Collections.Generic;
using p5.core;
using p5.exp.exceptions;

namespace p5.exp
{
    /// <summary>
    ///     Helper class encapsulating common operations for expressions.
    /// </summary>
    public static class XUtil
    {
        /// <summary>
        ///     Delegate used for updating collections.
        /// </summary>
        /// <param name="key">Key in collection</param>
        /// <param name="value">Value for item with specified key (can be null)</param>
        public delegate void SetDelegate (string key, object value);

        /// <summary>
        ///     Delegate used when retrieving collection values.
        /// </summary>
        /// <param name="key">Key to retrieve from collection</param>
        /// <returns></returns>
        public delegate object GetDelegate (string key);

        /// <summary>
        ///     Formats the node according to values returned by its children having empty names.
        /// </summary>
        /// <returns>The node</returns>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="context">Context</param>
        public static object FormatNode (
            ApplicationContext context,
            Node evaluatedNode)
        {
            return FormatNode (context, evaluatedNode, evaluatedNode);
        }

        /// <summary>
        ///     Formats the node according to values returned by its children, with the specified datasource node.
        /// </summary>
        /// <returns>The node</returns>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="dataSource">Data source</param>
        /// <param name="context">Context</param>
        public static object FormatNode (
            ApplicationContext context,
            Node evaluatedNode,
            Node dataSource)
        {
            // Making sure node contains formatting values, and if not, returning as is to avoid formatting or changing type.
            if (evaluatedNode.Children.Count (ix => ix.Name == "") == 0)
                return evaluatedNode.Value;

            var childrenValues = evaluatedNode.Children
                .Where (ix => ix.Name == "")
                .Select (ix => FormatNodeRecursively (context, ix, dataSource == evaluatedNode ? ix : dataSource) ?? "").ToArray ();

            // Returning node's value, after being formatted, according to its children node's values.
            var retVal = string.Format (evaluatedNode.Get<string> (context), childrenValues);

            // House cleaning before returning formatted value.
            foreach (var idx in evaluatedNode.Children.Where (ix => ix.Name == "").ToList ())
                idx.UnTie ();
            return retVal;
        }

        /// <summary>
        ///     Verifies node's value is an expression, and returns that expression to caller.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expressionNode"></param>
        /// <returns></returns>
        static public Expression DestinationExpression (ApplicationContext context, Node expressionNode)
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

        /// <summary>
        ///     Returns a node match object, optionally restricted to node type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expressionNode"></param>
        /// <returns></returns>
        static public Match DestinationMatch (
            ApplicationContext context,
            Node expressionNode,
            bool mustBeNodeTypeExpression = false)
        {
            var ex = DestinationExpression (context, expressionNode);
            var match = ex.Evaluate (context, expressionNode, expressionNode);

            // Checking if caller retricted type of expression, and if so, verifying it conforms.
            if (mustBeNodeTypeExpression && match.TypeOfMatch != Match.MatchType.node)
                throw new LambdaException (string.Format ("Destination for [{0}] was not a node type of expression", expressionNode.Name), expressionNode, context);

            // Success, returning match object to caller.
            return match;
        }

        /// <summary>
        ///     Returns one single value by evaluating evaluatedNode
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <typeparam name="T">The 1st type parameter</typeparam>
        public static T Single<T> (
            ApplicationContext context,
            Node evaluatedNode)
        {
            return Single<T> (context, evaluatedNode, evaluatedNode);
        }

        /// <summary>
        ///     Returns one single value by evaluating evaluatedNode
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="dataSource">Data source</param>
        /// <typeparam name="T">The 1st type parameter</typeparam>
        public static T Single<T> (
            ApplicationContext context,
            Node evaluatedNode,
            Node dataSource)
        {
            return Iterate<T> (context, evaluatedNode, dataSource).FirstOrDefault ();
        }

        /// <summary>
        ///     Iterates the given node, and returns multiple values
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <typeparam name="T">The 1st type parameter</typeparam>
        public static IEnumerable<T> Iterate<T> (ApplicationContext context, Node evaluatedNode)
        {
            return Iterate<T> (context, evaluatedNode, evaluatedNode);
        }

        /// <summary>
        ///     Iterates the given node, and returns multiple values
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="dataSource">Data source</param>
        /// <typeparam name="T">The 1st type parameter</typeparam>
        public static IEnumerable<T> Iterate<T> (
            ApplicationContext context,
            Node evaluatedNode,
            Node dataSource)
        {
            if (evaluatedNode.Value != null) {

                // Checking if node's value is an expression.
                if (evaluatedNode.Value is Expression) {

                    // We have an expression, creating a match object.
                    var match = (evaluatedNode.Value as Expression).Evaluate (context, dataSource, evaluatedNode);

                    // Checking type of match.
                    if (match.TypeOfMatch == Match.MatchType.count) {

                        // If expression is of type 'count', we return 'count', possibly triggering a conversion, returning count as type T, hence only iterating once.
                        yield return Utilities.Convert<T> (context, match.Count);
                        yield break;

                    }

                    // Expression was anything but 'count', we return it as type T, possibly triggering a conversion.
                    foreach (var idx in match) {
                        yield return Utilities.Convert<T> (context, idx.Value);
                    }
                } else {

                    // Returning single value created from value of node, but first applying formatting parameters, if there are any.
                    yield return Utilities.Convert<T> (context, FormatNode (context, evaluatedNode, dataSource));
                }
            } else {

                // Notice, since iteration of nodes might change children collection, we need "ToList" in all the versions below.
                if (typeof (T) == typeof (Node)) {

                    // Node's value is null, caller requests nodes, iterating through children of node, yielding results back to caller.
                    foreach (var idx in evaluatedNode.Children.ToList ()) {
                        yield return Utilities.Convert<T> (context, idx);
                    }
                } else if (typeof (T) == typeof (string)) {

                    // Caller requested string type of result, returning names of all children.
                    foreach (var idx in evaluatedNode.Children.ToList ()) {
                        yield return Utilities.Convert<T> (context, idx.Name);
                    }
                } else {

                    // Caller requests anything but node or string, iterating children, yielding values of children, converted to type back to caller.
                    foreach (var idx in evaluatedNode.Children.ToList ()) {
                        yield return idx.Get<T> (context);
                    }
                }
            }
        }

        /// <summary>
        ///     Returns source value for an Active Event.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public object Source (
            ApplicationContext context,
            Node args,
            params string [] avoidNodes)
        {
            // Finding source nodes.
            var srcNodes = args.Children.Where (ix => ix.Name != "" && !ix.Name.StartsWithEx (".") && !ix.Name.StartsWithEx ("_") && !avoidNodes.Contains (ix.Name)).ToList ();

            // Sanity check.
            if (srcNodes.Count > 1)
                throw new LambdaException ("Multiple source found for [" + args.Name + "]", args, context);
            if (srcNodes.Count == 0)
                return null;

            // Raising source Active Event, and returning results.
            context.RaiseEvent (srcNodes [0].Name, srcNodes [0]);

            // Sanity check
            if (srcNodes [0].Value == null && srcNodes [0].Count > 1)
                throw new LambdaException ("Source Active Event returned multiple source", args, context);

            // Returning value
            return srcNodes [0].Value ?? srcNodes [0].FirstChild;
        }

        /// <summary>
        ///     Returns the source nodes for an  Active Event invocation, which implies raising its children nodes as Active Events, and returning the result
        ///     as Nodes, avoiding using anything found in avoidNodes as source Active Events.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public List<Node> Sources (
            ApplicationContext context,
            Node args,
            params string [] avoidNodes)
        {
            // Finding source nodes.
            var srcNodes = args.Children.Where (ix => ix.Name != "" && !ix.Name.StartsWithEx (".") && !ix.Name.StartsWithEx ("_") && !avoidNodes.Contains (ix.Name)).ToList ();

            // Looping through all source nodes, invoking Active Events, and adding into return value.
            var retVal = new List<Node> ();
            foreach (var idxSrc in srcNodes) {

                // Raising source Active Event.
                context.RaiseEvent (idxSrc.Name, idxSrc);

                // We prioritize value if it exists after source Active Event invocation.
                if (idxSrc.Value != null) {
                    if (idxSrc.Value is Node)
                        retVal.Add (idxSrc.Value as Node);
                    else
                        retVal.AddRange (idxSrc.Get<Node> (context).Children);
                } else {
                    retVal.AddRange (idxSrc.Children);
                }
            }
            return retVal;
        }

        /// <summary>
        ///     Iterates a collection, and returns results as nodes back to caller.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="functor"></param>
        public static void Get (
            ApplicationContext context,
            Node args,
            GetDelegate functor)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (var argsRemover = new ArgsRemover (args, true)) {

                // Iterating through each key reqquested by caller.
                foreach (var idxKey in Iterate<string> (context, args)) {

                    // Checking if caller tries to access "protected storage key", which we do not allow, unless this is a native invocation, signified by that the caller invoked
                    // us with a "." as the name of the node.
                    if (!args.Name.StartsWithEx (".") && (idxKey.StartsWithEx ("_") || idxKey.StartsWithEx (".")))
                        throw new LambdaException (
                            string.Format ("Tried to access protected key in [{0}], key name was '{1}' ", args.Name, idxKey),
                            args,
                            context);

                    // Retrieving object by invoking delegate functor with key, and returning as child of args, if there is a value.
                    var value = functor (idxKey);
                    if (value != null) {

                        // Checking type of value, and acting accordingly.
                        if (value is Node)
                            args.Add (idxKey, null, (value as Node).Clone ());
                        else
                            args.Add (idxKey, value);
                    }
                }
            }
        }

        /// <summary>
        ///     Helper to set and update key collections.
        /// </summary>
        /// <param name="context">Application context object</param>
        /// <param name="args">Root node for updating collection</param>
        /// <param name="functor">Callback supplied by caller, that will be invoked once for each "key", with whatever value method 
        /// finds for specified key</param>
        /// <param name="exclusionArgs">Contains a list of node names that are excluded when looking for the "source" for values 
        /// for keys specified</param>
        public static void Set (
            ApplicationContext context,
            Node args,
            SetDelegate functor,
            params string [] exclusionArgs)
        {
            // Retrieving source.
            var source = Source (context, args);

            // Iterating through each result of expression.
            foreach (var idxKey in Iterate<string> (context, args)) {

                // Making sure collection key is not "hidden" key
                if (!args.Name.StartsWithEx (".") && (idxKey.StartsWithEx ("_") || idxKey.StartsWithEx (".")))
                    throw new LambdaException (
                        string.Format ("Tried to update protected key in [{0}], key name was '{1}' ", args.Name, idxKey),
                        args,
                        context);

                // Checking exclusion arguments.
                if (exclusionArgs.Any (ix => ix == idxKey))
                    continue;

                // Checking if this is deletion of item, or setting item, before invoking functor callback
                functor (idxKey, source);
            }
        }

        /// <summary>
        ///     Helper to list collection keys, used in combination with GetCollection and SetCollection.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="list"></param>
        public static void List (
            ApplicationContext context,
            Node args,
            IEnumerable list)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (args, true)) {

                // Retrieving filters, if any.
                var filter = new List<string> (Iterate<string> (context, args));

                // Looping through each existing key in collection, checking if it matches our filters, if there are any filters.
                foreach (string idxKey in list) {

                    // Making sure we do NOT return "protected keys", unless this is a protected invocation.
                    if (!args.Name.StartsWithEx (".") && (idxKey.StartsWithEx ("_") || idxKey.StartsWithEx (".")))
                        continue;

                    // Returning current key, if it matches our filter, or no filter is not given.
                    if (filter.Count == 0)
                        args.Add (idxKey);
                    else if (filter.Any (ix => ix.StartsWithEx ("~") ? idxKey.IndexOfEx (ix.Substring (1)) > -1 : ix == idxKey))
                        args.Add (idxKey);
                }
            }
        }

        /// <summary>
        ///     Raises a dynamically created Active Event or lambda object.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="eventNode">Arguments to pass into event</param>
        /// <param name="lambda">Lambda object to evaluate</param>
        public static void EvaluateLambda (
            ApplicationContext context,
            Node lambda,
            Node eventNode)
        {
            // Retrieving arguments, all of them.
            var allArgs = new List<Node> ();
            if (eventNode.Value != null)
                allArgs.AddRange (Iterate<object> (context, eventNode).Select (ix => new Node ("_arg", ix)));
            var args = eventNode.Children.Where (ix => ix.Name != "");
            allArgs.AddRange (args);

            // Inserting arguments, in addition to [offset] making sure no arguments are evaluated using "lambda injection".
            lambda.Insert (0, new Node ("offset", allArgs.Count));
            lambda.InsertRange (1, allArgs);

            // Evaluating lambda object now, by invoking [eval], which does the heavy lifting.
            context.RaiseEvent ("eval", lambda);

            // Making sure we return all nodes that was created during evaluation of event back to caller, in addition to value.
            // Notice Clear invocation, since eventNode still might contain formatting parameters.
            eventNode.Clear ().AddRange (lambda.Children);
            eventNode.Value = lambda.Value;
        }

        /*
         * Helper method to recursively format node's value
         */
        static object FormatNodeRecursively (
            ApplicationContext context,
            Node evaluatedNode,
            Node dataSource)
        {
            if (evaluatedNode.Value is Expression) {

                // Node is recursively formatted, and also an expression.
                // Formating node first, then evaluating expression.
                // PS, we cannot return null here, in case expression yields null
                return Single<object> (context, evaluatedNode, dataSource) ?? "";

            }
            return FormatNode (context, evaluatedNode, dataSource);
        }
    }
}
