/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Net;
using System.Web;
using phosphorus.core;
using MimeKit;

namespace phosphorus.net.response
{
    /// <summary>
    ///     Base class for all HTTP responses.
    /// 
    ///     Contains common methods for all HTTP response types, returned when creating HTTP requests.
    /// 
    ///     Class will take care of disposing the wrapped HttpWebResponse automatically.
    /// </summary>
    public abstract class HttpResponse : IResponse
    {
        private HttpWebResponse _response;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.response.HttpResponse"/> class.
        /// </summary>
        /// <param name="response">The wrapped response.</param>
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
        ///     Closes this instance, and disposes and releases all resources.
        /// </summary>
        public void Close ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        /// <summary>
        ///     Returns the wrapped HTTP response.
        /// </summary>
        /// <value>The HTTP response.</value>
        protected HttpWebResponse Response {
            get { return _response; }
        }

        /*
         * parses any HTTP headers returned from server
         */
        private void ParseHeaders (ApplicationContext context, Node node)
        {
            node.Add ("headers");
            foreach (var idxHeader in _response.Headers.AllKeys) {
                node.LastChild.Add (idxHeader, _response.Headers [idxHeader]);
            }
        }

        /*
         * parses any HTTP cookies returned from server
         */
        private void ParseCookies (ApplicationContext context, Node node)
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
        ///     Disposes the current instance.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> will dispose this instance.</param>
        protected virtual void Dispose (bool disposing)
        {
            if (disposing && _response != null) {
                _response.Dispose ();
                _response = null;
            }
        }

        /*
         * implementation of IDisposable interface, inherited from IResponse interface
         */
        void IDisposable.Dispose ()
        {
            Close ();
        }
    }
}
