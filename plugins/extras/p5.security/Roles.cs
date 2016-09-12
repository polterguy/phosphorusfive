/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.security.helpers;

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
        [ActiveEvent (Name = "list-roles", Protection = EventProtection.LambdaClosed)]
        public static void list_roles (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.GetRoles (context, e.Args);
        }
    }
}