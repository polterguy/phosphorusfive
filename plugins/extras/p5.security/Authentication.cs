/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Security;
using System.Configuration;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.core.configuration;

/// <summary>
///     Main namespace for security features of Phosphorus Five
/// </summary>
namespace p5.security
{
    /// <summary>
    ///     Class wrapping authentication features of Phosphorus Five
    /// </summary>
    internal static class Authentication
    {
        /// <summary>
        ///     Sink to associate a Ticket with ApplicationContext and initialize ApplicationContext object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.core.initialize-application-context", Protected = true)]
        private static void p5_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            // Try to login user from persistent cookie
            AuthenticationHelper.TryLoginFromPersistentCookie (context);
        }

        /// <summary>
        ///     Logs in a user to be associated with the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "login", Protected = true)]
        private static void login (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.Login (context, e.Args);
        }

        /// <summary>
        ///     Logs out a user from the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "logout", Protected = true)]
        private static void logout (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.Logout (context);
        }

        /// <summary>
        ///     Returns the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "user", Protected = true)]
        private static void user (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add("username", AuthenticationHelper.Ticket.Username);
            e.Args.Add("role", AuthenticationHelper.Ticket.Role);
            e.Args.Add("default", AuthenticationHelper.Ticket.IsDefault);
        }

        /// <summary>
        ///     Creates a new user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "create-user", Protected = true)]
        private static void create_user (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.CreateUser (context, e.Args);
        }

        /// <summary>
        ///     Returns all roles in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "roles", Protected = true)]
        private static void roles (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.GetRoles (context, e.Args);
        }

        /*
         * Invoked during installation. Sets root password, but only if existing password is null!
         */
        [ActiveEvent (Name = "p5.web.set-root-password")]
        private static void p5_web_set_root_password (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.SetRootPassword (context, e.Args);
        }

        /*
         * Invoked during installation. Returns true if root password is null (server needs setup)
         */
        [ActiveEvent (Name = "p5.web.root-password-is-null", Protected = true)]
        private static void p5_web_root_password_is_null (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = AuthenticationHelper.RootPasswordIsNull (context);
        }
    }
}