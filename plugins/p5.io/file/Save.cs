/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using System.Collections.Generic;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help save files
    /// </summary>
    public static class Save
    {
        /// <summary>
        ///     Saves files to disc
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "save-file")]
        public static void save_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Checking if we're given an expression as destination, to make sure we support Active Event source
                if (XUtil.IsExpression (e.Args.Value)) {

                    // Destination is an expression, which means we might have an Active Event invocation source
                    var destEx = e.Args.Value as Expression;
                    foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                        // Getting relative source, and saving to file
                        List<byte> content = GetSource (context, e.Args, idxDestination.Node);

                        // Saving currently iterated file
                        SaveFile (
                            context,
                            e.Args,
                            rootFolder,
                            Common.GetSystemPath (context, Utilities.Convert<string> (context, idxDestination.Value)),
                            content);
                    }
                } else {

                    // Getting relative source, and saving to file
                    List<byte> content = GetSource (context, e.Args, null);

                    // Destination was not an expression, assuming constant filepath, possibly formatted
                    SaveFile (
                        context,
                        e.Args,
                        rootFolder,
                        Common.GetSystemPath (context, e.Args.GetExValue<string> (context)),
                        content);
                }
            }
        }

        /*
         * Helper for above
         */
        private static List<byte> GetSource (ApplicationContext context, Node parentNode, Node destination)
        {
            // Retrieving content to save, possibly relative to currently iterated expression result
            var src = XUtil.Source (context, parentNode, destination);

            // Creating our return value
            List<byte> content = new List<byte> ();
            foreach (var idxSrc in src) {

                // Converting currenyl iterated source to blob, and appending into combined content
                content.AddRange (Utilities.Convert (context, idxSrc, new byte[] { }));

                // Appending CR/LF after every lambda source, to make sure we preserve good Hyperlambda syntax, 
                // created from multiple source nodes
                if (idxSrc is Node)
                    content.AddRange (new byte[] { 13, 10 });
            }

            // Returning combined "byte[]" (blob) results to caller
            return content;
        }

        /*
         * Saves the specified text file, retrieving source from args
         */
        private static void SaveFile (
            ApplicationContext context, 
            Node args, 
            string rootFolder,
            string fileName,
            List<byte> content)
        {
            // Verifying user is allowed to save to file
            Common.RaiseAuthorizeEvent (context, args, "modify-file", fileName);

            // Saving file
            using (FileStream stream = File.Create (rootFolder + fileName)) {

                // Writing content to file stream
                stream.Write (content.ToArray (), 0, content.Count);
            }
        }
    }
}
