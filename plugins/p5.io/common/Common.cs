/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

using p5.core;
using p5.exp.exceptions;

namespace p5.io.common
{
    /// <summary>
    ///     Helper nethods for IO operations.
    /// </summary>
    public static class Common
    {
        /// <summary>
        ///     Unrolls the path for current node, if path contains variables, such as @SYS42 etc.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        [ActiveEvent (Name = ".p5.io.unroll-path")]
        public static void _p5_io_unroll_path (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = GetSystemPath (context, e.Args.Get<string> (context));
        }

        /*
         * Returns the root folder of application pool back to caller.
         */
        internal static string GetRootFolder (ApplicationContext context)
        {
            return context.RaiseActiveEvent (".p5.core.application-folder").Get<string> (context);
        }

        /*
         * Returns the "system path" unrolled, replacing for instance "~" and "@SYS42" in the beginning of path.
         */
        internal static string GetSystemPath (ApplicationContext context, string path)
        {
            // Checking if path contains alias variables.
            if (path.StartsWith ("~")) {

                // Returning user's folder.
                if (context.Ticket.IsDefault)
                    return "/common" + path.Substring (1);
                else
                    return "/users/" + context.Ticket.Username + path.Substring (1);

            } else if (path.StartsWith ("@")) {

                // Returning variable path according to results of Active Event invocation.
                var variable = path.Substring (0, path.IndexOf ("/"));
                var variableUnrolled = context.RaiseActiveEvent ("p5.io.unroll-path." + variable).Get<string> (context);

                // Recursively invoking self untill there is nothing more to unroll.
                return GetSystemPath (context, variableUnrolled + path.Substring (variable.Length));
            }
            return path;
        }

        /*
         * Raises the specified authorize event for given path.
         */
        internal static void RaiseAuthorizeEvent (
            ApplicationContext context, 
            Node args, 
            string eventName, 
            string path)
        {
            switch (eventName) {
                case "read-folder":
                case "read-file":
                case "modify-folder":
                case "modify-file":
                    context.RaiseActiveEvent (".p5.io.authorize." + eventName, new Node ("", GetSystemPath (context, path)).Add ("args", args));
                    break;
                default:
                throw new LambdaException (
                    string.Format ("Unknown authorize event '{0}' given to '{1}'.", eventName, args.Name), 
                    args, 
                    context);
            }
        }
    }
}
