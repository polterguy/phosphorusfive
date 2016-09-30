/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.exp;
using p5.core;
using p5.mime.helpers;
using MimeKit;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the MIME load features of Phosphorus Five
    /// </summary>
    public static class MimeLoad
    {
        /// <summary>
        ///     Loads and parses MIME message(s) from given file(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.load-file")]
        public static void p5_mime_load_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Keeping base folder to application around
                string baseFolder = Common.GetRootFolder (context).TrimEnd ('/');

                // Looping through each filename supplied by caller
                foreach (var idxFilename in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Verifying user is authorized to reading from currently iterated file
                    context.Raise (".p5.io.authorize.read-file", new Node ("", idxFilename).Add ("args", e.Args));

                    // Loading, processing and returning currently iterated message
                    var parser = new helpers.MimeParser (
                        context, 
                        e.Args, 
                        MimeEntity.Load (baseFolder + idxFilename), 
                        e.Args.GetExChildValue<string> ("attachment-folder", context));

                    // Parses the MimeEntity and stuffs results into e.Args node
                    parser.Process ();
                }
            }
        }
    }
}

