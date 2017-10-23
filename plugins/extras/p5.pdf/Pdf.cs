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
 * 
 * Notice, this module consumes iTextSharp, which is licensed as Affero GPL.
 * Please notice, that this has some consequences for your own code, if you
 * choose to use it. Primarily that you'll need to link to its source code,
 * regardless of whether or not you've bought a commercial license of P5 or 
 * not.
 */

using System.IO;
using System.Linq;
using System.Text;
using p5.exp;
using p5.core;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace p5.markdown
{
    /// <summary>
    ///     Class to help transform HTML to PDF
    /// </summary>
    public static class Pdf
    {
        /// <summary>
        ///     Parses a Markdown snippet, and creates PDF document from it.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "html2pdf")]
        [ActiveEvent (Name = "p5.pdf.html2pdf")]
        public static void html2pdf (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args, true)) {

                // Assumes there's only one document, or creates one result of it.
                var filename = XUtil.Single<string> (context, e.Args);
                filename = context.RaiseEvent (".p5.io.unroll-path", new Node ("", filename)).Get<string> (context);

                // Figuring out source.
                var html = XUtil.Source (context, e.Args, "css-file") as string;

                // Retrieving root path of app.
                var rootPath = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);

                // Loading up all CSS, which we'll need to pass into ParseXhtml further down.
                var css = "";
                foreach (var idxCssFile in e.Args.Children.Where (ix => ix.Name == "css-file")) {
                    var cssFile = idxCssFile.GetExValue<string> (context);
                    cssFile = context.RaiseEvent (".p5.io.unroll-path", new Node ("", cssFile)).Get<string> (context);
                    using (TextReader reader = File.OpenText (rootPath + cssFile)) {
                        css += reader.ReadToEnd () + "\r\n";
                    }
                }

                // Creating our document.
                using (var stream = new FileStream (rootPath + filename, FileMode.Create)) {
                    using (var document = new Document (PageSize.A4, 55, 55, 70, 70)) {
                        using (var writer = PdfWriter.GetInstance (document, stream)) {
                            document.Open ();
                            using (var htmlStream = new MemoryStream (Encoding.UTF8.GetBytes (html))) {
                                using (var cssStream = new MemoryStream (Encoding.UTF8.GetBytes (css))) {
                                    XMLWorkerHelper.GetInstance ().ParseXHtml (writer, document, htmlStream, cssStream);
                                }
                            }
                            document.Close ();
                        }
                    }
                }
            }
        }
    }
}
