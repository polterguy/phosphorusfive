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
    public static class RequestFactory
    {
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
                contentType = ContentType.Parse (
                    XUtil.Single<string> (
                    node ["headers"] ["Content-Type"].Value, 
                    node ["headers"] ["Content-Type"], context));

            // checking what type of serializer we should use for our request
            if (contentType.MediaType == "multipart") {

                // using MIME serializer
                return new MIMESerializer (contentType);
            } else if (contentType.MediaType == "text") {

                // using text serializer
                return new TextSerializer ();
            } else if (contentType.MediaType == "application" && contentType.MediaSubtype == "x-www-form-urlencoded") {

                // using x-www-form-urlencoded serializer
                return new UrlEncodedSerializer ();
            }

            // using the default/fallback serializer
            return new BinarySerializer ();
        }
    }
}
