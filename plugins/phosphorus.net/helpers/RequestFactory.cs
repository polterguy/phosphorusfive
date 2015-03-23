/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Class responsible for creating new types of HTTP requests.
    /// </summary>
    public static class RequestFactory
    {
        /// <summary>
        ///     Creates the specified 'method' type of HTTP request.
        /// 
        ///     Supported methods are 'GET', 'POST', 'PUT' and 'DELETE'. If caller specifies a request that is not supported by this method,
        ///     then null will be returned.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="context">Application Context.</param>
        /// <param name="node">Node.</param>
        /// <param name="method">Method.</param>
        public static IRequest CreateRequest (ApplicationContext context, Node node, string method)
        {
            switch (method.ToLower ()) {
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
