/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using phosphorus.ajax.core;
using phosphorus.ajax.widgets;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
// ReSharper disable PartialTypeWithSinglePart

namespace phosphorus.five.applicationpool
{
    using pf = ajax.widgets;

    /// <summary>
    /// This is the main .aspx page for the Application Pool, and the only page in the system, from which
    /// all web requests are being handled. Contains many useful helper Active events for things such as including
    /// JavaScript files, StyleSheet files, creating and manipulating web widgets, etc.
    /// </summary>
    public partial class Default : AjaxPage
    {
        // Application Context for page life cycle
        private ApplicationContext _context;

        // main container for all widgets
        protected pf.Container container;

        /*
         * Contains all ajax events, and their associated pf.lambda code, for all controls on page. Please notice that
         * when we do this like this, we have to store the ViewState into the Session object, which we're doing automatically in
         * OnInit, since otherwise the server-side functionality will follow the page to the client, and allow for the client side
         * to tamper with the server-side functionality, which of course would be a major security concern, if being allowed.
         */
        private Dictionary<string, Dictionary<string, List<Node>>> AjaxEvents
        {
            get
            {
                if (ViewState ["AjaxEvents"] == null)
                    ViewState ["AjaxEvents"] = new Dictionary<string, Dictionary<string, List<Node>>> ();
                return ViewState ["AjaxEvents"] as Dictionary<string, Dictionary<string, List<Node>>>;
            }
        }

        /*
         * Contains all dynamically created "widget events", which are Active Events local for a specific widget,
         * created as parts of the [events] keyword. These events are being raised in the "null Active Event handler",
         * further down on the page, where we're doing a lookup for each Active Event raised by the system, to see if
         * there is a local page handler for that Active Event.
         * The dictionary "key" is the name of the Active Event, and the Item1 in the Tuple is the ID of the Widget
         * that "owns" the Active Event. Item2 in our Tuple, is a list of all [lambda.xxx] objects from our event handler
         */
        private Dictionary<string, List<Tuple<string, List<Node>>>> PageActiveEvents
        {
            get
            {
                if (ViewState ["PageActiveEvents"] == null) {
                    ViewState ["PageActiveEvents"] = new Dictionary<string, List<Tuple<string, List<Node>>>> ();
                }
                return (Dictionary<string, List<Tuple<string, List<Node>>>>) ViewState ["PageActiveEvents"];
            }
        }

        /*
         * Overridden to create context, and do other types of initialization, such as mapping up our Page_Load event,
         * and making sure ViewState is being stored on Server, etc.
         */
        protected override void OnInit (EventArgs e)
        {
            // retrieving viewstate entries per session
            // please notice that if you change the setting of this key to "0", then the ViewState is no
            // longer stored on the server, which is a major security concern, since it allows for pf.lambda
            // code to be "ViewState hacked"
            ViewStateSessionEntries = int.Parse (ConfigurationManager.AppSettings ["viewstate-per-session-entries"]);

            // creating our application context for current request
            _context = Loader.Instance.CreateApplicationContext ();

            // registering "this" web page as listener object, since page contains many Active Event handlers itself
            _context.RegisterListeningObject (this);

            // rewriting path to what was actually requested, such that HTML form element doesn't become garbage ...
            // this ensures that our HTML form element stays correct. basically "undoing" what was done in Global.asax.cs
            // in addition, when retrieving request URL later, we get the "correct" request URL, and not the URL to "Default.aspx"
            HttpContext.Current.RewritePath ((string) HttpContext.Current.Items ["__pf_original_url"]);

            // mapping up our Page_Load event for initial loading of web page
            if (!IsPostBack)
                Load += Page_LoadInitialLoading;

            // call base
            base.OnInit (e);
        }

        /*
         * Invoked only for the initial request of our web page, to make sure we load up our UI, passing in any arguments.
         * Not invoked during any consecutive POST requests
         */
        private void Page_LoadInitialLoading (object sender, EventArgs e)
        {
            // raising our [pf.web.load-ui] Active Event, creating the node to pass in first
            // where the [_form] node becomes the name of the form requested, and all other HTTP GET arguments
            // are passed in through [_args]
            var args = new Node ("pf.web.load-ui");
            args.Add (new Node ("_form", (string) HttpContext.Current.Items ["__pf_original_url"]));

            // making sure we pass in any HTTP GET parameters
            if (Request.QueryString.Keys.Count > 0) {
                // retrieving all GET parameters and passing in as [_args]
                args.Add (new Node ("_args"));
                foreach (var idxArg in Request.QueryString.AllKeys) {
                    args.LastChild.Add (new Node (idxArg, Request.QueryString [idxArg]));
                }
            }

            // invoking the Active Event that actually loads our UI, now with a [_file] node, and possibly an [_args] node
            _context.Raise ("pf.web.load-ui", args);
        }

        /// <summary>
        ///     Creates a web widget. Optionally declare the type of widget you wish to create as [widget], and which parent 
        ///     widget it should have as [parent]. You can also optionally declare where you wish to position your widget, by
        ///     using either [before] or [after], to make sure your widget becomes either before some specific widget, or after
        ///     it. Internally, this Active Event uses the [pf.web.widgets.xxx] Active Events to actually create your widget.
        ///     See any of those Active Events to see what types of properties you can further decorate your widget with, which
        ///     varies according to what type of [widget] you are creating. [widget] defaults to "container", and [parent] defaults
        ///     to "container", which is your main root widget on the page. If neither [before] nor [after] is given, widget will
        ///     be appended into controls collection at the end of whatever [parent] widget you choose to use. You can also optionally
        ///     declare local widget Active Events, by passing in an [events] node, which will be locally declared Active Events for
        ///     your widget, only active, as long as Widget exists on page.
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.create")]
        [ActiveEvent (Name = "pf.web.create-widget")]
        private void pf_web_widgets_create (ApplicationContext context, ActiveEventArgs e)
        {
            // finding parent widget first, which defaults to "container" widget, if no parent is given
            var parentNode = e.Args.Find (idx => idx.Name == "parent" && idx.Value != null);
            var parent = (parentNode != null ? FindControl<pf.Container> (XUtil.Single<string> (parentNode, context), Page) : container) ?? container;

            // finding position in parent control's list, defaulting to "-1", meaning "append at end"
            int position = GetWidgetPosition (e.Args, context, parent);

            // finding type of widget
            string type = "container";
            var widgetType = e.Args.Find (idx => idx.Name == "widget" && idx.Value != null);
            if (widgetType != null)
                type = XUtil.Single<string> (widgetType, context);

            // creating widget. since CreateForm modifies node given, by e.g. adding the parent widget as [_parent],
            // we need to make sure the nodes are set back to wwhat they were before the invocation of our Active Event
            var widget = CreateWidget (context, e.Args, parent, position, type);

            // initializing Active Events for widget, if there are any given
            var eventNode = e.Args.Find (idx => idx.Name == "events");
            if (eventNode != null)
                CreateWidgetEvents (widget, eventNode, context);
        }

        /// <summary>
        ///     clears the given widget, removing all its children widgets
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.clear")]
        private void pf_web_widgets_clear (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking
            if (e.Args.Value == null)
                return; // nothing to do here

            // loping through all control ID's given
            foreach (var ctrl in XUtil.Iterate<string> (e.Args, context).Select (idxCtrlId => FindControl<pf.Container> (idxCtrlId, Page))) {

                // checking if widget exists
                if (ctrl == null)
                    continue;

                // then looping through all of its children controls
                foreach (Control innerCtrl in ctrl.Controls) {
                    RemoveActiveEvents (innerCtrl);
                    RemoveEvents (innerCtrl);
                }

                // clearing child controls, and re-rendering widget
                ctrl.Controls.Clear ();
                ctrl.ReRenderChildren ();
            }
        }

        /// <summary>
        ///     Removes the given widget(s) entirely.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.remove")]
        private void pf_web_widgets_remove (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking
            if (e.Args.Value == null)
                return; // nothing to do here

            // loping through all control ID's given
            foreach (var widget in XUtil.Iterate<string> (e.Args, context).Select (idxCtrlId => FindControl<pf.Widget> (idxCtrlId, Page))) {

                // checking if widget exists
                if (widget == null)
                    continue;

                // removing all Ajax event handlers for widget
                RemoveEvents (widget);

                // removing all Active Event handlers for widget
                RemoveActiveEvents (widget);

                // actually removing widget from Page control collection, and persisting our change
                var parent = (pf.Container)widget.Parent;
                parent.RemoveControlPersistent (widget);
            }
        }
        
        /// <summary>
        ///     Lists all widgets on page. Alternatively supply a filter as argument.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.list")]
        private void pf_web_widgets_list (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving filter, if any
            var filter = new List<string> (XUtil.Iterate<string> (XUtil.TryFormat<object> (e.Args, context, null), e.Args, context));
            if (e.Args.Value != null && filter.Count == 0)
                return; // possibly a filter expression, leading into oblivion

            // recursively retrieving all widgets on page
            ListWidgets (filter, e.Args, container);
        }

        /// <summary>
        ///     Returns the given event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.event.get")]
        private void pf_web_widgets_event_get (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking
            if (e.Args.Value == null)
                return; // nothing to do here

            // looping through all widgets
            foreach (var idxCtrlId in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget
                var widget = FindControl<Widget> (idxCtrlId, Page);
                if (widget == null || !AjaxEvents.ContainsKey (widget.ID))
                    continue; // widget doesn't exist, or has no events

                // checking if event exists for widget, and removing it if it does
                Node nodeToAdd = null;
                foreach (var idxEventNameNode in new List<Node> (e.Args.Children)) {
                    if (AjaxEvents [widget.ID].ContainsKey (idxEventNameNode.Name)) {
                        if (nodeToAdd == null)
                            nodeToAdd = e.Args.Add (widget.ID).LastChild;
                        Node evtNode = nodeToAdd.Add (idxEventNameNode.Name).LastChild;
                        foreach (var idx in AjaxEvents [widget.ID] [idxEventNameNode.Name]) {
                            evtNode.AddRange (idx.Clone ().Children);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the given event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.event.set")]
        private void pf_web_widgets_event_set (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking
            if (e.Args.Value == null)
                return; // nothing to do here

            // looping through all widgets
            foreach (var idxCtrlId in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget
                var widget = FindControl<Widget> (idxCtrlId, Page);
                if (widget == null)
                    continue; // widget doesn't exist, or has no events

                // looping through all events we should set
                foreach (var idxEventNameNode in e.Args.Children) {
                    if (!AjaxEvents.ContainsKey (widget.ID))
                        AjaxEvents [widget.ID] = new Dictionary<string, List<Node>> ();
                    AjaxEvents [widget.ID] [idxEventNameNode.Name] = new List<Node> ();
                    Node curEventNode = idxEventNameNode.Clone ();
                    AjaxEvents [widget.ID] [idxEventNameNode.Name].Add (curEventNode);
                }
            }
        }

        /// <summary>
        ///     Removes the given event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.event.remove")]
        private void pf_web_widgets_event_remove (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking
            if (e.Args.Value == null)
                return; // nothing to do here

            // looping through all widgets
            foreach (var idxCtrlId in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget
                var widget = FindControl<Widget> (idxCtrlId, Page);
                if (widget == null || !AjaxEvents.ContainsKey (widget.ID))
                    continue; // widget doesn't exist, or has no events

                // checking if event exists for widget, and removing it if it does
                foreach (var idxEventNameNode in e.Args.Children) {
                    if (AjaxEvents [widget.ID].ContainsKey (idxEventNameNode.Name))
                        AjaxEvents [widget.ID].Remove (idxEventNameNode.Name);
                    widget.RemoveAttribute (idxEventNameNode.Name);
                }
                if (AjaxEvents [widget.ID].Count == 0)
                    AjaxEvents.Remove (widget.ID);
            }
        }
        
        /// <summary>
        ///     Lists all existing events for given widget(s).
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.event.list")]
        private void pf_web_widgets_event_list (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through all widgets
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget
                var widget = FindControl<Widget> (idx, Page);
                if (widget == null)
                    continue;

                // then looping through all attribute keys, filtering everything out that does not start with "on"
                Node curNode = null;
                foreach (var idxAtr in widget.AttributeKeys) {
                    if (!idxAtr.StartsWith ("on"))
                        continue;
                    if (curNode == null)
                        curNode = e.Args.Add (widget.ID).LastChild;
                    curNode.Add (idxAtr);
                }
            }
        }

        /// <summary>
        ///     Null Active Event handler, for handling widget specific Active Events
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "")]
        private void null_handler (ApplicationContext context, ActiveEventArgs e)
        {
            // checking to see if the currently raised Active Event has a handler on our page
            if (!PageActiveEvents.ContainsKey (e.Name)) return;

            // keeping a reference to what we add to the current Active Event "root node"
            // such that we can clean up after ourselves afterwards
            var lambdas = new List<Node> ();

            // looping through each Active Event handler for current event
            foreach (var tmp in from idxEvt in PageActiveEvents [e.Name] from idxEvtContent in idxEvt.Item2 select idxEvtContent.Clone()) {
                e.Args.Add(tmp);
                lambdas.Add(tmp);
            }

            // invoking each [lambda.xxx] object from event
            foreach (var idxLambda in lambdas) {
                context.Raise (idxLambda.Name, idxLambda);
            }

            // cleaning up after ourselves, deleting only the lambda objects that came
            // from our dynamically created event
            foreach (var idxLambda in lambdas) {
                idxLambda.UnTie ();
            }
        }

        /*
         * Helper for above. Retrieves the position user requested widget to be inserted at
         */
        private int GetWidgetPosition (Node node, ApplicationContext context, Widget parent)
        {
            int position = -1;
            var beforeNode = node.Find (idx => idx.Name == "before" && idx.Value != null);
            if (beforeNode != null) {
                var beforeControl = FindControl<pf.Widget> (XUtil.Single<string> (beforeNode, context), parent);
                position = parent.Controls.IndexOf (beforeControl);
            } else {
                var afterNode = node.Find (idx => idx.Name == "after" && idx.Value != null);
                if (afterNode != null) {
                    var afterControl = FindControl<pf.Widget> (XUtil.Single<string> (afterNode, context), parent);
                    position = parent.Controls.IndexOf (afterControl) + 1;
                }
            }
            return position;
        }

        /*
         * creates widget according to node given, and returns to caller
         */
        private static Widget CreateWidget (
            ApplicationContext context, 
            Node node, 
            pf.Container parent, 
            int position, 
            string type)
        {
            // making sure the original node hierarchy is reset back to what it was after creation
            // since the creation Active Events changes the node hierarchy in all sorts of different ways
            var originalChildren = node.Clone();
            node.Insert (0, new Node ("__parent", parent));
            node.Insert (1, new Node ("_widget", node.Value));
            node.Insert (2, new Node ("_position", position));

            // raising the Active Event that actually creates our widget, and retrieving the created widget afterwards
            context.Raise ("pf.web.widgets." + type, node);
            var widget = node.Get<Widget> (context);

            // cleaning up our node structure afterwards
            node.Clear ();
            node.AddRange (originalChildren.Children);
            node.Value = originalChildren.Value;

            // returning widget back to caller
            return widget;
        }

        /*
         * creates local widget events for web widgets created through [pf.web.widgets.create]
         */
        private void CreateWidgetEvents (Control widget, Node eventNode, ApplicationContext context)
        {
            foreach (var idxEvt in XUtil.Iterate<Node> (eventNode, context, true)) {

                // checking to see if there's already an existing event with the given name
                if (!PageActiveEvents.ContainsKey (idxEvt.Name)) {
                    PageActiveEvents [idxEvt.Name] = new List<Tuple<string, List<Node>>> ();
                }

                // adding event, cloning node
                var tpl = new Tuple<string, List<Node>> (widget.ID, new List<Node> ());
                foreach (var idxLambda in idxEvt.FindAll (idxEvtChild => idxEvtChild.Name.StartsWith ("lambda", StringComparison.Ordinal))) {
                    tpl.Item2.Add (idxLambda.Clone ());
                }
                PageActiveEvents[idxEvt.Name].Add(tpl);
            }
        }

        /*
         * clears all Active Events for widget
         */
        private void RemoveActiveEvents (Control widget)
        {
            // removing all Active Events for given widget
            var keysToRemove = new List<string> ();
            foreach (var idxKey in PageActiveEvents.Keys) {
                var toRemove = PageActiveEvents [idxKey].Where (idxTuple => idxTuple.Item1 == widget.ID).ToList ();
                foreach (var idxToRemove in toRemove) {
                    PageActiveEvents [idxKey].Remove (idxToRemove);
                }
                if (PageActiveEvents [idxKey].Count == 0) {
                    keysToRemove.Add (idxKey);
                }
            }
            foreach (var idxKeyToRemove in keysToRemove) {
                PageActiveEvents.Remove (idxKeyToRemove);
            }

            // looping through all child widgets, and removing those, by recursively calling self
            foreach (Control idxChild in widget.Controls) {
                RemoveActiveEvents (idxChild);
            }
        }

        // TODO: do we really need this guy?
        /// <summary>
        ///     reloads the current URL
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.reload-location")]
        private void pf_web_reload_location (ApplicationContext context, ActiveEventArgs e)
        {
            Manager.SendJavaScriptToClient ("location.reload();");
        }

        /// <summary>
        ///     sends the given JavaScript to the client. JavaScript is given as value of [pf.web.include-javascript], and can
        ///     be a constant, an expression or a formatting expression
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.include-javascript")]
        private void pf_web_include_javascript (ApplicationContext context, ActiveEventArgs e)
        {
            var js = XUtil.Single<string> (e.Args, context);
            Manager.SendJavaScriptToClient (js);
        }

        // TODO: support [re-source], the same way we do in [set] in this guy
        /// <summary>
        ///     send the given string back to browser as JSON with the key given as value of [pf.web.return-value], and the string
        ///     sent being the value of the first child of [pf.web.return-value]. the value to send, can either be an expression, a
        ///     constant, or a node formatting expression
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.return-value")]
        private void pf_web_return_value (ApplicationContext context, ActiveEventArgs e)
        {
            var key = XUtil.Single<string> (e.Args, context);
            var str = XUtil.Single<string> (e.Args.LastChild, context);
            Manager.SendObject (key, str);
        }

        /// <summary>
        ///     creates an ajax event containing pf.lambda code for the given widget's event
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "_pf.web.add-widget-event")]
        private void _pf_web_add_widget_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving widget id, and creating an event collection for the given widget
            var widget = e.Args.Get<pf.Widget> (context);
            if (!AjaxEvents.ContainsKey (widget.ID))
                AjaxEvents [widget.ID] = new Dictionary<string, List<Node>> ();

            // creating an event collection for the given event for the given widget. notice that one widget might
            // create multiple pf.lambda objects for the same event, meaning one widget might have several ajax event handlers
            // for the same ajax event
            var eventName = e.Args [0].Name;
            if (!AjaxEvents [widget.ID].ContainsKey (eventName))
                AjaxEvents [widget.ID] [eventName] = new List<Node> ();

            // appending our pf.lambda object to the list of pf.lambda objects for the given widget's given event
            AjaxEvents [widget.ID] [eventName].Add (e.Args [0].Clone ());

            // mapping the widget's ajax event to our common event handler on page
            widget [eventName] = "common_event_handler";
        }

        /// <summary>
        ///     returns the control with the given ID as first child of args, from optionally [parent] control's ID given
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "_pf.web.find-control")]
        private void _pf_web_find_control (ApplicationContext context, ActiveEventArgs e)
        {
            // defaulting parent to page object, but checking to see if an explicit parent is given through e.Args
            Control parentCtrl = Page;
            var parentNode = e.Args.Find (idx => idx.Name == "parent");
            if (parentNode != null)
                parentCtrl = FindControl<Control> (parentNode.Get<string> (context), Page);

            // returning control as first child of e.Args
            e.Args.Insert (0, new Node (string.Empty, FindControl<Control> (e.Args.Get<string> (context), parentCtrl)));
        }

        /// <summary>
        ///     includes a JavaScript file on the client side
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.add-javascript-file")]
        private void pf_web_add_javascript_file (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxFile in XUtil.Iterate<string> (e.Args, context)) {
                RegisterJavaScriptFile (idxFile);
            }
        }

        /// <summary>
        ///     includes a stylesheet file on the client side
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.add-stylesheet-file")]
        private void pf_web_add_stylesheet_file (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxFile in XUtil.Iterate<string> (e.Args, context)) {
                RegisterStylesheetFile (idxFile);
            }
        }

        /// <summary>
        ///     changes the "title" HTML element's value of the portal
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.set-title")]
        private void pf_web_set_title (ApplicationContext context, ActiveEventArgs e)
        {
            if (Manager.IsPhosphorusRequest)
                throw new Exception ("You cannot set the title of your page in an Ajax Request, only during post requests, or initial loading of page");
            Title = XUtil.Single<string> (e.Args, context);
        }

        /*
         * recursively traverses entire Control hierarchy on page, and adds up into result node
         */
        private static void ListWidgets (List<string> filter, Node resultNode, Control current)
        {
            bool shouldAdd = filter.Count == 0;
            if (!shouldAdd) {
                foreach (var idxFilter in filter) {
                    if (current.ID.IndexOf (idxFilter) != -1) {
                        shouldAdd = true;
                        break;
                    }
                }
            }
            if (!shouldAdd)
                return; // didn't match filter

            resultNode.Add (current.ID);
            foreach (Control idxChild in current.Controls) {
                ListWidgets (filter, resultNode, idxChild);
            }
        }

        /*
         * recursively searches through page for Container with specified id, starting from "idx"
         */
        private T FindControl<T> (string id, Control idx) where T : Control
        {
            if (idx.ID == id)
                return idx as T;
            return (from Control idxChild in idx.Controls select FindControl<T> (id, idxChild)).FirstOrDefault (tmpRet => tmpRet != null);
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
        protected void common_event_handler (pf.Widget sender, pf.Widget.AjaxEventArgs e)
        {
            var id = sender.ID;
            var eventName = e.Name;
            var lambdas = AjaxEvents [id] [eventName];
            foreach (var idxLambda in lambdas) {
                _context.Raise ("lambda", idxLambda.Clone ());
            }
        }
    }
}