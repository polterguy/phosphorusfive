
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions
{
    /// <summary>
    /// contains useful helper methods for dealing with pf.lambda expressions
    /// </summary>
    public static class XUtil
    {
        // used to retrieve items in Single<T> methods
        private delegate IEnumerable<T> SingleDelegate<T> ();

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
                value.Length >= 4 && // "@{0}" is the shortest possible expression, and has 4 characters
                // an expression must have an iterator, referenced expression, string formatter, 
                // or a type declaration as its second character
                (value [1] == '?' || value [1] == '/' || value [1] == '{' || value [1] == '@');
        }

        /// <summary>
        /// returns type of expression
        /// </summary>
        /// <returns>type of expression</returns>
        /// <param name="expressionNode">node containing expression to check, will be formatted if necessary</param>
        /// <param name="context">application context</param>
        public static Match.MatchType ExpressionType (Node expressionNode, ApplicationContext context)
        {
            // checking if we're actually given an expression
            if (!IsExpression (expressionNode.Value))
                throw new ExpressionException (
                    expressionNode.Value as string,
                    "ExpressionType must be given an actual expression",
                    expressionNode,
                    context);

            string exp = TryFormat<string> (expressionNode, context);
            string type = exp.Substring (exp.LastIndexOf ('?') + 1);
            if (type.Contains ("."))
                type = type.Substring (0, type.IndexOf ('.'));

            // some additional code, to be able to provide intelligent errors back to caller, if something goes wrong ...
            Match.MatchType matchType;
            switch (type) {
            case "node":
            case "value":
            case "count":
            case "name":
            case "path":
                matchType = (Match.MatchType)Enum.Parse (typeof(Match.MatchType), type);
                break;
            default:
                throw new ExpressionException (
                    exp, 
                    string.Format ("'{0}' is an unknown type declaration for your expression", type),
                    expressionNode,
                    context);
            }

            // returning type back to caller
            return matchType;
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
                throw new ExpressionException (
                    (node.Value ?? "").ToString (), 
                    "Cannot format node, no formatting nodes exists, or node's value is not a string",
                    node,
                    context);

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
        /// checks if node is formatted, and if it is, it will format the node, and return the
        /// formatted value, as type T
        /// </summary>
        /// <returns>the value of the node</returns>
        /// <param name="node">node that might be formatted</param>
        /// <param name="context">application context</param>
        /// <param name="defaultValue">default value</param>
        /// <typeparam name="T">the type you wish to convert the node's value into</typeparam>
        public static T TryFormat<T> (Node node, ApplicationContext context, T defaultValue = default(T))
        {
            return TryFormat<T> (node, node, context, defaultValue);
        }
        
        /// <summary>
        /// checks if node is formatted, and if it is, it will format the node, and return the
        /// formatted value, as type T
        /// </summary>
        /// <returns>the value of the node</returns>
        /// <param name="node">node that might be formatted</param>
        /// <param name="dataSource">data source to use for formatting operation</param>
        /// <param name="context">application context</param>
        /// <param name="defaultValue">default value</param>
        /// <typeparam name="T">the type you wish to convert the node's value into</typeparam>
        public static T TryFormat<T> (Node node, Node dataSource, ApplicationContext context, T defaultValue = default(T))
        {
            if (IsFormatted (node)) {

                // node is formatted, formatting the node, and converting results to T
                return Utilities.Convert<T> (FormatNode (node, dataSource, context), context, defaultValue);
            } else {

                // node is not formatted, returning its value as type T, possibly triggering a conversion
                return Utilities.Convert<T> (node.Value, context, defaultValue);
            }
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
        public static T Single<T> (
            Node node, 
            Node dataSource, 
            ApplicationContext context, 
            T defaultValue = default (T))
        {
            return SingleImplementation<T> (
            delegate {
                return Iterate<T> (node, dataSource, context);
            }, context, defaultValue);
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
            return SingleImplementation<T> (
            delegate {
                return Iterate<T> (expression, dataSource, context);
            }, context, defaultValue);
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
            ApplicationContext context)
        {
            if (IsExpression (node.Value)) {

                // node's value is expression, iterating expression result, yielding back to caller
                string exp = TryFormat<string> (node, dataSource, context);
                foreach (var idx in Iterate<T> (exp, dataSource, context)) {
                    yield return idx;
                }
            } else if (node.Value != null) {

                // node's value is not null, converting value to type requested, possibly triggering a
                // formatting operation, and yielding the result back to caller
                yield return TryFormat<T> (node, dataSource, context);
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
            // syntax checking
            if (!IsExpression (expression))
                throw new ExpressionException (expression, dataSource, context);

            // creating a match object
            var match = Expression.Create (expression).Evaluate (dataSource, context);

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
        }

        // TODO: do we really need this one, we've got IterateNodes and Iterate, which checks for T being Node ...?
        /// <summary>
        /// retrieves a list of nodes from the given node somehow
        /// </summary>
        /// <returns>a list of nodes</returns>
        /// <param name="node">node containing either an expression leading to your node list, or a list of children</param>
        /// <param name="context">application context</param>
        public static IEnumerable<Node> IterateChildren (Node node, ApplicationContext context)
        {
            if (node.Value == null) {

                // returning children of given node
                foreach (Node idxChild in node.Children) {
                    yield return idxChild;
                }
            } else if (IsExpression (node.Value)) {

                // node's value is expression, depending upon type of expression, we either return nodes directly,
                // or children of match converted to node
                foreach (var idxMatch in Iterate (node, context)) {
                    if (idxMatch.TypeOfMatch == Match.MatchType.node) {

                        // node match, returning node directly
                        yield return Utilities.Convert<Node> (idxMatch.Value, context);
                    } else {

                        // not a node match, converting match to node, and returning children of that node
                        // since conversion to node will create a "root wrapper node", we return that node's children,
                        // to eliminate the automatically generated "root node"
                        foreach (Node innerNode in Utilities.Convert<Node> (idxMatch.Value, context).Children) {
                            yield return innerNode;
                        }
                    }
                }
            } else {

                // value of node is either a node itself, or a string that'll be converted to a node,
                // we return children of value, converting if necessary
                foreach (Node innerNode in Utilities.Convert<Node> (node.Value, context).Children) {
                    yield return innerNode;
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
        /// returns all matches from expression in node. node may contain formatting parameters, which will
        /// be evaluated first, using dataSource as start node, for any expressions within formatting expression
        /// parameters
        /// </summary>
        /// <param name="node">node being expression node</param>
        /// <param name="dataSource">node being data source node</param>
        /// <param name="context">application context</param>
        public static IEnumerable<MatchEntity> Iterate (
            Node node, 
            Node dataSource, 
            ApplicationContext context)
        {
            string exp = TryFormat<string> (node, dataSource, context);
            return Iterate (exp, dataSource, context);
        }

        /// <summary>
        /// returns all matches from expression in node. node may contain formatting parameters which will
        /// be evaluated before expression using dataSource as start node for any expressions within formatting
        /// parameters
        /// </summary>
        /// <param name="node">node being expression node</param>
        /// <param name="dataSource">node being data source node</param>
        /// <param name="context">application context</param>
        public static IEnumerable<MatchEntity> Iterate (
            Node node, 
            Node dataSource, 
            Node formattingSource, 
            ApplicationContext context)
        {
            string exp = TryFormat<string> (node, formattingSource, context);
            return Iterate (exp, dataSource, context);
        }

        /// <summary>
        /// returns all matches from given expression
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="dataSource">node being data source node</param>
        /// <param name="context">application context</param>
        public static IEnumerable<MatchEntity> Iterate (
            string expression, 
            Node dataSource, 
            ApplicationContext context)
        {
            // syntax checking
            if (!IsExpression (expression))
                throw new ExpressionException (expression, dataSource, context);

            // creating a match to iterate over
            var match = Expression.Create (expression).Evaluate (dataSource, context);

            // iterating over each MatchEntity in Match
            foreach (var idx in match) {

                // yielding MatchEntity back to caller
                yield return idx;
            }
        }

        // TODO: refactor these next buggers, too complex
        /// <summary>
        /// retrieves the value of [source], or [src] child node, converted into T. returns null if no source exists. 
        /// does not care about whether or not there are multiple values, and will return a List if there are, though
        /// will attempt to return only one value if it can
        /// </summary>
        /// <param name="node">node where [source] or [src] is expected to be a child</param>
        /// <param name="context">application context</param>
        public static object Source (Node node, ApplicationContext context)
        {
            object source = null;
            if (node.LastChild != null && (node.LastChild.Name == "source" || node.LastChild.Name == "src")) {

                // we have a [source] or [src] parameter here, figuring out what it points to, or contains
                if (IsExpression (node.LastChild.Value)) {

                    // this is an expression which might lead to multiple results, trying to return one result,
                    // but will resort to returning List of objects if necssary
                    List<object> tmpList = new List<object> (Iterate<object> (node.LastChild, context));
                    if (tmpList.Count == 0) {

                        // no source values
                        source = null;
                    } else if (tmpList.Count == 1) {

                        // one single object in list, returning only that single object
                        source = tmpList [0];
                    } else {

                        // multiple objects, returning all objects
                        source = tmpList;
                    }
                } else if (node.LastChild.Value != null) {

                    // source is a constant, might still be formatted
                    if (IsFormatted (node.LastChild)) {

                        // node is formatted
                        source = FormatNode (node.LastChild, context);
                    } else {

                        // node is not formatted
                        source = node.LastChild.Value;
                    }
                } else {

                    // there are no value in [src] node, trying to create source out of [src]'s children
                    if (node.LastChild.Count == 1) {

                        // source is a constant node, making sure we clone it, in case source and destination overlaps
                        source = node.LastChild.FirstChild.Clone ();
                    } else {

                        // more than one source, making sure we clone them, before we return the clones
                        source = new List<Node> (node.LastChild.Clone ().UntieChildren ());
                    }
                }
            }

            // making sure we support "escaped expressions"
            if (source is string && (source as string).StartsWith ("\\"))
                source = (source as string).Substring (1);

            // returning source
            return source;
        }
        
        /// <summary>
        /// retrieves the value of [source], or [src] child node(s), forcing one single return value, somehow.
        /// returns null if no source exists. used in among other things [set].
        /// </summary>
        /// <param name="node">node where [source] or [src] is expected to be a child</param>
        /// <param name="context">application context</param>
        public static object SourceSingle (Node node, ApplicationContext context)
        {
            object source = null;
            if (node.LastChild != null && (node.LastChild.Name == "source" || node.LastChild.Name == "src")) {

                // we have a [source] or [src] parameter here, figuring out what it points to, or contains
                if (node.LastChild.Value != null) {

                    // this might be an expression, or a constant, converting value to single object, somehow
                    source = Single<object> (node.LastChild, context, null);
                    if (source is Node) {

                        // source is node, making sure we clone it, in case source and destination overlaps
                        source = (source as Node).Clone ();
                    }
                } else {

                    // there are no values in [src] node, trying to create source out of [src]'s children
                    if (node.LastChild.Count == 1) {

                        // source is a constant node, making sure we clone it, in case source and destination overlaps
                        source = node.LastChild.FirstChild.Clone ();
                    } else {

                        // more than one source, making sure we convert it into one single value, meaning a 'string'
                        source = Utilities.Convert<string> (new List<Node> (node.LastChild.Children), context);
                    }
                }
            }

            // making sure we support "escaped expressions"
            if (source is string && (source as string).StartsWith ("\\"))
                source = (source as string).Substring (1);

            // returning source
            return source;
        }

        /// <summary>
        /// retrieves the value of [source], or [src] child node. used in among other things [append]
        /// </summary>
        /// <param name="node">node where [source] or [src] is expected to be a child</param>
        /// <param name="context">application context</param>
        public static List<Node> SourceNodes (Node node, ApplicationContext context)
        {
            // return value
            List<Node> sourceNodes = new List<Node> ();

            // checking if any source exists
            if (node.LastChild == null || (node.LastChild.Name != "source" && node.LastChild.Name != "src"))
                return null; // no source was given

            // checking to see if we're given an expression
            if (XUtil.IsExpression (node.LastChild.Value)) {

                // [source] or [src] is an expression somehow
                foreach (var idx in XUtil.Iterate (node.LastChild, context)) {
                    if (idx.TypeOfMatch != Match.MatchType.node && !(idx.Value is Node)) {

                        // [source] is an expression leading to something that's not a node, this
                        // will trigger conversion from string to node, adding a "root node" during
                        // conversion. we make sure we remove this node, when creating our source
                        foreach (var idxInner in Utilities.Convert<Node> (idx.Value, context).Children) {
                            sourceNodes.Add (idxInner.Clone ());
                        }
                    } else {

                        // [source] is an expression, leading to something that's already a node somehow
                        sourceNodes.Add ((idx.Value as Node).Clone ());
                    }
                }
            } else if (node.LastChild.Value is Node) {

                // value of source is a node, adding this node
                sourceNodes.Add ((node.LastChild.Value as Node).Clone ());
            } else if (node.LastChild.Value is string) {

                // source is not an expression, but has a string value. this will trigger a conversion
                // from string, to node, creating a "root node" during conversion. we are discarding this 
                // "root" node, and only adding children of that automatically generated root node
                foreach (var idx in Utilities.Convert<Node> (node.LastChild.Value, context).Children) {
                    sourceNodes.Add (idx.Clone ());
                }
            } else if (node.LastChild.Value == null) {

                // source has no value, neither static string values, nor expressions
                // adding all children of source node, if any
                foreach (var idx in node.LastChild.Children) {
                    sourceNodes.Add (idx.Clone ());
                }
            } else {
                
                // source is not an expression, but has a non-string value. making sure we create a node
                // out of that value, returning that node back to caller
                sourceNodes.Add (new Node (string.Empty, node.LastChild.Value));
            }

            // returning node list back to caller
            return sourceNodes.Count > 0 ? sourceNodes : null;
        }

        /*
         * helper method to recursively format node's value
         */
        private static string FormatNodeRecursively (
            Node node, 
            Node dataSource, 
            ApplicationContext context)
        {
            bool isFormatted = IsFormatted (node);
            bool isExpression = IsExpression (node.Value);

            if (isExpression && isFormatted) {

                // node is recursively formatted, and also an expression
                // formating node first, then evaluating expression
                // PS, we cannot return null here, in case expression yields null
                return Single<string> (FormatNode (node, dataSource, context), dataSource, context, "");
            } else if (isFormatted) {

                // node is formatted recursively, but not an expression
                return FormatNode (node, dataSource, context);
            } else if (isExpression) {

                // node is an expression, but not formatted
                // PS, we cannot return null here, in case expression yields null
                return Single<string> (node.Get<string> (context), dataSource, context, "");
            } else {

                // node is neither an expression, returning node's value
                // PS, we cannot return null here, in case value yields null
                return node.Get<string> (context, string.Empty);
            }
        }

        // TODO: try to refactor, too complex
        /*
         * common implementation for Single<T> methods. requires a delegate responsible for returning
         * the IEnumerable that the method iterates over
         */
        private static T SingleImplementation<T> (
            SingleDelegate<T> functor, 
            ApplicationContext context, 
            T defaultValue)
        {
            object singleRetVal = null;
            string multipleRetVal = null;
            bool firstRun = true;
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
                    if (idx is Node || (singleRetVal != null && singleRetVal is Node)) {

                        // current iteration contains a node, making sure we format our string nicely, such that
                        // the end result becomes valid hyperlisp, before trying to convert to type T afterwards
                        multipleRetVal += "\r\n";
                        singleRetVal = null;
                    }
                    multipleRetVal += Utilities.Convert<string> (idx, context);
                }
            }

            // if there was not multiple iterations above, we use our "singleRetVal" object, which never was
            // converted into a string, to make sure we don't convert unless necessary, and keep reference objects
            // stay just that
            if (multipleRetVal == null)
                return Utilities.Convert<T> (singleRetVal, context, defaultValue);

            // there were multiple return values, hence we'll need to use conversion
            return Utilities.Convert<T> (multipleRetVal, context, defaultValue);
        }
    }
}
