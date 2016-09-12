/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using p5.exp;
using p5.core;
using p5.ajax.core;
using p5.web.widgets;
using p5.web.widgets.helpers;

namespace p5.web {
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
        [ActiveEvent (Name = "send-javascript", Protection = EventProtection.LambdaClosed)]
        public void send_javascript (ApplicationContext context, ActiveEventArgs e)
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
        [ActiveEvent (Name = "include-javascript", Protection = EventProtection.LambdaClosed)]
        public void include_javascript (ApplicationContext context, ActiveEventArgs e)
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
        [ActiveEvent (Name = "include-javascript-file", Protection = EventProtection.LambdaClosed)]
        public void include_javascript_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each JavaScript file supplied
            foreach (var idxFile in XUtil.Iterate<string> (context, e.Args)) {

                // Passing file to client
                AjaxPage.RegisterJavaScriptFile (idxFile);
            }
        }

        /// <summary>
        ///     Includes CSS StyleSheet file(s) persistently
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "include-stylesheet-file", Protection = EventProtection.LambdaClosed)]
        public void include_stylesheet_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each stylesheet file supplied
            foreach (var idxFile in XUtil.Iterate<string> (context, e.Args)) {

                // Register file for inclusion back to client
                AjaxPage.RegisterStylesheetFile (idxFile);
            }
        }

        /// <summary>
        ///     Changes the URL/location of your web page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-location", Protection = EventProtection.LambdaClosed)]
        public void set_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if this is ajax callback, which means we'll have to redirect using JavaScript
            if (AjaxPage.Manager.IsPhosphorusAjaxRequest) {

                // Redirecting using JavaScript
                AjaxPage.Manager.SendJavaScriptToClient (
                    string.Format ("window.location='{0}';", 
                        XUtil.Single<string> (context, e.Args, true).Replace ("'", "\\'")));
            } else {

                // Redirecting using Response object
                AjaxPage.Response.Redirect (XUtil.Single<string> (context, e.Args));
            }
        }

        /// <summary>
        ///     Returns the URL/location of your web page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-location", Protection = EventProtection.LambdaClosed)]
        public void get_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args)) {

                // Returning current URL
                e.Args.Value = AjaxPage.Request.Url.ToString();
            }
        }

        /// <summary>
        ///     Returns the URL root location of your web application
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-base-location", Protection = EventProtection.LambdaClosed)]
        public void get_base_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args)) {

                // Returning web apps root URL
                e.Args.Value = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + AjaxPage.ResolveUrl("~/");
            }
        }

        /// <summary>
        ///     Reloads the current document
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "reload-location", Protection = EventProtection.LambdaClosed)]
        public void reload_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Redirecting using JavaScript
            AjaxPage.Manager.SendJavaScriptToClient (string.Format ("window.location.replace(window.location.pathname);"));
        }

        /// <summary>
        ///     Returns the given Node back to client as JSON
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "return-response-object", Protection = EventProtection.LambdaClosed)]
        public void return_response_object (ApplicationContext context, ActiveEventArgs e)
        {
            var key = XUtil.Single<string> (context, e.Args);
            var str = p5.core.Utilities.Convert<string> (context, XUtil.SourceSingle(context, e.Args), "");
            AjaxPage.Manager.SendObject (key, str);
        }

        #endregion

        #region [ -- Private helper methods and active events -- ]

        /*
         * Raised by page when an Ajax WebMethod is invoked
         */
        [ActiveEvent (Name = "p5.web.raise-ajax-event", Protection = EventProtection.NativeClosed)]
        private void p5_web_raise_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            var widgetID = e.Args.Name;
            var eventName = e.Args.Get<string> (context);
            context.RaiseLambda("eval", WidgetAjaxEventStorage[widgetID, eventName].Clone());
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
                context.RaiseNative(
                    "set-page-value",
                    new Node("", "_WidgetLambdaEventStorage", new Node[] { new Node("src", WidgetLambdaEventStorage) }));

                // Creating storage for widget ajax events
                WidgetAjaxEventStorage = new WidgetEventStorage();

                // Associating ajax event storage with page by creating a "page value"
                context.RaiseNative(
                    "set-page-value",
                    new Node("", "_WidgetAjaxEventStorage", new Node[] { new Node("src", WidgetAjaxEventStorage) }));
            } else {

                // Retrieving existing widget lambda event storage
                WidgetLambdaEventStorage = context.RaiseNative(
                    "get-page-value",
                    new Node("", "_WidgetLambdaEventStorage"))
                    .Get<WidgetEventStorage>(context);

                // Retrieving existing widget ajax event storage
                WidgetAjaxEventStorage = context.RaiseNative(
                    "get-page-value",
                    new Node("", "_WidgetAjaxEventStorage"))
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
        }

        #endregion
    }
}
