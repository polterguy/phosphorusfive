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
using p5.io.authorization.helpers;

namespace p5.io.authorization
{
    /// <summary>
    ///     Class wrapping authorization for files in Phosphorus Five
    /// </summary>
    static class Authorization
    {
        /// <summary>
        ///     Throws an exception if user is not authorized to read the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.io.authorize.read-file")]
        static void _p5_io_authorize_read_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeReadFile (
                context, 
                e.Args.Get<string> (context), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to modify/create the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.io.authorize.modify-file")]
        static void _p5_io_authorize_modify_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeModifyFile (
                context, 
                e.Args.Get<string> (context), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to read from the given folder
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.io.authorize.read-folder")]
        static void _authorize_read_folder (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeReadFolder (
                context, 
                e.Args.Get<string> (context), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to modify the given folder
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.io.authorize.modify-folder")]
        static void _p5_io_authorize_modify_folder (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeModifyFolder (
                context, 
                e.Args.Get<string> (context), 
                e.Args ["args"].Get<Node> (context));
        }
    }
}