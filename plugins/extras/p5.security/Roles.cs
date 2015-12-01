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

namespace p5.security
{
    /// <summary>
    ///     Class wrapping role features of Phosphorus Five
    /// </summary>
    internal static class Roles
    {
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
    }
}