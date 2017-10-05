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

namespace p5.core
{
    /// <summary>
    ///     Class used as ticket when raising Active Events.
    /// 
    ///     A "ticket" might wrap a logged in user, and the authorization/authentication context your event was raised from within.
    ///     This means you can associate authentication features and authorization features with your ApplicationContext, to verify
    ///     the user invoking your Active Event(s) has the right to perform some specific action or not.
    /// </summary>
    [Serializable]
    public class ContextTicket
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ApplicationContext+ContextTicket"/> class
        /// </summary>
        /// <param name="username">Username associated with ticket, if any</param>
        /// <param name="role">Role associated with ticket, if any</param>
        /// <param name="isDefault">If true, then the user is impersonated, and not an actual user</param>
        public ContextTicket (string username, string role, bool isDefault)
        {
            Username = username;
            Role = role;
            IsDefault = isDefault;
        }

        /// <summary>
        ///     Gets the username associated with the ticket.
        /// 
        ///     Notice, is "IsDefault" is true, then this is an impersonated user, and not a real user.
        /// </summary>
        /// <value>The username</value>
        public string Username {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the role associated with the ticket.
        /// 
        ///     Notice, is "IsDefault" is true, then this is an impersonated role, and not a real role.
        /// </summary>
        /// <value>The password</value>
        public string Role {
            get;
            private set;
        }

        /// <summary>
        ///     Gets whether or not this is an impersonated ticket or not.
        /// </summary>
        /// <value>Whethere or not user is impersonated, and not a real user object</value>
        public bool IsDefault {
            get;
            private set;
        }

        /// <summary>
        ///     Gets or sets the whitelist of legal Active Events for the current context.
        /// 
        ///     This can further restrict which authorization the given ticket has for evaluating code.
        /// </summary>
        public Node Whitelist {
            get;
            set;
        }
    }
}
