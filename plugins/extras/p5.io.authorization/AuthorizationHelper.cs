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
    /*
     * Helper class for authorization features in p5.io
     */
    internal static class AuthorizationHelper
    {
        /*
         * Verifies user is authorized writing to the specified file
         */
        internal static void AuthorizeSaveFile (ApplicationContext context, string filename, Node stack)
        {
            // Checking if user is root (root is authorized to do almost everything!)
            if (context.Ticket.Role != "root") {

                // Verifying file is underneath user's folder
                if (filename.IndexOf (string.Format ("/users/{0}/", context.Ticket.Username)) != 0)
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
                if (filename.ToLower ().StartsWith ("/users/") && 
                      filename.ToLower ().IndexOf (string.Format ("/users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("Root user '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);
            }

            // Verifying "auth file" is safe
            if (filename.ToLower () == Common.NormalizeFileName (Common.GetAuthFile (context)).ToLower ())
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to access 'auth' file", context.Ticket.Username, filename), 
                    stack, 
                    context);
        }

        /*
         * Verifies user is authorized reading from the specified file
         */
        internal static void AuthorizeLoadFile (ApplicationContext context, string filename, Node stack)
        {
            // Verifying file is underneath authenticated user's folder, if it is underneath "users/" folders
            if (filename.ToLower ().StartsWith ("users/") && 
                filename.ToLower ().IndexOf (string.Format ("users/{0}/", context.Ticket.Username)) != 0)
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to read file '{1}'", context.Ticket.Username, filename), 
                    stack, 
                    context);

            // Verifying auth file is safe
            if (filename.ToLower () == Common.NormalizeFileName (Common.GetAuthFile (context)).ToLower ())
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to access auth file", context.Ticket.Username, filename), 
                    stack, 
                    context);
        }

        /*
         * Verifies user is authorized writing to the specified folder
         */
        internal static void AuthorizeSaveFolder (ApplicationContext context, string foldername, Node stack)
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
        internal static void AuthorizeLoadFolder (ApplicationContext context, string foldername, Node stack)
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