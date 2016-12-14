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

using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.io.common
{
    /// <summary>
    ///     Common class to help copy, rename and/or move files.
    /// </summary>
    public static class MoveCopyHelper
    {
        // Delegate for actual implementation for handling one single file.
        internal delegate void MoveCopyDelegate (string rootFolder, string source, string destination);

        // Delegate for checking if fileobject exists.
        internal delegate bool ObjectExistDelegate (string destination);

        /*
         * Common helper for iterating files/folders for move/copy/rename operation.
         */
        internal static void CopyMoveFileObject (
            ApplicationContext context, 
            Node args, 
            string sourceAuthorizeEvent,
            string destinationAuthorizeEvent,
            MoveCopyDelegate functorMoveObject, 
            ObjectExistDelegate functorObjectExist)
        {
            // Sanity check.
            if (args.Value == null)
                throw new LambdaException (
                    string.Format("[{0}] requires a value being the source file(s) to operate upon", args.Name), 
                    args, 
                    context);

            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (args)) {

                // Retrieving destination file or folder, and root folder for app, making sure we get a [dest] node for destination.
                var dest = Common.GetSystemPath (context, Utilities.Convert<string> (context, XUtil.Source (context, args, "src")));
                var rootFolder = Common.GetRootFolder (context);

                // Iterating through each source.
                foreach (var idxSource in XUtil.Iterate<string> (context, args)) {

                    // Figuring out destination file, which might be "relative" to currently iterated source file.
                    var destinationFile = dest.EndsWith ("/") ? dest + idxSource.Substring (idxSource.LastIndexOf ("/") + 1) : dest;

                    // Making sure user is authorized to do operation on both source file and destination file.
                    Common.RaiseAuthorizeEvent (context, args, sourceAuthorizeEvent, idxSource);
                    Common.RaiseAuthorizeEvent (context, args, destinationAuthorizeEvent, destinationFile);

                    // Making sure destination file does not already exist.
                    if (functorObjectExist (rootFolder + destinationFile))
                        throw new LambdaException (string.Format ("The file '{0}' already exist from before", destinationFile), args, context);

                    functorMoveObject (
                        rootFolder, 
                        Common.GetSystemPath (context, Utilities.Convert<string> (context, idxSource)),
                        destinationFile);
                }
            }
        }
    }
}
