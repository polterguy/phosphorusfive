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
            string fileName = GetRootFolder (context) + e.Args.Get<string> ();
            using (TextReader reader = File.OpenText (fileName)) {
                string content = reader.ReadToEnd ();
                e.Args.Insert (0, new Node (string.Empty, content));
            }
        }

        /// <summary>
        /// list all the files in folder given as value of args given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.list-files")]
        private static void pf_file_list_files (ApplicationContext context, ActiveEventArgs e)
        {
            // iterating all files in given directory, and returning as nodes beneath args given
            foreach (var idxFile in Directory.GetFiles (GetRootFolder (context) + e.Args.Get<string> ())) {
                e.Args.Add (new Node (string.Empty, idxFile.Replace (GetRootFolder (context), "")));
            }
        }
        
        /// <summary>
        /// list all the folders in folder given as value of args given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.list-folders")]
        private static void pf_file_list_folders (ApplicationContext context, ActiveEventArgs e)
        {
            // iterating all files in given directory, and returning as nodes beneath args given
            foreach (var idxFolder in Directory.GetDirectories (e.Args.Get<string> ())) {
                e.Args.Add (new Node (string.Empty, idxFolder.Replace (GetRootFolder (context), "")));
            }
        }

        private static string _rootFolder;
        private static string GetRootFolder (ApplicationContext context)
        {
            if (_rootFolder == null) {
                Node rootNode = new Node ();
                context.Raise ("pf.get-application-root-folder", rootNode);
                _rootFolder = rootNode.Get<string> ();
            }
            return _rootFolder;
        }
    }
}

