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
                        string.Format ("Access denied to '{0}'", filename),
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
                        string.Format ("Access denied to '{0}'", filename),
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
                        string.Format ("Access denied to '{0}'", foldername),
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
                if (!UserHasWriteAccessToFolder (context, foldername.ToLowerInvariant ()))
                    throw new LambdaException (
                        string.Format ("Access denied to '{0}'", foldername),
                        stack,
                        context);
            }
        }
        
        /*
         * Verifies non-root user has access to reading file.
         * The default is "true".
         */
        internal static bool UserHasReadAccessToFile (ApplicationContext context, string path)
        {
            // Checking access rights first.
            bool explicitAccess;
            var accessDeclaration = CheckAccessRights (context, path, "read", true, out explicitAccess);
            if (explicitAccess) {

                // Explicitly denied or allowed.
                return accessDeclaration;
            }

            /*
             * We found no explicitly created access objects for the path, hence we resort to the default access rights in our system.
             */

            // Checking if this is an attempt at accessing a user's private/public files.
            if (path.StartsWithEx ("/users/") && !path.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username))) {

                // Checking if user tries to read from a publicly available file from another user.
                var entities = path.Split (new char [] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (entities.Length >= 4 && entities [2] == "documents" && entities [3] == "public") {

                    // Publicly shared file from another user.
                    return true;
                }

                // Private file.
                return false;
            }

            // Verify all database files are safe.
            var dbPath = context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", ".p5.data.path")) [0].Get (context, "/db/");
            if (path.StartsWithEx (dbPath))
                return false;

            // Verify web.config and app.config is safe.
            if (path == "/web.config")
                return false;
            if (path == "/app.config")
                return false;

            // Verifying "auth" file is safe.
            if (path == GetAuthFile (context).ToLower ())
                return false;

            // Defaulting access to true.
            return true;
        }
        
        /*
         * Verifies non-root user has access to reading folder.
         */
        internal static bool UserHasReadAccessToFolder (ApplicationContext context, string path)
        {
            // Checking access rights first.
            bool explicitAccess;
            var accessDeclaration = CheckAccessRights (context, path, "read", true, out explicitAccess);
            if (explicitAccess) {

                // Explicitly denied or allowed.
                return accessDeclaration;
            }

            /*
             * We found no explicitly created access objects for the path, hence we resort to the default access rights in our system.
             */

            // Shielding other user's folders here.
            if (path.StartsWithEx ("/users/") && path != "/users/" && !path.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username))) {

                // Checking if user tries to read from a publicly available file from another user.
                var entities = path.Split (new char [] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (entities.Length == 2 || (entities.Length == 3 && entities [2] == "documents") || (entities.Length >= 4 && entities [2] == "documents" && entities [3] == "public")) {

                    // Publicly shared folder from another user.
                    return true;
                }

                // Private file.
                return false;
            }

            // Verify all database folders are safe.
            var dbPath = context.RaiseEvent (".p5.config.get", new Node (".p5.config.get", ".p5.data.path")) [0].Get (context, "/db/");
            if (path.StartsWithEx (dbPath))
                return false;

            // Defaulting access to true.
            return true;
        }
        
        /*
         * Verifies non-root user has access to writing file.
         * The default is "false"
         */
        internal static bool UserHasWriteAccessToFile (ApplicationContext context, string path)
        {
            // Checking access rights first.
            bool explicitAccess;
            var accessDeclaration = CheckAccessRights (context, path, "write", true, out explicitAccess);
            if (explicitAccess) {

                // Explicitly denied or allowed.
                return accessDeclaration;
            }

            /*
             * We found no explicitly created access objects for the path, hence we resort to the default access rights in our system.
             */

            // Checking if this is user's file.
            if (path.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username)))
                return true;

            // Checking if this is a common folder.
            if (path.StartsWithEx ("/common/"))
                return true;

            // Defaulting to false.
            return false;
        }

        /*
         * Verifies non-root user has access to modify folder.
         */
        internal static bool UserHasWriteAccessToFolder (ApplicationContext context, string path)
        {
            // Checking access rights first.
            bool explicitAccess;
            var accessDeclaration = CheckAccessRights (context, path, "write", true, out explicitAccess);
            if (explicitAccess) {

                // Explicitly denied or allowed.
                return accessDeclaration;
            }

            /*
             * We found no explicitly created access objects for the path, hence we resort to the default access rights in our system.
             */

            // Checking if this is user's file.
            if (path.StartsWithEx ("/users/") && path.StartsWithEx (string.Format ("/users/{0}/", context.Ticket.Username)))
                return true;

            // Checking if this is a common folder.
            if (path.StartsWithEx ("/common/"))
                return true;

            // Defaulting to false.
            return false;
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
        private static bool CheckAccessRights (
            ApplicationContext context, 
            string path, 
            string operation, 
            bool defaultValue, 
            out bool explicitAccess)
        {
            // Invoking event responsible for determining if user has access to path and returning results to caller.
            var args = new Node ("", defaultValue);
            args.Add ("path", path);
            args.Add ("filter", "p5.io." + operation + "-file");

            // Returns access to caller.
            var access = context.RaiseEvent ("p5.auth.has-access", args).Get<bool> (context);
            explicitAccess = args.GetChildValue ("explicit", context, false);
            return access;
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