/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System.Web;
using phosphorus.core;
using phosphorus.web.ui.Common;

// ReSharper disable UnusedMember.Local

namespace phosphorus.web.ui
{
    /// <summary>
    ///     helper to retrieve and set application values
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class Application
    {
        /// <summary>
        ///     sets the given application key to the nodes given as children of [pf.web.application.set]. if no nodes are given,
        ///     the application object with the given key is cleared
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.application.set")]
        private static void pf_web_application_set (ApplicationContext context, ActiveEventArgs e)
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
        ///     returns the application object given through the value of [pf.web.application.get] as a node
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.application.get")]
        private static void pf_web_application_get (ApplicationContext context, ActiveEventArgs e) { CollectionBase.Get (e.Args, context, key => HttpContext.Current.Application [key]); }

        /// <summary>
        ///     lists all application keys
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.application.list")]
        private static void pf_web_application_list (ApplicationContext context, ActiveEventArgs e) { CollectionBase.List (e.Args, context, () => HttpContext.Current.Application.AllKeys); }
    }
}