/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
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
        /// helper to transform between hyperlisp code syntax and <see cref="phosphorus.core.Node"/> 
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.hl-2-node")]
        private static void hl_2_node (ApplicationContext context, ActiveEventArgs e)
        {
            string[] tokens = TokenizeHyperlisp (e.Args.Get<string> ());
            Node node = new Node ();
            NodeFromHyperlispTokens (node, tokens, 0, 0);
            e.Args.AddRange (node.Children);
        }

        /*
         * responsible for tokenizing hyperlisp code syntax
         */
        private static string[] TokenizeHyperlisp (string code)
        {
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
                        retVal [retVal.Count - 1] = retVal [retVal.Count - 1].Replace (@"""""", @"""");
                        buffer = retVal [retVal.Count - 1];
                        while ((buffer.Length - buffer.TrimEnd (new char[]{ '"' }).Length) % 2 != 1) {
                            buffer += "\r\n" + reader.ReadLine ().Replace (@"""""", @"""");
                        }
                        retVal [retVal.Count - 1] = buffer;
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
                            if (name.StartsWith (@""""))
                                name = name.Substring (1, name.Length - 2);
                            node [node.Count - 1].Name = name;
                            eon = true;
                            if (idxCur < tokens.Length && tokens [idxCur + 1] == ":")
                                idxCur += 1;
                            else
                                noSpace = 0;
                        } else {
                            string value = tokens [idxCur];
                            if (value.StartsWith (@"@"""))
                                value = value.Substring (2, value.Length - 3);
                            else if (value.StartsWith (@""""))
                                value = value.Substring (1, value.Length - 2);
                            node [node.Count - 1].Value = value;
                            noSpace = 0;
                        }
                    } else {
                        if (noSpace < level) {
                            return idxCur - noSpace;
                        } else {
                            idxCur = NodeFromHyperlispTokens (node [node.Count - 1], tokens, level + 1, idxCur - noSpace) - 1;
                            noSpace = 0;
                        }
                    }
                }
            }
            return tokens.Length;
        }
    }
}

