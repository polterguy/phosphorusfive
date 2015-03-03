/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Web.UI;
using phosphorus.ajax.widgets;
using phosphorus.core;
using phosphorus.expressions;
using Void = phosphorus.ajax.widgets.Void;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui.widgets
{
    /// <summary>
    ///     class for creating web widgets
    /// </summary>
    public static class Widgets
    {
        /// <summary>
        ///     Creates a generic container type of Widget, that can contain children widgets of itself. Active Event is not
        ///     meant to be raised directly, but through the [pf.web.create-widget] Active Event, since it needs a reference to
        ///     its parent control directly, among other things. Pass in [element] to override the HTML element rendered. Pass 
        ///     in [has-id] with "false" to remove the rendering of its HTML ID element. Pass in [controls] as a list of child
        ///     controls that will be rendered in its children controls collection. Pass in [oninitialload] to have some server-side
        ///     piece of pf.lambda code execute during its initial loading. Pass in [render-type], to override how the widget is 
        ///     rendered, by supplying either "SelfClosing", "NoClose" or "Default" as values, overriding how the element is closed,
        ///     if at all. Anything within the [controls] parameter passedd in, will have [pf.web.widgets.] appended in front of it,
        ///     and raised as an Active Event, adding the results of that Active Event into its [controls] collection automatically.
        ///     Any nodes starting with "on", will be handled as events. If your events ends with "-script", they will
        ///     be rendered back to client as JavaScript events, if they end with anything else but "-script", they will be assumed
        ///     to be DOM events, and your node containing pf.lambda code, supposed to execute during that DOM event on the server-side.
        ///     Any other nodes, are automatically added as HTML attributes, wwith their given values, and rendered as such back to client.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.container")]
        private static void pf_web_controls_container (ApplicationContext context, ActiveEventArgs e)
        {
            var widget = CreateControl<Container> (context, e.Args, "div");
            var formId = e.Args.Find (idx => idx.Name == "_widget");
            if (formId != null && formId.Value == null)
                formId.Value = widget.ClientID;
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        ///     Creates a generic literal type of Widget, that can contain an inner value, being HTML or text. Active Event is not
        ///     meant to be raised directly, but through the [pf.web.create-widget] Active Event, since it needs a reference to
        ///     its parent control directly, among other things. Pass in [element] to override the HTML element rendered. Pass 
        ///     in [has-id] with "false" to remove the rendering of its HTML ID element. Pass in [innerValue] as a text string
        ///     that will be rendered in its HTML or text content. Pass in [oninitialload] to have some server-side
        ///     piece of pf.lambda code execute during its initial loading. Pass in [render-type], to override how the widget is 
        ///     rendered, by supplying either "SelfClosing", "NoClose" or "Default" as values, overriding how the element is closed,
        ///     if at all. Any nodes starting with "on", will be handled as events. If your events ends with "-script", they will
        ///     be rendered back to client as JavaScript events, if they end with anything else but "-script", they will be assumed
        ///     to be DOM events, and your node containing pf.lambda code, supposed to execute during that DOM event on the server-side.
        ///     Any other nodes, are automatically added as HTML attributes, wwith their given values, and rendered as such back to client.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.literal")]
        private static void pf_web_controls_literal (ApplicationContext context, ActiveEventArgs e)
        {
            var widget = CreateControl<Literal> (context, e.Args, "p");
            var formId = e.Args.Find (idx => idx.Name == "_widget");
            if (formId != null && formId.Value == null)
                formId.Value = widget.ClientID;
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        ///     Creates a generic void type of Widget, that cannot contain neither "text" nor "widgets". Active Event is not
        ///     meant to be raised directly, but through the [pf.web.create-widget] Active Event, since it needs a reference to
        ///     its parent control directly, among other things. Pass in [element] to override the HTML element rendered. Pass 
        ///     in [has-id] with "false" to remove the rendering of its HTML ID element. Pass in [oninitialload] to have some server-side
        ///     piece of pf.lambda code execute during its initial loading. Pass in [render-type], to override how the widget is 
        ///     rendered, by supplying either "SelfClosing", "NoClose" or "Default" as values, overriding how the element is closed,
        ///     if at all. Any nodes starting with "on", will be handled as events. If your events ends with "-script", they will
        ///     be rendered back to client as JavaScript events, if they end with anything else but "-script", they will be assumed
        ///     to be DOM events, and your node containing pf.lambda code, supposed to execute during that DOM event on the server-side.
        ///     Any other nodes, are automatically added as HTML attributes, wwith their given values, and rendered as such back to client.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.void")]
        private static void pf_web_controls_void (ApplicationContext context, ActiveEventArgs e)
        {
            var widget = CreateControl<Void> (context, e.Args, "input");
            var formId = e.Args.Find (idx => idx.Name == "_widget");
            if (formId != null && formId.Value == null)
                formId.Value = widget.ClientID;
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /*
         * creates a widget from the given node
         */
        private static T CreateControl<T> (
            ApplicationContext context, 
            Node node, 
            string elementType, 
            Widget.RenderingType type = Widget.RenderingType.Default) where T : Widget, new ()
        {
            // creating widget as persistent control
            var parent = node.Find (idx => idx.Name == "__parent").Get<Container> (context);
            var position = -1;
            var posNode = node.Find (idx => idx.Name == "_position");
            if (posNode != null)
                position = posNode.Get<int> (context);

            // getting [oninitialload], if any
            var onInitialLoad = CreateLoadingEvents (context, node);
            var formId = XUtil.Single<string> (node.Find (idx => idx.Name == "_widget"), context);

            var widget = parent.CreatePersistentControl<T> (
                XUtil.Single<string> (node.Value, node, context),
                position,
                delegate (object sender, EventArgs e) {
                    if (onInitialLoad == null)
                        return;

                    onInitialLoad.Insert (0, new Node ("_widget", formId));
                    onInitialLoad.Insert (1, new Node ("_event", ((Control)sender).ID));
                    context.Raise ("lambda", onInitialLoad);
                });

            // setting ElementType (html element) of Widget
            widget.ElementType = elementType;

            // setting rendering type (no closing element, void or default)
            if (type != Widget.RenderingType.Default)
                widget.RenderType = type;

            // returning widget to caller
            return widget;
        }

        /*
         * creates the [oninitialload] event for widget, if we should
         */
        private static Node CreateLoadingEvents (ApplicationContext context, Node node)
        {
            // checking to see if we've got an "initialload" Active Event for widget, and if so, handle it
            var onInitialLoad = node.Find (idx => idx.Name == "oninitialload");
            if (onInitialLoad == null)
                return null;

            // finding lambda object(s) referred to by [oninitialload], and creating our lambda node structure
            var evtLambdas = new List<Node> (XUtil.Iterate<Node> (onInitialLoad, context, true));
            var evtNode = new Node ("oninitialload");
            foreach (var idxLambda in evtLambdas) {
                evtNode.Add (idxLambda.Clone ());
            }

            // returning pf.lambda node
            return evtNode;
        }

        /*
         * decorates widget with common properties
         */
        private static Widget DecorateWidget (ApplicationContext context, Widget widget, Node args)
        {
            // looping through all children nodes of Widget's node to decorate Widget
            foreach (var idxArg in args.Children) {
                switch (idxArg.Name) {
                    case "visible":
                        widget.Visible = XUtil.Single<bool> (idxArg, context);
                        break;
                    case "invisible-element":
                        widget.InvisibleElement = XUtil.Single<string> (idxArg, context);
                        break;
                    case "element":
                        widget.ElementType = XUtil.Single<string> (idxArg, context);
                        break;
                    case "has-id":
                        widget.NoIdAttribute = !XUtil.Single<bool> (idxArg, context);
                        break;
                    case "render-type":
                        widget.RenderType = (Widget.RenderingType) Enum.Parse (typeof (Widget.RenderingType), XUtil.Single<string> (idxArg, context));
                        break;
                    case "widgets":
                        CreateChildWidgets (context, widget, idxArg);
                        break;
                    case "controls":
                        if (idxArg.Count > 0 && idxArg.Value == null) // "old" style of accessing [widgets] collection
                            CreateChildWidgets (context, widget, idxArg);
                        else
                            HandleDefaultProperty (context, widget, idxArg);
                        break;
                    case "widget":
                    case "before":
                    case "after":
                    case "parent":
                    case "events":
                    case "has-name":
                        // skipping these buggers, since they're not supposed to be handled here
                        break;
                    default:

                        // this might be an event, it might be a node we should ignore (starting with "_") or it might be any arbitrary attribute
                        // we should render. HandleDefaultProperty will figure out
                        HandleDefaultProperty (context, widget, idxArg);
                        break;
                }
            }

            // ensures "name" property is created, if necessary
            EnsureNameProperty (widget, args, context);

            return widget;
        }

        /*
         * ensuring the "name" property is the same as the "ID" of the widget, unless a name property is explicitly given,
         * or element type doesn't necessarily require a "name" to function correctly
         */
        private static void EnsureNameProperty (Widget widget, Node node, ApplicationContext context)
        {
            if (widget ["name"] != null)
                return; // caller already explicitly added name attribute

            if (node.FindAll (delegate(Node idx) {
                return idx.Name == "has-name" && !idx.Get<bool> (context);
            }).GetEnumerator ().MoveNext ()) {
                return; // caller explicitly told us he didn't want no name
            }

            // making sure "input", "select" and "textarea" widgets have a name corresponding to 
            // their ID unless name is explicitly given
            var addName = false;
            switch (widget.ElementType) {
                case "input":
                    if (widget.ElementType != "button")
                        addName = true;
                    break;
                case "textarea":
                case "select":
                    addName = true;
                    break;
            }
            if (addName) {
                widget ["name"] = widget.ID;
            }
        }

        /*
         * creates children widgets of widget
         */
        private static void CreateChildWidgets (ApplicationContext context, Widget widget, Node children)
        {
            foreach (var idxChild in XUtil.Iterate<Node> (children, context, true)) {
                idxChild.Insert (0, new Node ("__parent", widget));
                idxChild.Insert (1, new Node ("_widget", XUtil.Single<string> (children.Parent.Find (idx => idx.Name == "_widget"), context)));
                context.Raise ("pf.web.widgets." + idxChild.Name, idxChild);
            }
        }

        /*
         * handles all default properties of Widget
         */
        private static void HandleDefaultProperty (ApplicationContext context, Widget widget, Node node)
        {
            if (node.Name.StartsWith ("on")) {
                if (node.Name != "oninitialload")
                    CreateEventHandler (context, widget, node);
            } else if (!node.Name.StartsWith ("_")) {
                widget [node.Name] = XUtil.Single<string> (node, context);
            }
        }

        /*
         * creates an event handler on the given widget for the given node. if the name of the node ends with "-script", the
         * event will be assumed to be a JavaScript event, and simply sent back to client as JavaScript. if it doesn not end
         * with "-script", the event will be handled as a server-side pf.lambda event
         */
        private static void CreateEventHandler (ApplicationContext context, Widget widget, Node node)
        {
            if (node.Name.EndsWith ("-script")) {
                // javascript code to be executed
                widget [node.Name.Replace ("-script", "")] = XUtil.Single<string> (node, context);
            } else {
                // finding lambda object(s) referred to by [event], and creating our lambda node structure
                var evtLambdas = new List<Node> (XUtil.Iterate<Node> (node, context, true));
                var evtNode = new Node (node.Name);
                foreach (var idxLambda in evtLambdas) {
                    evtNode.Add (idxLambda.Clone ());
                }
                evtNode.Insert (0, new Node ("_widget", XUtil.Single<string> (node.Parent.Find (idx => idx.Name == "_widget"), context)));
                evtNode.Insert (1, new Node ("_event", widget.ID));

                // raising the Active Event that actually creates our ajax event handler for our pf.lambda object
                var eventNode = new Node (string.Empty, widget);
                eventNode.Add (evtNode);
                context.Raise ("_pf.web.add-widget-event", eventNode);
            }
        }
    }
}