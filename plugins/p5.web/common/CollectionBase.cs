/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using p5.core;
using p5.exp;

/// <summary>
///     Namespace wrapping helpers for collections.
/// 
///     Namespace contains common helper operations for collection base classes, such as Session, Application and Cookies.
/// </summary>
namespace p5.web.ui.common
{
    /// <summary>
    ///     Helper class to help set, get and list collections.
    /// 
    ///     Helper class for Active Events that needs to set, get and list collections, such as Session, Cookies and Application.
    /// </summary>
    public static class CollectionBase
    {
        /// <summary>
        ///     Callback functor object for Get operation.
        /// 
        ///     Functor that expects callback to return an object matching the given key parameter.
        ///     Used in for instance Session and Application wrapper to retrieve one item with the given name.
        /// </summary>
        public delegate object GetDelegate (string key);

        /// <summary>
        ///     Callback functor object for list operation.
        /// 
        ///     Expects callback to return all keys. Used in among the Session and Application wrapper, to 
        ///     ask for all keys in Session and Application.
        /// </summary>
        public delegate IEnumerable<string> ListDelegate ();

        /// <summary>
        ///     Callback functor object for set operation.
        /// 
        ///     Expects callback to change or set one item, with the given name, to the given value.
        /// </summary>
        public delegate void SetDelegate (string key, object value);

        /// <summary>
        ///     Sets a single value in a collection.
        /// 
        ///     Requres caller to supply a functor callback, that should set one item in the collection. Will loop through
        ///     all keys caller requests to set, and invoke callback once for each item.
        /// </summary>
        /// <param name="node">Root node of collection Active Event invoker.</param>
        /// <param name="context">Application context.</param>
        /// <param name="functor">Callback functor, will be invoked once for each key.</param>
        public static void Set (Node node, ApplicationContext context, SetDelegate functor)
        {
            // retrieving source
            var source = Source (node.LastChild, context);

            // looping through each destination, creating an object, or removing an existing
            // object, for each destination
            foreach (var idx in XUtil.Iterate<string> (node, context)) {

                // single object
                functor (idx, source);
            }
        }

        /// <summary>
        ///     Gets a value from a collection.
        /// 
        ///     Expects caller to supply a functor callback, that should retrieve one item from the collection.
        ///     Will loop through all keys caller requests to set, and invoke callback once for each item.
        /// </summary>
        /// <param name="node">Root node of collection Active Event invoker.</param>
        /// <param name="context">Application context.</param>
        /// <param name="functor">Callback functor, will be invoked once for each key.</param>
        public static void Get (Node node, ApplicationContext context, GetDelegate functor)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover remover = new Utilities.ArgsRemover (node, false)) {

                // iterating through each "key"
                List<string> keys = new List<string> (XUtil.Iterate<string> (node, context));
                if (keys.Count == 1) {

                    // single key
                    var value = functor (keys [0]);
                    if (value != null) {

                        // adding value as value into root active event node
                        if (value is Node) {

                            // value is Node
                            node.Value = (value as Node).Clone ();
                        } else if (value is IEnumerable<Node>) {

                            // value is a bunch of nodes
                            Node resultNode = new Node ();
                            foreach (var idxValue in value as IEnumerable<Node>) {
                                resultNode.Add (idxValue.Clone ());
                            }
                            node.Value = resultNode;
                        } else if (value is IEnumerable<object>) {

                            // value is a bunch of object values
                            Node resultNode = new Node ();
                            foreach (var idxValue in value as IEnumerable<object>) {
                                resultNode.Add (string.Empty, idxValue);
                            }
                            node.Value = resultNode;
                        } else {

                            // value is any "other type of value", returning it anyway, even though it
                            // cannot possibly have come from p5.lambda, to allow user to retrieve "any values"
                            // that exists
                            node.Value = value;
                        }
                    } else {

                        // there was no value in session
                        node.Value = null;
                    }
                } else {

                    // multiple keys
                    foreach (var idx in keys) {

                        // retrieving object from key
                        var value = functor (idx);
                        if (value != null) {

                            // adding key node, and value as object, if value is not node, otherwise
                            // appending value nodes beneath key node
                            var resultNode = node.Add (idx).LastChild;
                            if (value is Node) {

                                // value is Node
                                resultNode.Add ((value as Node).Clone ());
                            } else if (value is IEnumerable<Node>) {

                                // value is a bunch of nodes
                                foreach (var idxValue in value as IEnumerable<Node>) {
                                    resultNode.Add (idxValue.Clone ());
                                }
                            } else if (value is IEnumerable<object>) {

                                // value is a bunch of object values
                                foreach (var idxValue in value as IEnumerable<object>) {
                                    resultNode.Add (string.Empty, idxValue);
                                }
                            } else {

                                // value is any "other type of value", returning it anyway, even though it
                                // cannot possibly have come from p5.lambda, to allow user to retrieve "any values"
                                // that exists
                                resultNode.Value = value;
                            }
                        } else {

                            // there was no value in session for key
                            var resultNode = node.Add (idx, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all items from a collection.
        /// 
        ///     Expects caller to supply a functor callback, that should return all keys from the collection.
        ///     Will loop through all keys supplied by caller, use them as a filter, and return all items from
        ///     the collection, having a key that matches the filter(s).
        /// </summary>
        /// <param name="node">Root node of Active Event invoked.</param>
        /// <param name="context">Application context.</param>
        /// <param name="functor">Callback functor, will be invoked once to retrieve all keys from collection.</param>
        public static void List (Node node, ApplicationContext context, ListDelegate functor)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (Utilities.ArgsRemover remover = new Utilities.ArgsRemover (node, true)) {

                // retrieving filters, if any
                var filter = new List<string> (XUtil.Iterate<string> (node, context));

                // looping through each existing key in collection
                foreach (var idxKey in functor ()) {

                    // returning current key, if it matches our filter, or filter is not given
                    if (filter.Count == 0) {

                        // no filter was given
                        node.Add (idxKey);
                    } else {

                        // filter was given, checking if key matches one of our filters
                        if (filter.Any (idxFilter => idxKey.IndexOf (idxFilter, StringComparison.Ordinal) != -1)) {
                            node.Add (idxKey);
                        }
                    }
                }
            }
        }
        
        private static object Source (Node evaluatedNode, ApplicationContext context)
        {
            object source = null;

            // we have a [source] or [src] parameter here, figuring out what it points to, or contains
            if (evaluatedNode == null) {
                return null;
            } else if (XUtil.IsExpression (evaluatedNode.Value)) {

                // this is an expression which might lead to multiple results, trying to return one result,
                // but will resort to returning List of objects if necssary
                var tmpList = new List<object> (XUtil.Iterate<object> (evaluatedNode, evaluatedNode, context));
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
                source = XUtil.FormatNode (evaluatedNode, evaluatedNode, context);

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
    }
}
