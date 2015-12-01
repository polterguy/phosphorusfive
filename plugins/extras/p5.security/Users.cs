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
    ///     Class wrapping user features of Phosphorus Five
    /// </summary>
    internal static class Users
    {
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
    }
}