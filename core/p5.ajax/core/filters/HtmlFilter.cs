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
            content = IncludeStylesheetFiles (content);
            content = IncludeJavaScript (content);
            content = SendJavaScriptContent (content);
            return content;
        }

        /*
         * includes the CSS stylesheet files we should include for this response
         */
        private string IncludeStylesheetFiles (string content)
        {
            if ((Manager.Page as IAjaxPage).StylesheetFilesToPush.Count == 0)
                return content; // Nothing to do here

            // Stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" 
            // parts to concatenate into result after inserting all JavaScript files inbetween
            var endBuffer = "";
            var idxPosition = 0;
            for (; idxPosition < content.Length; idxPosition ++) {
                if (endBuffer.EndsWith ("<head>"))
                    break;
                endBuffer += content [idxPosition];
            }
            var builder = new StringBuilder (endBuffer + "\r\n");

            // Including javascript files
            foreach (var idxFile in (Manager.Page as IAjaxPage).StylesheetFilesToPush) {
                builder.Append (string.Format ("        <link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\"></link>\r\n", idxFile));
            }

            // Adding back up again the "</html>" parts
            builder.Append (content.Substring (idxPosition));
            return builder.ToString ();
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
            var builder = new StringBuilder (content.Substring (0, idxPosition));

            // Including javascript files
            foreach (var idxFile in (Manager.Page as IAjaxPage).JavaScriptToPush) {
                if (idxFile.Item2) {

                    // This is a file
                    builder.Append (string.Format (@"    <script type=""text/javascript"" src=""{0}""></script>
    ", idxFile.Item1.Replace ("&", "&amp;")));
                } else {

                    // This is a simple inline inclusion
                    builder.Append (string.Format (@"    <script type=""text/javascript"">{0}</script>
    ", idxFile.Item1));
                }
            }

            // Adding back up again the "</html>" parts
            builder.Append (endBuffer);
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
            var builder = new StringBuilder (content.Substring (0, idxPosition));

            // Including javascript
            builder.Append (@"    <script type=""text/javascript"">window.onload = function() {
");
            foreach (var idxScript in Manager.Changes ["_p5_script"] as List<string>) {
                builder.Append (idxScript);
                builder.Append ("\r\n");
            }
            builder.Append (@"}    </script>
");

            // Adding back up again the "</html>" parts
            builder.Append (endBuffer);
            return builder.ToString ();
        }
    }
}