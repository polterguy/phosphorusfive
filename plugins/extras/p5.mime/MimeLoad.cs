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
using p5.mime.helpers;
using MimeKit;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the MIME load features of Phosphorus Five
    /// </summary>
    public static class MimeLoad
    {
        /// <summary>
        ///     Loads and parses MIME message(s) from given file(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.load-file")]
        public static void p5_mime_load_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Keeping base folder to application around
                string baseFolder = Common.GetRootFolder (context).TrimEnd ('/');

                // Looping through each filename supplied by caller
                foreach (var idxFilename in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Verifying user is authorized to reading from currently iterated file
                    context.Raise (".p5.io.authorize.read-file", new Node ("", idxFilename).Add ("args", e.Args));

                    // Loading, processing and returning currently iterated message
                    var parser = new helpers.MimeParser (
                        context, 
                        e.Args, 
                        MimeEntity.Load (baseFolder + idxFilename), 
                        e.Args.GetExChildValue<string> ("attachment-folder", context));

                    // Parses the MimeEntity and stuffs results into e.Args node
                    parser.Process ();
                }
            }
        }
    }
}

