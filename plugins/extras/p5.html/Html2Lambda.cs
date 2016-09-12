/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using System.Linq;
using HtmlAgilityPack;
using p5.exp;
using p5.core;

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
        /// <summary>
        ///     Parses an HTML document, and creates a p5 lambda node structure from the results
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "html2lambda", Protection = EventProtection.LambdaClosed)]
        public static void html2lambda (ApplicationContext context, ActiveEventArgs e)
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
         * Static CTOR to make sure we set state of HtmlNode to respect form tag as relational element
         */
        static Html2Lambda ()
        {
            // Making sure "form" element conforms to relational DOM structure
            HtmlNode.ElementsFlags.Remove ("form");
        }

        /*
         * Helper for above, recursively parses HTML node given
         */
        private static void ParseHtmlDocument (Node resultNode, HtmlNode htmlNode)
        {
            // Skipping document node
            if (htmlNode.Name != "#document") {

                // Adding all attributes
                resultNode.AddRange (htmlNode.Attributes.Select (ix => new Node ("@" + ix.Name, HttpUtility.HtmlDecode (ix.Value))));

                // Then the name of HTML element
                resultNode.Name = htmlNode.Name;
                if (htmlNode.Name == "#text") {

                    // This is a "simple node", with no children, only HTML content
                    resultNode.Value = HttpUtility.HtmlDecode (htmlNode.InnerText);
                }
            }

            // Then looping through each child HTML element
            foreach (var idxChild in htmlNode.ChildNodes) {

                // We don't add comments or empty elements
                if (idxChild.Name != "#comment") {
                    if (idxChild.Name == "#text" && string.IsNullOrEmpty (idxChild.InnerText.Trim ()))
                        continue;
                    resultNode.Add (new Node ());
                    ParseHtmlDocument (resultNode.LastChild, idxChild);
                }
            }
        }
    }
}
