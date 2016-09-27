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
    ///     Class to help URL-encode and URL-decode
    /// </summary>
    public static class UrlEncoder
    {
        /// <summary>
        ///     URL encodes a string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "url-encode")]
        public static void url_encode (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Used as return value
                StringBuilder builder = new StringBuilder();

                // Loops through all documents/fragments we're supposed to encode and eppending into StringBuilder
                foreach (var idxHtmlFragment in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Changing to 'safe HTML'
                    builder.Append (HttpUtility.UrlEncode (idxHtmlFragment));
                }

                // Returning "encoded HTML" back to caller
                e.Args.Value = builder.ToString ();
            }
        }

        /// <summary>
        ///     URL decodes a string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "url-decode")]
        public static void url_decode (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Used as return value
                StringBuilder builder = new StringBuilder();

                // Loops through all documents/fragments we're supposed to transform
                foreach (var idx in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Changing to 'safe HTML'
                    builder.Append (HttpUtility.UrlEncode (idx));
                }

                // Returning decoded HTML to caller
                e.Args.Value = builder.ToString ();
            }
        }
    }
}
