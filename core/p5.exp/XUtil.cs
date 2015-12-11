/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using p5.core;
using p5.exp.exceptions;
using p5.exp.matchentities;

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
                (evaluatedNode.Value as string).Contains ("{0}") && 
                evaluatedNode.Children.Count (ix => ix.Name == "") > 0;
        }

        /// <summary>
        ///     Throws an exception if given args Node's value is null
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="activeEventName">Active event name</param>
        public static void AssertHasValue (
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
        ///     Throws an exception if given value is null
        /// </summary>
        /// <param name="args">Argument</param>
        /// <param name="activeEventName">Active event name</param>
        public static void AssertHasValue (
            ApplicationContext context, 
            Node args, 
            object arg, 
            string activeEventName)
        {
            if (arg == null)
                throw new LambdaException (
                    string.Format ("No arguments supplied to [{0}], possibly expression leading into oblivion", activeEventName), 
                    args, 
                    context);
        }

        /// <summary>
        ///     Throws an exception if given args Node does not have children nodes
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="activeEventName">Active event name</param>
        public static void AssertHasChildren (
            ApplicationContext context, 
            Node args, 
            string activeEventName)
        {
            if (args.Count == 0)
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
        public static void AssertHasValueOrChildren (
            ApplicationContext context, 
            Node args, 
            string activeEventName)
        {
            if (args.Value == null && args.Count == 0)
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
            T defaultValue = default (T))
        {
            // Checking if node must have a value, and if so, running our assert
            if (mustHaveValue)
                AssertHasValue (context, evaluatedNode, evaluatedNode.Name);

            object singleRetVal = null;
            string multipleRetVal = null;
            var firstRun = true;
            foreach (var idx in Iterate<T> (context, evaluatedNode, dataSource)) {

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
            bool mustHaveValueOrChildren = false)
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
                    var match = (evaluatedNode.Value as Expression).Evaluate (context, dataSource, evaluatedNode);

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
            // Finding first [src] or [rel-src] node
            var firstSourceNode = evaluatedNode.Children.FirstOrDefault (ix => ix.Name != "");

            // Returning early if there is no source
            if (firstSourceNode == null)
                return null; // no source!

            // Checking what type of source we have, it might be [src], [rel-src] or any Active Event
            if (firstSourceNode.Name != "src" && firstSourceNode.Name != "rel-src") {

                // Active Event invocation source, iterating through all source events, invoking Active 
                // event, updating with result from Active Event invocation.
                // Logic here is that if value of executed node changes, and is not null, then value has presedence,
                // otherwise children nodes of executed node will be used as source.
                // Note, if ONE of the executed Active Events returns something as "value", then no nodes
                // returned from Active Events as children will be even evaluated as source candidates
                Node tmpRetVal = new Node ();
                foreach (var idxSrcNode in evaluatedNode.Children.Where (idx => idx.Name != "")) {

                    // Raising currently iterated Active Event source, but storing value to see if it changes
                    var oldValue = idxSrcNode.Value;
                    context.RaiseLambda (idxSrcNode.Name, idxSrcNode);

                    // Checking if we should use value or children as source, value has presedence, 
                    // but only if it exist, and has changed!
                    if ((oldValue == null && idxSrcNode.Value != null) || 
                        (oldValue != null && idxSrcNode.Value != null && !oldValue.Equals (idxSrcNode.Value))) {

                        // Value has presedence
                        if (tmpRetVal.Value == null)
                            tmpRetVal.Value = idxSrcNode.Value;
                        else
                            tmpRetVal.Value = tmpRetVal.Get<string> (context) + idxSrcNode.Value; // Concatenating as strings
                        tmpRetVal.Clear (); // Dropping all other potential candidates!
                    } else {

                        // Children nodes are used
                        tmpRetVal.AddRange (idxSrcNode.Clone ().Children);
                    }
                }
                return SourceSingleImplementation (context, tmpRetVal, tmpRetVal);
            } else {

                // Simple source, there might still exist several [src] or [rel-src] children, 
                // making sure we use them all, and making sure [src] and [rel-src] is not mixed!
                string srcType = null;

                // Used as return value
                Node retVal = new Node ();

                // Iterating through each [src] and/or [rel-src], verify that they're never mixed in same operation
                foreach (var idxSource in evaluatedNode.Children.Where (ix => ix.Name == "src" || ix.Name == "rel-src")) {

                    // Making sure [src] and [rel-src] is never mixed
                    if (srcType == null)
                        srcType = idxSource.Name;
                    else if (srcType != idxSource.Name)
                        throw new LambdaException (
                            "You cannot mix [src] with [rel-src] for the same operation", 
                            evaluatedNode, 
                            context);

                    // Retrieving surrently iterated source
                    var curSource = SourceSingleImplementation (
                        context,
                        idxSource, 
                        dataSource == evaluatedNode ? idxSource : dataSource);

                    // Appending to retval
                    retVal.Add ("", curSource);
                }

                // Making sure we return one single source object
                return Single<object> (context, retVal);
            }
        }

        /*
         * Actual implementation of SourceSingle, runs after above methods
         */
        private static object SourceSingleImplementation (
            ApplicationContext context,
            Node evaluatedNode, 
            Node dataSource)
        {
            if (evaluatedNode.Value != null) {

                // This might be an expression, or a constant, converting value to single object, somehow
                return Single<object> (context, evaluatedNode, dataSource);
            } else {

                // There are no values in [src] node, trying to create source out of [src]'s children
                if (evaluatedNode.Count == 1) {

                    // Source is a constant node, making sure we clone it, in case source and destination overlaps
                    return evaluatedNode.FirstChild.Clone ();
                } else {

                    // More than one source, returning "wrapper node", since we need a single source
                    return evaluatedNode.Clone ();
                }
            }
        }

        /// <summary>
        ///    Will return multiple values if feasable
        /// </summary>
        /// <returns>The nodes</returns>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="context">Context</param>
        public static List<Node> SourceNodes (
            ApplicationContext context,
            Node evaluatedNode)
        {
            return SourceNodes (context, evaluatedNode, evaluatedNode);
        }

        /// <summary>
        ///     Will return multiple values if possible
        /// </summary>
        /// <returns>The nodes</returns>
        /// <param name="evaluatedNode">Evaluated node</param>
        /// <param name="dataSource">Data source</param>
        /// <param name="context">Context</param>
        public static List<Node> SourceNodes (
            ApplicationContext context,
            Node evaluatedNode, 
            Node dataSource)
        {
            // Finding first [src] or [rel-src] node
            var firstSourceNode = evaluatedNode.Children.FirstOrDefault (ix => ix.Name != "");

            // Returning early if there is no source
            if (firstSourceNode == null)
                return null; // no source!

            // Checking what type of source we have, it might be [src], [rel-src] or any Active Event
            if (firstSourceNode.Name != "src" && firstSourceNode.Name != "rel-src") {

                // Active Event invocation source, iterating through all source events, invoking Active 
                // event, updating with result from Active Event invocation.
                // Logic here is that if value of executed node changes, and is not null, then value has presedence,
                // otherwise children nodes of executed node will be used as source.
                // Note, if ONE of the executed Active Events returns something as "value", then no nodes
                // returned from Active Events as children will be even evaluated as source candidates
                Node tmpRetVal = new Node ();
                foreach (var idxSrcNode in evaluatedNode.Children.Where (idx => idx.Name != "")) {

                    // Raising currently iterated Active Event source, but storing value to see if it changes
                    var oldValue = idxSrcNode.Value;
                    context.RaiseLambda (idxSrcNode.Name, idxSrcNode);

                    // Checking if we should use value or children as source, value has presedence, 
                    // but only if it exist, and has changed!
                    if ((oldValue == null && idxSrcNode.Value != null) || 
                        (oldValue != null && idxSrcNode.Value != null && !oldValue.Equals (idxSrcNode.Value))) {

                        // Value has presedence
                        if (idxSrcNode.Value is Node) {

                            // Value is node
                            tmpRetVal.Add (idxSrcNode.Get<Node> (context).Clone ());
                        } else {

                            // Value is NOT node, converting to node list before adding, but adding CHILDREN since conversion creates a "wrapper node"
                            tmpRetVal.AddRange (idxSrcNode.Get<Node> (context).Children);
                        }
                    } else {

                        // Children nodes are used, since there was no Value in Active Event invocation node after 
                        // invocation, or value was never changed!
                        tmpRetVal.AddRange (idxSrcNode.Clone ().Children);
                    }
                }
                return SourceNodesImplementation (context, tmpRetVal, tmpRetVal);
            } else {

                // Simple source, either [src] or [rel-src], but there might still exist multiple [src] or [rel-src] nodes!
                var retVal = new List<Node> ();

                // Iterating through each [src] or [rel-src] nodes, making sure there's only [src] sources or only [rel-src] sources
                // since these cannot be mixed!
                string srcType = null;
                foreach (var idxSource in evaluatedNode.Children.Where (ix => ix.Name == "src" || ix.Name == "rel-src")) {

                    // Basic syntax checking, to avoid mixing [rel-src] and [src]
                    if (srcType == null)
                        srcType = idxSource.Name;
                    else if (srcType != idxSource.Name)
                        throw new LambdaException ("You cannot mix [src] and [rel-src] in same operation", evaluatedNode, context);

                    // Getting currently iterated source
                    var curSrc = SourceNodesImplementation (
                        context,
                        idxSource, 
                        dataSource == evaluatedNode ? idxSource : dataSource);

                    // Appending currently iterated source into return value, but verify that there actually IS a source first!
                    if (curSrc != null)
                        retVal.AddRange (curSrc);
                }

                // Returning source
                return retVal;
            }
        }

        /*
         * Common implementation for SourceNodes
         */
        private static List<Node> SourceNodesImplementation (
            ApplicationContext context,
            Node evaluatedNode, 
            Node dataSource)
        {
            var sourceNodes = new List<Node> ();

            // Checking to see if we're given an expression
            if (IsExpression (evaluatedNode.Value)) {

                // [src] is an expression somehow
                foreach (var idx in evaluatedNode.Get<Expression> (context).Evaluate (context, dataSource, evaluatedNode)) {
                    if (idx.Value == null)
                        continue;
                    if (idx.TypeOfMatch != Match.MatchType.node && !(idx.Value is Node)) {

                        // [src] is an expression leading to something that's not a node.
                        // This will trigger conversion from string to node, adding a "root node" during conversion. 
                        // We make sure we remove this node, when creating our source
                        sourceNodes.AddRange (Utilities.Convert<Node> (context, idx.Value).Children.Select (idxInner => idxInner.Clone ()));
                    } else {

                        // [src] is an expression, leading to something that's already a node somehow
                        var nodeValue = idx.Value as Node;
                        if (nodeValue != null)
                            sourceNodes.Add (nodeValue.Clone ());
                    }
                }
            } else {
                var nodeValue = evaluatedNode.Value as Node;
                if (nodeValue != null) {

                    // Value of source is a node, adding this node
                    sourceNodes.Add (nodeValue.Clone ());
                } else if (evaluatedNode.Value is string) {

                    // Source is not an expression, but a string value. This will trigger a conversion
                    // from string, to node, creating a "root node" during conversion. We are discarding this 
                    // "root" node, and only adding children of that automatically generated root node
                    sourceNodes.AddRange (Utilities.Convert<Node> (context, FormatNode(context, evaluatedNode)).Children.Select (idx => idx.Clone ()));
                } else if (evaluatedNode.Value == null) {

                    // Source has no value, neither static string values, nor expressions.
                    // Adding all children of source node, if any
                    sourceNodes.AddRange (evaluatedNode.Children.Select (idx => idx.Clone ()));
                } else {

                    // Source is not an expression, but has a non-string value. Making sure we create a node
                    // out of that value, returning that node back to caller
                    sourceNodes.Add (new Node ("", evaluatedNode.Value));
                }
            }

            // Returning node list back to caller
            return sourceNodes.Count > 0 ? sourceNodes : null;
        }

        /// <summary>
        ///     Raises a dynamically created Active Event
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Arguments to pass into event</param>
        /// <param name="lambda">Lambda object to evaluate</param>
        /// <param name="eventName">Name of Active Event to raise</param>
        public static void RaiseEvent (ApplicationContext context, Node args, Node lambda, string eventName)
        {
            // Creating an "executable" lambda node, which becomes the node that is finally executed, after 
            // arguments is passed in and such. This is because we want the arguments at the TOP of the lambda node
            var exeLambda = new Node (eventName);

            // Adding up arguments, no need to clone, they should be gone after execution anyway.
            // But skipping all "empty name" arguments, since they're formatting parameters
            exeLambda.AddRange (args.Children.Where (ix => ix.Name != ""));

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

            // Then adding actual Active Event code, to make sure lambda event is at the END of entire node structure, after arguments
            exeLambda.AddRange (lambda.Clone ().Children);

            // Storing lambda block and value, such that we can "diff" after execution, 
            // and only return the nodes added during execution, and the value if it was changed
            var oldLambdaNode = new List<Node> (exeLambda.Children);
            args.Value = null; // To make sure we don't return what we came in with!

            // Executing lambda children, and not evaluating any expression in evaluated node!
            context.RaiseLambda ("eval-mutable", exeLambda);

            // Making sure we return all nodes that was created during execution of event back to caller
            // in addition to value, but only if it was changed
            args.Clear ().AddRange (exeLambda.Children.Where (ix => oldLambdaNode.IndexOf (ix) == -1));
            if (exeLambda.Value != null)
                args.Value = exeLambda.Value;
        }
    }
}
