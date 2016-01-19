/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help save files
    /// </summary>
    public static class Save
    {
        /// <summary>
        ///     Saves a file to disc
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "save-file", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "save-text-file", Protection = EventProtection.LambdaClosed)]
        public static void save_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Checking if we're given an expression as destination, to make sure we support [rel-src] and Active Event source
                if (XUtil.IsExpression (e.Args.Value)) {

                    // Destination is an expression, which means we might have a [rel-src] or Active Event source
                    var destEx = e.Args.Value as Expression;
                    foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                        // Retrieving content to save, possibly relative to currently iterated expression result
                        var content = XUtil.SourceSingle (context, e.Args, idxDestination.Node);

                        // Saving currently iterated file
                        SaveTextFile (
                            context, 
                            e.Args, 
                            rootFolder + Utilities.Convert<string> (context, idxDestination.Value), 
                            Utilities.Convert<string> (context, content));
                    }
                } else {

                    // Retrieving content to save
                    var content = Utilities.Convert<string> (context, XUtil.SourceSingle (context, e.Args));

                    // Destination was not an expression, assuming constant filepath, possibly formatted
                    SaveTextFile (
                        context,
                        e.Args,
                        rootFolder + e.Args.GetExValue<string> (context),
                        content);
                }
            }
        }

        /// <summary>
        ///     Saves a binary file to disc
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "save-binary-file", Protection = EventProtection.LambdaClosed)]
        public static void save_binary_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Checking if we're given an expression as destination, to make sure we support [rel-src] and Active Event source
                if (XUtil.IsExpression (e.Args.Value)) {

                    // Destination is an expression, which means we might have a [rel-src] or Active Event source
                    var destEx = e.Args.Value as Expression;
                    foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                        // Retrieving content to save, possibly relative to currently iterated expression result
                        var content = XUtil.SourceSingle (context, e.Args, idxDestination.Node);

                        // Saving currently iterated file
                        SaveBinaryFile (
                            context, 
                            e.Args, 
                            rootFolder + Utilities.Convert<string> (context, idxDestination.Value), 
                            Utilities.Convert<byte[]> (context, content));
                    }
                } else {

                    // Retrieving content to save
                    var content = Utilities.Convert<byte[]> (context, XUtil.SourceSingle (context, e.Args));

                    // Destination was not an expression, assuming constant filepath
                    SaveBinaryFile (
                        context,
                        e.Args,
                        rootFolder + e.Args.GetExValue<string> (context),
                        content);
                }
            }
        }

        /*
         * Saves the specified text file, retrieving source from args
         */
        private static void SaveTextFile (
            ApplicationContext context, 
            Node args, 
            string fileName,
            string content)
        {
            // Verifying user is allowed to save to file
            context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", fileName).Add ("args", args));

            // Saving file
            using (TextWriter writer = File.CreateText (fileName)) {

                // Writing content to file stream
                writer.Write (content);
            }
        }

        /*
         * Saves the specified text file, retrieving source from args
         */
        private static void SaveBinaryFile (
            ApplicationContext context, 
            Node args, 
            string fileName,
            byte[] content)
        {
            // Verifying user is allowed to save to file
            context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", fileName).Add ("args", args));

            // Saving file
            using (FileStream stream = File.Create (fileName)) {

                // Writing content to file stream
                stream.Write (content, 0, content.Length);
            }
        }
    }
}
