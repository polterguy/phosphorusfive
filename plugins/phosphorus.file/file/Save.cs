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
    ///     Class to help save files.
    /// </summary>
    public static class Save
    {
        /// <summary>
        ///     Saves zero or more files to disc.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.file.save")]
        private static void pf_file_save (ApplicationContext context, ActiveEventArgs e)
        {
            // getting root folder
            var rootFolder = Common.GetRootFolder (context);

            if (e.Args.LastChild.Name == "source" || e.Args.LastChild.Name == "src") {

                // static source
                var source = Utilities.Convert<string> (XUtil.SourceSingle (e.Args, context), context);

                // iterating through each file path given
                foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                    // saving source to currenly iterated file
                    using (TextWriter writer = File.CreateText (rootFolder + idx)) {
                        writer.Write (source);
                    }
                }
            } else if (e.Args.LastChild.Name == "rel-source" || e.Args.LastChild.Name == "rel-src") {

                // relative source
                foreach (var idx in XUtil.Iterate (e.Args, context)) {
                    using (TextWriter writer = File.CreateText (rootFolder + idx.Value)) {
                        // finding source relative to destination
                        var source = Utilities.Convert<string> (XUtil.SourceSingle (e.Args, idx.Node, context), context);
                        writer.Write (source);
                    }
                }
            }
        }
    }
}