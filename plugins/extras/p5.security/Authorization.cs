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

namespace p5.security
{
    /// <summary>
    ///     Class wrapping authorization features of Phosphorus Five
    /// </summary>
    internal static class Authorization
    {
        private static string _rootFolder;

        /// <summary>
        ///     Throws an exception if user is not authorized to doing the requested action
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "authorize", Protected = true)]
        private static void authorize (ApplicationContext context, ActiveEventArgs e)
        {
            // Verifying syntax of invocation
            if (e.Args.Count == 0)
                throw new LambdaException ("No arguments supplied to [authorize]", e.Args, context);

            switch (e.Args [0].Name) {
            case "read-file":
                AuthorizeReadFile (context, e.Args [0].Get<string> (context), e.Args);
                break;
            case "write-file":
                AuthorizeWriteFile (context, e.Args [0].Get<string> (context), e.Args);
                break;
            case "execute-file":
                break;
            default:
                throw new LambdaException ("Sorry, I don't know how to [authorize] '" + e.Args.Name + "'", e.Args, context);
            }
        }

        /*
         * Verifies user is authorized to reading specified file
         */
        private static void AuthorizeReadFile (ApplicationContext context, string filename, Node args)
        {
            // Checking of user is root (root is authorized to do everything!)
            if (context.Ticket.Role != "root") {

                // Making sure we remove root folder, if it is given as part of filename
                if (filename.IndexOf (GetRootFolder (context)) == 0)
                    filename = filename.Substring (GetRootFolder (context).Length);

                // Verifying file is underneath user's folder
                if (filename.IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to read file '{1}'", context.Ticket.Username, filename), 
                        args["args"].Get<Node> (context), 
                        context);

                // Verifying filename is not "protected" type (starts with "_")
                if (GetFileName (filename).StartsWith ("_"))
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to read protected file '{1}'", context.Ticket.Username, filename), 
                        args["args"].Get<Node> (context), 
                        context);

                // Verifying suffix of file is a type of file that user is allowed to read
                switch (GetFileSuffix (filename)) {

                // Creating WHITELIST ...!
                case "html":
                case "htm":
                case "css":
                case "js":
                case "png":
                case "gif":
                case "jpeg":
                case "jpg":
                case "pdf":
                case "txt":
                case "hl":
                case "md":
                    break;
                default:
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to read file '{1}'", context.Ticket.Username, filename), 
                        args["args"].Get<Node> (context), 
                        context);
                }
            }
        }

        /*
         * Verifies user is authorized to writing to specified file
         */
        private static void AuthorizeWriteFile (ApplicationContext context, string filename, Node args)
        {
            // Checking of user is root (root is authorized to do everything!)
            if (context.Ticket.Role != "root") {

                // Making sure we remove root folder, if it is given as part of filename
                if (filename.IndexOf (GetRootFolder (context)) == 0)
                    filename = filename.Substring (GetRootFolder (context).Length);

                // Verifying file is underneath user's folder
                if (filename.IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        args["args"].Get<Node> (context), 
                        context);

                // Verifying filename is not "protected" type (starts with "_")
                if (GetFileName (filename).StartsWith ("_"))
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to protected file '{1}'", context.Ticket.Username, filename), 
                        args["args"].Get<Node> (context), 
                        context);

                // Verifying suffix of file is a type of file that user is allowed to save
                switch (GetFileSuffix (filename)) {

                // Creating WHITELIST ...!
                case "html":
                case "htm":
                case "css":
                case "js":
                case "png":
                case "gif":
                case "jpeg":
                case "jpg":
                case "pdf":
                case "txt":
                case "hl":
                case "md":
                    break;
                default:
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        args["args"].Get<Node> (context), 
                        context);
                }
            }
        }

        /*
         * Helper to retrieve suffix of file
         */
        private static string GetFileSuffix (string filepath)
        {
            if (filepath.Contains ("/"))
                filepath = filepath.Substring (filepath.LastIndexOf ("/") + 1);
            if (filepath.Contains ("."))
                return filepath.Substring (filepath.LastIndexOf (".") + 1);
            return string.Empty; //no suffix ...
        }

        /*
         * Helper to retrieve filename of file, without path
         */
        private static string GetFileName (string filepath)
        {
            if (filepath.Contains ("/"))
                filepath = filepath.Substring (filepath.LastIndexOf ("/") + 1);
            return filepath;
        }

        /*
         * Helper to retrieve root folder of application
         */
        private static string GetRootFolder (ApplicationContext context)
        {
            if (_rootFolder == null)
                _rootFolder = context.Raise ("p5.core.application-folder").Get<string> (context);
            return _rootFolder;
        }
    }
}