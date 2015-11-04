/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using HtmlAgilityPack;
using p5.core;
using p5.exp;

/// <summary>
///     Main namespace for handling HTML.
/// 
///     Contains all Active Events related to the process of parsing, and creating HTML.
/// </summary>
namespace p5.html
{
    /// <summary>
    ///     Class to help encode and decode HTML
    /// </summary>
    public static class HtmlEncoder
    {
        /// <summary>
        ///     Encodes HTML
        /// 
        ///     Changes all occurrencies of lesser-than and greater-than angle brackets to their associated 'safe mode' HTML version
        ///     and returns the as value of main node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.html.html-encode")]
        private static void p5_html_html_encode (ApplicationContext context, ActiveEventArgs e)
        {
            // loops through all documents we're supposed to transform
            bool first = true;
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // changing to 'safe HTML'
                if (first)
                    e.Args.Value = "";
                e.Args.Value = e.Args.Get<string> (context) + idx.Replace ("<", "&lt;").Replace (">", "&gt;");
            }
        }

        /// <summary>
        ///     Decodes HTML
        /// 
        ///     Changes all occurrencies of lesser-than and greater-than HTML-escaped angle brackets to their 
        ///     associated 'non-safe mode' HTML version and returns the as value of main node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "p5.html.html-decode")]
        private static void p5_html_html_decode (ApplicationContext context, ActiveEventArgs e)
        {
            // loops through all documents we're supposed to transform
            bool first = true;
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // changing to 'safe HTML'
                if (first)
                    e.Args.Value = "";
                e.Args.Value = e.Args.Get<string> (context) + idx.Replace ("&lt;", "<").Replace ("&gt;", ">");
            }
        }
    }
}
