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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace p5.json
{
    /// <summary>
    ///     Class to help transform JSON to a lambda object.
    /// </summary>
    public static class Json2Lambda
    {
        /// <summary>
        ///     Parses one or more JSON snippets, and creates a lambda object [result] from each JSON snippet.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "json2lambda")]
        public static void json2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args, true)) {

                // Iterating through each fragment.
                foreach (var idxFragment in XUtil.Iterate<string> (context, e.Args)) {

					// Creating a result node for currently iterated JSON fragment.
					var result = e.Args.Add ("result").LastChild;

					// Using Json.NET to parse JSON.
					var jObject = JObject.Parse (idxFragment);

                    // Recursively iterating through result, adding nodes into [result].
                    foreach (var idx in jObject) {
                        ParseJSON (result, idx);
                    }
                }
            }
        }

        /*
         * Helper for above to recursively traverse JObject.
         * TODO: Refactor, too complex.
         */
        static void ParseJSON (Node node, KeyValuePair<string, JToken> cur)
        {
            // Checking type of value.
            var jObject = cur.Value as JObject;
            if (jObject != null) {

                // Value is some sort of complex object.
                var curNode = node.Add (cur.Key).LastChild; 
                foreach (var idx in jObject) {
                    ParseJSON (curNode, idx);
                }

            } else {

                var jArray = cur.Value as JArray;
                if (jArray != null) {

                    // Value is an array of some sort.
                    var parent = node.Add (cur.Key).LastChild; 
                    foreach (var idx in jArray) {

                        // Retrieving value of currently iterated object.
                        var val = idx.ToObject<object> ();

						// Checking if currently iterated idx value is a JObject.
						jObject = val as JObject;
                        if (jObject != null) {
							foreach (var idxObj in jObject) {
								ParseJSON (parent, idxObj);
							}
						} else {

                            // Simple type.
                            parent.Add ("", val);
                        }
                    }

                } else {

                    // Value is a simple .Net type of some sort, making sure we check for [_value], 
                    // to avoid impediance type mismatch between conversions.
                    if (cur.Key == "__value")
                        node.Value = cur.Value.ToObject<object> ();
                    else
                        node.Add (cur.Key, cur.Value.ToObject<object> ());
                }
            }
        }
    }
}
