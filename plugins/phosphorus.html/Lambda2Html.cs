/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System.Text;
using System.Collections.Generic;
using HtmlAgilityPack;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.html
{
    /// <summary>
    ///     Class to help transform pf.lambda trees to HTML.
    /// </summary>
    public static class Lambda2Html
    {
        /// <summary>
        ///     Creates an HTML document from the given pf.lambda structure.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.html.lambda2html")]
        private static void pf_html_lambda2html (ApplicationContext context, ActiveEventArgs e)
        {
            StringBuilder builder = new StringBuilder ();
            if (e.Args.Value != null) {
                Convert (XUtil.Iterate<Node> (e.Args.Value, e.Args, context, false), builder, 0, context);
            } else {
                Convert (e.Args.Children, builder, 0, context);
            }
            e.Args.Value = builder.ToString ().Trim ();
        }

        private static bool Convert (IEnumerable<Node> nodes, StringBuilder builder, int level, ApplicationContext context)
        {
            bool retVal = false;
            foreach (Node idxNode in nodes) {
                retVal = true;
                builder.Append ("\r\n");
                for (int idxSpacer = 0; idxSpacer < level; idxSpacer++) {
                    builder.Append (' ', 4);
                }
                builder.Append (string.Format ("<{0}{1}>", idxNode.Name, GetAttributes (idxNode)));
                bool hadChildren = Convert (idxNode.FindAll (idx => !idx.Name.StartsWith ("@")), builder, level + 1, context);
                if (hadChildren) {
                    builder.Append ("\r\n");
                    for (int idxSpacer = 0; idxSpacer < level; idxSpacer++) {
                        builder.Append (' ', 4);
                    }
                } else {
                    builder.Append ((idxNode.Get<string> (context) ?? "").Replace ("&", "&amp;").Replace ("<", "&lt;").Replace (">", "&gt;"));
                }
                builder.Append (string.Format ("</{0}>", idxNode.Name));
            }
            return retVal;
        }

        private static string GetAttributes (Node node)
        {
            string retVal = "";
            foreach (Node idxAtr in node.FindAll (idx => idx.Name.StartsWith ("@"))) {
                retVal += " " + idxAtr.Name.Substring (1) + "=\"" + idxAtr.Value + "\"";
            }
            return retVal;
        }
    }
}