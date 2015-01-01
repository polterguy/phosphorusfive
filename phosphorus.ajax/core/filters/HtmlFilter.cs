/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using phosphorus.ajax.core;

namespace phosphorus.ajax.core.filters
{
    /// <summary>
    /// the http response filter for rendering plain html back to client
    /// </summary>
    public class HtmlFilter : Filter
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.core.filters.HtmlFilter"/> class
        /// </summary>
        /// <param name="manager">the manager this instance is rendering for</param>
        public HtmlFilter (Manager manager)
            : base (manager)
        { }

        /// <summary>
        /// renders the response
        /// </summary>
        /// <returns>the response returned back to client</returns>
        protected override string RenderResponse ()
        {
            // TODO: pass in ContentEncoding directly in ctor, since Response is not necessarily available in this context (.net)
            TextReader reader = new StreamReader (this, Manager.Page.Response.ContentEncoding);
            string content = reader.ReadToEnd ();
            content = IncludeJavaScriptFiles (content);
            content = IncludeJavaScriptContent (content);
            return content;
        }

        private string IncludeJavaScriptFiles (string content)
        {
            if (Manager.JavaScriptFiles.Count == 0)
                return content; // nothing to do here

            // stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" parts to concatenate into result after
            // inserting all JavaScript files inbetween
            string endBuffer = "";
            int idxPosition = content.Length - 1;
            for (; idxPosition >= 0; idxPosition --) {
                endBuffer = content [idxPosition] + endBuffer;
                if (endBuffer.StartsWith ("<") && endBuffer.StartsWith ("</body>", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
            StringBuilder builder = new StringBuilder (content.Substring (0, idxPosition));

            // including javascript files
            foreach (string idxFile in Manager.JavaScriptFiles) {
                builder.Append (string.Format(@"    <script type=""text/javascript"" src=""{0}""></script>
    ", idxFile.Replace ("&", "&amp;")));
            }

            // adding back up again the "</html>" parts
            builder.Append (endBuffer);
            return builder.ToString ();
        }
        
        private string IncludeJavaScriptContent (string content)
        {
            if (!Manager.Changes.Contains ("__pf_script"))
                return content; // nothing to do here

            // stripping away "</body>...</html>" from the end, and keeping the "</body>...</html>" parts to concatenate into result after
            // inserting all JavaScript files inbetween
            string endBuffer = "";
            int idxPosition = content.Length - 1;
            for (; idxPosition >= 0; idxPosition --) {
                endBuffer = content [idxPosition] + endBuffer;
                if (endBuffer.StartsWith ("<") && endBuffer.StartsWith ("</body>", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
            StringBuilder builder = new StringBuilder (content.Substring (0, idxPosition));

            // including javascript files
            foreach (string idxFile in Manager.Changes ["__pf_script"] as List<string>) {
                builder.Append (string.Format(@"    <script type=""text/javascript"">{0}</script>
    ", idxFile));
            }

            // adding back up again the "</html>" parts
            builder.Append (endBuffer);
            return builder.ToString ();
        }
    }
}

