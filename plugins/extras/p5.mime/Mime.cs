/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.mime.helpers;
using p5.exp.exceptions;
using MimeKit;
using MimeKit.Cryptography;

/// <summary>
///     Main namespace regarding all MIME features of Phosphorus Five
/// </summary>
namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the MIME features of Phosphorus Five
    /// </summary>
    public static class Mime
    {
        /// <summary>
        ///     Invoked during initial startup of application
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.core.application-start", Protection = EventProtection.NativeOpen)]
        private static void p5_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // Registering our Cryptography context, which is the local installation of Gnu Privacy Guard
            CryptographyContext.Register (typeof (GnuPrivacyContext));
        }

        /// <summary>
        ///     Creates a native MimeEntity according to given arguments and returns to caller as MimeEntity
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.create-native", Protection = EventProtection.NativeClosed)]
        private static void p5_mime_create_native (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Children.Count != 1)
                throw new LambdaException (
                    "You must have exactly one root node of your MIME message",
                    e.Args,
                    context);

            // Creating and returning MIME message to caller as MimeEntity
            e.Args.Value = CreateMime.CreateEntity (context, e.Args.FirstChild, (List<Stream>)e.Args.Value);
        }

        /// <summary>
        ///     Creates a MIME message according to given arguments and returns as text
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.create", Protection = EventProtection.LambdaClosed)]
        private static void p5_mime_create (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Children.Count != 1)
                throw new LambdaException (
                    "You must have exactly one root node of your MIME message",
                    e.Args,
                    context);

            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Making sure we keep track of, close, and disposes all streams created during process
                List<Stream> streams = new List<Stream> ();
                try {

                    // Creating and returning MIME message to caller as string
                    e.Args.Value = CreateMime.CreateEntity (context, e.Args.FirstChild, streams).ToString ();
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

        /// <summary>
        ///     Parses the MIME message given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.parse-native", Protection = EventProtection.NativeClosed)]
        private static void p5_mime_parse_native (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving MimeEntity from caller's arguments
            var entity = e.Args.Get<MimeEntity> (context);
            ParseMime.ParseMimeEntity (
                context, 
                e.Args,
                entity);
        }

        /// <summary>
        ///     Parsess the MIME message given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.parse", Protection = EventProtection.LambdaClosed)]
        private static void p5_mime_parse (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving MimeEntity from caller's arguments as object
                using (var writer = new StreamWriter (new MemoryStream ())) {

                    // Writing MIME content to StreamWriter, flushing the stream, and setting reader head back to beginning
                    writer.Write (e.Args.Value);
                    writer.Flush ();
                    writer.BaseStream.Position = 0;

                    // Figuring out if user supplied a PGP private key email address, and password, and if so, passing these into
                    // the parse method, to automatically decrypt MIME entity
                    string pgpPrivateKey = null, pgpPassword = null;
                    if (e.Args ["email"] != null) {

                        // Fetching private key and password from args
                        pgpPrivateKey = e.Args.GetChildValue<string> ("email", context, null);
                        pgpPassword = e.Args ["email"].GetChildValue<string> ("password", context, null);
                    }

                    // Loading MimeEntity from MemoryStream before parsing, and putting results into args returns value
                    var entity = MimeEntity.Load (writer.BaseStream);
                    ParseMime.ParseMimeEntity (
                        context, 
                        e.Args,
                        entity,
                        false, 
                        pgpPrivateKey,
                        pgpPassword);
                }
            }
        }

        /// <summary>
        ///     Loads and parses a MIME message from given file(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.load", Protection = EventProtection.LambdaClosed)]
        private static void p5_mime_load (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Keeping base folder to application around
                string baseFolder = Common.GetBaseFolder (context);

                // Looping through each filename supplied by caller
                foreach (var idxFilename in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Verifying user is authorized to reading from currently iterated file
                    context.RaiseNative ("p5.io.authorize.read-file", new Node ("p5.io.authorize.read-file", idxFilename).Add ("args", e.Args));

                    // Loading, processing and returning currently iterated message
                    ParseMime.ParseMimeEntity (
                        context,
                        e.Args,
                        MimeEntity.Load (baseFolder + idxFilename));
                }
            }
        }
    }
}

