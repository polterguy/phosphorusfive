/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using System.Linq;
using System.Text;

namespace p5.ajax.core.filters
{
    /// <summary>
    ///     An HTTP response filter used when rendering plain HTML back to client.
    ///     Notice, this class will significantly "clean up" the HTML, taking great pride in nicely formatting the output HTML.
    /// </summary>
    public class HtmlFilter : Filter
    {
        /// <summary>
        ///     Initializes a new instance of the HtmlFilter class.
        /// </summary>
        /// <param name="page">The AjaxPage this instance is rendering for</param>
        public HtmlFilter (AjaxPage page)
            : base (page)
        { }

        /// <summary>
        ///     Renders the response.
        /// </summary>
        /// <returns>The HTML response returned back to client</returns>
        protected override string RenderResponse ()
        {
            // Retrieving entire HTML from stream.
            TextReader reader = new StreamReader (this, ContentEncoding);
            var content = reader.ReadToEnd ();

            // Cleaning up HTML, which means removing __VIEWSTATE input and nicely formatting <head> section
            content = RemoveViewState (content);
            content = CleanHead (content);

            // Including CSS files.
            content = IncludeCSSFiles (content);

            // Including JavaScript files and JavaScript inline content
            content = IncludeJavaScript (content);
            return content;
        }

        /*
         * Removes the __VIEWSTATE input element, in adddition to its "aspNetHidden" wrapper div, if it exists.
         */
        string RemoveViewState (string html)
        {
            // Setting up a stringbuilder to create our return value, being HTML without ViewState wrapper div.
            var buffer = new StringBuilder ();

            // First checking if we have a "wrapper div" around __VIEWSTATE input.
            int startOffset = html.IndexOf (@"<div class=""aspNetHidden"">", StringComparison.InvariantCulture);
            int endOffset = -1;
            if (startOffset != -1) {

                // Removing entire "__VIEWSTATE wrapper div".
                endOffset = html.IndexOf ("</div>", startOffset, StringComparison.InvariantCulture) + 6;
            } else {

                // Defaulting to only removing __VIEWSTATE input
                startOffset = html.IndexOf ("__VIEWSTATE", StringComparison.InvariantCulture);
                endOffset = html.IndexOf ('>', startOffset);
                while (html [startOffset] != '<')
                    startOffset -= 1;
            }

            // Appending HTML before ViewState into StringBuilder, making sure we trim it.
            buffer.Append (html.Substring (0, startOffset - 1).TrimEnd ());

            // Then adding some nice formatting, before we append the HTML after ViewState.
            buffer.Append ("\r\n\t\t\t");
            buffer.Append (html.Substring (endOffset).TrimStart ());
            return buffer.ToString ();
        }

        /*
         * Cleans up head section, which means nicely formatting <head>.
         */
        string CleanHead (string html)
        {
            // Creating buffer to hold return value, appending the top parts of HTML head into it.
            var builder = new StringBuilder ();
            builder.Append ("<!DOCTYPE html>\r\n<html>\r\n\t<head>" + "\r\n");

            // Figuring out where <head> starts and ends in given input.
            var indexOfHeadStart = html.IndexOf ("<head>", StringComparison.InvariantCulture) + 6;
            var indexOfHeadEnd = html.IndexOf ("</head>", StringComparison.InvariantCulture);

            // Looping through HTML between <head> and </head>, appending each element nicely formatted into result buffer.
            string element = "";
            while (indexOfHeadStart <= indexOfHeadEnd) {
                element += html [indexOfHeadStart++];

                // Checking if we're at end of element.
                if (element.EndsWith ("/>", StringComparison.InvariantCulture) || (element.EndsWith (">", StringComparison.InvariantCulture) && element.Contains ("</"))) {

                    // End of element, inserting into resulting StringBuilder, making sure we nicely format element, before we reset element buffer.
                    // Notice, we handle <title> differently, since it's all messed up by ASP.NET.
                    builder.Append ("\t\t");
                    if (element.StartsWith ("<title>", StringComparison.InvariantCulture)) {
                        builder.Append ("<title>");
                        builder.Append (element.Substring (7, element.Length - 15).Trim ());
                        builder.Append ("</title>");
                    } else {
                        builder.Append (element.TrimStart ());
                    }
                    element = "";
                    builder.Append ("\r\n");
                }
            }

            // Appending the rest of our HTML, making sure also our <form> element is nicely formatted.
            builder.Append ("\t</head>\r\n\t<body>\r\n\t\t");
            builder.Append (html.Substring (html.IndexOf ("<form", indexOfHeadEnd, StringComparison.InvariantCulture)));
            return builder.ToString ();
        }

        /*
         * Includes the CSS stylesheet files we should include for this response.
         */
        string IncludeCSSFiles (string html)
        {
            // If we don't have any CSS files to include, we return early.
            if (Page.PersistentCSSInclusions.Count == 0)
                return html;

            // Setting up a StringBuilder return buffer.
            var builder = new StringBuilder ();

            // Finding out where <head> ends, and appending all pre-existing content in <head> into StringBuilder.
            var indexOfHeadEnd = html.IndexOf ("</head>", StringComparison.InvariantCulture);
            builder.Append (html.Substring (0, indexOfHeadEnd).TrimEnd ());

            // Including CSS files, making sure we nicely format our inclusion HTML.
            foreach (var idxFile in Page.PersistentCSSInclusions) {
                builder.Append (string.Format ("\r\n\t\t<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />", idxFile));
            }

            // Appending everything from <head> end, and onwards, into StringBuilder return buffer, before we return the result.
            builder.Append ("\r\n\t");
            builder.Append (html.Substring (indexOfHeadEnd));
            return builder.ToString ();
        }

        /*
         * Includes the JavaScript files and inline JS inclusions we should include for this response.
         */
        string IncludeJavaScript (string html)
        {
            // Figuring out where <body> ends, by iterating backwards from end of HTML, until we've found </body>
            var endBuffer = "";
            var positionOfEndBody = html.Length - 1;
            for (; ; positionOfEndBody--) {
                endBuffer = html [positionOfEndBody] + endBuffer;
                if (endBuffer.StartsWith ("<", StringComparison.InvariantCulture) && endBuffer.StartsWith ("</body>", StringComparison.InvariantCulture))
                    break;
            }

            // Creating a StringBuilder holding everything up until </body> in its content.
            var builder = new StringBuilder (html.Substring (0, positionOfEndBody).TrimEnd ());

            // Then we include actual JavaScript into our HTML. Notice, order counts!
            // First we include files, then "inclusions" (which are persistent inline inclusions, supposed to be re-included in e.g. postbacks).
            // Then finally, we pass in [p5.web.send-javascript] content, supposed to only be transmitte to client once, as "bursts" of JS.
            // This is because send JS logic might depend upon inclusion JS, and inclusion inline JS might depend upon files.

            // Including javascript files, making sure we only retrieve files initially.
            foreach (var idxFile in Page.PersistensJSFileInclusions) {

                // Making sure we nicely format it, and URL encode its most usual variants.
                builder.Append (string.Format ("\r\n\t\t<script type=\"text/javascript\" src=\"{0}\"></script>", idxFile.Replace ("&", "&amp;")));
            }

            // Then including inline JavaScript inclusions, and [p5.web.send-javascript] bursts, if there are any.
            if (Page.SendScripts.Count () > 0 || Page.PersistensJSObjectInclusions.Count () > 0) {

                builder.Append ("\r\n\t\t<script type=\"text/javascript\">\r\nwindow.onload = function() {\r\n\r\n");

                // Inclusions have presedence, since they're logically almost like "JS files".
                foreach (var idxInclusion in Page.PersistensJSObjectInclusions) {

                    // Making sure we at least to some extent nicely format our JavaScript.
                    builder.Append (idxInclusion);
                    builder.Append ("\r\n\r\n");
                }

                // Then sending JavaScript content (last, after including files).
                foreach (var idxScript in Page.SendScripts) {

                    // Making sure we at least to some extent nicely format our JavaScript.
                    builder.Append (idxScript);
                    builder.Append ("\r\n\r\n");
                }
                builder.Append ("};\r\n\t\t</script>");
            }

            // Adding back up again the "</body></html>" parts as they originally appeared in HTML.
            builder.Append ("\r\n\t");
            builder.Append (endBuffer);
            return builder.ToString ();
        }
    }
}