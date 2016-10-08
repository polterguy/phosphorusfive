/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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

using p5.exp;
using p5.core;

namespace p5.io.common
{
    /// <summary>
    ///     Class to help iterate files and folders
    /// </summary>
    public static class ObjectIterator
    {
        // Callback for iterating files and folders. Unless you return "true", iteration will stop.
        public delegate void ObjectIteratorDelegate (string filename, string fullpath);

        /// <summary>
        ///     Allows you to iterate files and folders for querying them
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Parameters passed into Active Event</param>
        /// <param name="removeArgsValue">If true, will remove args.Value after evaluation</param>
        /// <param name="authorizeEvent">Name of [.p5.io.authorize] Active Event to authorize operation</param>
        public static void Iterate (
            ApplicationContext context, 
            Node args, 
            bool removeArgsValue, 
            string authorizeEvent,
            ObjectIteratorDelegate functor)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (args, removeArgsValue)) {

                // Getting root folder
                var rootFolder = Common.GetRootFolder (context);

                // Multiple filename source, returning existence of all files
                foreach (var idxFileObject in XUtil.Iterate<string> (context, args, true)) {

                    // Retrieving actual system path
                    var fileObjectName = Common.GetSystemPath (context, idxFileObject);

                    // Verifying user is authorized to reading from currently iterated file
                    Common.RaiseAuthorizeEvent (context, args, authorizeEvent, idxFileObject);

                    // Invoking callback delegate
                    functor (fileObjectName, rootFolder + fileObjectName);
                }
            }
        }
    }
}
