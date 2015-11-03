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
    /// \todo Cleanup all of these comments, in addition to rethinking the name of the "dataSource" parameters, since they're highly unintuitive for the moment
    /// <summary>
    ///     Helper class for handling p5.lambda Expression objects.
    /// 
    ///     This is the class you'd normally use when consuming expressions. Contains many useful helper methods for
    ///     iterating expression result-sets, retrieve single compund values from expressions, etc.
    /// </summary>
    public static class XUtil
    {
        /// <summary>
        ///     Returns true if value is an Expression.
        /// 
        ///     If given value is an Expression, then this method will return true.
        /// </summary>
        /// <returns><c>true</c> if value is an Expression; otherwise, <c>false</c>.</returns>
        /// <param name="value">Value to check.</param>
        public static bool IsExpression (object value)
        {
            return value is Expression;
        }

        /// <summary>
        ///     Returns true if given node's value is formatted.
        /// 
        ///     A formatted value of a <see cref="phosphorus.code.Node">Node</see>, means that the node has at least one 
        ///     child node, with an empty name. If it does, then the node is assumed to be "formatted", meaning its value 
        ///     should not be interpreted in isolation, but be formatted according to the values of all children nodes, who's
        ///     names are string,Empty (""), using similar type of logic as can be found in for instance string.Format from C#.
        /// 
        ///     An example of a formatted node;
        /// 
        ///     <pre>
        /// foo:bar {0}
        ///   :some-value</pre>
        /// </summary>
        /// <returns><c>true</c> if node contains formatting parameters; otherwise, <c>false</c>.</returns>
        /// <param name="node">Node to check.</param>
        public static bool IsFormatted (Node node)
        {
            // a formatted node is defined as having one or more children with string.Empty as name
            // and a value which is of type string
            return node.Value is string && node.FindAll (string.Empty).GetEnumerator ().MoveNext ();
        }

        /// <summary>
        ///     Formats the given node, and returns the formatted value.
        /// 
        ///     Basically enumerates all children nodes of given node, and uses all child node with an empty name as
        ///     a formatting parameter, which combined yields the "true" value of the node.
        /// </summary>
        /// <returns>Formatted string value.</returns>
        /// <param name="node">Node containing formatting expression, and formatting children nodes.</param>
        /// <param name="context">Application context.</param>
        public static object FormatNode (
            Node node,
            ApplicationContext context,
            Node dataSource = null)
        {
            // making sure node contains formatting values
            if (!IsFormatted (node))
                return node.Value;

            // retrieving all "formatting values"
            var childrenValues = new List<object> (node.ConvertChildren (
                delegate (Node idx) {

                    // we only use nodes who's names are empty as "formatting nodes"
                    if (idx.Name == string.Empty) {
                        // recursively format and evaluate expressions of children nodes
                        return FormatNodeRecursively (idx, context, dataSource) ?? "";
                    }

                    // this is not a part of the formatting values for our formating expression,
                    // since it doesn't have an empty name, hence we return null, to signal to 
                    // ConvertChildren that this is to be excluded from list
                    return null;
            }));

            // returning node's value, after being formatted, according to its children node's values
            // PS, at this point all childrenValues have already been converted by the engine itself to string values
            return string.Format (CultureInfo.InvariantCulture, node.Get<string> (context), childrenValues.ToArray ());
        }

        /// <summary>
        ///     Returns a single value of type T from the constant or expression in node's value.
        /// 
        ///     If node's value is an expression, then expression will be evaluated, and result of expression converted to T. 
        ///     If expression yields multiple results, then the results will be concatenated into a string, in order
        ///     evaluated, before string is converted to T and returned.
        /// 
        ///     If expression returns one result, or node's value is a constant, then no conversion will be performed, 
        ///     unless necessary due to different types in expression's result or constant.
        /// 
        ///     If node contains formatting children, these will be evaluated as a formatting expression, before Expression 
        ///     is created, or constant is returned.
        /// </summary>
        /// <param name="node">Node who's value will be evaluated.</param>
        /// <param name="context">Application context.</param>
        /// <param name="defaultValue">Default value to return if expression or constant yields null.</param>
        /// <typeparam name="T">Type of object to convert expression or constant's value into and return back to caller.</typeparam>
        public static T Single<T> (
            Node evaluatedNode,
            ApplicationContext context,
            T defaultValue = default (T),
            string inject = null)
        {
            return SingleImplementation (() => Iterate<T> (evaluatedNode, evaluatedNode, context), context, defaultValue, inject);
        }

        /// <summary>
        ///     Returns a single value of type T from the constant or expression in node's value.
        /// 
        ///     If node's value is an expression, then expression will be evaluated, and result of expression converted to T. 
        ///     If expression yields multiple results, then the results will be concatenated into a string, in order
        ///     evaluated, before string is converted to T and returned.
        /// 
        ///     If expression returns one result, or node's value is a constant, then no conversion will be performed, 
        ///     unless necessary due to different types in expression's result or constant.
        /// 
        ///     If node contains formatting children, these will be evaluated as a formatting expression, before Expression 
        ///     is created, or constant is returned.
        /// </summary>
        /// <param name="node">Node who's value will be evaluated.</param>
        /// <param name="dataSource">Node that will be used as data source for any expressions within formatting 
        /// paramaters of node's value.</param>
        /// <param name="context">Application context.</param>
        /// <param name="defaultValue">Default value to return if expression or constant yields null.</param>
        /// <typeparam name="T">Type of object to convert expression or constant's value into and return back to caller.</typeparam>
        public static T Single<T> (
            Node evaluatedNode,
            Node dataSource,
            ApplicationContext context,
            T defaultValue = default (T),
            string inject = null,
            bool useDataSourceForFormatting = false)
        {
            return SingleImplementation (() => Iterate<T> (evaluatedNode, dataSource, context, false, useDataSourceForFormatting), context, defaultValue, inject);
        }

        /// <summary>
        ///     Returns a single value of type T from the constant or expression given.
        /// 
        ///     If value is an expression, then expression will be evaluated, and result of expression converted to T. 
        ///     If expression yields multiple results, then the results will be concatenated into a string, in order
        ///     evaluated, before string is converted to T and returned.
        /// 
        ///     If expression returns one result, or node's value is a constant, then no conversion will be performed, 
        ///     unless necessary due to different types in expression's result or constant.
        /// </summary>
        /// <param name="node">Node who's value will be evaluated.</param>
        /// <param name="dataSource">Node that will be used as data source if first parameter is an Expression.</param>
        /// <param name="context">Application context.</param>
        /// <param name="defaultValue">Default value to return if expression or constant yields null.</param>
        /// <typeparam name="T">Type of object to convert expression or constant's value into and return back to caller.</typeparam>
        public static T Single<T> (
            object expressionOrConstant,
            Node dataSource,
            ApplicationContext context,
            T defaultValue = default (T),
            string inject = null)
        {
            return SingleImplementation (() => Iterate<T> (expressionOrConstant, dataSource, context), context, defaultValue, inject);
        }
        
        /*
         * common implementation for Single<T> methods. requires a delegate responsible for returning
         * the IEnumerable that the method iterates over
         */
        private static T SingleImplementation<T> (
            SingleDelegate<T> functor,
            ApplicationContext context,
            T defaultValue,
            string inject = null)
        {
            object singleRetVal = null;
            string multipleRetVal = null;
            var firstRun = true;
            foreach (var idx in functor ()) {
                // hack, to make sure we never convert object to string, unless necessary
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

        /// <summary>
        ///     Iterates the given node's value, which might be either an expression or a constant.
        /// 
        ///     If node's value is a constant, then this constant will be converted if necessary to T, before returned.
        /// 
        ///     If node's value is an Expression, then this expression will be evaluated, and all results converted
        ///     to T, before returned to caller.
        /// 
        ///     Node's value can contain formatting parameters, which will be evaluated if existing. If node contains
        ///     formatting parameters, these will be evaluated before Expression is evaluated.
        /// </summary>
        /// <param name="node">Node who's value will be evaluated.</param>
        /// <param name="context">Application context.</param>
        /// <param name="iterateChildren">If true, then the children nodes of the evaluated node  will be iterated, 
        /// and not the actual node itself.</param>
        /// <typeparam name="T">Type of object you wish to retrieve.</typeparam>
        public static IEnumerable<T> Iterate<T> (
            Node evaluatedNode, 
            ApplicationContext context, 
            bool iterateChildren = false)
        {
            return Iterate<T> (evaluatedNode, evaluatedNode, context, iterateChildren);
        }

        /// <summary>
        ///     Iterates the given node's value, which might be either an expression or a constant.
        /// 
        ///     If node's value is a constant, then this constant will be converted if necessary to T, before returned.
        /// 
        ///     If node's value is an Expression, then this expression will be evaluated, and all results converted
        ///     to T, before returned to caller.
        /// 
        ///     Node's value can contain formatting parameters, which will be evaluated if existing. If node contains
        ///     formatting parameters, these will be evaluated before Expression is evaluated.
        /// </summary>
        /// <param name="node">Node who's value will be evaluated.</param>
        /// <param name="dataSource">Node to use as data source for any formatting expressions within formatting parameters.</param>
        /// <param name="context">Application context.</param>
        /// <param name="iterateChildren">If true, then the children nodes of the evaluated node  will be iterated, 
        /// and not the actual node itself.</param>
        /// <typeparam name="T">Type of object you wish to retrieve.</typeparam>
        public static IEnumerable<T> Iterate<T> (
            Node evaluatedNode,
            Node dataSource,
            ApplicationContext context,
            bool iterateChildren = false,
            bool useDataSourceForFormatting = false)
        {
            return evaluatedNode != null && dataSource != null && evaluatedNode.Value != null ? 
                (IsExpression (evaluatedNode.Value) ? 
                     Iterate<T> (evaluatedNode.Get<Expression> (context).Build (evaluatedNode, context), dataSource, context, iterateChildren) : 
                     Iterate<T> (FormatNode (evaluatedNode, context, useDataSourceForFormatting ? dataSource : null), dataSource, context, iterateChildren)) : 
                IterateChildren<T> (evaluatedNode, context);
        }

        /// <summary>
        ///     Iterates the given node's value, which might be either an expression or a constant.
        /// 
        ///     If node's value is a constant, then this constant will be converted if necessary to T, before returned.
        /// 
        ///     If node's value is an Expression, then this expression will be evaluated, and all results converted
        ///     to T, before returned to caller.
        /// 
        ///     Node's value can contain formatting parameters, which will be evaluated if existing. If node contains
        ///     formatting parameters, these will be evaluated before Expression is evaluated.
        /// </summary>
        /// <param name="expressionOrConstant">expression to run on dataSource, or constant object to iterate.</param>
        /// <param name="dataSource">Node to use as data source if expressionOrConstant given is an Expression.</param>
        /// <param name="context">Application context.</param>
        /// <param name="iterateChildren">If true, then the children nodes of the evaluated node  will be iterated, 
        /// and not the actual node itself.</param>
        /// <typeparam name="T">Type of object you wish to retrieve.</typeparam>
        public static IEnumerable<T> Iterate<T> (
            object expressionOrConstant,
            Node dataSource,
            ApplicationContext context,
            bool iterateChildren = false)
        {
            // ending early, if we're given nothing to iterate
            if (expressionOrConstant == null)
                return new T [] {};

            // checking if node's value is an expression
            if (IsExpression (expressionOrConstant)) {

                // we have an expression in object form, making sure our expression iterator overload is invoked
                return Iterate<T> (expressionOrConstant as Expression, dataSource, context, iterateChildren);
            }

            // checking to see if user requests "children of conversions"
            if (iterateChildren && typeof (T) == typeof (Node)) {
                // user requests to iterate children, therefor we
                // iterate the children of our constant node, instead of the node itself
                return (IEnumerable<T>) Utilities.Convert<Node> (expressionOrConstant, context).Children;
            }
            return new [] { Utilities.Convert<T> (expressionOrConstant, context) };
        }

        /// <summary>
        ///     Iterates the given Expression.
        /// 
        ///     This expression will be evaluated, on the given dataSource node, and all results converted to T, 
        ///     before returned to caller.
        /// </summary>
        /// <param name="expression">expression to run on dataSource</param>
        /// <param name="dataSource">Node to use as data source if expressionOrConstant given is an Expression.</param>
        /// <param name="context">Application context.</param>
        /// <param name="iterateChildren">If true, then the children nodes of the evaluated node  will be iterated, 
        /// and not the actual node itself.</param>
        /// <typeparam name="T">Type of object you wish to retrieve.</typeparam>
        public static IEnumerable<T> Iterate<T> (
            Expression expression,
            Node dataSource,
            ApplicationContext context,
            bool iterateChildren = false)
        {
            // we have an expression, creating a match object
            var match = expression.Evaluate (dataSource, context);

            // checking type of match
            if (match.TypeOfMatch == Match.MatchType.count) {

                // if expression is of type 'count', we return 'count', possibly triggering
                // a conversion, returning count as type T, hence only iterating once
                yield return Utilities.Convert<T> (match.Count, context);
            } else {

                // caller requested anything but 'count', we return it as type T, possibly triggering
                // a conversion
                foreach (var idx in match) {
                    if (iterateChildren && typeof (T) == typeof (Node)) {

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
        }

        /// <summary>
        ///     Retrieves the value of the [source], [rel-source], [src] or [rel-src] child node.
        /// 
        ///     Converts the result to type T. Returns null if no source exists. Does not care about whether or 
        ///     not there are multiple values, and will return a List if there are, though
        ///     will attempt to return only one value if it can, such as when there's a list containing only one value.
        /// 
        ///     Will only evaluate the last child node of the given node parameter, expecting its name to be [source], [rel-source],
        ///     [src] or [rel-src]. If the last child node of the given node parameter does not match the previous criteria, then
        ///     there will be no valid source, and method will return null.
        /// </summary>
        /// <param name="node">Node where [source], [rel-source], [rel-src] or [src] is expected to be a child node.</param>
        /// <param name="context">Application context.</param>
        public static object Source (Node evaluatedNode, ApplicationContext context)
        {
            return Source (evaluatedNode, evaluatedNode, context);
        }

        /// \todo refactor these next buggers, they're too complex
        /// <summary>
        ///     Retrieves the value of the [source], [rel-source], [src] or [re-src] child node.
        /// 
        ///     Converts the result to type T. Returns null if no source exists. Does not care about whether or 
        ///     not there are multiple values, and will return a List if there are, though
        ///     will attempt to return only one value if it can, such as when there's a list containing only one value.
        /// 
        ///     Will only evaluate the last child node of the given node parameter, expecting its name to be [source], [rel-source],
        ///     [src] or [rel-src]. If the last child node of the given node parameter does not match the previous criteria, then
        ///     there will be no valid source, and method will return null.
        /// </summary>
        /// <param name="node">Node where [source], [rel-source], [rel-src] or [src] is expected to be a child node.</param>
        /// <param name="dataSource">Node used as data source for expressions within the node parameter.</param>
        /// <param name="context">Application context.</param>
        private static object Source (Node evaluatedNode, Node dataSource, ApplicationContext context)
        {
            object source = null;

            // we have a [source] or [src] parameter here, figuring out what it points to, or contains
            if (IsExpression (evaluatedNode.Value)) {

                // this is an expression which might lead to multiple results, trying to return one result,
                // but will resort to returning List of objects if necssary
                var tmpList = new List<object> (Iterate<object> (evaluatedNode.Get<Expression> (context), dataSource, context));
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
                source = FormatNode (evaluatedNode, context);

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

        /// <summary>
        ///     Retrieves the value of [source], [rel-source], [src] or [rel-src] child node
        /// 
        ///     Will force one single return value, somehow. Returns null if no source exists. Used in among other things [set].
        /// </summary>
        /// <param name="node">Node where [source], [rel-source], [rel-src] or [src] is expected to be a child.</param>
        /// <param name="context">Application context.</param>
        public static object SourceSingle (Node evaluatedNode, ApplicationContext context)
        {
            return SourceSingle (evaluatedNode, evaluatedNode, context);
        }

        /// <summary>
        ///     Retrieves the value of [source], [rel-source], [src] or [rel-src] child node.
        /// 
        ///     Will force one single return value, somehow. Returns null if no source exists. Used in among other things [set].
        /// </summary>
        /// <param name="node">Node where [source], [rel-source], [rel-src] or [src] is expected to be a child.</param>
        /// <param name="dataSource">Node which will be used as data source node if node's parameter's value is an Expression.</param>
        /// <param name="context">Application context.</param>
        public static object SourceSingle (
            Node evaluatedNode, 
            Node dataSource, 
            ApplicationContext context, 
            bool useDataSourceForFormatting = false)
        {
            object source = null;

            // we have a [source] or [src] parameter here, figuring out what it points to, or contains
            if (evaluatedNode.Value != null) {

                // this might be an expression, or a constant, converting value to single object, somehow
                source = Single<object> (evaluatedNode, dataSource, context, null, null, useDataSourceForFormatting);
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

        /// <summary>
        ///     Retrieves the value of [source], [rel-source], [src] or [rel-src] child node.
        /// 
        ///     Might return multiple values. Returns null if no source exists. Used in among other things [add].
        /// </summary>
        /// <param name="node">Node where [source], [rel-source], [rel-src] or [src] is expected to be a child.</param>
        /// <param name="context">Application context.</param>
        public static List<Node> SourceNodes (Node evaluatedNode, ApplicationContext context)
        {
            return SourceNodes (evaluatedNode, evaluatedNode, context);
        }

        /// <summary>
        ///     Retrieves the value of [source], [rel-source], [src] or [rel-src] child node.
        /// 
        ///     Might return multiple values. Returns null if no source exists. Used in among other things [append].
        /// </summary>
        /// <param name="node">Node where [source], [rel-source], [rel-src] or [src] is expected to be a child.</param>
        /// <param name="dataSource">Node used as dataSource for formatting expressions within node parameter.</param>
        /// <param name="context">Application context.</param>
        public static List<Node> SourceNodes (Node node, Node dataSource, ApplicationContext context)
        {
            // return value
            var sourceNodes = new List<Node> ();

            // checking to see if we're given an expression
            if (IsExpression (node.Value)) {

                // [source] or [src] is an expression somehow
                foreach (var idx in node.Get<Expression> (context).Evaluate (dataSource, context, node)) {
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
                var nodeValue = node.Value as Node;
                if (nodeValue != null) {

                    // value of source is a node, adding this node
                    sourceNodes.Add (nodeValue.Clone ());
                } else if (node.Value is string) {

                    // source is not an expression, but a string value. this will trigger a conversion
                    // from string, to node, creating a "root node" during conversion. we are discarding this 
                    // "root" node, and only adding children of that automatically generated root node
                    sourceNodes.AddRange (Utilities.Convert<Node> (node.Value, context).Children.Select (idx => idx.Clone ()));
                } else if (node.Value == null) {

                    // source has no value, neither static string values, nor expressions
                    // adding all children of source node, if any
                    sourceNodes.AddRange (node.Children.Select (idx => idx.Clone ()));
                } else {

                    // source is not an expression, but has a non-string value. making sure we create a node
                    // out of that value, returning that node back to caller
                    sourceNodes.Add (new Node (string.Empty, node.Value));
                }
            }

            // returning node list back to caller
            return sourceNodes.Count > 0 ? sourceNodes : null;
        }

        /*
         * helper method to recursively format node's value
         */
        private static object FormatNodeRecursively (
            Node node,
            ApplicationContext context,
            Node dataSource = null)
        {
            var isFormatted = IsFormatted (node);
            var isExpression = IsExpression (node.Value);

            if (isExpression) {

                // node is recursively formatted, and also an expression
                // formating node first, then evaluating expression
                // PS, we cannot return null here, in case expression yields null
                return Single (node, dataSource ?? node, context, "", null, true);
            }
            if (isFormatted) {

                // node is formatted recursively, but not an expression
                return FormatNode (node, context, dataSource);
            }
            return node.Value ?? "";
        }

        /*
         * used internally when somehow requesting children nodes being iterated
         */
        private static IEnumerable<T> IterateChildren<T> (
            Node node,
            ApplicationContext context)
        {
            if (node != null) {
                if (typeof(T) == typeof(Node)) {

                    // node's value is null, caller requests nodes, 
                    // iterating through children of node, yielding results back to caller
                    foreach (var idx in node.Children) {
                        yield return Utilities.Convert<T> (idx, context);
                    }
                } else {

                    // caller requests anything but node, iterating children, yielding
                    // values of children, converted to type back to caller
                    foreach (var idx in node.Children) {
                        yield return idx.Get<T> (context);
                    }
                }
            }
        }

        // used to retrieve items in Single<T> methods
        private delegate IEnumerable<T> SingleDelegate<out T> ();
    }
}
