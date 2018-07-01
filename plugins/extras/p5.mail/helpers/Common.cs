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

using p5.exp;
using p5.core;
using MailKit;

namespace p5.mail.helpers
{
    /// <summary>
    ///     Common helper class for mail features of Phosphorus Five
    /// </summary>
    static class Common
    {
        /// <summary>
        ///     Returns base folder for application
        /// </summary>
        /// <returns>The base folder</returns>
        /// <param name="context">Application Context</param>
        public static string GetBaseFolder (ApplicationContext context)
        {
            return context.RaiseEvent (".p5.core.application-folder").Get<string> (context);
        }

        /// <summary>
        ///     Connects the given server using credentials found in args
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="client">MailService type</param>
        /// <param name="args">Active Event Arguments</param>
        /// <param name="serverType">pop3 or smtp</param>
        public static void ConnectServer (ApplicationContext context, MailService client, Node args, string serverType)
        {
            // Retrieving server settings, defaulting to those found in web.config, if not explicitly overridden.
            var server = args.GetExChildValue ("server", context, "localhost");
            var port = args.GetExChildValue ("port", context, 25);
            bool useSsl = args.GetExChildValue ("ssl", context, false);

            // Connecting client to server
            client.Connect (
                server,
                port,
                useSsl);

            // Fuck OATH2!! [quote; its creator!]
            client.AuthenticationMechanisms.Remove ("XOAUTH2");

            // Authenticating user, if credentials were supplied.
            var username = args.GetExChildValue ("username", context, "");
            if (!string.IsNullOrEmpty (username)) {
                client.Authenticate (username, args.GetExChildValue ("password", context, ""));
            }
        }
    }
}

