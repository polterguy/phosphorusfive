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
    public static class XUtil
    {
        public static bool IsExpression (object value)
        {
            return value is Expression;
        }

        public static bool IsFormatted (Node evaluatedNode)
        {
            // a formatted node is defined as having one or more children with string.Empty as name
            // and a value which is of type string
            return evaluatedNode.Value is string && 
                (evaluatedNode.Value as string).Contains ("{0}") && 
                evaluatedNode.FindAll (string.Empty).GetEnumerator ().MoveNext ();
        }

        public static object FormatNode (
            Node evaluatedNode,
            ApplicationContext context)
        {
            return FormatNode (evaluatedNode, evaluatedNode, context);
        }
        
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

        public static T Single<T> (
            Node evaluatedNode,
            ApplicationContext context,
            T defaultValue = default (T),
            string inject = null)
        {
            return Single<T> (evaluatedNode, evaluatedNode, context, defaultValue, inject);
        }

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
        public static IEnumerable<T> Iterate<T> (
            Node evaluatedNode, 
            ApplicationContext context, 
            bool iterateChildren = false)
        {
            return Iterate<T> (evaluatedNode, evaluatedNode, context, iterateChildren);
        }

        public static IEnumerable<T> Iterate<T> (
            Node evaluatedNode,
            Node dataSource,
            ApplicationContext context,
            bool iterateChildren = false)
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
                            if (iterateChildren && typeof(T) == typeof(Node)) {

                                // user requested to iterateChildren, and since current match triggers a conversion,
                                // we iterate the children of that conversion, and not the automatically generated
                                // root node
                                foreach (var idxInner in Utilities.Convert<Node> (idx.Value, context).Children) {
                                    yield return Utilities.Convert<T> (idxInner, context);
                                }
                            } else {
                                yield return Utilities.Convert<T> (idx.Value, context);
                            }
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

        public static object Source (Node evaluatedNode, ApplicationContext context)
        {
            return Source (evaluatedNode, evaluatedNode, context);
        }

        private static object Source (Node evaluatedNode, Node dataSource, ApplicationContext context)
        {
            object source = null;

            // we have a [source] or [src] parameter here, figuring out what it points to, or contains
            if (IsExpression (evaluatedNode.Value)) {

                // this is an expression which might lead to multiple results, trying to return one result,
                // but will resort to returning List of objects if necssary
                var tmpList = new List<object> (Iterate<object> (evaluatedNode, dataSource, context));
                switch (tmpList.Count) {
                    case 0:
                        // no source values
                        break;
                    case 1:
                        // one single object in list, returning only that single object
                        source = tmpList [0];
                        break;
                    default:
                        source = tmpList;
                        break;
                }
            } else if (evaluatedNode.Value != null) {

                // source is a constant, might still be formatted
                source = FormatNode (evaluatedNode, dataSource, context);

                if (source is Node)
                    source = (source as Node).Clone ();
            } else {

                // there are no value in [src] node, trying to create source out of [src]'s children
                if (evaluatedNode.Count == 1) {

                    // source is a constant node, making sure we clone it, in case source and destination overlaps
                    source = evaluatedNode.FirstChild.Clone ();
                } else {

                    // more than one source, making sure we clone them, before we return the clones
                    source = new List<Node> (evaluatedNode.Clone ().UnTieChildren ());
                }
            }

            // returning source
            return source;
        }

        public static object SourceSingle (Node evaluatedNode, ApplicationContext context)
        {
            return SourceSingle (evaluatedNode, evaluatedNode, context);
        }

        public static object SourceSingle (
            Node evaluatedNode, 
            Node dataSource, 
            ApplicationContext context)
        {
            object source = null;

            // we have a [source] or [src] parameter here, figuring out what it points to, or contains
            if (evaluatedNode.Value != null) {

                // this might be an expression, or a constant, converting value to single object, somehow
                source = Single<object> (evaluatedNode, dataSource, context);
            } else {

                // there are no values in [src] node, trying to create source out of [src]'s children
                if (evaluatedNode.Count == 1) {

                    // source is a constant node, making sure we clone it, in case source and destination overlaps
                    source = evaluatedNode.FirstChild.Clone ();
                } else {

                    // more than one source, making sure we convert it into one single value, meaning a 'string'
                    source = Utilities.Convert<string> (evaluatedNode.Children, context);
                }
            }

            // returning source
            return source;
        }

        public static List<Node> SourceNodes (Node evaluatedNode, ApplicationContext context)
        {
            return SourceNodes (evaluatedNode, evaluatedNode, context);
        }

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

        // used to retrieve items in Single<T> methods
        private delegate IEnumerable<T> SingleDelegate<out T> ();
    }
}
