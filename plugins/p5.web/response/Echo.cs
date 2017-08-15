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

using System.IO;
using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.ui.response {
    /// <summary>
    ///     Class encapsulating the [p5.web.response.p5.web.echo] Active Event
    /// </summary>
    public static class Echo
    {
        /// <summary>
        ///     Echo content back to client
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.echo")]
        public static void p5_web_echo (ApplicationContext context, ActiveEventArgs e)
        {
            // Discarding current response, and removing session cookie, unless caller explicitly said he wanted to keep it
            HttpContext.Current.Response.Filter = null;
            HttpContext.Current.Response.ClearContent ();

            // Rendering content back on wire
            var val = e.Args.Value as byte[];
            if (val != null) {

                // Content is binary type of content
                HttpContext.Current.Response.BinaryWrite (val);
            } else {

                // Content is string, integer, etc type of content
                HttpContext.Current.Response.Write (XUtil.Single<string> (context, e.Args));
            }

            // Flushing response, and making sure default content is never rendered
            HttpContext.Current.Response.OutputStream.Flush ();
            HttpContext.Current.Response.Flush ();
            HttpContext.Current.Response.SuppressContent = true;
        }

        /// <summary>
        ///     Echo file back to client
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.echo-file")]
        public static void p5_web_echo_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Discarding current response, and removing session cookie, unless caller explicitly said he wanted to keep it
            HttpContext.Current.Response.Filter = null;
            HttpContext.Current.Response.ClearContent ();

            // Retrieving root node of web application
            var rootFolder = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);

            // Finding filename
            var fileName = context.RaiseEvent (".p5.io.unroll-path", new Node ("", XUtil.Single<string> (context, e.Args))).Get<string> (context);

            // Verifying user is authorized to reading from currently iterated file
            context.RaiseEvent (".p5.io.authorize.read-file", new Node ("", fileName).Add ("args", e.Args));

            // Rendering file back to client
            using (Stream fileStream = File.OpenRead (rootFolder + fileName)) {
                fileStream.CopyTo (HttpContext.Current.Response.OutputStream);
            }

            // Flushing response, and making sure default content is never rendered
            HttpContext.Current.Response.OutputStream.Flush ();
            HttpContext.Current.Response.Flush ();
            HttpContext.Current.Response.SuppressContent = true;
        }
    }
}
