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
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.file
{
    /// <summary>
    ///     Class to help delete one or more file(s).
    /// </summary>
    public static class Delete
    {
        /// <summary>
        ///     Deletes one or more file(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-file")]
        [ActiveEvent (Name = "p5.io.file.delete")]
        public static void p5_io_file_delete (ApplicationContext context, ActiveEventArgs e)
        {
            ObjectIterator.Iterate (
                context, 
                e.Args, 
                true, 
                "modify-file", 
                delegate (string filename, string fullpath) {
                    if (File.Exists (fullpath)) {

                        // File exists, deleting it, but first making sure it's not read only.
                        // Notice, Linux and Mac allows a read only file to be deleted, so even though .Net on Windows takes care
                        // of this automatically for us, we'll need this check in here, to have *nix and Windows systems to behave similarly.
                        if (new FileInfo (fullpath).IsReadOnly)
                            throw new LambdaException (
                                string.Format ("[delete-file] tried to delete a read only file called; '{0}'", filename),
                                e.Args,
                                context);
                        File.Delete (fullpath);

                    } else {

                        // Oops, file didn't exist, throwing an exception.
                        throw new LambdaException (
                            string.Format ("[delete-file] tried to delete non-existing file '{0}'", filename),
                            e.Args,
                            context);
                    }
            });
        }
    }
}
