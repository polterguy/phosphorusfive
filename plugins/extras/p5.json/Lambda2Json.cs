/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using p5.exp;
using p5.core;
using p5.exp.exceptions;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace p5.json
{
    /// <summary>
    ///     Class to help transform lambda to a JSON snippet.
    /// </summary>
    public static class Lambda2json
    {
        /// <summary>
        ///     Recursively iterates a lambda object and creates a JSON snippet out of it.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda2json")]
        public static void lambda2json (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Extracting nodes.
                var nodes = XUtil.Iterate<Node> (context, e.Args);

                if (!nodes.Any ()) {

                    // Empty object.
                    e.Args.Value = "{}";
                    return;

                } else if (nodes.First ().Name == "") {

                    // Simple array value.
                    e.Args.Value = new JArray (nodes.Select (ix => ArrayHelper (context, ix))).ToString (Formatting.None);
                    return;
                }

				// Complex object of some sort.
				var retVal = new JObject ();
				foreach (var idx in nodes) {
					retVal.Add (new JProperty (idx.Name, SerializeNode (context, idx)));
				}

				// Creating our return value.
                e.Args.Value = retVal.ToString (Formatting.None);
            }
        }

        /*
         * Helper for above.
         */
        static JToken SerializeNode (ApplicationContext context, Node node)
        {
            if (node.Count == 0)
                return JToken.FromObject (node.Value); // Simple object.

            if (node.FirstChild.Name == "") {

                // Sanity check.
                if (node.Value != null)
                    throw new LambdaException ("Cannot mix value with arrays when creating JSON from lambda", node, context);

                return new JArray (node.Children.Select (ix => ArrayHelper (context, ix)));
            }

            // Complex object.
            var retVal = new JObject ();
            if (node.Value != null)
                retVal.Add ("__value", JToken.FromObject (node.Value)); // Value AND Children, preserving value as "__value".

            // Looping through children, creating one property for each child.
            foreach (var idx in node.Children) {
                retVal.Add (new JProperty (idx.Name, SerializeNode (context, idx)));
            }
            return retVal;
        }

        /*
         * Helper for above.
         */
        static JToken ArrayHelper (ApplicationContext context, Node node)
        {
            if (node.Name == "" && node.Value != null)
                return JToken.FromObject (node.Value); // Simply value token

            // Checking if array instance is a complex object.
            if (node.Name == "") {

                // Complex array object, where neither value nor name plays any role.
                // This instance in your array does not have a value, due to the above if statement having already checked for that.
                var tmp = new JObject ();
                foreach (var idxChild in node.Children) {
                    tmp.Add (new JProperty (idxChild.Name, SerializeNode (context, idxChild)));
                }
                return tmp;
            }

            // Complex object.
            var retVal = new JObject ();
            retVal.Add (new JProperty (node.Name, SerializeNode (context, node)));
            return retVal;
        }
    }
}
