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
    /// class responsible for creating hyperlisp from a <see cref="phosphorus.core.Node"/> hierarchy
    /// </summary>
    public static class HyperlispBuilder
    {
        /// <summary>
        /// creates hyperlisp from a <see cref="phosphorus.core.Node"/> hierarchy
        /// </summary>
        /// <param name="builder">where to put the resulting hyperlisp</param>
        /// <param name="nodes">which nodes to construct hyperlisp from</param>
        /// <param name="context">application context</param>
        public static StringBuilder Nodes2Hyperlisp (ApplicationContext context, IEnumerable<Node> nodes)
        {
            StringBuilder builder = new StringBuilder ();
            Nodes2Hyperlisp (context, builder, nodes, 0);
            return builder;
        }

        /*
         * recursively invoked for every "level" in node hierarchy
         */
        private static void Nodes2Hyperlisp (ApplicationContext context, StringBuilder builder, IEnumerable<Node> nodes, int level)
        {
            foreach (Node idxNode in nodes) {
                int idxLevel = level;
                while (idxLevel-- > 0) {
                    builder.Append ("  ");
                }
                AppendName (builder, idxNode);
                AppendType (context, builder, idxNode);
                AppendValue (context, builder, idxNode);
                builder.Append ("\r\n");
                Nodes2Hyperlisp (context, builder, idxNode.Children, level + 1);
            }
        }

        /*
         * appends node's name to hyperlisp stringbuilder output
         */
        private static void AppendName (StringBuilder builder, Node node)
        {
            if (node.Name.Contains ("\r") || node.Name.Contains ("\n")) {
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
        private static void AppendType (ApplicationContext context, StringBuilder builder, Node node)
        {
            if (node.Value == null)
                return; // no type information here

            Type type = node.Value.GetType ();
            if (type == typeof(string))
                return; // string is "default" type information

            if (type == typeof(Node) && node.Get<Node> ().Root == node.Root) {
                // appending DNA Path and not node itself, since this is a reference node to another node inside the same tree
                builder.Append (":path");
                return;
            } else if (type == typeof(Node.DNA)) {
                builder.Append (":path");
                return;
            } else if (type == typeof(Node)) {
                builder.Append (":node");
                return;
            }

            string activeEventName = "pf.hyperlist.get-type-name." + node.Value.GetType ();
            Node typeNode = new Node ();
            context.Raise (activeEventName, typeNode);
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
        private static void AppendValue (ApplicationContext context, StringBuilder builder, Node node)
        {
            if (node.Value == null)
                return; // nothing to append here

            Type type = node.Value.GetType ();
            string value = null;
            if (type == typeof(string)) {
                value = node.Get <string> ();
            } else if (type == typeof(Node)) {

                // nodes are automatically handled, since they're native to hyperlisp, *obviously* ...!!
                if (node.Get<Node> ().Root == node.Root) {
                    // this is a "reference node", pointing to another node inside the same tree, hence we store the DNA
                    // and NOT the Node itself
                    value = node.Get<Node> ().Path.ToString ();
                } else {
                    // this is a "free node", meaning a node that is not a "reference node"
                    Node tmp = new Node ();
                    tmp.Add ((node.Value as Node).Clone ());
                    context.Raise ("pf.nodes-2-hyperlisp", tmp);
                    value = tmp.Value as string;
                }
            } else {
                // notice that this will yield a "null invocation" for all native types that supports automatic conversion 
                // through IConvertible unless a type converter Active Event is explicitly given. this means that the Get<string> () 
                // invocation after the null Active Event invocation will do its magic automatically, hence we don't need
                // type converters for anything that automatically supports conversion to string natively
                string activeEventName = "pf.hyperlist.get-string-value." + node.Value.GetType ();
                Node valueNode = new Node (string.Empty, node.Value);
                context.Raise (activeEventName, valueNode);
                value = valueNode.Get<string> ();
            }
            if (value.Contains ("\r") || value.Contains ("\n")) {
                builder.Append (string.Format (@":@""{0}""", value.Replace (@"""", @"""""")));
            } else if (value.Contains (":") || value.Trim () != value) {
                builder.Append (string.Format (@":""{0}""", value.Replace (@"""", @"\""")));
            } else {
                builder.Append (string.Format (":{0}", value));
            }
        }
    }
}

