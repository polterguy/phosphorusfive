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

using System.IO;
using p5.exp;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.file
{
    /// <summary>
    ///     Saves one or more file(s).
    /// </summary>
    public static class Save
    {
        /// <summary>
        ///     Saves one or more file(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.io.file.save")]
        public static void p5_io_file_save (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (e.Args.Value == null)
                throw new LambdaException ("[p5.io.file.save] requires a constant or an expression leading to its path", e.Args, context);

            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args)) {

                // Getting root folder.
                var rootFolder = Common.GetRootFolder (context);

                var content = Utilities.Convert<byte[]> (context, XUtil.Source (context, e.Args));

                // Iterating through each destination file path.
                foreach (var idxDestination in XUtil.Iterate<string> (context, e.Args)) {

                    // Verifying user is allowed to save to file
                    Common.RaiseAuthorizeEvent (context, e.Args, "modify-file", idxDestination);

                    // Saving file
                    using (FileStream stream = File.Create (rootFolder + Common.GetSystemPath (context, idxDestination))) {

                        // Writing content to file stream
                        stream.Write (content, 0, content.Length);
                    }
                }
            }
        }
    }
}
