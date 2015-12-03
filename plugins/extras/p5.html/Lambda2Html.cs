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
    ///     Class to help transform p5.lambda to HTML
    /// </summary>
    public static class Lambda2Html
    {
        /// <summary>
        ///     Creates an HTML document from the given p5.lambda object
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.html.lambda2html", Protection = EntranceProtection.Lambda)]
        private static void p5_html_lambda2html (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Used as buffer when converting from lambda to HTML
                StringBuilder builder = new StringBuilder ();

                // Doing actual conversion from lambda to HTML
                Convert (context, XUtil.Iterate<Node> (context, e.Args), builder);

                // Returning HTML to caller
                e.Args.Value = builder.ToString ().Trim ();
            }
        }

        /*
         * helper for above
         */
        private static bool Convert (
            ApplicationContext context, 
            IEnumerable<Node> nodes, 
            StringBuilder builder, 
            int level = 0)
        {
            bool retVal = false;
            foreach (Node idxNode in nodes) {
                retVal = true;
                builder.Append ("\r\n");
                for (int idxSpacer = 0; idxSpacer < level; idxSpacer++) {
                    builder.Append (' ', 4);
                }
                builder.Append (string.Format ("<{0}{1}>", idxNode.Name, GetAttributes (idxNode)));
                bool hadChildren = Convert (context, idxNode.FindAll (idx => !idx.Name.StartsWith ("@")), builder, level + 1);
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
