
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using HtmlAgilityPack;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.web
{
    /// <summary>
    /// helper to semantically manipulate or view HTML documents
    /// </summary>
    public static class html
    {
        /// <summary>
        /// parses an HTML document and creates a pf.lambda structure from it
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.html.parse")]
        private static void pf_html_parse (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure "form" element conforms to relational structure
            HtmlNode.ElementsFlags.Remove ("form");

            Expression.Iterate<string> (e.Args, true, 
            delegate (string idx) {
                HtmlDocument doc = new HtmlDocument ();
                doc.LoadHtml (idx);
                e.Args.Add (new Node ());
                ParseHtmlDocument (e.Args.LastChild, doc.DocumentNode);
            });
        }

        /*
         * helper for above, recursively parses HTML node given
         */
        private static void ParseHtmlDocument (Node res, HtmlNode cur)
        {
            res.Name = cur.Name;
            if (!cur.HasChildNodes && !string.IsNullOrEmpty(cur.InnerText.Trim())) {
                res.Value = cur.InnerText;
            }
            foreach (var idxAtr in cur.Attributes) {
                res.Add (new Node ("@" + idxAtr.Name, idxAtr.Value));
            }
            bool first = true;
            foreach (var idxChild in cur.ChildNodes) {
                if (idxChild.Name != "#comment" && 
                    (idxChild.HasAttributes || idxChild.HasChildNodes || !string.IsNullOrEmpty(idxChild.InnerText.Trim()))) {
                    if (first) {
                        first = false;
                        res.Add (new Node ("children"));
                    }
                    res.LastChild.Add (new Node ());
                    ParseHtmlDocument (res.LastChild.LastChild, idxChild);
                }
            }
        }
    }
}
