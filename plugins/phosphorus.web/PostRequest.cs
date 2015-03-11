/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace phosphorus.web
{
    /// <summary>
    ///     Class wrapping the [pf.web.post] Active Event.
    /// 
    ///     Contains the [pf.web.post] Active Event, and its associated helper methods.
    /// </summary>
    public static class PostRequest
    {
        /// <summary>
        ///     Creates an HTTP POST request.
        /// 
        ///     Allows you to create an HTTP POST request to a URL, to post values, and download documents over the web.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.post")]
        private static void pf_web_post (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through each destination
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                // creating our HTTP web request
                var request = (HttpWebRequest) WebRequest.Create (idx);
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                // writing parameters over request stream
                using (var writer = new StreamWriter (request.GetRequestStream ())) {
                    foreach (var idxPar in e.Args.FindAll (idxNode => idxNode.Name != string.Empty).Where (idxPar => idxPar.Value != null)) {
                        writer.Write (
                            HttpUtility.UrlEncode (idxPar.Name) +
                            "=" +
                            HttpUtility.UrlEncode (idxPar.Get<string> (context)));
                    }
                }

                // retrieving response and its encoding
                var response = (HttpWebResponse) request.GetResponse ();
                var encoding = response.CharacterSet == null ?
                    Encoding.Default :
                    Encoding.GetEncoding (response.CharacterSet);

                // retrieving files from response stream, and appending into node
                using (var stream = response.GetResponseStream ()) {
                    if (stream == null)
                        throw new Exception ("no response from url");
                    using (TextReader reader = new StreamReader (stream, encoding)) {
                        e.Args.Add (new Node (response.ResponseUri.ToString (), reader.ReadToEnd ()));
                    }
                }
            }
        }
    }
}
