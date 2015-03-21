/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Net;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.web.helpers
{
    public abstract class HttpRequest : IRequest
    {
        private HttpWebRequest _request;

        public virtual IResponse Execute (ApplicationContext context, Node node, string url)
        {
            _request = (HttpWebRequest)WebRequest.Create (GetUri (context, node, url));
            AddHeaders (context, node, _request);
            if (_request.ContentType == null)
                _request.ContentType = "text/plain; charset=utf-8";
            AddCookies (context, node, _request);
            Decorate (context, node, _request, ContentType.Parse (_request.ContentType));
            return GetResponse (_request);
        }

        protected virtual string GetUri (ApplicationContext context, Node node, string url)
        {
            return url;
        }

        protected virtual void AddHeaders (ApplicationContext context, Node node, HttpWebRequest request)
        {
            // looping through each header in our [headers] collection, and adding to request
            foreach (var idxHeader in XUtil.Iterate<Node> (node ["headers"], context)) {

                // %&$@ing MSFT HttpWebRequest needs special treatment of all sorts of weird headers ...!!
                switch (idxHeader.Name) {
                case "Accept":
                    request.Accept = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Connection":
                    request.Connection = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Content-Length":
                    request.ContentLength = XUtil.Single<long> (idxHeader.Value, idxHeader, context);
                    break;
                case "Content-Type":
                    request.ContentType = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Date":
                    request.Date = XUtil.Single<DateTime> (idxHeader.Value, idxHeader, context);
                    break;
                case "Expect":
                    request.Expect = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Host":
                    request.Host = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "If-Modifies-Since":
                    request.IfModifiedSince = XUtil.Single<DateTime> (idxHeader.Value, idxHeader, context);
                    break;
                case "Referer":
                    request.Referer = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "Transfer-Encoding":
                    request.TransferEncoding = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                case "User-Agent":
                    request.UserAgent = XUtil.Single<string> (idxHeader.Value, idxHeader, context);
                    break;
                default:
                    request.Headers.Add (idxHeader.Name, XUtil.Single<string> (idxHeader.Value, idxHeader, context));
                    break;
                }
            }
        }

        protected virtual void AddCookies (ApplicationContext context, Node node, HttpWebRequest request)
        {
            // making sure we've got our Cookie Container
            request.CookieContainer = new CookieContainer ();

            // looping through all request cookies passed in by caller
            foreach (var idxCookie in XUtil.Iterate<Node> (node ["cookies"], context)) {
                request.CookieContainer.Add (
                    new Cookie (idxCookie.Name, HttpUtility.UrlEncode (XUtil.Single<string> (idxCookie, context))) { 
                    Domain = request.RequestUri.Host
                });
            }
        }

        protected abstract void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type);

        protected virtual IResponse GetResponse (HttpWebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            ContentType type = ContentType.Parse (response.ContentType);
            switch (type.MediaType) {
            case "multipart":
                return new MultipartResponse (response);
            case "text":
                return new TextResponse (response);
            default:
                return new BinaryResponse (response);
            }
        }
        
        /*
         * returns base path of application
         */
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
    }
}
