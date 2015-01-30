
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Globalization;
using System.Security.Cryptography;
using phosphorus.core;
using phosphorus.lambda;
using phosphorus.ajax.widgets;

namespace phosphorus.web
{
    /// <summary>
    /// helper to retrieve and set session values
    /// </summary>
    public static class session
    {
        /// <summary>
        /// sets one or more session values
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.session.set")]
        private static void pf_web_session_set (ApplicationContext context, ActiveEventArgs e)
        {
            Expression.Iterate<string> (e.Args, false, 
            delegate (string idx) {
                if (e.Args.Count > 0)
                    HttpContext.Current.Session [idx] = e.Args.Clone ();
                else
                    HttpContext.Current.Session.Remove (idx);
            });
        }

        /// <summary>
        /// returns one or more session values back to caller as nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.session.get")]
        private static void pf_web_session_get (ApplicationContext context, ActiveEventArgs e)
        {
            Expression.Iterate<string> (e.Args, false, 
            delegate (string idx) {
                Node tmp = HttpContext.Current.Session [idx] as Node;
                if (tmp != null) {
                    if (Expression.IsExpression (e.Args.Value)) {

                        // adding key node, and values beneath key node
                        e.Args.Add (new Node (idx));
                        e.Args.LastChild.AddRange ((tmp as Node).Clone ().Children);
                    } else {

                        // since this is not an expression, we simply append values into main node
                        e.Args.AddRange ((tmp as Node).Clone ().Children);
                    }
                }
            });
        }
    }
}
