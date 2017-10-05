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
using p5.exp;
using p5.core;
using p5.mime.helpers;
using p5.exp.exceptions;
using MimeKit;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the MIME parse features of Phosphorus Five
    /// </summary>
    public static class MimeParse
    {
        /// <summary>
        ///     Parses the MIME message given as MimeEntity
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.mime.parse-native")]
        private static void _p5_mime_parse_native (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving MimeEntity from caller's arguments
            var entity = e.Args.Get<MimeEntity> (context);
            var parser = new helpers.MimeParser (
                context,
                e.Args,
                entity,
                e.Args.GetExChildValue<string> ("attachment-folder", context));

            // Parses the MimeEntity and stuffs results into e.Args node
            parser.Process ();
        }

        /// <summary>
        ///     Parses the MIME message given as a stream.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.mime.load-from-stream")]
        public static void _p5_mime_load_from_stream (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves.
            using (new ArgsRemover (e.Args, true)) {
                // Sanity check.
                var stream = e.Args.Value as Stream;
                if (e.Args.Value == null)
                    throw new LambdaException (
                        "No stream provided to [.p5.mime.parse-from-stream]",
                        e.Args,
                        context);

                // Loading MimeEntity from specified stream.
                MimeEntity entity = null;
                if (e.Args ["Content-Type"] != null)
                    entity = MimeEntity.Load (ContentType.Parse (e.Args ["Content-Type"].Get<string> (context)), stream);
                else
                    entity = MimeEntity.Load (stream);
                var parser = new helpers.MimeParser (
                    context,
                    e.Args,
                    entity,
                    e.Args.GetExChildValue<string> ("attachment-folder", context),
                    e.Args.GetExChildValue<bool> ("attachment-folder-no-prefix", context, true));

                // Parses the MimeEntity and stuffs results into e.Args node
                parser.Process ();
            }
        }

        /// <summary>
        ///     Parses the MIME message given as string.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.parse")]
        public static void p5_mime_parse (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new ArgsRemover (e.Args, true)) {

                // Looping through each MIME message supplied
                foreach (var idxMimeMessage in XUtil.Iterate<string> (context, e.Args)) {

                    // Sanity check
                    if (string.IsNullOrEmpty (idxMimeMessage))
                        throw new LambdaException (
                            "No MIME message provided to [p5.mime.parse]",
                            e.Args,
                            context);

                    // Loading MIME entity from stream
                    using (var writer = new StreamWriter (new MemoryStream ())) {

                        // Writing MIME content to StreamWriter, flushing the stream, and setting reader head back to beginning
                        writer.Write (idxMimeMessage);
                        writer.Flush ();
                        writer.BaseStream.Position = 0;

                        // Loading MimeEntity from MemoryStream
                        MimeEntity entity = null;
                        if (e.Args ["Content-Type"] != null)
                            entity = MimeEntity.Load (ContentType.Parse (e.Args ["Content-Type"].Get<string> (context)), writer.BaseStream);
                        else
                            entity = MimeEntity.Load (writer.BaseStream);
                        var parser = new helpers.MimeParser (
                            context,
                            e.Args,
                            entity,
                            e.Args.GetExChildValue<string> ("attachment-folder", context),
                            e.Args.GetExChildValue<bool> ("attachment-folder-no-prefix", context, true));

                        // Parses the MimeEntity and stuffs results into e.Args node
                        parser.Process ();
                    }
                }
            }
        }

        /// <summary>
        ///     Loads the file given as a string, and creates a MIME message from it.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mime.load")]
        public static void p5_mime_load (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new ArgsRemover (e.Args, true)) {

                // Looping through each MIME message supplied
                foreach (var idxMimeMessage in XUtil.Iterate<string> (context, e.Args)) {

                    // Sanity check
                    if (string.IsNullOrEmpty (idxMimeMessage))
                        throw new LambdaException (
                            "No MIME message provided to [p5.mime.parse]",
                            e.Args,
                            context);

                    // Retrieving output filename, and doing some basic sanity checking.
                    var filePath = idxMimeMessage;
                    filePath = context.RaiseEvent (".p5.io.unroll-path", new Node ("", filePath)).Get<string> (context);
                    context.RaiseEvent (".p5.io.authorize.read-file", new Node ("", filePath).Add ("args", e.Args));

                    // Loading MIME entity from currentl iterated file.
                    using (var reader = File.OpenRead (Common.GetRootFolder (context) + filePath)) {

                        // Loading MimeEntity from MemoryStream
                        MimeEntity entity = null;
                        if (e.Args ["Content-Type"] != null)
                            entity = MimeEntity.Load (ContentType.Parse (e.Args ["Content-Type"].Get<string> (context)), reader);
                        else
                            entity = MimeEntity.Load (reader);
                        var parser = new helpers.MimeParser (
                            context,
                            e.Args,
                            entity,
                            e.Args.GetExChildValue<string> ("attachment-folder", context),
                            e.Args.GetExChildValue<bool> ("attachment-folder-no-prefix", context, true));

                        // Parses the MimeEntity and stuffs results into e.Args node
                        parser.Process ();
                    }
                }
            }
        }
    }
}

