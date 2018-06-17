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
using p5.exp.exceptions;
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping initialization of server Active Events.
    ///     This implies events that are for some reasons necessary to setup your Phosphorus Five server.
    /// </summary>
    static class InitializeServerEvents
    {
        /// <summary>
        ///     Returns the server salt.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.auth.get-server-salt")]
        static void _p5_auth_get_server_salt (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = ServerSalt.GetServerSalt (context);
        }
        
        /// <summary>
        ///     Returns true if a server salt has already been created.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.auth._has-server-salt")]
        static void p5_auth__has_server_salt (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = !string.IsNullOrEmpty (ServerSalt.GetServerSalt (context));
        }
        
        /// <summary>
        ///     Sets the server salt for the server.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.auth._set-server-salt")]
        static void p5_auth__set_server_salt (ApplicationContext context, ActiveEventArgs e)
        {
            ServerSalt.SetServerSalt (context, e.Args, e.Args.GetExValue<byte[]> (context));
        }
        
        /// <summary>
        ///     Returns true if a server has been initialized with a GnuPGP fingerprint.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.auth._has-server-pgp-key")]
        static void p5_auth__has_server_pgp_key (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = !string.IsNullOrEmpty (PGPKey.GetFingerprint (context));
        }
        
        /// <summary>
        ///     Sets the server PGP key's fingerprint.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.auth._set-server-pgp-key")]
        static void p5_auth__set_server_pgp_key (ApplicationContext context, ActiveEventArgs e)
        {
            PGPKey.SetFingerprint (context, e.Args, e.Args.GetExValue<string> (context));
        }
        
        /// <summary>
        ///     Returns the server's PGP key's fingerprint.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.auth.pgp.get-fingerprint")]
        static void p5_auth_pgp_get_fingerprint (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = PGPKey.GetFingerprint (context);
        }
        
        /// <summary>
        ///     Changes the PGP key that the auth file is persisted with.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.auth.pgp.change-server-key")]
        static void p5_auth_pgp_change_server_key (ApplicationContext context, ActiveEventArgs e)
        {
            if (context.Ticket.Role != "root")
                throw new LambdaSecurityException ("Only a root account can change the server's PGP keypair", e.Args, context);
            PGPKey.ChangeServerPGPKey (context, e.Args.Get<string> (context), e.Args);
        }
        
        /// <summary>
        ///     Returns true if root password is not previously set.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        [ActiveEvent (Name = "p5.auth._root-password-is-null")]
        public static void p5_auth__root_password_is_null (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = !Root.HasRootAccount (context);
        }

        /// <summary>
        ///     Sets the root password of the server.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        [ActiveEvent (Name = "p5.auth._set-root-password")]
        public static void p5_auth_set_root_password (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * If root account's password is not null, and this Active Event is 
             * invoked, it is a major security concern! This Active Event is only
             * supposed to be raised during installation of system!
             */
            if (Root.HasRootAccount (context))
                throw new LambdaSecurityException ("[p5.auth._set-root-password] was invoked for root account while root account's password was not null!", e.Args, context);

            Root.SetRootPassword (context, e.Args);
        }
    }
}