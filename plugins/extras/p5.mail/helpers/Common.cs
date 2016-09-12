/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using MailKit;

namespace p5.mail.helpers
{
    /// <summary>
    ///     Common helper class for email features of Phosphorus Five
    /// </summary>
    internal static class Common
    {
        /// <summary>
        ///     Returns base folder for application
        /// </summary>
        /// <returns>The base folder</returns>
        /// <param name="context">Application Context</param>
        public static string GetBaseFolder (ApplicationContext context)
        {
            return context.RaiseNative ("p5.core.application-folder").Get<string> (context);
        }

        /// <summary>
        ///     Connects the given server using credentials found in args
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="server">Server to connect</param>
        /// <param name="args">Active Event Arguments</param>
        public static void ConnectServer (
            ApplicationContext context, 
            MailService client, 
            Node args,
            string serverType)
        {
            // Retrieving server settings, defaulting to those found in web.config if not explicitly overridden
            string server = 
                args.GetChildValue<string> ("server", context) ??
                context.RaiseNative (string.Format ("p5.mail.get-{0}-server", serverType)).Get<string> (context);
            int port = args["port"] != null ? 
                args.GetChildValue<int> ("port", context) : 
                context.RaiseNative (string.Format ("p5.mail.get-{0}-port", serverType)).Get<int> (context);
            bool useSsl = args["ssl"] != null ? 
                args.GetChildValue<bool> ("ssl", context) : 
                context.RaiseNative (string.Format ("p5.mail.get-{0}-use-ssl", serverType)).Get<bool> (context);

            // Connecting client to server
            client.Connect (
                server, 
                port,
                useSsl);

            // Fuck OATH2!! [quote; its creator!]
            client.AuthenticationMechanisms.Remove ("XOAUTH2");

            // Finding username and password to use to authenticate
            string username = null, password = null;

            // Checking if caller supplied username and password, and if so, using those credentials
            if (args ["username"] != null) {

                // Notice, this logic allows caller to supply null or empty password, in case specified server does 
                // not require authorisation to send emails, in which case, client.Authenticate below will never be invoked!
                username = args.GetChildValue ("username", context, "");
                password = args.GetChildValue ("password", context, "");
            } else {

                // Retrieving default username/password from web.config
                username = context.RaiseNative (string.Format ("p5.mail.get-{0}-username", serverType)).Get<string> (context);
                password = context.RaiseNative (string.Format ("p5.mail.get-{0}-password", serverType)).Get<string> (context);
            }

            if (!string.IsNullOrEmpty (username)) {

                // Authenticating
                client.Authenticate (username, password);
            }
        }
    }
}

