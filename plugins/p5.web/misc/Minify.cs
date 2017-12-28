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
    ///     Helper to minify JavaScript and CSS.
    /// </summary>
    public static class Minify
    {
        /// <summary>
        ///     Minifies JavaScript.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.javascript.minify")]
        public static void p5_web_js_minify (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {

                // Iterating through each CSS content supplied, and minifying it, returning the results to caller.
                foreach (var idxCss in XUtil.Iterate<string> (context, e.Args)) {

                    // Minifying CSS content.
                    var minifier = new Microsoft.Ajax.Utilities.Minifier ();

                    // Returning minified content to caller.
                   e.Args.Add ("result", minifier.MinifyJavaScript (idxCss));
                }
            }
        }

        /// <summary>
        ///     Minifies CSS.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.css.minify")]
        public static void p5_web_css_minify (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new ArgsRemover (e.Args, true)) {

                // Iterating through each CSS content supplied, and minifying it, returning the results to caller.
                foreach (var idxCss in XUtil.Iterate<string> (context, e.Args)) {

                    // Minifying CSS content.
                    var minifier = new Microsoft.Ajax.Utilities.Minifier ();
                    var options = new Microsoft.Ajax.Utilities.CssSettings ();
                    options.CommentMode = Microsoft.Ajax.Utilities.CssComment.None;

                    // Returning minified content to caller.
                    e.Args.Add ("result", minifier.MinifyStyleSheet (idxCss, options));
                }
            }
        }
    }
}
