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
    public static class Collection
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
        public static void Set (ApplicationContext context, Node args, SetDelegate functor, bool isNative)
        {
            // Special handling of native invocations
            if (isNative) {

                // This is a native invocation
                functor (args.Get<string> (context), args [0].Value);
            } else {

                if (args.Children.Count (ix => ix.Name != "") > 0 && args ["src"] == null) {

                    // Sanity check!
                    var destEx = args.Value as Expression;
                    if (destEx == null)
                        throw new LambdaException (
                            string.Format ("Not a valid destination expression given to Set, value was '{0}', expected expression", args.Value),
                            args, 
                            context);

                    // Iterating through all destinations, figuring out source either relative to each destinations,
                    // or using Active Event source invocation
                    foreach (var idxDestination in destEx.Evaluate (context, args, args)) {

                        // Making sure caller does not try to set "protected data"
                        var idxKey = Utilities.Convert<string> (context, idxDestination.Value);
                        if (idxKey.IndexOf ("_") == 0)
                            throw new LambdaException ("User tried to update protected value in collection", args, context);

                        // Source is relative to destination, postponing figuring it out, until we're inside 
                        // our destination nodes, on each iteration, passing in destination node as data source
                        functor (idxKey, XUtil.SourceSingle (context, args, idxDestination.Node));
                    }
                } else {

                    // Static source, or "null source", hence retrieving source before iteration starts, 
                    // in case destination and source overlaps
                    var source = XUtil.SourceSingle (context, args);

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
        }

        /// <summary>
        ///     Gets a value from a collection
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="node">Root node of collection Active Event invoker</param>
        /// <param name="functor">Callback functor, will be invoked once for each key</param>
        public static void Get (ApplicationContext context, Node args, GetDelegate functor, bool isNative)
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

                        DecorateArgsWithValue (args, idxKey, value);
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
        public static void List (ApplicationContext context, Node node, IEnumerable list, bool isNative)
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
         * Given key and value, will put value of object into args
         */
        private static void DecorateArgsWithValue (Node args, string key, object value)
        {
            // Adding node for given key, defaulting to null value
            var resultNode = args.Add (key).LastChild;

            // Checking if value is not null, and if not, adding result, 
            // according to what type of value we're given
            if (value != null) {

                // Adding key node, and value as object, if value is not node, otherwise
                // appending value nodes beneath key node
                if (value is Node) {

                    // Value is Node
                    resultNode.Add ((value as Node).Clone ());
                } else if (value is IEnumerable<Node>) {

                    // Value is a bunch of nodes, adding them all
                    resultNode.AddRange ((value as IEnumerable<Node>).Select (ix => ix.Clone ()));
                } else if (value is IEnumerable<object>) {

                    // Value is a bunch of object values, adding them all as values of children appended into args
                    resultNode.AddRange ((value as IEnumerable<object>).Select (ix => new Node ("", ix)));
                } else {

                    // Value is any "other type of value", returning it "as is"
                    resultNode.Value = value;
                }
            }
        }
    }
}
