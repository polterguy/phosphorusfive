
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.file
{
    /// <summary>
    /// class to help load and save files
    /// </summary>
    public static class file
    {
        /// <summary>
        /// loads zero or more files from disc or over http. can be given either an expression or a constant.
        /// if file does not exist, false will be returned as value
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.load")]
        private static void pf_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            Expression.Iterate<string> (e.Args, true, 
            delegate (string idx) {
                if (idx.StartsWith ("http://") || idx.StartsWith ("https://")) {
                    HttpWebRequest request = WebRequest.Create (idx) as HttpWebRequest;
                    request.AllowAutoRedirect = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
                    Encoding encoding = Encoding.GetEncoding (response.CharacterSet);
                    using (Stream stream = response.GetResponseStream ()) {
                        using (TextReader reader = new StreamReader (stream, encoding)) {
                            e.Args.Add (new Node (response.ResponseUri.ToString (), reader.ReadToEnd ()));
                        }
                    }
                } else {
                    if (!File.Exists (rootFolder + idx)) {
                        e.Args.Add (new Node (idx, false));
                    } else {
                        using (TextReader reader = File.OpenText (rootFolder + idx)) {
                            e.Args.Add (new Node (idx, reader.ReadToEnd ()));
                        }
                    }
                }
            });
        }

        /// <summary>
        /// saves the last child of node, as one or more text files from the path given as value of args, 
        /// which might be a constant, or an expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.save")]
        private static void pf_file_save (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            Expression.Iterate<string> (e.Args, true, 
            delegate (string idx) {
                using (TextWriter writer = File.CreateText (rootFolder + idx)) {
                    writer.Write (e.Args.LastChild.Get<string> ());
                }
            });
        }

        /// <summary>
        /// removes one or more files from the path given as value of args, which might be a constant, or
        /// an expression. all files that are successfully removed, will be returned as children nodes, 
        /// with path of file being name, and value being true. if file is not successfully removed, return
        /// value for that file will be false
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.remove")]
        private static void pf_file_remove (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            Expression.Iterate<string> (e.Args, true, 
            delegate (string idx) {
                if (File.Exists (rootFolder + idx)) {
                    File.Delete (rootFolder + idx);
                    e.Args.Add (new Node (idx, true));
                } else {
                    e.Args.Add (new Node (idx, false));
                }
            });
        }

        /// <summary>
        /// returns true for each file in constant or expression of path given as args that exists
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.exists")]
        private static void pf_file_exists (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            Expression.Iterate<string> (e.Args, true, 
            delegate (string idx) {
                e.Args.Add (new Node (idx, File.Exists (rootFolder + idx)));
            });
        }
    }
}
