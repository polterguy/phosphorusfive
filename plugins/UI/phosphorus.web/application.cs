
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.web
{
    /// <summary>
    /// helper to retrieve and set application values
    /// </summary>
    public static class application
    {
        /// <summary>
        /// sets the given application key to the nodes given as children of [pf.web.application.set]. if no nodes are given,
        /// the application object with the given key is cleared
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.application.set")]
        private static void pf_web_application_set (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                if (e.Args.Count > 0)
                    HttpContext.Current.Application [idx] = e.Args.Clone ();
                else
                    HttpContext.Current.Application.Remove (idx);
            }
        }

        /// <summary>
        /// returns the application object given through the value of [pf.web.application.get] as a node
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.application.get")]
        private static void pf_web_application_get (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                Node tmp = HttpContext.Current.Application [idx] as Node;
                if (tmp != null) {
                    if (XUtil.IsExpression (e.Args.Value)) {

                        // adding key node, and values beneath key node
                        e.Args.Add (new Node (idx));
                        e.Args.LastChild.AddRange ((tmp as Node).Clone ().Children);
                    } else {

                        // since this is not an expression, we simply append values into main node
                        e.Args.AddRange ((tmp as Node).Clone ().Children);
                    }
                }
            }
        }
    }
}
