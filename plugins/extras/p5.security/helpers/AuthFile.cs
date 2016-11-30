/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
using p5.core;

namespace p5.security.helpers
{
    /// <summary>
    ///     Class wrapping access to read and write operations to and from "auth" file
    /// </summary>
    internal static class AuthFile
    {
        // Used as delegate for modification of "auth" file
        internal delegate void ModifyAuthFileDelegate (Node authFile);

        // Used to lock access to password file
        private static object _passwordFileLocker = new object ();

        // Used to cache password file, for faster access
        private static Node _authFileContent = null;

        /*
         * Helper to retrieve "auth" file as lambda object
         */
        internal static Node GetAuthFile (ApplicationContext context)
        {
            // Making sure we lock file as we retrieve it
            lock (_passwordFileLocker) {

                // Returning auth file
                return GetAuthFileInternal (context);
            }
        }

        /*
         * Helper to modify auth file
         */
        internal static void ModifyAuthFile (ApplicationContext context, ModifyAuthFileDelegate functor)
        {
            // Making sure we lock file as we retrieve it and allows caller to modify it
            lock (_passwordFileLocker) {

                // Retrieving auth file
                Node authFileNode = GetAuthFileInternal (context);

                // Invoking callback functor
                functor (authFileNode);

                // Saves updated authFileNode
                SaveAuthFileInternal (context, authFileNode);
            }
        }
        
        /*
         * Creates a new "salt" for use with hashing of passwords
         */
        internal static string CreateNewSalt(ApplicationContext context)
        {
            return context.Raise ("create-cs-random").Get<string> (context);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Private implementation of retrieving auth file
         */
        private static Node GetAuthFileInternal (ApplicationContext context)
        {
            // Checking if we can return cached version
            if (_authFileContent != null)
                return _authFileContent;

            // Getting path
            string pwdFilePath = GetAuthFilePath (context);

            // Checking file exist
            if (!File.Exists (pwdFilePath))
                return new Node ("").Add ("users"); // First time retrieval of "auth" file

            // Reading up passwords file
            using (TextReader reader = new StreamReader (File.OpenRead (pwdFilePath))) {

                // Retrieving content of file
                var fileContent = reader.ReadToEnd ();
                _authFileContent = Utilities.Convert<Node> (context, fileContent);
                return _authFileContent;
            }
        }

        /*
         * Private implementation of saving auth file
         */
        private static void SaveAuthFileInternal (ApplicationContext context, Node authFileNode)
        {
            // Updating cached version
            _authFileContent = authFileNode.Clone ();

            // Getting path
            string pwdFilePath = GetAuthFilePath (context);

            // Saving file
            using (TextWriter writer = new StreamWriter (File.Create (pwdFilePath))) {
                writer.Write (Utilities.Convert<string> (context, authFileNode.Children));
            }
        }

        /*
         * Returns path to auth file
         */
        private static string GetAuthFilePath (ApplicationContext context)
        {
            // Getting filepath to pwd file
            string rootFolder = context.Raise (".p5.core.application-folder").Get<string> (context);

            // The logic below makes it possible to store auth file OUTSIDE of main web application folder!
            string pwdFilePath = rootFolder + context.Raise (".p5.security.get-auth-file").Get<string> (context);

            // Returning path to caller
            return pwdFilePath;
        }

        #endregion
    }
}