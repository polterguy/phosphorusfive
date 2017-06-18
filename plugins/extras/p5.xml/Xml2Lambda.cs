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

using System.Xml;
using p5.exp;
using p5.core;

namespace p5.xml
{
    /// <summary>
    ///     Class to help transform XML to a p5.lambda structure
    /// </summary>
    public static class Xml2Lambda
    {
        /// <summary>
        ///     Parses an XML document, and creates a p5.lambda node structure from the results
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "xml2lambda")]
        public static void xml2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args, true)) {

                // Loops through all documents we're supposed to transform
                foreach (var idxXmlDoc in XUtil.Iterate<string> (context, e.Args)) {

                    // Converting currently iterated document/fragment
                    var doc = new XmlDocument ();
                    doc.LoadXml (idxXmlDoc);
                    ParseXmlNode (e.Args.Add ("result").LastChild, doc.DocumentElement);
                }
            }
        }

        /*
         * Helper for above, recursively parses HTML node given
         */
        private static void ParseXmlNode (Node resultNode, XmlNode xmlNode)
        {
            // Adding all attributes
            if (xmlNode.Attributes != null) {
                foreach (XmlAttribute ix in xmlNode.Attributes) {
                    resultNode.Add (new Node ("@" + ix.LocalName, ix.Value));
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
                        ParseXmlNode (resultNode.LastChild, idxChild);
                    }
                }
            }
        }
    }
}
