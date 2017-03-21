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
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.mail.helpers;
using MimeKit;
using MailKit.Net.Pop3;

namespace p5.mail
{
    /// <summary>
    ///     Class wrapping the POP3 email features of Phosphorus Five.
    /// </summary>
    public static class Pop3
    {
        // Contains all "standard headers", which we handle in special cases, and should not be handled by generic header handler.
        private static List<HeaderId> _excludedHeaders;

        /*
         * Static CTOR to initialize _excludedHeaders, containing list of all headers to not handle in generic handler.
         */
        static Pop3 ()
        {
            _excludedHeaders = new List<HeaderId> (new HeaderId [] {
                HeaderId.Bcc, HeaderId.Cc, HeaderId.Date, HeaderId.From, HeaderId.Importance, HeaderId.InReplyTo, HeaderId.MessageId, 
                HeaderId.MimeVersion, HeaderId.Priority, HeaderId.References, HeaderId.ReplyTo, HeaderId.ResentBcc, HeaderId.ResentCc, 
                HeaderId.ResentDate, HeaderId.ResentFrom, HeaderId.ResentReplyTo, HeaderId.ResentSender, HeaderId.ResentTo, 
                HeaderId.Sender, HeaderId.Subject, HeaderId.To, HeaderId.XPriority});
        }

        /// <summary>
        ///     Retrieves messages from a POP3 server.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.pop3.get")]
        public static void p5_pop3_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we remove arguments supplied.
            using (new ArgsRemover (e.Args, true)) {

                // Creating our POP3 client.
                using (var client = new Pop3Client ()) {

                    // Connecting to POP3 server using helper from Common class.
                    Common.ConnectServer (context, client, e.Args, "pop3");

                    // Making sure we're able to post QUIT signal when done, regardless of what happens inside of this code.
                    try {

                        // Figuring out how many messages to retrieve, defaulting to "5" if not explicitly told something else by caller,
                        // making sure we never try to retrieve more messages than server actually has.
                        int noMessages = Math.Min (client.Count, e.Args.GetExChildValue ("count", context, 5));

                        // Fetching messages from server, but not any more messages than caller requested, or number of available messages.
                        for (int idxMsg = 0; idxMsg < noMessages; idxMsg++) {

                            // Process message returned from POP3 server by building Node structure wrapping message.
                            var msgNode = ProcessMessage (
                                context,
                                e.Args,
                                client.GetMessage (idxMsg));

                            // Handle message after processing.
                            e.Args.Add (msgNode);

                            // Checking if we should delete message from server.
                            if (e.Args.GetExChildValue ("delete", context, false)) {

                                // Deleting message from server.
                                client.DeleteMessage (idxMsg);
                            }
                        }
                    } finally {

                        // Disconnecting from server, making sure we send the QUIT signal.
                        client.Disconnect (true);
                    }
                }
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Helper to process on message retrieved from POP3 server.
         */
        private static Node ProcessMessage (
            ApplicationContext context, 
            Node args,
            MimeMessage message)
        {
            // Node structure containing all headers, content, and other properties of message.
            Node msgNode = new Node ("envelope");

            // Processing message, headers first.
            ProcessMessageHeaders (context, msgNode, message);

            // Then content of message, making sure MimeEntity never leaves this method.
            msgNode.Value = message.Body;
            try {
                msgNode.AddRange (args.Children.Where (ix => ix.Name == "attachment-folder" || ix.Name == "decrypt").Select (ix => ix.Clone ()));
                context.RaiseEvent (".p5.mime.parse-native", msgNode);

            } finally {

                msgNode.Value = null;
                msgNode.RemoveAll (ix => ix.Name == "attachment-folder" || ix.Name == "decrypt");
            }

            // Making sure ID of email is value of [envelope] node.
            msgNode.Value = message.MessageId;

            // Return node containing processed message.
            return msgNode;
        }

        /*
         * Adds up all headers into given node.
         */
        private static void ProcessMessageHeaders (
            ApplicationContext context, 
            Node msgNode, 
            MimeMessage message)
        {
            // Subject.
            msgNode.Add ("Subject", message.Subject);

            // All address fields.
            GetAddresses (context, msgNode, message.From, "From");
            GetAddresses (context, msgNode, message.ResentFrom, "Resent-From");
            GetAddresses (context, msgNode, message.Bcc, "Bcc");
            GetAddresses (context, msgNode, message.ResentBcc, "Resent-Bcc");
            GetAddresses (context, msgNode, message.Cc, "Cc");
            GetAddresses (context, msgNode, message.ResentCc, "Resent-Cc");
            GetAddresses (context, msgNode, message.ReplyTo, "Reply-To");
            GetAddresses (context, msgNode, message.ResentReplyTo, "Resent-Reply-To");
            GetAddresses (context, msgNode, message.To, "To");
            GetAddresses (context, msgNode, message.ResentTo, "Resent-To");

            // Other standard headers.
            msgNode.Add ("Date", message.Date.DateTime);

            if (!string.IsNullOrEmpty (message.ResentMessageId))
                msgNode.Add ("Resent-Message-ID", message.ResentMessageId);

            if (message.Sender != null)
                msgNode.Add ("Sender", null, new Node[] { new Node (message.Sender.Name ?? "", message.Sender.Address) });

            if (message.ResentSender != null)
                msgNode.Add ("Resent-Sender", null, new Node[] { new Node (message.ResentSender.Name ?? "", message.ResentSender.Address) });

            if (message.MimeVersion != null)
                msgNode.Add ("MIME-Version", message.MimeVersion.ToString ());

            if (message.ResentDate != DateTimeOffset.MinValue)
                msgNode.Add ("Resent-Date", message.ResentDate.DateTime);

            if (message.Importance != MessageImportance.Normal)
                msgNode.Add ("Importance", message.Importance.ToString ());

            if (!string.IsNullOrEmpty (message.InReplyTo))
                msgNode.Add ("In-Reply-To", message.InReplyTo);

            if (message.Priority != MessagePriority.Normal)
                msgNode.Add ("Priority", message.Priority.ToString ());

            // Looping through and adding all IDs for messages this message is referencing.
            foreach (var id in message.References) {

                // Adding currently iterated References ID back to caller.
                msgNode.FindOrInsert ("References").Add (id);
            }

            // Non-standard headers.
            foreach (var idxHeader in message.Headers.Where (ix => _excludedHeaders.IndexOf (ix.Id) == -1)) {

                // Retrieving currently iterated header
                msgNode.FindOrInsert ("X-Headers").Add (idxHeader.Field, idxHeader.Value);
            }
        }

        /*
         * Returns all addresses from list as name node.
         */
        private static void GetAddresses (
            ApplicationContext context, 
            Node msgNode, 
            InternetAddressList list, 
            string name)
        {
            // Looping through each address in list.
            foreach (MailboxAddress idxAdr in list) {

                // Appending currently iterated address to args.
                msgNode.FindOrInsert (name).Add (idxAdr.Name, idxAdr.Address);
            }
        }

        #endregion
    }
}

