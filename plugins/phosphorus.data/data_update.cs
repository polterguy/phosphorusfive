
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
    /// class wrapping [pf.data.update] and its associated supporting methods
    /// </summary>
    public static class data_update
    {
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
            Common.Initialize (context);

            // figuring out source, and executing the corresponding logic
            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-source") {

                // static source, not a node, might be an expression
                UpdateRelativeSource (e.Args, context);
            } else if (e.Args.Count > 0 && e.Args.LastChild.Name == "source") {

                // relative source, source must be an expression
                UpdateStaticSource (e.Args, context);
            } else {

                // syntax error
                throw new ArgumentException ("no [source] or [rel-source] was given to [pf.data.update]");
            }
        }
        
        /*
         * sets all destination nodes relative to themselves
         */
        private static void UpdateRelativeSource (Node node, ApplicationContext context)
        {
            // iterating through all destinations, figuring out source relative to each destinations
            List<Node> changed = new List<Node> ();
            foreach (var idxDestination in XUtil.Iterate (node, Common.Database, context)) {
                
                // figuring out which file Node updated belongs to, and storing in changed list
                Common.AddNodeToChanges (idxDestination.Node, changed);

                // retrieving source relative to destination
                object source = XUtil.Single<object> (node.LastChild, idxDestination.Node, context, null);

                // making sure update creates a valid updated node structure
                SyntaxCheckUpdate (idxDestination, ref source, context);

                // source is relative to destination
                idxDestination.Value = source;
            }

            // saving all affected files
            Common.SaveAffectedFiles (context, changed);
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
            foreach (var idxDestination in XUtil.Iterate (node, Common.Database, context)) {
                
                // figuring out which file Node updated belongs to, and storing in changed list
                Common.AddNodeToChanges (idxDestination.Node, changed);
                
                // making sure update creates a valid updated node structure
                SyntaxCheckUpdate (idxDestination, ref source, context);

                // doing actual update
                idxDestination.Value = source;
            }

            // saving all affected files
            Common.SaveAffectedFiles (context, changed);
        }

        /*
         * verifies node marked for an update operation is a legal valid node
         */
        private static void SyntaxCheckUpdate (MatchEntity entity, ref object source, ApplicationContext context)
        {
            // we only really syntax check level 2 nodes, or "root data nodes" in database
            if (entity.Node.Path.Count == 2) {

                // this is a "level 2" update, and needs to be checked for integrity, making sure it
                // doesn't invalidate 'type' or ID of object
                if (entity.TypeOfMatch == Match.MatchType.node) {

                    // syntax checking, and potentially creating a new ID for a 'node' update
                    SyntaxCheckNodeUpdate (entity.Node, ref source, context);

                } else if (entity.TypeOfMatch == Match.MatchType.name) {

                    // converting source to string, to verify name is not empty or null
                    string nName = Utilities.Convert<string> (source, context);
                    if (string.IsNullOrEmpty (nName))
                        throw new ArgumentException ("[pf.data.update] cannot leave root data object node with an empty name");
                } else if (entity.TypeOfMatch == Match.MatchType.value) {

                    // syntax checking a 'value' update
                    SyntaxCheckValueUpdate (entity.Node, ref source, context);
                }
            }
        }
        
        /*
         * syntax checks a 'node' update
         */
        private static void SyntaxCheckNodeUpdate (Node node, ref object source, ApplicationContext context)
        {
            // converting source to node, to syntax check and make sure new node is a valid node for database
            Node nNode = Utilities.Convert<Node> (source, context);

            // making sure all "root data object nodes" in database has a name
            if (string.IsNullOrEmpty (nNode.Name))
                throw new ArgumentException ("[pf.data.update] cannot leave root object node with an empty name");

            // making sure all "root data object nodes" in database has a unique ID
            // but only if an ID is explicitly given, since otherwise engine will automatically assign
            // a unique Guid to node
            if (nNode.Value == null)
                nNode.Value = node.Value; // keeping old ID
            else if (XUtil.Iterate (
                string.Format (
                    @"@/*/*/=""{0}""/?node", 
                    nNode.Value), Common.Database, context).GetEnumerator ().MoveNext ())
                throw new ArgumentException ("[pf.data.update] requires that all nodes in database has a unique ID");
        }

        /*
         * syntax checks a 'value' update
         */
        private static void SyntaxCheckValueUpdate (Node node, ref object source, ApplicationContext context)
        {
            // checking to see if we should keep our old ID or not
            if (source == null) {

                // keeping old id
                source = node.Value;
            } else {

                // converting source to string, to verify ID is unique, if an ID is given
                string nID = Utilities.Convert<string> (source, context);
                if (nID != null && 
                    XUtil.Iterate (
                    string.Format (
                        @"@/*/*/=""{0}""/?node", 
                        nID), Common.Database, context).GetEnumerator ().MoveNext ())
                    throw new ArgumentException ("[pf.data.update] requires that all nodes in database has a unique ID");
            }
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
    }
}
