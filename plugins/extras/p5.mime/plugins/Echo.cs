/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.IO;
using System.Web;
using System.Collections.Generic;
using p5.core;
using MimeKit;

/// <summary>
///     Main namespace for MIME features pluging into other parts of the system
/// </summary>
namespace p5.mime.plugins
{
    /// <summary>
    ///     Class encapsulating the [echo-mime] Active Event for web apps to return MIME over HTTP response
    /// </summary>
    public static class Echo
    {
        /// <summary>
        ///     Echo MIME message back to client
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "echo-mime")]
        public static void echo_mime (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Discarding current response, and removing session cookie, unless caller explicitly said he wanted to keep it
                HttpContext.Current.Response.Filter = null;
                HttpContext.Current.Response.ClearContent ();

                // Used to keep track of all streams created during MIME creation process, such that we can dispose them afterwards
                var streams = new List<Stream> ();
                try {

                    // Creating MIME message
                    e.Args.Value = streams;
                    var entity = context.Raise (".p5.mime.create-native", e.Args).Get<MimeEntity> (context);

                    // Making sure we render the headers of root MimeEntity to response headers
                    RenderHeaders (entity);

                    // Serialising entity to Response Stream, making sure we render ONLY "content"
                    entity.WriteTo (HttpContext.Current.Response.OutputStream, true);

                    // Flushing response, and making sure default content is never rendered
                    HttpContext.Current.Response.OutputStream.Flush ();
                    HttpContext.Current.Response.Flush ();
                    HttpContext.Current.Response.SuppressContent = true;
                } finally {

                    // Closing and disposing streams created during creation of MIME message
                    foreach (var idxStream in streams) {
                        idxStream.Close ();
                        idxStream.Dispose ();
                    }
                }
            }
        }

        /*
         * Will render all headers from MimeEntity into HTTP response header collection
         */
        private static void RenderHeaders (MimeEntity entity)
        {
            foreach (var idxHeader in entity.Headers) {
                switch (idxHeader.Id) {
                case HeaderId.ContentType:
                    HttpContext.Current.Response.ContentType = idxHeader.Value;
                    break;
                default:
                    HttpContext.Current.Response.Headers.Add (idxHeader.Field, idxHeader.Value);
                    break;
                }
            }
        }
    }
}
