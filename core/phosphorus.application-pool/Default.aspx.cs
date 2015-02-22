
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.applicationpool
{
    using System;
    using System.Web;
    using System.Text;
    using System.Web.UI;
    using System.Configuration;
    using System.Collections.Generic;
    using phosphorus.core;
    using phosphorus.expressions;
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
         * contains all dynamically created "widget events", which are Active Events local for a specific widget,
         * created as parts of the [events] keyword
         */
        private Dictionary<string, List<Tuple<string, Node>>> PageEvents {
            get {
                if (ViewState ["PageEvents"] == null) {
                    ViewState ["PageEvents"] = new Dictionary<string, List<Tuple<string, Node>>> ();
                }
                return (Dictionary<string, List<Tuple<string, Node>>>)ViewState ["PageEvents"];
            }
        }

        /*
         * overridden to create context, and do other initialization, such as mapping up our Page_Load event,
         * but only for the initial loading of our page
         */
        protected override void OnInit (EventArgs e)
        {
            // retrieving viewstate entries per session
            ViewStateSessionEntries = int.Parse (ConfigurationManager.AppSettings ["viewstate-per-session-entries"]);

            // creating application context
            _context = Loader.Instance.CreateApplicationContext ();

            // registering "this" web page as listener object
            _context.RegisterListeningObject (this);

            // rewriting path to what was actually requested, such that HTML form element doesn't become garbage ...
            // this ensures that our HTML form element stays correct. basically "undoing" what was done in Global.asax.cs
            // in addition, when retrieving request URL later, we get the "correct" request URL, and not the URL to "Default.aspx"
            HttpContext.Current.RewritePath (HttpContext.Current.Items ["__pf_original_url"] as string);

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
            // raising our [pf.web.load-ui] Active Event, creating the node to pass in first
            // where the [_form] node becomes the name of thr form requested
            Node args = new Node ("pf.web.load-ui");
            args.Add (new Node ("_form", HttpContext.Current.Items ["__pf_original_url"] as string));

            // making sure we pass in any HTTP GET parameters
            if (Request.QueryString.Keys.Count > 0) {

                // retrieving all GET parameters and passing in as [_args]
                args.Add (new Node ("_args"));
                foreach (var idxArg in Request.QueryString.AllKeys) {
                    args [1].Add (new Node (idxArg, Request.QueryString [idxArg]));
                }
            }

            // invoking the Active Event that actually loads our UI, now with a [_file] node, and possibly an [_args] node
            _context.Raise ("pf.web.load-ui", args);
        }

        /// <summary>
        /// creates a web widget (form) specified through its children nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.create-widget")]
        private void pf_web_create_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // finding parent widget first, which defaults to "container" widget, if no widget is given
            Node parentNode = e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "parent";
            });
            Container parent = parentNode != null ? FindControl<Container> (parentNode.Get<string> (context), Page) : container;

            // creating widget
            Node creationalArgs = e.Args.Clone ();
            CreateForm (context, creationalArgs, parent);
            Control widget = creationalArgs.Get<Control> (context);

            // creating events for widget
            Node eventNode = e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "events";
            });
            if (eventNode != null) {
                CreateWidgetEvents (widget, eventNode, context);
            }
        }

        /*
         * creates events for web widget created through [pf.web.create-widget]
         */
        private void CreateWidgetEvents (Control widget, Node eventNode, ApplicationContext context)
        {
            foreach (var idxEvt in XUtil.IterateChildren (eventNode, context)) {
                if (!PageEvents.ContainsKey (idxEvt.Name)) {
                    PageEvents [idxEvt.Name] = new List<Tuple<string, Node>> ();
                }
                PageEvents [idxEvt.Name].Add (new Tuple<string, Node> (widget.ID, idxEvt.Clone ()));
            }
        }

        /// <summary>
        /// null Active Event handler, for handling widget specific Active Events
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "")]
        private void null_handler (ApplicationContext context, ActiveEventArgs e)
        {
            // checking to see if the currently raised Active Event has a calllback within our page
            if (PageEvents.ContainsKey (e.Name)) {

                // this Active Event is handled by one of the widgets on this page object
                // making sure we raise it passing in the arguments passed in to the Active Event cloned
                foreach (var idxEvt in PageEvents [e.Name]) {
                    Node exeCopy = idxEvt.Item2.Clone ();
                    exeCopy.Value = e.Args.Value;
                    foreach (Node idxArg in e.Args.Children) {
                        exeCopy.Add (idxArg.Clone ());
                    }
                    context.Raise ("lambda", exeCopy);
                }
            }
        }

        /// <summary>
        /// clears the given widget, removing all its children widgets
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.clear-widget")]
        private void pf_web_clear_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // loping through all control ID's given
            foreach (var idxCtrlId in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget with given ID
                Container ctrl = FindControl<Container> (idxCtrlId, Page);

                // removing all Active Event handlers and all Ajax Event handlers for widget
                foreach (Control idx in ctrl.Controls) {
                    RemoveActiveEvents (idx);
                    RemoveEvents (idx);
                }

                // clearing child controls, and re-rendering widget
                ctrl.Controls.Clear ();
                ctrl.ReRenderChildren ();
            }
        }

        /// <summary>
        /// removes the widget with the id of the value of the [pf.web.remove-widget]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.remove-widget")]
        private void pf_web_remove_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // loping through all control ID's given
            foreach (var idxCtrlId in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget with given ID
                Widget widget = FindControl<Widget> (idxCtrlId, Page);

                // removing all Ajax event handlers for widget
                RemoveEvents (widget);

                // removing all Active Event handlers for widget
                RemoveActiveEvents (widget);

                // actually removing widget from Page control collection, and persisting our change
                Container parent = widget.Parent as Container;
                parent.RemoveControlPersistent (widget);
            }
        }

        /*
         * clears all Active Events for widget
         */
        private void RemoveActiveEvents (Control widget)
        {
            // removing all Active Events for given widget
            List<string> keysToRemove = new List<string> ();
            foreach (string idxKey in PageEvents.Keys) {
                List<Tuple<string, Node>> toRemove = new List<Tuple<string, Node>> ();
                foreach (var idxTuple in PageEvents [idxKey]) {
                    if (idxTuple.Item1 == widget.ID) {
                        toRemove.Add (idxTuple);
                    }
                }
                foreach (var idxToRemove in toRemove) {
                    PageEvents [idxKey].Remove (idxToRemove);
                }
                if (PageEvents [idxKey].Count == 0) {
                    keysToRemove.Add (idxKey);
                }
            }
            foreach (string idxKeyToRemove in keysToRemove) {
                PageEvents.Remove (idxKeyToRemove);
            }

            // looping through all child widgets, and removing those, by recursively calling self
            foreach (Control idxChild in widget.Controls) {
                RemoveActiveEvents (idxChild);
            }
        }

        /// <summary>
        /// reloads the current URL
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.reload-location")]
        private void pf_web_reload_location (ApplicationContext context, ActiveEventArgs e)
        {
            Manager.SendJavaScriptToClient ("location.reload();");
        }

        /// <summary>
        /// sends the given JavaScript to the client. JavaScript is given as value of [pf.web.include-javascript], and can
        /// be a constant, an expression or a formatting expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.include-javascript")]
        private void pf_web_include_javascript (ApplicationContext context, ActiveEventArgs e)
        {
            string js = XUtil.Single<string> (e.Args, context);
            Manager.SendJavaScriptToClient (js);
        }
        
        /// <summary>
        /// send the given string back to browser as JSON with the key given as value of [pf.web.return-value], and the string
        /// sent being the value of the first child of [pf.web.return-value]. the value to send, can either be an expression, a
        /// constant, or a node formatting expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.return-value")]
        private void pf_web_return_value (ApplicationContext context, ActiveEventArgs e)
        {
            string key = XUtil.Single<string> (e.Args, context);
            string str = XUtil.Single<string> (e.Args.LastChild, context);
            Manager.SendObject (key, str);
        }

        /// <summary>
        /// creates an ajax event containing pf.lambda code for the given widget's event
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "_pf.web.add-widget-event")]
        private void _pf_web_add_widget_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving widget id, and creating an event collection for the given widget
            Widget widget = e.Args.Get<Widget> (context);
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

        /// <summary>
        /// returns the control with the given ID as first child of args, from optionally [parent] control's ID given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "_pf.web.find-control")]
        private void _pf_web_find_control (ApplicationContext context, ActiveEventArgs e)
        {
            // defaulting parent to page object, but checking to see if an explicit parent is given through e.Args
            Control parentCtrl = Page;
            Node parentNode = e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "parent";
            });
            if (parentNode != null)
                parentCtrl = FindControl<Control> (parentNode.Get<string> (context), Page);

            // returning control as first child of e.Args
            e.Args.Insert (0, new Node (string.Empty, FindControl<Control> (e.Args.Get<string> (context), parentCtrl)));
        }

        /// <summary>
        /// includes a JavaScript file on the client side
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.add-javascript-file")]
        private void pf_web_add_javascript_file (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxFile in XUtil.Iterate<string> (e.Args, context)) {
                RegisterJavaScriptFile (idxFile);
            }
        }

        /// <summary>
        /// includes a stylesheet file on the client side
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.add-stylesheet-file")]
        private void pf_web_add_stylesheet_file (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxFile in XUtil.Iterate<string> (e.Args, context)) {
                RegisterStylesheetFile (idxFile);
            }
        }

        /// <summary>
        /// changes the "title" HTML element's value of the portal
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.set-title")]
        private void pf_web_set_title (ApplicationContext context, ActiveEventArgs e)
        {
            Title = XUtil.Single<string> (e.Args, context);
        }

        /*
         * recursively searches through page for Container with specified id, starting from "idx"
         */
        private T FindControl<T> (string id, Control idx) where T : Control
        {
            if (idx.ID == id)
                return idx as T;
            foreach (Control idxChild in idx.Controls) {
                T tmpRet = FindControl<T> (id, idxChild);
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
            node.Insert (1, new Node ("_form-id", node.Value));
            context.Raise ("pf.web.widgets.container", node);
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
    }
}
