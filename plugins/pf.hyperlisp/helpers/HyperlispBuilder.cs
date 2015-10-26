/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Text;
using pf.core;

/// <summary>
///     Contains common helper methods for Hyperlisp.
/// 
///     Contains all the common helper methods necessary to create and parse Hyperlisp.
/// </summary>
namespace phosphorus.hyperlisp.helpers
{
    /// <summary>
    ///     Class encapsulating internals of creation of Hyperlisp.
    /// 
    ///     Class containing actual implementation of logic behind the [pf.hyperlisp.lambda2hyperlisp] Active Event.
    /// </summary>
    public class HyperlispBuilder
    {
        private readonly ApplicationContext _context;
        private readonly IEnumerable<Node> _nodes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HyperlispBuilder" /> class.
        /// </summary>
        /// <param name="context">Application context object.</param>
        /// <param name="nodes">Nodes to convert into Hyperlisp.</param>
        public HyperlispBuilder (ApplicationContext context, IEnumerable<Node> nodes)
        {
            _context = context;
            _nodes = nodes;
        }

        /// <summary>
        ///     Parses and retrieves the Hyperlisp.
        /// 
        ///     Will convert the given pf.lambda node structure to Hyperlisp and return to caller.
        /// </summary>
        /// <value>hyperlisp</value>
        public string Hyperlisp
        {
            get
            {
                var builder = new StringBuilder ();
                Nodes2Hyperlisp (builder, _nodes, 0);
                return builder.ToString ().TrimEnd ();
            }
        }

        /*
         * recursively invoked for every "level" in node hierarchy
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
         * appends node's name to hyperlisp stringbuilder output
         */
        private void AppendName (StringBuilder builder, Node node)
        {
            if (node.Name.Contains ("\n")) {
                builder.Append (string.Format (@"@""{0}""", node.Name.Replace (@"""", @"""""")));
            } else if ((node.Name == string.Empty && node.Value == null) || 
                       node.Name.Contains (":") || node.Name.Trim () != node.Name) {
                builder.Append (string.Format (@"""{0}""", node.Name.Replace (@"""", @"\""")));
            } else {
                builder.Append (string.Format ("{0}", node.Name));
            }
        }

        /*
         * appends node's type to hyperlisp stringbuilder output
         */
        private void AppendType (StringBuilder builder, Node node)
        {
            if (node.Value == null)
                return; // no type information here

            var type = node.Value.GetType ();
            if (type == typeof (string))
                return; // string is "default" type information

            builder.Append (
                string.Format (":{0}",
                    _context.Raise (
                        "pf.hyperlisp.get-type-name." + node.Value.GetType (),
                        new Node ()).Get<string> (_context)));
        }

        /*
         * appends node's value to hyperlisp stringbuilder output
         */
        private void AppendValue (StringBuilder builder, Node node)
        {
            if (node.Value == null)
                return; // nothing to append here

            var value = node.StringEncodeValue (_context, true);
            if (value.Contains ("\n")) {
                builder.Append (string.Format (@":@""{0}""", value.Replace (@"""", @"""""")));
            } else if (value.Contains (":") || value.Trim () != value) {
                builder.Append (string.Format (@":""{0}""", value.Replace (@"""", @"\""")));
            } else {
                builder.Append (string.Format (":{0}", value));
            }
        }
    }
}
