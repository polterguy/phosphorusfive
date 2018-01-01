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
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping access associated Active Events.
    /// </summary>
    static class Access
    {
        /// <summary>
        ///     Returns all access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.list")]
        public static void p5_auth_access_list (ApplicationContext context, ActiveEventArgs e)
        {
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.ListAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Returns all access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.add")]
        public static void p5_auth_access_add (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.AddAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Deletes specified access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.delete")]
        public static void p5_auth_access_delete (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.DeleteAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Deletes specified access rights in system.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.access.set-all")]
        public static void p5_auth_access_set_all (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Non-root user tried to execute a protected operation", e.Args, context);
            using (new ArgsRemover (e.Args, true)) {
                AuthenticationHelper.SetAccess (context, e.Args);
            }
        }
        
        /// <summary>
        ///     Verifies that user has access to some sort of path.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.has-access-to-path")]
        public static void p5_auth_has_access_to_path (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, false)) {

                // Checking is user is root, at which he has access to everything.
                if (context.Ticket.Role == "root") {
                    e.Args.Add ("explicit", true);
                    e.Args.Value = true;
                    return;
                }
    
                // Retrieving [filter] argument.
                var filter = e.Args.GetExChildValue<string> ("filter", context);
                if (string.IsNullOrEmpty (filter))
                    throw new LambdaException ("No [filter] supplied", e.Args, context);
    
                // Retrieving [path] argument.
                var path = e.Args.GetExChildValue<string> ("path", context);
                if (string.IsNullOrEmpty (path))
                    throw new LambdaException ("No [path] supplied", e.Args, context);

                // Making sure we unroll path.
                path = context.RaiseEvent (".p5.io.unroll-path", new Node ("", path)).Get<string> (context);

                // Retrieving all access objects.
                var node = new Node ();
                AuthenticationHelper.ListAccess (context, node);
                
                // Defaulting access to invoker node's existing value.
                var has_access = e.Args.Get (context, false);

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
                            e.Args.Add ("explicit", true);
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
                e.Args.Value = has_access;
            }
        }
    }
}