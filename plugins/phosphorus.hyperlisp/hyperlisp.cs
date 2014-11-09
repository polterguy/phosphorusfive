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
            string[] tokens = TokenizeHyperlisp (e.Args.Get<string> ());
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
                builder.Append (string.Format ("{0}", idx.Name));
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
            code = (code ?? "").TrimStart ();
            List<string> retVal = new List<string> ();
            retVal.Add ("");
            using (TextReader reader = new StringReader (code)) {
                string buffer = reader.ReadLine ();
                while (buffer != null) {
                    bool eos = false;
                    bool eon = false;
                    for (int idxNo = 0; idxNo < buffer.Length; idxNo++) {
                        if (!eos && buffer [idxNo] == ' ') {
                            if (retVal [retVal.Count - 1] == " ")
                                retVal [retVal.Count - 1] += " ";
                            else
                                retVal.Add (" ");
                        } else {
                            if (!eos) {
                                retVal.Add ("");
                                eos = true;
                            }
                            if (!eon && buffer [idxNo] != ':') {
                                retVal [retVal.Count - 1] += buffer [idxNo];
                            } else {
                                if (!eon) {
                                    retVal.Add (":");
                                    retVal.Add ("");
                                    eon = true;
                                } else {
                                    retVal [retVal.Count - 1] += buffer [idxNo];
                                }
                            }
                        }
                    }
                    if (retVal [retVal.Count - 1].StartsWith (@"@""")) {
                        buffer = retVal [retVal.Count - 1].Replace (@"""""", @"""");
                        while ((buffer.Length - buffer.TrimEnd (new char[]{ '"' }).Length) % 2 != 1) {
                            buffer += "\r\n" + reader.ReadLine ().Replace (@"""""", @"""");
                        }
                        retVal [retVal.Count - 1] = buffer.Substring (2, buffer.Length - 3); // removing @" start and " end parts
                    } else {
                        retVal [retVal.Count - 1] = retVal [retVal.Count - 1].Trim ();
                    }
                    buffer = reader.ReadLine ();
                }
            }
            retVal.RemoveAt (0);
            return retVal.ToArray ();
        }

        /*
         * responsible for parsing tokenized hyperlisp syntax and create Node structure from it
         */
        private static int NodeFromHyperlispTokens (Node node, string[] tokens, int level, int start)
        {
            int noSpace = 0;
            bool eon = false;
            for (int idxCur = start; idxCur < tokens.Length; idxCur++) {
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
                            noSpace = 0;
                            eon = false;
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

