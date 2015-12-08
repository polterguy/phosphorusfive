/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using MimeKit;
using MimeKit.Cryptography;
using MailKit;
using MailKit.Net.Smtp;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for all features regarding sending and receiving emails
/// </summary>
namespace p5.mail
{
    /// <summary>
    ///     Class wrapping the send email features of Phosphorus Five
    /// </summary>
    public static class SendMail
    {
        /// <summary>
        ///     Invoked during initial startup of application
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.core.application-start", Protection = EventProtection.NativeOpen)]
        private static void p5_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            CryptographyContext.Register (typeof (GnuPrivacyContext));
        }

        /// <summary>
        ///     Sends an email
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.smtp.send-mail", Protection = EventProtection.LambdaClosed)]
        private static void p5_smtp_send_mail (ApplicationContext context, ActiveEventArgs e)
        {
            // Creates MimeMessage according to args given
            var message = CreateAndDecorateMessage (context, e.Args);

            // Sends MimeMessage
            SendMessage (context, e.Args, message);
        }

        #region [ -- Private helper methods -- ]

        /*
         * Creates and decorates MimeMessage according to args given
         */
        private static MimeMessage CreateAndDecorateMessage (ApplicationContext context, Node args)
        {
            // Creating message to return
            var message = new MimeMessage ();

            // Getting all to/from/cc/etc addresses for message
            message.From.AddRange (GetAddresses (context, args, "from"));
            message.To.AddRange (GetAddresses (context, args, "to"));
            message.Cc.AddRange (GetAddresses (context, args, "cc"));
            message.Bcc.AddRange (GetAddresses (context, args, "bcc"));
            message.ReplyTo.AddRange (GetAddresses (context, args, "reply-to"));

            // Getting subject of message
            message.Subject = args.GetChildValue ("subject", context, "[No subject]");

            // Getting body of message using BodyBuilder (not Arnold! ;)
            var builder = new BodyBuilder ();
            if (args ["text-body"] != null)
                builder.TextBody = args.GetChildValue ("text-body", context, "");
            if (args ["html-body"] != null)
                builder.HtmlBody = args.GetChildValue ("html-body", context, "");

            // Getting resources, both linked resources and normal attachments
            GetResources (context, args, "linked-resources", builder.LinkedResources);
            GetResources (context, args, "attachments", builder.Attachments);

            // Retrieving body of message from builder
            message.Body = SignAndEncryptEntity (
                context, 
                args, 
                message.From.Mailboxes.First (), 
                message.To.Mailboxes, 
                builder.ToMessageBody ());

            // Returning message
            return message;
        }

        /*
         * Signs and encrypts MimeEntity, if requested
         */
        private static MimeEntity SignAndEncryptEntity (
            ApplicationContext context, 
            Node args, 
            MailboxAddress sender,
            IEnumerable<MailboxAddress> recipients, 
            MimeEntity entity)
        {
            // Checking if we should encrypt message
            if (args.GetChildValue<bool> ("encrypt", context, false)) {

                // Caller requested that he wished to have message encrypted, hence we encrypt, 
                using (var ctx = new GnuPrivacyContext ()) {

                    // Checking if user also wants to sign message
                    if (args.GetChildValue<string> ("sign", context, null) != null) {

                        // Setting password to retrieve signing certificate from GnuPG context
                        ctx.Password = args.GetChildValue<string> ("sign", context, null);

                        // Signing and Encrypting content of email
                        entity = MultipartEncrypted.SignAndEncrypt (ctx, sender, DigestAlgorithm.Sha1, recipients, entity);
                    } else {

                        // Encrypting content of email
                        entity = MultipartEncrypted.Encrypt (ctx, recipients, entity);
                    }
                }
            } else if (args.GetChildValue<string> ("sign", context, null) != null) {

                // Caller requested that he wished to have message signed, hence we sign
                using (var ctx = new GnuPrivacyContext ()) {

                    // Setting password to retrieve signing certificate from GnuPG context
                    ctx.Password = args.GetChildValue<string> ("sign", context, null);

                    // Signing content of email
                    entity = MultipartSigned.Create (ctx, sender, DigestAlgorithm.Sha1, entity);
                }
            }

            // Returning MimeEntity
            return entity;
        }

        /*
         * Retrieves all emails beneath the args node's name child
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
         * Retrieves all resources of specified type and puts into collection
         */
        private static void GetResources (
            ApplicationContext context, 
            Node args, 
            string name, 
            AttachmentCollection collection)
        {
            // Checking if there is a declaration for this type of resource
            if (args [name] != null) {

                // Looping through each resource of the specified type
                foreach (var idxRes in args[name].Children) {
                    collection.Add (Common.GetBaseFolder (context) + idxRes.Get<string> (context));
                }
            }
        }

        /*
         * Sends a MimeMessage
         */
        private static void SendMessage (ApplicationContext context, Node args, MimeMessage message)
        {
            // Sending message
            using (var client = new SmtpClient ()) {
                client.Connect (
                    args.GetChildValue("server", context, ""), 
                    args.GetChildValue("port", context, 25),
                    args.GetChildValue("ssl", context, true));

                // Fuck OATH2!! [quote; its creator!]
                client.AuthenticationMechanisms.Remove ("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate (
                    args.GetChildValue ("username", context, ""), 
                    args.GetChildValue ("password", context, ""));

                // Sending message
                client.Send (message);
                client.Disconnect (true);
            }
        }

        #endregion
    }
}

