
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.lambda.iterators;

namespace phosphorus.lambda
{
    /// <summary>
    /// utility class to handle Expressions
    /// </summary>
    public static class XUtil
    {
        /// <summary>
        /// determines if value is an expression or not
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
        /// determines if value is an expression or not
        /// </summary>
        /// <returns><c>true</c> if value is an expression; otherwise, <c>false</c></returns>
        /// <param name="strValue">value to check</param>
        public static bool IsExpression (string strValue)
        {
            return strValue != null && 
                strValue.StartsWith ("@") && 
                strValue.Length > 5; // "@?node" is the shortest possible expression, and has 6 characters
        }

        /// <summary>
        /// returns formatted node according to children nodes
        /// </summary>
        /// <returns>the formatted string expression, or the original node's value if there are no formatters</returns>
        /// <param name="node">node to format</param>
        public static object FormatNode (Node node)
        {
            List<Node> formatingValues = new List<Node> (node.FindAll (string.Empty));
            if (formatingValues.Count > 0) {

                // this is a formatted node
                List<string> childrenValues = new List<string> ();
                foreach (Node idxNode in formatingValues) {
                    object value = FormatNode (idxNode);
                    if (IsExpression (value))
                        value = Expression.Create (value as string).Evaluate (idxNode).GetValue (0, string.Empty);
                    childrenValues.Add (value as string);
                }

                // returning node's value after being formatted according to its children nodes
                return string.Format (CultureInfo.InvariantCulture, node.Get<string> (), childrenValues.ToArray ());
            }

            // not a formatted node
            return node.Value;
        }

        /// <summary>
        /// iterator callback for iterating node expressions
        /// </summary>
        public delegate void IteratorCallbackVoid<T> (T idx);

        /// <summary>
        /// iterator callback for iterating node expressions
        /// </summary>
        public delegate void IteratorCallbackNode (Node idx, Match.MatchType type);

        /// <summary>
        /// iterates a value of a node, which might be either a constant, or an expression, and
        /// invokes callback for each value in constant or expression result
        /// </summary>
        /// <param name="node">node that contains constant or expression as value</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expressions</param>
        /// <param name="callback">code to invoke once for each result</param>
        /// <typeparam name="T">the type of object 'value' is</typeparam>
        public static void Iterate<T> (Node node, IteratorCallbackVoid<T> callback)
        {
            object nodeValue = FormatNode (node);
            if (IsExpression (nodeValue)) {
                var match = Expression.Create (nodeValue as string).Evaluate (node);
                for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                    callback (match.GetValue<T> (idxNo));
                }
            } else {
                // short hand helper for converting type correctly
                callback (new Node (string.Empty, nodeValue).Get<T> ());
            }
        }

        /// <summary>
        /// iterates a value of a node, which might be either a constant, or an expression, and
        /// invokes callback for each value in constant or expression result
        /// </summary>
        /// <param name="node">node that contains constant or expression as value</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expressions</param>
        /// <param name="callback">code to invoke once for each result</param>
        /// <typeparam name="T">the type of object 'value' is</typeparam>
        public static void IterateNodes (Node node, IteratorCallbackNode callback)
        {
            object nodeValue = FormatNode (node);
            if (IsExpression (nodeValue)) {
                var match = Expression.Create (nodeValue as string).Evaluate (node);
                foreach (Node idx in match.Matches) {
                    callback (idx, match.TypeOfMatch);
                }
            } else {
                throw new ArgumentException ("IterateNodes expected an expression, but was not given one");
            }
        }

        /// <summary>
        /// returns a single value of type T from expression in node's value given
        /// </summary>
        /// <param name="node">node containing expression, being current node for expression</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expression</param>
        /// <typeparam name="T">the 1st type parameter</typeparam>
        public static T Single<T> (Node node)
        {
            object nodeValue = FormatNode (node);
            if (IsExpression (nodeValue)) {
                var match = Expression.Create (nodeValue as string).Evaluate (node);
                if (match.TypeOfMatch == Match.MatchType.Count)
                    return (T)(object)match.Count;
                if (match.Count > 1)
                    throw new ArgumentException ("Single expected single value of expression, but expression returned multiple results");
                return match.GetValue<T> (0);
            } else {
                return new Node (string.Empty, nodeValue).Get<T> (); // short hand helper for converting type correctly
            }
        }

        /// <summary>
        /// clones all nodes in expression of node's value, if no expression exists,
        /// all children nodes of node are cloned and returned
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<Node> Clone (Node node)
        {
            List<Node> retVal = null;
            if (IsExpression (node.Value)) {
                retVal = new List<Node> ();
                Iterate<Node> (node,
                delegate (Node idx) {
                    retVal.Add (idx);
                });
            } else {
                retVal = new List<Node> (node.Children);
            }
            foreach (Node idx in retVal) {
                yield return idx.Clone ();
            }
        }

        /// <summary>
        /// returns a single string value from expression in node's value given by concatenating results
        /// if expression results to multiple results
        /// </summary>
        /// <param name="node">node containing expression, being current node for expression</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expression</param>
        /// <param name="spacingString">string to put between all results before returning value to caller</param>
        public static string Single (Node node, string spacingString = "")
        {
            string nodeValue = new Node (string.Empty, FormatNode (node)).Get<string> (); // shorthand to get string correctly
            if (IsExpression (nodeValue)) {
                var match = Expression.Create (nodeValue).Evaluate (node);
                string retVal = "";
                bool first = true;
                for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                    if (first) {
                        first = false;
                    } else {
                        retVal += spacingString;
                    }
                    retVal += match.GetValue (idxNo).ToString ();
                }
                return retVal;
            } else {
                if (nodeValue.StartsWith ("\\"))
                    nodeValue = nodeValue.Substring (1); // supporting escaped expressions
                return nodeValue;
            }
        }

        /// <summary>
        /// returns a single string value from expression in node's value given by concatenating results
        /// if expression results to multiple results
        /// </summary>
        /// <param name="node">node containing expression, being current node for expression</param>
        /// <param name="formatExpression">if set to <c>true</c> will format expression</param>
        /// <param name="spacingString">string to put between all results before returning value to caller</param>
        public static string SingleNameValuePair (Node node, string spacingString = "", string inbetweenPairString = "")
        {
            string nodeValue = new Node (string.Empty, FormatNode (node)).Get<string> (); // shorthand to get string correctly
            if (IsExpression (nodeValue)) {
                var match = Expression.Create (nodeValue).Evaluate (node);
                string retVal = "";
                bool first = true;
                for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                    if (first) {
                        first = false;
                    } else {
                        retVal += spacingString;
                    }
                    retVal += match.GetNode (idxNo).Name + inbetweenPairString + match.GetNode (idxNo).Get<string> ();
                }
                return retVal;
            } else {
                if (nodeValue.StartsWith ("\\"))
                    nodeValue = nodeValue.Substring (1); // supporting escaped expressions
                return nodeValue;
            }
        }
    }
}
