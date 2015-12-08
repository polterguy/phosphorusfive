/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.core;
using p5.exp;

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
        private static void get_http_param (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through each parameter requested by caller
                foreach (var idx in XUtil.Iterate<string> (context, e.Args)) {

                    // Adding parameter's name/value as Node return value
                    if (HttpContext.Current.Request.Params [idx] != null)
                        e.Args.Add (idx, HttpContext.Current.Request.Params [idx]);
                }
            }
        }

        /// <summary>
        ///     Lists all keys for our GET and POST parameters
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-http-params", Protection = EventProtection.LambdaClosed)]
        private static void list_http_params (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (context, e.Args, HttpContext.Current.Request.Params.AllKeys);
        }
    }
}
