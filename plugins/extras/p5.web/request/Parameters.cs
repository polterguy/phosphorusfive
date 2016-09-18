/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.ui.request
{
    /// <summary>
    ///     Helper to retrieve POST and GET HTTP request parameters
    /// </summary>
    public static class Parameters
    {
        /// <summary>
        ///     Returns one or more HTTP GET or POST request parameter(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-http-param", Protection = EventProtection.LambdaClosed)]
        public static void get_http_param (ApplicationContext context, ActiveEventArgs e)
        {
            Collection.Get (context, e.Args, key => HttpContext.Current.Request.Params [key], e.NativeSource);
        }

        /// <summary>
        ///     Lists all keys for our GET and POST parameters
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-http-params", Protection = EventProtection.LambdaClosed)]
        public static void list_http_params (ApplicationContext context, ActiveEventArgs e)
        {
            Collection.List (context, e.Args, HttpContext.Current.Request.Params.AllKeys, e.NativeSource);
        }
    }
}
