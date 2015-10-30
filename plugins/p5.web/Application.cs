/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using p5.core;
using p5.web.ui.common;

/// <summary>
///     Main namespace for everything related to Web User Interface.
/// 
///     Contains helper classes for creating a web User Interface, such as creation of Ajax Web Widgets, retrieving
///     and setting items in your Session object, etc.
/// </summary>
namespace p5.web.ui
{
    /// <summary>
    ///     Helper to retrieve and set Application values.
    /// 
    ///     Allows for you to retrieve and set items in your Application object.
    /// 
    ///     The Application object is a "global shared" object between all sessions, visitors and user of your web site, 
    ///     and allows for you to share information between different users of your web site.
    /// </summary>
    public static class Application
    {
        /// <summary>
        ///     Sets one or more Application object(s).
        /// 
        ///     Where [source], or [src], becomes the nodes that are stored in the application. The main node's value(s), becomes
        ///     the key your items are stored with.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.web.set-application")]
        private static void p5_web_set_application (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Set (e.Args, context, delegate (string key, object value) {
                if (value == null) {
                    // removing object, if it exists
                    HttpContext.Current.Application.Remove (key);
                } else {
                    // adding object
                    HttpContext.Current.Application [key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves Application object(s).
        /// 
        ///     Supply one or more keys to which items you wish to retrieve as the value of your main node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.web.get-application")]
        private static void p5_web_get_application (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Get (e.Args, context, key => HttpContext.Current.Application [key]);
        }

        /// <summary>
        ///     Lists all keys in the Application object.
        /// 
        ///     Returns all keys for all items in your Application object.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.web.list-application")]
        private static void p5_web_application_list (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (e.Args, context, () => HttpContext.Current.Application.AllKeys);
        }
    }
}