/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

using p5.exp;
using p5.core;
using CommonMark;

namespace p5.markdown
{
    /// <summary>
    ///     Class to help transform Markdown to HTML
    /// </summary>
    public static class Markdown
    {
		/// <summary>
		///     Parses a Markdown snippet, and creates an HTML snippet from the result.
		/// </summary>
		/// <param name="context">Application Context</param>
		/// <param name="e">Parameters passed into Active Event</param>
		[ActiveEvent(Name = "markdown2html")]
		[ActiveEvent(Name = "p5.markdown.markdown2html")]
		public static void markdown2html (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args, false)) {

                // Assumes there's only one document, or creates one result of it.
                var md = XUtil.Single<string> (context, e.Args);

                // Making sure we correctly resolve URLs, if user specified a [root-url] argument.
                var root = e.Args.GetExChildValue ("root-url", context, "");
                CommonMarkSettings settings = CommonMarkSettings.Default;
				if (root != "") {

                    // Unrolling path.
                    root = context.RaiseEvent ("p5.io.unroll-path", new Node ("", root)).Get<string>(context);

                    // To make sure we don't change global settings.
                    settings = settings.Clone ();
                    settings.UriResolver = delegate (string arg) {
                        if (arg.StartsWithEx ("http://") || arg.StartsWithEx ("https://"))
                            return arg;
                        return root + arg.TrimStart ('/');
                    };
                }

                // Doing actual conversion.
                e.Args.Value = CommonMarkConverter.Convert (md, settings);
            }
        }
    }
}
