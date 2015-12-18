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

/// <summary>
///     Main namespace for security features of Phosphorus Five
/// </summary>
namespace p5.security
{
    /// <summary>
    ///     Class wrapping authentication features of Phosphorus Five
    /// </summary>
    internal static class Common
    {
        /// <summary>
        ///     Sink to associate a Ticket with ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.core.initialize-application-context", Protection = EventProtection.NativeOpen)]
        private static void p5_core_initialize_application_context (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if session is null, which it might be, during for instance [p5.core.application-start]
            if (HttpContext.Current.Session != null) {

                // Checking if ContextTicket is already set, and if not, we try to login user from persistent cookie
                if (!AuthenticationHelper.ContextTicketIsSet) {

                    // No Context Ticket, try to login user from persistent cookie
                    AuthenticationHelper.TryLoginFromPersistentCookie (context);
                }

                // Updating Application Context ticket with ticket from AuthenticationHelper
                context.UpdateTicket (AuthenticationHelper.GetTicket (context));
            }
        }
    }
}