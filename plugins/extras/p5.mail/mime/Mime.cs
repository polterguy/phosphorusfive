/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Collections.Generic;
using p5.core;
using MimeKit;
using MimeKit.Cryptography;

/// <summary>
///     Main namespace for all features regarding MIME
/// </summary>
namespace p5.mail.mime
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
            // Registers our Cryptography context, which is the local installation of Gnu Privacy Guard
            CryptographyContext.Register (typeof (GnuPrivacyContext));
        }

        /// <summary>
        ///     Creates a native MimeEntity according to given arguments and returns to caller
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.mime.create-native", Protection = EventProtection.NativeClosed)]
        private static void p5_mail_mime_create_native (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Creating and returning MIME message to caller, in addition to all streams created during process
                e.Args.Value = MimeCreator.Create (context, e.Args, (List<Stream>)e.Args.Value);
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
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args)) {

                // Making sure we keep track of, close, and disposes all streams created during process
                List<Stream> streams = new List<Stream> ();
                try {

                    // Creating and returning MIME message to caller
                    e.Args.Value = MimeCreator.Create (context, e.Args, streams).ToString ();
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

