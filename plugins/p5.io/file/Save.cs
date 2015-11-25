/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.file.file
{
    /// <summary>
    ///     Class to help save files.
    /// </summary>
    public static class Save
    {
        /// <summary>
        ///     Saves a file to disc
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "save-file")]
        [ActiveEvent (Name = "save-text-file")]
        private static void save_file (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // getting source
                var source = Utilities.Convert<string> (XUtil.SourceSingle (e.Args, context), context);

                // getting filename
                string fileName = XUtil.Single<string> (e.Args, context);

                // saving file
                using (TextWriter writer = File.CreateText (rootFolder + fileName)) {
                    writer.Write (source);
                }

                // Returning back to caller that creation was successful
                e.Args.Value = fileName;
            }
        }

        /// <summary>
        ///     Saves zero or more binary files to disc.
        /// 
        ///     The files to save, should be given as either [source], [src], [rel-source] or [rel-src]. If 
        ///     you save one file, using a constant as the file path, then you must use [source] or [src], which
        ///     are synonyms btw. If you use an expression pointing to your file path(s), then you can use a relative
        ///     source, if you wish, by either using [rel-source] or [rel-src] pointing to the actual contents to be saved.
        /// 
        ///     Example that saves a file with the name of "foo.png"
        /// 
        ///     <pre>_data
        ///   _1:iVBORw0KGgoAAAANSUhEUgAAAAUAAAAICAYAAAAx8TU7AAAABmJLR0QAAAAAAAD5Q7t/
        ///   _2:AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3wMRESMW1yBFBwAAADBJREFUCNeNjj
        ///   _3:EKACAQw1Lx/1+Og9wJ4mC2BloaQC4mgB6fZMsKxeBBS7Vn/ut5XVoUIQsTo7AhfAAAAABJRU5ErkJggg==
        /// save-binary-file:foo.png
        ///   source:@/./-/*?value</pre>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "save-binary-file")]
        private static void save_binary_file (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // getting root folder
                var rootFolder = Common.GetRootFolder (context);

                if (e.Args.LastChild.Name != "src")
                    throw new LambdaException ("No [src] given to [save-file]", e.Args, context);

                // getting source
                var source = Utilities.Convert<byte[]> (XUtil.SourceSingle (e.Args, context), context);

                // getting filename
                string fileName = XUtil.Single<string> (e.Args, context);

                // saving file
                using (FileStream stream = File.Create (rootFolder + fileName)) {
                    stream.Write (source, 0, source.Length);
                }
            }
        }
    }
}
