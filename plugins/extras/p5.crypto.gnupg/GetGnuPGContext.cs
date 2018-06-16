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

using MimeKit.Cryptography;
using p5.core;
using p5.crypto.gnupg.helpers;

namespace p5.crypto.gnupg
{
    /// <summary>
    ///     Class wrapping retrieval of GnuPG context.
    /// </summary>
    public static class GetGnuPGContext
    {
        /// <summary>
        ///     Invoked during initial startup of application.
        ///     Registers cryptography context (GnuPG).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.core.application-start")]
        static void _p5_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // Registering our Cryptography context, which is wrapping the local installation of Gnu Privacy Guard
            CryptographyContext.Register (typeof (GnuPrivacyContext));
        }

        /// <summary>
        ///     Returns a PGP context.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.crypto.pgp-keys.context.create")]
        public static void _p5_crypto_pgp_keys_context_create (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = new GnuPrivacyContext (
                e.Args.Get<bool> (context), 
                e.Args.GetChildValue<string> ("password", context, null));
        }
    }
}

