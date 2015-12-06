/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.ajax.core;
using p5.webapp.code;
using p5.ajax.widgets;

/// <summary>
///     Main namespace for your Phosphorus Five web app
/// </summary>
namespace p5.webapp
{
    /// <summary>
    ///     Main asp.net web page for your Application Pool
    /// </summary>
    public partial class Default : AjaxPage
    {
        // Main root container for all widgets
        protected Container cnt;

        // Application Context for page life cycle
        private ApplicationContext _context;

        #region [ -- Page overrides and initializers -- ]

        /*
         * Overridden to create context, and do other types of initialization, such as mapping up our Page_Load event,
         * URL-rewriting, and so on.
         */
        protected override void OnInit (EventArgs e)
        {
            // Retrieving viewstate entries per session
            ViewStateSessionEntries = int.Parse (ConfigurationManager.AppSettings ["viewstate-per-session-entries"]);

            // Creating our application context for current request
            _context = Loader.Instance.CreateApplicationContext ();

            // Registering "this" web page as listener object, since page contains many Active Event handlers itself
            _context.RegisterListeningObject (this);

            // Rewriting path to what was actually requested, such that HTML form element's action doesn't become garbage.
            // This ensures that our HTML form element stays correct. Basically "undoing" what was done in Global.asax.cs
            // In addition, when retrieving request URL later, we get the "correct" request URL, and not the URL to "Default.aspx"
            HttpContext.Current.RewritePath ((string) HttpContext.Current.Items ["_p5_original_url"]);

            // Mapping up our Page_Load event for initial loading of web page, but only if is not a form postback or ajax request
            if (!IsPostBack)
                Load += Page_LoadInitialLoading;

            // Call base
            base.OnInit (e);
        }

        /*
         * Invokes initialize page Active Event
         */
        protected override void OnLoad(EventArgs e)
        {
            // Creating args for event
            var args = new Node();
            args.Add ("page", this);
            args.Add ("manager", Manager);

            // Raising our "page initialized" active event
            _context.RaiseNative ("p5.web.initialize-page", args);

            // Calling base
            base.OnLoad(e);
        }

        /*
         * Invoked only for the initial request of our web page, to make sure we load up our UI 
         * according to which URL is requested. Not invoked during any consecutive POST requests or Ajax requests
         */
        private void Page_LoadInitialLoading (object sender, EventArgs e)
        {
            // Raising our [p5.web.load-ui] Active Event, creating the node to pass in first,
            // where the [_form] node becomes the name of the form requested
            var args = new Node ("p5.web.load-ui");
            args.Add (new Node ("_form", (string) HttpContext.Current.Items ["_p5_original_url"]));

            // Invoking the Active Event that actually loads our UI, now with a [_form] node being the URL of the requested page
            _context.RaiseNative ("p5.web.load-ui", args);
        }

        #endregion


        #region [ -- Page value setters and getters -- ]

        /// <summary>
        ///     Sets one or more page object(s)
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-page-value", Protection = EventProtection.LambdaClosed)]
        private void set_page_value (ApplicationContext context, ActiveEventArgs e)
        {
            // Using collection helper to traverse all keys supplied by caller
            p5.exp.CollectionBase.Set (context, e.Args, (key, value) => {

                // Checking if this is delete operation
                if (value == null) {

                    // Removing object, if it exists
                    ViewState.Remove (key);
                } else {

                    // Adding object
                    ViewState [key] = value;
                }
            }, e.NativeSource);
        }

        /// <summary>
        ///     Retrieves page object(s)
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-page-value", Protection = EventProtection.LambdaClosed)]
        private void get_page_value (ApplicationContext context, ActiveEventArgs e)
        {
            p5.exp.CollectionBase.Get (context, e.Args, key => ViewState [key], e.NativeSource);
        }

        /// <summary>
        ///     Lists all keys in the page object
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-page-keys", Protection = EventProtection.LambdaClosed)]
        private void list_page_keys (ApplicationContext context, ActiveEventArgs e)
        {
            p5.exp.CollectionBase.List (context, e.Args, ViewState.Keys, e.NativeSource);
        }

        #endregion

        #region [ -- General Page Active Events -- ]

        /// <summary>
        ///     Changes the title of your web page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-title", Protection = EventProtection.LambdaClosed)]
        private void set_title (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving new Title of page
            var title = XUtil.Single<string>(context, e.Args, true);

            // Checking if this is ajax request, at which point we'll have to update title using JavaScript
            if (Manager.IsPhosphorusRequest) {

                // Passing title to client as JavaScript update, making sure we escape string
                Manager.SendJavaScriptToClient(
                    string.Format("document.title='{0}';", title.Replace("\\", "\\\\").Replace("'", "\\'")));
            } else {

                // Updating Title element of page
                Title = title;
            }

            // Storing Title in ViewState such that we can retrieve correct title later
            ViewState["_pf_title"] = Title;
        }

        /// <summary>
        ///     Returns the title of your web page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-title", Protection = EventProtection.LambdaClosed)]
        private void get_title (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args)) {

                // ViewState title has presedence, since it might have been changed, 
                // and "Title" property of page is not serialized into ViewState
                e.Args.Value = ViewState["_pf_title"] ?? Title;
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
            if (Manager.IsPhosphorusRequest) {

                // Redirecting using JavaScript
                Manager.SendJavaScriptToClient (
                    string.Format ("window.location='{0}';", XUtil.Single<string> (context, e.Args, true)));
            } else {

                // Redirecting using Response object
                Page.Response.Redirect (XUtil.Single<string> (context, e.Args));
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
            Manager.SendJavaScriptToClient (string.Format ("window.location.replace(window.location.pathname);"));
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
                e.Args.Value = Request.Url.ToString();
            }
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
            Manager.SendObject (key, str);
        }

        #endregion

        /*
         * Common ajax event handler for all widget's events on page
         */
        [WebMethod]
        protected void common_event_handler (Widget sender, Widget.AjaxEventArgs e)
        {
            var args = new Node(sender.ID, e.Name);
            _context.RaiseNative("p5.web.raise-ajax-event", args);
        }
    }
}
