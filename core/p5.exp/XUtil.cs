/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
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
                (evaluatedNode.Value as string).Contains ("{") && 
                (evaluatedNode.Value as string).Contains ("}") && 
                evaluatedNode.Children.Count (ix => ix.Name == "") > 0;
        }

        /// <summary>
        ///     Helper method to invoke [src] node, or other types of sources, for Active Events that requires such
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="parent">Parent node expected to have a source node</param>
        /// <param name="destination">Current destination node</param>
        /// <returns></returns>
        public static List<object> InvokeSource (ApplicationContext context, Node parent, Node destination, List<string> excludeNodes = null)
        {
            // For simplicity, avoiding null reference exceptions inside of Linq later down
            if (excludeNodes == null)
                excludeNodes = new List<string> ();

            // Retrieving source nodes, making sure we do not add up "formatting nodes"
            var srcList = parent.Children.Where (ix => ix.Name != "" && !excludeNodes.Contains (ix.Name)).ToList ();

            // Sanity check, making sure there's only one or zero
            if (srcList.Count > 1) {

                // Oops, logical error
                throw new LambdaException ("Multiple sources defined for; " + parent.Name, parent, context);
            } else if (srcList.Count == 0) {

                // No source, making sure we never return null
                return new List<object> ();
            } else {

                // Active Event source invocation
                // Storing original children,  to make each invocation immutable, before we pass in "destination node"
                var originalParentNode = parent.Clone ();
                srcList[0].Insert (0, new Node ("_dn", destination ?? srcList[0]));

                // Raising source Active Event
                context.RaiseLambda (srcList[0].Name, srcList[0]);

                // Building up our return value(s)
                var retVal = new List<object> ();
                if (srcList[0].Value != null && srcList[0].Value is List<object>) {

                    // Value is a list of objects
                    retVal.AddRange (srcList[0].Value as List<object>);
                } else if (srcList[0].Value != null) {

                    // Adding value into return values
                    retVal.Add (srcList[0].Value);
                } else {

                    // Fallback to children
                    retVal.AddRange (srcList[0].Children.Where (ix => ix.Name != "_dn").Select (ix => ix.Clone ()));
                }

                // Making sure we reset original source node baack to what it was
                parent.Clear ().AddRange (originalParentNode.Children);

                // Returning list to caller
                return retVal;
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
            return Single<T> (context, evaluatedNode, evaluatedNode, mustHaveValue, defaultValue);
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
                        // the end result becomes valid hyperlisp, before trying to convert to type T afterwards
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
        ///     Will return one single source value from evaluatedNode
        /// </summary>
        /// <returns>The single</returns>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        public static object SourceSingle (
            ApplicationContext context,
            Node evaluatedNode)
        {
            return SourceSingle (context, evaluatedNode, evaluatedNode);
        }

        /// <summary>
        ///     Will return one single source value from evaluatedNode
        /// </summary>
        /// <returns>The single</returns>
        /// <param name="context">Context</param>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="dataSource">Data source</param>
        public static object SourceSingle (
            ApplicationContext context,
            Node evaluatedNode, 
            Node dataSource)
        {
            // Retrieving all children who's value is not "", as potential source nodes
            var srcNodes = evaluatedNode.Children.Where (ix => ix.Name != "").ToList ();

            // Returning early if there is no source
            if (srcNodes.Count == 0)
                return null; // no source!

            // Assuming first node with non-empty name is source node ...
            var srcNode = srcNodes [0];

            // Checking what type of source we have, it might be [src] or any Active Event invocation
            if (srcNode.Name == "src") {

                // Returning "simple source"
                // To support having sources which differs from evaluated lambda, such as [select-data] requires, 
                // we pass in dataSource if dataSource differs from evaluatedNode, otherwise we pass in source node 
                // itself as dataSource
                return SourceSingleImplementation (
                    context,
                    srcNode, 
                    dataSource == evaluatedNode ? srcNode : dataSource,
                    null);
            } else {

                // Active Event invocation source, invoking Active Event, return source as result from Active Event invocation.
                // Cloning Active Event source node, such that we can reset node after invocation, and invoke event immutable
                var origSrcNode = srcNode.Clone ();

                // Adding "Data Node" as argument to Active Event
                srcNode.Add ("_dn", dataSource);

                // Making sure source evaluation is immutable, regardless of whether or not exceptions occur!
                try {

                    // Raising Active Event given as source
                    context.RaiseLambda (srcNode.Name, srcNode);

                    // Returning result of Active Event invocation as source
                    return SourceSingleImplementation (context, srcNode, srcNode, null);
                } finally {

                    // Making sure we reset source node back to its original state, such that evaluation of Active Event becomes immutable
                    srcNode.Value = origSrcNode.Value;
                    srcNode.Name = origSrcNode.Name;
                    srcNode.Clear ().AddRange (origSrcNode.Children);
                }
            }
        }

        /*
         * Actual implementation of SourceSingle, runs after above methods
         */
        private static object SourceSingleImplementation (
            ApplicationContext context,
            Node evaluatedNode, 
            Node dataSource,
            Node formattingNode)
        {
            // Prioritizing value!
            if (evaluatedNode.Value != null) {

                // This might be an expression, or a constant, converting value to single object, somehow
                return Single<object> (context, evaluatedNode, dataSource, false, null, formattingNode);
            } else {

                // There is no value in [src] node, trying to create source out of [src]'s children
                if (evaluatedNode.Children.Count == 0) {

                    // "null source"
                    return null;
                } else if (evaluatedNode.Children.Count == 1) {

                    // Source is a constant node, making sure we clone it, before returning it
                    return evaluatedNode.FirstChild.Clone ();
                } else {

                    // More than one source, which is a logical error!
                    throw new LambdaException (
                        "Sorry, I cannot create a sane single source from given argument, there are multiple children, and no value", 
                        evaluatedNode, 
                        context);
                }
            }
        }
        /// <summary>
        ///     Raises a dynamically created Active Events or lambda objects
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Arguments to pass into event</param>
        /// <param name="lambda">Lambda object to evaluate</param>
        /// <param name="eventName">Name of Active Event to raise</param>
        public static void RaiseEvent (
            ApplicationContext context, 
            Node args, 
            Node lambda, 
            string eventName)
        {
            // Creating an "executable" lambda node, which becomes the node that is finally executed, after 
            // arguments is passed in and such. This is because we want the arguments at the TOP of the lambda node
            var exeLambda = new Node (eventName);

            // Adding up arguments, no need to clone, they should be gone after execution anyway.
            // But skipping all "empty name" arguments, since they're formatting parameters
            exeLambda.AddRange (args.Children.Where (ix => ix.Name != ""));

            // Applying "value argument"
            if (args.Value is Expression) {

                // Evaluating node, and stuffing results into arguments
                foreach (var idx in XUtil.Iterate<object> (context, args)) {

                    // Adding currently iterated results into execution object
                    var idxNode = idx as Node;
                    if (idxNode != null) {

                        // Appending node into execution object
                        exeLambda.Add ("_arg", null, new Node [] { idxNode.Clone () });
                    } else {

                        // Appending simple value into execution object with [_arg] name
                        exeLambda.Add ("_arg", idx);
                    }
                }
            } else if (args.Value != null) {

                // Simple value argument
                exeLambda.Add ("_arg", XUtil.FormatNode (context, args));
            }

            // Removing all empty nodes of args, to be sure we don't keeo garbage around
            // Notice, we do this AFTER evaluating expressions in value! To make sure formatting parameters
            // are kept around during evaluation!
            args.Clear ();

            // Then adding actual Active Event code, to make sure lambda event is at the END of entire node structure, after arguments
            exeLambda.AddRange (lambda.Children);

            // Storing lambda block and value, such that we can "diff" after execution, 
            // and only return the nodes added during execution, and the value if it was changed
            var oldLambdaNode = new List<Node> (exeLambda.Children);
            args.Value = null; // To make sure we don't return what we came in with!

            // Executing lambda children, and not evaluating any expression in evaluated node!
            context.RaiseLambda ("eval", exeLambda);

            // Making sure we return all nodes that was created during execution of event back to caller
            // in addition to value
            args.AddRange (exeLambda.Children.Where (ix => oldLambdaNode.IndexOf (ix) == -1));
            if (exeLambda.Value != null)
                args.Value = exeLambda.Value;
        }
    }
}
