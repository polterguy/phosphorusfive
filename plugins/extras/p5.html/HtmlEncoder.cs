/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using System.Text;
using p5.exp;
using p5.core;

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
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "html-encode", Protection = EventProtection.LambdaClosed)]
        public static void html_encode (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Used as return value
                StringBuilder builder = new StringBuilder();

                // Loops through all documents/fragments we're supposed to encode and eppending into StringBuilder
                foreach (var idxHtmlFragment in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Changing to 'safe HTML'
                    builder.Append (HttpUtility.HtmlEncode (idxHtmlFragment));
                }

                // Returning "encoded HTML" back to caller
                e.Args.Value = builder.ToString ();
            }
        }

        /// <summary>
        ///     Decodes HTML
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "html-decode", Protection = EventProtection.LambdaClosed)]
        public static void html_decode (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Used as return value
                StringBuilder builder = new StringBuilder();

                // Loops through all documents/fragments we're supposed to transform
                foreach (var idx in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Changing to 'safe HTML'
                    builder.Append (HttpUtility.HtmlDecode (idx));
                }

                // Returning decoded HTML to caller
                e.Args.Value = builder.ToString ();
            }
        }
    }
}
