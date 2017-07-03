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
using System.Linq;
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
                    HandleToken (result, JToken.Parse (idxFragment));
                }
            }
        }

        /*
         * Helper for above.
         */
        static void HandleToken (Node node, JToken token)
        {
            if (token is JArray) {

                HandleArray (node, token as JArray);

            } else if (token is JObject) {

                HandleObject (node, token as JObject);

            } else if (token is JValue) {

                var val = token as JValue;
                node.Value = val.Value; 
            }
        }

        /*
         * Helper for above.
         */
        static void HandleObject (Node node, JObject obj)
        {
            // Special treatment for "__value" property.
            var val = obj.Children().FirstOrDefault (ix => ix is JProperty && (ix as JProperty).Name == "__value") as JProperty;
            if (val != null) {

                // Handling "__value" by setting the root node for object to "__value" property's value.
                node.Value = (val.Value as JValue).Value;
                obj.Remove ("__value");
            }
            foreach (var idx in obj) {
                HandleToken (node.Add (idx.Key).LastChild, idx.Value);
            }
        }

        /*
         * Helper for above.
         */
        static void HandleArray (Node node, JArray arr)
        {
            foreach (var idx in arr) {

                // Special treatment for JObjects with only one property.
                if (idx is JObject) {

                    // Checking if object has only one property.
                    var obj = idx as JObject;
                    if (obj.Count == 1 && obj.First is JProperty) {

                        // Object is a simple object with a single value.
                        var prop = obj.First as JProperty;
                        node.Add (prop.Name, (prop.Value as JValue).Value);
                        continue;
                    }
                }
                HandleToken (node.Add ("").LastChild, idx);
            }
        }
    }
}
