/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
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
        [ActiveEvent (Name = "pf.hyperlisp-2-node")]
        private static void pf_hyperlisp_2_node (ApplicationContext context, ActiveEventArgs e)
        {
            string[] tokens = TokenizeHyperlisp (e.Args.Get<string> ("").TrimStart ());
            Node node = new Node ();
            NodeFromHyperlispTokens (node, tokens, 0, 0);
            e.Args.AddRange (node.Children);
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
            Node2Hyperlisp (builder, e.Args, 0);
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
        private static void Node2Hyperlisp (StringBuilder builder, Node node, int level)
        {
            foreach (Node idx in node.Children) {
                int idxLevel = level;
                while (idxLevel-- > 0) {
                    builder.Append ("  ");
                }
                string name = idx.Name;
                if (name.Contains ("\r") || name.Contains ("\n") || name.Contains (@"""") || name.Trim () != name || 
                    (name == string.Empty && idx.Value == null)) {
                    builder.Append (string.Format (@"@""{0}""", name.Replace (@"""", @"""""")));
                } else {
                    builder.Append (string.Format ("{0}", name));
                }
                string value = idx.Get<string> ();
                if (value != null) {
                    if (value.Contains ("\r\n") || value.Trim () != value) {
                        builder.Append (string.Format (@":@""{0}""", value.Replace (@"""", @"""""")));
                    } else {
                        builder.Append (string.Format (":{0}", value));
                    }
                }
                builder.Append ("\r\n");
                Node2Hyperlisp (builder, idx, level + 1);
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
                    if (code [index + 1] == '"')
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
        private static int NodeFromHyperlispTokens (Node node, string[] tokens, int level, int start)
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
                            if (idxCur < tokens.Length && tokens [idxCur + 1] == ":")
                                idxCur += 1;
                            else
                                noSpace = 0;
                        } else {
                            string value = tokens [idxCur];
                            node [node.Count - 1].Value = value;
                        }
                    } else {
                        if (noSpace < level) {
                            return idxCur - noSpace;
                        } else {
                            idxCur = NodeFromHyperlispTokens (node [node.Count - 1], tokens, level + 1, idxCur - noSpace) - 1;
                            noSpace = 0;
                            eon = false;
                        }
                    }
                }
            }
            return tokens.Length;
        }
    }
}

