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
using p5.auth.helpers;

namespace p5.auth
{
    /// <summary>
    ///     Class wrapping password related Active Events.
    /// </summary>
    static class PasswordEvents
    {
        /// <summary>
        ///     Returns boolean true if password is accepted, otherwise the friendly description for the password regime.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.is-good-password")]
        public static void p5_auth_is_good_password (ApplicationContext context, ActiveEventArgs e)
        {
            if (Passwords.IsGoodPassword (context, e.Args.GetExValue<string> (context, ""))) {

                // Password was accepted.
                e.Args.Value = true;

            } else {

                // Password was not accepted.
                var pwdRulesNode = new Node (".p5.config.get", "p5.auth.password-rules-info");
                var pwdRule = context.RaiseEvent (".p5.config.get", pwdRulesNode) [0]?.Get (context, "");
                e.Args.Value = pwdRule;
            }
        }

        /// <summary>
        ///     Changes the password for the currently logged in user.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.auth.misc.change-my-password")]
        public static void p5_auth_misc_change_my_password (ApplicationContext context, ActiveEventArgs e)
        {
            using (new ArgsRemover (e.Args, true)) {
                Passwords.ChangePassword (context, e.Args);
            }
        }
    }
}