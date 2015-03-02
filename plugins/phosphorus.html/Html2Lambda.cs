/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using HtmlAgilityPack;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.html
{
    /// <summary>
    ///     Class to help transform HTML to pf.lambda trees.
    /// </summary>
    public static class Html2Lambda
    {
        static Html2Lambda ()
        {
            // making sure "form" element conforms to relational structure
            HtmlNode.ElementsFlags.Remove ("form");
        }

        /// <summary>
        ///     Parses an HTML document, and creates a pf.lambda structure from it.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.html.html2lambda")]
        private static void pf_html_html2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // loops through all documents we're supposed to transform
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                var doc = new HtmlDocument ();
                doc.LoadHtml (idx);
                e.Args.Add (new Node ());
                ParseHtmlDocument (e.Args.LastChild, doc.DocumentNode);
            }
        }

        /*
         * helper for above, recursively parses HTML node given
         */
        private static void ParseHtmlDocument (Node resultNode, HtmlNode htmlNode)
        {
            // looping through each attribute
            foreach (var idxAtr in htmlNode.Attributes) {
                resultNode.Add (new Node ("@" + idxAtr.Name, idxAtr.Value));
            }

            // then the name of HTML element
            resultNode.Name = htmlNode.Name;
            if (htmlNode.ChildNodes.Count == 1 && htmlNode.ChildNodes [0].Name == "#text") {

                // this is a "simple node", with no children, only HTML content
                resultNode.Value = htmlNode.InnerText;
                return; // don't care about children
            }

            // then looping through each child HTML element
            foreach (var idxChild in htmlNode.ChildNodes) {

                // we don't add comments or empty elements
                if (idxChild.Name != "#comment" &&
                    (idxChild.HasAttributes || idxChild.HasChildNodes || !string.IsNullOrEmpty (idxChild.InnerText.Trim ()))) {
                    resultNode.Add (new Node ());
                    ParseHtmlDocument (resultNode.LastChild, idxChild);
                }
            }
        }
    }
}