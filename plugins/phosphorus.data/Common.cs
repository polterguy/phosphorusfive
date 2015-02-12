
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.data
{
    /// <summary>
    /// helper class for common operations and methods needed in the [pf.data.xxx] namespace
    /// </summary>
    public static class Common
    {
        // contains the path to the database folder, where all the pf.lambda files are stored
        private static string _dbPath;

        // actual database content
        private static Node _database;

        // used for locking access to database operations to create thread safe solutions
        private static object _lock;

        static Common ()
        {
            _lock = new object ();
        }

        /// <summary>
        /// returns the node containing the actual data in the database
        /// </summary>
        /// <value>database tree</value>
        public static Node Database {
            get { return _database; }
        }

        /// <summary>
        /// used to lock database access
        /// </summary>
        /// <value>lock object</value>
        public static object Lock {
            get { return _lock; }
        }

        /// <summary>
        /// makes sure database is properly initialized
        /// </summary>
        /// <param name="context">application context</param>
        public static void Initialize (ApplicationContext context)
        {
            // verifying database is not already initialized from before
            if (string.IsNullOrEmpty (_dbPath)) {

                // need to initialize database
                _database = new Node ();

                // finding and setting our databasse root directory
                _dbPath = ConfigurationManager.AppSettings ["database-path"] ?? "database/";

                // checking to see if database directory exist
                Node dbPath = new Node (string.Empty, _dbPath);
                context.Raise ("pf.folder.exists", dbPath);
                if (!dbPath [0].Get<bool> (context)) {
                    context.Raise ("pf.folder.create", dbPath);
                }

                // iterating through all folders inside of database directory and loading all files in all folders inside of database directory
                foreach (string idxDirectory in GetDirectories (context, _dbPath)) {
                    foreach (string idxFile in GetFiles (context, idxDirectory)) {
                        _database.Add (LoadFile (context, idxFile));
                    }
                }
            }
        }

        /*
         * appends node to list of changes, if it doesn't already exist there
         */
        public static void AddNodeToChanges (Node idxDest, List<Node> changed)
        {
            // finding "file node"
            Node dnaFile = idxDest;
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
        public static void SaveAffectedFiles (ApplicationContext context, List<Node> changed)
        {
            foreach (Node idxNode in changed) {
                SaveFileNode (context, idxNode);
                if (idxNode.Count == 0)
                    idxNode.UnTie ();
            }
        }
        
        /*
         * saves a database node to disc
         */
        public static void SaveFileNode (ApplicationContext context, Node fileNode)
        {
            if (fileNode.Count == 0) {

                // removing file entirely since it no longer has any data
                context.Raise ("pf.file.remove", new Node (string.Empty, fileNode.Value));

                // checking to see if we should remove folder entirely
                Node folderNode = new Node (string.Empty, fileNode.Get<string> (context).Substring (0, fileNode.Get<string> (context).LastIndexOf ("/")));
                context.Raise ("pf.file.list-files", folderNode);
                if (folderNode.Count == 0) {
                    context.Raise ("pf.folder.remove", folderNode);
                }
            } else {

                // converts node to code
                Node convertNode = new Node ();
                foreach (Node idx in fileNode.Children) {
                    convertNode.Add (idx.Clone ());
                }
                context.Raise ("pf.hyperlisp.lambda2code", convertNode);

                // saves code
                Node saveNode = new Node (string.Empty, fileNode.Value);
                saveNode.Add (new Node ("source", convertNode.Value));
                context.Raise ("pf.file.save", saveNode);
            }
        }

        /*
         * returns the next available database file node to store nodes within
         */
        public static Node GetAvailableFileNode (ApplicationContext context)
        {
            // searching through database to see if there are any nodes we can use from before
            int objectsPerFile = int.Parse(ConfigurationManager.AppSettings ["database-nodes-per-file"] ?? "32");
            foreach (Node idxFileNode in _database.Children)
            {
                if (idxFileNode.Count < objectsPerFile)
                    return idxFileNode;
            }

            // creating new node and appending into database
            string newFileName = FindAvailableNewFileName (context);
            Node newNode = new Node (string.Empty, newFileName);
            _database.Add (newNode);

            // making sure fil exists on disc, for future new creations of files before save operation occurs
            Node createFile = new Node (string.Empty, newFileName);
            createFile.Add (new Node ("source", ""));
            context.Raise ("pf.file.save", createFile);

            // returning available file node back to caller
            return newNode;
        }

        /*
         * loads a file from "path" and returns as Node
         */
        private static Node LoadFile (ApplicationContext context, string path)
        {
            // loading file
            Node loadFileNode = new Node (string.Empty, path);
            context.Raise ("pf.file.load", loadFileNode);

            // converting file to Node
            Node convertNode = new Node (string.Empty, loadFileNode [0].Value);
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", convertNode);

            // returning Node with node value being path, and content being children
            Node retVal = new Node (string.Empty, path);
            retVal.AddRange (convertNode.Children);
            return retVal;
        }

        /*
         * returns all directories within db path
         */
        private static IEnumerable<string> GetDirectories (ApplicationContext context, string directory)
        {
            Node dbFoldersNode = new Node (string.Empty, directory);
            context.Raise ("pf.folder.list-folders", dbFoldersNode);

            dbFoldersNode.Sort (
                delegate (Node left, Node right)
                {
                    int leftInt = int.Parse (left.Get<string> (context).Replace (_dbPath, "").Substring (2));
                    int rightInt = int.Parse (right.Get<string> (context).Replace (_dbPath, "").Substring (2));
                    return leftInt.CompareTo (rightInt);
            });

            foreach (Node idxDirectory in dbFoldersNode.Children)
                yield return idxDirectory.Value as string;
        }
        
        /*
         * returns files within directory
         */
        private static IEnumerable<string> GetFiles (ApplicationContext context, string directory)
        {
            Node dbFoldersNode = new Node (string.Empty, directory);
            context.Raise ("pf.folder.list-files", dbFoldersNode);

            dbFoldersNode.Sort (
                delegate (Node left, Node right)
                {
                    int leftInt = int.Parse (left.Get<string> (context).Replace (directory, "").Substring (3).Replace (".hl", ""));
                    int rightInt = int.Parse (right.Get<string> (context).Replace (directory, "").Substring (3).Replace (".hl", ""));
                    return leftInt.CompareTo (rightInt);
            });

            foreach (Node idxFile in dbFoldersNode.Children)
                yield return idxFile.Value as string;
        }

        /*
         * returns the next available filename for a new database file
         */
        private static string FindAvailableNewFileName (ApplicationContext context)
        {
            int maxFilesPerDirectory = int.Parse (ConfigurationManager.AppSettings ["database-files-per-directory"] ?? "256");

            // checking to see if we can use existing directory
            List<string> directoryList = new List<string> (GetDirectories (context, _dbPath));
            foreach (string idxDirectory in directoryList)
            {
                List<string> filesList = new List<string> (GetFiles (context, idxDirectory));
                if (filesList.Count >= maxFilesPerDirectory)
                    continue;
                for (int idxNo = 0; idxNo < filesList.Count; idxNo++)
                {
                    if (!filesList.Exists (
                        delegate (string file)
                        {
                        return file == idxDirectory + "/db" + idxNo + ".hl";
                    }))
                        return idxDirectory + "/db" + idxNo + ".hl";
                }
                return idxDirectory + "/db" + filesList.Count + ".hl";
            }

            // didn't find an available filename, without creating new directory
            for (int idxNo = 0; idxNo < directoryList.Count; idxNo++)
            {
                if (!directoryList.Exists (
                    delegate (string dirNode)
                    {
                    return dirNode == _dbPath + "db" + idxNo;
                }))
                {
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
            Node createDirectoryNode = new Node (string.Empty, directory);
            context.Raise ("pf.folder.create", createDirectoryNode);
        }
    }
}
