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

            // Raising our "page initialized" active event
            ApplicationContext.Raise (".p5.web.initialize-page", args);

            // Calling base
            base.OnLoad(e);
        }

        #endregion

        #region [ -- Page specific Active Events -- ]

        /// <summary>
        ///     Sets one or more page object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-page-value")]
        [ActiveEvent (Name = ".set-page-value")]
        public void set_page_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Set (context, e.Args, delegate (string key, object value) {
                if (value == null) {

                    // Removal
                    ViewState.Remove (key);
                } else {

                    // Setting or updating
                    ViewState[key] = value;
                }
            });
        }

        /// <summary>
        ///     Retrieves page object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-page-value")]
        [ActiveEvent (Name = ".get-page-value")]
        public void get_page_value (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.Get (context, e.Args, key => ViewState [key]);
        }

        /// <summary>
        ///     Lists all keys in the page object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-page-keys")]
        [ActiveEvent (Name = ".list-page-keys")]
        public void list_page_keys (ApplicationContext context, ActiveEventArgs e)
        {
            XUtil.List (context, e.Args, ViewState.Keys);
        }

        #endregion

        #region [ -- General Page Active Events -- ]

        /// <summary>
        ///     Changes the title of your web page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-title")]
        public void set_title (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving new Title of page
            var title = XUtil.Single<string>(context, e.Args);

            // Checking if this is ajax request, at which point we'll have to update title using JavaScript
            if (Manager.IsPhosphorusAjaxRequest) {

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
        [ActiveEvent (Name = "get-title")]
        public void get_title (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args)) {

                // ViewState title has presedence, since it might have been changed, 
                // and "Title" property of page is not serialized into ViewState
                e.Args.Value = ViewState["_pf_title"] ?? Title;
            }
        }

        /// <summary>
        ///     Returns true if this is an Ajax callback.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "is-callback")]
        public void is_callback (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Manager.IsPhosphorusAjaxRequest;
        }

        #endregion

        /*
         * Common ajax event handler for all widget events on page
         */
        [WebMethod]
        protected void common_event_handler (Widget sender, Widget.AjaxEventArgs e)
        {
            var args = new Node (sender.ID, e.Name);

            // Raising event, making sure we can handle any exceptions occuring.
            try {
                ApplicationContext.Raise (".p5.web.raise-ajax-event", args);
            } catch (Exception err) {

                // Notice, we rethrow exception if handler didn't return true.
                if (!HandleException (err))
                    throw;
            }
        }

        /*
         * Invoked when an exception occurs.
         */
        protected bool HandleException (Exception err)
        {
            var message = err.Message;
            var trace = err.StackTrace;
            var idxType = err.GetType ();
            while (idxType != typeof (object)) {

                // Checkng if we have exceptions handlers for current type of exception.
                var idxTypeName = idxType.Name;
                var args = new Node ();
                args.Add ("_message", message);
                args.Add ("_trace", trace);
                args.Add ("_type", err.GetType ().Name);
                ApplicationContext.Raise ("p5.error." + idxTypeName, args);
                if (args.Get (ApplicationContext, false))
                    return true;
                idxType = idxType.BaseType;
            }
            return false;
        }
    }
}

