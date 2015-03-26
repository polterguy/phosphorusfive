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
    public abstract class HttpRequest : IRequest
    {
        public HttpRequest (ApplicationContext context, Node node, string url, string method)
        {
            Request = (HttpWebRequest)WebRequest.Create (GetURL (context, node, url));
            Request.Method = method;
            AddHeaders (context, node);
            AddCookies (context, node);
        }

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

        protected HttpWebRequest Request {
            get;
            private set;
        }

        protected virtual string GetURL (ApplicationContext context, Node node, string url)
        {
            return url;
        }
        
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
            Request.AllowAutoRedirect = 
                XUtil.Single (
                    node.GetChildValue<object> ("allow-auto-redirect", context, null), node ["allow-auto-redirect"], context, true);
        }

        private void AddCookies (ApplicationContext context, Node node)
        {
            // making sure we've got our Cookie Container
            Request.CookieContainer = new CookieContainer ();

            // looping through all request cookies passed in by caller
            foreach (var idxCookie in XUtil.Iterate<Node> (node ["cookies"], context)) {
                Request.CookieContainer.Add (
                    new Cookie (idxCookie.Name, HttpUtility.UrlEncode (XUtil.Single<string> (idxCookie, context))) { 
                    Domain = Request.RequestUri.Host
                });
            }
        }

        private static string _basePath;
        protected static string GetBasePath (ApplicationContext context)
        {
            if (_basePath == null) {
                Node node = new Node ();
                context.Raise ("pf.core.application-folder", node);
                _basePath = node.Get<string> (context);
            }
            return _basePath;
        }
        
        public static IEnumerable<Node> GetArguments (Node node)
        {
            return node.FindAll (ix => ix.Name != "headers" && ix.Name != "cookies" && ix.Name != "method");
        }
    }
}
