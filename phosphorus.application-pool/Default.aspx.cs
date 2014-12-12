/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.applicationpool
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Configuration;
    using System.Collections.Generic;
    using phosphorus.core;
    using phosphorus.ajax.core;
    using phosphorus.ajax.widgets;
    using pf = phosphorus.ajax.widgets;

    public partial class Default : AjaxPage
    {
        // Application Context for page life cycle
        private ApplicationContext _context;

        // main container for all widgets
        protected Container container;

        /*
         * contains all ajax events, and their associated pf.lambda code, for all controls on page. please notice that
         * when we do this like this, we have to store the ViewState into the Session object, which we're doign automatically in
         * OnInit, since otherwise the server-side functionality will follow the page to the client, and allow for the client side
         * to tamper with the server-side functionality, and modify server-side methods or Active Events
         */
        private Dictionary<string, Dictionary<string, List<Node>>> AjaxEvents {
            get {
                if (ViewState ["AjaxEvents"] == null)
                    ViewState ["AjaxEvents"] = new Dictionary<string, Dictionary<string, List<Node>>> ();
                return ViewState ["AjaxEvents"] as Dictionary<string, Dictionary<string, List<Node>>>;
            }
        }

        /*
         * overridden to create context, and do other initialization, such as mapping up our Page_Load event,
         * but only for the initial loading of our page
         */
        protected override void OnInit (EventArgs e)
        {
            // creating application context
            _context = Loader.Instance.CreateApplicationContext ();

            // making sure we store our ViewState in our session object. this is CRUCIAL since we store pf.lambda execution
            // objects in the ViewState, and unless we don't do this, pf.lambda code executed on the server, might be changed by the client
            // which is a huge security risk!
            StoreViewStateInSession = true;

            // registering "this" web page as listener object
            _context.RegisterListeningObject (this);

            // mapping up our Page_Load event for initial loading of web page
            if (!IsPostBack)
                Load += Page_LoadInitialLoading;

            // call base
            base.OnInit (e);
        }

        /*
         * invoked for the first loading of web page to make sure we load up our UI, passing in any arguments
         */
        private void Page_LoadInitialLoading (object sender, EventArgs e)
        {
            // retrieving "form name", and passing it into [pf.load-form], which for web is the local path and name of webpage requested
            // minus ".aspx" parts, in lower characters
            string formName = Request.Params ["file"];

            // raising our [pf.form-load] Active Event, creating the node to passs in first
            Node args = new Node ();
            args.Add (new Node ("_form", formName));

            // making sure we pass in any HTTP GET parameters
            if (Request.QueryString.Keys.Count > 1) {

                // retrieving all GET parameters and passing in as [_args]
                args.Add (new Node ("_args"));
                foreach (var idxArg in Request.QueryString.AllKeys) {
                    args [1].Add (new Node (idxArg, Request.QueryString [idxArg]));
                }
            }

            // invoking the Active Event that actually loads our UI, now with a [_file] node, and possibly an [_args] node
            _context.Raise ("pf.load-ui", args);
        }

        /// <summary>
        /// creates a web form specified through its children nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.create-form")]
        private void pf_create_form (ApplicationContext context, ActiveEventArgs e)
        {
            // finding parent widget first, which defaults to "container" widget, if no widget is given
            Container parent = container;

            // checking to see if "parent" is explicitly given
            foreach (Node idxNode in e.Args.Children) {
                if (idxNode.Name == "parent") {
                    parent = FindWidget<Container> (idxNode.Get<string> (), Page);
                }
            }

            // creating widget
            CreateForm (context, e.Args.Clone (), parent);
        }

        /// <summary>
        /// deletes the widget with the id of the value of the [pf.delete-widget]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.remove-widget")]
        private void pf_remove_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // finding widget to remove
            Widget widget = FindWidget<Widget> (e.Args.Get<string> (), Page);

            // removing all event handlers for widget
            RemoveEvents (widget);

            // actually removing widget from Page control collection, and persisting our change
            Container parent = widget.Parent as Container;
            parent.RemoveControlPersistent (widget);
        }

        /// <summary>
        /// creates an ajax event containing pf.lambda code for the given widget's event
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.add-widget-event")]
        private void pf_web_add_widget_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving widget id, and creating an event collection for the given widget
            Widget widget = e.Args.Get<Widget> ();
            if (!AjaxEvents.ContainsKey (widget.ID))
                AjaxEvents [widget.ID] = new Dictionary<string, List<Node>> ();

            // creating an event collection for the given event for the given widget. notice that one widget might
            // create multiple pf.lambda objects for the same event, meaning one widget might have several ajax event handlers
            // for the same ajax event
            string eventName = e.Args [0].Name;
            if (!AjaxEvents [widget.ID].ContainsKey (eventName))
                AjaxEvents [widget.ID] [eventName] = new List<Node> ();

            // appending our pf.lambda object to the list of pf.lambda objects for the given widget's given event
            AjaxEvents [widget.ID] [eventName].Add (e.Args [0].Clone ());

            // mapping the widget's ajax event to our common event handler on page
            widget [eventName] = "common_event_handler";
        }

        /*
         * recursively searches through page for Container with specified id, starting from "idx"
         */
        public T FindWidget<T> (string id, Control idx) where T : Widget
        {
            if (idx.ID == id)
                return idx as T;
            foreach (Control idxChild in idx.Controls) {
                T tmpRet = FindWidget<T> (id, idxChild);
                if (tmpRet != null)
                    return tmpRet;
            }
            return null;
        }

        /*
         * creates widget according to node given, and returns to caller
         */
        private void CreateForm (ApplicationContext context, Node node, Container parent)
        {
            node.Insert (0, new Node ("__parent", parent));
            context.Raise ("pf.web.widgets.panel", node);
        }
        
        /*
         * recursively removes all events for control and all of its children controls
         */
        private void RemoveEvents (Control idx)
        {
            // removing all ajax events belonging to widget
            if (AjaxEvents.ContainsKey (idx.ID)) {
                AjaxEvents.Remove (idx.ID);
            }

            // recursively removing all ajax events for all of control's children controls
            foreach (Control idxChild in idx.Controls) {
                RemoveEvents (idxChild);
            }
        }

        /*
         * common ajax event handler for all widget's events on page
         */
        [WebMethod]
        protected void common_event_handler (pf.Widget sender, Widget.AjaxEventArgs e)
        {
            string id = sender.ID;
            string eventName = e.Name;
            List<Node> lambdas = AjaxEvents [id] [eventName];
            foreach (Node idxLambda in lambdas) {
                _context.Raise ("lambda", idxLambda.Clone ());
            }
        }

        protected override void OnPreRender (EventArgs e)
        {
            base.OnPreRender (e);
        }
    }
}

