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

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using p5.exp;
using p5.core;

namespace p5.data.helpers
{
    /// <summary>
    ///     Helper class for common operations.
    /// </summary>
    public static class Common
    {
        // Contains full path to database folder, number of files per folder, and number of objects per file.
        static string _dbFullPath;
        static int _maxFiles;
        static int _maxObjects;

        // Content of database.
        public static Node Database { get; private set; }

		// Used to synchronize access to database.
		internal sealed class Lock : IDisposable
        {
            readonly bool _write;
            readonly static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim ();
			public Lock (bool write = false)
            {
                _write = write;
                if (_write)
                    _lock.EnterWriteLock ();
                else
                    _lock.EnterReadLock ();
			}

            public void Dispose ()
            {
                if (_write)
                    _lock.ExitWriteLock ();
                else
                    _lock.ExitReadLock ();
            }
        }

        /// <summary>
        ///     Initializes the database
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">E</param>
        [ActiveEvent (Name = ".p5.core.application-start")]
        static void _p5_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // Acquiring lock on database, before we initialize it.
            using (new Lock (true)) {
                Initialize (context);
            }
        }

		/*
         * Make sure database is properly initialized
         */
        static void Initialize (ApplicationContext context)
        {
            // Verifying database is not already initialized from before.
            if (Database == null) {

                // Need to initialize database.
                Database = new Node ();

                // Finding and setting our database root directory.
                var dbPath = context.RaiseEvent (
                    ".p5.config.get",
                    new Node (".p5.config.get", ".p5.data.path")) [0].Get (context, "/db/");
                _dbFullPath = GetRootFolder (context) + dbPath;

				// Figuring out other settings.
                _maxObjects = context.RaiseEvent (
					".p5.config.get",
					new Node (".p5.config.get", ".p5.data.nodes-per-file")) [0].Get (context, 32);
                _maxFiles = context.RaiseEvent (
					".p5.config.get",
					new Node (".p5.config.get", ".p5.data.files-per-folder")) [0].Get (context, 256);


				// Checking to see if database directory exist.
				if (!Directory.Exists (_dbFullPath)) {

                    // Database folder did not exist, therefor we need to create it.
                    Directory.CreateDirectory (_dbFullPath);

                } else {

                    // Iterating through all folders inside of database directory.
                    foreach (var idxDirectory in Directory.GetDirectories (_dbFullPath)) {

                        // Iterating through each file in currently iterated folder.
                        foreach (var idxFile in Directory.GetFiles (idxDirectory)) {

                            // Loading currently iterated file.
                            Database.Add (LoadFile (context, idxFile));
                        }
                    }
                }
            }
        }

		/*
         * Loads a file from "path" and returns as Node.
         */
		static Node LoadFile (ApplicationContext context, string path)
		{
			// Reading file from disc.
			using (TextReader reader = File.OpenText (path)) {

				// Converting file to lambda.
				Node retVal = context.RaiseEvent ("hyper2lambda", new Node ("", reader.ReadToEnd ()));
                retVal.Value = "/" + path.Substring (_dbFullPath.Length).Trim ('/');
				return retVal;
			}
		}

		/*
         * Adds the file node to changes, unless it already has been added.
         */
		public static void AddNodeToChanges (Node idxDest, List<Node> changed)
        {
            // Finding file node.
            var dnaFile = idxDest;
            while (dnaFile.OffsetToRoot > 1) {
                dnaFile = dnaFile.Parent;
            }

            // Adding file node to changes, unless it already exists from before.
            if (!changed.Contains (dnaFile)) {
                changed.Add (dnaFile);
            }
        }

		/*
         * Saves the affected files.
         */
        public static void SaveAffectedFiles (ApplicationContext context, List<Node> changed)
        {
            // Looping through all files that needs to be saved.
            foreach (var idxNode in changed) {

                // Making sure file is stored to disc (or deleted, if it is empty).
                SaveFileNode (context, idxNode);
            }
        }

		/*
         * Gets the first available file node.
         */
        public static Node GetAvailableFileNode (ApplicationContext context, bool forceAppend)
        {
            // Searching through database to see if there are any nodes we can use from before.
            if (forceAppend) {

                // Only returning last node, if it has room.
                if (Database.Count > 0) {
                    if (Database [Database.Count - 1].Count < _maxObjects)
                        return Database [Database.Count - 1];
                }
            } else {

                // Returning first available node, if any.
                foreach (var idxFileNode in Database.Children) {

                    // Checking if currently iterated file has room for more data.
                    if (idxFileNode.Count < _maxObjects)
                        return idxFileNode; // We found an available node.
                }
            }

            // Creating new node and appending into database.
            var newFileName = FindAvailableNewFileName (context);
            var newNode = new Node ("", newFileName);
            Database.Add (newNode);

            // Returning available file node back to caller.
            return newNode;
        }

        /*
         * Saves a database node to disc, or deletes it if it is empty
         */
		static void SaveFileNode (ApplicationContext context, Node fileNode)
        {
            // Checking to see if we should remove file entirely, due to it having no more content.
            if (fileNode.Count == 0) {

                // Removing file since it no longer has any data.
                if (File.Exists (_dbFullPath.TrimEnd ('/') + fileNode.Value))
                    File.Delete (_dbFullPath.TrimEnd ('/') + fileNode.Value);
                fileNode.UnTie ();

                // Checking to see if we should remove folder entirely.
                string folder = fileNode.Get<string> (context);
                folder = folder.Substring (0, folder.LastIndexOfEx ("/") + 1);
                if (!GetFolders (context).Any (ix => ix == folder)) {

                    // Deleting folder, since there are no more files in it, but first making sure it exists.
                    if (Directory.Exists (_dbFullPath.TrimEnd ('/') + folder))
                        Directory.Delete (_dbFullPath.TrimEnd ('/') + folder);
                }
            } else {

                // Making sure folder exists.
                var filename = fileNode.Get<string> (context);
                var folder = filename.Substring (0, filename.LastIndexOfEx ("/") + 1);
                if (!Directory.Exists (_dbFullPath.TrimEnd ('/') + folder))
                    Directory.CreateDirectory (_dbFullPath.TrimEnd ('/') + folder);

                // Saves node as Hyperlambda.
                using (TextWriter writer = File.CreateText (_dbFullPath.TrimEnd ('/') + filename)) {
                    writer.Write (context.RaiseEvent ("lambda2hyper", new Node ().AddRange (fileNode.Clone ().Children)).Value);
                }
            }
        }

        /*
         * Returns all distinct sub-folders within database folder, according to content of database.
         */
        static IEnumerable<string> GetFolders (ApplicationContext context)
        {
            var retVal = new List<string> ();
            foreach (var idx in Database.Children) {

                // Retrieving filename of currently iterated node.
                var idxFilename = idx.Get<string> (context);

                // Removing the filename parts of our path.
                var idxFolder = idxFilename.Substring (0, idxFilename.LastIndexOfEx ("/") + 1);

                // Checking if folder has been added in a previous iteration, and if not, making sure we add it.
                if (!retVal.Any (ix => ix == idxFolder))
                    retVal.Add (idxFolder);
            }
            return retVal;
        }

        /*
         * Returns files within directory.
         */
        static IEnumerable<string> GetFiles (ApplicationContext context, string folder)
        {
            var retVal = new List<string> ();
            foreach (var idxNode in Database.Children) {

                // Retrieving filename of currently iterated node.
                var idxFilename = idxNode.Get<string> (context);

                // Making sure currently iterated file is inside of specified folder, and if so, adding it.
                if (idxFilename.StartsWithEx (folder))
                    retVal.Add (idxFilename);
            }
            return retVal;
        }

        /*
         * Returns the next available filename for a new database file.
         */
        static string FindAvailableNewFileName (ApplicationContext context)
        {
            // Retrieving all folders currently in use.
            var folders = GetFolders (context).ToList ();

            // Looping through each existing directory.
            foreach (var idxFolder in folders) {

                // Retrieving all files in currently iterated folder.
                var files = GetFiles (context, idxFolder).ToList ();

                // Checking if this folder has room for another file.
                if (files.Count >= _maxFiles)
                    continue; // No more room here ...

                // Finding first available filename in current folder.
                for (var idxNo = 0; idxNo < _maxFiles; idxNo++) {

                    // Checking if currently iterated filename exist.
                    if (!files.Exists (file => file == idxFolder + "db" + idxNo + ".hl"))

                        // Filename did not exist, returning as next available filename.
                        return idxFolder + "db" + idxNo + ".hl";
                }
            }

            // Didn't find an available filename, without creating new folder, looping through each folder.
            for (var idxNo = 0; idxNo < folders.Count; idxNo++) {

                // Checking if currently iterated folder exist.
                if (!folders.Exists (ix => ix == "/db" + idxNo + "/")) {

                    // Folder did not exist, create it, and returning to caller.
                    return "/db" + idxNo + "/db0.hl";
                }
            }

            // We'll need to create a new folder to have room for our next available file node.
            return "/db" + folders.Count + "/db0.hl";
        }

        /*
         * Helper to retrieve root folder of application
         */
        static string GetRootFolder (ApplicationContext context)
        {
            return context.RaiseEvent (".p5.core.application-folder").Get<string> (context);
        }
    }
}
