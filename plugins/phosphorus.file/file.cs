/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using phosphorus.core;

namespace phosphorus.file
{
    /// <summary>
    /// class to help load and save files
    /// </summary>
    public static class file
    {
        /// <summary>
        /// helper to load text file
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.load")]
        private static void pf_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            string fileName = e.Args.Get<string> ();
            using (TextReader reader = File.OpenText (fileName)) {
                string content = reader.ReadToEnd ();
                e.Args.Add (new Node (string.Empty, content));
            }
        }

        /// <summary>
        /// helper to save text as file
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.save")]
        private static void pf_file_save (ApplicationContext context, ActiveEventArgs e)
        {
            Node tmp = new Node ("code");
            foreach (Node idx in e.Args.Root.Children) {
                tmp.Add (idx.Clone ());
            }
            context.Raise ("pf.nodes-2-code", tmp);
            string fileName = HttpContext.Current.Server.MapPath ("~/debug.txt");
            using (TextWriter writer = File.CreateText (fileName)) {
                writer.WriteLine (tmp.Get <string> ());
            }
        }
    }
}

