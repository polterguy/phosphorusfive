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
using p5.core.configuration;

namespace p5.io
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
        [ActiveEvent (Name = "_authorize-save-file", Protected = true)]
        private static void _authorize_save_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizeWriteFile (context, e.Args.Get<string> (context).TrimStart ('/'), e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to read the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "_authorize-load-file", Protected = true)]
        private static void _authorize_load_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizeLoadFile (context, e.Args.Get<string> (context).TrimStart ('/'), e.Args ["args"].Get<Node> (context));
        }

        /*
         * Verifies user is authorized writing to the specified file
         */
        private static void AuthorizeWriteFile (ApplicationContext context, string filename, Node stack)
        {
            // Checking if user is root (root is authorized to do everything!)
            if (context.Ticket.Role != "root") {

                // Verifying file is underneath user's folder
                if (filename.IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);

                // Verifying suffix of file is a type of file that user is allowed to save
                switch (Path.GetExtension (filename)) {

                // Creating blacklist ...!
                case "config":
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);
                }
            }
        }

        /*
         * Verifies user is authorized reading the specified file
         */
        private static void AuthorizeLoadFile (ApplicationContext context, string filename, Node stack)
        {
            // Verifying file is underneath user's folder, if it is underneath "users/" folders
            if (filename.StartsWith ("users/") && filename.IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) != 0)
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to read file '{1}'", context.Ticket.Username, filename), 
                    stack, 
                    context);
        }
    }
}