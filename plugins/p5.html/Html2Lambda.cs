/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using HtmlAgilityPack;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for handling HTML.
/// 
///     Contains all Active Events related to the process of parsing, and creating HTML.
/// </summary>
namespace p5.html
{
    /// <summary>
    ///     Class to help transform HTML to a p5.lambda node structure.
    /// 
    ///     Encapsulates the [p5.html.html2lambda] Active Event, and its associated helper methods.
    /// </summary>
    public static class Html2Lambda
    {
        static Html2Lambda ()
        {
            // making sure "form" element conforms to relational structure
            HtmlNode.ElementsFlags.Remove ("form");
        }

        /// <summary>
        ///     Parses an HTML document, and creates a p5.lambda node structure from the results.
        /// 
        ///     Allows you to parse HTML, and create a semantic p5.lambda node structure from the results.
        /// 
        ///     Example that downloads the main landing page from Digg.com, creates a lambda structure, extracts
        ///     all (distinct) hyperlink URLs starting with "http", and puts the results into the [_res] node. Afterwards,
        ///     it removes the downloaded HTML and the intermediate p5.lambda structure created by the HTML;
        /// 
        ///     <pre>_res
        /// p5.web.get:"http://digg.com"
        /// p5.html.html2lambda:@/-/"*"?value
        /// append:@/../"*"/_res/?node
        ///   source:@/./-/"**"/"/@href/"/"=/^http/d"?node
        /// set:@/-3|/-2|/-1?node</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.html.html2lambda")]
        private static void p5_html_html2lambda (ApplicationContext context, ActiveEventArgs e)
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
            resultNode.Name = htmlNode.Name == "#document" ? "doc" : htmlNode.Name;
            if (htmlNode.ChildNodes.Count == 1 && htmlNode.ChildNodes [0].Name == "#text") {

                // this is a "simple node", with no children, only HTML content
                resultNode.Value = htmlNode.InnerText.Replace ("&lt;", "<").Replace ("&gt;", ">").Replace ("&amp;", "&");
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
