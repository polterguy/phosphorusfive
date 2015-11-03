/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Text;
using System.Collections.Generic;
using HtmlAgilityPack;
using p5.core;
using p5.exp;

namespace p5.html
{
    /// <summary>
    ///     Class to help transform p5.lambda trees to HTML.
    /// 
    ///     Encapsulates the [p5.html.lambda2html] Active Event, and its associated helper methods.
    /// </summary>
    public static class Lambda2Html
    {
        /// <summary>
        ///     Creates an HTML/XML document from the given p5.lambda structure.
        /// 
        ///     Will create an HTML, or XML document, from the given p5.lambda structure, reversing the process
        ///     from [p5.html.html2lambda].
        /// 
        ///     Any child node who's name starts with an "@" character, will be transformed into an HTML/XML attribute,
        ///     while all other nodes, will be assumed to be children nodes, and the resulting "value" their content.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.html.lambda2html")]
        private static void p5_html_lambda2html (ApplicationContext context, ActiveEventArgs e)
        {
            StringBuilder builder = new StringBuilder ();
            if (e.Args.Value != null) {
                Convert (XUtil.Iterate<Node> (e.Args.Value, e.Args, context, false), builder, 0, context);
            } else {
                Convert (e.Args.Children, builder, 0, context);
            }
            e.Args.Value = builder.ToString ().Trim ();
        }

        /*
         * helper for above
         */
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

        /*
         * helper method, to parse and retrieve attributes
         */
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
