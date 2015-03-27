/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.net.requests;
using phosphorus.net.requests.serializers;
using MimeKit;

namespace phosphorus.net
{
    /// <summary>
    ///     Request factory class.
    /// 
    ///     Will create one HTTP request for you, according to the given parameters. Supports 'get', 'post', 'put' and 'delete'. 
    ///     Will also instantiate the correct serializer for your request, according to the type of request, and the 'Content-Type'
    ///     header of your request.
    /// </summary>
    public static class RequestFactory
    {
        /// <summary>
        ///     Creates an HTTP request and returns to caller.
        /// 
        ///     Will create the correct HTTP request, according to the parameters given, and return back to caller.
        /// </summary>
        /// <returns>The request created.</returns>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node where parameters can be found.</param>
        /// <param name="method">Method to use for HTTP operation, such as 'get', 'post', 'put' or 'delete'.</param>
        /// <param name="url">URL for request.</param>
        public static IRequest CreateRequest (ApplicationContext context, Node node, string method, string url)
        {
            switch (method) {
            case "get":
                return new HttpGetRequest (context, node, url);
            case "delete":
                return new HttpDeleteRequest (context, node, url);
            case "post":
                return new HttpPostRequest (context, node, url, GetSerializer (context, node));
            case "put":
                return new HttpPutRequest (context, node, url, GetSerializer (context, node));
            default:
                return null;
            }
        }

        /*
         * figures out what type of serializer to use, according to the 'Content-Type' given
         */
        private static ISerializer GetSerializer (ApplicationContext context, Node node)
        {
            // figuring out 'Content-Type'. defaulting to "application/x-www-form-urlencoded", unless caller specifies something else
            ContentType contentType = new ContentType ("application", "x-www-form-urlencoded");
            if (node ["headers"] != null && node ["headers"] ["Content-Type"] != null)
                contentType = ContentType.Parse (node ["headers"] ["Content-Type"].GetExValue<string> (context));

            // checking what type of serializer we should use for our request
            if (contentType.MediaType == "multipart") {

                // using MIME serializer
                return new MultipartSerializer (contentType);
            } else if (contentType.MediaType == "application" && contentType.MediaSubtype == "x-www-form-urlencoded") {

                // using x-www-form-urlencoded serializer
                return new UrlEncodedSerializer ();
            }

            // using the default/fallback serializer
            return new ContentSerializer ();
        }
    }
}
