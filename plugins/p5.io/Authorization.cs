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
        [ActiveEvent (Name = "_authorize-save-file")]
        private static void _authorize_save_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizeSaveFile (context, e.Args.Get<string> (context).TrimStart ('/'), e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to read the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "_authorize-load-file")]
        private static void _authorize_load_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizeLoadFile (context, e.Args.Get<string> (context).TrimStart ('/'), e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to save the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "_authorize-save-folder")]
        private static void _authorize_save_folder (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizeSaveFolder (context, "/" + e.Args.Get<string> (context).TrimStart ('/'), e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to read the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "_authorize-load-folder")]
        private static void _authorize_load_folder (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizeLoadFolder (context, "/" + e.Args.Get<string> (context).TrimStart ('/'), e.Args ["args"].Get<Node> (context));
        }

        /*
         * Verifies user is authorized writing to the specified file
         */
        private static void AuthorizeSaveFile (ApplicationContext context, string filename, Node stack)
        {
            // Checking if user is root (root is authorized to do almost everything!)
            if (context.Ticket.Role != "root") {

                // Verifying file is underneath user's folder
                if (filename.IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);

                // Verifying suffix of file is a type of file that user is allowed to save
                switch (Path.GetExtension (filename)) {

                // Blacklisted ...!
                case ".config":
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);
                }
            } else {

                // Verifying file is not underneath ANOTHER user's folder, which is not legal even for root account!
                if (filename.StartsWith ("users/") && filename.IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("Root user '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);
            }

            // Verifying "auth file" is safe
            if (filename == Common.GetAuthFile (context))
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to access 'auth' file", context.Ticket.Username, filename), 
                    stack, 
                    context);
        }

        /*
         * Verifies user is authorized reading from the specified file
         */
        private static void AuthorizeLoadFile (ApplicationContext context, string filename, Node stack)
        {
            // Verifying file is underneath authenticated user's folder, if it is underneath "users/" folders
            if (filename.StartsWith ("users/") && filename.IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) != 0)
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to read file '{1}'", context.Ticket.Username, filename), 
                    stack, 
                    context);

            // Verifying auth file is safe
            filename = filename.TrimStart ('/').TrimEnd ('.');
            var authFile = Common.GetAuthFile (context).Replace ("~/", "");
            if (filename.ToLower () == authFile.ToLower ())
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to access auth file", context.Ticket.Username, filename), 
                    stack, 
                    context);
        }

        /*
         * Verifies user is authorized writing to the specified folder
         */
        private static void AuthorizeSaveFolder (ApplicationContext context, string foldername, Node stack)
        {
            // Checking if user is root (root is authorized to do almost everything!)
            if (context.Ticket.Role != "root") {

                // Verifying file is underneath user's folder
                if (foldername.IndexOf (string.Format ("/users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to folder '{1}'", context.Ticket.Username, foldername), 
                        stack, 
                        context);
            } else {

                // Verifying folder is not underneath ANOTHER user's folder, which is not legal even for root account!
                if (foldername.StartsWith ("/users/") && foldername.IndexOf (string.Format ("/users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("Root user '{0}' tried to write to folder '{1}'", context.Ticket.Username, foldername), 
                        stack, 
                        context);
            }
        }

        /*
         * Verifies user is authorized reading from the specified folder
         */
        private static void AuthorizeLoadFolder (ApplicationContext context, string foldername, Node stack)
        {
            // Verifying file is underneath authorized user's folder, if it is underneath "/users/" folders
            if (foldername.StartsWith ("/users/") && foldername.Length != 7 && foldername.IndexOf (string.Format ("/users/{0}/", context.Ticket.Username)) != 0)
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to read from folder '{1}'", context.Ticket.Username, foldername), 
                    stack, 
                    context);
        }
    }
}