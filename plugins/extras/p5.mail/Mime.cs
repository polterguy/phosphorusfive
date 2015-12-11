/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.mail.mime;
using p5.mail.helpers;
using MimeKit;
using MimeKit.Cryptography;

/// <summary>
///     Main namespace regarding all email features of Phosphorus Five
/// </summary>
namespace p5.mail
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
        [ActiveEvent (Name = "p5.mail.mime.create-native", Protection = EventProtection.NativeClosed)]
        private static void p5_mail_mime_create_native (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Count != 1)
                throw new LambdaException (
                    "You must have one root node of your MIME message, use [multipart] as root to associate multiple objects with your message",
                    e.Args,
                    context);

            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Creating and returning MIME message to caller, in addition to all streams created during process
                e.Args.Value = CreateMime.CreateEntity (context, e.Args.FirstChild, (List<Stream>)e.Args.Value);
            }
        }

        /// <summary>
        ///     Creates a MIME message according to given arguments and returns as text
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.mime.create", Protection = EventProtection.LambdaClosed)]
        private static void p5_mail_mime_create (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Count != 1)
                throw new LambdaException (
                    "You must have one root node of your MIME message, use [multipart] as root to associate multiple objects with your message",
                    e.Args,
                    context);

            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Making sure we keep track of, close, and disposes all streams created during process
                List<Stream> streams = new List<Stream> ();
                try {

                    // Creating and returning MIME message to caller
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
        ///     Loads and parses a MIME message from given file(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.mime.load", Protection = EventProtection.LambdaClosed)]
        private static void p5_mail_mime_load (ApplicationContext context, ActiveEventArgs e)
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
                        e.Args.Add ("body").LastChild,
                        MimeEntity.Load (baseFolder + idxFilename));
                }
            }
        }

        /// <summary>
        ///     Parsess the MIME message given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.mime.parse", Protection = EventProtection.LambdaClosed)]
        private static void p5_mail_mime_parse (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving MimeEntity from caller's arguments
                using (var writer = new StreamWriter (new MemoryStream ())) {
                    writer.Write (e.Args.GetExValue (context, ""));
                    writer.Flush ();
                    writer.BaseStream.Position = 0;
                    var entity = MimeEntity.Load (writer.BaseStream);
                    ParseMime.ParseMimeEntity (
                        context, 
                        e.Args,
                        entity);
                }
            }
        }

        /// <summary>
        ///     Parses the MIME message given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.mime.parse-native", Protection = EventProtection.NativeClosed)]
        private static void p5_mail_mime_parse_native (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving MimeEntity from caller's arguments
                var entity = e.Args.Get<MimeEntity> (context);
                ParseMime.ParseMimeEntity (
                    context, 
                    e.Args.Add ("body").LastChild,
                    entity);
            }
        }
    }
}

