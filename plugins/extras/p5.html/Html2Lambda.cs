/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Web;
using System.Linq;
using HtmlAgilityPack;
using p5.exp;
using p5.core;

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
        [ActiveEvent (Name = "p5.html.html2lambda")]
        public static void p5_html_html2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args, true)) {

                // Loops through all documents we're supposed to transform
                foreach (var idxHtmlDoc in XUtil.Iterate<string> (context, e.Args)) {

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
        static void ParseHtmlDocument (Node resultNode, HtmlNode htmlNode)
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
