
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Reflection;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.file
{
    /// <summary>
    /// class to help create, traverse, modify and delete folders on disc
    /// </summary>
    public static class folder
    {
        /// <summary>
        /// creates a folder on disc
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.create-folder")]
        private static void pf_file_create_folder (ApplicationContext context, ActiveEventArgs e)
        {
            string folderName = e.Args.Get<string> ();
            if (Expression.IsExpression (folderName)) {
                var match = Expression.Create (folderName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.remove] must be given an expression returning one single 'value' or 'name'");
                folderName = match.GetValue (0) as string;
            } else if (e.Args.Count > 0) {
                folderName = Expression.FormatNode (e.Args);
            }
            Directory.CreateDirectory (common.GetRootFolder (context) + folderName);
        }

        /// <summary>
        /// removes a folder from disc
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.remove-folder")]
        private static void pf_file_remove_folder (ApplicationContext context, ActiveEventArgs e)
        {
            string folderName = e.Args.Get<string> ();
            if (Expression.IsExpression (folderName)) {
                var match = Expression.Create (folderName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.remove] must be given an expression returning one single 'value' or 'name'");
                folderName = match.GetValue (0) as string;
            } else if (e.Args.Count > 0) {
                folderName = Expression.FormatNode (e.Args);
            }
            Directory.Delete (common.GetRootFolder (context) + folderName, true);
        }

        /// <summary>
        /// checks to see if a folder exist on disc
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.folder-exists")]
        private static void pf_file_folder_exists (ApplicationContext context, ActiveEventArgs e)
        {
            string folderName = e.Args.Get<string> ();
            if (Expression.IsExpression (folderName)) {
                var match = Expression.Create (folderName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.remove] must be given an expression returning one single 'value' or 'name'");
                folderName = match.GetValue (0) as string;
            } else if (e.Args.Count > 0) {
                folderName = Expression.FormatNode (e.Args);
            }
            e.Args.Add (new Node (string.Empty, Directory.Exists (common.GetRootFolder (context) + folderName)));
        }

        /// <summary>
        /// list all the files in folder given as value of args given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.list-files")]
        private static void pf_file_list_files (ApplicationContext context, ActiveEventArgs e)
        {
            string folderName = e.Args.Get<string> ();
            if (Expression.IsExpression (folderName)) {
                var match = Expression.Create (folderName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.remove] must be given an expression returning one single 'value' or 'name'");
                folderName = match.GetValue (0) as string;
            } else if (e.Args.Count > 0) {
                folderName = Expression.FormatNode (e.Args);
            }

            // iterating all files in given directory, and returning as nodes beneath args given
            foreach (var idxFile in Directory.GetFiles (common.GetRootFolder (context) + folderName)) {
                if (!idxFile.EndsWith ("~")) // "hidden" file ...
                    e.Args.Add (new Node (string.Empty, idxFile.Replace (common.GetRootFolder (context), "")));
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
            string folderName = e.Args.Get<string> ();
            if (Expression.IsExpression (folderName)) {
                var match = Expression.Create (folderName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.remove] must be given an expression returning one single 'value' or 'name'");
                folderName = match.GetValue (0) as string;
            } else if (e.Args.Count > 0) {
                folderName = Expression.FormatNode (e.Args);
            }

            // iterating all files in given directory, and returning as nodes beneath args given
            foreach (var idxFolder in Directory.GetDirectories (common.GetRootFolder (context) + folderName)) {
                e.Args.Add (new Node (string.Empty, idxFolder.Replace (common.GetRootFolder (context), "")));
            }
        }
    }
}
