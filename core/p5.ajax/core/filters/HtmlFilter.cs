/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace p5.ajax.core.filters
{
    /// <summary>
    ///     An http response filter for rendering plain html back to client
    /// </summary>
    public class HtmlFilter : Filter
    {
        /// <summary>
        ///     Initializes a new instance of the HtmlFilter class
        /// </summary>
        /// <param name="manager">The manager this instance is rendering for</param>
        public HtmlFilter (Manager manager)
            : base (manager)
        { }

        /// <summary>
        ///     Renders the response
        /// </summary>
        /// <returns>The HTML response returned back to client</returns>
        protected override string RenderResponse ()
        {
            TextReader reader = new StreamReader (this, ContentEncoding);
            var content = reader.ReadToEnd ();
            content = RemoveViewState (content);
            content = CleanHead (content);
            content = IncludeStylesheetFiles (content);
            content = IncludeJavaScript (content);
            content = SendJavaScriptContent (content);
            return content;
        }

        /*
         * Cleans up head section.
         */
        private string CleanHead (string content)
        {
            // Buffer
            var builder = new StringBuilder ();
            builder.Append ("<!DOCTYPE html>\r\n<html>\r\n\t<head>" + "\r\n");

            // Figuring out head start and end.
            var indexOfHeadStart = content.IndexOf ("<head>") + 6;
            var indexOfHeadEnd = content.IndexOf ("</head>");

            // Retrieving entire <head></head> section, trimming, and removing every single CR/LF sequence.
            var headerContent = content.Substring (indexOfHeadStart, indexOfHeadEnd - indexOfHeadStart).Trim ().Replace ("\r\n", "");
            var indexEnd = 0;
            var indexStart = 0;
            while (true) {
                indexEnd = headerContent.IndexOf ("<", indexStart + 1);
                if (indexEnd == -1)
                    break; /// Done!
                if (headerContent[indexEnd + 1] == '/')
                    indexEnd = headerContent.IndexOf ("<", indexEnd + 1);
                if (indexEnd == -1) {
                    var tagCnt = headerContent.Substring (indexStart);
                    builder.Append ("\t\t" + tagCnt + "\r\n");
                    break;
                } else {
                    var tagCnt = headerContent.Substring (indexStart, indexEnd - indexStart);
                    if (tagCnt.StartsWith ("<title>")) {
                        tagCnt = "<title>" + tagCnt.Replace ("<title>", "").Replace ("</title>", "").Trim () + "</title>";
                    }
                    builder.Append ("\t\t" + tagCnt + "\r\n");
                    indexStart = indexEnd;
                }
            }
            builder.Append ("\t</head>\r\n\t<body>\r\n\t\t" + content.Substring (content.IndexOf ("<form", indexOfHeadEnd)));
            return builder.ToString ();
        }

        /*
         * Includes the CSS stylesheet files we should include for this response
         */
        private string IncludeStylesheetFiles (string content)
        {
            if ((Manager.Page as IAjaxPage).StylesheetFilesToPush.Count == 0)
                return content; // Nothing to do here

            // Stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" 
            // parts to concatenate into result after inserting all JavaScript files inbetween
            var builder = new StringBuilder ();
            var indexOfHeadEnd = content.IndexOf ("</head>");
            var cnt = content.Substring (0, indexOfHeadEnd).Trim ();
            builder.Append (cnt);

            // Including CSS files.
            foreach (var idxFile in (Manager.Page as IAjaxPage).StylesheetFilesToPush) {
                builder.Append (string.Format ("\r\n\t\t<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\"></link>", idxFile));
            }
            builder.Append ("\r\n\t" + content.Substring (indexOfHeadEnd));
            return builder.ToString ();
        }

        /*
         * Removes the ViewState wrapper div.
         */
        private string RemoveViewState (string content)
        {
            var startOffset = content.IndexOf (@"<div class=""aspNetHidden"">");
            var endOffset = content.IndexOf ("</div>", startOffset) + 6;
            StringBuilder buffer = new StringBuilder ();
            buffer.Append (content.Substring (0, startOffset).Trim());
            buffer.Append ("\r\n\t\t\t" + content.Substring (endOffset).TrimStart ());
            return buffer.ToString ();
        }

        /*
         * Includes the JavaScript files/content we should include for this response
         */
        private string IncludeJavaScript (string content)
        {
            if ((Manager.Page as IAjaxPage).JavaScriptToPush.Count == 0)
                return content; // nothing to do here

            // Stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" parts to concatenate into result after
            // inserting all JavaScript files inbetween
            var endBuffer = "";
            var idxPosition = content.Length - 1;
            for (; idxPosition >= 0; idxPosition --) {
                endBuffer = content [idxPosition] + endBuffer;
                if (endBuffer.StartsWith ("<") && endBuffer.StartsWith ("</body>", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
            var builder = new StringBuilder (content.Substring (0, idxPosition).TrimEnd(' '));

            // Including javascript files
            foreach (var idxFile in (Manager.Page as IAjaxPage).JavaScriptToPush) {
                if (idxFile.Item2) {

                    // This is a file
                    builder.Append (string.Format ("\t\t<script type=\"text/javascript\" src=\"{0}\"></script>\r\n", idxFile.Item1.Replace ("&", "&amp;")));
                }
            }

            // Adding back up again the "</html>" parts
            builder.Append ("\t" + endBuffer);
            return builder.ToString ();
        }

        /*
         * Includes the JavaScript content we should include for this response
         */
        private string SendJavaScriptContent (string content)
        {
            if (!Manager.Changes.Contains ("_p5_script"))
                return content; // nothing to do here

            // Stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" parts to concatenate into result after
            // inserting all JavaScript inbetween
            var endBuffer = "";
            var idxPosition = content.Length - 1;
            for (; idxPosition >= 0; idxPosition --) {
                endBuffer = content [idxPosition] + endBuffer;
                if (endBuffer.StartsWith ("<") && endBuffer.StartsWith ("</body>", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
            var builder = new StringBuilder (content.Substring (0, idxPosition).TrimEnd());

            // Including javascript
            builder.Append ("\r\n\t\t<script type=\"text/javascript\">\r\nwindow.onload = function() {\r\n");
            foreach (var idxScript in Manager.Changes ["_p5_script"] as List<string>) {
                builder.Append ("  " + idxScript.Trim().Replace ("\r\n", "\r\n  "));
                builder.Append ("\r\n");
            }

            // Including javascript files
            foreach (var idxFile in (Manager.Page as IAjaxPage).JavaScriptToPush) {
                if (!idxFile.Item2) {

                    // This is a simple inline inclusion
                    builder.Append ("  " + idxFile.Item1.Trim().Replace("\r\n", "\r\n  ") + "\r\n");
                }
            }
            builder.Append ("};\r\n\t\t</script>\r\n");

            // Adding back up again the "</html>" parts
            builder.Append ("\t" + endBuffer);
            return builder.ToString ();
        }
    }
}