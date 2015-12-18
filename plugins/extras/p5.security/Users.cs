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
using p5.security.helpers;

namespace p5.security
{
    /// <summary>
    ///     Class wrapping user features of Phosphorus Five
    /// </summary>
    internal static class Users
    {
        /// <summary>
        ///     Logs in a user to be associated with the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "login", Protection = EventProtection.LambdaClosed)]
        private static void login (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.Login (context, e.Args);
        }

        /// <summary>
        ///     Logs out a user from the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "logout", Protection = EventProtection.LambdaClosed)]
        private static void logout (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.Logout (context);
        }

        /// <summary>
        ///     Creates a new user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "create-user", Protection = EventProtection.LambdaClosed)]
        private static void create_user (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.CreateUser (context, e.Args);
        }

        /// <summary>
        ///     Lists all users in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "list-users", Protection = EventProtection.LambdaClosed)]
        private static void list_users (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.ListUsers (context, e.Args);
        }

        /// <summary>
        ///     Returns the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "whoami", Protection = EventProtection.LambdaClosed)]
        private static void whoami (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add("username", AuthenticationHelper.GetTicket (context).Username);
            e.Args.Add("role", AuthenticationHelper.GetTicket (context).Role);
            e.Args.Add("default", AuthenticationHelper.GetTicket (context).IsDefault);
        }
    }
}