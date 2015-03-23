/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Net;
using System.Web;
using phosphorus.core;
using MimeKit;

namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Base class for all HTTP types of responses.
    /// 
    ///     Abstract base class for all HTTP types of responses.
    /// </summary>
    public abstract class HttpResponse : IResponse
    {
        private HttpWebResponse _response;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.helpers.HttpResponse"/> class.
        /// </summary>
        /// <param name="response">Response.</param>
        public HttpResponse (HttpWebResponse response)
        {
            _response = response;
        }

        public virtual void Parse (ApplicationContext context, Node node)
        {
            Node current = node.Add ("result", Response.ResponseUri.ToString ()).LastChild;

            // HTTP headers and cookies
            ParseHeaders (context, current);
            ParseCookies (context, current);
        }

        /// <summary>
        ///     Closes the response, and frees up all resources.
        /// </summary>
        public void Close ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        /// <summary>
        ///     Returns the wrappedd HTTP web response.
        /// </summary>
        /// <value>The response.</value>
        protected HttpWebResponse Response {
            get { return _response; }
        }

        /// <summary>
        ///     Parses any HTTP headers, and puts them into the given node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node.</param>
        protected void ParseHeaders (ApplicationContext context, Node node)
        {
            node.Add ("headers");
            foreach (var idxHeader in _response.Headers.AllKeys) {
                node.LastChild.Add (idxHeader, _response.Headers [idxHeader]);
            }
        }
        
        /// <summary>
        ///     Parses any HTTP cookies, and puts them into the given node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node.</param>
        protected void ParseCookies (ApplicationContext context, Node node)
        {
            foreach (Cookie idxCookie in _response.Cookies) {
                if (!idxCookie.Expired) {

                    // cookie was not expired, making sure we've got our [cookies] wrapper node
                    if (node.LastChild.Name != "cookies")
                        node.Add ("cookies");

                    node.LastChild.Add (idxCookie.Name);
                    node.LastChild.LastChild.Add ("expires", idxCookie.Expires);
                    node.LastChild.LastChild.Add ("value", HttpUtility.UrlDecode (idxCookie.Value));
                }
            }
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing of resources will be done.</param>
        protected virtual void Dispose (bool disposing)
        {
            if (disposing && _response != null) {
                _response.Dispose ();
                _response = null;
            }
        }

        void IDisposable.Dispose ()
        {
            Close ();
        }
    }
}
