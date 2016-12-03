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

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using p5.core;
using p5.exp.exceptions;

namespace p5.exp
{
    /// <summary>
    ///     Helper class encapsulating common operations for p5 lambda expressions
    /// </summary>
    public static class XUtil
    {
        /// <summary>
        ///     Delegate used for updating collections
        /// </summary>
        /// <param name="key">Key in collection</param>
        /// <param name="value">Value for item with specified key (can be null)</param>
        public delegate void SetCollectionDelegate (string key, object value);

        /// <summary>
        ///     Delegate used when retrieving collection values
        /// </summary>
        /// <param name="key">Key to retrieve from collection</param>
        /// <returns></returns>
        public delegate object GetCollectionDelegate (string key);

        /// <summary>
        ///     Returns true if given object is an expression
        /// </summary>
        /// <returns><c>true</c> if is expression the specified value; otherwise, <c>false</c></returns>
        /// <param name="value">Value</param>
        public static bool IsExpression (object value)
        {
            return value is Expression;
        }

        /// <summary>
        ///     Returns true if given node contains formatting parameters
        /// </summary>
        /// <returns><c>true</c> if is formatted the specified evaluatedNode; otherwise, <c>false</c></returns>
        /// <param name="evaluatedNode">Evaluated node</param>
        public static bool IsFormatted (Node evaluatedNode)
        {
            // A formatted node is defined as having one or more children with "" as name
            // and a value which is of type string
            return evaluatedNode.Value is string && 
                (evaluatedNode.Value as string).Contains ("{0}") && 
                evaluatedNode.Children.Count (ix => ix.Name == "") > 0;
        }

        /// <summary>
        ///     Helper method to invoke [src] node, or other types of sources, for Active Events that requires such
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="parent">Parent node expected to have a source node</param>
        /// <param name="destination">Current destination node</param>
        /// <param name="restrictionSrcName">Can be either "src" or "dest", and restricts the name of the source node to be either. 
        /// Has no effect on other Active Event sources, which it will still allow, regardless of its value</param>
        /// <param name="excludeNodes">Names of nodes to exclude when looking for source node.</param>
        /// <param name="sourceIsOffsetChild">When one Active Event can contain multiple "source nodes",
        /// this restricts the lookup process to being the n'th source node, instead of looking for simply the first</param>
        /// <returns></returns>
        public static List<object> Source (
            ApplicationContext context, 
            Node parent, 
            Node destination, 
            string restrictionSrcName = "src",
            List<string> excludeNodes = null,
            int sourceIsOffsetChild = 0)
        {
            // For simplicity, avoiding null reference exceptions inside of Linq later down.
            if (excludeNodes == null)
                excludeNodes = new List<string> ();

            // Retrieving source nodes, making sure we do not add up "formatting nodes".
            var srcList = parent.Children.Where (ix => ix.Name != "" && !excludeNodes.Contains (ix.Name)).ToList ();

            if (srcList.Count == 0) {

                // No source, making sure we never return null.
                return new List<object> ();

            } else {

                // Making sure we remove up until offset, for cases where the same Active Event contains multiple invocation nodes to this method.
                while (sourceIsOffsetChild > 0) {

                    srcList.RemoveAt (0);
                    sourceIsOffsetChild -= 1;
                }

                // Making sure we obey by the declared "restriction" for Active Event name. Either user should use "src" or "dest".
                // These two cannot be interchanged, to further clarify the language, such that no occurrency of "dest" is being used,
                // where the logic is actually source, vice versa.
                switch (srcList[0].Name) {
                    case "src":
                    case "dest":
                        if (restrictionSrcName != srcList[0].Name)
                            throw new LambdaException ("Sorry, you cannot use '" + srcList[0] + "' here.", parent, context);
                        break;
                }

                // Active Event source invocation.
                // Storing original children,  to make each invocation immutable, before we pass in "destination node".
                var originalParentNode = parent.Clone ();
                srcList[0].Insert (0, new Node ("_dn", destination ?? srcList[0]));

                // Raising source Active Event.
                context.Raise (srcList[0].Name, srcList[0]);

                // Building up our return value(s).
                var retVal = new List<object> ();
                if (srcList[0].Value != null) {

                    // Adding value into return values.
                    retVal.Add (srcList[0].Value);
                } else {

                    // Fallback to children.
                    retVal.AddRange (srcList[0].Children.Where (ix => ix.Name != "_dn").Select (ix => ix.Clone ()));
                }

                // Making sure we reset original source node baack to what it was.
                parent.Clear ().AddRange (originalParentNode.Children);

                // Returning list to caller.
                return retVal;
            }
        }

        /// <summary>
        ///     Common helper for iterating and updating a collection with new value(s)
        /// </summary>
        /// <param name="context">Application context object</param>
        /// <param name="args">Root node for updating collection</param>
        /// <param name="functor">Callback supplied by caller, that will be invoked once for each "key", with whatever value method 
        /// finds for specified key</param>
        /// <param name="isNative">If true, then this is a "native invocation", which allows for setting any object type into the session,
        /// at which the logic will change, and set the value "raw" into the collection, not iterating destinations etc. Only applicable if there
        /// are no expressions in args.Value, but only constants.</param>
        /// <param name="exclusionArgs">Contains a list of node names that are excluded when looking for the "source" for values 
        /// for keys specified</param>
        public static void SetCollection (
            ApplicationContext context, 
            Node args, 
            SetCollectionDelegate functor, 
            bool isNative,
            List<string> exclusionArgs = null)
        {
            // Iterating through each destinations, updating with source
            if (IsExpression (args.Value)) {

                // Expression destination (keys)
                foreach (var idxDestination in args.Get<Expression> (context).Evaluate (context, args, args)) {

                    // Figuring out source, possibly relative to destination
                    var source = Source (
                        context,
                        args,
                        idxDestination.Node,
                        "src",
                        exclusionArgs);

                    // Making sure there's only one source
                    if (source.Count > 1)
                        throw new LambdaException ("[" + args.Name + "]'s source returned multiple values", args, context);

                    // Retrieving key and value for cookie
                    var key = Utilities.Convert<string> (context, idxDestination.Value);

                    // Making sure collection key is not "hidden" key
                    if (!isNative && (key.StartsWith ("_") || key.StartsWith (".")))
                        throw new LambdaException ("Caller tried to access a protected collection key named; " + key, args, context);

                    // Checking if this is deletion of item, or setting item, before invoking functor callback
                    functor (key, source.Count == 0 ? null : source[0]);
                }
            } else {

                // Retrieving key and value for cookie
                var key = Single<string> (context, args);
                var source = Source (
                    context,
                    args,
                    args,
                    "src",
                    exclusionArgs);

                // Making sure there's only one source
                if (source.Count > 1)
                    throw new LambdaException ("[" + args.Name + "]'s source returned multiple values", args, context);

                // Checking if this is deletion of item, or setting item, before invoking functor callback
                functor (key, source.Count == 0 ? null : source[0]);
            }
        }

        /// <summary>
        ///     Iterates a collection, and returns as nodes back to caller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="functor"></param>
        public static void GetCollection (
            ApplicationContext context, 
            Node args, 
            GetCollectionDelegate functor,
            bool isNative)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (var argsRemover = new Utilities.ArgsRemover (args, true)) {

                // Iterating through each "key"
                foreach (var idxKey in Iterate<string> (context, args, true)) {

                    // Checking if caller tries to access "protected storage key"
                    if (!isNative && (idxKey.StartsWith ("_") || idxKey.StartsWith (".")))
                        throw new LambdaException ("Tried to access 'protected key' in " + args.Name, args, context);

                    // Retrieving object by invoking functor with key
                    var value = functor (idxKey);

                    // Adding node for given key, defaulting to null value
                    var resultNode = args.Add (idxKey).LastChild;

                    // Checking if value is not null, and if not, adding result, 
                    // according to what type of value we're given
                    if (value != null) {

                        // Adding key node, and value as object, if value is not node, otherwise
                        // appending value nodes beneath key node
                        if (value is Node) {

                            // Value is Node
                            resultNode.Add ((value as Node).Clone ());
                        } else if (value is IEnumerable<Node>) {

                            // Value is a bunch of nodes, adding them all
                            resultNode.AddRange ((value as IEnumerable<Node>).Select (ix => ix.Clone ()));
                        } else if (value is IEnumerable<object>) {

                            // Value is a bunch of object values, adding them all as values of children appended into args
                            resultNode.AddRange ((value as IEnumerable<object>).Select (ix => new Node ("", ix)));
                        } else {

                            // Value is any "other type of value", returning it "as is"
                            resultNode.Value = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Helper to list collection keys, used in association with GetCollection and SetCollection
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        /// <param name="list"></param>
        public static void ListCollection (ApplicationContext context, Node node, IEnumerable list, bool isNative)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (node, true)) {

                // Retrieving filters, if any
                var filter = new List<string> (Iterate<string> (context, node));

                // Looping through each existing key in collection
                foreach (string idxKey in list) {

                    // Making sure we do NOT return "protected keys"
                    if (!isNative && (idxKey.StartsWith ("_") || idxKey.StartsWith (".")))
                        continue;

                    // Returning current key, if it matches our filter, or filter is not given
                    if (filter.Count == 0) {

                        // No filter was given, returning everything
                        node.Add (idxKey);
                    } else {

                        // Filter was given, checking if key matches one of our filters
                        if (filter.Any (idxFilter => idxKey.IndexOf (idxFilter, StringComparison.Ordinal) != -1)) {
                            node.Add (idxKey);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Throws an exception if given args Node's value is null
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="activeEventName">Active event name</param>
        private static void AssertHasValue (
            ApplicationContext context, 
            Node args, 
            string activeEventName)
        {
            if (args == null || args.Value == null)
                throw new LambdaException (
                    string.Format ("No arguments supplied to [{0}]", activeEventName), 
                    args, 
                    context);
        }

        /// <summary>
        ///     Throws an exception if given args Node does not have children nodes
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="activeEventName">Active event name</param>
        private static void AssertHasChildren (
            ApplicationContext context, 
            Node args, 
            string activeEventName)
        {
            if (args.Children.Count == 0)
                throw new LambdaException (
                    string.Format ("No arguments supplied to [{0}]", activeEventName), 
                    args, 
                    context);
        }

        /// <summary>
        ///     Throws an exception if given args Node's value is null and args node has no children
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="activeEventName">Active event name</param>
        private static void AssertHasValueOrChildren (
            ApplicationContext context, 
            Node args, 
            string activeEventName)
        {
            if (args.Value == null && args.Children.Count == 0)
                throw new LambdaException (
                    string.Format ("No arguments or children nodes supplied to [{0}]", activeEventName), 
                    args, 
                    context);
        }

        /// <summary>
        ///     Formats the node according to values returned by its children
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
        ///     Formats the node according to values returned by its children
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
            // Making sure node contains formatting values
            if (!IsFormatted (evaluatedNode))
                return evaluatedNode.Value;

            var childrenValues = evaluatedNode.Children
                .Where (ix => ix.Name == "")
                .Select (ix => FormatNodeRecursively (context, ix, dataSource == evaluatedNode ? ix : dataSource) ?? "").ToArray();

            // Returning node's value, after being formatted, according to its children node's values
            // PS, at this point all childrenValues have already been converted by the engine itself to string values
            return string.Format (evaluatedNode.Value as string, childrenValues);
        }
        
        /*
         * Helper method to recursively format node's value
         */
        private static object FormatNodeRecursively (
            ApplicationContext context,
            Node evaluatedNode,
            Node dataSource)
        {
            var isFormatted = IsFormatted (evaluatedNode);
            var isExpression = IsExpression (evaluatedNode.Value);

            if (isExpression) {

                // Node is recursively formatted, and also an expression.
                // Formating node first, then evaluating expression.
                // PS, we cannot return null here, in case expression yields null
                return Single<object> (context, evaluatedNode, dataSource, false, "");
            }
            if (isFormatted) {

                // Node is formatted recursively, but not an expression
                return FormatNode (context, evaluatedNode, dataSource);
            }
            return evaluatedNode.Value ?? "";
        }

        /// <summary>
        ///     Returns one single value by evaluating evaluatedNode
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="defaultValue">Default value</param>
        /// <typeparam name="T">The 1st type parameter</typeparam>
        public static T Single<T> (
            ApplicationContext context,
            Node evaluatedNode,
            bool mustHaveValue = false,
            T defaultValue = default (T))
        {
            return Single (context, evaluatedNode, evaluatedNode, mustHaveValue, defaultValue);
        }

        /// <summary>
        ///     Returns one single value by evaluating evaluatedNode
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="dataSource">Data source</param>
        /// <param name="defaultValue">Default value</param>
        /// <typeparam name="T">The 1st type parameter</typeparam>
        public static T Single<T> (
            ApplicationContext context,
            Node evaluatedNode,
            Node dataSource,
            bool mustHaveValue = false,
            T defaultValue = default (T),
            Node formattingNode = null)
        {
            object singleRetVal = null;
            string multipleRetVal = null;
            var firstRun = true;
            foreach (var idx in Iterate<T> (context, evaluatedNode, dataSource, mustHaveValue, false, false, formattingNode)) {

                // To make sure we never convert object to string, unless absolutely necessary
                if (firstRun) {

                    // First iteration of foreach loop
                    singleRetVal = idx;
                    firstRun = false;
                } else {

                    // Second, third, or fourth, etc, iteration of foreach.
                    // This means we will have to convert the iterated objects into string, concatenate the objects,
                    // before converting to type T afterwards
                    if (multipleRetVal == null) {

                        // Second iteration of foreach
                        multipleRetVal = Utilities.Convert<string> (context, singleRetVal);
                    }
                    if (idx is Node || (singleRetVal is Node)) {

                        // Current iteration contains a node, making sure we format our string nicely, such that
                        // the end result becomes valid hyperlambda, before trying to convert to type T afterwards
                        multipleRetVal += "\r\n";
                        singleRetVal = null;
                    }
                    multipleRetVal += Utilities.Convert<string> (context, idx);
                }
            }

            // If there was not multiple iterations above, we use our "singleRetVal" object, which never was
            // converted into a string
            var retVal = Utilities.Convert (context, multipleRetVal ?? singleRetVal, defaultValue);

            // Returning result
            return retVal;
        }

        /// <summary>
        ///     Iterates the given node, and returns multiple values
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <typeparam name="T">The 1st type parameter</typeparam>
        public static IEnumerable<T> Iterate<T> (
            ApplicationContext context,
            Node evaluatedNode,
            bool mustHaveValue = false,
            bool mustHaveChildren = false,
            bool mustHaveValueOrChildren = false)
        {
            return Iterate<T> (context, evaluatedNode, evaluatedNode, mustHaveValue);
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
            Node dataSource,
            bool mustHaveValue = false,
            bool mustHaveChildren = false,
            bool mustHaveValueOrChildren = false,
            Node formattingNode = null)
        {
            // Checking if node must have a value, and running relevant assertion
            if (mustHaveValue)
                AssertHasValue (context, evaluatedNode, evaluatedNode.Name);

            // Checking if node must have children, and running the relevant assertion
            if (mustHaveChildren)
                AssertHasChildren (context, evaluatedNode, evaluatedNode.Name);

            // Checking if node must have children or value, and running the relevant assertion
            if (mustHaveValueOrChildren)
                AssertHasValueOrChildren (context, evaluatedNode, evaluatedNode.Name);

            if (evaluatedNode.Value != null) {

                // Checking if node's value is an expression
                if (IsExpression (evaluatedNode.Value)) {

                    // We have an expression, creating a match object
                    var match = (evaluatedNode.Value as Expression).Evaluate (context, dataSource, evaluatedNode, formattingNode);

                    // Checking type of match
                    if (match.TypeOfMatch == Match.MatchType.count) {

                        // If expression is of type 'count', we return 'count', possibly triggering
                        // a conversion, returning count as type T, hence only iterating once
                        yield return Utilities.Convert<T> (context, match.Count);
                        yield break;
                    } else {

                        // Caller requested anything but 'count', we return it as type T, possibly triggering
                        // a conversion
                        foreach (var idx in match) {
                            yield return Utilities.Convert<T> (context, idx.Value);
                        }
                    }
                } else {

                    // Returning single value created from value of node, but first applying formatting parameters, 
                    // if there are any
                    yield return Utilities.Convert<T> (context, FormatNode (context, evaluatedNode, dataSource));
                }
            } else {

                if (typeof(T) == typeof(Node)) {

                    // Node's value is null, caller requests nodes, iterating through children of node, 
                    // yielding results back to caller
                    foreach (var idx in new List<Node> (evaluatedNode.Children)) {
                        yield return Utilities.Convert<T> (context, idx);
                    }
                } else if (typeof(T) == typeof(string)) {

                    // Caller requests string, iterating children, yielding names of children
                    foreach (var idx in new List<Node> (evaluatedNode.Children)) {
                        yield return Utilities.Convert<T> (context, idx.Name);
                    }
                } else {

                    // Caller requests anything but node and string, iterating children, yielding
                    // values of children, converted to type back to caller
                    foreach (var idx in new List<Node> (evaluatedNode.Children)) {
                        yield return idx.Get<T> (context);
                    }
                }
            }
        }

        /// <summary>
        ///     Raises a dynamically created Active Event or lambda object.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="eventNode">Arguments to pass into event</param>
        /// <param name="lambda">Lambda object to evaluate</param>
        /// <param name="eventName">Name of Active Event to raise</param>
        public static void EvaluateLambda (
            ApplicationContext context,
            string eventName,
            Node lambda,
            Node eventNode)
        {
            // Adding up children arguments, no need to clone, they should be gone after execution anyway.
            // But skipping all "empty name" arguments, since they're formatting parameters.
            lambda.InsertRange (0, eventNode.Children.Where (ix => ix.Name != ""));

            // Applying "value arguments" last, meaning they'll end up first.
            lambda.InsertRange (0, Iterate<object> (context, eventNode).Select (ix => new Node ("_arg", ix)));

            // Evaluating lambda object now, by invoking [eval], which does the heavy lifting.
            context.Raise ("eval", lambda);

            // Making sure we return all nodes that was created during evaluation of event back to caller, in addition to value.
            // Notice Clear invocation, since eventNode still might contain formatting parameters.
            eventNode.Clear ().AddRange (lambda.Children);
            eventNode.Value = lambda.Value;
        }
    }
}
