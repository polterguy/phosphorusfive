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

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp.exceptions;
using p5.mail.helpers;
using MimeKit;
using MailKit.Net.Smtp;

namespace p5.mail
{
    /// <summary>
    ///     Class wrapping the SMTP email features of Phosphorus Five
    /// </summary>
    public static class Smtp
    {
        /// <summary>
        ///     Sends emails
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.smtp.send-email")]
        public static void p5_smtp_send_email (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (e.Args.Children.Count (ix => ix.Name == "envelope") == 0)
                throw new LambdaException (
                    "No [envelope] nodes found",
                    e.Args,
                    context);

            // Making sure we remove arguments supplied
            using (new ArgsRemover (e.Args, true)) {

                // Creating our SMTP client
                using (var client = new SmtpClient ()) {

                    // Connecting to SMTP server
                    Common.ConnectServer (context, client, e.Args, "smtp");

                    // Making sure we're able to post QUIT signal when done, regardless of what happens inside of this code
                    try {

                        // Loops through all [envelopes], and creates and sends as email message over given client
                        SendMessages (context, e.Args, client);
                    } finally {

                        // Disconnecting client, making sure we send QUIT signal
                        client.Disconnect (true);
                    }
                }
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Sends all [envelopes] found
         */
        private static void SendMessages (
            ApplicationContext context, 
            Node args, 
            SmtpClient client)
        {
            // Looping through each message caller wants to send
            foreach (var idxEnvelopeNode in args.Children.Where (ix => ix.Name == "envelope")) {

                // Keeping track of any streams created during creation process of message
                var streams = new List<Stream> ();
                try {

                    // Creating and sending our currently iterated message
                    client.Send (CreateMessage (context, idxEnvelopeNode, streams));
                } finally {

                    // Disposing all streams created during process of creating message
                    foreach (var idxStream in streams) {

                        // Closing and disposing currently iterated stream
                        idxStream.Close ();
                        idxStream.Dispose ();
                    }
                }
            }
        }

        /*
         * Creates and decorates MimeMessage according to given args
         */
        private static MimeMessage CreateMessage (
            ApplicationContext context, 
            Node envelopeNode,
            List<Stream> streams)
        {
            // Creating message to return
            var message = new MimeMessage ();

            // Deocrates headers of email
            DecorateMessageEnvelope (context, envelopeNode, message);

            // Retrieving [body] node of envelope, and doing basic syntax checking
            Node body = envelopeNode["body"];
            if (body == null)
                throw new LambdaException (
                    "No [body] found inside of [envelope]",
                    envelopeNode,
                    context);

            // Making sure we pass in our streams to creator, such that we can dispose them after message is sent
            body.Value = streams;

            // Creating MIME message by using [create-native] MIME Active Event
            message.Body = context.RaiseEvent (".p5.mime.create-native", body).Get<MimeEntity> (context);

            // Returning message
            return message;
        }

        /*
         * Decorates headers of MimeMessage
         */
        static void DecorateMessageEnvelope (
            ApplicationContext context, 
            Node args, 
            MimeMessage message)
        {
            message.Subject = args.GetChildValue ("Subject", context, "");

            message.From.AddRange (GetAddresses (context, args, "From"));
            message.ResentFrom.AddRange (GetAddresses (context, args, "Resent-From"));
            message.To.AddRange (GetAddresses (context, args, "To"));
            message.ResentTo.AddRange (GetAddresses (context, args, "Resent-To"));
            message.Cc.AddRange (GetAddresses (context, args, "Cc"));
            message.ResentCc.AddRange (GetAddresses (context, args, "Resent-Cc"));
            message.Bcc.AddRange (GetAddresses (context, args, "Bcc"));
            message.ResentBcc.AddRange (GetAddresses (context, args, "Resent-Bcc"));
            message.ReplyTo.AddRange (GetAddresses (context, args, "Reply-To"));
            message.ResentReplyTo.AddRange (GetAddresses (context, args, "Resent-Reply-To"));

            if (args ["Sender"] != null)
                message.Sender = new MailboxAddress (args ["Sender"] [0].Name, args ["Sender"] [0].Get<string> (context));
            
            if (args ["In-Reply-To"] != null)
                message.InReplyTo = args.GetChildValue ("In-Reply-To", context, "");
            
            if (args ["Resent-Message-ID"] != null)
                message.ResentMessageId = args.GetChildValue ("Resent-Message-Id", context, "");
            
            if (args ["Resent-Sender"] != null)
                message.ResentSender = new MailboxAddress (args ["Resent-Sender"] [0].Name, args ["Resent-Sender"] [0].Get<string> (context));
            
            if (args ["Importance"] != null)
                message.Importance = (MessageImportance)Enum.Parse (typeof(MessageImportance), args.GetChildValue ("Importance", context, ""));
            
            if (args ["Priority"] != null)
                message.Priority = (MessagePriority)Enum.Parse (typeof(MessagePriority), args.GetChildValue ("Priority", context, ""));
            
            if (args ["headers"] != null) {
                
                // Looping through all custom headers in message, adding them to message
                foreach (var idxHeader in args ["headers"].Children) {
                    message.Headers.Add (new Header (idxHeader.Name, idxHeader.Get<string> (context)));
                }
            }
        }

        /*
         * Retrieves all emails beneath the args node's child with the given name
         */
        private static IEnumerable<MailboxAddress> GetAddresses (
            ApplicationContext context, 
            Node args, 
            string name)
        {
            // Checking there exist a node with supplied name on args
            if (args [name] != null) {

                // Returning all emails
                return args[name].Children.Select (ix => new MailboxAddress (ix.Name, ix.Get<string>(context)));
            }

            // No addresses for this request
            return new MailboxAddress[] { };
        }

        #endregion
    }
}

