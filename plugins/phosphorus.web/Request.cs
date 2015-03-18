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
            foreach (var idxUrl in XUtil.Iterate<string> (e.Args.Value, e.Args, context)) {

                // creates, decorates, and executes request
                HttpWebResponse response = ExecuteRequest (idxUrl, e.Args, context);

                // adding "result node" for current request
                e.Args.Add ("result", idxUrl);

                // returning result of request, headers, cookies and content. First headers
                ParseHeaders (response, e.Args.LastChild);

                // then cookies
                ParseCookies (response, e.Args.LastChild);

                // then content
                ParseContent (
                    response, 
                    e.Args.LastChild, 
                    context, 
                    XUtil.Single<bool> (e.Args.GetChildValue<object> ("parse-mime", context, null), e.Args ["parse-mime"], context, true));
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
            request.Timeout = XUtil.Single (node.GetChildValue<object> ("timeout", context, null), node ["timeout"], context, 100000);
            request.Method = XUtil.Single (node.GetChildValue<object> ("method", context, null), node ["method"], context, "GET").ToUpper ();

            // retrieving response, and returning to caller
            return ExecuteRequest (request, node, context);
        }
        
        /*
         * creates the URL for the current request
         * if this is a "GET" method request, then all [args] will be a part of the URL, in encoded format
         */
        private static string CreateUrl (string url, Node node, ApplicationContext context)
        {
            // figuring out what type of request this is, defaulting to GET unless [method] is explicitly given
            string method = XUtil.Single (node.GetChildValue<string> ("method", context, null), node, context, "GET").ToUpper ();
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

                // fucking MSFT HttpWebRequest needs special treatment of all sorts of weird headers ...!!
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

        /*
         * adding cookies to request
         */
        private static void AddCookiesToRequest (HttpWebRequest request, Node node, ApplicationContext context)
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

        /*
         * executes response, and returns to caller
         */
        private static HttpWebResponse ExecuteRequest (HttpWebRequest request, Node node, ApplicationContext context)
        {
            switch (request.Method) {
            case "GET":
            case "DELETE":

                // nothing to do here, [args] was passed in as a part of URL in encoded form
                return (HttpWebResponse)request.GetResponse ();
            case "POST":
            case "PUT":

                // need to handle [args] in some intelligent manner
                return CreateComplexResponse (request, node, context);
            default:

                // we only support the 4 basic methods; GET, DELETE, POST and PUT
                throw new ArgumentException (string.Format ("Sorry, [pf.web.request.create] don't know how to create a '{0}' type of request", node ["method"].Value));
            }
        }

        /*
         * creates a "complex" response, from either "PUT" or "POST" method type of request
         */
        private static HttpWebResponse CreateComplexResponse (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // retrieving Content-Type, defaulting to "application/x-www-form-urlencoded", unless explicitly overridden
            request.ContentType = node ["headers"] == null ? 
                "application/x-www-form-urlencoded" : 
                XUtil.Single<string> (
                        node ["headers"].GetChildValue<object> ("Content-Type", context, null), 
                        node ["headers"] ["Content-Type"], 
                        context, 
                        "application/x-www-form-urlencoded");

            // checking what type of request this is, and acting accordingly
            if (request.ContentType == "application/x-www-form-urlencoded") {

                // creating a simple URL encoded request
                return CreateUrlEncodedResponse (request, node, context);
            } else if (request.ContentType == "multipart/form-data") {

                // using MimeKit to create a complex "multipart/form-data" request
                return CreateMultipartResponse (request, node, context);
            } else {

                // only "application/x-www-form-urlencoded" and "multipart/form-data" are supported
                throw new ArgumentException ("Sorry, I don't know how to create such a request!");
            }
        }

        /*
         * creates a "application/x-www-form-urlencoded" response from a POST request
         */
        private static HttpWebResponse CreateUrlEncodedResponse (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // creating a stream writer wrapping the "request content stream"
            using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                bool first = true;

                // looping through each [args] children node given, and URL encoding it into request stream writer
                foreach (var idxArg in XUtil.Iterate<Node> (node ["args"], context)) {
                    if (first)
                        first = false; // first parameter
                    else
                        writer.Write ("&"); // second, third, or fourth, etc, parameter, making sure we separate our parameters correctly
                    writer.Write (string.Format ("{0}={1}", idxArg.Name, HttpUtility.UrlEncode (idxArg.Get<string> (context))));
                }
            }

            // returning response to caller
            return (HttpWebResponse)request.GetResponse ();
        }
        
        /*
         * creates a "multipart/form-data" response from a POST request
         */
        private static HttpWebResponse CreateMultipartResponse (HttpWebRequest request, Node node, ApplicationContext context)
        {
            // creating root Multipart to hold all [args] and [files]
            Multipart root = new Multipart (ContentType.Parse (request.ContentType).MediaSubtype);

            // looping through each [args], building the correct MimePart according to type
            foreach (var idxArg in XUtil.Iterate<Node> (node ["args"], context)) {

                // figuring out Content-Type of current argument, defaulting to "text/plain"
                string contentTypeStr = "text/plain";
                if (idxArg ["content-type"] != null) {
                    contentTypeStr = XUtil.Single<string> (idxArg ["content-type"].Value, idxArg ["content-type"], context, "text/plain");
                }
                ContentType contentType = ContentType.Parse (contentTypeStr);
                if (contentType.MediaType == "text") {

                    // normal "text" part argument
                    TextPart part = new TextPart (contentType.MediaSubtype);
                    part.Headers.Add ("Content-Disposition", string.Format ("form-data; name={0}", idxArg.Name));
                    part.SetText (Encoding.UTF8, XUtil.Single<string> (idxArg.Value, idxArg, context));
                    root.Add (part);
                } else {

                    // anything but "text" type of content
                    MimePart part = new MimePart (contentType);
                    part.Headers.Add ("Content-Disposition", string.Format ("form-data; name={0}", idxArg.Name));
                    byte [] content = XUtil.Single<byte []> (idxArg.Value, idxArg, context);
                    MemoryStream stream = new MemoryStream ();
                    stream.Write (content, 0, content.Length);
                    stream.Position = 0;
                    part.ContentObject = new ContentObject (stream);
                    part.ContentTransferEncoding = ContentEncoding.Base64;
                    root.Add (part);
                }
            }

            // making sure Content-Type header also have boundary
            request.ContentType = root.ContentType.MimeType + root.ContentType.Parameters;

            // writing multipart to request stream, for then to return response
            root.WriteTo (request.GetRequestStream ());
            return (HttpWebResponse)request.GetResponse ();
        }
        
        /*
         * parses headers of response
         */
        private static void ParseHeaders (HttpWebResponse response, Node node)
        {
            // checking to see if we've got any headers, and if so, adding a root [headers] node
            if (response.Headers.Count > 0) {
                node.Add ("headers");

                // looping through each header, and adding into [headers] node
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
            // looping through each cookie
            foreach (Cookie idxCookie in response.Cookies) {
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

        /*
         * responsible for parsing content returned from HTTP request
         */
        private static void ParseContent (
            HttpWebResponse response, 
            Node node, 
            ApplicationContext context, 
            bool parseMime)
        {
            // figuring out what type of response we were given, and acting accordingly
            var contentType = ContentType.Parse (response.ContentType);

            switch (contentType.MediaType) {
            case "multipart":

                // server returned "multipart", checking to see if we should use MimeKit to parse, or simply return MIME message "raw"
                if (parseMime) {

                    // using MimeKit to parse response
                    ParseMultiPartContent (response, node, context, parseMime);
                } else {

                    // no parsing of MIME should be done, even though return value is a "multipart"
                    using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
                        node.Add ("content", reader.ReadToEnd ());
                    }
                }
                break;
            default:

                // server returned a "single object", no need to parse this bugger
                ParseSinglePartContent (response, node, context);
                break;
            }
        }

        /*
         * parses "multipart" content using MimeKit
         */
        private static void ParseMultiPartContent (
            HttpWebResponse response, 
            Node node, 
            ApplicationContext context,
            bool parseMime)
        {
            // creating and looping through each MimePart in response
            var rootMultiPart = Multipart.Load (response.GetResponseStream ()) as Multipart;
            foreach (MimePart idxPart in rootMultiPart) {

                // handling one MimePart, setting value of [content] node to "name" from ContentDisposition, but only if Disposition equals "form-data"
                string name = null;
                if (idxPart.ContentDisposition != null && 
                    idxPart.ContentDisposition.Disposition == "form-data" && 
                    idxPart.ContentDisposition.Parameters.Contains ("name"))
                    name = idxPart.ContentDisposition.Parameters ["name"];
                node.Add ("content", name);
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
                        byte[] buffer = new byte [stream.Length];
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
            // adding root [content] node, and [content-type]
            node.Add ("content");
            node.LastChild.Add ("content-type", ContentType.Parse (response.ContentType).MimeType);

            // checking what type of response this is
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
