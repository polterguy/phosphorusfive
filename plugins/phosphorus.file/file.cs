
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
        /// loads a text file from the path given as value of args given and returns as first child node's value.
        /// the value given can either be a constant, expression or formatting expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.load")]
        private static void pf_file_load (ApplicationContext context, ActiveEventArgs e)
        {
            string fileName = e.Args.Get<string> ();
            if (Expression.IsExpression (fileName)) {
                var match = Expression.Create (fileName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.load] must be given an expression returning one single 'value' or 'name'");
                fileName = match.GetValue (0) as string;
            } else if (e.Args.Count > 0) {
                fileName = Expression.FormatNode (e.Args);
            }
            using (TextReader reader = File.OpenText (common.GetRootFolder (context) + fileName)) {
                string content = reader.ReadToEnd ();
                e.Args.Insert (0, new Node (string.Empty, content));
            }
        }

        /// <summary>
        /// saves a piece of text from the first child node's value to the path given as value of args.
        /// the value given can either be a constant or an expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.save")]
        private static void pf_file_save (ApplicationContext context, ActiveEventArgs e)
        {
            string fileName = e.Args.Get<string> ();
            if (Expression.IsExpression (fileName)) {
                var match = Expression.Create (fileName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.save] must be given an expression returning one single 'value' or 'name'");
                fileName = match.GetValue (0) as string;
            }
            using (TextWriter writer = File.CreateText (common.GetRootFolder (context) + fileName)) {
                writer.Write (e.Args [0].Get<string> ());
            }
        }

        /// <summary>
        /// removes a file from disc
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.remove")]
        private static void pf_file_remove (ApplicationContext context, ActiveEventArgs e)
        {
            string fileName = e.Args.Get<string> ();
            if (Expression.IsExpression (fileName)) {
                var match = Expression.Create (fileName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.remove] must be given an expression returning one single 'value' or 'name'");
                fileName = match.GetValue (0) as string;
            } else if (e.Args.Count > 0) {
                fileName = Expression.FormatNode (e.Args);
            }
            File.Delete (common.GetRootFolder (context) + fileName);
        }

        /// <summary>
        /// checks to see if a file exists on disc
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.exists")]
        private static void pf_file_exists (ApplicationContext context, ActiveEventArgs e)
        {
            string fileName = e.Args.Get<string> ();
            if (Expression.IsExpression (fileName)) {
                var match = Expression.Create (fileName).Evaluate (e.Args);
                if (!match.IsSingleLiteral)
                    throw new ArgumentException ("[pf.file.remove] must be given an expression returning one single 'value' or 'name'");
                fileName = match.GetValue (0) as string;
            } else if (e.Args.Count > 0) {
                fileName = Expression.FormatNode (e.Args);
            }
            e.Args.Add (new Node (string.Empty, File.Exists (common.GetRootFolder (context) + fileName)));
        }
    }
}
