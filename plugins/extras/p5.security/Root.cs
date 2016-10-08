/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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
using p5.exp.exceptions;
using p5.security.helpers;

namespace p5.security
{
    /// <summary>
    ///     Class wrapping "root user" features of Phosphorus Five
    /// </summary>
    internal static class Root
    {
        /*
         * Useful during installation. Sets root password, but only if existing password is null!
         */
        [ActiveEvent (Name = "p5.security.set-root-password")]
        public static void p5_security_set_root_password (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * If root account's password is not null, and this Active Event is 
             * invoked, it is a major security concern! This Active Event is only
             * supposed to be raised during installation of system!
             */
            if (!AuthenticationHelper.NoExistingRootAccount (context))
                throw new LambdaSecurityException ("[p5.security.set-root-password] was invoked for root account while root account's password was not null!", e.Args, context);

            AuthenticationHelper.SetRootPassword (context, e.Args);
        }

        /*
         * Useful during installation. Sets root password, but only if existing password is null!
         */
        [ActiveEvent (Name = "p5.security.root-password-is-null")]
        public static void p5_security_root_password_is_null (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = AuthenticationHelper.NoExistingRootAccount (context);
        }
    }
}