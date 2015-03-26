/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.web.ui.response.echo;
using MimeKit;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui.response
{
    /// <summary>
    ///     Class encapsulating the [pf.web.response.echo] Active Event
    /// 
    ///     Class wrapping the Active Events necessary to echo, or write, a specific piece of text, nodes and/or files,
    ///     back to client over the HTTP response.
    /// </summary>
    public static class Echo
    {
        /// <summary>
        ///     Echo content and/or files back to client.
        /// 
        ///     Writes the given content and/or files back to the client over the existing HTTP response. Works similar to 
        ///     [pf.net.create-request], only in the 'other direction'. When invoked, it will inspect the 'Content-Type' header for
        ///     the response, and depending upon the value of that header, will serialize the given files and/or parameters back to
        ///     the client accordingly.
        /// 
        ///     If the 'Content-Type' header for the current response is 'multipart/something', then it will use MimeKit to create
        ///     a multipart message, and return to the client, the same way [pf.web.create-request] will when serializing a MIME message,
        ///     it pushes to the server end-point.
        /// 
        ///     If the 'Content-Type' header is anything but 'multipart', then all parameters and/or files will be serialized back to
        ///     the client as a single value.
        /// 
        ///     For understanding how this Active Event works, please see the documentation for 
        ///     <see cref="phosphorus.net.CreateRequest.pf_net_create_request">[pf.net.create-request]</see>, and realize they're 
        ///     mostly the same, except that this Active Event has no [cookies], [headers] or [method] parameters. If you wish to change HTTP
        ///     headers and/or cookies for your echo response, you must invoke [pf.web.response.headers.set]/[pf.web.response.cookies.set]
        ///     before you invoke this Active Event. Besides from that, this Active Event and [pf.net.create-request] will build their
        ///     messages more or less identical, except of course that this Active Event will return the message created, while [pf.net.create-request]
        ///     will serialize it in the other direction.
        /// 
        ///     Below is an example of how you could return one parameter and one file as a 'multipart/mixed' type of response.
        /// 
        ///     <pre>pf.web.response.headers.set:Content-Type
        ///   source:multipart/mixed
        /// pf.web.response.echo
        ///   arg1:foo bar
        ///     Content-Type:text/plain
        ///   arg2
        ///     Content-Type:text/Hyperlisp
        ///     Content-Disposition:attachment; filename=application-startup.hl; name=file
        ///     Content-Transfer-Encoding;base64</pre>
        /// 
        ///     This Active Event, effectively allows you to create Web Services in your web apps, which you again can invoke from other
        ///     clients by using [pf.net.create-request].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.response.echo")]
        private static void pf_web_response_echo (ApplicationContext context, ActiveEventArgs e)
        {
            // checking to see if we should keep session around, defaulting to false
            bool keepSession = e.Args.GetExChildValue ("keep-session", context, false);

            // discarding current response, and removing session cookie, unless caller explicitly said he wanted to keep it
            HttpContext.Current.Response.Filter = null;
            HttpContext.Current.Response.ClearContent ();
            if (!keepSession) {

                // abandoning session, and removing session cookie from response
                HttpContext.Current.Response.Cookies.Remove ("ASP.NET_SessionId");
                HttpContext.Current.Session.Abandon ();
            }

            // actual rendering of content caller wants to echo. first we retrieve the 'Content-Type' header from the response headers, 
            // then we figure out what type of rendering caller wants, and renders accordingly
            ContentType cntType = ContentType.Parse (HttpContext.Current.Response.ContentType);
            CreateEchoResponse (cntType).Echo (context, e.Args, HttpContext.Current.Response);

            // flushing response, and making sure default content is never rendered
            HttpContext.Current.Response.OutputStream.Flush ();
            HttpContext.Current.Response.Flush ();
            HttpContext.Current.Response.SuppressContent = true;
        }

        /*
         * creates our echo response according to the given 'Content-Type' header, and returns IEchoResponse to caller
         */
        private static IEchoResponse CreateEchoResponse (ContentType contentType)
        {
            if (contentType.MediaType == "multipart")
                return new EchoResponseMultipart (contentType);
            else
                return new EchoResponseContent ();
        }
    }
}
