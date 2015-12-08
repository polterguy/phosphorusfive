/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.ajax.core;
using p5.web.widgets;
using p5.ajax.widgets;
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
        ///     Initializes a new instance of the <see cref="p5.web.widgets.PageManager"/> class
        /// </summary>
        /// <param name="page">Page</param>
        private PageManager (ApplicationContext context, AjaxPage page, Manager manager)
        {
            // Setting Page for instance
            AjaxPage = page;

            // Setting Ajax Manager for page
            AjaxManager = manager;

            // Initializing lambda and ajax event storage
            InitializeEventStorage (context);

            // Registers all event listeners
            RegisterListeners (context);
        }

        /*
         * AjaxPage for current HTTP request
         */
        internal AjaxPage AjaxPage {
            get;
            set;
        }

        /*
         * AjaxManager for current HTTP request
         */
        internal Manager AjaxManager {
            get;
            set;
        }

        /*
         * Used as storage for widget lambda events
         */
        internal WidgetEventStorage WidgetLambdaEventStorage {
            get;
            set;
        }

        /*
         * Used as storage for widget ajax events
         */
        internal WidgetEventStorage WidgetAjaxEventStorage {
            get;
            set;
        }

        /*
         * Raised by page during initialization of page
         */
        [ActiveEvent (Name = "p5.web.initialize-page", Protection = EventProtection.NativeOpen)]
        private static void p5_web_initialize_page (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving Page and Manager for current HTTP context
            var page = e.Args ["page"].Get<AjaxPage> (context);
            var manager = e.Args ["manager"].Get<Manager> (context);

            // Creating an instance of this class, registering it as event listener in App Context
            var instance = new PageManager (
                context, 
                page,
                manager);
            context.RegisterListeningObject (instance);
        }

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

        #region [ -- Misc. global Active Events -- ]

        /// <summary>
        ///     Sends the given JavaScript to client
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "send-javascript", Protection = EventProtection.LambdaClosed)]
        private void send_javascript (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each JavaScript snippet
            foreach (var idxSnippet in XUtil.Iterate<string> (context, e.Args)) {

                // Passing file to client
                AjaxManager.SendJavaScriptToClient (idxSnippet);
            }
        }

        /// <summary>
        ///     Includes the given JavaScript on page persistently
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "include-javascript", Protection = EventProtection.LambdaClosed)]
        private void include_javascript (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each JavaScript snippet
            foreach (var idxSnippet in XUtil.Iterate<string> (context, e.Args)) {

                // Passing file to client
                AjaxPage.RegisterJavaScript (idxSnippet);
            }
        }

        /// <summary>
        ///     Includes JavaScript file(s) persistently
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "include-javascript-file", Protection = EventProtection.LambdaClosed)]
        private void include_javascript_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each JavaScript file
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
        private void include_stylesheet_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each stylesheet file given
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
        private void set_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if this is ajax callback, which means we'll have to redirect using JavaScript
            if (AjaxManager.IsPhosphorusRequest) {

                // Redirecting using JavaScript
                AjaxManager.SendJavaScriptToClient (
                    string.Format ("window.location='{0}';", XUtil.Single<string> (context, e.Args, true)));
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
        private void get_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args)) {

                // Returning current URL
                e.Args.Value = AjaxPage.Request.Url.ToString();
            }
        }

        /// <summary>
        ///     Reloads the current document
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "reload-location", Protection = EventProtection.LambdaClosed)]
        private void reload_location (ApplicationContext context, ActiveEventArgs e)
        {
            // Redirecting using JavaScript
            AjaxManager.SendJavaScriptToClient (string.Format ("window.location.replace(window.location.pathname);"));
        }

        /// <summary>
        ///     Returns the given Node back to client as JSON
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "return-response-object", Protection = EventProtection.LambdaClosed)]
        private void return_response_object (ApplicationContext context, ActiveEventArgs e)
        {
            var key = XUtil.Single<string> (context, e.Args);
            var str = p5.core.Utilities.Convert<string> (context, XUtil.SourceSingle(context, e.Args), "");
            AjaxManager.SendObject (key, str);
        }

        #endregion

        #region [ -- Private helper methods and active events -- ]

        /*
         * Initializes storage for ajax and lambda events
         */
        private void InitializeEventStorage (ApplicationContext context)
        {
            // Checking if this is initial load of page or not
            if (AjaxPage.IsPostBack) {

                // Retrieving existing widget lambda event storage
                WidgetLambdaEventStorage = context.RaiseNative (
                    "get-page-value", 
                    new Node ("", "_WidgetLambdaEventStorage"))
                    .Get<WidgetEventStorage> (context);

                // Retrieving existing widget ajax event storage
                WidgetAjaxEventStorage = context.RaiseNative (
                    "get-page-value", 
                    new Node ("", "_WidgetAjaxEventStorage"))
                    .Get<WidgetEventStorage> (context);
            } else {

                // Creating storage for widget lambda events
                WidgetLambdaEventStorage = new WidgetEventStorage ();

                // Associating lambda event storage with page by creating a "page value"
                context.RaiseNative (
                    "set-page-value", 
                    new Node ("", "_WidgetLambdaEventStorage", new Node [] { new Node ("src", WidgetLambdaEventStorage) }));

                // Creating storage for widget ajax events
                WidgetAjaxEventStorage = new WidgetEventStorage ();

                // Associating ajax event storage with page by creating a "page value"
                context.RaiseNative (
                    "set-page-value", 
                    new Node ("", "_WidgetAjaxEventStorage", new Node [] { new Node ("src", WidgetAjaxEventStorage) }));
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

        /*
         * Recursively searches through page for Container with specified id, starting from "startWidget"
         */
        internal T FindControl<T> (string id, Control startWidget) where T : Control
        {
            if (startWidget.ID == id)
                return startWidget as T;
            return (from Control idxChild in startWidget.Controls select FindControl<T> (id, idxChild)).FirstOrDefault (tmpRet => tmpRet != null);
        }

        /*
         * Helper to figure out if Active Event is protected or not
         */
        internal bool CanOverrideEventInLambda (ApplicationContext context, string evt)
        {
            // Verifying Active Event is not protected, first checking native handlers
            if (context.HasEvent (evt)) {

                // There exist a native handler for this Active Event, now getting protection level of event
                if (context.GetEventProtection(evt) == EventProtection.LambdaOpen)
                    return true;
            }

            // Checking if protected events contains given event name, and if so, returning true, else returning false
            return context.RaiseNative("p5 lambda.get-protected-events")[evt] == null;
        }

        /*
         * Helper to retrieve a list of widgets from a Node, throws if widget with specified ID does not exist
         */
        internal IEnumerable<T> FindWidgets<T> (ApplicationContext context, Node args, string activeEventName) where T : Control
        {
            // Looping through all Widget IDs supplied by caller, finding widget with specified ID
            foreach (var idxWidgetID in XUtil.Iterate<string> (context, args, true)) {

                // Retrieving Widget with currently iterated ID
                var idxWidget = FindControl<Control>(idxWidgetID, AjaxPage);

                // Throwing exception if widget does not exist
                if (idxWidget == null)
                    throw new LambdaException(
                        string.Format ("Couldn't find widget with ID '{0}'", idxWidgetID),
                        args, 
                        context);

                // Verifies widget is of requested type
                var retVal = idxWidget as T;
                if (retVal == null) {

                    // Widget was not correct type, figuring out type of widget
                    string typeString = typeof (T).FullName;
                    if (typeof(T).BaseType == typeof(Widget)) {

                        // Using "short version" typename for all widgets inheriting from p5 Ajax Widget
                        typeString = typeString.Substring(typeString.LastIndexOf(".") + 1).ToLower();
                    }

                    // Throwing exception
                    throw new LambdaException(
                        string.Format("You cannot use [{0}] on a Control that is not of type '{1}'", activeEventName, typeString),
                        args,
                        context);
                }

                // Returning widget to caller
                yield return retVal;
            }
        }

        #endregion
    }
}
