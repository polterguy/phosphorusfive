/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.mime.helpers;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the MIME save features of Phosphorus Five
    /// </summary>
    public static class MimeSave
    {
        /// <summary>
        ///     Creates and saves a MIME message from given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.save-file")]
        public static void p5_mime_save_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Keeping base folder to application around
                string baseFolder = Common.GetRootFolder (context);

                // Retrieving filename supplied by caller to serialize MimeEntity to
                var filename = XUtil.Single<string> (context, e.Args, true);

                // Verifying user is authorized to saving to the specified file
                context.Raise (".p5.io.authorize.modify-file", new Node ("", filename).Add ("args", e.Args));

                // Retrieving root MIME entity from args
                var mimeNode = e.Args [0];

                // Making sure we keep track of, closes, and disposes all streams created during process
                List<Stream> streams = new List<Stream> ();
                try {

                    // Creating and returning MIME message to caller as string
                    MimeCreator creator = new MimeCreator(
                        context, 
                        mimeNode,
                        streams);
                    var mimeEntity = creator.Create ();

                    // Creating file to store MIME entity into
                    using (var stream = File.Create (baseFolder + filename)) {

                        // Writing MimeEntity to file stream
                        mimeEntity.WriteTo (stream);
                    }
                } finally {

                    // Disposing all streams created during process
                    foreach (var idxStream in streams) {

                        // Closing and disposing currently iterated stream
                        idxStream.Close ();
                        idxStream.Dispose ();
                    }
                }
            }
        }
    }
}

