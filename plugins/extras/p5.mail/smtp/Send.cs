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
using p5.mail.helpers;

/// <summary>
///     Main namespace for all features regarding sending and receiving emails
/// </summary>
namespace p5.mail.smtp
{
    /// <summary>
    ///     Class wrapping the send email features of Phosphorus Five
    /// </summary>
    public static class Send
    {
        /// <summary>
        ///     Sends an email
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.smtp.send", Protection = EventProtection.LambdaClosed)]
        private static void p5_mail_smtp_send (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we remove arguments supplied
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Making sure we keep track of all streams created during process
                List<Stream> streams = new List<Stream> ();
                try {

                    // Creates, decorates and send a MimeMessage according to given args
                    SendMessage (context, e.Args, CreateAndDecorateMessage (context, e.Args, streams));
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

        #region [ -- Private helper methods -- ]

        /*
         * Creates and decorates MimeMessage according to given args
         */
        private static MimeMessage CreateAndDecorateMessage (
            ApplicationContext context, 
            Node args,
            List<Stream> streams)
        {
            // Creating message to return
            var message = new MimeMessage ();

            // Getting subject of message
            message.Subject = args.GetChildValue ("subject", context, "[No subject]");

            // Deocrates headers of email
            DecorateHeaders (context, args, message);

            // Creating MIME message
            args["message"].Value = streams;
            message.Body = context.RaiseNative ("p5.mail.mime.create-native", args["message"]).Get<MimeEntity> (context);

            // Returning message
            return message;
        }

        /*
         * Decorates headers of MimeMessage
         */
        static void DecorateHeaders (
            ApplicationContext context, 
            Node args, 
            MimeMessage message)
        {
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

        /*
         * Sends a MimeMessage
         */
        private static void SendMessage (
            ApplicationContext context, 
            Node args, 
            MimeMessage message)
        {
            // Sending message
            using (var client = new SmtpClient ()) {

                // Connecting to SMTP server
                client.Connect (
                    args.GetChildValue("server", context, ""), 
                    args.GetChildValue("port", context, 25),
                    args.GetChildValue("ssl", context, true));

                // Fuck OATH2!! [quote; its creator!]
                client.AuthenticationMechanisms.Remove ("XOAUTH2");

                // Checking if caller supplied username, and if so, authenticate against SMTP server
                if (args ["username"] != null) {

                    // Authenticating
                    client.Authenticate (
                        args.GetChildValue ("username", context, ""), 
                        args.GetChildValue ("password", context, ""));
                }

                // Sending message
                client.Send (message);
                client.Disconnect (true);
            }
        }

        #endregion
    }
}

