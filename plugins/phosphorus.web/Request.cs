/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
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
    ///     Contains the [pf.web.request.create] Active Event, and its associated helper methods.
    /// </summary>
    public static class Request
    {
        /// <summary>
        ///     Creates a new HTTP REST request.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.request.create")]
        private static void pf_web_request_create (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // iterating through every URL requested by caller
            foreach (var idxUrl in XUtil.Iterate<string> (e.Args, context)) {

                // creates, decorates, and executes request
                HttpWebResponse response = ExecuteRequest (idxUrl, e.Args, context);

                // adding "result node" for current request
                e.Args.Add ("result", idxUrl);

                // returning result of request, headers, cookies and content. First headers
                ParseHeaders (response, e.Args.LastChild);

                // then cookies
                ParseCookies (response, e.Args.LastChild);

                // then content
                ParseContent (response, e.Args.LastChild, context);
            }
        }

        /*
         * creates one request
         */
        private static HttpWebResponse ExecuteRequest (string url, Node node, ApplicationContext context)
        {
            // creating request, and adding headers and cookies, setting some properties, and returning response to caller
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create (CreateUrl (url, node, context));

            // adding headers
            AddHeadersToRequest (request, node, context);

            // adding cookies
            AddCookiesToRequest (request, node, context);

            // setting other properties
            request.Timeout = node.GetChildValue ("timeout", context, 100000);
            request.Method = node.GetChildValue ("method", context, "GET").ToUpper ();

            // retrieving response, and returning to caller
            return ExecuteRequest (request, node, context);
        }
        
        /*
         * creates the URL for the current request
         * if this is a "GET" method request, then all [args] will be a part of the URL
         */
        private static string CreateUrl (string url, Node node, ApplicationContext context)
        {
            // figuring out what type of request this is, defaulting to GET unless [method] is explicitly given
            string method = node.GetChildValue ("method", context, "GET").ToUpper ();
            if (method == "GET" || method == "DELETE") {

                // this is either a "GET" or a "DELETE" method type of request, hence passing in the [args] as part of URL
                bool first = url.IndexOf ("?") == -1;
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
         * adding headers to request
         */
        private static void AddHeadersToRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
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
        }

        /*
         * adding cookies to request
         */
        private static void AddCookiesToRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            request.CookieContainer = new CookieContainer ();
            foreach (var idxCookie in XUtil.Iterate<Node> (node ["cookies"], context)) {
                request.CookieContainer.Add (
                    new Cookie (idxCookie.Name, HttpUtility.UrlEncode (XUtil.Single<string> (idxCookie, context))) { 
                    Domain = request.RequestUri.Host
                });
            }
        }

        /*
         * executes response, and returns to caller
         */
        private static HttpWebResponse ExecuteRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            switch (request.Method) {
            case "GET":
            case "DELETE":
                return (HttpWebResponse)request.GetResponse ();
            case "POST":
            case "PUT":
                return CreateComplexResponse (request, node, context);
            default:
                throw new ArgumentException (string.Format ("Sorry, [pf.web.request.create] don't know how to create a '{0}' type of request", node ["method"].Value));
            }
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

                // creating a simple URL encoded request
                return CreateUrlEncodedResponse (request, node, context);
            } else if (contentType == "multipart/form-data") {

                // using MimeKit to create a complex "multipart" request
                return CreateMultipartResponse (request, node, context);
            } else {
                throw new ArgumentException ("Sorry, I don't know how to create such a request!");
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
            // TODO: implement using MimeKit!!
            return (HttpWebResponse)request.GetResponse ();
        }
        
        /*
         * parses headers of response
         */
        private static void ParseHeaders (HttpWebResponse response, Node node)
        {
            if (response.Headers.Count > 0) {
                node.Add ("headers");
                foreach (var idxHeader in response.Headers.AllKeys) {
                    node.LastChild.Add (idxHeader, response.Headers [idxHeader]);
                }
            }
        }

        /*
         * parses cookies from response
         */
        private static void ParseCookies (HttpWebResponse response, Node node)
        {
            if (response.Cookies.Count > 0) {
                node.Add ("cookies");
                foreach (Cookie idxCookie in response.Cookies) {
                    if (!idxCookie.Expired) {
                        node.LastChild.Add (idxCookie.Name);
                        node.LastChild.LastChild.Add ("expires", idxCookie.Expires);
                        node.LastChild.LastChild.Add ("value", HttpUtility.UrlDecode (idxCookie.Value));
                    }
                }
            }
        }

        /*
         * responsible for parsing content returned from HTTP request
         */
        private static void ParseContent (HttpWebResponse response, Node node, ApplicationContext context)
        {
            var contentType = ContentType.Parse (response.ContentType);
            switch (contentType.MediaType) {
            case "multipart":

                // using MimeKit to parse response
                ParseMultiPartContent (response, node, context);
                break;
            default:

                // server returned a "single object"
                ParseSinglePartContent (response, node, context);
                break;
            }
        }

        /*
         * parses "multipart" content using MimeKit
         */
        private static void ParseMultiPartContent (HttpWebResponse response, Node node, ApplicationContext context)
        {
            var rootMultiPart = Multipart.Load (response.GetResponseStream ()) as Multipart;
            foreach (MimePart idxPart in rootMultiPart) {
                node.Add ("content");
                node.LastChild.Add ("content-type", idxPart.ContentType.MimeType);
                if (idxPart.ContentType.MediaType == "text") {

                    // text Content-Type
                    using (StreamReader reader = new StreamReader (idxPart.ContentObject.Open ())) {
                        node.LastChild.Add ("value", reader.ReadToEnd ());
                    }
                } else {

                    // some sort of binary or "non-text" Content-Type
                    using (MemoryStream stream = new MemoryStream ()) {
                        idxPart.ContentObject.DecodeTo (stream);
                        stream.Position = 0;
                        byte [] buffer = new byte [stream.Length];
                        stream.Read (buffer, 0, buffer.Length);
                        node.LastChild.Add ("value", buffer);
                    }
                }
            }
        }

        /*
         * parsing a "single object" return value from HttpWebResponse
         */
        private static void ParseSinglePartContent (HttpWebResponse response, Node node, ApplicationContext context)
        {
            node.Add ("content");
            node.LastChild.Add ("content-type", ContentType.Parse (response.ContentType).MimeType);
            if (ContentType.Parse (response.ContentType).MediaType == "text") {

                // some sort of text Content-Type
                using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
                    node.LastChild.Add ("value", reader.ReadToEnd ());
                }
            } else {

                // some sort of binary or "non-text" Content-Type
                using (MemoryStream stream = new MemoryStream ()) {
                    response.GetResponseStream ().CopyTo (stream);
                    stream.Position = 0;
                    byte [] buffer = new byte [stream.Length];
                    stream.Read (buffer, 0, buffer.Length);
                    node.LastChild.Add ("value", buffer);
                }
            }
        }
    }
}
