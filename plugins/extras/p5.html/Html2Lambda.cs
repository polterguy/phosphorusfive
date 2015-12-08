/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using HtmlAgilityPack;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for handling HTML
/// </summary>
namespace p5.html
{
    /// <summary>
    ///     Class to help transform HTML to a p5 lambda structure
    /// </summary>
    public static class Html2Lambda
    {
        static Html2Lambda ()
        {
            // Making sure "form" element conforms to relational DOM structure
            HtmlNode.ElementsFlags.Remove ("form");
        }

        /// <summary>
        ///     Parses an HTML document, and creates a p5 lambda node structure from the results
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.html.html2lambda", Protection = EventProtection.LambdaClosed)]
        private static void p5_html_html2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Loops through all documents we're supposed to transform
                foreach (var idxHtmlDoc in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Converting currently iterated document/fragment
                    var doc = new HtmlDocument ();
                    doc.LoadHtml (idxHtmlDoc);
                    ParseHtmlDocument (e.Args, doc.DocumentNode);
                }
            }
        }

        /*
         * Helper for above, recursively parses HTML node given
         */
        private static void ParseHtmlDocument (Node resultNode, HtmlNode htmlNode)
        {
            // Skipping document node
            if (htmlNode.Name != "#document") {

                // Looping through each attribute
                foreach (var idxAtr in htmlNode.Attributes) {
                    resultNode.Add (new Node ("@" + idxAtr.Name, idxAtr.Value));
                }

                // Then the name of HTML element
                resultNode.Name = htmlNode.Name;
                if (htmlNode.ChildNodes.Count == 1 && htmlNode.ChildNodes [0].Name == "#text") {

                    // This is a "simple node", with no children, only HTML content
                    resultNode.Value = htmlNode.InnerText.Replace ("&lt;", "<").Replace ("&gt;", ">").Replace ("&amp;", "&");
                    return; // don't care about children
                }
            }

            // Then looping through each child HTML element
            foreach (var idxChild in htmlNode.ChildNodes) {

                // We don't add comments or empty elements
                if (idxChild.Name != "#comment" &&
                    (idxChild.HasAttributes || idxChild.HasChildNodes || !string.IsNullOrEmpty (idxChild.InnerText.Trim ()))) {
                    resultNode.Add (new Node ());
                    ParseHtmlDocument (resultNode.LastChild, idxChild);
                }
            }
        }
    }
}
