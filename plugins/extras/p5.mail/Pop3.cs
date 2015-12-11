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
using p5.mail.mime;

namespace p5.mail
{
    /// <summary>
    ///     Class wrapping the pop3 email features of Phosphorus Five
    /// </summary>
    public static class Pop3
    {
        // Contains all "standard headers", which we handle in special cases, and should not be handled by generic header handler
        private static List<HeaderId> _excludedHeaders;

        /*
         * Static CTOR to initialize _excludedHeaders, containing list of all headers to not handle in generic handler
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
        ///     Retrieves messages from a POP3 server
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.mail.pop3.get-emails", Protection = EventProtection.LambdaClosed)]
        private static void p5_mail_pop3_get_emails (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we remove arguments supplied
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Creating our POP3 client
                using (var client = new Pop3Client ()) {

                    // Connecting to POP3 server using helper from Common class
                    Common.ConnectServer (context, client, e.Args, "pop3");

                    // Figuring out how many messages to retrieve, defaulting to "5" if not explicitly told something else by caller,
                    // making sure we never try to retrieve more messages than server actually has
                    int noMessages = Math.Min (client.Count, e.Args.GetChildValue ("count", context, 5));

                    // Fetching messages from server, but not any more messages than caller requested, or number of available messages
                    for (int idxMsg = 0; idxMsg < noMessages; idxMsg++) {

                        // Process message returned from POP3 server by building Node structure wrapping message
                        var mNode = ProcessMessage (
                            context, 
                            e.Args, 
                            client.GetMessage (idxMsg));

                        // Handle message after processing
                        HandleMessage (context, mNode, e.Args);

                        // Checking if we should delete message from server
                        if (e.Args.GetChildValue ("delete", context, false)) {

                            // Deleting message from server, making sure we wait til deletion is done before continuting execution
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
        private static Node ProcessMessage (
            ApplicationContext context, 
            Node args, 
            MimeMessage message)
        {
            // Node structure containing all headers, content, and other properties of message
            Node mNode = new Node ("envelope", message.MessageId);

            // Checking if caller does NOT want to have message processed in any ways ...
            if (!args.GetChildValue ("process-envelope", context, true)) {

                // Caller does NOT want to have message processed, returning entire message as string
                mNode.Add ("raw-envelope", message.ToString ());
            } else {

                // Processing message, headers first
                ProcessMessageHeaders (context, mNode, message);

                // Checking if caller wants to have body processed
                if (!args.GetChildValue ("process-body", context, true)) {

                    // Caller does NOT want to have content processed, returning entire Body as string
                    mNode.Add ("raw-body", message.Body.ToString ());
                } else {

                    // Then content of message
                    ParseMime.ParseMimeEntity (
                        context, 
                        mNode.Add ("body").LastChild, 
                        message.Body);
                }
            }

            // Return node containing processed message
            return mNode;
        }

        /*
         * Adds up all headers into given node
         */
        private static void ProcessMessageHeaders (
            ApplicationContext context, 
            Node mNode, 
            MimeMessage message)
        {
            // Subject
            mNode.Add ("Subject", message.Subject);

            // All address fields
            GetAddresses (context, mNode, message.From, "From");
            GetAddresses (context, mNode, message.ResentFrom, "Resent-From");
            GetAddresses (context, mNode, message.Bcc, "Bcc");
            GetAddresses (context, mNode, message.ResentBcc, "Resent-Bcc");
            GetAddresses (context, mNode, message.Cc, "Cc");
            GetAddresses (context, mNode, message.ResentCc, "Resent-Cc");
            GetAddresses (context, mNode, message.ReplyTo, "Reply-To");
            GetAddresses (context, mNode, message.ResentReplyTo, "Resent-Reply-To");
            GetAddresses (context, mNode, message.To, "To");
            GetAddresses (context, mNode, message.ResentTo, "Resent-To");

            // Other standard headers
            mNode.Add ("Date", message.Date.DateTime);

            if (!string.IsNullOrEmpty (message.ResentMessageId))
                mNode.Add ("Resent-Message-ID", message.ResentMessageId);

            if (message.Sender != null)
                mNode.Add ("Sender", null, new Node[] { new Node (message.Sender.Name ?? "", message.Sender.Address) });

            if (message.ResentSender != null)
                mNode.Add ("Resent-Sender", null, new Node[] { new Node (message.ResentSender.Name ?? "", message.ResentSender.Address) });

            if (message.MimeVersion != null)
                mNode.Add ("MIME-Version", message.MimeVersion.ToString ());

            if (message.ResentDate != DateTimeOffset.MinValue)
                mNode.Add ("Resent-Date", message.ResentDate.DateTime);

            if (message.Importance != MessageImportance.Normal)
                mNode.Add ("Importance", message.Importance.ToString ());

            if (!string.IsNullOrEmpty (message.InReplyTo))
                mNode.Add ("In-Reply-To", message.InReplyTo);

            if (message.Priority != MessagePriority.Normal)
                mNode.Add ("Priority", message.Priority.ToString ());

            // Looping through and adding all IDs for messages this message is referencing
            foreach (var id in message.References) {

                // Adding currently iterated References ID back to caller
                mNode.FindOrCreate ("References").Add (id);
            }

            // Non-standard headers
            foreach (var idxHeader in message.Headers.Where (ix => _excludedHeaders.IndexOf (ix.Id) == -1)) {

                // Retrieving currently iterated header
                mNode.FindOrCreate ("X-Headers").Add (idxHeader.Field, idxHeader.Value);
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

        /*
         * Handles message, either by returning node to caller, or invoking lambda [functor], depending upn args structure
         */
        private static void HandleMessage (
            ApplicationContext context, 
            Node mNode,
            Node args)
        {
            // Checking what to do with message, either return as [message] node, or invoke [functor] with [message] as first child
            if (args ["functor"] != null) {

                // Caller supplied [functor] object he wish to have evaluated [eval] for every message retrieved
                Node exe = args ["functor"].Clone ();

                // Making sure we avoid raising the message node as an Active Event
                mNode.Insert (0, new Node ("offset", 2 /* Remember the [offset] node itself! */));

                // Adding currently iterated message to [functor] and evaluating using [eval]
                exe.Add (mNode);
                context.RaiseLambda ("eval", exe);
            } else {

                // Returning node with message to caller
                args.Add (mNode);
            }
        }
    }
}

