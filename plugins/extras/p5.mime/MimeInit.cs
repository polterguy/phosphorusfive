/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.mime.helpers;
using helpers = p5.mime.helpers;
using p5.exp.exceptions;
using MimeKit;
using MimeKit.Cryptography;

/// <summary>
///     Main namespace regarding all MIME features of Phosphorus Five
/// </summary>
namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the common MIME features of Phosphorus Five
    /// </summary>
    public static class MimeInit
    {
        /// <summary>
        ///     Invoked during initial startup of application. Registers cryptography context (GnuPG)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.core.application-start", Protection = EventProtection.NativeOpen)]
        private static void p5_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // Registering our Cryptography context, which is wrapping the local installation of Gnu Privacy Guard
            CryptographyContext.Register (typeof (GnuPrivacyContext));
        }
    }
}

