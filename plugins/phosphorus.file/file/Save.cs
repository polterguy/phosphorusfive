/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.file.file
{
    /// <summary>
    ///     class to help save files
    /// </summary>
    public static class Save
    {
        /// <summary>
        ///     saves the last child of node, as one or more text files from the path given as value of args,
        ///     which might be a constant, or an expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.save")]
        private static void pf_file_save (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.LastChild.Name == "source" || e.Args.LastChild.Name == "src") {
                // static source
                SaveFileStaticSource (e.Args, context);
            } else if (e.Args.LastChild.Name == "rel-source" || e.Args.LastChild.Name == "rel-src") {
                // relative source
                SaveFileRelativeSource (e.Args, context);
            }
        }

        /*
         * saves one or more files with a static source (content)
         */

        private static void SaveFileStaticSource (Node sourceNode, ApplicationContext context)
        {
            // getting root folder
            var rootFolder = Common.GetRootFolder (context);

            // retrieving source, or "content" for file(s)
            var source = Utilities.Convert<string> (XUtil.SourceSingle (sourceNode, context), context);

            // iterating through each file path given
            foreach (var idx in XUtil.Iterate<string> (sourceNode, context)) {
                // saving source to currenly iterated file
                using (TextWriter writer = File.CreateText (rootFolder + idx)) {
                    writer.Write (source);
                }
            }
        }

        /*
         * saves a file with relative source (content)
         */

        private static void SaveFileRelativeSource (Node sourceNode, ApplicationContext context)
        {
            // getting root folder
            var rootFolder = Common.GetRootFolder (context);

            // iterating over each destination
            foreach (var idx in XUtil.Iterate (sourceNode, context)) {
                using (TextWriter writer = File.CreateText (rootFolder + idx.Value)) {
                    // finding source relative to destination
                    var source = Utilities.Convert<string> (XUtil.SourceSingle (sourceNode, idx.Node, context), context);
                    writer.Write (source);
                }
            }
        }
    }
}