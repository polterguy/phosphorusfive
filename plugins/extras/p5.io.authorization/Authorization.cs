/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Security;
using System.Configuration;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.io.authorization.helpers;

namespace p5.io.authorization
{
    /// <summary>
    ///     Class wrapping authorization for files in Phosphorus Five
    /// </summary>
    internal static class Authorization
    {
        /// <summary>
        ///     Throws an exception if user is not authorized to save the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.io.authorize.save-file", Protection = EventProtection.NativeClosed)]
        private static void p5_io_authorize_save_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeSaveFile (
                context, 
                Common.NormalizeFileName (e.Args.Get<string> (context)), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to read the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.io.authorize.load-file", Protection = EventProtection.NativeClosed)]
        private static void p5_io_authorize_load_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeLoadFile (
                context, 
                Common.NormalizeFileName (e.Args.Get<string> (context)), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to save the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.io.authorize.save-folder", Protection = EventProtection.NativeClosed)]
        private static void p5_io_authorize_save_folder (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeSaveFolder (
                context, 
                Common.NormalizeFolderName (e.Args.Get<string> (context)), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to read the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.io.authorize.load-folder", Protection = EventProtection.NativeClosed)]
        private static void _authorize_load_folder (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeLoadFolder (
                context, 
                Common.NormalizeFolderName (e.Args.Get<string> (context)), 
                e.Args ["args"].Get<Node> (context));
        }
    }
}