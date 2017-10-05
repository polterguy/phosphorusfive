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
using System.Linq;
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
        [ActiveEvent (Name = "p5.json.lambda2json.indented")]
        public static void lambda2json (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Figuring out if we should nicely format our JSON.
                var format = e.Name == "lambda2json" ? Formatting.None : Formatting.Indented;

                // Extracting nodes.
                var nodes = XUtil.Iterate<Node> (context, e.Args);

                // Checking type of JSON object we should return.
                if (!nodes.Any ()) {

                    // No input given.
                    return;

                } else if (nodes.First ().Name == "") {

                    // Simple array value not wrapped in an object.
                    e.Args.Value = new JArray (nodes.Select (ix => ArrayHelper (context, ix))).ToString (format);

                } else {

                    // Complex object of some sort.
                    e.Args.Value = new JObject (nodes.Select (ix => new JProperty (ix.Name, SerializeNode (context, ix)))).ToString (format);
                }
            }
        }

        /*
         * Helper for above.
         */
        static JToken SerializeNode (ApplicationContext context, Node node)
        {
            if (node.Count == 0)
                return node.Value == null ? null : JToken.FromObject (node.Value); // Simple object.

            if (node.FirstChild.Name == "" && node.Value == null)
                return new JArray (node.Children.Select (ix => ArrayHelper (context, ix)));

            // Complex object.
            var retVal = new JObject (node.Children.Select (ix => new JProperty (ix.Name, SerializeNode (context, ix))));
            if (node.Value != null)
                retVal.AddFirst (new JProperty ("__value", JToken.FromObject (node.Value))); // Value AND Children, preserving value as "__value".
            return retVal;
        }

        /*
         * Helper for above.
         */
        static JToken ArrayHelper (ApplicationContext context, Node node)
        {
            if (node.Name == "" && node.Count == 0)
                return node.Value == null ? null : JToken.FromObject (node.Value); // Simply value token

            // Checking if array instance is a complex object.
            if (node.Name == "")
                return new JObject (node.Children.Select (ix => new JProperty (ix.Name, SerializeNode (context, ix))));

            // Complex object.
            return new JObject (new JProperty (node.Name, SerializeNode (context, node)));
        }
    }
}
