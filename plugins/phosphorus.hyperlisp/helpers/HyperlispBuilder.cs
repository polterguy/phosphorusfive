
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Text;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.hyperlisp
{
    /// <summary>
    /// class responsible for creating hyperlisp from a <see cref="phosphorus.core.Node"/> list
    /// </summary>
    public class HyperlispBuilder
    {
        private ApplicationContext _context;
        private IEnumerable<Node> _nodes;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.hyperlisp.HyperlispBuilder"/> class
        /// </summary>
        /// <param name="context">application context object</param>
        /// <param name="nodes">nodes to convert into hyperlisp</param>
        public HyperlispBuilder (ApplicationContext context, IEnumerable<Node> nodes)
        {
            _context = context;
            _nodes = nodes;
        }

        /// <summary>
        /// retrieves the hyperlisp
        /// </summary>
        /// <value>hyperlisp</value>
        public string Hyperlisp {
            get {
                StringBuilder builder = new StringBuilder ();
                Nodes2Hyperlisp (builder, _nodes, 0);
                return builder.ToString ().TrimEnd ('\r', '\n');
            }
        }

        /*
         * recursively invoked for every "level" in node hierarchy
         */
        private void Nodes2Hyperlisp (StringBuilder builder, IEnumerable<Node> nodes, int level)
        {
            foreach (Node idxNode in nodes) {
                int idxLevel = level;
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
            } else if ((node.Name == string.Empty && node.Value == null) || node.Name.Contains (":") || node.Name.Trim () != node.Name) {
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

            Type type = node.Value.GetType ();
            if (type == typeof(string))
                return; // string is "default" type information

            string activeEventName = "pf.hyperlist.get-type-name." + node.Value.GetType ();
            Node typeNode = new Node ();
            _context.Raise (activeEventName, typeNode);
            if (typeNode.Value == null) {
                throw new ArgumentException ("cannot convert type; '" + 
                    type.FullName + 
                    "' to hyperlisp since no converter exist. make sure you create an Active Event called; '" +
                    "pf.hyperlisp.get-type-name." + node.Value.GetType () + 
                    "' that returns the hyperlisp typename for your type, such as 'int', 'decimal', 'node', etc");
            } else {
                builder.Append (string.Format (":{0}", typeNode.Get<string> ()));
            }
        }
        
        /*
         * appends node's value to hyperlisp stringbuilder output
         */
        private void AppendValue (StringBuilder builder, Node node)
        {
            if (node.Value == null)
                return; // nothing to append here

            Type type = node.Value.GetType ();
            string value = null;
            if (type == typeof(string)) {
                value = node.Value as string;
            } else {
                // notice that this will yield a "null invocation" for all native types that supports automatic conversion 
                // through IConvertible, unless a type converter Active Event is explicitly given. this means that the Get<string> () 
                // invocation after the null Active Event invocation will do its magic automatically, hence we don't need
                // type converters for anything that automatically supports conversion to string natively, in a *sane* way
                string activeEventName = "pf.hyperlist.get-string-value." + node.Value.GetType ();
                Node valueNode = new Node (string.Empty, node.Value);
                _context.Raise (activeEventName, valueNode);
                value = valueNode.Get<string> ();
            }
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
