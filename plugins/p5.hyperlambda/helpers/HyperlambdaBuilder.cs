/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Text;
using System.Collections.Generic;
using p5.core;

/// <summary>
///     Contains common helper methods for Hyperlambda
/// </summary>
namespace p5.hyperlambda.helpers
{
    /// <summary>
    ///     Class encapsulating internals of creation of Hyperlambda
    /// </summary>
    public class HyperlambdaBuilder
    {
        private readonly ApplicationContext _context;
        private readonly IEnumerable<Node> _nodes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HyperlambdaBuilder" /> class
        /// </summary>
        /// <param name="context">Application context object</param>
        /// <param name="nodes">Nodes to convert into Hyperlambda</param>
        public HyperlambdaBuilder (ApplicationContext context, IEnumerable<Node> nodes)
        {
            _context = context;
            _nodes = nodes;
        }

        /// <summary>
        ///     Parses and retrieves the Hyperlambda
        /// </summary>
        /// <value>hyperlambda</value>
        public string Hyperlambda
        {
            get
            {
                var builder = new StringBuilder ();
                Nodes2Hyperlisp (builder, _nodes, 0);
                return builder.ToString ().TrimEnd ();
            }
        }

        /*
         * Recursively invoked for every "level" in node hierarchy
         */
        private void Nodes2Hyperlisp (StringBuilder builder, IEnumerable<Node> nodes, int level)
        {
            foreach (var idxNode in nodes) {
                var idxLevel = level;
                while (idxLevel-- > 0) {
                    builder.Append ("  ");
                }
                AppendName (builder, idxNode);
                AppendType (builder, idxNode);
                AppendValue (builder, idxNode);
                builder.Append ("\r\n");
                Nodes2Hyperlisp (builder, idxNode.Children, level + 1);
            }
        }

        /*
         * Appends node's name to Hyperlambda StringBuilder output
         */
        private void AppendName (StringBuilder builder, Node node)
        {
            if (node.Name.Contains ("\n")) {
                builder.Append (string.Format (@"@""{0}""", node.Name.Replace (@"""", @"""""")));
            } else if ((node.Name == "" && node.Value == null) || 
                       node.Name.Contains (":") || node.Name.Trim () != node.Name) {
                builder.Append (string.Format (@"""{0}""", node.Name.Replace (@"""", @"\""")));
            } else {
                builder.Append (string.Format ("{0}", node.Name));
            }
        }

        /*
         * Appends node's type to Hyperlambda StringBuilder output
         */
        private void AppendType (StringBuilder builder, Node node)
        {
            if (node.Value == null)
                return; // no type information here

            var type = node.Value.GetType ();
            if (type == typeof (string))
                return; // String is "default" type information

            builder.Append (
                string.Format (":{0}",
                    _context.Raise (
                        "p5.hyperlambda.get-type-name." + node.Value.GetType (),
                        new Node ()).Get<string> (_context)));
        }

        /*
         * Appends node's value to Hyperlambda StringBuilder output
         */
        private void AppendValue (StringBuilder builder, Node node)
        {
            if (node.Value == null)
                return; // Nothing to append here

            var value = Utilities.Convert<string> (_context, node.Value, null, true);
            if (value.Contains ("\n") || value.Contains ("\"")) {
                builder.Append (string.Format (@":@""{0}""", value.Replace (@"""", @"""""")));
            } else if (value.Contains (":") || value.Trim () != value) {
                builder.Append (string.Format (@":""{0}""", value.Replace (@"""", @"\""")));
            } else {
                builder.Append (string.Format (":{0}", value));
            }
        }
    }
}
