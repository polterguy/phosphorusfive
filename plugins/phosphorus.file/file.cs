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
        /// loads a text file from the path given as value of args given and returns as first child node's value
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.load")]
        private static void pf_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            string fileName = e.Args.Get<string> ();
            using (TextReader reader = File.OpenText (fileName)) {
                string content = reader.ReadToEnd ();
                e.Args.Insert (0, new Node (string.Empty, content));
            }
        }

        /// <summary>
        /// list all the files in directory given as value of args given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.list-files")]
        private static void pf_file_list_files (ApplicationContext context, ActiveEventArgs e)
        {
            // iterating all files in given directory, and returning as nodes beneath args given
            foreach (var idxFile in Directory.GetFiles (e.Args.Get<string> ())) {
                e.Args.Add (new Node (string.Empty, idxFile));
            }
        }
    }
}

