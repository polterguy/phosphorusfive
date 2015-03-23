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
        public static IRequest CreateRequest (ApplicationContext context, Node node, string method)
        {
            switch (method) {
            case "get":
                return new HttpGetRequest ();
            case "post":
                return new HttpPostRequest ();
            case "put":
                return new HttpPutRequest ();
            case "delete":
                return new HttpDeleteRequest ();
            default:
                return null;
            }
        }
    }
}
