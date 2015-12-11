/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit.Cryptography;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.mail.helpers;

/// <summary>
///     Main namespace for all features regarding sending and receiving emails
/// </summary>
namespace p5.mail
{
    /// <summary>
    ///     Class wrapping the send email features of Phosphorus Five
    /// </summary>
    public static class Smtp
    {
        /// <summary>
        ///     Sends emails
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.smtp.send-email", Protection = EventProtection.LambdaClosed)]
        private static void p5_mail_smtp_send_email (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we remove arguments supplied
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Sending message
                using (var client = new SmtpClient ()) {

                    // Connecting to SMTP server
                    Common.ConnectServer (context, client, e.Args, "smtp");

                    // Making sure we're able to post QUIT signal when done, regardless of what happens inside of this code
                    try {

                        // Loops through all [envelopes], and creates and sends as email message
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

                // Keeping track of any streams created during creation process
                var streams = new List<Stream> ();
                try {

                    // Sending currently iterated message
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

            // Retrieving [content] node of envelope, and doing basic sytntax checking
            Node body = envelopeNode["body"];
            if (body == null)
                throw new LambdaException (
                    "No [body] found inside of [envelope]",
                    envelopeNode,
                    context);

            // Making sure we pass in our streams to creator, such that we can dispose them after message is sent
            body.Value = streams;

            // Creating MIME message by using [create-native] MIME Active Event
            message.Body = context.RaiseNative ("p5.mail.mime.create-native", body)
                .Get<MimeEntity> (context);

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
            message.Subject = args.GetChildValue ("subject", context, "");

            message.From.AddRange (GetAddresses (context, args, "from"));
            message.ResentFrom.AddRange (GetAddresses (context, args, "resent-from"));
            message.To.AddRange (GetAddresses (context, args, "to"));
            message.ResentTo.AddRange (GetAddresses (context, args, "resent-to"));
            message.Cc.AddRange (GetAddresses (context, args, "cc"));
            message.ResentCc.AddRange (GetAddresses (context, args, "resent-cc"));
            message.Bcc.AddRange (GetAddresses (context, args, "bcc"));
            message.ResentBcc.AddRange (GetAddresses (context, args, "resent-bcc"));
            message.ReplyTo.AddRange (GetAddresses (context, args, "reply-to"));
            message.ResentReplyTo.AddRange (GetAddresses (context, args, "resent-reply-to"));

            if (args ["sender"] != null)
                message.Sender = new MailboxAddress (args ["sender"] [0].Name, args ["sender"] [0].Get<string> (context));
            
            if (args ["in-reply-to"] != null)
                message.InReplyTo = args.GetChildValue ("in-reply-to", context, "");
            
            if (args ["resent-message-id"] != null)
                message.ResentMessageId = args.GetChildValue ("resent-message-id", context, "");
            
            if (args ["resent-sender"] != null)
                message.ResentSender = new MailboxAddress (args ["resent-sender"] [0].Name, args ["resent-sender"] [0].Get<string> (context));
            
            if (args ["importance"] != null)
                message.Importance = (MessageImportance)Enum.Parse (typeof(MessageImportance), args.GetChildValue ("importance", context, ""));
            
            if (args ["priority"] != null)
                message.Priority = (MessagePriority)Enum.Parse (typeof(MessagePriority), args.GetChildValue ("priority", context, ""));
            
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

