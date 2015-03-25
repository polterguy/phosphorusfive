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
    public abstract class HttpResponse : IResponse
    {
        private HttpWebResponse _response;

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

        public void Close ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected HttpWebResponse Response {
            get { return _response; }
        }

        protected void ParseHeaders (ApplicationContext context, Node node)
        {
            node.Add ("headers");
            foreach (var idxHeader in _response.Headers.AllKeys) {
                node.LastChild.Add (idxHeader, _response.Headers [idxHeader]);
            }
        }
        
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
