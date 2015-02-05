
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Reflection;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.file
{
    /// <summary>
    /// class to help create, traverse, modify and delete folders on disc
    /// </summary>
    public static class folder
    {
        /// <summary>
        /// creates one or more folder on disc from the path given as value of args, 
        /// which might be a constant, or an expression. all folders that are successfully created will
        /// be returned as children nodes, having the path to the folder as name, and its value being true.
        /// if a folder already exists from before, the return value will be false
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.create-folder")]
        private static void pf_file_create_folder (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            XUtil.Iterate<string> (e.Args, context,
            delegate (string idx) {
                if (!Directory.Exists (rootFolder + idx)) {
                    Directory.CreateDirectory (rootFolder + idx);
                    e.Args.Add (new Node (idx, true));
                } else {
                    e.Args.Add (new Node (idx, false));
                }
            });
        }

        /// <summary>
        /// removes one or more folders from the path given as value of args, which might be a constant, or
        /// an expression. all folders that are successfully removed will be returned as children nodes,
        /// having the path to the folder as name, and its value being true. if a folder does not exist,
        /// the return value will be false for that folder
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.remove-folder")]
        private static void pf_file_remove_folder (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            XUtil.Iterate<string> (e.Args, context,
            delegate (string idx) {
                if (Directory.Exists (rootFolder + idx)) {
                    Directory.Delete (rootFolder + idx, true);
                    e.Args.Add (new Node (idx, true));
                } else {
                    e.Args.Add (new Node (idx, false));
                }
            });
        }

        /// <summary>
        /// checks to see if one or more folders exists, from the path given as value of args, which might
        /// be a constant, or an expression. all folders that exists, will be returned as children nodes,
        /// having the path to the folder as name, and its value being true. if a folder does not exist,
        /// the return value will be false for that folder
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.folder-exists")]
        private static void pf_file_folder_exists (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            XUtil.Iterate<string> (e.Args, context,
            delegate (string idx) {
                e.Args.Add (new Node (idx, Directory.Exists (rootFolder + idx)));
            });
        }

        /// <summary>
        /// list all the files in folder given as value of args given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.list-files")]
        private static void pf_file_list_files (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            XUtil.Iterate<string> (e.Args, context,
            delegate (string idx) {

                // iterating all files in current directory, and returning as nodes beneath args given
                foreach (var idxFile in Directory.GetFiles (rootFolder + idx)) {
                    e.Args.Add (new Node (string.Empty, idxFile.Replace (rootFolder, "")));
                }
            });
        }
        
        /// <summary>
        /// list all the folders in folder given as value of args given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.list-folders")]
        private static void pf_file_list_folders (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            XUtil.Iterate<string> (e.Args, context,
            delegate (string idx) {

                // iterating all folders in current directory, and returning as nodes beneath args given
                foreach (var idxFolder in Directory.GetDirectories (rootFolder + idx)) {
                    e.Args.Add (new Node (string.Empty, idxFolder.Replace (rootFolder, "")));
                }
            });
        }
    }
}
