/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using MimeKit;
using MailKit;
using MailKit.Net.Pop3;
using MimeKit.Cryptography;
using p5.exp;
using p5.core;
using p5.mail.helpers;

namespace p5.mail.pop3
{
    /// <summary>
    ///     Class wrapping the pop3 email features of Phosphorus Five
    /// </summary>
    public static class Get
    {
        // Contains all "standard headers", which we handle in special cases, and should not be handled by generic header handler
        private static List<HeaderId> _excludedHeaders;

        static Get ()
        {
            _excludedHeaders = new List<HeaderId> (new HeaderId [] {
                HeaderId.Bcc, HeaderId.Cc, HeaderId.Date, HeaderId.From, HeaderId.Importance, HeaderId.InReplyTo, HeaderId.MessageId, 
                HeaderId.MimeVersion, HeaderId.Priority, HeaderId.References, HeaderId.ReplyTo, HeaderId.ResentBcc, HeaderId.ResentCc, 
                HeaderId.ResentDate, HeaderId.ResentFrom, HeaderId.ResentReplyTo, HeaderId.ResentSender, HeaderId.ResentTo, 
                HeaderId.Sender, HeaderId.Subject, HeaderId.To, HeaderId.XPriority});
        }

        /// <summary>
        ///     Retrieves messages from a POP3 server
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.pop3.get", Protection = EventProtection.LambdaClosed)]
        private static void p5_mail_pop3_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we remove arguments supplied.
            // This is VERY important since passwords might be sent into this method!
            using (new p5.core.Utilities.ArgsRemover (e.Args)) {

                // Creating our POP3 client
                using (var client = new Pop3Client ()) {

                    // Connecting to POP3 server
                    client.Connect (
                        e.Args.GetChildValue ("server", context, ""), 
                        e.Args.GetChildValue ("port", context, 25),
                        e.Args.GetChildValue ("ssl", context, true));

                    // Fuck OATH2!! [quote; its creator!]
                    client.AuthenticationMechanisms.Remove ("XOAUTH2");

                    // Checking if caller supplied username, and if so, authenticate against POP3 server
                    if (e.Args ["username"] != null) {

                        // Authenticating
                        client.Authenticate (
                            e.Args.GetChildValue ("username", context, ""), 
                            e.Args.GetChildValue ("password", context, ""));
                    }

                    // Fetching messages from server, but not any more messages than caller requested, or number of available messages
                    for (int idxMsg = 0; idxMsg < Math.Min (client.Count, e.Args.GetChildValue ("count", context, client.Count)); idxMsg++) {

                        // Process message, either return in args, or invoke functor callback
                        ProcessMessage (
                            context, 
                            e.Args, 
                            client.GetMessage (idxMsg), 
                            e.Args.GetChildValue<string>("attachment-directory", context, null));

                        // Checking if we should delete message from server
                        if (e.Args.GetChildValue ("delete", context, false)) {

                            // Deleting message from POP3 server
                            client.DeleteMessageAsync (idxMsg).Wait ();
                        }
                    }

                    // Disconnecting from server, making sure we send the QUIT signal
                    client.Disconnect (true);
                }
            }
        }

        /*
         * Helper to process on message retrieved from POP3 server
         */
        private static void ProcessMessage (
            ApplicationContext context, 
            Node args, 
            MimeMessage message,
            string attachmentDirectory)
        {
            // Node structure containing all headers, content, and other properties of message
            Node mNode = new Node ("message", message.MessageId);

            // Retrieving headers
            ProcessMessageHeaders (context, mNode, message);

            // Then content, supplying password for retrieving private key to decrypt
            ProcessMimeEntity (
                context, 
                mNode, 
                message.Body, 
                args.GetChildValue<string> ("decryption-password", context, null),
                attachmentDirectory);

            // Checking what to do with message, alternatives are [functor] which will [eval] piece of code,
            // or default, which is to simply return message back to caller
            if (args ["functor"] != null) {

                // Caller supplied [functor] object he wish to have evaluated [eval] for every message retrieved
                Node exe = args ["functor"].Clone ();

                // Making sure we avoid raising the message node as an Active Event
                mNode.Insert (0, new Node ("offset", 2 /* Remember the [offset] node */));

                // Adding currently iterated message to [functor] and evaluating using [eval]
                exe.Add (mNode);
                context.RaiseLambda ("eval", exe);
            } else {

                // Returning node with message to caller
                args.Add (mNode);
            }
        }

        /*
         * Adds up all headers into given node
         */
        private static void ProcessMessageHeaders (
            ApplicationContext context, 
            Node mNode, 
            MimeMessage message)
        {
            // All address fields
            GetAddresses (context, mNode, message.From, "from");
            GetAddresses (context, mNode, message.ResentFrom, "resent-from");
            GetAddresses (context, mNode, message.Bcc, "bcc");
            GetAddresses (context, mNode, message.ResentBcc, "resent-bcc");
            GetAddresses (context, mNode, message.Cc, "cc");
            GetAddresses (context, mNode, message.ResentCc, "resent-cc");
            GetAddresses (context, mNode, message.ReplyTo, "reply-to");
            GetAddresses (context, mNode, message.ResentReplyTo, "resent-reply-to");
            GetAddresses (context, mNode, message.To, "to");
            GetAddresses (context, mNode, message.ResentTo, "resent-to");

            // Subject
            mNode.Add ("subject", message.Subject);

            // Standard headers
            mNode.Add ("date", message.Date.DateTime);

            if (!string.IsNullOrEmpty (message.ResentMessageId))
                mNode.Add ("resent-message-id", message.ResentMessageId);
            
            if (message.Sender != null)
                mNode.Add ("sender", null, new Node[] { new Node (message.Sender.Name ?? "", message.Sender.Address) });
            
            if (message.ResentSender != null)
                mNode.Add ("resent-sender", null, new Node[] { new Node (message.ResentSender.Name ?? "", message.ResentSender.Address) });
            
            if (message.MimeVersion != null)
                mNode.Add ("mime-version", message.MimeVersion.ToString ());

            if (message.ResentDate != DateTimeOffset.MinValue)
                mNode.Add ("resent-date", message.ResentDate.DateTime);
            
            if (message.Importance != MessageImportance.Normal)
                mNode.Add ("importance", message.Importance.ToString ());
            
            if (!string.IsNullOrEmpty (message.InReplyTo))
                mNode.Add ("in-reply-to", message.InReplyTo);
            
            if (message.Priority != MessagePriority.Normal)
                mNode.Add ("priority", message.Priority.ToString ());

            // Looping through and adding all IDs for messages this message is referencing
            foreach (var id in message.References) {

                // Adding currently iterated References ID back to caller
                mNode.FindOrCreate ("references").Add (id);
            }

            // Non-standard headers
            foreach (var idxHeader in message.Headers.Where (ix => _excludedHeaders.IndexOf (ix.Id) == -1)) {

                // Retrieving currently iterated header
                mNode.FindOrCreate ("x-headers").Add (idxHeader.Field, idxHeader.Value);
            }
        }

        /*
         * Processes one MimeEntity recursively
         */
        private static void ProcessMimeEntity (
            ApplicationContext context,
            Node mNode,
            MimeEntity entity,
            string decryptPassword,
            string attachmentDirectory)
        {
            // Creating node for "part"
            var nodePart = mNode.Add ("part").LastChild;

            // Checking to see if message is encrypted
            if (entity is MultipartEncrypted) {

                // Marking item as Multipart, deferring value till we know if it is ALSO signed!
                nodePart.Name = "multipart";

                // Decrypting message, transforming the currently iterated entity into result of decryption operation
                entity = DecryptEntity (
                    entity as MultipartEncrypted, 
                    decryptPassword, 
                    nodePart);

                // Invoking "self" recursively on result of decryption
                ProcessMimeEntity (
                    context, 
                    nodePart, 
                    entity, 
                    decryptPassword,
                    attachmentDirectory);
            } else if (entity is MultipartSigned) {

                // Marking item as Multipart
                nodePart.Name = "multipart";
                nodePart.Value = "signed";

                // Verifies validity of signature(s)
                VerifySignatures ((entity as MultipartSigned).Verify (), nodePart);

                // Invoking "self" recursively for each entity inside of MultiPart signed
                foreach (var idxEntity in entity as MultipartSigned) {

                    // Invoking "self" for each entity inside of MultipartSigned
                    ProcessMimeEntity (
                        context, 
                        nodePart, 
                        idxEntity, 
                        decryptPassword, 
                        attachmentDirectory);
                }
            } else if (entity is Multipart) {

                // Making sure we return this as correct type
                nodePart.Name = "multipart";
                nodePart.Value = (entity as Multipart).ContentType.MediaSubtype;

                // Looping through each MimeEntity in Multipart, invoking "self"
                foreach (var idxEntity in entity as Multipart) {

                    // Processing individual MimeEntity
                    ProcessMimeEntity (
                        context, 
                        nodePart, 
                        idxEntity, 
                        decryptPassword,
                        attachmentDirectory);
                }
            } else {

                // Building node structure to return to caller
                HandleLeafMimeEntity (context, nodePart, (MimePart)entity, attachmentDirectory);
            }
        }

        /*
         * Decrypts the given MimeEntity
         */
        private static MimeEntity DecryptEntity (
            MultipartEncrypted encrypted, 
            string decryptPassword, 
            Node nodePart)
        {
            // Decrypting entity using our GnuPG context, with the supplied password
            using (var ctx = new GnuPrivacyContext ()) {

                // Setting password to retrieve decryption private key from GnuPG context
                ctx.Password = decryptPassword;

                // Decrypting email, while also retrieving signatures for entity
                DigitalSignatureCollection signatures = null;
                MimeEntity decrypted = encrypted.Decrypt (ctx, out signatures);

                // Checking signatures, if there are any
                if (signatures != null) {

                    // Entity was signed, verifying integrity of all signatures
                    VerifySignatures (signatures, nodePart);

                    // Signaling to caller entity was signed AND encrypted
                    nodePart.Value = "signed-and-encrypted";
                } else {

                    // Signaling to caller entity was encrypted
                    nodePart.Value = "encrypted";
                }

                // Returning decrypted entity to caller
                return decrypted;
            }
        }

        /*
         * Will verify the validity of all signatures
         */
        private static void VerifySignatures (
            DigitalSignatureCollection signed,
            Node nodePart)
        {
            // Sometimes verification process will throw exceptions, at which point the signature is NOT valid.
            // Making sure we handle those cases here
            try {

                // Looping through all signatures of message
                foreach (var signature in signed) {

                    // Verifying currently iterated signature
                    if (signature.Verify ()) {

                        // Signature was valid, returning data about signing operation and certificate
                        nodePart.FindOrCreate ("verified-signatures").Add (
                            signature.SignerCertificate.Email, 
                            signature.SignerCertificate.Fingerprint,
                            new Node[] { 
                                new Node ("creation-date", signature.CreationDate), 
                                new Node ("digest-algorithm", signature.DigestAlgorithm.ToString()),
                                new Node ("public-key-algorithm", signature.PublicKeyAlgorithm.ToString()),
                                new Node ("certificate-expiration-date", signature.SignerCertificate.ExpirationDate),
                                new Node ("certificate-creation-date", signature.SignerCertificate.CreationDate),
                                new Node ("certificate-name", signature.SignerCertificate.Name)});

                        // Setting the "root boolean value" to "OK", unless previous iteration has set it to false
                        if (nodePart.FindOrCreate ("verified-signatures").Value == null)
                            nodePart.FindOrCreate ("verified-signatures").Value = true;
                    } else {

                        // Signature was NOT valid! Returning data about signing operation and certificate
                        nodePart.FindOrCreate ("invalid-signatures").Add (
                            signature.SignerCertificate.Email, 
                            signature.SignerCertificate.Fingerprint,
                            new Node[] { 
                                new Node ("creation-date", signature.CreationDate), 
                                new Node ("digest-algorithm", signature.DigestAlgorithm.ToString()),
                                new Node ("public-key-algorithm", signature.PublicKeyAlgorithm.ToString()),
                                new Node ("certificate-expiration-date", signature.SignerCertificate.ExpirationDate),
                                new Node ("certificate-creation-date", signature.SignerCertificate.CreationDate),
                                new Node ("certificate-name", signature.SignerCertificate.Name)});
                        nodePart.FindOrCreate ("verified-signatures").Value = false;
                    }
                }
            } catch (DigitalSignatureVerifyException) {

                // Signature was not valid, or could not be verified
                nodePart.FindOrCreate ("verified-signatures").Value = false;
                return;
            }
        }

        /*
         * Builds up a node structure from given MimeEntity, after MimeEntity is decrypted
         * 
         * entity can be either;
         * MessagePart (rfc88/news), MessageDeliveryStatus, MessageDispositionNotification, MessagePartial, TextPart (HTML/text),
         * TnefPart, MultipartAlternative, MultipartRelated
         * 
         * MultipartEncrypted and MultipartSigned are already handled at this point!
         */
        private static void HandleLeafMimeEntity (
            ApplicationContext context, 
            Node mNode, 
            MimePart entity,
            string attachmentDirectory)
        {
            // Linked Attachments does NOT have the "IsAttachment" property set to true for some reasons ...
            if (!string.IsNullOrEmpty (entity.FileName)) {

                // This is some sort of attachment
                string fileName = attachmentDirectory + (entity.FileName);

                // Verify user is authorised to writing to specified filename
                context.RaiseNative ("p5.io.authorize.modify-file", new Node ("p5.io.authorize.modify-file", fileName).Add ("args", mNode));
                using (FileStream stream = File.Create (Common.GetBaseFolder (context).TrimEnd ('/') + fileName)) {

                    // Saving attachment to stream
                    entity.ContentObject.DecodeTo (stream);
                }

                // Making sure caller gets to know path and type of leaf part
                mNode.Value = "attachment";
                mNode.Add ("saved-to", fileName);
                mNode.Add ("filename", entity.FileName);
            } else {

                // This is some sort of content, might still be "inline attachment"
                var textPart = entity as TextPart;
                if (textPart != null) {

                    // Adding part up as content
                    if (textPart.IsFlowed)
                        mNode.Value = "flowed";
                    else if (textPart.IsHtml)
                        mNode.Value = "html";
                    else if (textPart.IsPlain)
                        mNode.Value = "text";
                    else if (textPart.IsRichText)
                        mNode.Value = "rich";
                    mNode.FindOrCreate ("content").Value = textPart.Text;
                }
            }
        }

        /*
         * Returns all addresses from list as name node
         */
        private static void GetAddresses (
            ApplicationContext context, 
            Node args, 
            InternetAddressList list, 
            string name)
        {
            // Looping through each address in list
            foreach (MailboxAddress idxAdr in list) {

                // Appending currently iterated address to args
                args.FindOrCreate (name).Add (idxAdr.Name, idxAdr.Address);
            }
        }
    }
}

