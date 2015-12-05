/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using System.Web.UI;
using p5.ajax.core.internals;

// This might look fuuny, but to document the namespace, such that Doxygen recognizes it, 
// and can create documentation for it, we must do it like this.
/// <summary>
///     Contains all Ajax functionality in Phosphorus.
/// 
///     Contains all the Ajax functionality in Phosphorus, such as its Widgets, AjaxPage and so on. p5.ajax is a C# ASP.NET WebControls
///     library, that can be used without bringing in any other dependencies from Phosphorus Five. In such a regard, p5.ajax is an alternative
///     to other Ajax Libraries, such as Anthem.NET and ASP.NET Ajax.
/// 
///     If all you wish to to, is to have a nice Ajax library for your C# or VB.NET ASP.NET projects, then p5.ajax is a decent alternative.
///     If this is what you want, then the only project dll you need to reference inside of your own projects is <em>"p5.ajax.dll"</em>.
/// 
///     p5.ajax is a managed Ajax Library, and all JavaScript for the library is roughly 5KB when built in Release Build. In addition, 
///     p5.ajax only sends the changes back from the server to your client, which means that the amount of bandwidth usage when using
///     p5.ajax is probably unmatched by other similar libraries.
/// 
///     With p5.ajax, you have complete control over what HTML is rendered from your server. There are no <em>"magic divs"</em>, or huge
///     JavaScript files being transfered. p5.ajax tries to fix what needs to be fixed, and leave the rest up to you. p5.ajax is
///     100% perfectly compatible with all other major Ajax Libraries out there, such as jQuery and Prototype.js.
/// 
///     With p5.ajax, you can create your WebControls, just like you would in a normal ASP.NET project, and let the Ajax parts, and updating
///     of your widget's properties, automtically be taken care of. While still retaining 100% control over what goes over the wire, and what HTML
///     is rendered.
/// 
///     For examples of how to use p5.ajax, you can check out the example web project from the Phosphorus Five download called 
///     <em>"p5.ajax.samples"</em>.
/// </summary>
namespace p5.ajax
{
    /// <summary>
    ///     Contains all core functionality in p5.ajax.
    /// 
    ///     Contains all the core helper and supporting features in p5.ajax, such as the Manager, AjaxPage and so on.
    ///     Helps glue together the Ajax Widgets with your Page, and helps you handle ViewState, create Ajax WebMethods and so on.
    /// </summary>
    namespace core
    {
        /// <summary>
        ///     Helper class for implementing core Ajax functionality on your page.
        /// 
        ///     Inherit all your ASP.NET Pages from this class in your solution to allow for them to have Ajax functionality.
        ///     If you do not wish to inherit from this class,
        ///     you can implement the <see cref="p5.ajax.core.IAjaxPage">IAjaxPage</see> interface on your page instead,
        ///     and create an instance of the <see cref="p5.ajax.core.Manager" /> yourself, during the
        ///     initialization of your page.
        /// </summary>
        public class AjaxPage : Page, IAjaxPage
        {
            private PageStatePersister _statePersister;
            private List<Tuple<string, bool>> _newJavaScriptObjects = new List<Tuple<string, bool>> ();
            private List<string> _newCssFiles = new List<string> ();

            /// <summary>
            ///     Maximum number of ViewState entries in Session.
            /// 
            ///     If this is zero, ViewState will not be stored in Session,
            ///     but sent back and forth between client and browser as usual. This might be a security issue for you, in 
            ///     addition to that it increases the size of all your HTTP requests significantly. Unless you know what you are
            ///     doing, you should always store the ViewState on the server, having a positive value, as small as possible,
            ///     of this property.
            /// 
            ///     The higher this number is, the more memory your serer is going to consume. The lower this number is, the
            ///     less number of consecutive open windows the end user can have towards your application at the same time.
            /// 
            ///     If you set this number to "-1", then an infinite amount of state objects per session will be stored on your
            ///     server, which opens up your server to a whole range of difficulties, such as easily draining your server for
            ///     all its memory by simply pressing CTRL+R hundreds of times, etc. For some intranet sites though, this might
            ///     be a useful value, if you know you can trust your users not to sabotage your site.
            /// 
            ///     Useful and safe values for this property, probably ranges from anything from 5 to 20, depending upon the amount
            ///     of memory you have, and what type of site you're creating. If your site is a "single page Ajax application", then
            ///     having "1" as your value will be OK. If your site has no Ajax functionality almost at all, then you can also 
            ///     probably come away with a value of "1" for your page.
            /// 
            ///     If your site however mixes Ajax functionality with multiple URLs, or your users frequently opens up more than
            ///     one window to your site, then you should probably increase this number beyond "1", since otherwise every time
            ///     a user opens up a new window to your site, he will invalidate the state for all previously opened windows, and
            ///     break their Ajax functionality as he does.
            /// 
            ///     I recommend "5" to "10" as a general rule of thumb for this value, unless you know what you're doing.
            /// </summary>
            /// <value>The number of valid viewstate entries for each session.</value>
            public int ViewStateSessionEntries { get; set; }

            /// <summary>
            /// Gets the page state persister.
            /// </summary>
            /// <value>The page state persister.</value>
            protected override PageStatePersister PageStatePersister
            {
                get
                {
                    if (ViewStateSessionEntries == 0)
                        return base.PageStatePersister;
                    return _statePersister ?? (_statePersister = new StatePersister (this, ViewStateSessionEntries));
                }
            }

            /// <summary>
            ///     Returns the ajax manager for your page.
            /// </summary>
            /// <value>the ajax manager</value>
            public Manager Manager { get; private set; }

            /// <summary>
            ///     Registers JavaScript file for page, that will be included on the client-side.
            /// </summary>
            /// <param name="url">url to JavaScript to register</param>
            public void RegisterJavaScriptFile (string url)
            {
                if (ViewState ["_p5_js_objects"] == null)
                    ViewState ["_p5_js_objects"] = new List<Tuple<string, bool>> ();
                var lst = ViewState ["_p5_js_objects"] as List<Tuple<string, bool>>;
                if (lst.Find (delegate (Tuple<string, bool> idx) { return idx.Item1 == url; }) == null) {
                    lst.Add (new Tuple<string, bool>(url, true));
                    _newJavaScriptObjects.Add (new Tuple<string, bool>(url, true));
                }
            }
            
            /// <summary>
            ///     Registers JavaScript for page, that will be included on the client-side.
            /// </summary>
            /// <param name="url">url to JavaScript to register</param>
            public void RegisterJavaScript (string script)
            {
                if (ViewState ["_p5_js_objects"] == null)
                    ViewState ["_p5_js_objects"] = new List<Tuple<string, bool>> ();
                var lst = ViewState ["_p5_js_objects"] as List<Tuple<string, bool>>;
                if (lst.Find (delegate (Tuple<string, bool> idx) { return idx.Item1 == script; }) == null) {
                    lst.Add (new Tuple<string, bool>(script, false));
                    _newJavaScriptObjects.Add (new Tuple<string, bool>(script, true));
                }
            }

            /// <summary>
            ///     Registers stylesheet file for page, that will be included on the client-side.
            /// </summary>
            /// <param name="url">url to stylesheet to register</param>
            public void RegisterStylesheetFile (string url)
            {
                if (ViewState ["_p5_css_files"] == null)
                    ViewState ["_p5_css_files"] = new List<string> ();
                var lst = ViewState ["_p5_css_files"] as List<string>;
                if (!lst.Contains (url)) {
                    lst.Add (url);
                    _newCssFiles.Add (url);
                }
            }

            /*
             * returns the JavaScript file URL's we need to push to client during this request
             */
            List<Tuple<string, bool>> IAjaxPage.JavaScriptToPush
            {
                get { return ViewState ["_p5_js_objects"] as List<Tuple<string, bool>>; }
            }

            /*
             * returns the JavaScript file URL's we need to push to client during this request
             */
            List<Tuple<string, bool>> IAjaxPage.NewJavaScriptToPush
            {
                get { return _newJavaScriptObjects; }
            }

            /*
             * returns the JavaScript file URL's we need to push to client during this request
             */
            List<string> IAjaxPage.StylesheetFilesToPush
            {
                get { return ViewState ["_p5_css_files"] as List<string>; }
            }

            /*
             * returns the JavaScript file URL's we need to push to client during this request
             */
            List<string> IAjaxPage.NewStylesheetFilesToPush
            {
                get { return _newCssFiles; }
            }

            protected override void OnPreInit (EventArgs e)
            {
                Manager = new Manager (this);
                base.OnPreInit (e);
            }
        }
    }
}
