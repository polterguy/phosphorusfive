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
                // As a general rule, user is allowed to execute most files ...
                break;
            case "delete-data":
                AuthorizeDeleteData (context, e.Args [0].Get<Node> (context), e.Args);
                break;
            case "insert-data":
                AuthorizeInsertData (context, e.Args [0].Get<Node> (context), e.Args);
                break;
            case "select-data":
                AuthorizeSelectData (context, e.Args [0].Get<Node> (context), e.Args);
                break;
            case "update-data":
                AuthorizeUpdateData (context, e.Args [0].Get<Node> (context), e.Args);
                break;
            default:
                throw new LambdaException (
                    "Sorry, I don't know how to [authorize] '" + e.Args [0].Name + "'", 
                    e.Args["args"].Get<Node> (context), 
                    context);
            }
        }

        /*
         * Verifies user is authorized to deleting specified data node
         */
        private static void AuthorizeDeleteData (ApplicationContext context, Node node, Node args)
        {
            // Making sure it is impossible to remove root nodes or file nodes from database, regardless of who you are!
            if (node.Path.Count < 2)
                throw new LambdaSecurityException (
                    "You cannot remove file nodes, or root nodes from your database", 
                    args["args"].Get<Node> (context), 
                    context);
            
            // Checking if user is root (root is authorized to do almost everything!)
            if (context.Ticket.Role != "root") {

                // Making sure protected data is not deleted
                while (node.Path.Count != 2) {
                    node = node.Parent;
                }

                // Verifying main data node is not protected (starts with "_")
                if (node.Name.StartsWith ("_"))
                    throw new LambdaSecurityException (
                        string.Format ("You cannot delete protected node '{0}' from your database, or parts of its descendants", node.Name), 
                        args["args"].Get<Node> (context), 
                        context);
            }
        }

        /*
         * Verifies user is authorized to insert specified data node
         */
        private static void AuthorizeInsertData (ApplicationContext context, Node node, Node args)
        {
            // Checking if user is root (root is authorized to do almost everything!)
            if (context.Ticket.Role != "root") {

                // Verifying main data node is not protected (starts with "_")
                if (node.Name.StartsWith ("_"))
                    throw new LambdaSecurityException (
                        string.Format ("You cannot insert a protected node '{0}' into your database", node.Name), 
                        args["args"].Get<Node> (context), 
                        context);
            }
        }

        /*
         * Verifies user is authorized to deleting specified data node
         */
        private static void AuthorizeSelectData (ApplicationContext context, Node node, Node args)
        {
            // Checking if user is root (root is authorized to do almost everything!)
            if (context.Ticket.Role != "root") {

                // Making sure protected data is not deleted
                while (node.Path.Count != 2) {
                    node = node.Parent;
                }

                // Verifying main data node is not protected (starts with "_")
                if (node.Name.StartsWith ("_"))
                    throw new LambdaSecurityException (
                        string.Format ("You cannot select protected node '{0}' from your database, or parts of its descendants", node.Name), 
                        args["args"].Get<Node> (context), 
                        context);
            }
        }

        /*
         * Verifies user is authorized to deleting specified data node
         */
        private static void AuthorizeUpdateData (ApplicationContext context, Node node, Node args)
        {
            // Making sure it is impossible to update root nodes or file nodes from database, regardless of who you are!
            if (node.Path.Count < 2)
                throw new LambdaSecurityException (
                    "You cannot update file nodes, or root nodes from your database", 
                    args["args"].Get<Node> (context), 
                    context);

            // Checking if user is root (root is authorized to do almost everything!)
            if (context.Ticket.Role != "root") {

                // Making sure protected data is not deleted
                while (node.Path.Count != 2) {
                    node = node.Parent;
                }

                // Verifying main data node is not protected (starts with "_")
                if (node.Name.StartsWith ("_"))
                    throw new LambdaSecurityException (
                        string.Format ("You cannot update protected node '{0}' from your database, or parts of its descendants", node.Name), 
                        args["args"].Get<Node> (context), 
                        context);
            }
        }

                /*
         * Verifies user is authorized reading the specified file
         */
        private static void AuthorizeReadFile (ApplicationContext context, string filename, Node args)
        {
            // Checking if user is root (root is authorized to do everything!)
            if (context.Ticket.Role != "root") {

                // Making sure we remove root folder, if it is given as part of filename
                if (filename.IndexOf (GetRootFolder (context)) == 0)
                    filename = filename.Substring (GetRootFolder (context).Length);

                // Verifying file is not underneath another user's folder
                if (filename.IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) == -1 && filename.StartsWith ("users/"))
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

                // Verifying filename is not underneath a protected folder
                foreach (var idxFolder in filename.Split (new char [] {'/'}, StringSplitOptions.RemoveEmptyEntries)) {

                    // Verifying currently iterated folder is NOT protected (starts with "_")
                    if (idxFolder.StartsWith ("_"))
                        throw new LambdaSecurityException (
                            string.Format ("User '{0}' tried to read file '{1}' within protected folder '{2}'", context.Ticket.Username, filename, idxFolder), 
                            args["args"].Get<Node> (context), 
                            context);
                }
            }
        }

        /*
         * Verifies user is authorized writing to the specified file
         */
        private static void AuthorizeWriteFile (ApplicationContext context, string filename, Node args)
        {
            // Checking if user is root (root is authorized to do everything!)
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

                // Verifying filename is not underneath a protected folder
                foreach (var idxFolder in filename.Split (new char []{'/'}, StringSplitOptions.RemoveEmptyEntries)) {

                    // Verifying currently iterated folder is NOT protected (starts with "_")
                    if (idxFolder.StartsWith ("_"))
                        throw new LambdaSecurityException (
                            string.Format ("User '{0}' tried to read file '{1}' within protected folder '{2}'", context.Ticket.Username, filename, idxFolder), 
                            args["args"].Get<Node> (context), 
                            context);
                }

                // Verifying suffix of file is a type of file that user is allowed to save
                switch (GetFileSuffix (filename)) {

                // Creating blacklist ...!
                case "config":
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