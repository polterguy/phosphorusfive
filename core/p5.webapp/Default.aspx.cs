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
///     Main namespace for your Phosphorus Five Web App
/// </summary>
namespace p5.webapp
{
    // Helper namespace alias to avoid nameclashing with ASP.NET core controls
    using pf = p5.ajax.widgets;

    /// <summary>
    ///     Main asp.net web page for your Application Pool
    /// </summary>
    public partial class Default : AjaxPage
    {
        // List of all dynamically created protected events
        private Node _protectedDynamicEvents = null;

        // Main root container for all widgets
        protected pf.Container cnt;

        // Application Context for page life cycle
        private ApplicationContext _context;

        /*
         * Used as storage for Widget Ajax Events
         */
        private WidgetEventStorage WidgetAjaxEventStorage
        {
            get {
                if (ViewState["_WidgetAjaxEventStorage"] == null)
                    ViewState["_WidgetAjaxEventStorage"] = new WidgetEventStorage ();
                return ViewState["_WidgetAjaxEventStorage"] as WidgetEventStorage;
            }
        }

        /*
         * Used as storage for Widget Lambda Events
         */
        private WidgetEventStorage WidgetLambdaEventStorage
        {
            get {
                if (ViewState["_WidgetLambdaEventStorage"] == null)
                    ViewState["_WidgetLambdaEventStorage"] = new WidgetEventStorage ();
                return ViewState["_WidgetLambdaEventStorage"] as WidgetEventStorage;
            }
        }

        #region [ -- Page overrides and initializers, plus login of user -- ]

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

        #region [ -- Creating and deleting Widgets -- ]

        /// <summary>
        ///     Creates a web widget
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-widget", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "create-void-widget", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "create-literal-widget", Protection = EventProtection.LambdaClosed)]
        private void create_widget (ApplicationContext context, ActiveEventArgs e)
        {
            var splits = e.Name.Split('-');
            string type = splits.Length == 2 ? "container" : splits[1];
            CreateWidget (context, e.Args, type);
        }

        /// <summary>
        ///     Checks if the given widget(s) exist
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "widget-exist", Protection = EventProtection.LambdaClosed)]
        private void widget_exist (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args)) {

                // Looping through all IDs given
                foreach (var widgetId in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Adding a boolean as result node, with widget's ID as name, indicating existence of widget
                    e.Args.Add(widgetId, FindControl<Control>(widgetId, Page) != null);
                }

                // Return true as main Active Event node if ALL widgets existed, otherwise returning false
                e.Args.Value = !e.Args.Children.Where(ix => !ix.Get<bool> (context)).GetEnumerator().MoveNext();
            }
        }

        /// <summary>
        ///     Deletes the given widget(s) entirely
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-widget", Protection = EventProtection.LambdaClosed)]
        private void delete_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all IDs given
            foreach (var idxWidget in FindWidgets<Control> (context, e.Args, "delete-widget")) {

                // Removing widget
                RemoveWidget (context, e.Args, idxWidget);
            }
        }

        /// <summary>
        ///     Clears the given widget(s), removing all of its children widgets
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "clear-widget", Protection = EventProtection.LambdaClosed)]
        private void clear_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all IDs given
            foreach (var idxWidget in FindWidgets<pf.Container> (context, e.Args, "clear-widget")) {

                // Then looping through all of its children controls
                foreach (Control idxChildWidget in new ArrayList(idxWidget.Controls)) {

                    // Removing widget from page control collection
                    RemoveWidget (context, e.Args, idxChildWidget);
                }
            }
        }

        #endregion

        #region [ -- Retrieving Widgets -- ]

        /// <summary>
        ///     Returns the ID and type of the given widget's parent(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-parent-widget", Protection = EventProtection.LambdaClosed)]
        private void get_parent_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args)) {

                // Looping through all IDs given
                foreach (var idxWidget in FindWidgets <Control> (context, e.Args, "get-parent-widget")) {

                    // Finding parent and returning type as Name and ID as Value
                    var parent = idxWidget.Parent;
                    string type = parent.GetType().FullName;

                    // Checking if type is from p5 Ajax, and if so, returning "condensed" typename
                    if (parent is Widget)
                        type = type.Substring(type.LastIndexOf(".") + 1).ToLower();
                    e.Args.Add(type, parent.ID);
                }
            }

            // Making sure we set value of main event node to widget's ID if only one widget was requested
            if (e.Args.Count == 1)
                e.Args.Value = e.Args.FirstChild.Value;
            else
                e.Args.Value = null; // Making sure we remove arguments if there are multiple return values
        }

        /// <summary>
        ///     Returns the ID and type of the given widget's children
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-children-widgets", Protection = EventProtection.LambdaClosed)]
        private void get_children_widgets (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all IDs given
                foreach (var idxWidget in FindWidgets <Control> (context, e.Args, "get-children-widgets")) {

                    // Adding currently iterated widget's ID
                    e.Args.Add(idxWidget.ID);

                    // Then looping through currently iterated widget's children, adding them
                    foreach (Control idxCtrl in idxWidget.Controls) {

                        // Adding type of widget as name, and ID as value
                        string type = idxCtrl.GetType().FullName;

                        // Making sure we return "condensed typename" if widget type is from p5 Ajax
                        if (idxCtrl is Widget)
                            type = type.Substring(type.LastIndexOf(".") + 1).ToLower();
                        e.Args.LastChild.Add(type, idxCtrl.ID);
                    }
                }
            }
        }
        
        /// <summary>
        ///     Find widget(s) according to criteria underneath an (optionally) declared widget, and returns its ID
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "find-widget", Protection = EventProtection.LambdaClosed)]
        private void find_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args)) {

                // Retrieving where to start looking
                var root = FindControl<Widget> (e.Args.GetExValue (context, "cnt"), Page);

                // Retrieving all controls having properties matching whatever arguments supplied
                foreach (var idxWidget in FindWidgetsBy <Widget> (e.Args, root, context)) {

                    // Adding type of widget as name, and ID as value
                    string type = idxWidget.GetType().FullName;

                    // Making sure we return "condensed typename" if widget type is from p5 Ajax
                    if (idxWidget is Widget)
                        type = type.Substring(type.LastIndexOf(".") + 1).ToLower();
                    e.Args.Add (type, idxWidget.ID);
                }
            }

            // Making sure we set value of main event node to widget's ID if only one widget was found
            if (e.Args.Count == 1)
                e.Args.Value = e.Args.FirstChild.Value;
            else
                e.Args.Value = null; // Making sure we remove arguments if there are multiple return values
        }

        /// <summary>
        ///     Lists all widgets on page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widgets", Protection = EventProtection.LambdaClosed)]
        private void list_widgets (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving filter, if any
                var filter = XUtil.Iterate<string>(context, e.Args).ToList ();
                if (e.Args.Value != null && filter.Count == 0)
                    return; // Possibly a filter expression, leading into oblivion

                // Recursively retrieving all widgets on page
                ListWidgets(filter, e.Args, cnt);
            }
        }

        #endregion

        #region [ -- Widget properties -- ]

        /// <summary>
        ///     Returns properties and/or attributes requested by caller as children nodes
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-property", Protection = EventProtection.LambdaClosed)]
        private void get_widget_property (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                if (e.Args.Value == null || e.Args.Count == 0)
                    return; // Nothing to do here ...

                // Looping through all widget IDs given by caller
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "get-widget-property")) {

                    // Looping through all properties requested by caller
                    foreach (var nameNode in e.Args.Children.Where (ix => ix.Name != "").ToList ()) {

                        // Checking if this is a generic attribute, or a specific property
                        switch (nameNode.Name) {
                           case "visible":
                                CreatePropertyReturn (e.Args, nameNode, idxWidget, idxWidget.Visible);
                                break;
                            case "invisible-element":
                                CreatePropertyReturn (e.Args, nameNode, idxWidget, idxWidget.InvisibleElement);
                                break;
                            case "element":
                                CreatePropertyReturn (e.Args, nameNode, idxWidget, idxWidget.ElementType);
                                break;
                            case "has-id":
                                CreatePropertyReturn (e.Args, nameNode, idxWidget, idxWidget.HasID);
                                break;
                            case "render-type":
                                CreatePropertyReturn (e.Args, nameNode, idxWidget, idxWidget.RenderType.ToString ());
                                break;
                            default:
                                if (!string.IsNullOrEmpty (nameNode.Name))
                                    CreatePropertyReturn (e.Args, nameNode, idxWidget);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Sets properties and/or attributes of web widgets
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-widget-property", Protection = EventProtection.LambdaClosed)]
        private void set_widget_property (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.Count == 0)
                return; // Nothing to do here ...

            // Looping through all widget IDs given by caller
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "set-widget-property")) {

                // Looping through all properties requested by caller
                foreach (var valueNode in e.Args.Children.Where (ix => ix.Name != "")) {
                    switch (valueNode.Name) {
                        case "visible":
                            idxWidget.Visible = valueNode.GetExValue<bool> (context);
                            break;
                        case "invisible-element":
                            idxWidget.InvisibleElement = valueNode.GetExValue<string> (context);
                            break;
                        case "element":
                            idxWidget.ElementType = valueNode.GetExValue<string> (context);
                            break;
                        case "has-id":
                            idxWidget.HasID = valueNode.GetExValue<bool> (context);
                            break;
                        case "render-type":
                            idxWidget.RenderType = (Widget.RenderingType) Enum.Parse (typeof (Widget.RenderingType), valueNode.GetExValue<string> (context));
                            break;
                        default:
                            idxWidget [valueNode.Name] = valueNode.GetExValue<string> (context);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Removes the properties and/or attributes of web widgets
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-widget-property", Protection = EventProtection.LambdaClosed)]
        private void delete_widget_property (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.Count == 0)
                return; // Nothing to do here ...

            // Looping through all widgets supplied by caller
            foreach (var widget in FindWidgets<Widget> (context, e.Args, "delete-widget-property")) {

                // Looping through each property to remove
                foreach (var nameNode in e.Args.Children.Where (ix => ix.Name != "")) {

                    // Verifying property can be removed
                    switch (nameNode.Name) {
                        case "visible":
                        case "invisible-element":
                        case "element":
                        case "has-id":
                        case "has-name":
                        case "render-type":
                            throw new LambdaException ("Cannot remove property; '" + nameNode.Name + "' of widget", e.Args, context);
                        default:
                            widget.RemoveAttribute (nameNode.Name);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all existing properties for given web widget
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-properties", Protection = EventProtection.LambdaClosed)]
        private void list_widget_properties (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                if (e.Args.Value == null)
                    return; // Nothing to do here ...

                // Looping through all widgets
                foreach (var widget in FindWidgets<Widget> (context, e.Args, "list-widget-properties")) {

                    // Creating our "return node" for currently handled widget
                    Node curNode = e.Args.Add (widget.ID).LastChild;

                    // First listing "static properties"
                    if (!widget.Visible)
                        curNode.Add ("visible", false);
                    if ((widget is Container && widget.ElementType != "div") || (widget is Literal && widget.ElementType != "p"))
                        curNode.Add ("element", widget.ElementType);
                    if (!widget.HasID)
                        curNode.Add ("has-id", false);

                    // Then the generic attributes
                    foreach (var idxAtr in widget.AttributeKeys) {

                        // Dropping the Tag property and all events
                        if (idxAtr == "Tag" || idxAtr.StartsWith ("on"))
                            continue;
                        curNode.Add (idxAtr, widget [idxAtr]);
                    }
                }
            }
        }

        #endregion

        #region [ -- Widget Ajax events -- ]

        /// <summary>
        ///     Returns the given ajax event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-ajax-event", Protection = EventProtection.LambdaClosed)]
        private void get_widget_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widgets
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "get-widget-ajax-event")) {

                    // Looping through events requested by caller
                    foreach (var idxEventNameNode in e.Args.Children.Where (ix => ix.Name != "").ToList ()) {

                        // Returning lambda object for Widget Ajax event
                        if (WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name] != null)
                            e.Args.FindOrCreate(idxWidget.ID).Add(WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name].Clone());
                    }
                }
            }
        }

        /// <summary>
        ///     Changes the given ajax event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-widget-ajax-event", Protection = EventProtection.LambdaClosed)]
        private void set_widget_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all widgets
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "set-widget-ajax-event")) {

                // Looping through events requested by caller
                foreach (var idxEventNameNode in e.Args.Children) {

                    // Checking if we should delete existing event
                    if (idxEventNameNode.Count == 0) {

                        // Deleting existing ajax event
                        WidgetAjaxEventStorage.Remove (idxWidget.ID, idxEventNameNode.Name);
                    } else {

                        // Setting Widget's Ajax event to whatever we were given
                        WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name] = idxEventNameNode.Clone();
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all existing ajax events for given widget(s)
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-ajax-events", Protection = EventProtection.LambdaClosed)]
        private void list_widget_ajax_events (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args, true)) {

                // Looping through all widgets supplied
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "list-widget-ajax-events")) {

                    // Then looping through all attribute keys, filtering everything out that does not start with "on"
                    Node curNode = new Node(idxWidget.ID);
                    foreach (var idxAtr in idxWidget.AttributeKeys.Where (ix => !ix.StartsWith ("on"))) {

                        // Adding this attribute
                        curNode.Add(idxAtr);
                    }

                    // Special handling of [oninit], since it never leaves the server, and is hence not in widget's attribute collection
                    if (WidgetAjaxEventStorage[idxWidget.ID, "oninit"] != null)
                        curNode.Add("oninit");

                    // Checking if we've got more than zero events for given widget, and if so, adding event node, containing list of events
                    if (curNode.Count > 0)
                        e.Args.Add(curNode);
                }
            }
        }

        #endregion

        #region [ -- Widget lambda events -- ]
        
        /// <summary>
        ///     Returns the given lambda event(s) for the given widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-lambda-event", Protection = EventProtection.LambdaClosed)]
        private void get_widget_lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widgets
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "get-widget-lambda-event")) {

                    // Looping through events requested by caller
                    foreach (var idxEventNameNode in e.Args.Children.ToList ()) {

                        // Returning lambda object for Widget Ajax event
                        if (WidgetLambdaEventStorage[idxEventNameNode.Name, idxWidget.ID] != null) {

                            // We found a Lambda event with that name for that widget
                            var evtNode = WidgetLambdaEventStorage[idxEventNameNode.Name, idxWidget.ID].Clone();
                            evtNode.Name = idxEventNameNode.Name;
                            e.Args.FindOrCreate(idxWidget.ID).Add(evtNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Changes the given lambda event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-widget-lambda-event", Protection = EventProtection.LambdaClosed)]
        private void set_widget_lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all widget IDs
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "set-widget-lambda-event")) {

                // Looping through events requested by caller
                foreach (var idxEventNameNode in e.Args.Children) {

                    // Verifying Active Event is not protected
                    if (!CanOverrideEventInLambda (context, idxEventNameNode.Name))
                        throw new LambdaSecurityException(
                            string.Format ("You cannot override Active Event '{0}' since it is protected", e.Args.Name),
                            e.Args,
                            context);

                    // Checking if this is actually a deletion invocation
                    if (idxEventNameNode.Count == 0) {

                        // Deleting existing event
                        WidgetLambdaEventStorage.Remove (idxEventNameNode.Name, idxWidget.ID);
                    } else {

                        // Setting Widget's Ajax event to whatever we were given
                        WidgetLambdaEventStorage[idxEventNameNode.Name, idxWidget.ID] = idxEventNameNode.Clone();
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all existing lambda events for given widget(s)
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-lambda-events", Protection = EventProtection.LambdaClosed)]
        private void list_widget_lambda_events (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args, true)) {

                // Looping through all widgets
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "list-widget-lambda-events")) {

                    // Then looping through all attribute keys, filtering everything out that does not start with "on"
                    Node curNode = new Node(idxWidget.ID);
                    foreach (var idxAtr in WidgetLambdaEventStorage.FindByKey2 (idxWidget.ID)) {

                        // Checking if this attribute is an event or not
                        curNode.Add(idxAtr);
                    }

                    // Checking if we've got more than zero events for given widget, and if so, 
                    // adding event node, containing list of events
                    if (curNode.Count > 0)
                        e.Args.Add(curNode);
                }
            }
        }

        #endregion

        #region [ -- Page object setters and getters -- ]

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
            });
        }

        /// <summary>
        ///     Retrieves page object(s)
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-page-value", Protection = EventProtection.LambdaClosed)]
        private void get_page_value (ApplicationContext context, ActiveEventArgs e)
        {
            p5.exp.CollectionBase.Get (context, e.Args, key => ViewState [key]);
        }

        /// <summary>
        ///     Lists all keys in the page object
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-page-keys", Protection = EventProtection.LambdaClosed)]
        private void list_page_keys (ApplicationContext context, ActiveEventArgs e)
        {
            p5.exp.CollectionBase.List (context, e.Args, ViewState.Keys);
        }

        #endregion

        #region [ -- Misc. global helpers -- ]

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
                Manager.SendJavaScriptToClient (idxSnippet);
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
                RegisterJavaScript (idxSnippet);
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
                RegisterJavaScriptFile (idxFile);
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
                RegisterStylesheetFile (idxFile);
            }
        }

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

        #region [ -- "Private" helper Active Events -- ]

        /*
         * Invoked by p5.web during creation of Widgets, creates a Widget Ajax event
         */
        [ActiveEvent (Name = "_p5.web.add-widget-ajax-event", Protection = EventProtection.NativeClosed)]
        private void _p5_web_add_widget_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving widget
            var widget = e.Args.Get<pf.Widget> (context);
            e.Args.Value = null; // dropping the actual Widget, to avoid serializing it into ViewState!

            // Mapping the widget's ajax event to our common event handler on page
            // But intentionally dropping [oninit], since it's a purely server-side lambda event, and not supposed to be pushed to client
            if (e.Args.Name != "oninit")
                widget [e.Args.Name] = "common_event_handler";

            // Storing our Widget Ajax event in storage
            WidgetAjaxEventStorage [widget.ID, e.Args.Name] = e.Args;
        }
        
        /*
         * Invoked by p5.web during creation of Widgets, creates a Widget Lambda event
         */
        [ActiveEvent (Name = "_p5.web.add-widget-lambda-event", Protection = EventProtection.NativeClosed)]
        private void _p5_web_add_widget_lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving widget
            var widget = e.Args.Get<pf.Widget> (context);
            e.Args.Value = null; // dropping the actual Widget, to avoid serializing it into ViewState!

            // Verifying Active Event is not protected
            if (!CanOverrideEventInLambda(context, e.Args.Name))
                throw new ApplicationException(string.Format ("You cannot override Active Event '{0}' since it is protected", e.Args.Name));

            WidgetLambdaEventStorage [e.Args.Name, widget.ID] = e.Args;
        }

        #endregion

        #region [ -- Private and protected helper methods -- ]

        /*
         * Raises Widget specific lambda events
         */
        [ActiveEvent (Name = "", Protection = EventProtection.NativeOpen)]
        private void null_handler (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each lambda event handler for given event
            foreach (var idxLambda in WidgetLambdaEventStorage [e.Name].ToList ()) {

                // Raising Active Event
                XUtil.RaiseEvent (context, e.Args, idxLambda, e.Name);
            }
        }

        /*
         * Common ajax event handler for all widget's events on page
         */
        [WebMethod]
        protected void common_event_handler (pf.Widget sender, pf.Widget.AjaxEventArgs e)
        {
            _context.RaiseLambda("eval", WidgetAjaxEventStorage[sender.ID, e.Name].Clone());
        }

        /*
         * Helper to retrieve a list of widgets from a Node, throws if widget with specified ID does not exist
         */
        private IEnumerable<T> FindWidgets<T> (ApplicationContext context, Node args, string activeEventName) where T : Control
        {
            // Looping through all Widget IDs supplied by caller, finding widget with specified ID
            foreach (var idxWidgetID in XUtil.Iterate<string> (context, args, true)) {

                // Retrieving Widget with currently iterated ID
                var idxWidget = FindControl<Control>(idxWidgetID, Page);

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
        
        /*
         * Helper to retrieve a list of widgets from a Node that serves as criteria
         */
        private IEnumerable<T> FindWidgetsBy<T> (Node args, Widget idx, ApplicationContext context) where T : Widget
        {
            if (idx == null)
                yield break;

            bool match = true;
            foreach (var idxNode in args.Children) {
                if (!idx.HasAttribute(idxNode.Name) || idx[idxNode.Name] != idxNode.GetExValue<string>(context, null)) {
                    match = false;
                    break;
                }
            }
            if (match)
                yield return idx as T;
            foreach (var idxChild in idx.Controls) {
                foreach (var idxSubFind in FindWidgetsBy<T> (args, idxChild as T, context)) {
                    yield return idxSubFind;
                }
            }
        }

        /*
         * Recursively traverses entire Control hierarchy on page, and adds up into result node
         */
        private static void ListWidgets (List<string> filter, Node args, Control current)
        {
            if (filter.Count == 0 || filter.Count(ix => ix == current.ID) > 0) {

                // Current Control is matching one of our filters, or there are no filters
                string typeString = current.GetType().FullName;
                if (current is Widget)
                    typeString = typeString.Substring(typeString.LastIndexOf(".") + 1).ToLower();

                args.Add(typeString, current.ID);
            }
            foreach (Control idxChild in current.Controls) {

                // Recursively invoking "self"
                ListWidgets (filter, args, idxChild);
            }
        }

        /*
         * Common helper method for creating widgets
         */
        private void CreateWidget (ApplicationContext context, Node args, string type)
        {
            // Finding parent widget first, which defaults to "main container" widget, if no parent is given
            var parent = FindControl<pf.Widget>(args.GetChildValue ("parent", context, "cnt"), Page);

            // Creating our widget by raising the active event responsible for creating it
            var createNode = args.Clone ();
            if (createNode.Value == null) {

                // Creating a unique ID for widget BEFORE we create Widget, since we need it to create our Widget Events 
                // before we create widget itself, since [oninit] might depend upon widget events, and oninit 
                // is raised during creation process, before it returns.
                createNode.Value = Container.CreateUniqueId ();
            }

            // ORDER COUNTS!! Read above comments!

            // Initializing Active Events for widget, BEFORE widget is created, 
            // since [oninit] might depend upon Widget events for widget
            var eventNode = createNode.Find (idx => idx.Name == "events");
            if (eventNode != null)
                CreateWidgetLambdaEvents (createNode.Get<string> (context), eventNode.UnTie (), context);

            createNode.Insert (0, new Node ("__parent", parent));
            context.RaiseNative ("p5.web.widgets." + type, createNode);
        }

        /*
         * helper for [get-widget-property], creates a return value for one property
         */
        private static void CreatePropertyReturn (Node node, Node nameNode, Widget widget, object value = null)
        {
            // checking if widget has the attribute, if it doesn't, we don't even add any return nodes at all, to make it possible
            // to separate widgets which has the property, but no value, (such as the selected property on checkboxes for instance),
            // and widgets that does not have the property at all
            if (value == null && !widget.HasAttribute (nameNode.Name))
                return;

            node.FindOrCreate (widget.ID).Add (nameNode.Name).LastChild.Value = value == null ? widget [nameNode.Name] : value;
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
         * creates local widget events for web widgets created through [create-widget]
         */
        private void CreateWidgetLambdaEvents (string widgetId, Node eventNode, ApplicationContext context)
        {
            // Looping through each event in args
            foreach (var idxEvt in eventNode.Children.ToList ()) {

                // Verifying Active Event is not protected
                if (!CanOverrideEventInLambda (context, idxEvt.Name))
                    throw new ApplicationException(string.Format ("You cannot override Active Event '{0}' since it is protected", idxEvt.Name));

                // Adding lambda event to lambda event storage
                WidgetLambdaEventStorage [idxEvt.Name, widgetId] = idxEvt;
            }
        }

        /*
         * Helper to remove a widget from Page control collection
         */
        private void RemoveWidget (ApplicationContext context, Node args, Control widget)
        {
            // Basic logical error checking
            var parent = widget.Parent as pf.Container;
            if (parent == null)
                throw new LambdaException (
                    "You cannot delete a widget who's parent is not a Phosphorus Five Ajax Container widget. Tried to delete; " + widget.ID + " which is of type " + widget.GetType().FullName,
                    args,
                    context);

            // Removing all events, both "lambda" and "ajax"
            RemoveAllEventsRecursive (widget);

            // Removing widget itself from page control collection, making sure we persist the change to parent Container
            parent.RemoveControlPersistent (widget);
        }

        /*
         * Helper to figure out if Active Event is protected or not
         */
        private bool CanOverrideEventInLambda (ApplicationContext context, string evt)
        {
            // Verifying Active Event is not protected, first checking native handlers
            if (context.HasEvent (evt)) {

                // There exist a native handler for this Active Event, now getting protection level of event
                if (context.GetEventProtection(evt) == EventProtection.LambdaOpen)
                    return true;
            }

            // Checking if dynamically created protected events are retrieved from before, and if not, retrieving them
            if (_protectedDynamicEvents == null) {

                // Retrieving dynamically protected events
                _protectedDynamicEvents = context.RaiseNative("_p5.lambda.get-protected-events");
            }

            // Checking if protected events contains given event name, and if so, returning true, else returning false
            return _protectedDynamicEvents[evt] == null;
        }

        /*
         * Clears all lambda Active Events for widget
         */
        private void RemoveWidgetLambdaEvents (Control widget)
        {
            // Removing widget entirely from WidgetEventStorage
            WidgetLambdaEventStorage.RemoveFromKey2(widget.ID);

            // looping through all child widgets, and removing those, by recursively calling self
            foreach (Control idxChild in widget.Controls) {
                RemoveWidgetLambdaEvents (idxChild);
            }
        }

        /*
         * recursively removes all events for control and all of its children controls
         */
        private void RemoveWidgetAjaxEvents (Control idx)
        {
            // Removing all Ajax Events for widget
            WidgetAjaxEventStorage.RemoveFromKey1(idx.ID);

            // Recursively removing all ajax events for all of control's children controls
            foreach (Control idxChild in idx.Controls) {
                RemoveWidgetAjaxEvents (idxChild);
            }
        }
        
        /*
         * Removing all events for widget recursively
         */
        private void RemoveAllEventsRecursive (Control widget)
        {
            if (widget is Widget) {

                // Removing all Ajax Events for widget
                WidgetAjaxEventStorage.RemoveFromKey1(widget.ID);

                // Removing all lambda events for widget
                WidgetLambdaEventStorage.RemoveFromKey2(widget.ID);
            }

            // Recursively invoking "self" for all children widgets
            foreach (Widget idxChildWidget in widget.Controls) {

                // Removing all events for currently iterated child
                RemoveAllEventsRecursive(idxChildWidget);
            }
        }

        #endregion
    }
}
