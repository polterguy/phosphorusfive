/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.ajax.core;
using p5.ajax.widgets;

/// <summary>
///     Main namespace for your Application Pool.
/// 
///     Your Application Pool is your "web project", and where everything happens, if you use Phosphorus Five to create
///     web applications. Normally you do not need to change anything at all in here, or even fiddle with, or realize that
///     there even is an Application Pool created for you, since you can use Phosphorus Five entirely through its Active Event
///     system, and create plugins that automatically plugs into your Application Pool.
/// 
///     However, if you wish to achieve more detailed control over your end web project, then this is where you'd do just that.
/// </summary>
namespace p5.webapp
{
    using pf = p5.ajax.widgets;

    /// <summary>
    ///     Main .aspx web page for your Application Pool.
    /// 
    ///     This is the main, and only, .aspx page for your Application Pool, from which all web requests are being handled.
    ///     It contains many useful helper Active events, for things such as including JavaScript files, including StyleSheet files,
    ///     creating and manipulating web widgets, etc.
    /// </summary>
    public partial class Default : AjaxPage
    {
        // Application Context for page life cycle
        private ApplicationContext _context;

        // main container for all widgets
        protected pf.Container cnt;

        /*
         * Contains all ajax events, and their associated p5.lambda code, for all controls on page. Please notice that
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
         * Contains all additional data associated with widgets on page.
         */
        private Dictionary<string, Node> WidgetData
        {
            get
            {
                if (ViewState ["WidgetData"] == null) {
                    ViewState ["WidgetData"] = new Dictionary<string, Node> ();
                }
                return (Dictionary<string, Node>) ViewState ["WidgetData"];
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
            // longer stored on the server, which is a major security concern, since it allows for p5.lambda
            // code to be "ViewState hacked"
            ViewStateSessionEntries = int.Parse (ConfigurationManager.AppSettings ["viewstate-per-session-entries"]);

            // creating our application context for current request
            _context = Loader.Instance.CreateApplicationContext ();

            // registering "this" web page as listener object, since page contains many Active Event handlers itself
            _context.RegisterListeningObject (this);

            // rewriting path to what was actually requested, such that HTML form element doesn't become garbage ...
            // this ensures that our HTML form element stays correct. basically "undoing" what was done in Global.asax.cs
            // in addition, when retrieving request URL later, we get the "correct" request URL, and not the URL to "Default.aspx"
            HttpContext.Current.RewritePath ((string) HttpContext.Current.Items ["__p5_original_url"]);

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
            // raising our [p5.web.load-ui] Active Event, creating the node to pass in first
            // where the [_form] node becomes the name of the form requested
            var args = new Node ("p5.web.load-ui");
            args.Add (new Node ("_form", (string) HttpContext.Current.Items ["__p5_original_url"]));

            // invoking the Active Event that actually loads our UI, now with a [_file] node, and possibly an [_args] node
            _context.Raise ("p5.web.load-ui", args);
        }

        /// <summary>
        ///     Creates a container web widget.
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-widget")]
        [ActiveEvent (Name = "create-container-widget")]
        private void create_widget (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget (context, e.Args, "container");
        }

        
        /// <summary>
        ///     Creates a literal web widget.
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-literal-widget")]
        private void create_literal_widget (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget (context, e.Args, "literal");
        }

        /// <summary>
        ///     Creates a void web widget.
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-void-widget")]
        private void create_void_widget (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget (context, e.Args, "void");
        }

        /// <summary>
        ///     Removes the given widget(s) entirely.
        ///
        ///     Removes the specified widget from your page in its entirety, including all of its children widgets.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-widget")]
        private void delete_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking
            if (e.Args.Value == null)
                return; // nothing to do here

            // loping through all control ID's given
            foreach (var widget in XUtil.Iterate<string> (e.Args, context).Select (idxCtrlId => FindControl<pf.Widget> (idxCtrlId, Page))) {

                // checking if widget exists
                if (widget == null)
                    continue;

                // removing all Active Event handlers for widget
                RemoveActiveEvents (widget);
                
                // removing all Ajax event handlers for widget
                RemoveEvents (widget);

                // removes all additional data associated with widget
                RemoveData (widget);

                // actually removing widget from Page control collection, and persisting our change
                var parent = (pf.Container)widget.Parent;
                parent.RemoveControlPersistent (widget);
            }
        }
        
        /// <summary>
        ///     Clears the given widget, removing all its children widgets.
        ///
        ///     Empties your Widget's Controls collection entirely, but leaving the widget itself. Useful for entirely emptying 
        ///     your widget, removing all of its children widgets.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "clear-widget")]
        private void clear_widget (ApplicationContext context, ActiveEventArgs e)
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
                var list = new List<Control>();
                foreach (var idx in ctrl.Controls) {
                    list.Add((Control)idx);
                }
                foreach (Control innerCtrl in list) {
                    RemoveActiveEvents (innerCtrl);
                    RemoveEvents (innerCtrl);
                    RemoveData (innerCtrl);

                    // actually removing widget from Page control collection, and persisting our change
                    ctrl.RemoveControlPersistent (innerCtrl);
                }
            }
        }

        /// <summary>
        ///     Returns the ID and type of the given widget's parent.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-parent-widget")]
        private void get_parent_widget (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                var ctrl = FindControl<pf.Widget>(idx, Page);
                if (ctrl == null)
                    continue;
                ctrl = ctrl.Parent as Widget;
                if (ctrl == null)
                    continue;
                string type = ctrl.GetType().FullName;
                type = type.Substring(type.LastIndexOf(".") + 1).ToLower();
                e.Args.Add (type, ctrl.ID);
            }
        }

        /// <summary>
        ///     Returns the ID and type of the given widget's children.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-children-widgets")]
        private void get_children_widgets (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                var ctrl = FindControl<pf.Container>(idx, Page);
                if (ctrl == null)
                    continue;
                e.Args.Add(ctrl.ID);
                foreach (Control idxCtrl in ctrl.Controls) {

                    // looping through all children of currently iterated widget
                    string type = idxCtrl.GetType().FullName;
                    type = type.Substring(type.LastIndexOf(".") + 1).ToLower();
                    e.Args.LastChild.Add(type, idxCtrl.ID);
                }
            }
        }

        /// <summary>
        ///     Lists all widgets on page.
        ///
        ///     Alternatively supply a filter as argument. If you do, then only widgets matching your filter will be returned.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widgets")]
        private void list_widgets (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving filter, if any
            var filter = new List<string> (XUtil.Iterate<string> (e.Args, context));
            if (e.Args.Value != null && filter.Count == 0)
                return; // possibly a filter expression, leading into oblivion

            // recursively retrieving all widgets on page
            ListWidgets (filter, e.Args, cnt);
        }
        
        /// <summary>
        ///     Sets additional data associated with widget(s).
        ///
        ///     This additional "data" is never rendered to the client, but exclusively kept on the server, which allows
        ///     you to associate pieces of additional information coupled with your widgets, such as "database IDs", etc.
        ///     The Widget(s) you wish to associate your data with, is passed in as the value of your main Node, the data
        ///     you wish to associate with your widget, is passed in as children nodes of your main Node.
        ///
        ///     If you remove your Widget from your page, somehow, then this data is automatically removed too.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-widget-info")]
        private void set_widget_info (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.Count == 0)
                return; // nothing to do here ...

            // looping through all widget IDs given by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget, to make sure it exists
                var widget = FindControl<Widget> (idx, Page);
                if (widget == null)
                    continue; // widget doesn't exist

                // looping through all keys beneath args caller wishes to set for widget
                foreach (var idxKeyNode in e.Args.Children.ToList ()) {
                    if (idxKeyNode.Name == string.Empty)
                        continue; // formatting parameter

                    // making sure we've got a key for our widget
                    if (!WidgetData.ContainsKey (widget.ID))
                        WidgetData [widget.ID] = new Node ();

                    // removing any pre-existing data with the same key
                    if (WidgetData [widget.ID].Find (idxKeyNode.Name) != null)
                        WidgetData [widget.ID].Find (idxKeyNode.Name).UnTie ();

                    // adding current node as additional data node associated with widget
                    WidgetData [widget.ID].Add (idxKeyNode.Clone ());
                }
            }
        }

        /// <summary>
        ///     Retrieves additional data associated with widget(s).
        ///
        ///     Returns any additional information you have stored with your widget. Which pieces of data
        ///     you wish to retrieve, must be defined through the names of the children of the main Node when
        ///     invoking this Active Event.
        ///
        ///     Which widget to retrieve data for, is given as the value of the main node when invoking this Active Event.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-info")]
        private void get_widget_info (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.Count == 0)
                return; // nothing to do here ...

            // looping through all widget IDs given by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget, to make sure it exists
                var widget = FindControl<Widget> (idx, Page);
                if (widget == null || !WidgetData.ContainsKey (widget.ID))
                    continue; // widget doesn't exist, or has no additional data associated with it

                // looping through all keys beneath args caller wishes to retrieve for widget
                Node widgetResultNode = null;
                foreach (var idxKeyNode in e.Args.Children.ToList ()) {
                    if (idxKeyNode.Name == string.Empty)
                        continue; // formatting parameter

                    // making sure we've got a key for our widget
                    if (WidgetData[widget.ID].Find (idxKeyNode.Name) == null)
                        continue; // this data segment doesn't exist for widget

                    // making sure we've got a result node for our current widget
                    if (widgetResultNode == null)
                        widgetResultNode = e.Args.Add (widget.ID).LastChild;

                    // returning data to caller
                    widgetResultNode.Add (WidgetData[widget.ID].Find (idxKeyNode.Name).Clone ());
                }
            }
        }

        /// <summary>
        ///     Removes additional data associated with widget(s).
        /// 
        ///     Which widget you wish to remove data for, is given as value of main Node. Which data you wish to
        ///     remove, is defined through the children nodes of your Active Event invocation.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-widget-info")]
        private void delete_widget_info (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.Count == 0)
                return; // nothing to do here ...

            // looping through all widget IDs given by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget, to make sure it exists
                var widget = FindControl<Widget> (idx, Page);
                if (widget == null)
                    continue; // widget doesn't exist

                // looping through all keys beneath args caller wishes to set for widget
                foreach (var idxKeyNode in e.Args.Children) {
                    if (idxKeyNode.Name == string.Empty)
                        continue; // formatting parameter

                    // making sure we've got a key for our widget
                    if (!WidgetData.ContainsKey(widget.ID))
                        continue;

                    // removing existing data with the given key
                    if (WidgetData [widget.ID].Find (idxKeyNode.Name) != null)
                        WidgetData [widget.ID].Find (idxKeyNode.Name).UnTie ();
                }
            }
        }

        /// <summary>
        ///     Lists all additional data associated with widget(s).
        /// 
        ///     Returns a list of all keys of additional data that is associated with your Widget. Supply what widget
        ///     you're interested in querying as the value of the main Node when invoking this Active Event.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-info")]
        private void list_widget_info (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here ...

            // looping through all widget IDs given by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget, to make sure it exists
                var widget = FindControl<Widget> (idx, Page);
                if (widget == null || !WidgetData.ContainsKey (widget.ID))
                    continue; // widget doesn't exist, or has no additional data associated with it

                // loping through all data keys for current widget
                Node widgetResultNode = null;
                foreach (Node idxResult in WidgetData [widget.ID].Children) {
                    if (widgetResultNode == null)
                        widgetResultNode = e.Args.Add (widget.ID).LastChild;

                    // returning key to caller
                    widgetResultNode.Add (idxResult.Name);
                }
            }
        }

        /// <summary>
        ///     Returns the given event(s) for the given widget(s).
        /// 
        ///     Returns Active Events associated with your widget, matching the names of the children nodes
        ///     of the main node used when invoking this Active Event.
        /// 
        ///     Supply which Widget you wish to retrieve events for ass the value of your main Node when invoking this
        ///     Active Event, and what events you wish to retrieve as the names of the children nodes of your main node.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-event")]
        private void get_widget_event (ApplicationContext context, ActiveEventArgs e)
        {
            // syntax checking
            if (e.Args.Value == null)
                return; // nothing to do here

            // looping through all widgets
            foreach (var idxCtrlId in XUtil.Iterate<string> (e.Args, context)) {
                
                // finding widget
                var widget = FindControl<Widget>(idxCtrlId, Page);
                if (widget == null)
                    continue;

                // looping through events requested by caller
                foreach (var idxEventNameNode in new List<Node> (e.Args.Children)) {

                    Node nodeToAdd = new Node(widget.ID);
                    if (AjaxEvents.ContainsKey(widget.ID) && AjaxEvents[widget.ID].ContainsKey(idxEventNameNode.Name)) {

                        // checking if current child name is an event for current widget
                        if (AjaxEvents[widget.ID].ContainsKey(idxEventNameNode.Name)) {

                            Node evtNode = nodeToAdd.Add(idxEventNameNode.Name).LastChild;
                            foreach (var idx in AjaxEvents [widget.ID] [idxEventNameNode.Name]) {

                                evtNode.AddRange(idx.Clone().Children);
                            }
                        }
                    } else if (widget.HasAttribute(idxEventNameNode.Name)) {
                        nodeToAdd.Add(idxEventNameNode.Name + "-script", widget[idxEventNameNode.Name]); // javascript!*/
                    }
                    if (nodeToAdd.Count > 0)
                        e.Args.Add(nodeToAdd);
                }
            }
        }

        /// <summary>
        ///     Changes the given event(s) for the given widget(s).
        /// 
        ///     Allows you to change one or more event(s) for one or more Widget(s). The widget you wish to change the 
        ///     Active Events for is given as the value of your main node when invoking this Active Event. The events
        ///     you wish to change, and their new content, is given as children nodes of your main node when invoking 
        ///     this Active Event.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-widget-event")]
        private void set_widget_event (ApplicationContext context, ActiveEventArgs e)
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
        ///     Removes the given event(s) for the given widget(s).
        /// 
        ///     The Widget you wish to remove Active Events from is given as the value of your main node 
        ///     when invoking this Active Event. The Active Events you wish to remove, is defined as the names of the children 
        ///     of your main node when invoking this Active Event.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "remove-widget-event")]
        private void remove_widget_event (ApplicationContext context, ActiveEventArgs e)
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
        /// 
        ///     Returns a list of all Active Events for the specified Widget(s) defined as the value of your main node
        ///     when invoking this Active Event.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-events")]
        private void list_widget_events (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through all widgets
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget
                var widget = FindControl<Widget> (idx, Page);
                if (widget == null)
                    continue;

                // then looping through all attribute keys, filtering everything out that does not start with "on"
                Node curNode = new Node (widget.ID);
                foreach (var idxAtr in widget.AttributeKeys) {
                    if (!idxAtr.StartsWith ("on"))
                        continue;
                    curNode.Add (idxAtr);
                }
                if (curNode.Count > 0)
                    e.Args.Add(curNode);
            }
        }

        /// <summary>
        ///     Sends the given JavaScript to the client.
        /// 
        ///     JavaScript is given as value of main node.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "include-javascript")]
        private void include_javascript (ApplicationContext context, ActiveEventArgs e)
        {
            var js = XUtil.Single<string> (e.Args, context);
            Manager.SendJavaScriptToClient (js);
        }

        /// \todo implement inclusion of JavaScript files become async. Today it's included using a non-async GET request.
        /// \todo Make sure inclusion of JavaScript files, and inclusion of JavaScript content is being executed sequentially,
        // such that if I include JavaScript first, for then to include a file, then the JavaScript is executed before the
        // file is downloaded, vice versa. This is a problem with among other things the CK Editor in System42.
        /// <summary>
        ///     Includes JavaScript file(s) on the client side.
        /// 
        ///     Using this method, you can dynamically declare JavaScript you wish to have included on the client-side
        ///     in either Ajax requests, normal POST HTTP requests, or the initial loading of your page.
        /// 
        ///     The framework itself, will automatically take care of including your JavaScript files on the client-side.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "include-javascript-file")]
        private void include_javascript_file (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxFile in XUtil.Iterate<string> (e.Args, context)) {
                RegisterJavaScriptFile (idxFile);
            }
        }

        /// \todo implement support for including CSS files during Ajax requests.
        /// \todo implement support for including CSS directly, without having a css file where css is included
        /// <summary>
        ///     Includes CSS StyleSheet file(s) on the client side.
        /// 
        ///     Using this method, you can dynamically declare CSS StyleSheet files you wish to have included on the client-side
        ///     in either Ajax requests, normal POST HTTP requests, or the initial loading of your page.
        /// 
        ///     The framework itself, will automatically take care of including your JavaScript files on the client-side.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "include-stylesheet-file")]
        private void include_stylesheet_file (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idxFile in XUtil.Iterate<string> (e.Args, context)) {
                RegisterStylesheetFile (idxFile);
            }
        }

        /// <summary>
        ///     Changes the title of your web page.
        /// 
        ///     Changes the "title" HTML element's content of your web page.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-title")]
        private void set_title (ApplicationContext context, ActiveEventArgs e)
        {
            if (Manager.IsPhosphorusRequest) {
                Manager.SendJavaScriptToClient (string.Format ("document.title='{0}';", XUtil.Single<string>(e.Args, context).Replace ("'", "\\'")));
            }
            Title = XUtil.Single<string>(e.Args, context);
            ViewState["P5-Title"] = Title;
        }

        /// <summary>
        ///     Returns the title of your web page.
        /// 
        ///     Returns the "title" HTML element's content.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-title")]
        private void get_title (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = ViewState ["P5-Title"] ?? Title;
        }

        /// <summary>
        ///     Changes the URL/location of your web page.
        /// 
        ///     Changes the "location" of your web page. This will load another document for you.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-location")]
        private void set_location (ApplicationContext context, ActiveEventArgs e)
        {
            if (Manager.IsPhosphorusRequest) {
                Manager.SendJavaScriptToClient (string.Format ("window.location='{0}';", XUtil.Single<string> (e.Args, context)));
            } else {
                Page.Response.Redirect (XUtil.Single<string> (e.Args, context));
            }
        }

        /// <summary>
        ///     Returns the URL/location of your web page.
        /// 
        ///     Returns the URL of your page.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-location")]
        private void get_location (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Request.Url.ToString ();
        }

        /// \todo use XUtil.SourceSingle
        /// <summary>
        ///     Returns the given Node back to client as JSON.
        /// 
        ///     Sends the given Node back to client as JSON, with the key given as value of main Node, and the content sent being
        ///     its children nodes.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "return-value")]
        private void return_value (ApplicationContext context, ActiveEventArgs e)
        {
            var key = XUtil.Single<string> (e.Args, context);
            var str = XUtil.Single<string> (e.Args.LastChild, context);
            Manager.SendObject (key, str);
        }

        /// <summary>
        ///     Creates an ajax event containing p5.lambda code for the given widget's event.
        /// 
        ///     Used internally by plugins that needs to declare Ajax Events for Widgets, containing p5.lambda code to be executed,
        ///     upon callbacks to server.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "_p5.web.add-widget-event")]
        private void _p5_web_add_widget_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving widget id, and creating an event collection for the given widget
            var widget = e.Args.Get<pf.Widget> (context);
            if (!AjaxEvents.ContainsKey (widget.ID))
                AjaxEvents [widget.ID] = new Dictionary<string, List<Node>> ();

            // creating an event collection for the given event for the given widget. notice that one widget might
            // create multiple p5.lambda objects for the same event, meaning one widget might have several ajax event handlers
            // for the same ajax event
            var eventName = e.Args [0].Name;
            if (!AjaxEvents [widget.ID].ContainsKey (eventName))
                AjaxEvents [widget.ID] [eventName] = new List<Node> ();

            // appending our p5.lambda object to the list of p5.lambda objects for the given widget's given event
            AjaxEvents [widget.ID] [eventName].Add (e.Args [0].Clone ());

            // mapping the widget's ajax event to our common event handler on page
            widget [eventName] = "common_event_handler";
        }

        /// <summary>
        ///     Finds the specified Control on your page, and returns to caller.
        /// 
        ///     Returns the control with the specified ID, from optionally [parent] control's. If no [parent] node is defined,
        ///     the first control matching the id given from the Page object will be returned.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "_p5.web.find-control")]
        private void _p5_web_find_control (ApplicationContext context, ActiveEventArgs e)
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
        ///     Null Active Event handler, for handling widget specific Active Events.
        /// 
        ///     Null Active Event handler, that checks for Widget specific events, and invoked the associated p5.lambda
        ///     code if a match is found.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "")]
        private void null_handler (ApplicationContext context, ActiveEventArgs e)
        {
            // checking to see if the currently raised Active Event has a handler on our page
            if (!PageActiveEvents.ContainsKey (e.Name))
                return;

            // keeping a reference to what we add to the current Active Event "root node"
            // such that we can clean up after ourselves afterwards
            var lambdas = new List<Node> ();

            // looping through each Active Event handler for current event
            foreach (var tmp in from idxEvt in PageActiveEvents [e.Name] from idxEvtContent in idxEvt.Item2 select idxEvtContent.Clone()) {
                tmp.Value = e.Args.Value;
                e.Args.Add(tmp);
                lambdas.Add(tmp);
            }

            // invoking each [lambda.xxx] object from event
            foreach (var idxLambda in lambdas) {
                context.Raise ("lambda-children", idxLambda);
            }

            // cleaning up after ourselves, deleting only the lambda objects that came
            // from our dynamically created event
            foreach (var idxLambda in lambdas) {
                idxLambda.UnTie ();
            }
        }
        
        /*
         * common helper method for creating widgets
         */
        private void CreateWidget (ApplicationContext context, Node args, string type)
        {
            // finding parent widget first, which defaults to "main container" widget, if no parent is given
            var parent = FindControl<pf.Widget>(args.GetChildValue ("parent", context, "cnt"), Page);

            // creating our widget by raising the active event responsible for creating it
            var createNode = args.Clone ();
            createNode.Insert (0, new Node ("__parent", parent));
            context.Raise ("p5.web.widgets." + type, createNode);
            var widget = createNode.Get<Widget> (context);

            // initializing Active Events for widget, if there are any given
            var eventNode = args.Find (idx => idx.Name == "events");
            if (eventNode != null)
                CreateWidgetEvents (widget, eventNode, context);
        }

        /*
         * creates local widget events for web widgets created through [create-widget]
         */
        private void CreateWidgetEvents (Control widget, Node eventNode, ApplicationContext context)
        {
            foreach (var idxEvt in XUtil.Iterate<Node> (eventNode, context)) {

                // checking to see if there's already an existing event with the given name
                if (!PageActiveEvents.ContainsKey (idxEvt.Name)) {
                    PageActiveEvents [idxEvt.Name] = new List<Tuple<string, List<Node>>> ();
                }

                // adding event, cloning node
                var tpl = new Tuple<string, List<Node>> (widget.ID, new List<Node> ());
                tpl.Item2.Add(idxEvt.Clone ());
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
         * removes all additional data associated with widget
         */
        private void RemoveData (Control ctrl)
        {
            if (WidgetData.ContainsKey (ctrl.ID))
                WidgetData.Remove (ctrl.ID);
        }

        /*
         * recursively traverses entire Control hierarchy on page, and adds up into result node
         */
        private static void ListWidgets (List<string> filter, Node resultNode, Control current)
        {
            bool shouldAdd = filter.Count == 0;
            if (!shouldAdd) {
                foreach (var idxFilter in filter) {
                    if (current.ID == idxFilter) {
                        shouldAdd = true;
                        break;
                    }
                }
            }
            if (shouldAdd) {

                string typeString = current.GetType().FullName;
                if (current is Widget)
                    typeString = typeString.Substring(typeString.LastIndexOf(".") + 1).ToLower();

                resultNode.Add(typeString, current.ID);
            }
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
         * common ajax event handler for all widget's events on page
         */
        [WebMethod]
        protected void common_event_handler (pf.Widget sender, pf.Widget.AjaxEventArgs e)
        {
            var id = sender.ID;
            var eventName = e.Name;
            var lambdas = AjaxEvents [id] [eventName];
            foreach (var idxLambda in lambdas) {
                _context.Raise ("eval", idxLambda.Clone ());
            }
        }
    }
}
