/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

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
        ///     Creates a new user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "create-user")]
        public static void create_user (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.CreateUser (context, e.Args);
        }

        /// <summary>
        ///     Edits an existing user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "edit-user")]
        public static void edit_user (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.EditUser (context, e.Args);
        }

        /// <summary>
        ///     Lists all users in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "list-users")]
        public static void list_users (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.ListUsers (context, e.Args);
        }

        /// <summary>
        ///     Retrieves a specific user in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "get-user")]
        public static void get_user (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.GetUser (context, e.Args);
            }
        }

        /// <summary>
        ///     Deletes a specific user in system
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "delete-user")]
        public static void delete_user (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.DeleteUser (context, e.Args);
        }
    }
}