/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using pf.core;

/// <summary>
///     Main namespace for common data classes.
/// 
///     Contains common helper classes for the [pf.data.xxx] namespace in Phosphorus.Five.
/// </summary>
namespace phosphorus.data.helpers
{
    /// <summary>
    ///     Helper class for common operations.
    /// 
    ///     Contains helper methods for common operations shared among for instance the [pf.data.select] and [pf.data.remove]
    ///     Active Events.
    /// </summary>
    public static class Common
    {
        // contains the path to the database folder, where all the pf.lambda files are stored
        private static string _dbPath;

        static Common ()
        {
            Lock = new object ();
        }

        /// <summary>
        ///     This is your actual Database.
        /// 
        ///     The [pf.data.xxx] namespace in Phosphorus.Five, is a memory-based Database, allowing you to have a quick and dirty
        ///     database implementation, not usable for huge data centers, but smaller websites, in addition to serving as a cache
        ///     object for you. This is where you actual Database data is stored.
        /// </summary>
        /// <value>Database node tree.</value>
        public static Node Database { get; private set; }

        /// <summary>
        ///     Used to lock database access.
        /// 
        ///     Some operations in the database layer of Phosphorus.Five, requires that only one thread access the database 
        ///     at the same time. This is the object we use to lock such access to the database.
        /// </summary>
        /// <value>lock object</value>
        public static object Lock { get; private set; }

        /// <summary>
        ///     Makes sure database is properly initialized.
        /// 
        ///     Basically loads all files from the database path, which can be found in your application configuration file, and
        ///     loads up all items from all pf.lambda files inside your databasse directory, and stores them in memory.
        /// 
        ///     After initial execution, this method will return immediately, not being particularly costy in any ways.
        /// </summary>
        /// <param name="context">application context</param>
        public static void Initialize (ApplicationContext context)
        {
            // verifying database is not already initialized from before
            if (string.IsNullOrEmpty (_dbPath)) {
                // need to initialize database
                Database = new Node ();

                // finding and setting our databasse root directory
                _dbPath = ConfigurationManager.AppSettings ["database-path"] ?? "database/";

                // checking to see if database directory exist
                var dbPath = new Node (string.Empty, _dbPath);
                context.Raise ("pf.folder.exists", dbPath);
                if (!dbPath [0].Get<bool> (context)) {
                    context.Raise ("pf.folder.create", dbPath);
                }

                // iterating through all folders inside of database directory and loading all files in all folders inside of database directory
                foreach (var idxDirectory in GetDirectories (context, _dbPath)) {
                    foreach (var idxFile in GetFiles (context, idxDirectory)) {
                        Database.Add (LoadFile (context, idxFile));
                    }
                }
            }
        }

        /// <summary>
        ///     Adds the file node to changes.
        /// 
        ///     When you update or remove items from the database, then this method is useful for figuring
        ///     out which files are affected by your operation.
        /// 
        ///     Pass in the node that was either changed or removed, and the method will append the file node necessary to
        ///     save as a consequence in the changed parameter.
        /// </summary>
        /// <param name="idxDest">Which node was changed.</param>
        /// <param name="changed">List of nodes containing the files that needs to be saved due to change operation.</param>
        public static void AddNodeToChanges (Node idxDest, List<Node> changed)
        {
            // finding "file node"
            var dnaFile = idxDest;
            while (dnaFile.Path.Count > 1) {
                dnaFile = dnaFile.Parent;
            }

            // making sure changed list of items contains "file node"
            if (!changed.Contains (dnaFile)) {
                changed.Add (dnaFile);
            }
        }

        /*
         * saves all affected files
         */
        /// <summary>
        ///     Saves the affected files.
        /// 
        ///     Will save all files in the changed parameter that needs to be save as a consequence of a change or
        ///     remove operation into the database.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="changed">List of files that were changed, and hence will be saved.</param>
        public static void SaveAffectedFiles (ApplicationContext context, List<Node> changed)
        {
            foreach (var idxNode in changed) {
                SaveFileNode (context, idxNode);
                if (idxNode.Count == 0)
                    idxNode.UnTie ();
            }
        }

        /// \todo Do we really need to save the file? It is not really supposed to be necessary...
        /// <summary>
        ///     Gets the available file node.
        /// 
        ///     Will automatically figure out the next available file node for you, and create a container file
        ///     which will be used for the node.
        /// </summary>
        /// <returns>The next available file node.</returns>
        /// <param name="context">Aplication context.</param>
        public static Node GetAvailableFileNode (ApplicationContext context)
        {
            // searching through database to see if there are any nodes we can use from before
            var objectsPerFile = int.Parse (ConfigurationManager.AppSettings ["database-nodes-per-file"] ?? "32");
            foreach (var idxFileNode in Database.Children) {
                if (idxFileNode.Count < objectsPerFile)
                    return idxFileNode;
            }

            // creating new node and appending into database
            var newFileName = FindAvailableNewFileName (context);
            var newNode = new Node (string.Empty, newFileName);
            Database.Add (newNode);

            // making sure fil exists on disc, for future new creations of files before save operation occurs
            var createFile = new Node (string.Empty, newFileName);
            createFile.Add (new Node ("source", ""));
            context.Raise ("pf.file.save", createFile);

            // returning available file node back to caller
            return newNode;
        }

        /*
         * saves a database node to disc
         */
        private static void SaveFileNode (ApplicationContext context, Node fileNode)
        {
            if (fileNode.Count == 0) {
                // removing file entirely since it no longer has any data
                context.Raise ("pf.file.remove", new Node (string.Empty, fileNode.Value));

                // checking to see if we should remove folder entirely
                var folderNode = new Node (
                    string.Empty,
                    fileNode.Get<string> (context)
                        .Substring (0, fileNode.Get<string> (context).LastIndexOf ("/", StringComparison.Ordinal)));
                context.Raise ("pf.file.list-files", folderNode);
                if (folderNode.Count == 0) {
                    context.Raise ("pf.folder.remove", folderNode);
                }
            } else {
                // converts node to code
                var convertNode = new Node ();
                foreach (var idx in fileNode.Children) {
                    convertNode.Add (idx.Clone ());
                }
                context.Raise ("pf.hyperlisp.lambda2hyperlisp", convertNode);

                // saves code
                var saveNode = new Node (string.Empty, fileNode.Value);
                saveNode.Add (new Node ("source", convertNode.Value));
                context.Raise ("pf.file.save", saveNode);
            }
        }

        /*
         * loads a file from "path" and returns as Node
         */
        private static Node LoadFile (ApplicationContext context, string path)
        {
            // loading file
            var loadFileNode = new Node (string.Empty, path);
            context.Raise ("pf.file.load", loadFileNode);

            // converting file to Node
            var convertNode = new Node (string.Empty, loadFileNode [0].Value);
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", convertNode);

            // returning Node with node value being path, and content being children
            var retVal = new Node (string.Empty, path);
            retVal.AddRange (convertNode.Children);
            return retVal;
        }

        /*
         * returns all directories within given folder
         */
        private static IEnumerable<string> GetDirectories (ApplicationContext context, string folder)
        {
            var dbFoldersNode = new Node (string.Empty, folder);
            context.Raise ("pf.folder.list-folders", dbFoldersNode);

            dbFoldersNode.Sort (
                delegate (Node left, Node right) {
                    var leftInt = int.Parse (left.Get<string> (context).Replace (_dbPath, "").Substring (2));
                    var rightInt = int.Parse (right.Get<string> (context).Replace (_dbPath, "").Substring (2));
                    return leftInt.CompareTo (rightInt);
                });

            return dbFoldersNode.Children.Select (idxDirectory => (string) idxDirectory.Value);
        }

        /*
         * returns files within directory
         */
        private static IEnumerable<string> GetFiles (ApplicationContext context, string directory)
        {
            var dbFoldersNode = new Node (string.Empty, directory);
            context.Raise ("pf.folder.list-files", dbFoldersNode);

            dbFoldersNode.Sort (
                delegate (Node left, Node right) {
                    var leftInt = int.Parse (left.Get<string> (context).Replace (directory, "").Substring (3).Replace (".hl", ""));
                    var rightInt = int.Parse (right.Get<string> (context).Replace (directory, "").Substring (3).Replace (".hl", ""));
                    return leftInt.CompareTo (rightInt);
                });

            return dbFoldersNode.Children.Select (idxFile => idxFile.Value as string);
        }

        /*
         * returns the next available filename for a new database file
         */
        private static string FindAvailableNewFileName (ApplicationContext context)
        {
            var maxFilesPerDirectory = int.Parse (ConfigurationManager.AppSettings ["database-files-per-directory"] ?? "256");

            // checking to see if we can use existing directory
            var directoryList = new List<string> (GetDirectories (context, _dbPath));
            foreach (var idxDirectory in directoryList) {
                var filesList = new List<string> (GetFiles (context, idxDirectory));
                if (filesList.Count >= maxFilesPerDirectory)
                    continue;
                for (var idxNo = 0; idxNo < filesList.Count; idxNo++) {
                    if (!filesList.Exists (
                        file => file == idxDirectory + "/db" + idxNo + ".hl"))
                        return idxDirectory + "/db" + idxNo + ".hl";
                }
                return idxDirectory + "/db" + filesList.Count + ".hl";
            }

            // didn't find an available filename, without creating new directory
            for (var idxNo = 0; idxNo < directoryList.Count; idxNo++) {
                if (!directoryList.Exists (
                    dirNode => dirNode == _dbPath + "db" + idxNo)) {
                    CreateNewDirectory (context, _dbPath + "db" + idxNo);
                    return _dbPath + "db" + idxNo + "/db0.hl";
                }
            }

            CreateNewDirectory (context, _dbPath + "db" + directoryList.Count);
            return _dbPath + "db" + directoryList.Count + "/db0.hl";
        }

        /*
         * helper to create directory
         */
        private static void CreateNewDirectory (ApplicationContext context, string directory)
        {
            var createDirectoryNode = new Node (string.Empty, directory);
            context.Raise ("pf.folder.create", createDirectoryNode);
        }
    }
}
