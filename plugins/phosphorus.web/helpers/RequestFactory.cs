/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.web.helpers
{
    public static class RequestFactory
    {
        public static IRequest CreateRequest (ApplicationContext context, Node node)
        {
            switch (XUtil.Single<string> (node.GetChildValue ("method", context, "get"), node ["method"], context).ToLower ()) {
            case "http-get":
            case "https-get":
                return new HttpGetRequest ();
            case "http-post":
            case "https-post":
                return new HttpPostRequest ();
            case "http-put":
            case "https-put":
                return new HttpPutRequest ();
            case "http-delete":
            case "https-delete":
                return new HttpDeleteRequest ();
            default:
                return null;
            }
        }
    }
}
