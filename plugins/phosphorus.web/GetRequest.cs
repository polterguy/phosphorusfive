/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web
{
    /// <summary>
    ///     class to help load files from web using GET
    /// </summary>
    public static class GetRequest
    {
        /*
         * static ctor, to make sure we allow any SSL certificates when downloading files from web
         */

        static GetRequest ()
        {
            // setting up some global settings
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        /// <summary>
        ///     loads zero or more files over http
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.get")]
        private static void pf_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                // load file as HttpWebRequest
                LoadFileFromUrl (e.Args, idx);
            }
        }

        /*
         * loads a file from a URL
         */

        private static void LoadFileFromUrl (Node node, string url)
        {
            // setting up HttpWebRequest
            var request = (HttpWebRequest) WebRequest.Create (url);
            request.AllowAutoRedirect = true;

            // retrieving response and its encoding, defaulting encoding to UTF8
            var response = (HttpWebResponse) request.GetResponse ();
            var encoding = response.CharacterSet == null ?
                Encoding.UTF8 :
                Encoding.GetEncoding (response.CharacterSet);

            // retrieving files from response stream, and appending into node
            using (var stream = response.GetResponseStream ()) {
                if (stream == null)
                    throw new Exception ("no response from url");
                using (TextReader reader = new StreamReader (stream, encoding)) {
                    node.Add (new Node (response.ResponseUri.ToString (), reader.ReadToEnd ()));
                }
            }
        }
    }
}