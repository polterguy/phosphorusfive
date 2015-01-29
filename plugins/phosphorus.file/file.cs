
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
    /// class to help load and save files
    /// </summary>
    public static class file
    {
        /// <summary>
        /// loads one or more text files from the path given as value of args, which might be a constant,
        /// or an expression. if it is an expression, the expression is expected to return the path to the
        /// file as its result. multiple files can be loaded if expression leads to multiple results. each
        /// file will be appended as value of child nodes, in order retrieved from expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.load")]
        private static void pf_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            Expression.Iterate<string> (e.Args, true, 
            delegate(string idx) {
                using (TextReader reader = File.OpenText (rootFolder + idx)) {
                    string content = reader.ReadToEnd ();
                    e.Args.Add (new Node (idx, content));
                }
                return true;
            });
        }

        /// <summary>
        /// saves the last child of main node, as one or more text files from the path given as value of args, 
        /// which might be a constant, or an expression. if it is an expression, the expression is expected 
        /// to return the path to the file as its result. multiple files can be saved if expression leads to 
        /// multiple results
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.save")]
        private static void pf_file_save (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            Expression.Iterate<string> (e.Args, true, 
            delegate(string idx) {
                using (TextWriter writer = File.CreateText (rootFolder + idx)) {
                    writer.Write (e.Args.LastChild.Get<string> ());
                }
                return true;
            });
        }

        /// <summary>
        /// removes one or more files from the path given as value of args, which might be a constant, or
        /// an expression. if it is an expression, the expression is expected to return the path to the
        /// file as its result. multiple files can be removed if expression leads to multiple results
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.remove")]
        private static void pf_file_remove (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            Expression.Iterate<string> (e.Args, true, 
            delegate(string idx) {
                if (File.Exists (rootFolder + idx))
                    File.Delete (rootFolder + idx);
                return true;
            });
        }

        /// <summary>
        /// checks to see if one or more files exists, from the path given as value of args, which might
        /// be a constant, or an expression. if it is an expression, the expression is expected to return
        /// the path to the file as its result. multiple files can be checked for existence if expression
        /// leads to multiple results. if multiple files are givne through a constant, then all files must
        /// exist for Active Event to return true, otherwise Active Event will return false, and path to
        /// file that did not exist will be given as 'name' of node containing false
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.exists")]
        private static void pf_file_exists (ApplicationContext context, ActiveEventArgs e)
        {
            string rootFolder = common.GetRootFolder (context);
            Expression.Iterate<string> (e.Args, true, 
            delegate(string idx) {
                if (!File.Exists (rootFolder + idx)) {
                    e.Args.Add (new Node (idx, false));
                    return false;
                }
                return true;
            });
            e.Args.Add (new Node (string.Empty, true));
        }
    }
}
