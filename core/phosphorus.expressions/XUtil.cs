
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions
{
    /// <summary>
    /// contains useful helper methods for dealing with pf.lambda expressions
    /// </summary>
    public static class XUtil
    {
        /// <summary>
        /// returns true if value is an expression
        /// </summary>
        /// <returns><c>true</c> if value is an expression; otherwise, <c>false</c></returns>
        /// <param name="value">value to check</param>
        public static bool IsExpression (object value)
        {
            if (value == null)
                return false;
            return IsExpression (value as string);
        }

        /// <summary>
        /// returns true if value is an expression
        /// </summary>
        /// <returns><c>true</c> if value is an expression; otherwise, <c>false</c></returns>
        /// <param name="value">value to check</param>
        public static bool IsExpression (string value)
        {
            return value != null && 
                value.StartsWith ("@") && 
                value.Length >= 4; // "@{0}" is the shortest possible expression, and has 4 characters
        }

        /// <summary>
        /// returns true if given node contains formatting parameters
        /// </summary>
        /// <returns><c>true</c> if node contains formatting parameters; otherwise, <c>false</c></returns>
        /// <param name="node">node to check</param>
        public static bool IsFormatted (Node node)
        {
            // a formatted node is defined as having one or more children with string.Empty as name
            // and a value which is of type string
            return node.Value is string && node.FindAll (string.Empty).GetEnumerator ().MoveNext ();
        }

        /// <summary>
        /// formats the node's value as a string.Format expression, using each child node
        /// with a string.Empty name as indexed formatting parameters
        /// </summary>
        /// <returns>formatted string</returns>
        /// <param name="node">node containing formatting expression and formatting children nodes</param>
        /// <param name="context">application context</param>
        public static string FormatNode (Node node, ApplicationContext context)
        {
            return FormatNode (node, node, context);
        }

        /// <summary>
        /// formats the node's value as a string.Format expression, using each child node
        /// with a string.Empty name as indexed formatting parameters, using dataSource node
        /// as the root node for any expressions within node's formatting children values
        /// </summary>
        /// <returns>formatted string</returns>
        /// <param name="node">node containing formatting expression and formatting children nodes</param>
        /// <param name="dataSource">node to use as dataSource for any expressions within formatting parameters</param>
        /// <param name="context">application context</param>
        public static string FormatNode (Node node, Node dataSource, ApplicationContext context)
        {
            // making sure node contains formatting values
            if (!IsFormatted (node))
                throw new ArgumentException ("cannot format node, no formatting nodes exists, or node's value is not a string");

            // retrieving all "formatting values"
            List<string> childrenValues = new List<string> (node.ConvertChildren<string> (
            delegate (Node idx) {

                // we only use nodes who's names are empty as "formatting nodes"
                if (idx.Name == string.Empty) {

                    // recursively format and evaluate expressions of children nodes
                    return FormatNodeRecursively (idx, dataSource == node ? idx : dataSource, context) ?? "";
                } else {

                    // this is not a part of the formatting values for our formating expression
                    return null;
                }
            }));

            // returning node's value, after being formatted, according to its children node's values
            // PS, at this point all childrenValues have already been converted by the engine itself to string values
            return string.Format (CultureInfo.InvariantCulture, node.Get<string> (context), childrenValues.ToArray ());
        }

        /// <summary>
        /// returns a single value of type T from the constant or expression in node's value. if node's value
        /// is an expression, then expression will be evaluated, and result of expression converted to T. if
        /// expression yields multiple results, then the results will be concatenated into a string, in order
        /// evaluated, before string is converted to T and returned. if expression returns one result, or
        /// node's value is a constant, then no conversion will be performed, unless necessary due to different
        /// types in expression's result or constant. if node contains formatting children, these will be
        /// evaluated as a formatting expression before expression is created, or constant is returned
        /// </summary>
        /// <param name="node">node who's value will be evaluated</param>
        /// <param name="context">application context</param>
        /// <param name="defaultValue">default value to return if expression or constant yields null</param>
        /// <typeparam name="T">type of object to return</typeparam>
        public static T Single<T> (Node node, ApplicationContext context, T defaultValue = default (T))
        {
            return Single<T> (node, node, context, defaultValue);
        }

        /// <summary>
        /// returns a single value of type T from the constant or expression in node's value. if node's value
        /// is an expression, then expression will be evaluated, and result of expression converted to T. if
        /// expression yields multiple results, then the results will be concatenated into a string, in order
        /// evaluated, before string is converted to T and returned. if expression returns one result, or
        /// node's value is a constant, then no conversion will be performed, unless necessary due to different
        /// types in expression's result or constant. if node contains formatting children, these will be
        /// evaluated as a formatting expression before expression is created, or constant is returned
        /// </summary>
        /// <param name="node">node who's value will be evaluated</param>
        /// <param name="dataSource">node that will be used as data source for any expressions within formatting
        /// paramaters of node's value</param>
        /// <param name="context">application context</param>
        /// <param name="defaultValue">default value to return if expression or constant yields null</param>
        /// <typeparam name="T">type of object to return</typeparam>
        public static T Single<T> (Node node, Node dataSource, ApplicationContext context, T defaultValue = default (T))
        {
            object singleRetVal = null;
            string multipleRetVal = null;
            foreach (var idx in Iterate<T> (node, dataSource, context)) {

                // hack to make sure we never convert object to string unless necessary
                if (singleRetVal == null) {
                    singleRetVal = idx;
                } else {
                    if (multipleRetVal == null)
                        multipleRetVal = Utilities.Convert<string> (singleRetVal, context);
                    multipleRetVal += Utilities.Convert<string> (idx, context);
                }
            }

            // making sure we never convert results unless necessary
            if (multipleRetVal == null)
                return Utilities.Convert<T> (singleRetVal, context, defaultValue);

            // there were multiple return values, hence we'll need to use conversion
            return Utilities.Convert<T> (multipleRetVal, context, defaultValue);
        }

        /// <summary>
        /// returns a single value of type T from the result of the expression given. if
        /// expression yields multiple results, then the results will be concatenated into a string, in order
        /// evaluated, before string is converted to T and returned. if expression returns one result, or
        /// node's value is a constant, then no conversion will be performed, unless necessary due to different
        /// types in expression's result or constant. if node contains formatting children, these will be
        /// evaluated as a formatting expression before expression is created, or constant is returned
        /// </summary>
        /// <param name="expression">expression to evaluate</param>
        /// <param name="dataSource">node to use as start node for expression</param>
        /// <param name="context">application context</param>
        /// <param name="defaultValue">default value to return if expression or constant yields null</param>
        /// <typeparam name="T">type of object to return</typeparam>
        public static T Single<T> (
            string expression, 
            Node dataSource, 
            ApplicationContext context, 
            T defaultValue = default (T))
        {
            object singleRetVal = null;
            string multipleRetVal = null;
            foreach (var idx in Iterate<T> (expression, dataSource, context)) {

                // hack to make sure we never convert object to string unless necessary
                if (singleRetVal == null) {
                    singleRetVal = idx;
                } else {
                    if (multipleRetVal == null)
                        multipleRetVal = Utilities.Convert<string> (singleRetVal, context);
                    multipleRetVal += Utilities.Convert<string> (idx, context);
                }
            }

            // making sure we never convert results unless necessary
            if (multipleRetVal == null)
                return Utilities.Convert<T> (singleRetVal, context, defaultValue);

            // there were multiple return values, hence we'll need to use conversion
            return Utilities.Convert<T> (multipleRetVal, context, defaultValue);
        }

        /// <summary>
        /// iterates the given node's value, which might be either an expression or a constant. if node's
        /// value is a constant, then this constant will be converted if necessary to T before returned. if
        /// node's value is an expression, then this expression will be evaluated, and all results converted
        /// to T before returned to caller. node's value can contain formatting parameters, which will be
        /// evaluated if existing. if node contains formatting parameters, these will be evaluated before
        /// expression is evaluated
        /// </summary>
        /// <param name="node">node who's value will be evaluated</param>
        /// <param name="context">application context</param>
        /// <typeparam name="T">type of object you wish to retrieve</typeparam>
        public static IEnumerable<T> Iterate<T> (Node node, ApplicationContext context)
        {
            return Iterate<T> (node, node, context);
        }

        /// <summary>
        /// iterates the given node's value, which might be either an expression or a constant. if node's
        /// value is a constant, then this constant will be converted if necessary to T before returned. if
        /// node's value is an expression, then this expression will be evaluated, and all results converted
        /// to T before returned to caller. node's value can contain formatting parameters, which will be
        /// evaluated if existing. if node contains formatting parameters, these will be evaluated before
        /// expression is evaluated
        /// </summary>
        /// <param name="node">node who's value will be evaluated</param>
        /// <param name="dataSource">node to use as start node for any expressions within formatting parameters</param>
        /// <param name="context">application context</param>
        /// <param name="retrieveInner">if true, will retrieve inner nodes of type is Node and node's value is a string,
        /// converted into nodes</param>
        /// <typeparam name="T">type of object you wish to retrieve</typeparam>
        public static IEnumerable<T> Iterate<T> (
            Node node, 
            Node dataSource, 
            ApplicationContext context, 
            bool retrieveInner = true)
        {
            if (IsExpression (node.Value)) {

                // node's value is expression, iterating expression result, yielding back to caller
                string exp = IsFormatted (node) ? FormatNode (node, dataSource, context) : node.Get<string> (context);
                foreach (var idx in Iterate<T> (exp, dataSource, context)) {
                    yield return idx;
                }
            } else if (node.Value != null) {

                // node's value is not null, converting value to type requests, and yielding back to caller
                object value = IsFormatted (node) ? FormatNode (node, dataSource, context) : node.Value;
                if (retrieveInner && node.Value is string && typeof(T) == typeof(Node)) {

                    // nodes was created from a string representation, making sure we return inner nodes, to
                    // eliminate root node created automatically for us during conversion
                    foreach (Node idxInner in Utilities.Convert<Node> (value, context).Children) {
                        yield return Utilities.Convert<T> (idxInner.Clone (), context);
                    }
                } else {
                    yield return Utilities.Convert<T> (value, context);
                }
            } else if (typeof(T) == typeof(Node)) {

                // node's value is null, caller requests nodes, 
                // iterating through children, yielding children back to caller
                foreach (Node idx in node.Children) {
                    yield return Utilities.Convert<T> (idx, context);
                }
            } else {

                // node's value is null, caller requests anything but node, iterating children, yielding
                // values of children, converted to type back to caller
                foreach (Node idx in node.Children) {
                    yield return idx.Get<T> (context);
                }
            }
        }

        /// <summary>
        /// iterates the given expression on the given dataSource node and converts each result from expression to
        /// type T before returning back to caller
        /// </summary>
        /// <param name="expression">expression to run on dataSource</param>
        /// <param name="dataSource">node to use as start node for any expressions within formatting parameters</param>
        /// <param name="context">application context</param>
        /// <typeparam name="T">type of object you wish to retrieve</typeparam>
        public static IEnumerable<T> Iterate<T> (string expression, Node dataSource, ApplicationContext context)
        {
            if (!IsExpression (expression))
                throw new ArgumentException ("Iterate was not given a valid expression");

            var match = Expression.Create (expression).Evaluate (dataSource, context);
            if (match.TypeOfMatch == Match.MatchType.count) {
                yield return Utilities.Convert<T> (match.Count, context);
            } else {
                foreach (var idx in match) {
                    yield return Utilities.Convert<T> (idx.Value, context);
                }
            }
        }

        /// <summary>
        /// returns all matches from expression in node. node may contain formatting parameters which will
        /// be evaluated before expression 
        /// </summary>
        /// <param name="node">node being both expression node and data source node</param>
        /// <param name="context">application context</param>
        public static IEnumerable<MatchEntity> Iterate (Node node, ApplicationContext context)
        {
            return Iterate (node, node, context);
        }

        /// <summary>
        /// returns all matches from expression in node. node may contain formatting parameters which will
        /// be evaluated before expression using dataSource as start node for any expressions within formatting
        /// parameters
        /// </summary>
        /// <param name="node">node being expression node</param>
        /// <param name="dataSource">node being data source node</param>
        /// <param name="context">application context</param>
        public static IEnumerable<MatchEntity> Iterate (Node node, Node dataSource, ApplicationContext context)
        {
            string exp = IsFormatted (node) ? FormatNode (node, dataSource, context) : node.Get<string> (context);
            return Iterate (exp, dataSource, context);
        }

        /// <summary>
        /// returns all matches from given expression
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="dataSource">node being data source node</param>
        /// <param name="context">application context</param>
        public static IEnumerable<MatchEntity> Iterate (string expression, Node dataSource, ApplicationContext context)
        {
            if (!IsExpression (expression))
                throw new ArgumentException ("Iterate was not given a valid expression");

            var match = Expression.Create (expression).Evaluate (dataSource, context);
            foreach (var idx in match) {
                yield return idx;
            }
        }

        /*
         * helper method to recursively format node's value
         */
        private static string FormatNodeRecursively (Node node, Node dataSource, ApplicationContext context)
        {
            bool isFormatted = IsFormatted (node);
            bool isExpression = IsExpression (node.Value);

            if (isExpression && isFormatted) {

                // node is recursively formatted, and also an expression. formating node first, then evaluating expression
                return Single<string> (FormatNode (node, dataSource, context), dataSource, context) ?? ""; // cannot return null here, in case expression yields null
            } else if (isFormatted) {

                // node is formatted recursively, but not an expression
                return FormatNode (node, dataSource, context);
            } else if (isExpression) {

                // node is an expression, but not formatted
                return Single<string> (node.Get<string> (context), dataSource, context);
            } else {

                // node is neither an expression, returning node's value, defaulting to string.Empty if no value exists
                return node.Get<string> (context, string.Empty);
            }
        }
    }
}
