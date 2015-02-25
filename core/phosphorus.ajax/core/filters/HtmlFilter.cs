/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// ReSharper disable PossibleNullReferenceException

namespace phosphorus.ajax.core.filters
{
    /// <summary>
    ///     the http response filter for rendering plain html back to client
    /// </summary>
    public class HtmlFilter : Filter
    {
        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.ajax.core.filters.HtmlFilter" /> class
        /// </summary>
        /// <param name="manager">the manager this instance is rendering for</param>
        public HtmlFilter (Manager manager)
            : base (manager) { }

        /// <summary>
        ///     renders the response
        /// </summary>
        /// <returns>the response returned back to client</returns>
        protected override string RenderResponse ()
        {
            TextReader reader = new StreamReader (this, Encoding);
            var content = reader.ReadToEnd ();
            content = IncludeStylesheetFiles (content);
            content = IncludeJavaScriptFiles (content);
            content = IncludeJavaScriptContent (content);
            return content;
        }

        /*
         * includes the CSS stylesheet files we should include for this response
         */

        private string IncludeStylesheetFiles (string content)
        {
            if ((Manager.Page as IAjaxPage).StylesheetFilesToPush.Count == 0)
                return content; // nothing to do here

            // stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" parts to concatenate into result after
            // inserting all JavaScript files inbetween
            var endBuffer = "";
            var idxPosition = 0;
            for (; idxPosition < content.Length; idxPosition ++) {
                if (endBuffer.EndsWith (">") && endBuffer.EndsWith ("<head>", StringComparison.InvariantCultureIgnoreCase))
                    break;
                endBuffer += content [idxPosition];
            }
            var builder = new StringBuilder (endBuffer + "\r\n");

            // including javascript files
            foreach (var idxFile in (Manager.Page as IAjaxPage).StylesheetFilesToPush) {
                builder.Append (string.Format ("        <link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\"></link>\r\n", idxFile));
            }

            // adding back up again the "</html>" parts
            builder.Append (content.Substring (idxPosition));
            return builder.ToString ();
        }

        /*
         * includes the JavaScript files we should include for this response
         */

        private string IncludeJavaScriptFiles (string content)
        {
            if ((Manager.Page as IAjaxPage).JavaScriptFilesToPush.Count == 0)
                return content; // nothing to do here

            // stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" parts to concatenate into result after
            // inserting all JavaScript files inbetween
            var endBuffer = "";
            var idxPosition = content.Length - 1;
            for (; idxPosition >= 0; idxPosition --) {
                endBuffer = content [idxPosition] + endBuffer;
                if (endBuffer.StartsWith ("<") && endBuffer.StartsWith ("</body>", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
            var builder = new StringBuilder (content.Substring (0, idxPosition));

            // including javascript files
            foreach (var idxFile in (Manager.Page as IAjaxPage).JavaScriptFilesToPush) {
                builder.Append (string.Format (@"    <script type=""text/javascript"" src=""{0}""></script>
    ", idxFile.Replace ("&", "&amp;")));
            }

            // adding back up again the "</html>" parts
            builder.Append (endBuffer);
            return builder.ToString ();
        }

        /*
         * includes the JavaScript content we should include for this response
         */

        private string IncludeJavaScriptContent (string content)
        {
            if (!Manager.Changes.Contains ("__pf_script"))
                return content; // nothing to do here

            // stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" parts to concatenate into result after
            // inserting all JavaScript files inbetween
            var endBuffer = "";
            var idxPosition = content.Length - 1;
            for (; idxPosition >= 0; idxPosition --) {
                endBuffer = content [idxPosition] + endBuffer;
                if (endBuffer.StartsWith ("<") && endBuffer.StartsWith ("</body>", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
            var builder = new StringBuilder (content.Substring (0, idxPosition));

            // including javascript files
            builder.Append (@"    <script type=""text/javascript"">window.onload = function() {
");
            foreach (var idxScript in Manager.Changes ["__pf_script"] as List<string>) {
                builder.Append (idxScript);
                builder.Append ("\r\n");
            }
            builder.Append (@"}    </script>
");

            // adding back up again the "</html>" parts
            builder.Append (endBuffer);
            return builder.ToString ();
        }
    }
}