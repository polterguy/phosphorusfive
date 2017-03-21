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

using System.IO;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.mime.helpers;
using p5.exp.exceptions;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the MIME creation features of Phosphorus Five
    /// </summary>
    public static class MimeCreate
    {
        /// <summary>
        ///     Creates a MIME message according to given arguments and returns as string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.create")]
        public static void p5_mime_create (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new ArgsRemover (e.Args, true)) {

                // Iterating through each node given, either as child of main node, or through expression
                foreach (var idxMimeNode in XUtil.Iterate<Node> (context, e.Args)) {

                    // Making sure we keep track of, closes, and disposes all streams created during process
                    List<Stream> streams = new List<Stream> ();
                    try {

                        // Creating and returning MIME message to caller as string
                        var creator = new MimeCreator (
                            context, 
                            idxMimeNode,
                            streams);
                        e.Args.Add ("result", creator.Create ().ToString ());

                    } finally {

                        // Disposing all streams created during process
                        foreach (var idxStream in streams) {

                            // Closing and disposing currently iterated stream
                            idxStream.Close ();
                            idxStream.Dispose ();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Creates a native MimeEntity according to given arguments and returns to caller as MimeEntity
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.mime.create-native")]
        private static void _p5_mime_create_native (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Count != 1)
                throw new LambdaException (
                    "You must have exactly one root node of your MIME message",
                    e.Args,
                    context);

            // Creating and returning MIME message to caller as MimeEntity
            MimeCreator creator = new MimeCreator (
                context, 
                e.Args.FirstChild,
                (List<Stream>)e.Args.Value);
            e.Args.Value = creator.Create ();
        }
    }
}

