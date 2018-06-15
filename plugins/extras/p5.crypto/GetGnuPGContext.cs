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

using p5.core;
using p5.crypto.helpers;

namespace p5.crypto
{
    /// <summary>
    ///     Class wrapping retrieval of GnuPG context.
    /// </summary>
    public static class GetGnuPGContext
    {
        /// <summary>
        ///     Returns a GnuPG context.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.crypt.get-pgp-context")]
        public static void _p5_crypt_get_pgp_context (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = new GnuPrivacyContext (e.Args.Get<bool> (context));
        }
    }
}

