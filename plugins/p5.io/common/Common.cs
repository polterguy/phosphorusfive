/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for all file operations in Phosphorus Five
/// </summary>
namespace p5.io.common
{
    /// <summary>
    ///     Helper nethods for IO operations.
    /// </summary>
    public static class Common
    {
        [ActiveEvent (Name = ".p5.io.unroll-path")]
        public static void _p5_io_unroll_path (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = GetSystemPath (context, e.Args.Get<string> (context));
        }

        /// <summary>
        ///     Returns the root folder of application pool back to caller
        /// </summary>
        /// <returns>The root folder of system</returns>
        /// <param name="context">Application Context</param>
        public static string GetRootFolder (ApplicationContext context)
        {
            return context.Raise (".p5.core.application-folder").Get<string> (context);
        }

        /// <summary>
        ///     Returns the "system path" unrolled, replacing for insance "~" with actual path.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="path">Path to unroll</param>
        /// <returns></returns>
        public static string GetSystemPath (ApplicationContext context, string path)
        {
            if (path.StartsWith ("~")) {
                return "/users/" + context.Ticket.Username + path.Substring (1);
            }
            return path;
        }

        /// <summary>
        ///     Raises the specified authorize event for given path
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="args">Root node for Active Event invoking method</param>
        /// <param name="eventName">Authorize event, will be checked against list of pre-defined event names</param>
        /// <param name="path">Path to authorize</param>
        public static void RaiseAuthorizeEvent (
            ApplicationContext context, 
            Node args, 
            string eventName, 
            string path)
        {
            switch (eventName) {
                case "read-folder":
                case "read-file":
                case "modify-folder":
                case "modify-file":
                    context.Raise (".p5.io.authorize." + eventName, new Node ("", GetSystemPath (context, path)).Add ("args", args));
                    break;
                default:
                    throw new LambdaException ("Unknown authorize event given to " + args.Name, args, context);
            }
        }
    }
}
