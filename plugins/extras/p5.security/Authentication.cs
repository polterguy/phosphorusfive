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
        ///     Sink to associate a Ticket with ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.core.initialize-application-context", Protection = EventProtection.NativeOpen)]
        private static void p5_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            if (!AuthenticationHelper.TryLoginFromPersistentCookie (context))
                context.UpdateTicket (AuthenticationHelper.GetTicket (context));
        }

        /// <summary>
        ///     Returns the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "get-user", Protection = EventProtection.LambdaClosed)]
        private static void get_user (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add("username", AuthenticationHelper.GetTicket (context).Username);
            e.Args.Add("role", AuthenticationHelper.GetTicket (context).Role);
            e.Args.Add("default", AuthenticationHelper.GetTicket (context).IsDefault);
        }

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
    }
}