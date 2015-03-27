/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.net.response;
using MimeKit;

namespace phosphorus.net.requests
{
    /// <summary>
    ///     Common base class for all HTTP requests supported by Phosphorus.Five.
    /// 
    ///     This is the common base class for all HTTP requests supported by Phosphorus.Five. Such as 'GET', 'POST', 'PUT' and 'DELETE'.
    /// </summary>
    public abstract class HttpRequest : IRequest
    {
        private static string _basePath;

        static HttpRequest ()
        {
            ServicePointManager.ServerCertificateValidationCallback += delegate {
                return true;
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.net.requests.HttpRequest"/> class.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node wrapping the request parameters.</param>
        /// <param name="url">URL for request.</param>
        /// <param name="method">HTTP method to use for request.</param>
        public HttpRequest (ApplicationContext context, Node node, string url, string method)
        {
            Request = (HttpWebRequest)WebRequest.Create (GetURL (context, node, url));
            Request.Method = method;
            AddHeaders (context, node);
            AddCookies (context, node);
        }

        /// <summary>
        ///     Executes the request, and returns the response.
        /// 
        ///     Will execute and transmit the request to the server end-point, and return the response.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node wrapping the request.</param>
        public virtual IResponse Execute (ApplicationContext context, Node node)
        {
            HttpWebResponse response = (HttpWebResponse)Request.GetResponse ();
            ContentType cntType = ContentType.Parse (response.ContentType);

            // checking what type of serializer we should use for our request
            if (cntType.MediaType == "multipart") {

                // using MIME de-serializer
                return new MultipartResponse (response);
            } else if (cntType.MediaType == "text") {

                // using text de-serializer
                return new TextResponse (response);
            }

            // using the default/fallback de-serializer
            return new BinaryResponse (response);
        }

        /// <summary>
        ///     Returns the HTTPWebRequest wrapped by this instance.
        /// </summary>
        /// <value>The request.</value>
        protected HttpWebRequest Request {
            get;
            private set;
        }

        /// <summary>
        ///     Returns the URL for your request.
        /// 
        ///     Override this method to modify or validate the request URL.
        /// </summary>
        /// <returns>The URL for the request.</returns>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node wrapping request.</param>
        /// <param name="url">Original URL, before any modifications occurs.</param>
        protected virtual string GetURL (ApplicationContext context, Node node, string url)
        {
            return url;
        }

        /*
         * adds up all HTTP headers for request
         */
        private void AddHeaders (ApplicationContext context, Node node)
        {
            // looping through each header in our [headers] collection, and adding to request
            foreach (var idxHeader in XUtil.Iterate<Node> (node ["headers"], context)) {

                // %&$@ing MSFT HttpWebRequest needs special treatment of all sorts of weird headers ...!!
                switch (idxHeader.Name) {
                case "Accept":
                    Request.Accept = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Connection":
                    Request.Connection = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Content-Length":
                    Request.ContentLength = XUtil.Single<long> (idxHeader.Value, idxHeader, context);
                    break;
                case "Content-Type":
                    Request.ContentType = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Date":
                    Request.Date = XUtil.Single<DateTime> (idxHeader.Value, idxHeader, context);
                    break;
                case "Expect":
                    Request.Expect = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Host":
                    Request.Host = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "If-Modifies-Since":
                    Request.IfModifiedSince = XUtil.Single<DateTime> (idxHeader.Value, idxHeader, context);
                    break;
                case "Referer":
                    Request.Referer = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Transfer-Encoding":
                    Request.TransferEncoding = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "User-Agent":
                    Request.UserAgent = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                default:
                    Request.Headers.Add (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                    break;
                }
            }

            // if no Content-Type header is supplied, we default it to "application/x-www-form-urlencoded"
            if (Request.ContentType == null)
                Request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";

            // if no AllowAutoRedirect is provided
            Request.AllowAutoRedirect = node.GetExChildValue ("allow-auto-redirect", context, true);
        }

        /*
         * adds up all HTTP cookies for request
         */
        private void AddCookies (ApplicationContext context, Node node)
        {
            // making sure we've got our Cookie Container
            Request.CookieContainer = new CookieContainer ();

            // looping through all request cookies passed in by caller
            foreach (var idxCookie in XUtil.Iterate<Node> (node ["cookies"], context)) {
                Request.CookieContainer.Add (
                    new Cookie (idxCookie.Name, HttpUtility.UrlEncode (idxCookie.GetExValue<string> (context))) { 
                    Domain = Request.RequestUri.Host
                });
            }
        }

        /// <summary>
        ///     Returns base filepath for your application pool.
        /// </summary>
        /// <returns>The base path for your app.</returns>
        /// <param name="context">Application context.</param>
        public static string GetBasePath (ApplicationContext context)
        {
            if (_basePath == null) {
                Node node = new Node ();
                context.Raise ("pf.core.application-folder", node);
                _basePath = node.Get<string> (context);
            }
            return _basePath;
        }

        /// <summary>
        ///     Returns all parameters for request.
        /// </summary>
        /// <returns>The parameters.</returns>
        /// <param name="node">Node to traverse for parameters.</param>
        public static IEnumerable<Node> GetParameters (Node node)
        {
            return node.FindAll (
                ix => ix.Name != "headers" && 
                ix.Name != "cookies" && 
                ix.Name != "method" && 
                ix.Name != "sign" && 
                ix.Name != "encrypt");
        }
    }
}
