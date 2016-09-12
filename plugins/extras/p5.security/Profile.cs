/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.security.helpers;

namespace p5.security
{
    /// <summary>
    ///     Class wrapping profile features of Phosphorus Five
    /// </summary>
    internal static class Profile
    {
        /// <summary>
        ///     Logs in a user to be associated with the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "login", Protection = EventProtection.LambdaClosed)]
        public static void login (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.Login (context, e.Args);
        }

        /// <summary>
        ///     Logs out a user from the ApplicationContext
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "logout", Protection = EventProtection.LambdaClosed)]
        public static void logout (ApplicationContext context, ActiveEventArgs e)
        {
            AuthenticationHelper.Logout (context);
        }

        /// <summary>
        ///     Returns the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "whoami", Protection = EventProtection.LambdaClosed)]
        public static void whoami (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add("username", AuthenticationHelper.GetTicket (context).Username);
            e.Args.Add("role", AuthenticationHelper.GetTicket (context).Role);
            e.Args.Add("default", AuthenticationHelper.GetTicket (context).IsDefault);
        }

        /// <summary>
        ///     Returns the settings for the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "user-settings", Protection = EventProtection.LambdaClosed)]
        public static void user_settings (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.GetSettings (context, e.Args);
            }
        }

        /// <summary>
        ///     Changes the settings for the currently logged in Context user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "change-user-settings", Protection = EventProtection.LambdaClosed)]
        public static void change_user_settings (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.ChangeSettings (context, e.Args);
            }
        }

        /// <summary>
        ///     Changes the password for currently logged in user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "change-password", Protection = EventProtection.LambdaClosed)]
        public static void change_password (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.ChangePassword (context, e.Args);
            }
        }

        /// <summary>
        ///     Deletes the currently logged in user
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "delete-my-user", Protection = EventProtection.LambdaClosed)]
        public static void delete_my_user (ApplicationContext context, ActiveEventArgs e)
        {
            using (new Utilities.ArgsRemover (e.Args, true)) {
                AuthenticationHelper.DeleteMyUser (context, e.Args);
            }
        }
    }
}