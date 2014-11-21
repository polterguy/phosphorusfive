/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.hyperlisp
{
    /// <summary>
    /// class to help transform between hyperlisp and <see cref="phosphorus.core.Node"/> 
    /// </summary>
    public static class hyperlisp
    {
        /// <summary>
        /// helper to transform from hyperlisp code syntax to <see cref="phosphorus.core.Node"/> tree structure
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.hyperlisp-2-nodes")]
        private static void pf_hyperlisp_2_nodes (ApplicationContext context, ActiveEventArgs e)
        {
            string[] tokens = TokenizeHyperlisp (e.Args.Get<string> ("").TrimStart ());
            Node node = new Node ();
            NodeFromHyperlispTokens (context, node, tokens, 0, 0);
            e.Args.AddRange (node.Children);
        }

        /// <summary>
        /// helper to transform from <see cref="phosphorus.core.Node"/> tree structure to hyperlisp code syntax
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.nodes-2-hyperlisp")]
        private static void pf_nodes_2_hyperlisp (ApplicationContext context, ActiveEventArgs e)
        {
            StringBuilder builder = new StringBuilder ();
            Nodes2Hyperlisp (builder, e.Args.Children, 0, context);
            if (builder.Length == 0) {
                e.Args.Value = null;
            } else {
                string value = builder.ToString ();
                e.Args.Value = value.TrimEnd ('\r', '\n'); // getting rid of last carriage return
            }
        }

        /// <summary>
        /// helper to transform from <see cref="phosphorus.core.Node"/> tree structure to hyperlisp code syntax
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.node-2-hyperlisp")]
        private static void pf_node_2_hyperlisp (ApplicationContext context, ActiveEventArgs e)
        {
            StringBuilder builder = new StringBuilder ();
            Nodes2Hyperlisp (builder, new Node[] { e.Args }, 0, context);
            if (builder.Length == 0) {
                e.Args.Value = null;
            } else {
                string value = builder.ToString ();
                e.Args.Value = value.Substring (0, value.Length - 2); // getting rid of last carriage return
            }
        }

        /*
         * responsible for creating hyperlisp code from node tree structure
         */
        private static void Nodes2Hyperlisp (StringBuilder builder, IEnumerable<Node> nodes, int level, ApplicationContext context)
        {
            foreach (Node idx in nodes) {
                int idxLevel = level;
                while (idxLevel-- > 0) {
                    builder.Append ("  ");
                }
                string name = idx.Name;
                if (name.Contains ("\r") || name.Contains ("\n")) {
                    builder.Append (string.Format (@"@""{0}""", name.Replace (@"""", @"""""")));
                } else if ((name == string.Empty && idx.Value == null) || name.Contains (":") || name.Trim () != name) {
                    builder.Append (string.Format (@"""{0}""", name.Replace (@"""", @"\""")));
                } else {
                    builder.Append (string.Format ("{0}", name));
                }
                if (idx.Value is Node) {
                    Node nodeValue = idx.Get<Node> ();
                    if (nodeValue == idx.Find (nodeValue.Path)) {

                        // using DNA when constructing value, since this is a reference to within the tree itself
                        builder.Append (string.Format (":path:{0}", nodeValue.Path));
                    } else {

                        // must convert node to hyperlisp, since it's not from "this tree"
                        Node tmpCode = new Node ("root");
                        tmpCode.Add (nodeValue.Clone ());
                        context.Raise ("pf.nodes-2-hyperlisp", tmpCode);
                        builder.Append (string.Format (@":node:@""{0}""", tmpCode.Get<string> ().Replace (@"""", @"""""")));
                    }
                } else if (idx.Value != null) {
                    object value = idx.Value;
                    string typeInfo = null;
                    if (!(value is string)) {
                        value = ConvertToString (context, value, out typeInfo);
                        builder.Append (string.Format (@":{0}", typeInfo));
                    }
                    string strValue = value as string;
                    if (strValue.Contains ("\r") || strValue.Contains ("\n") || strValue.Trim () != strValue) {
                        builder.Append (string.Format (@":@""{0}""", strValue.Replace (@"""", @"""""")));
                    } else {
                        builder.Append (string.Format (":{0}", strValue));
                    }
                }
                builder.Append ("\r\n");
                Nodes2Hyperlisp (builder, idx.Children, level + 1, context);
            }
        }

        /*
         * responsible for tokenizing hyperlisp code syntax
         */
        private static string[] TokenizeHyperlisp (string code)
        {
            List<string> tokens = new List<string> ();
            int idxNo = 0;
            string previousToken = null;
            string token = GetNextToken (code, ref idxNo, previousToken);
            while (token != null) {
                tokens.Add (token);
                previousToken = token;
                token = GetNextToken (code, ref idxNo, previousToken);
            }
            return tokens.ToArray ();
        }

        /*
         * reads next token from code
         */
        private static string GetNextToken (string code, ref int index, string previousToken)
        {
            if (index >= code.Length)
                return null;

            char tmp = code [index];
            StringBuilder builder = new StringBuilder ();
            switch (tmp) {
                case ':':
                    if (previousToken == null || previousToken == "  " || previousToken == "\r\n") {
                        return "";
                    }
                    index++;
                    return ":";
                case ' ':
                    if (previousToken != ":") {
                        if (code [index + 1] != ' ')
                            throw new ArgumentException ("syntax error in hyperlisp, to few spaces in separator");
                        index += 2;
                        return "  ";
                    }
                    index += 1;
                    builder.Append (tmp);
                    break;
                case '\r':
                    index += 2;
                    return "\r\n";
                case '\n':
                    index += 1;
                    return "\r\n";
                case '@':
                    if (code.Length > index + 1 && code [index + 1] == '"')
                        return Utilities.GetStringToken (code, ref index);
                    index += 1;
                    builder.Append ('@');
                    break;
                case '"':
                    return Utilities.GetStringToken (code, ref index);
                default:
                    index += 1;
                    builder.Append (tmp);
                    break;
            }
            bool finished = false;
            while (true) {
                tmp = code [index];
                switch (tmp) {
                case ':':
                    finished = true;
                    break;
                case '\r':
                    finished = true;
                    break;
                case '\n':
                    finished = true;
                    break;
                default:
                    index += 1;
                    builder.Append (tmp); // appending character to token
                    break;
                }
                if (finished || index >= code.Length)
                    break;
            }
            return builder.ToString ().Trim ();
        }
        
        /*
         * responsible for parsing tokenized hyperlisp syntax and create Node structure from it
         */
        private static int NodeFromHyperlispTokens (ApplicationContext context, Node node, string[] tokens, int level, int start)
        {
            int noSpace = 0;
            bool eon = false;
            for (int idxCur = start; idxCur < tokens.Length; idxCur++) {
                if (tokens [idxCur] == "\r\n") {
                    noSpace = 0;
                    eon = false;
                    continue;
                }
                if (tokens [idxCur] == "  ") {
                    if (eon) {
                        eon = false;
                        noSpace = 1;
                    } else {
                        noSpace += 1;
                    }
                } else {
                    if (noSpace == level) {
                        if (!eon) {
                            node.Add (new Node ());
                            string name = tokens [idxCur];
                            node [node.Count - 1].Name = name;
                            eon = true;
                            if (idxCur < tokens.Length && tokens.Length > idxCur + 1 && tokens [idxCur + 1] == ":")
                                idxCur += 1;
                            else
                                noSpace = 0;
                        } else {
                            string value = tokens [idxCur];
                            if (value != ":") {
                                if (node [node.Count - 1].Value != null) {
                                    // existing value is type information
                                    string typeInfo = node [node.Count - 1].Get<string> ();
                                    node [node.Count - 1].Value = ConvertValue (context, typeInfo, value);
                                } else {
                                    node [node.Count - 1].Value = value;
                                }
                            }
                        }
                    } else {
                        if (noSpace < level) {
                            return idxCur - noSpace;
                        } else {
                            idxCur = NodeFromHyperlispTokens (context, node [node.Count - 1], tokens, level + 1, idxCur - noSpace) - 1;
                            noSpace = 0;
                            eon = false;
                        }
                    }
                }
            }
            return tokens.Length;
        }

        /*
         * converts given "value" according to "typeInfo" and returns object representing value converted from string to actual type
         */
        private static object ConvertValue (ApplicationContext context, string typeInfo, string value)
        {
            switch (typeInfo) {
            case "string":
                return value; // string is implicit
            case "path":
                return new Node.DNA (value);
            case "int":
                return int.Parse (value, CultureInfo.InvariantCulture);
            case "bool":
                return bool.Parse (value);
            case "float":
                return float.Parse (value, CultureInfo.InvariantCulture);
            case "double":
                return double.Parse (value, CultureInfo.InvariantCulture);
            case "decimal":
                return decimal.Parse (value, CultureInfo.InvariantCulture);
            case "byte":
                return byte.Parse (value, CultureInfo.InvariantCulture);
            case "char":
                return char.Parse (value);
            case "date":
                return DateTime.ParseExact (value, "yyyy.MM.dd", CultureInfo.InvariantCulture);
            case "datetime":
                return DateTime.ParseExact (value, "yyyy.MM.ddTHH:mm:ss", CultureInfo.InvariantCulture);
            case "timespan":
                return TimeSpan.Parse (value, CultureInfo.InvariantCulture);
            default:
                Node tmp = new Node (string.Empty, value);
                if (typeInfo == "node") {
                    context.Raise ("pf.code-2-nodes", tmp);
                    tmp.Value = null;
                    if (tmp.Count > 1)
                        throw new ArgumentException ("you cannot represent nodes as a string literal inside a hyperlisp file unless you have only one 'root node' in your hierarchy");
                    else if (tmp.Count == 0)
                        return null;
                    return tmp [0].Untie ();
                } else {
                    context.Raise (typeInfo, tmp);
                    return tmp.Value;
                }
            }
        }

        /*
         * converts given value to its string representation
         */
        private static string ConvertToString (ApplicationContext context, object value, out string typeInfo)
        {
            switch (value.GetType ().FullName) {
            case "System.Int32":
                typeInfo = "int";
                return Convert.ToString (value, CultureInfo.InvariantCulture);
            case "System.Boolean":
                typeInfo = "bool";
                return Convert.ToString (value, CultureInfo.InvariantCulture);
            case "System.Single":
                typeInfo = "float";
                return Convert.ToString (value, CultureInfo.InvariantCulture);
            case "System.Double":
                typeInfo = "double";
                return Convert.ToString (value, CultureInfo.InvariantCulture);
            case "System.Decimal":
                typeInfo = "decimal";
                return Convert.ToString (value, CultureInfo.InvariantCulture);
            case "System.Byte":
                typeInfo = "byte";
                return Convert.ToString (value, CultureInfo.InvariantCulture);
            case "System.Char":
                typeInfo = "char";
                return Convert.ToString (value, CultureInfo.InvariantCulture);
            case "System.DateTime":
                DateTime date = (DateTime)value;
                if (date.Hour == 0 && date.Minute == 0 && date.Second == 0) {
                    typeInfo = "date";
                    return date.ToString ("yyyy.MM.dd", CultureInfo.InvariantCulture);
                }
                typeInfo = "datetime";
                return date.ToString ("yyyy.MM.ddTHH:mm:ss", CultureInfo.InvariantCulture);
            case "System.TimeSpan":
                typeInfo = "timespan";
                return ((TimeSpan)value).ToString ();
            default:
                Node tmp = new Node (string.Empty, value);
                context.Raise (value.GetType ().FullName + ".to-string", tmp);
                typeInfo = tmp [0].Get<string> ();
                return tmp.Get<string> ();
            }
        }
    }
}

