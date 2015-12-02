/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Text;
using HtmlAgilityPack;
using p5.core;
using p5.exp;

namespace p5.html
{
    /// <summary>
    ///     Class to help encode and decode HTML
    /// </summary>
    public static class HtmlEncoder
    {
        /// <summary>
        ///     Encodes HTML
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.html.html-encode")]
        private static void p5_html_html_encode (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Used as return value
                StringBuilder builder = new StringBuilder();

                // Loops through all documents/fragments we're supposed to encode and adding them into value
                foreach (var idxHtmlFragment in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Changing to 'safe HTML'
                    builder.Append (idxHtmlFragment.Replace ("<", "&lt;").Replace (">", "&gt;"));
                }

                // Returning "safe HTML" back to caller
                e.Args.Value = builder.ToString ();
            }
        }

        /// <summary>
        ///     Decodes HTML
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.html.html-decode")]
        private static void p5_html_html_decode (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Used as return value
                StringBuilder builder = new StringBuilder();

                // Loops through all documents/fragments we're supposed to transform
                foreach (var idx in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Changing to 'safe HTML'
                    builder.Append (idx.Replace ("&lt;", "<").Replace ("&gt;", ">"));
                }

                // Returning decoded HTML to caller
                e.Args.Value = builder.ToString ();
            }
        }
    }
}
