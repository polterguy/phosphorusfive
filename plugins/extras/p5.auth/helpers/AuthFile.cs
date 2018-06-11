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
using System.Threading;
using p5.core;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping access to read and write operations to and from "auth" file.
    /// </summary>
    static class AuthFile
    {
        // Used as delegate for modification of "auth" file.
        internal delegate void ModifyAuthFileDelegate (Node authFile);

        // Used to lock access to password file.
        static ReaderWriterLockSlim _locker = new ReaderWriterLockSlim ();

        // Used to cache password file in memory, for faster access.
        static Node _authFileContent;

        /*
         * Helper to retrieve "auth" file as lambda object.
         */
        internal static Node GetAuthFile (ApplicationContext context)
        {
            // Making sure we lock file as we retrieve it.
            _locker.EnterReadLock ();
            try {

                // Returning auth file.
                return GetAuthFileInternal (context);

            } finally {
                _locker.ExitReadLock ();
            }
        }

        /*
         * Helper to modify auth file.
         */
        internal static void ModifyAuthFile (ApplicationContext context, ModifyAuthFileDelegate functor)
        {
            // Making sure we lock file as we retrieve it and allows caller to modify it.
            _locker.EnterWriteLock ();
            try {

                // In case this is our first attempt at tampering with auth file, we create a default "empty" file.
                if (_authFileContent == null)
                    _authFileContent = new Node ("").Add ("users");

                // Invoking callback functor.
                functor (_authFileContent);

                // Saves updated authFileNode.
                SaveAuthFileInternal (context);

            } finally {
                _locker.ExitWriteLock ();
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Private implementation of retrieving auth file.
         */
        static Node GetAuthFileInternal (ApplicationContext context)
        {
            // Checking if we can return cached version.
            if (_authFileContent != null)
                return _authFileContent;

            // Getting path.
            string pwdFilePath = GetAuthFilePath (context);

            // Checking if file exist.
            if (!File.Exists (pwdFilePath)) {
                _authFileContent = new Node ("").Add ("users"); // First time retrieval of "auth" file.
                return _authFileContent;
            }

            // Reads auth file and decrypts it.
            using (TextReader reader = new StreamReader (File.OpenRead (pwdFilePath))) {

                // Retrieving fingerprint for PGP key to use to decrypt file.
                var fingerprintLine = reader.ReadLine ();
                var fingerprint = fingerprintLine.Split (':') [1];

                // Retrieving GnuPG key's password from web.config.
                var confNode = new Node ("", "gpg-server-keypair-password");
                var gnuPgPassword = context.RaiseEvent ("p5.config.get", confNode).FirstChild.Get<string> (context);
                
                // Retrieving the rest of the content of file.
                var fileContent = reader.ReadToEnd ();

                // Decrypting file's content with PGP key referenced at the top of the file.
                var node = new Node ("", fileContent);
                node.Add ("decrypt").LastChild
                    .Add ("fingerprint", fingerprint).LastChild
                    .Add ("password", gnuPgPassword);
                context.RaiseEvent ("p5.mime.parse", node);

                // Converting Hyperlambda content of file to a node, caching it, and returning it to caller.
                // Making sure we explicitly add the [gnupg-keypair] to the "auth" node first.
                _authFileContent = Utilities.Convert<Node> (context, node.FirstChild ["text"] ["content"].Value);
                _authFileContent.Add (PGPKey.GnuPgpFingerprintNodeName, fingerprint);
                return _authFileContent;
            }
        }

        /*
         * Implementation of saving auth file.
         */
        static void SaveAuthFileInternal (ApplicationContext context)
        {
            // Getting path.
            string pwdFilePath = GetAuthFilePath (context);

            // Saving file, making sure we encrypt it in the process.
            using (TextWriter writer = new StreamWriter (File.Create (pwdFilePath))) {

                // Retrieving fingerprint from auth file, and removing the fingerprint node, since
                // it's not supposed to be save inside of the ecnrypted MIME part of our auth file's content.
                var fingerprint = _authFileContent [PGPKey.GnuPgpFingerprintNodeName]?.UnTie ().Get<string> (context) ?? "";
                if (string.IsNullOrEmpty (fingerprint))
                    return; // Fingerprint has not (yet) been set, hence we don't save file.

                try {

                    // Writing fingerprint of PGP key used to encrypt auth file at the top of the file.
                    writer.WriteLine (string.Format ("gnupg-keypair:{0}", fingerprint));

                    // Encrypting auth file's content.
                    var node = new Node ();
                    node.Add ("text", "plain").LastChild
                        .Add ("content", Utilities.Convert<string> (context, _authFileContent.Children))
                        .Add ("encrypt").LastChild
                        .Add ("fingerprint", fingerprint);
                    context.RaiseEvent ("p5.mime.create", node);

                    // Writing encrypted content to stream.
                    writer.Write (node ["result"].Get<string> (context));

                } finally {

                    // Adding back fingerprint to cached auth file.
                    _authFileContent.Insert (0, new Node (PGPKey.GnuPgpFingerprintNodeName, fingerprint));
                }
            }
        }

        /*
         * Returns path to auth file
         */
        static string GetAuthFilePath (ApplicationContext context)
        {
            // Getting filepath to pwd file.
            string rootFolder = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);

            // The logic below makes it possible to store auth file OUTSIDE of main web application folder!
            string pwdFilePath = rootFolder + context.RaiseEvent (".p5.auth.get-auth-file").Get<string> (context);

            // Returning path to caller
            return pwdFilePath;
        }

        #endregion
    }
}