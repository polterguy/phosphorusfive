
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
    /// class wrapping the internal memory-based pf.lambda expression database
    /// </summary>
    public static class database
    {
        // contains the path to the database folder, where all the pf.lambda files are stored
        private static string _dbPath;

        // actual database content
        private static Node _database;

        /// <summary>
        /// selects items from database according to expression given as value of node, and returns the matches
        /// as children nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.select")]
        private static void pf_data_select (ApplicationContext context, ActiveEventArgs e)
        {
            // verifying syntax
            if (!XUtil.IsExpression (e.Args.Value))
                throw new ArgumentException ("[pf.data.select] requires an expression to select items from database");

            // making sure database is initialized
            Initialize (context);

            // iterating through each result from database node tree
            foreach (var idxMatch in XUtil.Iterate (e.Args, _database, context)) {

                // aborting iteration early if it is a 'count' expression
                if (idxMatch.TypeOfMatch == Match.MatchType.count) {
                    e.Args.Add (new Node (string.Empty, idxMatch.Match.Count));
                    return;
                }

                // dependent upon type of expression, we either return a bunch of nodes, flat, with
                // name being string.Empty, and value being matched value, or we append node itself back
                // to caller. this allows us to select using expressions which are not of type 'node'
                if (idxMatch.TypeOfMatch != Match.MatchType.node) {

                    // returning 'value', 'name' or 'path' of expression as children nodes of argument node
                    // having name of returned node being string.Empty and value being result of expression
                    e.Args.Add (new Node (string.Empty, idxMatch.Value));
                } else {

                    // returning node itself, after cloning
                    e.Args.Add (idxMatch.Node.Clone ());
                }
            }
        }

        /// <summary>
        /// updates the results of the given expression in database, either according to a static [soure] node,
        /// or a relative [rel-source] node. if you supply a static [source], then source can either be a constant
        /// value, or an expression. if you supply a [rel-source], then source must be relative to nodes you wish
        /// to update
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.update")]
        private static void pf_data_update (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure database is initialized
            Initialize (context);

            // figuring out source, and executing the corresponding logic
            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-source") {

                // static source, not a node, might be an expression
                UpdateRelativeSource (e.Args, context);
            } else {

                // relative source, source must be an expression
                UpdateStaticSource (e.Args, context);
            }
        }
        
        /*
         * sets all destination nodes relative to themselves
         */
        private static void UpdateRelativeSource (Node node, ApplicationContext context)
        {
            // iterating through all destinations, figuring out source relative to each destinations
            List<Node> changed = new List<Node> ();
            foreach (var idxDestination in XUtil.Iterate (node, _database, context)) {
                
                // figuring out which file Node updated belongs to, and storing in changed list
                AddNodeToChanges (idxDestination.Node, changed);

                // source is relative to destination
                idxDestination.Value = XUtil.Single<object> (node.LastChild, idxDestination.Node, context, null);
            }

            // saving all affected files
            SaveAffectedFiles (context, changed);
        }

        /*
         * sets all destinations to static value where value is string or expression
         */
        private static void UpdateStaticSource (Node node, ApplicationContext context)
        {
            // figuring out source
            object source = GetStaticSource (node, context);

            // iterating through all destinations, updating with source
            List<Node> changed = new List<Node> ();
            foreach (var idxDestination in XUtil.Iterate (node, _database, context)) {
                
                // figuring out which file Node updated belongs to, and storing in changed list
                AddNodeToChanges (idxDestination.Node, changed);

                // doing actual update
                idxDestination.Value = source;
            }

            // saving all affected files
            SaveAffectedFiles (context, changed);
        }

        /*
         * retrieves the source for a "static source" update operation
         */
        private static object GetStaticSource (Node node, ApplicationContext context)
        {
            object retVal = null;

            // checking to see if there is a source at all
            if (node.LastChild.Name == "source") {

                // we have source nodes
                if (node.LastChild.Value != null) {

                    // source is either constant value or an expression
                    retVal = XUtil.Single<object> (node.LastChild, context, null);
                } else {

                    // source is either a node or null
                    if (node.LastChild.Count == 1) {

                        // source is a node
                        retVal = node.LastChild.FirstChild;
                    } else if (node.LastChild.Count == 0) {

                        // source is null
                        retVal = null;
                    } else {

                        // more than one source
                        throw new ArgumentException ("[pf.data.update] requires that you give it only one source");
                    }
                }
            }

            // returning source (or null) back to caller
            return retVal;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.insert")]
        private static void pf_data_insert (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure database is initialized
            Initialize (context);

            // verifying syntax of statement
            if (e.Args.Count == 0 && string.IsNullOrEmpty (e.Args.Value as string))
                throw new ArgumentException ("[pf.data.insert] requires at least one child node, or a source expression");

            // looping through all nodes given as children and saving them to database
            List<Node> changed = new List<Node> ();
            foreach (Node idx in XUtil.Iterate<Node> (e.Args, context)) {

                // making sure insert node gets an ID, unless one is explicitly given
                if (idx.Value == null)
                    idx.Value = Guid.NewGuid ();

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
            var match = Expression.Create (e.Args.Get<string> (context)).Evaluate (_database, context);

            // looping through database matches and removing nodes while storing which files have been changed
            List<Node> changed = new List<Node> ();
            foreach (var idxDest in match) {

                // figuring out which file Node updated belongs to, and storing in changed list
                AddNodeToChanges (idxDest.Node, changed);

                // replacing node in database
                idxDest.Node.UnTie ();
            }

            // saving all affected files
            foreach (Node idxNode in changed) {
                SaveFileNode (context, idxNode);

                // checking to see if database node is empty, and if so, removing it from database nodes
                if (idxNode.Count == 0)
                    idxNode.UnTie ();
            }
        }
        
        /// <summary>
        /// generates a new Guid for you, which you can use as a unique ID for database objects
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.data.new-guid")]
        private static void pf_data_new_guid (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Guid.NewGuid ();
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
         * saves a database node to disc
         */
        private static void SaveFileNode (ApplicationContext context, Node fileNode)
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
