/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.exp
{
    /// <summary>
    ///     Helper class to help set, get and list collections
    /// </summary>
    public static class CollectionBase
    {
        /// <summary>
        ///     Callback functor object for Get operation
        /// </summary>
        public delegate object GetDelegate (string key);

        /// <summary>
        ///     Callback functor object for set operation
        /// </summary>
        public delegate void SetDelegate (string key, object value);

        /// <summary>
        ///     Sets one or more values in collection
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="node">Root node of collection Active Event invoker</param>
        /// <param name="functor">Callback functor, will be invoked once for each key</param>
        public static void Set (ApplicationContext context, Node args, SetDelegate functor, bool isNative = false)
        {
            // Special handling of native invocations
            if (isNative) {

                // This is a native invocation
                functor (args.Get<string> (context), args [0].Value);
            } else {

                // Retrieving source
                var source = Source (args.LastChild, context);

                // Looping through each destination, invoking functor for each object
                foreach (var idxKey in XUtil.Iterate<string> (context, args, true)) {

                    // Making sure caller does not try to set "protected data"
                    if (idxKey.IndexOf ("_") == 0)
                        throw new LambdaException ("User tried to update protected value in collection", args, context);

                    // Invoking functor
                    functor (idxKey, source);
                }
            }
        }

        /// <summary>
        ///     Gets a value from a collection
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="node">Root node of collection Active Event invoker</param>
        /// <param name="functor">Callback functor, will be invoked once for each key</param>
        public static void Get (ApplicationContext context, Node args, GetDelegate functor, bool isNative = false)
        {
            // Special handling of native invocations
            if (isNative) {

                // This is a native invocation
                args.Value = functor (args.Get<string>(context));
                
            } else {

                // Making sure we clean up and remove all arguments passed in after execution
                using (new Utilities.ArgsRemover (args, true)) {

                    // Iterating through each "key"
                    foreach (var idxKey in XUtil.Iterate<string> (context, args, true)) {

                        // Making sure caller does not try to retrieve "protected data"
                        if (!isNative && idxKey.IndexOf ("_") == 0)
                            throw new LambdaException ("User tried to access protected value from collection", args, context);

                        // Retrieving object by invoking functor with key
                        var value = functor (idxKey);

                        // Checking if value is not null, and if not, adding result, 
                        // according to what type of value we're given
                        if (value != null) {

                            // Adding key node, and value as object, if value is not node, otherwise
                            // appending value nodes beneath key node
                            var resultNode = args.Add (idxKey).LastChild;
                            if (value is Node) {

                                // Value is Node
                                resultNode.Add ((value as Node).Clone ());
                            } else if (value is IEnumerable<Node>) {

                                // Value is a bunch of nodes, adding them all
                                foreach (var idxValue in value as IEnumerable<Node>) {
                                    resultNode.Add (idxValue.Clone ());
                                }
                            } else if (value is IEnumerable<object>) {

                                // Value is a bunch of object values, adding them all as values of children appended into args
                                foreach (var idxValue in value as IEnumerable<object>) {
                                    resultNode.Add ("", idxValue);
                                }
                            } else {

                                // Value is any "other type of value", returning it anyway, even though it
                                // cannot possibly have come from p5 lambda, to allow user to retrieve "any values"
                                // that exists
                                resultNode.Value = value;
                            }
                        } else {

                            // There was no value in collection for key, reflecting this fact back to caller
                            args.Add (idxKey, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all items from a collection
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="node">Root node of Active Event invoked</param>
        /// <param name="functor">Callback functor, will be invoked once to retrieve all keys from collection</param>
        public static void List (ApplicationContext context, Node node, IEnumerable list, bool isNative = false)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (node, true)) {

                // Retrieving filters, if any
                var filter = new List<string> (XUtil.Iterate<string> (context, node));

                // Looping through each existing key in collection
                foreach (string idxKey in list) {

                    // Checking if currently iterated key is "protected data"
                    if (!isNative && idxKey.IndexOf ("_") == 0)
                        continue; // Ignoring "protected data"

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

        /*
         * Helper to retrieve source for setters
         */
        private static object Source (Node evaluatedNode, ApplicationContext context)
        {
            object source = null;

            // We have a [src] parameter here, figuring out what it points to, or contains
            if (evaluatedNode == null) {
                return null;
            } else if (XUtil.IsExpression (evaluatedNode.Value)) {

                // This is an expression which might lead to multiple results, trying to return one result,
                // but will resort to returning List of objects if necssary
                var tmpList = new List<object> (XUtil.Iterate<object> (context, evaluatedNode, evaluatedNode));
                switch (tmpList.Count) {
                    case 0:
                        // No source values
                        break;
                    case 1:
                        // One single object in list, returning only that single object
                        source = tmpList [0];
                        break;
                    default:
                        source = tmpList;
                        break;
                }
            } else if (evaluatedNode.Value != null) {

                // Source is a constant, might still be formatted
                source = XUtil.FormatNode (context, evaluatedNode, evaluatedNode);

                if (source is Node)
                    source = (source as Node).Clone ();
            } else {

                // There are no value in [src] node, trying to create source out of [src]'s children
                if (evaluatedNode.Children.Count == 1) {

                    // Source is a constant node, making sure we clone it, in case source and destination overlaps
                    source = evaluatedNode.FirstChild.Clone ();
                } else {

                    // More than one source, making sure we clone them, before we return the clones
                    source = evaluatedNode.Clone ().Children;
                }
            }

            // returning source
            return source;
        }
    }
}
