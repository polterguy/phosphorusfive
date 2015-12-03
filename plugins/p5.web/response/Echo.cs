/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Web;
using p5.core;
using p5.exp;

namespace p5.web.ui.response
{
    /// <summary>
    ///     Class encapsulating the [p5.web.response.echo] Active Event
    /// 
    ///     Class wrapping the Active Events necessary to echo, or write, a specific piece of text, nodes and/or files,
    ///     back to client over the HTTP response.
    /// </summary>
    public static class Echo
    {
        /// <summary>
        ///     Echo content back to client.
        /// 
        ///     Writes the given content back to the client over the existing HTTP response. Content can be either binary type, 
        ///     or any other type you wish. There is no session associated with the response, so using the session object 
        ///     across requests while intending to execute this Active Event to echo back specific content is futile.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "echo", Protection = EventProtection.LambdaClosed)]
        private static void echo (ApplicationContext context, ActiveEventArgs e)
        {
            // discarding current response, and removing session cookie, unless caller explicitly said he wanted to keep it
            HttpContext.Current.Response.Filter = null;
            HttpContext.Current.Response.ClearContent ();

            // abandoning session, and removing session cookie from response
            HttpContext.Current.Response.Cookies.Remove ("ASP.NET_SessionId");
            HttpContext.Current.Session.Abandon ();

            // rendering content back on wire
            byte[] val = e.Args.Value as byte[];
            if (val != null) {

                // Content is binary type of content
                HttpContext.Current.Response.BinaryWrite (val);
            } else {

                // Content is string, integer, etc type of content
                HttpContext.Current.Response.Write (XUtil.Single<string> (context, e.Args));
            }

            // flushing response, and making sure default content is never rendered
            HttpContext.Current.Response.OutputStream.Flush ();
            HttpContext.Current.Response.Flush ();
            HttpContext.Current.Response.SuppressContent = true;
        }

        /// <summary>
        ///     Echo file back to client.
        /// 
        ///     Writes the given file back to the client over the existing HTTP response without loading it into memory first. 
        ///     File can be either binary, textually based, or any other type you wish.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "echo-file", Protection = EventProtection.LambdaClosed)]
        private static void echo_file (ApplicationContext context, ActiveEventArgs e)
        {
            // discarding current response, and removing session cookie, unless caller explicitly said he wanted to keep it
            HttpContext.Current.Response.Filter = null;
            HttpContext.Current.Response.ClearContent ();

            // retrieving root node of web application
            var rootNode = new Node ();
            context.RaiseNative ("p5.core.application-folder", rootNode);
            var rootFolder = rootNode.Get<string> (context);

            // making sure we normalize folder separators, to have uniform folder structure
            // for both Linux and Windows
            rootFolder = rootFolder.Replace ("\\", "/");

            // rendering file back to client over response
            var fullPath = rootFolder + XUtil.Single<string> (context, e.Args);
            using (Stream fileStream = File.OpenRead (fullPath)) {
                fileStream.CopyTo (HttpContext.Current.Response.OutputStream);
            }

            // flushing response, and making sure default content is never rendered
            HttpContext.Current.Response.OutputStream.Flush ();
            HttpContext.Current.Response.Flush ();
            HttpContext.Current.Response.SuppressContent = true;
        }
    }
}
