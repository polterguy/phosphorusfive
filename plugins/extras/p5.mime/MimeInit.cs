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
using p5.mime.helpers;
using MimeKit.Cryptography;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the common MIME features of Phosphorus Five
    /// </summary>
    public static class MimeInit
    {
        /// <summary>
        ///     Invoked during initial startup of application. Registers cryptography context (GnuPG)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = ".p5.core.application-start")]
        static void _p5_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // Registering our Cryptography context, which is wrapping the local installation of Gnu Privacy Guard
            CryptographyContext.Register (typeof (GnuPrivacyContext));
        }
    }
}

