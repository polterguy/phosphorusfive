/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Web;
using phosphorus.core;
using phosphorus.web.ui.Common;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui
{
    /// <summary>
    ///     helper to retrieve and set session values
    /// </summary>
    public static class Session
    {
        /// <summary>
        ///     sets one or more session values
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.session.set")]
        private static void pf_web_session_set (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Set (e.Args, context, delegate (string key, object value) {
                if (value == null) {
                    // removing object, if it exists
                    HttpContext.Current.Session.Remove (key);
                } else {
                    // adding object
                    HttpContext.Current.Session [key] = value;
                }
            });
        }

        /// <summary>
        ///     returns one or more session values back to caller as nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.session.get")]
        private static void pf_web_session_get (ApplicationContext context, ActiveEventArgs e) { CollectionBase.Get (e.Args, context, key => HttpContext.Current.Session [key]); }

        /// <summary>
        ///     lists all session keys
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.session.list")]
        private static void pf_web_session_list (ApplicationContext context, ActiveEventArgs e) { CollectionBase.List (e.Args, context, () => (from object idx in HttpContext.Current.Session.Keys select idx.ToString ()).ToList ()); }
    }
}