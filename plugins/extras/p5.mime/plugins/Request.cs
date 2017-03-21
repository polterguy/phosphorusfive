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
 */

using System.Web;
using p5.core;
using MimeKit;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping MIME Active Events related to the HTTP request
    /// </summary>
    public static class Request
    {
        /// <summary>
        ///     Returns the current HTTP request's body as parsed MIME
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.request.parse-mime")]
        public static void p5_web_request_parse_mime (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new ArgsRemover (e.Args, true)) {

                // Loading MimeEntity from request stream
                var entity = MimeEntity.Load (
                    ContentType.Parse (HttpContext.Current.Request.ContentType), 
                    HttpContext.Current.Request.InputStream);
                e.Args.Value = entity;
                context.RaiseEvent (".p5.mime.parse-native", e.Args);
            }
        }
    }
}
