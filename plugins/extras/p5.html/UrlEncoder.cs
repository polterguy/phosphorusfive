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
        [ActiveEvent (Name = "p5.html.url-encode")]
        public static void p5_html_url_encode (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args)) {

                // Used as return value
                var builder = new StringBuilder ();

                // Loops through all documents/fragments we're supposed to encode and eppending into StringBuilder
                foreach (var idxHtmlFragment in XUtil.Iterate<string> (context, e.Args)) {

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
        [ActiveEvent (Name = "p5.html.url-decode")]
        public static void p5_html_url_decode (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args)) {

                // Used as return value
                var builder = new StringBuilder ();

                // Loops through all documents/fragments we're supposed to transform
                foreach (var idx in XUtil.Iterate<string> (context, e.Args)) {

                    // Changing to 'safe HTML'
                    builder.Append (HttpUtility.UrlEncode (idx));
                }

                // Returning decoded HTML to caller
                e.Args.Value = builder.ToString ();
            }
        }
    }
}
