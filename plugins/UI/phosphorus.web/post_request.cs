
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.web
{
    /// <summary>
    /// helper to create an HTTP POST request
    /// </summary>
    public static class post_request
    {
        /// <summary>
        /// creates an HTTP POST request
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.post")]
        private static void pf_web_post (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through each destination
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // creating our HTTP web request
                HttpWebRequest request = WebRequest.Create (idx) as HttpWebRequest;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                // writing parameters over request stream
                using (StreamWriter writer = new StreamWriter (request.GetRequestStream ())) {
                    foreach (var idxPar in e.Args.FindAll (delegate (Node idxNode) { return idxNode.Name != string.Empty; })) {
                        if (idxPar.Value != null) {
                            writer.Write (
                                HttpUtility.UrlEncode (idxPar.Name) + 
                                "=" + 
                                HttpUtility.UrlEncode (idxPar.Get<string> (context)));
                        }
                    }
                }

                // retrieving response and its encoding
                HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
                Encoding encoding = response.CharacterSet == null ? 
                    Encoding.Default : 
                    Encoding.GetEncoding (response.CharacterSet);

                // retrieving files from response stream, and appending into node
                using (Stream stream = response.GetResponseStream ()) {
                    using (TextReader reader = new StreamReader (stream, encoding)) {
                        e.Args.Add (new Node (response.ResponseUri.ToString (), reader.ReadToEnd ()));
                    }
                }
            }
        }
    }
}
