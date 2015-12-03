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
using p5.exp.exceptions;

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
        [ActiveEvent (Name = "p5.web.set-root-password", Protection = EventProtection.LambdaClosed)]
        private static void p5_web_set_root_password (ApplicationContext context, ActiveEventArgs e)
        {
            // If root account's password is not null, then if this Active Event is 
            // invoked, it is a major security concern! Only supposed to be executed during 
            // initial installation of system
            if (!AuthenticationHelper.RootPasswordIsNull (context))
                throw new LambdaSecurityException ("[p5.web.set-root-password] was invoked for root account while root account's password was not null!", e.Args, context);

            AuthenticationHelper.SetRootPassword (context, e.Args);
        }

        /*
         * Useful during installation. Returns true if root password is null (server needs setup)
         */
        [ActiveEvent (Name = "p5.web.root-password-is-null", Protection = EventProtection.LambdaClosed)]
        private static void p5_web_root_password_is_null (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = AuthenticationHelper.RootPasswordIsNull (context);
        }
    }
}