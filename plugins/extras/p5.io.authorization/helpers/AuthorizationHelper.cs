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
         * Verifies non-root user has access to reading file.
         * The default is "true".
         */
        private static bool UserHasReadAccessToFile (ApplicationContext context, string filename)
        {
            // Verifying file is underneath authenticated user's folder, if it is underneath "/users/" folder.
            if (filename.StartsWithEx ("/users/") && !filename.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username)))
                return false;

            // Verify all database files are safe.
            var dbPath = context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", ".p5.data.path")) [0].Get (context, "/db/");
            if (filename.StartsWithEx (dbPath))
                return false;
            
            // Verify *.config is safe.
            if (Path.GetExtension (filename) == ".config")
                return false;
            
            // Verifying "auth" file is safe.
            if (filename == GetAuthFile (context).ToLower ())
                return false;
                
            // Checking explicitly overridden deny rights.
            var access = new Node ();
            context.RaiseEvent ("p5.auth.access.list", access);
            foreach (var idxAccess in access.Children.ToList ()) {
                
                // Checking currently iterated access right object.
                if ((idxAccess.Name == "*" || idxAccess.Name == context.Ticket.Role) && idxAccess ["deny-folder"] != null) {

                    // This is an access right relevant for the user, now checking if it's relevant for the current path.
                    if (filename.StartsWithEx (idxAccess ["deny-folder"].Get<string> (context))) {
                        return false;
                    }
                }
            }

            // Success.
            return true;
        }
        
        /*
         * Verifies non-root user has access to writing file.
         * The default is "false"
         */
        private static bool UserHasWriteAccessToFile (ApplicationContext context, string filename)
        {
            // Checking if this is user's file.
            if (filename.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username)))
                return true;

            // Checking if this is a common folder.
            if (filename.StartsWithEx ("/common/"))
                return true;
            
            // Checking explicitly overridden write access rights.
            var access = new Node ();
            context.RaiseEvent ("p5.auth.access.list", access);
            foreach (var idxAccess in access.Children.ToList ()) {

                // Checking currently iterated access right object.
                if ((idxAccess.Name == "*" || idxAccess.Name == context.Ticket.Role) && idxAccess ["write-folder"] != null) {

                    // This is an access right relevant for the user, now checking if it's relevant for the current path.
                    if (filename.StartsWithEx (idxAccess ["write-folder"].Get<string> (context))) {
                        
                        // Checking explicitly overridden deny rights.
                        access = new Node ();
                        context.RaiseEvent ("p5.auth.access.list", access);
                        foreach (var idxAccessInner in access.Children.ToList ()) {

                            // Checking currently iterated access right object.
                            if ((idxAccessInner.Name == "*" || idxAccessInner.Name == context.Ticket.Role) && idxAccessInner ["deny-folder"] != null) {

                                // This is an access right relevant for the user, now checking if it's relevant for the current path.
                                if (filename.StartsWithEx (idxAccessInner ["deny-folder"].Get<string> (context))) {
                                    return false;
                                }
                            }
                        }
                        return true;
                    }
                }
            }

            // Failure.
            return false;
        }

        /*
         * Verifies non-root user has access to reading folder.
         */
        private static bool UserHasReadAccessToFolder (ApplicationContext context, string foldername)
        {
            // Verifying file is underneath authenticated user's folder, if it is underneath "/users/" folder.
            if (foldername.StartsWithEx ("/users/") && 
                foldername.Length > "/users/".Length && 
                !foldername.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username)))
                return false;

            // Verify all database files are safe.
            var dbPath = context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", ".p5.data.path")) [0].Get (context, "/db/");
            if (foldername.StartsWithEx (dbPath))
                return false;
            
            // Checking explicitly overridden read access rights.
            var access = new Node ();
            context.RaiseEvent ("p5.auth.access.list", access);
            foreach (var idxAccess in access.Children.ToList ()) {

                // Checking currently iterated access right object.
                if ((idxAccess.Name == "*" || idxAccess.Name == context.Ticket.Role) && idxAccess ["deny-folder"] != null) {

                    // This is an access right relevant for the user, now checking if it's relevant for the current path.
                    if (foldername.StartsWithEx (idxAccess ["deny-folder"].Get<string> (context))) {
                        return false;
                    }
                }
            }

            // Success.
            return true;
        }

        /*
         * Verifies non-root user has access to modify folder.
         */
        private static bool UserHasWriteAccessToFolder (ApplicationContext context, string foldername)
        {
            // Checking if this is user's file.
            if (foldername.StartsWithEx ("/users/") && foldername.IndexOfEx (string.Format ("/users/{0}/", context.Ticket.Role)) == 0)
                return true;
            
            // Checking if this is a common folder.
            if (foldername.StartsWithEx ("/common/"))
                return true;
            
            // Checking explicitly overridden write access rights.
            var access = new Node ();
            context.RaiseEvent ("p5.auth.access.list", access);
            foreach (var idxAccess in access.Children.ToList ()) {

                // Checking currently iterated access right object.
                if ((idxAccess.Name == "*" || idxAccess.Name == context.Ticket.Role) && idxAccess ["write-folder"] != null) {

                    // This is an access right relevant for the user, now checking if it's relevant for the current path.
                    if (foldername.StartsWithEx (idxAccess ["write-folder"].Get<string> (context))) {
                        
                        // Checking explicitly overridden deny rights.
                        access = new Node ();
                        context.RaiseEvent ("p5.auth.access.list", access);
                        foreach (var idxAccessInner in access.Children.ToList ()) {

                            // Checking currently iterated access right object.
                            if ((idxAccessInner.Name == "*" || idxAccessInner.Name == context.Ticket.Role) && idxAccessInner ["deny-folder"] != null) {

                                // This is an access right relevant for the user, now checking if it's relevant for the current path.
                                if (foldername.StartsWithEx (idxAccessInner ["deny-folder"].Get<string> (context))) {
                                    return false;
                                }
                            }
                        }
                        return true;
                    }
                }
            }

            // Failure.
            return false;
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