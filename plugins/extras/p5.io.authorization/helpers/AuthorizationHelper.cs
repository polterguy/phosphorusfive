/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.io.authorization.helpers
{
    /*
     * Helper class for authorization features in p5.io
     */
    static class AuthorizationHelper
    {
        /*
         * Verifies user is authorized reading from the specified file.
         */
        internal static void AuthorizeReadFile (
            ApplicationContext context, 
            string filename, 
            Node stack)
        {
            // Verifies filename is valid.
            if (string.IsNullOrEmpty (filename) || !filename.StartsWithEx ("/") || filename.Contains ("\\") || filename.Contains ("//") || filename.Contains (".."))
                throw new LambdaException (
                    string.Format ("Path '{0}' was not a valid file path", filename), 
                    stack, 
                    context);

            // Making sure web.config file is safe!
            if (context.Ticket.Role != "root") {
                if (Path.GetFileName (filename.ToLower ()) == "web.config")
    				throw new LambdaException(
    					"Sorry, your web.config file is off limits!",
    					stack,
    					context);
            }

			// Verifying auth file is safe.
			if (filename.ToLower() == GetAuthFile(context).ToLower())
				throw new LambdaSecurityException(
					string.Format("User '{0}' tried to access auth file", context.Ticket.Username),
					stack,
					context);

			// Extra security for non-root users.
			if (context.Ticket.Role != "root") {

                // Checking if this is "common folder", at which point we return immediately,
                // since all users have access to this folder.
                if (filename.ToLower ().StartsWithEx ("/common/"))
                    return; // Legal!

                // Verifying file is underneath authenticated user's folder, if it is underneath "/users/" folder.
                if (filename.ToLower ().StartsWithEx ("/users/") && 
                    filename.ToLower ().IndexOfEx (string.Format ("/users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to read file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);

                // Verify all database files are safe.
                if (filename.ToLower ().StartsWithEx (context.RaiseEvent (
                    ".p5.config.get",
                    new Node (".p5.config.get", ".p5.data.path"))[0].Get (context, "/db/")))
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to read from database file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);
            }
        }

        /*
         * Verifies user is authorized modifying the specified file.
         */
        internal static void AuthorizeModifyFile (
            ApplicationContext context, 
            string filename, 
            Node stack)
        {
            // Verifies filename is valid.
            if (string.IsNullOrEmpty (filename) || !filename.StartsWithEx ("/") || filename.Contains ("\\") || filename.Contains ("//") || filename.Contains (".."))
                throw new LambdaException (
                    string.Format ("Path '{0}' was not a valid file path", filename), 
                    stack, 
                    context);

			// Making sure web.config file is safe!
            if (context.Ticket.Role != "root") {
    			if (Path.GetFileName (filename.ToLower ()) == "web.config")
    				throw new LambdaException (
    					"Sorry, your web.config file is off limits!",
    					stack,
    					context);
            }

			// Extra security for non-root users.
			if (context.Ticket.Role != "root") {

                // Verifying suffix of file is a type of file that user is allowed to save.
                switch (Path.GetExtension (filename).ToLower ()) {

                    // Blacklisted ...!
                    case ".config":
                        throw new LambdaSecurityException (
                            string.Format ("User '{0}' tried to write to file '{1}'", context.Ticket.Username, filename),
                            stack,
                            context);
                }

                // Checking if this is "common folder", at which point we return immediately,
                // since all users have access to this folder.
                if (filename.ToLower ().StartsWithEx ("/common/"))
                    return; // Legal

                // Verifying file is not underneath ANOTHER user's folder, which is not legal!
                if (filename.ToLower ().StartsWithEx ("/users/") && 
                    filename.ToLower ().IndexOfEx (string.Format ("/users/{0}/", context.Ticket.Username.ToLower ())) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("Root user '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);

                // Verifying "auth file" is safe.
                if (filename.ToLower () == GetAuthFile (context).ToLower ())
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to access 'auth' file", context.Ticket.Username), 
                        stack, 
                        context);

                // Verifies only root account can write to anything but "user files".
                if (!filename.ToLower ().StartsWithEx ("/users/"))
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to write to file '{1}'", context.Ticket.Username, filename), 
                        stack, 
                        context);
            }
        }

        /*
         * Verifies user is authorized reading from the specified folder.
         */
        internal static void AuthorizeReadFolder (
            ApplicationContext context, 
            string foldername, 
            Node stack)
        {
            // Verifies foldername is a valid foldername.
            if (string.IsNullOrEmpty (foldername) || !foldername.StartsWithEx ("/") || !foldername.EndsWithEx ("/") || foldername.Contains ("\\") || foldername.Contains ("//") || foldername.Contains (".."))
                throw new LambdaException (
                    string.Format ("Path '{0}' was not a valid folder path", foldername), 
                    stack, 
                    context);

            // Verifies nobody can read from database folder.
            if (foldername.ToLower ().StartsWithEx (context.RaiseEvent (
                ".p5.config.get",
                new Node (".p5.config.get", ".p5.data.path"))[0].Get (context, "/db/")))
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to read from database folder '{1}'", context.Ticket.Username, foldername), 
                    stack, 
                    context);

            // Extra security for non-root users.
            if (context.Ticket.Role != "root") {

                // Checking if this is "common folder", at which point we return immediately,
                // since all users have access to this folder.
                if (foldername.ToLower ().StartsWithEx ("/common/"))
                    return; // Legal

                // Verifies file is underneath authorized user's folder, if it is underneath "/users/" folders.
                if (foldername.ToLower ().StartsWithEx ("/users/") &&
                    foldername.ToLower ().IndexOfEx (string.Format ("/users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("User '{0}' tried to read from another user's folder; '{1}'", context.Ticket.Username, foldername), 
                        stack, 
                        context);
            }
        }

        /*
         * Verifies user is authorized writing to the specified folder
         */
        internal static void AuthorizeModifyFolder (
            ApplicationContext context, 
            string foldername, 
            Node stack)
        {
            // Verifies foldername is valid.
            if (string.IsNullOrEmpty (foldername) || !foldername.StartsWithEx ("/") || !foldername.EndsWithEx ("/") || foldername.Contains ("\\") || foldername.Contains ("//") || foldername.Contains (".."))
                throw new LambdaException (
                    string.Format ("Path '{0}' was not a valid folder path", foldername), 
                    stack, 
                    context);

            // Verifies nobody can write to database folder.
            if (foldername.ToLower ().StartsWithEx (context.RaiseEvent (
                ".p5.config.get",
                new Node (".p5.config.get", ".p5.data.path"))[0].Get (context, "/db/")))
                throw new LambdaSecurityException (
                    string.Format ("User '{0}' tried to write to database folder '{1}'", context.Ticket.Username, foldername), 
                    stack, 
                    context);

            // Checking if user is root (root is authorized to do almost everything!)
            if (context.Ticket.Role != "root") {

                // Checking if this is "common folder", at which point we return immediately,
                // since all users have access to this folder
                if (foldername.ToLower ().StartsWithEx ("/common/"))
                    return; // Legal

                // Verifying folder is not underneath ANOTHER user's folder, which is not legal even for root account!
                if (foldername.ToLower ().StartsWithEx ("/users/") && foldername.IndexOfEx (string.Format ("/users/{0}/", context.Ticket.Username)) != 0)
                    throw new LambdaSecurityException (
                        string.Format ("Root user '{0}' tried to write to folder '{1}'", context.Ticket.Username, foldername), 
                        stack, 
                        context);
            }
        }

        /*
         * Returns the filename of our "auth" file.
         */
        public static string GetAuthFile (ApplicationContext context)
        {
            return context.RaiseEvent (".p5.auth.get-auth-file").Get<string> (context);
        }
    }
}