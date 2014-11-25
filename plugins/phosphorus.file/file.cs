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
    }
}

