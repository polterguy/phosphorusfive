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

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Security;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.auth.helpers
{
    /// <summary>
    ///     Class wrapping server salt features of Phosphorus Five.
    /// </summary>
    static class ServerSalt
    {
        /*
         * Returns server-salt for application.
         */
        public static string GetServerSalt (ApplicationContext context)
        {
            // Retrieving "auth" file in node format.
            return AuthFile.GetAuthFile (context).GetChildValue<string> ("server-salt", context);
        }

        /*
         * Sets the server salt for server.
         */
        public static void SetServerSalt (ApplicationContext context, Node args, byte[] salt)
        {
            using (var sha512 = SHA512.Create ()) {
                salt = sha512.ComputeHash (salt);
            }
            AuthFile.ModifyAuthFile (context, delegate (Node node) {
                if (node.Children.Any (ix => ix.Name == "server-salt"))
                    throw new LambdaSecurityException ("Tried to change server salt after initial creation", args, context);
                node.Add ("server-salt", salt);
            });
        }
    }
}