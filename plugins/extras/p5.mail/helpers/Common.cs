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
            return context.Raise (".p5.core.application-folder").Get<string> (context);
        }

        /// <summary>
        ///     Connects the given server using credentials found in args
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="client">MailService type</param>
        /// <param name="args">Active Event Arguments</param>
        /// <param name="serverType">pop3 or smtp</param>
        public static void ConnectServer (
            ApplicationContext context, 
            MailService client, 
            Node args,
            string serverType)
        {
            // Retrieving server settings, defaulting to those found in web.config if not explicitly overridden
            string server = 
                args.GetChildValue<string> ("server", context) ??
                context.Raise (
                    ".p5.config.get",
                    new Node ("", string.Format ("p5.{0}.server", serverType))) [0].Get<string> (context);
            int port = args["port"] != null ? 
                args.GetChildValue<int> ("port", context) :
                context.Raise (
                    ".p5.config.get",
                    new Node ("", string.Format ("p5.{0}.port", serverType)))[0].Get<int> (context);
            bool useSsl = args["ssl"] != null ? 
                args.GetChildValue<bool> ("ssl", context) :
                context.Raise (
                    ".p5.config.get",
                    new Node ("", string.Format ("p5.{0}.use-ssl", serverType)))[0].Get<bool> (context);

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
                username = context.Raise (
                    ".p5.config.get", 
                    new Node ("", string.Format ("p5.{0}.username", serverType))) [0].Get<string> (context);
                password = context.Raise (
                    ".p5.config.get",
                    new Node ("", string.Format (".p5.{0}.password", serverType))) [0].Get<string> (context);
            }

            if (!string.IsNullOrEmpty (username)) {

                // Authenticating
                client.Authenticate (username, password);
            }
        }
    }
}

