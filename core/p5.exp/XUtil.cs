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
    ///     Helper class encapsulating common operations for p5.lambda expressions
    /// 
    ///     Contains helpers to iterate expressions, convert expressions to single values, and so on
    /// </summary>
    public static class XUtil
    {
        /// <summary>
        ///     Returns true if given object is an expression
        /// </summary>
        /// <returns><c>true</c> if is expression the specified value; otherwise, <c>false</c>.</returns>
        /// <param name="value">Value.</param>
        public static bool IsExpression (object value)
        {
            return value is Expression;
        }

        /// <summary>
        ///     Returns true if given node contains formatting parameters
        /// </summary>
        /// <returns><c>true</c> if is formatted the specified evaluatedNode; otherwise, <c>false</c>.</returns>
        /// <param name="evaluatedNode">Evaluated node.</param>
        public static bool IsFormatted (Node evaluatedNode)
        {
            // a formatted node is defined as having one or more children with string.Empty as name
            // and a value which is of type string
            return evaluatedNode.Value is string && 
                (evaluatedNode.Value as string).Contains ("{0}") && 
                evaluatedNode.FindAll (string.Empty).GetEnumerator ().MoveNext ();
        }

        /// <summary>
        ///     Formats the node according to values returned by its children
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="context">Context.</param>
        public static object FormatNode (
            Node evaluatedNode,
            ApplicationContext context)
        {
            return FormatNode (evaluatedNode, evaluatedNode, context);
        }

        /// <summary>
        ///     Formats the node according to values returned by its children
        /// 
        ///     Uses dataSource as source to evaluate expressions in formatting parameters, unless
        ///     dataSource is the same as evaluatedNode
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="context">Context.</param>
        public static object FormatNode (
            Node evaluatedNode,
            Node dataSource,
            ApplicationContext context)
        {
            // making sure node contains formatting values
            if (!IsFormatted (evaluatedNode))
                return evaluatedNode.Value;

            // retrieving all "formatting values"
            var childrenValues = new List<object> (evaluatedNode.ConvertChildren (
                delegate (Node idx) {

                    // we only use nodes who's names are empty as "formatting nodes"
                    if (idx.Name == string.Empty) {

                        // recursively format and evaluate expressions of children nodes
                        return FormatNodeRecursively (idx, dataSource == evaluatedNode ? idx : dataSource, context) ?? "";
                    }

                    // this is not a part of the formatting values for our formating expression,
                    // since it doesn't have an empty name, hence we return null, to signal to 
                    // ConvertChildren that this is to be excluded from list
                    return null;
            }));

            // returning node's value, after being formatted, according to its children node's values
            // PS, at this point all childrenValues have already been converted by the engine itself to string values
            return string.Format (CultureInfo.InvariantCulture, evaluatedNode.Value as string, childrenValues.ToArray ());
        }
        
        /*
         * helper method to recursively format node's value
         */
        private static object FormatNodeRecursively (
            Node evaluatedNode,
            Node dataSource,
            ApplicationContext context)
        {
            var isFormatted = IsFormatted (evaluatedNode);
            var isExpression = IsExpression (evaluatedNode.Value);

            if (isExpression) {

                // node is recursively formatted, and also an expression
                // formating node first, then evaluating expression
                // PS, we cannot return null here, in case expression yields null
                return Single<object> (evaluatedNode, dataSource, context);
            }
            if (isFormatted) {

                // node is formatted recursively, but not an expression
                return FormatNode (evaluatedNode, dataSource, context);
            }
            return evaluatedNode.Value ?? "";
        }

        /// <summary>
        ///     Returns one single value by evaluating evaluatedNode
        /// 
        ///     Will either evaluate node's expression, or iterate its children, value, and so on, and return
        ///     one single value of type (T) back to caller
        /// </summary>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="context">Context.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="inject">Inject.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Single<T> (
            Node evaluatedNode,
            ApplicationContext context,
            T defaultValue = default (T),
            string inject = null)
        {
            return Single<T> (evaluatedNode, evaluatedNode, context, defaultValue, inject);
        }

        /// <summary>
        ///     Returns one single value by evaluating evaluatedNode
        /// 
        ///     Will either evaluate node's expression, or iterate its children, value, and so on, and return
        ///     one single value of type (T) back to caller. Uses dataSource as source to evaluate any expressions
        ///     in evaluateddNode's value
        /// </summary>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="context">Context.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="inject">Inject.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Single<T> (
            Node evaluatedNode,
            Node dataSource,
            ApplicationContext context,
            T defaultValue = default (T),
            string inject = null)
        {
            object singleRetVal = null;
            string multipleRetVal = null;
            var firstRun = true;
            foreach (var idx in Iterate<T> (evaluatedNode, dataSource, context)) {

                // to make sure we never convert object to string, unless absolutely necessary
                if (firstRun) {

                    // first iteration of foreach loop
                    singleRetVal = idx;
                    firstRun = false;
                } else {

                    // second, third, or fourth, etc, iteration of foreach
                    // this means we will have to convert the iterated objects into string, concatenate the objects,
                    // before converting to type T afterwards
                    if (multipleRetVal == null) {

                        // second iteration of foreach
                        multipleRetVal = Utilities.Convert<string> (singleRetVal, context);
                    }
                    if (idx is Node || (singleRetVal is Node)) {

                        // current iteration contains a node, making sure we format our string nicely, such that
                        // the end result becomes valid hyperlisp, before trying to convert to type T afterwards
                        if (inject != "\r\n")
                            multipleRetVal += "\r\n";
                        singleRetVal = null;
                    }
                    if (inject != null)
                        multipleRetVal += inject;
                    multipleRetVal += Utilities.Convert<string> (idx, context);
                }
            }

            // if there was not multiple iterations above, we use our "singleRetVal" object, which never was
            // converted into a string
            return Utilities.Convert (multipleRetVal ?? singleRetVal, context, defaultValue);
        }

        /*
         * common implementation for Single<T> methods
         */
        /// <summary>
        ///     Iterates the given node, and returns multiple values
        /// 
        ///     If node contains an expression, then this expression will be evaluated. Otherwise, node's value
        ///     will be converted if possible. If no value in node, then children nodes of evaluates node will
        ///     be returned during iteration
        /// </summary>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="context">Context.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> Iterate<T> (
            Node evaluatedNode, 
            ApplicationContext context)
        {
            return Iterate<T> (evaluatedNode, evaluatedNode, context);
        }

        /// <summary>
        ///     Iterates the given node, and returns multiple values
        /// 
        ///     If node contains an expression, then this expression will be evaluated. Otherwise, node's value
        ///     will be converted if possible. If no value in node, then children nodes of evaluates node will
        ///     be returned during iteration. Uses dataSource to evaluate any expressions in evaluatedNode
        /// </summary>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="context">Context.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> Iterate<T> (
            Node evaluatedNode,
            Node dataSource,
            ApplicationContext context)
        {
            if (evaluatedNode.Value != null) {

                // checking if node's value is an expression
                if (IsExpression (evaluatedNode.Value)) {

                    // we have an expression in object form, making sure our expression iterator overload is invoked
                    // we have an expression, creating a match object
                    var match = (evaluatedNode.Value as Expression).Evaluate (dataSource, context, evaluatedNode);

                    // checking type of match
                    if (match.TypeOfMatch == Match.MatchType.count) {

                        // if expression is of type 'count', we return 'count', possibly triggering
                        // a conversion, returning count as type T, hence only iterating once
                        yield return Utilities.Convert<T> (match.Count, context);
                    } else {

                        // caller requested anything but 'count', we return it as type T, possibly triggering
                        // a conversion
                        foreach (var idx in match) {
                            yield return Utilities.Convert<T> (idx.Value, context);
                        }
                    }
                } else {

                    // returning single value created from value of node, but first applying formatting parameters, if there are any
                    yield return Utilities.Convert<T> (FormatNode (evaluatedNode, dataSource, context), context);
                }
            } else {

                if (typeof(T) == typeof(Node)) {

                    // node's value is null, caller requests nodes, 
                    // iterating through children of node, yielding results back to caller
                    foreach (var idx in evaluatedNode.Children) {
                        yield return Utilities.Convert<T> (idx, context);
                    }
                } else {

                    // caller requests anything but node, iterating children, yielding
                    // values of children, converted to type back to caller
                    foreach (var idx in evaluatedNode.Children) {
                        yield return idx.Get<T> (context);
                    }
                }
            }
        }

        /*
         * In use in [set] [p5.file.save] and [p5.data.update]
         */
        /// <summary>
        ///     Will return one single source value from evaluatedNode
        /// 
        ///     Useful for operations where you need one single [src] value, such as when saving a file, [set]'ing 
        ///     a node's value/name, etc.
        /// </summary>
        /// <returns>The single.</returns>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="context">Context.</param>
        public static object SourceSingle (Node evaluatedNode, ApplicationContext context)
        {
            return SourceSingle (evaluatedNode, evaluatedNode, context);
        }

        /// <summary>
        ///     Will return one single source value from evaluatedNode
        /// 
        ///     Useful for operations where you need one single [src] value, such as when saving a file, [set]'ing 
        ///     a node's value/name, etc. Uses dataSource to evaluate any expressions in evaluatedNode's value
        /// </summary>
        /// <returns>The single.</returns>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="context">Context.</param>
        public static object SourceSingle (
            Node evaluatedNode, 
            Node dataSource, 
            ApplicationContext context)
        {
            object source = null;

            if (evaluatedNode == null || evaluatedNode.Name == string.Empty)
                return null; // no source!

            if (evaluatedNode.Name != "src" && evaluatedNode.Name != "rel-src") {

                // Active Event invocation source, iterating through all destinations, after invocting Active 
                // event, updating with result from Active Event invocation
                context.Raise (evaluatedNode.Name, evaluatedNode);
                source = SourceImplementation (evaluatedNode, dataSource, context);
            } else {

                // simple source
                source = SourceImplementation (evaluatedNode, dataSource, context);
            }

            // returning source
            return source;
        }

        private static object SourceImplementation (Node evaluatedNode, Node dataSource, ApplicationContext context)
        {
            if (evaluatedNode.Value != null) {

                // this might be an expression, or a constant, converting value to single object, somehow
                return Single<object> (evaluatedNode, dataSource, context);
            } else {

                // there are no values in [src] node, trying to create source out of [src]'s children
                if (evaluatedNode.Count == 1) {

                    // source is a constant node, making sure we clone it, in case source and destination overlaps
                    return evaluatedNode.FirstChild.Clone ();
                } else {

                    // more than one source, making sure we convert it into one single value, meaning a 'string'
                    return Utilities.Convert<string> (evaluatedNode.Children, context);
                }
            }
        }

        /*
         * In use in [add], and to be used in [insert-before] and [insert-after]
         */
        /// <summary>
        ///    Will return multiple values if feasable
        /// 
        ///    Useful for operations such as [add] and [insert-before], that expects multiple values or a list of nodes
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="context">Context.</param>
        public static List<Node> SourceNodes (Node evaluatedNode, ApplicationContext context)
        {
            return SourceNodes (evaluatedNode, evaluatedNode, context);
        }

        /// <summary>
        ///     Will return multiple values if feasable
        /// 
        ///     Useful for operations such as [add] and [insert-before], that expects multiple values or a list of nodes.
        ///     Uses dataSource to evaluate any expressions in evaluatedNode
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="evaluatedNode">Evaluated node.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="context">Context.</param>
        public static List<Node> SourceNodes (Node evaluatedNode, Node dataSource, ApplicationContext context)
        {
            // return value
            var sourceNodes = new List<Node> ();

            // checking to see if we're given an expression
            if (IsExpression (evaluatedNode.Value)) {

                // [source] or [src] is an expression somehow
                foreach (var idx in evaluatedNode.Get<Expression> (context).Evaluate (dataSource, context, evaluatedNode)) {
                    if (idx.Value == null)
                        continue;
                    if (idx.TypeOfMatch != Match.MatchType.node && !(idx.Value is Node)) {

                        // [source] is an expression leading to something that's not a node, this
                        // will trigger conversion from string to node, adding a "root node" during
                        // conversion. we make sure we remove this node, when creating our source
                        sourceNodes.AddRange (Utilities.Convert<Node> (idx.Value, context).Children.Select (idxInner => idxInner.Clone ()));
                    } else {

                        // [source] is an expression, leading to something that's already a node somehow
                        var nodeValue = idx.Value as Node;
                        if (nodeValue != null)
                            sourceNodes.Add (nodeValue.Clone ());
                    }
                }
            } else {
                var nodeValue = evaluatedNode.Value as Node;
                if (nodeValue != null) {

                    // value of source is a node, adding this node
                    sourceNodes.Add (nodeValue.Clone ());
                } else if (evaluatedNode.Value is string) {

                    // source is not an expression, but a string value. this will trigger a conversion
                    // from string, to node, creating a "root node" during conversion. we are discarding this 
                    // "root" node, and only adding children of that automatically generated root node
                    sourceNodes.AddRange (Utilities.Convert<Node> (evaluatedNode.Value, context).Children.Select (idx => idx.Clone ()));
                } else if (evaluatedNode.Value == null) {

                    // source has no value, neither static string values, nor expressions
                    // adding all children of source node, if any
                    sourceNodes.AddRange (evaluatedNode.Children.Select (idx => idx.Clone ()));
                } else {

                    // source is not an expression, but has a non-string value. making sure we create a node
                    // out of that value, returning that node back to caller
                    sourceNodes.Add (new Node (string.Empty, evaluatedNode.Value));
                }
            }

            // returning node list back to caller
            return sourceNodes.Count > 0 ? sourceNodes : null;
        }
    }
}
