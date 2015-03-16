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
using MimeKit;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

/// <summary>
///     Namespace wrapping Active Events related to HTTP REST requests.
/// 
///     Contains useful helper Classes for creating HTTP REST requests.
/// </summary>
namespace phosphorus.web
{
    /// <summary>
    ///     Class wrapping [pf.web.request.create] Active Event.
    /// 
    ///     Contains all REST Active Events, and their associated helper methods.
    /// </summary>
    public static class Request
    {
        /// <summary>
        ///     Creates a new REST HTTP request.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.request.create")]
        private static void pf_web_rest_methods (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // iterating through every URL requested by caller
            foreach (var idxUrl in XUtil.Iterate<string> (e.Args, context)) {

                // creates, decorates and executes request
                HttpWebResponse response = ExecuteRequest (idxUrl, e.Args, context);

                // returning result of request, headers, cookies and content. First headers
                e.Args.Add (idxUrl);
                if (response.Headers.Count > 0) {
                    e.Args.LastChild.Add ("headers");
                    foreach (var idxHeader in response.Headers.AllKeys) {
                        e.Args.LastChild.LastChild.Add (idxHeader, response.Headers [idxHeader]);
                    }
                }
                
                // then cookies
                bool firstCookie = true;
                foreach (Cookie idxCookie in response.Cookies) {
                    if (!idxCookie.Expired) {
                        if (firstCookie) {
                            e.Args.LastChild.Add ("cookies");
                            firstCookie = false;
                        }
                        e.Args.LastChild.LastChild.Add (idxCookie.Name, idxCookie.Value);
                        e.Args.LastChild.LastChild.LastChild.Add ("expires", idxCookie.Expires);
                    }
                }

                // then content
                ParseContent (response, e.Args, context);
            }
        }

        /*
         * responsible for parsing content returned from HTTP request
         */
        private static void ParseContent (HttpWebResponse response, Node node, ApplicationContext context)
        {
            var contentType = ContentType.Parse (response.ContentType);
            switch (contentType.MimeType) {
            case "multipart/form-data":
                // using MimeKit to parse response
                var entity = MimeEntity.Load (contentType, response.GetResponseStream ());
                break;
            default:
                using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
                    node.LastChild.Add ("content", reader.ReadToEnd ());
                }
                break;
            }
        }

        /*
         * creates one request
         */
        private static HttpWebResponse ExecuteRequest (string url, Node node, ApplicationContext context)
        {
            // creating request, and adding headers and cookies
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create (CreateUrl (url, node, context));
            request.CookieContainer = new CookieContainer ();

            // adding cookies
            foreach (var idxCookie in XUtil.Iterate<Node> (node ["cookies"], context)) {
                request.CookieContainer.Add (
                    new Cookie (idxCookie.Name, HttpUtility.UrlEncode (XUtil.Single<string> (idxCookie, context))) { Domain = url });
            }

            // adding headers
            foreach (var idxHeader in XUtil.Iterate<Node> (node ["headers"], context)) {
                switch (idxHeader.Name) {
                case "Accept":
                    request.Accept = XUtil.Single<string> (idxHeader, context);
                    break;
                case "Connection":
                    request.Connection = XUtil.Single<string> (idxHeader, context);
                    break;
                case "Content-Length":
                    request.ContentLength = XUtil.Single<long> (idxHeader, context);
                    break;
                case "Content-Type":
                    request.ContentType = XUtil.Single<string> (idxHeader, context);
                    break;
                case "Date":
                    request.Date = XUtil.Single<DateTime> (idxHeader, context);
                    break;
                case "Expect":
                    request.Expect = XUtil.Single<string> (idxHeader, context);
                    break;
                case "Host":
                    request.Host = XUtil.Single<string> (idxHeader, context);
                    break;
                case "If-Modifies-Since":
                    request.IfModifiedSince = XUtil.Single<DateTime> (idxHeader, context);
                    break;
                case "Referer":
                    request.Referer = XUtil.Single<string> (idxHeader, context);
                    break;
                case "Transfer-Encoding":
                    request.TransferEncoding = XUtil.Single<string> (idxHeader, context);
                    break;
                case "User-Agent":
                    request.UserAgent = XUtil.Single<string> (idxHeader, context);
                    break;
                default:
                    request.Headers.Add (idxHeader.Name, XUtil.Single<string> (idxHeader, context));
                    break;
                }
            }

            request.Timeout = node.GetChildValue ("timeout", context, 100000);

            request.Method = node.GetChildValue ("method", context, "GET").ToUpper ();
            HttpWebResponse response;
            switch (request.Method) {
            case "GET":
            case "DELETE":
                response = (HttpWebResponse)request.GetResponse ();
                break;
            case "POST":
            case "PUT":
                response = CreateComplexResponse (request, node, context);
                break;
            default:
                throw new ArgumentException (string.Format ("Sorry, [pf.web.request.create] don't know how to create a '{0}' type of request", node ["method"].Value));
            }
            return response;
        }

        /*
         * creates the URL for the current request
         */
        private static string CreateUrl (string url, Node node, ApplicationContext context)
        {
            bool first = url.IndexOf ("?") == -1;
            string method = node.GetChildValue ("method", context, "GET").ToUpper ();
            if (method == "GET" || method == "DELETE") {
                foreach (var idxArg in XUtil.Iterate <Node> (node ["args"], context)) {
                    if (first) {
                        first = false;
                        url += "?" + idxArg.Name + "=" + HttpUtility.UrlEncode (idxArg.Get<string> (context));
                    } else {
                        url += "&" + idxArg.Name + "=" + HttpUtility.UrlEncode (idxArg.Get<string> (context));
                    }
                }
            }
            return url;
        }

        /*
         * creates a "complex" response, from either "PUT" or "POST" method type of request
         */
        private static HttpWebResponse CreateComplexResponse (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // retrieving Content-Type, defaulting to "application/x-www-form-urlencoded"
            string contentType = node ["headers"] != null ? 
                node ["headers"].GetChildValue ("Content-Type", context, "application/x-www-form-urlencoded") : 
                    "application/x-www-form-urlencoded";
            if (request.ContentType == null)
                request.ContentType = contentType;
            if (contentType == "application/x-www-form-urlencoded") {
                return CreateUrlEncodedResponse (request, node, context);
            } else {
                return CreateMultipartResponse (request, node, context);
            }
        }

        /*
         * creates a "application/x-www-form-urlencoded" response
         */
        private static HttpWebResponse CreateUrlEncodedResponse (HttpWebRequest request, Node node, ApplicationContext context)
        {
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                bool first = true;
                foreach (var idxArg in XUtil.Iterate<Node> (node ["args"], context)) {
                    if (first) {
                        first = false;
                    } else {
                        writer.Write ("&");
                    }
                    writer.Write (string.Format ("{0}={1}", idxArg.Name, HttpUtility.UrlEncode (idxArg.Get<string> (context))));
                }
            }
            return (HttpWebResponse)request.GetResponse ();
        }
        
        /*
         * creates a "multipart/form-data" response
         */
        private static HttpWebResponse CreateMultipartResponse (HttpWebRequest request, Node node, ApplicationContext context)
        {
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                foreach (var idxArg in XUtil.Iterate<Node> (node ["args"], context)) {
                    writer.Write (string.Format ("{0}={1}\n", idxArg.Name, HttpUtility.UrlEncode (idxArg.Get<string> (context))));
                }
            }
            return (HttpWebResponse)request.GetResponse ();
        }
    }
}
