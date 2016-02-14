/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using System.Xml;
using System.Linq;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for handling HTML
/// </summary>
namespace p5.xml
{
    /// <summary>
    ///     Class to help transform HTML to a p5 lambda structure
    /// </summary>
    public static class Xml2Lambda
    {
        /// <summary>
        ///     Parses an HTML document, and creates a p5 lambda node structure from the results
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "xml2lambda", Protection = EventProtection.LambdaClosed)]
        public static void xml2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Loops through all documents we're supposed to transform
                foreach (var idxHtmlDoc in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Converting currently iterated document/fragment
                    var doc = new XmlDocument ();
                    doc.LoadXml (idxHtmlDoc);
                    ParseXmlDocument (e.Args.Add ("result").LastChild, doc.DocumentElement);
                }
            }
        }

        /*
         * Helper for above, recursively parses HTML node given
         */
        private static void ParseXmlDocument (Node resultNode, XmlNode xmlNode)
        {
            // Adding all attributes
            if (xmlNode.Attributes != null) {
                foreach (XmlAttribute ix in xmlNode.Attributes) {
                    resultNode.Add (new Node (ix.LocalName, ix.Value));
                }
            }

            // Then the name of HTML element
            resultNode.Name = xmlNode.Name;
            if (xmlNode.ChildNodes.Count == 1 && xmlNode.ChildNodes [0].Name == "#text") {

                // This is a "simple node", with no children, only HTML content
                resultNode.Value = xmlNode.ChildNodes [0].InnerText;
            } else {

                // Then looping through each child HTML element
                foreach (XmlNode idxChild in xmlNode.ChildNodes) {

                    // We don't add comments or empty elements
                    if (idxChild.LocalName != "#comment") {
                        resultNode.Add (new Node ());
                        ParseXmlDocument (resultNode.LastChild, idxChild);
                    }
                }
            }
        }
    }
}
