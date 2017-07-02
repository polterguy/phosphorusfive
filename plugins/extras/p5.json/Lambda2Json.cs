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

                // Creating our return value.
                if (e.Args.Value != null) {

                    // Expression or static value being node.
                    var tmp = new Node ();
                    tmp.AddRange (XUtil.Iterate<Node> (context, e.Args).Select (ix => ix.Clone ()));
                    e.Args.Value = SerializeNode (context, tmp).ToString ();

                } else {

                    // Iterating children.
                    e.Args.Value = SerializeNode (context, e.Args).ToString ();
                }
            }
        }

        /*
         * Helper for above.
         */
        static object SerializeNode (ApplicationContext context, Node node)
        {
            var ret = new JObject ();
            if (node.Value != null)
                ret.Add (new JProperty ("__value", JToken.FromObject (node.Value))); 
            foreach (var idx in node.Children) {
                if (idx.Count > 0) {

                    // Recursively invoking self.
                    ret.Add (new JProperty (idx.Name, SerializeNode (context, idx)));

                } else {

                    // Simple key/value pair.
					ret.Add (idx.Name, idx.Value == null ? null : JToken.FromObject (idx.Value));
				}
            }
            return ret;
        }
    }
}
