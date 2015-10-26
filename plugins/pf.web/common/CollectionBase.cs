/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using pf.core;
using phosphorus.expressions;

/// <summary>
///     Namespace wrapping helpers for collections.
/// 
///     Namespace contains common helper operations for collection base classes, such as Session, Application and Cookies.
/// </summary>
namespace phosphorus.web.ui.common
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
            var source = XUtil.Source (node, context);

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
            // iterating through each "key"
            foreach (var idx in XUtil.Iterate<string> (node, context)) {
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
                    }
                    else if (value is IEnumerable<object>) {
                        // value is a bunch of object values
                        foreach (var idxValue in value as IEnumerable<object>) {
                            resultNode.Add (string.Empty, idxValue);
                        }
                    } else {
                        // value is any "other type of value", returning it anyway, even though it
                        // cannot possibly have come from pf.lambda, to allow user to retrieve "any values"
                        // that exists
                        resultNode.Value = value;
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
}