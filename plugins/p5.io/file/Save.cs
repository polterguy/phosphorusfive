/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;

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
        [ActiveEvent (Name = "save-file", Protection = EventProtection.LambdaClosed)]
        public static void save_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Checking if we're given an expression as destination, to make sure we support Active Event source
                if (XUtil.IsExpression (e.Args.Value)) {

                    // Destination is an expression, which means we might have an Active Event invocation source
                    var destEx = e.Args.Value as Expression;
                    foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                        // Retrieving content to save, possibly relative to currently iterated expression result
                        var content = XUtil.SourceSingle (context, e.Args, e.Args.LastChild.Name == "src" ? e.Args : idxDestination.Node);

                        // Saving currently iterated file
                        SaveFile (
                            context, 
                            e.Args, 
                            rootFolder,
                            Utilities.Convert<string> (context, idxDestination.Value), 
                            Utilities.Convert<byte[]> (context, content, new byte[0]));
                    }
                } else {

                    // Retrieving content to save
                    var content = XUtil.SourceSingle (context, e.Args);

                    // Destination was not an expression, assuming constant filepath, possibly formatted
                    SaveFile (
                        context,
                        e.Args,
                        rootFolder,
                        e.Args.GetExValue<string> (context),
                        Utilities.Convert<byte[]> (context, content, new byte[0]));
                }
            }
        }

        /*
         * Saves the specified text file, retrieving source from args
         */
        private static void SaveFile (
            ApplicationContext context, 
            Node args, 
            string rootFolder,
            string fileName,
            byte[] content)
        {
            // Verifying user is allowed to save to file
            context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", fileName).Add ("args", args));

            // Saving file
            using (FileStream stream = File.Create (rootFolder + fileName)) {

                // Writing content to file stream
                stream.Write (content, 0, content.Length);
            }
        }
    }
}
