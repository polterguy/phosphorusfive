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
            foreach (var idxUserNode in pwdFile ["access"].Children) {

                // Adding currently iterated access to return args.
                if (roles == null) {

                    // Adding everything.
                    args.Add (idxUserNode.Clone ());

                } else {

                    // Adding only access objects requested by caller.
                    if (idxUserNode.Name == "*" || idxUserNode.Name == roles)
                        args.Add (idxUserNode.Clone ());
                }
            }
        }
        
        /*
         * Returns all access objects for system.
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
         * Returns all access objects for system.
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
         * Returns all access objects for system.
         */
        public static void HasAccessToPath (ApplicationContext context, Node args)
        {
            // Checking is user is root, at which he has access to everything.
            if (context.Ticket.Role == "root") {
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

            // Retrieving all access objects.
            var node = new Node ();
            Access.ListAccess (context, node);

            // Defaulting access to invoker node's existing value.
            var has_access = args.Get (context, false);

            // Checking if we have any access objects at all.
            if (node.Count > 0) {

                // Getting children as list, such that we can more easily modify it.
                var access = node.Children.ToList ();

                // Removing all access right objects not relevant to current user, current path, and current operation type.
                access.RemoveAll (ix => ix.Name != "*" && ix.Name != context.Ticket.Role);
                access.RemoveAll (ix => ix [filter + ".allow"] == null && ix [filter + ".deny"] == null);

                // Notice, to support pats such as "~/xxx" and "@FOO/", we explicitly unroll paths in our access object(s), before doing a comparison for a match.
                access.RemoveAll (ix => !path.StartsWithEx (context.RaiseEvent (".p5.io.unroll-path", new Node ("", ix [0].Get<string> (context))).Get<string> (context)));

                // Checking if we still have some access right object(s).
                if (access.Count > 0) {

                    // Sorting remaining access rights on their path value.
                    access.Sort (delegate (Node lhs, Node rhs) {

                        // First doing a path comparison.
                        var retVal = string.Compare (lhs [0].Get<string> (context), rhs [0].Get<string> (context), true, CultureInfo.InvariantCulture);

                        /*
                         * If the paths were similar, we make sure all asterix (*) roles are sorted before any special role overrides.
                         * We do this such that a specifically mentioned role can override the value for an asterix (*) role declaration.
                         */
                        if (retVal == 0) {
                            if (lhs.Name == "*" && rhs.Name != "*")
                                retVal = -1;
                            else if (lhs.Name != "*" && rhs.Name == "*")
                                retVal = 1;
                        }
                        return retVal;
                    });

                    /*
                     * Looping through any remaining access rights, to see if that modifies our return value.
                     * Making sure we return to caller whether or not anything was found at all.
                     */
                    if (access.Count > 0)
                        args.Add ("explicit", true);
                    foreach (var idxAccess in access) {
                        if (idxAccess [0].Name == filter + ".allow") {

                            // Then we must verify that the file's type is correct, if there is an explicit [file-type] argument in this access object.
                            // Or allow access, if this is a folder request (ending eith "/") and the access object is a "folder type of access object".
                            var file_types = idxAccess [0].GetChildValue ("file-type", context, "").Split (new char [] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            if (file_types.Length > 0) {

                                // File type declaration, making sure it matches specified path.
                                if (file_types.Any (ix => path.EndsWithEx ("." + ix))) {
                                    has_access = true;
                                } else {
                                    has_access = false;
                                }

                            } else if (path.EndsWithEx ("/") && idxAccess [0].GetChildValue ("folder", context, false)) {

                                // Folder access.
                                has_access = true;

                            } else {

                                // No type declaration for access object.
                                has_access = true;
                            }

                        } else if (idxAccess [0].Name == filter + ".deny") {

                            // Then we must verify that the file's type is correct, if there is an explicit [file-type] argument in this access object.
                            // Or allow access, if this is a folder request (ending eith "/") and the access object is a "folder type of access object".
                            var file_types = idxAccess [0].GetChildValue ("file-type", context, "").Split (new char [] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            if (file_types.Length > 0) {

                                // File type declaration, making sure it matches specified path.
                                if (file_types.Any (ix => path.EndsWithEx ("." + ix))) {
                                    has_access = true;
                                } else {
                                    has_access = false;
                                }

                            } else if (path.EndsWithEx ("/") && idxAccess [0].GetChildValue ("folder", context, false)) {

                                // Folder access.
                                has_access = false;

                            } else {

                                // No type declaration for access object.
                                has_access = false;
                            }
                        }
                    }
                }
            }

            // Returns access to caller.
            args.Value = has_access;
        }
            
        /*
         * Returns all access objects for system.
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

                        // Removing all matches.
                        access.Children.First (ix => ix.Name == idxAccess.Name && ix.Get (context, "") == idxAccess.GetExValue (context, "")).UnTie ();
                    }
                });
        }
    }
}