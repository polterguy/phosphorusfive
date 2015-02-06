
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
    /// class to help save files
    /// </summary>
    public static class file_save
    {
        /// <summary>
        /// saves the last child of node, as one or more text files from the path given as value of args, 
        /// which might be a constant, or an expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.save")]
        private static void pf_file_save (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.LastChild.Name == "source") {

                // static source
                SaveFileStaticSource (e.Args.LastChild, e.Args, context);
            } else if (e.Args.LastChild.Name == "rel-source") {

                // relative source
                SaveFileRelativeSource (e.Args, context);
            } else {

                // oops ...!
                throw new ArgumentException ("no [source] or [rel-source] given to [pf.file.save]");
            }
        }

        /*
         * saves one or more files with a static source (content)
         */
        private static void SaveFileStaticSource (Node sourceNode, Node destinationNode, ApplicationContext context)
        {
            // getting root folder
            string rootFolder = common.GetRootFolder (context);

            // retrieving source, or "content" for file(s)
            string source = XUtil.Single<string> (sourceNode, context);

            // iterating through each path given
            XUtil.Iterate<string> (destinationNode, context,
            delegate (string idx) {
                using (TextWriter writer = File.CreateText (rootFolder + idx)) {
                    writer.Write (source);
                }
            });
        }

        /*
         * saves a file with relative source (content)
         */
        private static void SaveFileRelativeSource (Node destinationNode, ApplicationContext context)
        {
            // getting root folder
            string rootFolder = common.GetRootFolder (context);

            // iterating over each destination
            XUtil.Iterate (destinationNode.Get<string> (context), destinationNode, context,
            delegate (MatchEntity idx) {
                using (TextWriter writer = File.CreateText (rootFolder + idx.Value)) {

                    // finding source relative to destination
                    string source = XUtil.Single<string> (destinationNode.LastChild, idx.Node, context);
                    writer.Write (source);
                }
            });
        }
    }
}
