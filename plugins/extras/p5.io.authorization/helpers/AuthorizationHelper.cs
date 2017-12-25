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
using System.Linq;
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
        internal static void AuthorizeReadFile (ApplicationContext context, string filename, Node stack)
        {
            // Verifies filename is valid.
            if (!VerifySaneFileName (filename))
                throw new LambdaException (
                    string.Format ("Path '{0}' was not a valid file path", filename),
                    stack,
                    context);
            
            // Checking role of user, and invoking relevant check.
            if (context.Ticket.Role != "root") {
                
                // Making sure we do a lowers comparison.
                if (!UserHasReadAccessToFile (context, filename.ToLowerInvariant ()))
                    throw new LambdaException (
                        string.Format ("File '{0}' is off limits", filename),
                        stack,
                        context);
            }
        }

        /*
         * Verifies user is authorized modifying the specified file.
         */
        internal static void AuthorizeModifyFile (ApplicationContext context, string filename, Node stack)
        {
            // Verifies filename is valid.
            if (!VerifySaneFileName (filename))
                throw new LambdaException (
                    string.Format ("Path '{0}' was not a valid file path", filename),
                    stack,
                    context);

            // Checking role of user, and invoking relevant check.
            if (context.Ticket.Role != "root") {

                // Making sure we do a lowers comparison.
                if (!UserHasWriteAccessToFile (context, filename.ToLowerInvariant ()))
                    throw new LambdaException (
                        string.Format ("File '{0}' is off limits", filename),
                        stack,
                        context);
            }
        }
        
        /*
         * Verifies user is authorized reading from the specified folder.
         */
        internal static void AuthorizeReadFolder (ApplicationContext context, string foldername, Node stack)
        {
            // Verifies foldername is a valid foldername.
            if (!VerifySaneFolderName (foldername))
                throw new LambdaException (
                    string.Format ("Path '{0}' was not a valid folder path", foldername),
                    stack,
                    context);

            // Checking role of user, and invoking relevant check.
            if (context.Ticket.Role != "root") {

                // Making sure we do a lowers comparison.
                if (!UserHasReadAccessToFolder (context, foldername.ToLowerInvariant ()))
                    throw new LambdaException (
                        string.Format ("Folder '{0}' is off limits", foldername),
                        stack,
                        context);
            }

        }

        /*
         * Verifies user is authorized writing to the specified folder
         */
        internal static void AuthorizeModifyFolder (ApplicationContext context, string foldername, Node stack)
        {
            // Verifies foldername is valid.
            if (!VerifySaneFolderName (foldername))
                throw new LambdaException (
                    string.Format ("Folder '{0}' was not a valid folder path", foldername),
                    stack,
                    context);

            // Checking role of user, and invoking relevant check.
            if (context.Ticket.Role != "root") {

                // Making sure we do a lowers comparison.
                if (!UserHasReadAccessToFolder (context, foldername.ToLowerInvariant ()))
                    throw new LambdaException (
                        string.Format ("Folder '{0}' is off limits", foldername),
                        stack,
                        context);
            }
        }

        /*
         * Private helper methods below here.
         */

        /*
         * Verifies filename is sane.
         */
        private static bool VerifySaneFileName (string filename)
        {
            // Basic sanity check of given filename.
            if (string.IsNullOrEmpty (filename))
                return false;
            if (filename [0] != '/')
                return false;
            if (filename.Contains ("//"))
                return false;
            if (filename.Contains ("\\"))
                return false;
            if (filename.Contains (".."))
                return false;

            // Success.
            return true;
        }

        /*
         * Verifies foldername is sane.
         */
        private static bool VerifySaneFolderName (string foldername)
        {
            // A sane foldername has the same rules as a sane filename, except it must end with a "/".
            if (!VerifySaneFileName (foldername))
                return false;
            if (foldername [foldername.Length - 1] != '/')
                return false;

            // Success.
            return true;
        }
        
        /*
         * Helper used both for files and folders to check access rights.
         */
        private static bool CheckReadAccessRights (ApplicationContext context, string path)
        {
            // Checking explicitly overridden deny rights.
            var access = new Node ();
            context.RaiseEvent ("p5.auth.access.list", access);
            foreach (var idxAccess in access.Children) {

                // Checking currently iterated access right object.
                if ((idxAccess.Name == "*" || idxAccess.Name == context.Ticket.Role) && idxAccess ["deny-folder"] != null) {

                    // This is an access right relevant for the current user's role, now checking if it's relevant for the current path.
                    if (path.StartsWithEx (idxAccess ["deny-folder"].Get<string> (context))) {

                        // Checking if this is the generic deny, and there exists an explicit override for the current role.
                        if (idxAccess.Name == "*") {

                            // Above deny was generic, checking if explicit allow exists, in one form or another.
                            foreach (var idxAccessInner in access.Children) {

                                // Checking if currently iterated access right object belongs to the current user's role.
                                if ((idxAccessInner.Name == context.Ticket.Role)) {

                                    // This is an overridden access right relevant for the user, now checking if it's relevant for the current path.
                                    // Notice, a path will never start with "x".
                                    // Hence, if this is not neither an [allow-folder], nor a [write-folder], it won't be a match.
                                    if (path.StartsWithEx (idxAccessInner.GetChildValue ("allow-folder", context, "x")) ||
                                        path.StartsWithEx (idxAccessInner.GetChildValue ("write-folder", context, "x"))) {

                                        // Explicit override exists.
                                        return true;
                                    }
                                }
                            }
                        }
                        
                        // User is denied read access to file/folder.
                        return false;
                    }
                }
            }

            // Defaulting to true.
            return true;
        }
        
        /*
         * Helper used both for files and folders to check access rights.
         */
        private static bool CheckWriteAccessRights (ApplicationContext context, string path)
        {
            // Checking explicitly overridden write rights.
            var access = new Node ();
            context.RaiseEvent ("p5.auth.access.list", access);
            foreach (var idxAccess in access.Children) {

                // Checking currently iterated access right object.
                if ((idxAccess.Name == "*" || idxAccess.Name == context.Ticket.Role) && idxAccess ["write-folder"] != null) {

                    // This is an access right relevant for the current user's role, now checking if it's relevant for the current path.
                    if (path.StartsWithEx (idxAccess ["write-folder"].Get<string> (context))) {

                        // Checking if this is the generic write, and there exists an explicit override for the current role.
                        if (idxAccess.Name == "*") {

                            // Above write was generic, checking if explicit deny exists.
                            foreach (var idxAccessInner in access.Children) {

                                // Checking if currently iterated access right object belongs to the current user's role.
                                if ((idxAccessInner.Name == context.Ticket.Role)) {

                                    // This is an overridden access right relevant for the user, now checking if it's relevant for the current path.
                                    // Notice, a path will never start with "x".
                                    // Hence, if this is not a [deny-folder], it won't be a match.
                                    if (path.StartsWithEx (idxAccessInner.GetChildValue ("deny-folder", context, "x"))) {

                                        // Explicit override deny exists.
                                        return false;
                                    }
                                }
                            }
                        }
                        
                        // User is allowed write access to file/folder.
                        return true;
                    }
                }
            }

            // Defaulting to true.
            return false;
        }
        
        /*
         * Verifies non-root user has access to reading file.
         * The default is "true".
         */
        private static bool UserHasReadAccessToFile (ApplicationContext context, string path)
        {
            // Verifying file is underneath authenticated user's folder, if it is underneath "/users/" folder.
            if (path.StartsWithEx ("/users/") && !path.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username)))
                return false;

            // Verify all database files are safe.
            var dbPath = context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", ".p5.data.path")) [0].Get (context, "/db/");
            if (path.StartsWithEx (dbPath))
                return false;
            
            // Verify *.config is safe.
            if (Path.GetExtension (path) == ".config")
                return false;
            
            // Verifying "auth" file is safe.
            if (path == GetAuthFile (context).ToLower ())
                return false;
                
            // Returning value of access rights check.
            return CheckReadAccessRights (context, path);
        }
        
        /*
         * Verifies non-root user has access to reading folder.
         */
        private static bool UserHasReadAccessToFolder (ApplicationContext context, string path)
        {
            // Shielding other user's folders here.
            if (path.StartsWithEx ("/users/") && 
                path.Length > "/users/".Length && 
                !path.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username)))
                return false;

            // Verify all database folders are safe.
            var dbPath = context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", ".p5.data.path")) [0].Get (context, "/db/");
            if (path.StartsWithEx (dbPath))
                return false;
            
            // Returning value of access rights check.
            return CheckReadAccessRights (context, path);
        }
        
        /*
         * Verifies non-root user has access to writing file.
         * The default is "false"
         */
        private static bool UserHasWriteAccessToFile (ApplicationContext context, string path)
        {
            // Checking if this is user's file.
            if (path.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username)))
                return true;

            // Checking if this is a common folder.
            if (path.StartsWithEx ("/common/"))
                return true;
            
            // Returning value of access rights check.
            return CheckWriteAccessRights (context, path);
        }

        /*
         * Verifies non-root user has access to modify folder.
         */
        private static bool UserHasWriteAccessToFolder (ApplicationContext context, string path)
        {
            // Checking if this is user's file.
            if (path.StartsWithEx ("/users/") && path.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Role)))
                return true;
            
            // Checking if this is a common folder.
            if (path.StartsWithEx ("/common/"))
                return true;
            
            // Returning value of access rights check.
            return CheckWriteAccessRights (context, path);
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