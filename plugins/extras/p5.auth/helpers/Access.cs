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
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping access features of Phosphorus Five.
    /// </summary>
    static class Access
    {
        /*
         * Returns all access objects for system.
         */
        public static void ListAccess (ApplicationContext context, Node args)
        {
            // Getting password file in Node format.
            Node pwdFile = AuthFile.GetAuthFile (context);

            // Checking if caller wants to remove GUID IDs of access objects in the system.
            var keepGuids = args.GetExChildValue ("guids", context, false);

            // Checking if we have any access rights in system.
            if (pwdFile ["access"] == null)
                return;

            // Checking which role caller requests access objects on behalf.
            string roles = null;
            if (context.Ticket.Role == "root") {

                // Checking if caller requested a particular role.
                roles = args.GetExChildValue<string> ("role", context, null);

            } else {

                // A non-root user is not allowed to request anything besides his own access objects.
                if (args ["role"] != null)
                    throw new LambdaException ("A non-root user cannot request access objects for anything but his own role", args, context);

                // Making sure user only retrieves access objects that are relevant for his own role.
                roles = context.Ticket.Role;
            }

            // Looping through each user object in password file, retrieving all roles.
            foreach (var idxAccessNode in pwdFile ["access"].Children) {

                // Checking what types of access object(s) are relevant to caller.
                if (roles == null) {
                    
                    // Cloning access object node.
                    var cur = idxAccessNode.Clone ();

                    // Removing GUID IDs if caller specified we should do so.
                    Guid guid;
                    if (!keepGuids && Guid.TryParse (idxAccessNode.Get<string> (context), out guid))
                        cur.Value = null;

                    // Caller wants everything.
                    args.Add (cur);

                } else {

                    // Checking if currently iterated access object is relevant to caller.
                    if (idxAccessNode.Name == "*" || idxAccessNode.Name == roles) {
                        
                        // Cloning access object node.
                        var cur = idxAccessNode.Clone ();

                        // Removing GUID IDs if caller specified we should do so.
                        Guid guid;
                        if (!keepGuids && Guid.TryParse (idxAccessNode.Get<string> (context), out guid))
                            cur.Value = null;

                        // Adding currently iterated access object.
                        args.Add (cur);
                    }
                }
            }
        }
        
        /*
         * Adds a new access object to the system.
         */
        public static void AddAccess (ApplicationContext context, Node args)
        {
            // Locking access to password file as we create new access object.
            AuthFile.ModifyAuthFile (
                context,
                delegate (Node authFile) {

                    // Verifying access rights exists.
                    if (authFile ["access"] == null)
                        authFile.Add ("access");

                    // Iterating all access objects passed in by caller, and adding them to root access node.
                    var access = authFile ["access"];
                    var newAccessRights = XUtil.Iterate<Node> (context, args).ToList ();
                    foreach (var idxAccess in newAccessRights) {

                        // Sanity checking.
                        var val = idxAccess.GetExValue (context, "");
                        if (string.IsNullOrEmpty (val)) {

                            // Creating a new random GUID as the ID of our access object.
                            val = Guid.NewGuid ().ToString ();
                            idxAccess.Value = val;

                        } else {

                            // Verifying access ID is unique.
                            if (access.Children.Any (ix => ix.Get (context, "") == val))
                                throw new LambdaException ("Each access right must have a unique name/value combination, and there's already another access right with the same name/value combination in your access list", idxAccess, context);
                        }

                        // Sanity checking access object.
                        if (idxAccess.Count == 0)
                            throw new LambdaException ("There's no actual content in your access object", idxAccess, context);

                        // Adding currently iterated access object.
                        access.Add (idxAccess.Clone ()); 
                    }
                });
        }
        
        /*
         * Sets all access objects for system.
         */
        public static void SetAccess (ApplicationContext context, Node args)
        {
            // Locking access to password file as we create new access object.
            AuthFile.ModifyAuthFile (
                context,
                delegate (Node authFile) {

                    // Verifying access rights exists.
                    if (authFile ["access"] == null)
                        authFile.Add ("access");

                    // Retrieving access root node.
                    var access = authFile ["access"];

                    // Clearing all previous access objects.
                    access.Clear ();

                    // Iterating all access objects supplied by caller, adding them to our access node.
                    var newAccessRights = XUtil.Iterate<Node> (context, args).ToList ();
                    foreach (var idxAccess in newAccessRights) {

                        // Sanity checking.
                        var val = idxAccess.GetExValue (context, "");
                        if (string.IsNullOrEmpty (val)) {

                            // Creating a new random GUID as the ID of our access object.
                            val = Guid.NewGuid ().ToString ();
                            idxAccess.Value = val;

                        } else {

                            // Verifying access ID is unique.
                            if (access.Children.Any (ix => ix.Get (context, "") == val))
                                throw new LambdaException ("Each access right must have a unique name/value combination, and there's already another access right with the same name/value combination in your access list", idxAccess, context);
                        }
                        
                        // Sanity checking access object.
                        if (idxAccess.Count == 0)
                            throw new LambdaException ("There's no actual content in your access object", idxAccess, context);

                        // Adding currently iterated access object.
                        access.Add (idxAccess.Clone ()); 
                    }
                });
        }
        
        /*
         * Deletes all specified access objects.
         */
        public static void DeleteAccess (ApplicationContext context, Node args)
        {
            // Locking access to password file as we create new access object.
            AuthFile.ModifyAuthFile (
                context,
                delegate (Node authFile) {

                    // Verifying access rights exists.
                    if (authFile ["access"] == null)
                        return;

                    // Iterating all access objects passed in by caller.
                    var access = authFile ["access"];
                    var delAccess = XUtil.Iterate<Node> (context, args).ToList ();
                    foreach (var idxAccess in delAccess) {

                        // Removing all matches by simply "untying" the access object matching currently iterated access object.
                        access.Children.First (ix => ix.Name == idxAccess.Name && ix.Get (context, "") == idxAccess.GetExValue (context, "")).UnTie ();
                    }
                });
        }

        /*
         * Returns true if currently logged in user has access to some "path".
         */
        public static void HasAccessToPath (ApplicationContext context, Node args)
        {
            // Checking is user is root, at which point he has access to everything.
            if (context.Ticket.Role == "root") {

                // Making sure we notify caller that explicit access was granted, and returning early,
                // Defaulting access to true, since root has access to everything anyway.
                args.Add ("explicit", true);
                args.Value = true;
                return;
            }

            // Retrieving [filter] argument.
            var filter = args.GetExChildValue<string> ("filter", context);
            if (string.IsNullOrEmpty (filter))
                throw new LambdaException ("No [filter] supplied", args, context);

            // Retrieving [path] argument.
            var path = args.GetExChildValue<string> ("path", context);
            if (string.IsNullOrEmpty (path))
                throw new LambdaException ("No [path] supplied", args, context);

            // Making sure we unroll path.
            path = context.RaiseEvent (".p5.io.unroll-path", new Node ("", path)).Get<string> (context);
            
            // Defaulting access to invoker node's existing value, or "false" if no default is supplied by caller.
            var has_access = args.Get (context, false);

            // Retrieving all access objects.
            var accessNode = new Node ();
            Access.ListAccess (context, accessNode);

            // Checking if we have any access objects at all.
            if (accessNode.Count > 0) {

                // Removing all non-relevant access objects, and creating a list with remaining (relevant) objects.
                var accessList = ExtractRelevantAccessObjects (context, filter, path, accessNode);

                // Checking if we still have some access right object(s).
                if (accessList.Count > 0) {

                    // Making sure we return to caller that access was 'explicitly' granted or denied.
                    args.Add ("explicit", true);

                    // Sorting access objects according to precedense.
                    SortAccessObjects (context, accessList);

                    // Looping through any remaining access rights, to see if that modifies our return value.
                    foreach (var idxAccess in accessList) {

                        // Updates return value in accordance to currently iterated access object.
                        has_access = DetermineAccess (context, filter, path, has_access, idxAccess);
                    }
                }
            }

            // Returns access to caller.
            args.Value = has_access;
        }

        /*
         * Extracts only relevant access objects and returns to caller.
         * 
         * This implies removing all access objects that can't be found in the start of the given path and
         * removing those that doesn't match the filter.
         */
        private static List<Node> ExtractRelevantAccessObjects (ApplicationContext context, string filter, string path, Node accessNode)
        {
            // Getting children as list, such that we can more easily modify it.
            var access = accessNode.Children.ToList ();

            // Removing all access right objects not relevant to current user, and current operation type.
            access.RemoveAll (ix => ix.Name != "*" && ix.Name != context.Ticket.Role);
            access.RemoveAll (ix => ix [filter + ".allow"] == null && ix [filter + ".deny"] == null);

            /*
             * Making sure we remove all access objects that are not relevant for the specified [path] supplied by caller.
             * This implies that we only keep access objects that can be found in the start of the [path] supplied by caller.
             * 
             * This allows access objects to "cascade", implying checking for access to "/foo/bar/" will yield false if user
             * does not have access to "/foo/", etc.
             * 
             * Notice, to support pats such as "~/xxx" and "@FOO/" in our access objects, we explicitly unroll our access object(s), 
             * before doing a comparison for a match.
             */
            access.RemoveAll (ix => !path.StartsWithEx (
                context.RaiseEvent (
                    ".p5.io.unroll-path",
                    new Node ("", ix [0].Get<string> (context))).Get<string> (context)));
            return access;
        }
        
        /*
         * Sorts specified access object list according to precedence.
         * 
         * This implies sorting objects first by their paths, then putting * objects first, for then
         * to put ".allow" objects first. This implies the most restrictive and especific access objects
         * ends up last, and hence will be applied last.
         */
        private static void SortAccessObjects (ApplicationContext context, List<Node> accessList)
        {
            /*
             * Sorting remaining access rights on their path value.
             * 
             * This makes sure an access object with a path of "/foo/bar/" will 'override' and access object with the path of "/foo/".
             */
            accessList.Sort (delegate (Node lhs, Node rhs) {

                // First doing a path comparison.
                var retVal = string.Compare (lhs [0].Get<string> (context), rhs [0].Get<string> (context), true, CultureInfo.InvariantCulture);

                /*
                 * If the paths were equal, we make sure all asterix (*) roles are sorted before any explicitly named role overrides,
                 * for then to make sure any "deny" access objects overrides "allow" objects.
                 * 
                 * We do this such that an explicitly mentioned role can override the value for an asterix (*) role declaration,
                 * while still allowing any "deny" roles to override any "allow" roles.
                 */
                if (retVal == 0) {
                    if (lhs.Name == "*" && rhs.Name != "*")
                        retVal = -1;
                    else if (lhs.Name != "*" && rhs.Name == "*")
                        retVal = 1;
                    else if (lhs.FirstChild.Name.EndsWithEx (".deny") && rhs.FirstChild.Name.EndsWithEx (".allow"))
                        retVal = 1;
                    else if (lhs.FirstChild.Name.EndsWithEx (".allow") && rhs.FirstChild.Name.EndsWithEx (".deny"))
                        retVal = -1;
                }
                return retVal;
            });
        }
        
        /*
         * Determines access according to specified access object.
         * 
         * Whether or not access object grants user access is determined according to its lats parts,
         * which can be ".allow" or ".deny". Whether or not the access object matches the given path, depends
         * upon the access object's parameters. An access object can be paratemtrized with [file-type], which
         * is a string of file extensions, separated by "|", e.g. "hl|md|js". The access object can also be
         * parametrized with [folder] having a value of true, at which it'll only match the path if the path
         * ends with a "/".
         */
        private static bool DetermineAccess (ApplicationContext context, string filter, string path, bool previousAccess, Node accessNode)
        {
            // Checking if this is a "simple" access object, without file/folder parameters.
            if (accessNode [0].Count == 0) {

                /*
                 * Simple access object without parameters.
                 * 
                 * Access granted or denied, depending upon access object's name.
                 */
                previousAccess = accessNode [0].Name == filter + ".allow";

            } else {

                /*
                 * This might be a [file] or [folder] type of access object.
                 */
                if (accessNode [0].FirstChild.Name == "folder") {

                    // Folder access object.
                    if (path.EndsWithEx ("/") && accessNode [0].GetChildValue ("folder", context, false)) {

                        // Access granted or denied, depending upon access object's name.
                        previousAccess = accessNode [0].Name == filter + ".allow";
                    }
                } else if (accessNode [0].FirstChild.Name == "file-type") {

                    // File type of access object.
                    var file_types = accessNode [0].GetChildValue ("file-type", context, "")
                                                  .Split (new char [] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (file_types.Any (ix => path.EndsWithEx ("." + ix))) {

                        // Access granted or denied, depending upon access object's name.
                        previousAccess = accessNode [0].Name == filter + ".allow";
                    }
                } else if (accessNode [0].FirstChild.Name == "exact" && accessNode [0].FirstChild.Get<bool> (context)) {

                    // Currently iterated object requires an "exact" match.
                    if (path == accessNode [0].Get<string> (context)) {
                        previousAccess = accessNode [0].Name == filter + ".allow";
                    }
                }
            }

            return previousAccess;
        }
    }
}