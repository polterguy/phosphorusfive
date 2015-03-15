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
using RestSharp;

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
    ///     Class wrapping [pf.web.rest.get/post/put/delete] Active Events.
    /// 
    ///     Contains all REST Active Events, and their associated helper methods.
    /// </summary>
    public static class PFRestRequest
    {
        /// <summary>
        ///     Creates a new REST HTTP request.
        /// 
        ///     Allows you to create an HTTP REST request. Pass in parameters as [args] as HTTTP parameters, and
        ///     any custom HTTP headers as [headers]. [timeout] sets how many milliseconds the method shall wait, 
        ///     before timing out the request. Cookies can be explicitly passed in through [cookies]. Optionally, pass
        ///     in [username] and [password] as children of [credentials] node, to authenticate your request. If you pass in [credentials],
        ///     you can also pass in [domain] as an optional child of [credentials].
        /// 
        ///     The latter parts of the name of the Active Event name you use to invoke this method, defines what type of
        ///     REST request is created.
        /// 
        ///     If you do not supply [args], then you can optionally choose to supply a [body] node, which will be written as the
        ///     content of your HTTP request, if your request type is compatible with having a body. If you supply [body], then you 
        ///     can set its type by adding a [type] as a child of your [body] node. The default [type] is "text/Hyperlisp", if not supplied.
        ///     The actual content written, is expected to be found inside of [source], beneath [body].
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.rest.get")]
        [ActiveEvent (Name = "pf.web.rest.post")]
        [ActiveEvent (Name = "pf.web.rest.put")]
        [ActiveEvent (Name = "pf.web.rest.delete")]
        private static void pf_web_rest_methods (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // iterating through every URL requested by caller
            foreach (var idxUrl in XUtil.Iterate<string> (e.Args, context)) {

                // getting client
                Uri uri = new Uri (idxUrl);
                var client = GetClient (uri);

                // figuring out HTTP method, and creates our request
                string method = e.Name.Substring (e.Name.LastIndexOf (".") + 1).ToUpper ();
                var request = new RestRequest (uri.LocalPath, (Method)Enum.Parse (typeof (Method), method));
                if (e.Args.Find ("timeout") != null)
                    request.Timeout = XUtil.Single<int> (e.Args.Find ("timeout"), context);

                // decorating request with [args]
                foreach (var idxArg in XUtil.Iterate<Node> (e.Args.Find ("args"), context)) {
                    request.AddParameter (idxArg.Name, XUtil.Single<string> (idxArg, context));
                }

                // decorating request with [headers]
                foreach (var idxArg in XUtil.Iterate<Node> (e.Args.Find ("headers"), context)) {
                    request.AddHeader (idxArg.Name, XUtil.Single<string> (idxArg, context));
                }

                // decorating request with [cookies]
                foreach (var idxArg in XUtil.Iterate<Node> (e.Args.Find ("cookies"), context)) {
                    request.AddCookie (idxArg.Name, XUtil.Single<string> (idxArg, context));
                }

                // decorating request with [files]
                foreach (var idxArg in XUtil.Iterate<Node> (e.Args.Find ("files"), context)) {
                    request.AddFile (idxArg.Name, XUtil.Single<string> (idxArg, context));
                }

                // decorating request with [credentials]
                if (e.Args.Find ("credentials") != null) {
                    var cred = new NetworkCredential (
                        XUtil.Single<string> (e.Args ["credentials"] ["username"], context), 
                        XUtil.Single<string> (e.Args ["credentials"] ["password"], context));
                    if (e.Args ["credentials"].Find ("domain") != null)
                        cred.Domain = XUtil.Single<string> (e.Args ["credentials"] ["domain"], context);
                    request.Credentials = cred;
                }

                if (e.Args.Find ("body") != null) {
                    string type = "text/Hyperlisp";
                    if (e.Args ["body"].Find ("type") != null)
                        type = XUtil.Single<string> (e.Args ["body"] ["type"], context);
                    request.AddParameter (type, Utilities.Convert<string> (XUtil.Source (e.Args ["body"], context), context), ParameterType.RequestBody);
                }

                // retrieving response, and returning result to caller
                IRestResponse response = client.Execute (request);
                e.Args.Add (idxUrl, response.Content);
            }
        }

        /// <summary>
        ///     Retrieves type of request.
        /// 
        ///     Will return the type of request, such as for instance "GET" or "POST" as [request-type] return value.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.request.get-type")]
        private static void pf_web_request_get_type (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = HttpContext.Current.Request.HttpMethod;
        }

        /// <summary>
        ///     Retrieves body of request.
        /// 
        ///     Will return the raw body of request.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.request.get-body")]
        private static void pf_web_request_get_body (ApplicationContext context, ActiveEventArgs e)
        {
            MemoryStream mem = new MemoryStream ();
            HttpContext.Current.Request.InputStream.CopyTo (mem);
            mem.Position = 0;
            using (StreamReader reader = new StreamReader (mem))
            {
                e.Args.Value = reader.ReadToEnd ();
            }
        }

        /*
         * returns the cached RestClient for given url
         */
        private static Dictionary<string, RestClient> _clients = new Dictionary<string, RestClient> ();
        public static RestClient GetClient (Uri uri)
        {
            string hostUrl = uri.Scheme + "://" + uri.Authority;
            if (!_clients.ContainsKey (hostUrl)) {
                _clients [hostUrl] = new RestClient (hostUrl);
                _clients [hostUrl].CookieContainer = new CookieContainer ();
            }
            return _clients [hostUrl];
        }
    }
}
