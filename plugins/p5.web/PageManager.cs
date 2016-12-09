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

using System;
using System.Web;
using p5.exp;
using p5.core;
using p5.ajax.core;
using p5.web.widgets;
using p5.exp.exceptions;
using p5.web.widgets.helpers;

namespace p5.web
{
    /// <summary>
    ///     Class managing page for one HTTP request
    /// </summary>
    public class PageManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.PageManager"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="page">Ajax Page</param>
        /// <param name="manager">Ajax Manager</param>
        public PageManager (ApplicationContext context, AjaxPage page)
        {
            // Setting Page for instance
            AjaxPage = page;

            // Initializing lambda and ajax event storage
            InitializeEventStorage (context);

            // Registers all event listeners
            RegisterListeners (context);
        }

        /*
         * AjaxPage for current HTTP request
         */
        public AjaxPage AjaxPage {
            get;
            set;
        }

        /*
         * Used as storage for widget lambda events
         */
        public WidgetEventStorage WidgetLambdaEventStorage {
            get;
            set;
        }

        /*
         * Used as storage for widget ajax events
         */
        public WidgetEventStorage WidgetAjaxEventStorage {
            get;
            set;
        }

        #region [ -- Misc. global Active Events -- ]

        /// <summary>
        ///     Sends the given JavaScript to client one time
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.send-javascript")]
        public void p5_web_send_javascript (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each JavaScript snippet supplied
            foreach (var idxSnippet in XUtil.Iterate<string> (context, e.Args)) {

                // Passing JavaScript to client
                AjaxPage.Manager.SendJavaScriptToClient (idxSnippet);
            }
        }

        /// <summary>
        ///     Includes the given JavaScript on page persistently
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.include-javascript")]
        public void p5_web_include_javascript (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each JavaScript snippet supplied
            foreach (var idxSnippet in XUtil.Iterate<string> (context, e.Args)) {

                // Passing JavaScript to client
                AjaxPage.RegisterJavaScript (idxSnippet);
            }
        }

        /// <summary>
        ///     Includes JavaScript file(s) persistently
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.include-javascript-file")]
        public void p5_web_include_javascript_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each JavaScript file supplied
            foreach (var idxFile in XUtil.Iterate<string> (context, e.Args)) {

                // Passing file to client
                AjaxPage.RegisterJavaScriptFile (context.Raise (".p5.io.unroll-path", new Node ("", idxFile)).Get<string> (context));
            }
        }

        /// <summary>
        ///     Includes CSS StyleSheet file(s) persistently
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.include-css-file")]
        public void p5_web_include_css_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each stylesheet file supplied
            foreach (var idxFile in XUtil.Iterate<string> (context, e.Args)) {

                // Register file for inclusion back to client
                AjaxPage.RegisterStylesheetFile (context.Raise (".p5.io.unroll-path", new Node ("", idxFile)).Get<string> (context));
            }
        }

        /// <summary>
        ///     Changes the URL/location of your web page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.set-location")]
        public void p5_web_set_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if this is ajax callback, which means we'll have to redirect using JavaScript
            if (AjaxPage.Manager.IsPhosphorusAjaxRequest) {

                // Redirecting using JavaScript
                AjaxPage.Manager.SendJavaScriptToClient (
                    string.Format ("window.location='{0}';", 
                        XUtil.Single<string> (context, e.Args).Replace ("'", "\\'")));
            } else {

                // Redirecting using Response object
                AjaxPage.Response.Redirect (XUtil.Single<string> (context, e.Args));
            }
        }

        /// <summary>
        ///     Returns the URL/location of your web page with any HTTP GET parameters.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.get-location")]
        public void p5_web_get_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new core.Utilities.ArgsRemover (e.Args)) {

                // Returning current URL
                e.Args.Value = AjaxPage.Request.Url.ToString();
            }
        }

        /// <summary>
        ///     Returns the URL/location of your web page without any HTTP GET parameters.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.get-location-url")]
        public void p5_web_get_location_url (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new core.Utilities.ArgsRemover (e.Args)) {

                // Returning current URL
                e.Args.Value = AjaxPage.Request.Url.GetLeftPart (UriPartial.Path);
            }
        }

        /// <summary>
        ///     Returns the URL root location of your web application.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.get-root-location")]
        public void p5_web_get_root_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new core.Utilities.ArgsRemover(e.Args)) {

                // Returning web apps root URL
                e.Args.Value = HttpContext.Current.Request.Url.GetLeftPart (UriPartial.Authority) + AjaxPage.ResolveUrl("~/");
            }
        }

        /// <summary>
        ///     Reloads the current location.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.reload-location")]
        public void p5_web_reload_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Redirecting using JavaScript
            AjaxPage.Manager.SendJavaScriptToClient (string.Format ("window.location.replace(window.location.href);"));
        }

        /// <summary>
        ///     Returns the given Node back to client as JSON
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.return-response-object")]
        public void p5_web_return_response_object (ApplicationContext context, ActiveEventArgs e)
        {
            var key = XUtil.Single<string> (context, e.Args);
            var source = XUtil.Source (context, e.Args);
            AjaxPage.Manager.SendObject (key, core.Utilities.Convert<string> (context, source));
        }

        #endregion

        #region [ -- Private helper methods and active events -- ]

        /*
         * Raised by page when an Ajax WebMethod is invoked.
         */
        [ActiveEvent (Name = ".p5.web.raise-ajax-event")]
        private void _p5_web_raise_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            var widgetID = e.Args.Name;
            var eventName = e.Args.Get<string> (context);
            context.Raise("eval", WidgetAjaxEventStorage[widgetID, eventName].Clone());
        }

        /*
         * Initializes storage for ajax and lambda events
         */
        private void InitializeEventStorage (ApplicationContext context)
        {
            // Checking if we should re/initialize storage
            // Sometimes .Net on IIS Express messes this up, having a different assembly on two consecutive debugging sessions, hence we cannot
            // check for simply "IsPostBack"
            if (!AjaxPage.IsPostBack) {

                // Initial loading of page, creating storage for widget lambda events
                WidgetLambdaEventStorage = new WidgetEventStorage();

                // Associating lambda event storage with page by creating a "page value"
                context.Raise(
                    ".set-page-value",
                    new Node(".set-page-value", ".WidgetLambdaEventStorage", new Node[] { new Node("src", WidgetLambdaEventStorage) }));

                // Creating storage for widget ajax events
                WidgetAjaxEventStorage = new WidgetEventStorage();

                // Associating ajax event storage with page by creating a "page value"
                context.Raise(
                    ".set-page-value",
                    new Node(".set-page-value", ".WidgetAjaxEventStorage", new Node[] { new Node("src", WidgetAjaxEventStorage) }));
            } else {

                // Retrieving existing widget lambda event storage
                WidgetLambdaEventStorage = context.Raise (
                    ".get-page-value",
                    new Node(".get-page-value", ".WidgetLambdaEventStorage"))[0]
                    .Get<WidgetEventStorage>(context);

                // Retrieving existing widget ajax event storage
                WidgetAjaxEventStorage = context.Raise (
                    ".get-page-value",
                    new Node(".get-page-value", ".WidgetAjaxEventStorage"))[0]
                    .Get<WidgetEventStorage>(context);
            }
        }

        /*
         * Registers all event listeners
         */
        private void RegisterListeners (ApplicationContext context)
        {
            // Creating and registering our WidgetCreator as event listener
            context.RegisterListeningObject (new WidgetCreator (context, this));

            // Creating and registering our WidgetRetriever as event listener
            context.RegisterListeningObject (new WidgetRetriever (context, this));

            // Creating and registering our WidgetProperties as event listener
            context.RegisterListeningObject (new WidgetProperties (context, this));

            // Creating and registering our WidgetAjaxEvents as event listener
            context.RegisterListeningObject (new WidgetAjaxEvents (context, this));

            // Creating and registering our WidgetLambdaEvents as event listener
            context.RegisterListeningObject (new WidgetLambdaEvents (context, this));

            // Creating and registering our WidgetTypes as event listener
            context.RegisterListeningObject (new WidgetTypes (context, this));

            // Creating and registering our WidgetMisc as event listener
            context.RegisterListeningObject (new WidgetMisc (context, this));
        }

        #endregion
    }
}
