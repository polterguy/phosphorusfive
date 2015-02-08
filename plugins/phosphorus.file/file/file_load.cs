
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.file
{
    /// <summary>
    /// class to help load files
    /// </summary>
    public static class file_load
    {
        /*
         * static ctor to make sure we allow any SSL certificates when downloading files from web
         */
        static file_load ()
        {
            // setting up some global settings
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        /// <summary>
        /// loads zero or more files from disc or over http. can be given either an expression or a constant.
        /// if file does not exist, false will be returned as value
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.load")]
        private static void pf_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                if (idx.StartsWith ("http://") || idx.StartsWith ("https://")) {

                    // load file as HttpWebRequest
                    LoadFileFromURL (e.Args, idx);
                } else {

                    // local file
                    LoadFileLocally (e.Args, idx, context);
                }
            }
        }

        /*
         * loads a file from the local file system
         */
        private static void LoadFileLocally (Node node, string filename, ApplicationContext context)
        {
            string rootFolder = common.GetRootFolder (context);
            if (File.Exists (rootFolder + filename)) {

                // file exists, loading it as text file and appending into node
                using (TextReader reader = File.OpenText (rootFolder + filename)) {
                    node.Add (new Node (filename, reader.ReadToEnd ()));
                }
            } else {

                // file didn't exist, making sure we signal caller by appending a "false" node
                node.Add (new Node (filename, false));
            }
        }

        /*
         * loads a file from a URL
         */
        private static void LoadFileFromURL (Node node, string url)
        {
            // setting up HttpWebRequest
            HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
            request.AllowAutoRedirect = true;

            // retrieving response and its encoding
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            Encoding encoding = response.CharacterSet == null ? 
                Encoding.Default : 
                Encoding.GetEncoding (response.CharacterSet);

            // retrieving files from response stream, and appending into node
            using (Stream stream = response.GetResponseStream ()) {
                using (TextReader reader = new StreamReader (stream, encoding)) {
                    node.Add (new Node (response.ResponseUri.ToString (), reader.ReadToEnd ()));
                }
            }
        }
    }
}
