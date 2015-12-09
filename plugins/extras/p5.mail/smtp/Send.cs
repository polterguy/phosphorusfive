/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
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
namespace p5.smtp
{
    /// <summary>
    ///     Class wrapping the send email features of Phosphorus Five
    /// </summary>
    public static class Send
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
        [ActiveEvent (Name = "p5.mail.smtp.send", Protection = EventProtection.LambdaClosed)]
        private static void p5_mail_smtp_send (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we remove arguments supplied, 
            // VERY important since passwords might be sent into this method!
            using (new p5.core.Utilities.ArgsRemover (e.Args)) {

                // Creates MimeMessage according to args given
                var message = CreateAndDecorateMessage (context, e.Args);

                // Sends MimeMessage
                SendMessage (context, e.Args, message);
            }
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
            message.ResentFrom.AddRange (GetAddresses (context, args, "resent-from"));

            message.To.AddRange (GetAddresses (context, args, "to"));
            message.ResentTo.AddRange (GetAddresses (context, args, "resent-to"));

            message.Cc.AddRange (GetAddresses (context, args, "cc"));
            message.ResentCc.AddRange (GetAddresses (context, args, "resent-cc"));

            message.Bcc.AddRange (GetAddresses (context, args, "bcc"));
            message.ResentBcc.AddRange (GetAddresses (context, args, "resent-bcc"));

            message.ReplyTo.AddRange (GetAddresses (context, args, "reply-to"));
            message.ResentReplyTo.AddRange (GetAddresses (context, args, "resent-reply-to"));

            // Getting subject of message
            message.Subject = args.GetChildValue ("subject", context, "[No subject]");

            // Setting "Sender" header, if given
            if (args ["sender"] != null)
                message.Sender = new MailboxAddress (args ["sender"] [0].Name, args ["sender"] [0].Get<string> (context));

            // Setting "In-Reply-To" header, if given
            if (args ["in-reply-to"] != null)
                message.InReplyTo = args.GetChildValue ("in-reply-to", context, "");

            // Setting "Resent-MessageID" header, if given
            if (args ["resent-message-id"] != null)
                message.ResentMessageId = args.GetChildValue ("resent-message-id", context, "");

            // Setting "Resent-Sender" header, if given
            if (args ["resent-sender"] != null)
                message.ResentSender = new MailboxAddress (args ["resent-sender"] [0].Name, args ["resent-sender"] [0].Get<string> (context));

            // Setting "Importance" header, if given
            if (args ["importance"] != null)
                message.Importance = (MessageImportance)Enum.Parse (typeof(MessageImportance), args.GetChildValue ("importance", context, ""));

            // Setting "Priority" header, if given
            if (args ["priority"] != null)
                message.Priority = (MessagePriority)Enum.Parse (typeof(MessagePriority), args.GetChildValue ("priority", context, ""));

            // Checking if there are any "custom headers" in message
            if (args ["headers"] != null) {

                // Looping through all custom headers in message, adding them to message
                foreach (var idxHeader in args ["headers"].Children) {
                    message.Headers.Add (new Header (idxHeader.Name, idxHeader.Get<string> (context)));
                }
            }

            // Getting bodies (HTML and Text) of message using BodyBuilder
            var builder = new BodyBuilder ();
            builder.TextBody = args.GetChildValue<string> ("body-text", context, null);
            builder.HtmlBody = args.GetChildValue<string> ("body-html", context, null);

            // Getting resources, both linked resources and normal attachments
            GetResources (context, args, "linked-resources", builder.LinkedResources);
            GetResources (context, args, "attachments", builder.Attachments);

            // Signing and encrypting message if we should
            message.Body = SignAndEncryptEntity (
                context, 
                args, 
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
            IEnumerable<MailboxAddress> recipients, 
            MimeEntity entity)
        {
            // Getting MailboxAddress to use for signing, if any
            MailboxAddress signersEmail = null;
            string signingPrivateKeyPassword = null;
            DigestAlgorithm algo = DigestAlgorithm.Sha1;
            if (args ["signature"] != null) {

                // Finding name to use for signing
                string nameToSignFor = args ["signature"].Get<string> (context);

                // Finding email to use for signing
                string emailToSignFor = args ["signature"].GetChildValue<string> ("email", context);

                // Finding thumbprint to use for signing, which overrides email!
                string keyFingerPrint = args ["signature"].GetChildValue<string> ("fingerprint", context);

                // Creating our signing MailboxAddress
                if (!string.IsNullOrEmpty (keyFingerPrint))
                    signersEmail = new SecureMailboxAddress (nameToSignFor, emailToSignFor ?? "", keyFingerPrint);
                else
                    signersEmail = new MailboxAddress (nameToSignFor, emailToSignFor);

                // Figuring out which DigestAlgorithm to use (defaulting to Sha1)
                if (args ["signature"] ["digest-algorithm"] != null)
                    algo = (DigestAlgorithm)Enum.Parse (typeof(DigestAlgorithm), args ["signature"] ["digest-algorithm"].Get<string> (context));

                // Setting password to retrieve signing certificate from GnuPG context
                signingPrivateKeyPassword = (args["signature"] ["fingerprint"] ?? args["signature"] ["email"])["password"].Get<string> (context);

            }

            // Checking if we should encrypt message with public certificate beloning to recipients
            if (args.GetChildValue<bool> ("encrypt", context, false)) {

                // Caller requested that he wished to have message encrypted, hence we encrypt, 
                using (var ctx = new GnuPrivacyContext ()) {

                    // Checking if user also wants to sign message
                    if (args["signature"] != null) {

                        // Setting password to retrieve signing certificate from GnuPG context
                        ctx.Password = signingPrivateKeyPassword;

                        // Signing and Encrypting content of email
                        entity = MultipartEncrypted.SignAndEncrypt (
                            ctx, 
                            signersEmail, 
                            algo, 
                            recipients, 
                            entity);
                    } else {

                        // Encrypting content of email, without any signatures
                        entity = MultipartEncrypted.Encrypt (ctx, recipients, entity);
                    }
                }
            } else if (args.GetChildValue<string> ("signature", context, null) != null) {

                // Caller requested that he wished to have message signed, hence we sign
                using (var ctx = new GnuPrivacyContext ()) {

                    // Setting password to retrieve signing certificate from GnuPG context
                    ctx.Password = signingPrivateKeyPassword;

                    // Signing content of email
                    entity = MultipartSigned.Create (
                        ctx, 
                        signersEmail,
                        algo, 
                        entity);
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

