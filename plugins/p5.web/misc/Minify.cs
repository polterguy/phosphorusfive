/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.ui.misc
{
    /// <summary>
    ///     Minifies given JavaScript.
    /// </summary>
    public static class Minify
    {
        /// <summary>
        ///     Returns one or more HTTP request header(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.javascript.minify")]
        public static void p5_web_js_minify (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args)) {

                // Retrieves JavaScript, minifies it, and returning it to caller.
                var js = e.Args.GetExValue (context, "");
                var minifier = new Microsoft.Ajax.Utilities.Minifier ();
                e.Args.Value = minifier.MinifyJavaScript (js);
            }
        }

        /// <summary>
        ///     Returns one or more HTTP request header(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.css.minify")]
        public static void p5_web_css_minify (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args)) {

                // Retrieves CSS, minifies it, and returning it to caller.
                var css = e.Args.GetExValue (context, "");
                var minifier = new Microsoft.Ajax.Utilities.Minifier ();

                // Turning off ALL comments.
                var options = new Microsoft.Ajax.Utilities.CssSettings ();
                options.CommentMode = Microsoft.Ajax.Utilities.CssComment.None;
                e.Args.Value = minifier.MinifyStyleSheet (css, options);
            }
        }
    }
}
