/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using p5.core;

/// <summary>
///     Main namespace for common data classes
/// </summary>
namespace p5.data.helpers
{
    /// <summary>
    ///     Helper class for common operations
    /// </summary>
    public static class Common
    {
        /// <summary>
        ///     Initializes the database
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">E</param>
        [ActiveEvent (Name = "p5.core.application-start", Protection = EventProtection.LambdaClosed)]
        private static void p5_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // Acquiring lock on database
            lock (Common.Lock) {

                // Making sure database is initialized
                Common.Initialize (context);
            }
        }

        // Contains full path to database folder
        private static string _dbFullPath;

        static Common ()
        {
            Lock = new object ();
        }

        /// <summary>
        ///     This is your actual Database
        /// </summary>
        /// <value>Database node lambda object</value>
        public static Node Database { get; private set; }

        /// <summary>
        ///     Used to lock database access
        /// </summary>
        /// <value>lock object</value>
        public static object Lock { get; private set; }

        /// <summary>
        ///     Make sure database is properly initialized
        /// </summary>
        /// <param name="context">application context</param>
        private static void Initialize (ApplicationContext context)
        {
            // Verifying database is not already initialized from before
            if (Database == null) {

                // Need to initialize database
                Database = new Node ();

                // Finding and setting our database root directory
                _dbFullPath = (ConfigurationManager.AppSettings ["database-path"] ?? "~/_database/");
                _dbFullPath = _dbFullPath.Replace ("~/", GetRootFolder (context));

                // Checking to see if database directory exist
                if (!Directory.Exists (_dbFullPath)) {

                    // Database folder did NOT exist, therefor we need to create
                    Directory.CreateDirectory (_dbFullPath);
                }

                // Iterating through all folders inside of database directory, and loading all files in 
                // all folders inside of database directory
                foreach (var idxDirectory in GetFolders (context)) {

                    // Loading all files in currently iterated folder
                    foreach (var idxFile in GetFiles (context, idxDirectory)) {

                        // Loading currently iterated file
                        Database.Add (LoadFile (context, idxFile));
                    }
                }
            }
        }

        /// <summary>
        ///     Adds the file node to changes
        /// </summary>
        /// <param name="idxDest">Which node was changed</param>
        /// <param name="changed">List of nodes containing the files that needs to be saved due to change operation</param>
        public static void AddNodeToChanges (Node idxDest, List<Node> changed)
        {
            // Finding "file node"
            var dnaFile = idxDest;
            while (dnaFile.OffsetToRoot > 1) {
                dnaFile = dnaFile.Parent;
            }

            // Making sure changed list of items contains "file node"
            if (!changed.Contains (dnaFile)) {
                changed.Add (dnaFile);
            }
        }

        /// <summary>
        ///     Saves the affected files
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="changed">List of files that were changed, and hence will be saved</param>
        public static void SaveAffectedFiles (ApplicationContext context, List<Node> changed)
        {
            // Looping through all files that needs to be saved
            foreach (var idxNode in changed) {

                // Making sure file is stored to disc (or deleted, if it is empty)
                SaveFileNode (context, idxNode);

                // Removing node entirely from database, if file node was empty, at which case the file was deleted above
                if (idxNode.Count == 0)
                    idxNode.UnTie ();
            }
        }

        /// <summary>
        ///     Gets the first available file node
        /// </summary>
        /// <returns>The next available file node</returns>
        /// <param name="context">Aplication context</param>
        public static Node GetAvailableFileNode (ApplicationContext context)
        {
            // Searching through database to see if there are any nodes we can use from before
            var objectsPerFile = int.Parse (ConfigurationManager.AppSettings ["database-nodes-per-file"] ?? "32");
            foreach (var idxFileNode in Database.Children) {

                // Checking if currently iterated file has room for more data
                if (idxFileNode.Count < objectsPerFile)
                    return idxFileNode; // We found an available node
            }

            // Creating new node and appending into database
            var newFileName = FindAvailableNewFileName (context);
            var newNode = new Node ("", newFileName);
            Database.Add (newNode);

            // Making sure file exists on disc, for future new creations of files, before save operation occurs
            using (File.CreateText (_dbFullPath + newFileName)) { }

            // Returning available file node back to caller
            return newNode;
        }

        /*
         * Saves a database node to disc, or deletes it if it is empty
         */
        private static void SaveFileNode (ApplicationContext context, Node fileNode)
        {
            // Checking to see if we should remove file entirely, due to it having no more content
            if (fileNode.Count == 0) {

                // Removing file since it no longer has any data
                File.Delete (_dbFullPath + fileNode.Value);

                // Checking to see if we should remove folder entirely
                string folder = fileNode.Get<string> (context).Substring (0, fileNode.Get<string> (context).LastIndexOf ("/") + 1);
                var fileList = Directory.GetFiles (_dbFullPath + folder).ToList ();
                fileList.RemoveAll (ix => !Path.GetFileName (ix).StartsWith ("db") && !Path.GetFileName (ix).EndsWith (".hl"));
                if (fileList.Count == 0) {

                    // Deleting folder, since there are no more files in it
                    Directory.Delete (_dbFullPath + folder);
                }
            } else {

                // Saves node as Hyperlisp
                using (TextWriter writer = File.CreateText (_dbFullPath + fileNode.Value)) {
                    writer.Write (context.RaiseNative ("lambda2lisp", new Node ().AddRange (fileNode.Clone ().Children)).Value);
                }
            }
        }

        /*
         * Loads a file from "path" and returns as Node
         */
        private static Node LoadFile (ApplicationContext context, string path)
        {
            // Reading file from disc
            using (TextReader reader = File.OpenText (_dbFullPath + path)) {

                // Converting file to lambda
                Node retVal = context.RaiseNative ("lisp2lambda", new Node ("", reader.ReadToEnd ()));
                retVal.Value = path;
                return retVal;
            }
        }

        /*
         * Returns all directories within database folder
         */
        private static IEnumerable<string> GetFolders (ApplicationContext context)
        {
            // Looping through each subfolder in folder given
            var folders = Directory.GetDirectories (_dbFullPath).ToList ();
            folders.Sort (
                (x, y) => 
                int.Parse (x.Substring (_dbFullPath.Length + 2)).CompareTo (int.Parse (y.Substring (_dbFullPath.Length + 2))));
            foreach (var idxDirectory in folders) {

                // Returning currently iterated subfolder
                yield return "/" + idxDirectory.Substring (_dbFullPath.Length).Trim ('/') + "/";
            }
        }

        /*
         * Returns files within directory
         */
        private static IEnumerable<string> GetFiles (ApplicationContext context, string folder)
        {
            // Looping through each file in folder given
            var files = Directory.GetFiles (_dbFullPath + folder.Substring (1)).ToList ();
            files.RemoveAll (ix => !Path.GetFileName (ix).StartsWith ("db") || Path.GetFileName (ix).EndsWith ("~"));
            files.Sort ((x, y) => 
                int.Parse (Path.GetFileName (x).Substring (2).Replace (".hl", "")).CompareTo (int.Parse (Path.GetFileName (y).Substring (2).Replace (".hl", ""))));
            foreach (var idxFile in files) {

                // Returning currently iterated file
                yield return idxFile.Substring (_dbFullPath.Length);
            }
        }

        /*
         * Returns the next available filename for a new database file
         */
        private static string FindAvailableNewFileName (ApplicationContext context)
        {
            // Retrieving maximum number of files for folder
            var maxFilesPerDirectory = int.Parse (ConfigurationManager.AppSettings ["database-files-per-directory"] ?? "256");

            // Retrieving all folders currently in use
            var directoryList = GetFolders (context).ToList ();

            // Looping through each existing directory
            foreach (var idxDirectory in directoryList) {

                // Retrieving all files in currently iterated folder
                var filesList = GetFiles (context, idxDirectory).ToList ();

                // Checking if this folder has room for another file
                if (filesList.Count >= maxFilesPerDirectory)
                    continue; // No more room here ...

                // Finding first available filename in current folder
                for (var idxNo = 0; idxNo < filesList.Count; idxNo++) {

                    // Checking if currently iterated filename exist
                    if (!filesList.Exists (file => file == idxDirectory.Substring (1) + "db" + idxNo + ".hl"))

                        // Filename did not exist, returning as next available filename
                        return idxDirectory.Substring (1) + "db" + idxNo + ".hl";
                }

                // No missing files in folder, returning next available filename
                return idxDirectory.TrimStart('/') + "db" + filesList.Count + ".hl";
            }

            // Didn't find an available filename, without creating new directory, looping through each folder
            for (var idxNo = 0; idxNo < directoryList.Count; idxNo++) {

                // Checking if currently iterated folder exist
                if (!directoryList.Exists (dirNode => dirNode == "/db" + idxNo + "/")) {

                    // Folder did not exist, create it, and returning to caller
                    CreateNewDirectory (context, "/db" + idxNo + "/");
                    return "db" + idxNo + "/db0.hl";
                }
            }

            // Creating next available folder, and returning to caller
            CreateNewDirectory (context, "/db" + directoryList.Count + "/");
            return "db" + directoryList.Count + "/db0.hl";
        }

        /*
         * helper to create directory
         */
        private static void CreateNewDirectory (ApplicationContext context, string directory)
        {
            Directory.CreateDirectory (_dbFullPath + directory.Substring (1));
        }

        /*
         * Helper to retrieve root folder of application
         */
        private static string GetRootFolder (ApplicationContext context)
        {
            return context.RaiseNative ("p5.core.application-folder").Get<string> (context);
        }
    }
}
