/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.data
{
    /// <summary>
    /// class wrapping the internal memory-based pf.lambda expression database
    /// </summary>
    public static class database
    {
        // contains the path to the database folder, where all the pf.lambda files are stored
        private static string _dbPath;

        // actual database content
        private static Node _database;

        /// <summary>
        /// loads items from the database matching the expression from the [pf.data.select] node's value,
        /// and returns matches as children of the [pf.data.select] node. [pf.data.select] can select
        /// either node, value, name, count or path expressions. if you use anything but a 'node'
        /// expression, the literal you select will be appended as th value of the nodes beneath [pf.data.select]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.select")]
        private static void pf_data_select (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure database is initialized
            Initialize (context);

            // returning all matches as children nodes of [pf.data.load]
            var expression = Expression.Create (e.Args.Get<string> ());
            var match = expression.Evaluate (_database);
            if (match.TypeOfMatch == Match.MatchType.Count) {

                // returning count of expression
                e.Args.Add (new Node (string.Empty, match.Count));
            }
            else if (match.TypeOfMatch != Match.MatchType.Node) {

                // returning 'value', 'name' or 'path' of expression as children values of [pf.data.select] node
                for (int idxNo = 0; idxNo < match.Count; idxNo++) {
                    e.Args.Add (new Node (string.Empty, match.GetValue (idxNo)));
                }
            } else {

                // appending all matches as children nodes of [pf.data.select]
                foreach (Node idx in match.Matches) {
                    e.Args.Add (idx.Clone ());
                }
            }
        }

        /// <summary>
        /// updates your database according to the expression given as value to either
        /// the node given as the first child, or expression given as first child's value, if first child
        /// name is empty and first child's value is an expression, or the value of the first child node,
        /// if name is empty, and value is not an expression.  [pf.data.update] works similar to [set] in
        /// pf.lambda
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.update")]
        private static void pf_data_update (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure database is initialized
            Initialize (context);

            // finding source to update destination with, which might be a collection of nodes, or an expression
            object sourceValue = GetUpdateSourceValue (e.Args);

            // updating all nodes from database matching expression given as value of [pf.data.update]
            var match = Expression.Create (e.Args.Get<string> ()).Evaluate (_database);

            // looping through database matches and updating nodes while storing which files have been changed
            List<Node> changed = new List<Node> ();
            foreach (Node idxDest in match.Matches) {

                // verifying user is not updating actual file nodes in database, which is a logical error
                if (idxDest.Path.Count < 2)
                    throw new ArgumentException ("you cannot update actual file nodes in database with [pf.data.update]");
                
                // figuring out which file Node updated belongs to, and storing in changed list
                AddNodeToChanges (idxDest, changed);

                // updating value in database
                UpdateMatchDestination (idxDest, match, sourceValue);
            }

            // saving all affected files
            SaveAffectedFiles (context, changed);
        }

        /// <summary>
        /// inserts new nodes to database
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.insert")]
        private static void pf_data_insert (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure database is initialized
            Initialize (context);

            // verifying syntax of statement
            if (e.Args.Count == 0 && !Expression.IsExpression (e.Args.Value))
                throw new ArgumentException ("[pf.data.insert] requires at least one child node as its argument, or a source expression as its value");

            // looping through all nodes given as children and saving them to database
            List<Node> changed = new List<Node> ();
            foreach (Node idx in GetInsertSource (e.Args)) {

                // finding next available database file node
                Node fileNode = GetAvailableFileNode (context);
                
                // figuring out which file Node updated belongs to, and storing in changed list
                if (!changed.Contains (fileNode))
                    changed.Add (fileNode);

                // actually appending node into database
                fileNode.Add (idx.Clone ());
            }
            
            // saving all affected files
            SaveAffectedFiles (context, changed);
        }

        /// <summary>
        /// removes the nodes from the database matching the given expression and returns number of items affected
        /// as value of [pf.data.remove] node's child
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.delete")]
        private static void pf_data_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure database is initialized
            Initialize (context);
            
            // verifying syntax of statement
            if (e.Args.Count != 0)
                throw new ArgumentException ("[pf.data.delete] does not take any arguments");

            // finding all nodes to remove
            var match = Expression.Create (e.Args.Get<string> ()).Evaluate (_database);

            // looping through database matches and removing nodes while storing which files have been changed
            List<Node> changed = new List<Node> ();
            foreach (Node idxDest in match.Matches) {

                // figuring out which file Node updated belongs to, and storing in changed list
                AddNodeToChanges (idxDest, changed);

                // replacing node in database
                idxDest.Untie ();
            }

            // saving all affected files
            foreach (Node idxNode in changed) {
                SaveFileNode (context, idxNode);

                // checking to see if database node is empty, and if so, removing it from database nodes
                if (idxNode.Count == 0)
                    idxNode.Untie ();
            }
        }

        /*
         * returns the source for which node(s) to insert into the database
         */
        private static IEnumerable<Node> GetInsertSource (Node node)
        {
            if (Expression.IsExpression (node.Value)) {
                var match = Expression.Create (node.Get<string> ()).Evaluate (node);
                if (match.TypeOfMatch != Match.MatchType.Node)
                    throw new ArgumentException ("[pf.data.insert] can only take 'node' expressions as source expressions");
                return match.Matches;
            } else {
                return node.Children;
            }
        }

        /*
         * updates the current node according to what type of match and source object we're dealing with
         */
        private static void UpdateMatchDestination (Node idxDest, Match match, object sourceValue)
        {
            switch (match.TypeOfMatch) {
            case Match.MatchType.Name:
                if (sourceValue is Node)
                    throw new ArgumentException ("cannot update name to become a node");
                idxDest.Name = (sourceValue ?? "").ToString ();
                break;
            case Match.MatchType.Node:
                if (!(sourceValue is Node))
                    throw new ArgumentException ("you can only update node to become another node");
                idxDest.Replace ((sourceValue as Node).Clone ());
                break;
            case Match.MatchType.Value:
                idxDest.Value = sourceValue;
                break;
            default:
                throw new ArgumentException ("you cannot update 'path' or 'count' with [pf.data.update] since these are read-only values");
            }
        }

        /*
         * appends node to list of changes, if it doesn't already exist there
         */
        private static void AddNodeToChanges (Node idxDest, List<Node> changed)
        {
            Node dnaFile = idxDest;
            while (dnaFile.Path.Count > 1)
                dnaFile = dnaFile.Parent;
            if (!changed.Contains (dnaFile))
                changed.Add (dnaFile);
        }

        /*
         * returns the source node for an update operation
         */
        private static object GetUpdateSourceValue (Node node)
        {
            // verifying syntax of statement
            if (node.Count != 1)
                throw new ArgumentException ("[pf.data.update] takes one and only one argument");

            // finding source, destination is expression in value of e.args, by default source is first child of e.Args,
            // but source can also be an expression, pointing to a position in the execution tree
            Node sourceNode = node [0];
            if (sourceNode.Name == string.Empty && !Expression.IsExpression (sourceNode.Value)) {

                // checking to see if it's a formatting expression
                if (sourceNode.Count > 0)
                    return Expression.FormatNode (sourceNode);
                return sourceNode.Value;
            } else if (sourceNode.Name == string.Empty) {

                // assigning the result of an expression here
                Match sourceMatch = Expression.Create (Expression.FormatNode (sourceNode)).Evaluate (sourceNode);

                // returning match according to type
                if (sourceMatch.TypeOfMatch == Match.MatchType.Count) {

                    // source was a count expression
                    return sourceMatch.Count;
                } else if (sourceMatch.Count == 1) {

                    // destination is an expression with only one result
                    return sourceMatch.GetValue (0);
                } else {
                    throw new ArgumentException ("[pf.data.update] needs a source expression yielding 1 node match as its result, unless it's a 'count' expression");
                }
            }
            return sourceNode;
        }
        
        /*
         * saves all affected files
         */
        private static void SaveAffectedFiles (ApplicationContext context, List<Node> changed)
        {
            foreach (Node idxNode in changed) {
                SaveFileNode (context, idxNode);
            }
        }

        /*
         * ensures the database is initialized
         */
        private static void Initialize (ApplicationContext context)
        {
            // verifying database is not already initialized from before
            if (string.IsNullOrEmpty (_dbPath)) {

                // need to initialize database
                _database = new Node ();

                // finding and setting our databasse root directory
                _dbPath = ConfigurationManager.AppSettings ["database-path"] ?? "database/";

                // checking to see if database directory exist
                Node dbPath = new Node (string.Empty, _dbPath);
                context.Raise ("pf.file.folder-exists", dbPath);
                if (!dbPath [0].Get<bool> ()) {
                    context.Raise ("pf.file.create-folder", dbPath);
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
         * loads a file from "path" and returns as Node
         */
        private static Node LoadFile (ApplicationContext context, string path)
        {
            // loading file
            Node loadFileNode = new Node (string.Empty, path);
            context.Raise ("pf.file.load", loadFileNode);

            // converting file to Node
            Node convertNode = new Node (string.Empty, loadFileNode [0].Value);
            context.Raise ("pf.code-2-nodes", convertNode);

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
            context.Raise ("pf.file.list-folders", dbFoldersNode);

            dbFoldersNode.Sort (
                delegate (Node left, Node right)
                {
                    int leftInt = int.Parse (left.Name.Replace (_dbPath, "").Substring (2));
                    int rightInt = int.Parse (right.Name.Replace (_dbPath, "").Substring (2));
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
            context.Raise ("pf.file.list-files", dbFoldersNode);

            dbFoldersNode.Sort (
                delegate (Node left, Node right)
                {
                    int leftInt = int.Parse (left.Name.Replace (directory, "").Substring (3).Replace (".hl", ""));
                    int rightInt = int.Parse (right.Name.Replace (directory, "").Substring (3).Replace (".hl", ""));
                    return leftInt.CompareTo (rightInt);
            });

            foreach (Node idxFile in dbFoldersNode.Children)
                yield return idxFile.Value as string;
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
                Node folderNode = new Node (string.Empty, fileNode.Get<string> ().Substring (0, fileNode.Get<string> ().LastIndexOf ("/")));
                context.Raise ("pf.file.list-files", folderNode);
                if (folderNode.Count == 0) {
                    context.Raise ("pf.file.remove-folder", folderNode);
                }
            } else {

                // converts node to code
                Node convertNode = new Node ();
                foreach (Node idx in fileNode.Children) {
                    convertNode.Add (idx.Clone ());
                }
                context.Raise ("pf.nodes-2-code", convertNode);

                // saves code
                Node saveNode = new Node (string.Empty, fileNode.Value);
                saveNode.Add (new Node (string.Empty, convertNode.Value));
                context.Raise ("pf.file.save", saveNode);
            }
        }
        
        /*
         * returns the next available database file node to store nodes within
         */
        private static Node GetAvailableFileNode (ApplicationContext context)
        {
            // searching through database to see if there are any nodes we can use from before
            int objectsPerFile = int.Parse(ConfigurationManager.AppSettings ["database-nodes-per-file"] ?? "32");
            foreach (Node idxFileNode in _database.Children)
            {
                if (idxFileNode.Count < objectsPerFile)
                    return idxFileNode;
            }

            // creating new node and appending into database
            Node newNode = new Node (string.Empty, FindAvailableNewFileName (context));
            _database.Add (newNode);
            return newNode;
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
            context.Raise ("pf.file.create-folder", createDirectoryNode);
        }
    }
}

