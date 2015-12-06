/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using p5.exp;
using p5.core;
using p5.ajax.core;
using p5.ajax.widgets;

namespace p5.webapp.code
{
    /// <summary>
    ///     A Phosphorus Five web application page
    /// </summary>
    public class PhosphorusPage : AjaxPage
    {
        /// <summary>
        ///     Application context for current HTTP request
        /// </summary>
        /// <value>The context</value>
        protected ApplicationContext ApplicationContext {
            get;
            private set;
        }

        #region [ -- Page overrides and initializers -- ]

        /*
         * Overridden to create and initialize application context
         */
        protected override void OnInit (EventArgs e)
        {
            // Creating our application context for current request
            ApplicationContext = Loader.Instance.CreateApplicationContext ();

            // Registering "this" web page as listener object, since page contains many Active Event handlers itself
            ApplicationContext.RegisterListeningObject (this);

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
            ApplicationContext.RaiseNative ("p5.web.initialize-page", args);

            // Calling base
            base.OnLoad(e);
        }

        #endregion

        #region [ -- Page value Active Events -- ]

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

        #endregion

        /*
         * Common ajax event handler for all widget events on page
         */
        [WebMethod]
        protected void common_event_handler (Widget sender, Widget.AjaxEventArgs e)
        {
            var args = new Node(sender.ID, e.Name);
            ApplicationContext.RaiseNative("p5.web.raise-ajax-event", args);
        }
    }
}

