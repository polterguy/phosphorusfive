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
using p5.exp.exceptions;
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping "root user" Active Events.
    /// </summary>
    static class Root
    {
        /// <summary>
        ///     Invoked only once, during setup of system.
        ///     If root password is set previously, this event will throw an exception.
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
            if (!AuthenticationHelper.NoExistingRootAccount (context))
                throw new LambdaSecurityException ("[p5.auth._set-root-password] was invoked for root account while root account's password was not null!", e.Args, context);

            AuthenticationHelper.SetRootPassword (context, e.Args);
        }

        /// <summary>
        ///     Returns true if root password is not previously set.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        [ActiveEvent (Name = "p5.auth._root-password-is-null")]
        public static void p5_auth__root_password_is_null (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = AuthenticationHelper.NoExistingRootAccount (context);
        }
    }
}