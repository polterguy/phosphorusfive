/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;
using p5.ajax.widgets;
using p5.core;
using p5.exp;
using Void = p5.ajax.widgets.Void;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating web widgets
    /// </summary>
    public static class WidgetTypes
    {
        /// <summary>
        ///     Creates an ajax container web widget
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.container", Protection = EventProtection.NativeClosed)]
        private static void p5_web_controls_container (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget<Container> (context, e.Args, "div");
        }

        /// <summary>
        ///     Creates an ajax literal web widget
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.literal", Protection = EventProtection.NativeClosed)]
        private static void p5_web_controls_literal (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget<Literal> (context, e.Args, "p");
        }

        /// <summary>
        ///     Creates an ajax void web widget
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.void", Protection = EventProtection.NativeClosed)]
        private static void p5_web_controls_void (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget<Void> (context, e.Args, "input");
        }

        /*
         * Creates a widget from the given node
         */
        private static void CreateWidget<T> (
            ApplicationContext context, 
            Node args, 
            string elementType) where T : Widget, new ()
        {
            // Creating widget as persistent control
            var parent = args.GetChildValue<Container> ("__parent", context);
            var position = args.GetChildValue ("position", context, -1);

            // Getting [oninit], if any
            var onInitialLoad = CreateLoadingEvents (context, args);
            EventHandler handler = null;
            if (onInitialLoad != null) {
                handler = delegate (object sender, EventArgs e) {
                    onInitialLoad.Insert (0, new Node ("_event", ((Control)sender).ID));
                    context.RaiseLambda ("eval", onInitialLoad);
                };
            }

            // Creating control as persistent control
            var widget = parent.CreatePersistentControl<T> (
                args.Get<string> (context),
                position,
                handler);

            // Setting ElementType (html element) of Widget
            widget.ElementType = elementType;

            // Making sure main widget ID is passed into decoration process
            DecorateWidget (context, widget, args);

            // Making sure we return Widget to caller
            args.Value = widget;
        }

        /*
         * Creates the [oninit] event for widget, if there is one defined
         */
        private static Node CreateLoadingEvents (ApplicationContext context, Node node)
        {
            // Checking to see if we've got an "oninit" Active Event for widget, and if so, return lambda for it
            var onInitialLoad = node.Find ("oninit");
            if (onInitialLoad == null)
                return null;

            // Returning lambda object
            return onInitialLoad.Clone ();
        }

        /*
         * Decorates widget with common properties
         */
        private static Widget DecorateWidget (ApplicationContext context, Widget widget, Node args)
        {
            // Looping through all children nodes of Widget's node to decorate Widget
            foreach (var idxArg in args.Children) {

                switch (idxArg.Name) {
                    case "visible":
                        widget.Visible = idxArg.Get<bool> (context);
                        break;
                    case "invisible-element":
                        widget.InvisibleElement = idxArg.Get<string> (context);
                        break;
                    case "element":
                        widget.ElementType = idxArg.Get<string> (context);
                        break;
                    case "has-id":
                        widget.HasID = idxArg.Get<bool> (context);
                        break;
                    case "render-type":
                        widget.RenderType = (Widget.RenderingType) Enum.Parse (typeof (Widget.RenderingType), idxArg.Get<string> (context));
                        break;
                    case "widgets":
                        CreateChildWidgets (context, widget, idxArg);
                        break;
                    case "events":
                        CreateWidgetLambdaEvents (context, widget, idxArg);
                        break;
                    case "__parent":
                    case "position":
                    case "parent":
                    case "has-name":
                        // Skipping these buggers, since they're not supposed to be handled here
                        break;
                    default:

                        // This might be an event, or an arbitrary attribute
                        HandleDefaultProperty (context, widget, idxArg);
                        break;
                }
            }

            // Ensures "name" property is created, if necessary
            EnsureNameProperty (widget, args, context);
            
            // Ensures "value" property is created, if necessary
            EnsureValueProperty (widget, args, context);

            // Returning widget to caller
            return widget;
        }

        /*
         * Ensuring that the "value" property is the same as the "ID" of the widget, 
         * but only for widgets that require a value attribute to function properly, and
         * for widgets that has not explicitly been given a value
         */
        private static void EnsureValueProperty (Widget widget, Node node, ApplicationContext context)
        {
            if (widget.HasAttribute ("value"))
                return; // Caller already explicitly added value attribute
            
            // Making sure "input" type "radio" widgets have a value corresponding to 
            // their ID, unless value is explicitly given
            if (widget.ElementType == "input" && widget ["type"] == "radio") {
                widget ["value"] = widget.ID;
            }
        }
            
        /*
         * Ensuring that the "name" property is the same as the "ID" of the widget, unless a name property is explicitly given,
         * or element type doesn't necessarily require a "name" to function properly, or [has-name] equals false
         */
        private static void EnsureNameProperty (Widget widget, Node node, ApplicationContext context)
        {
            if (widget.HasAttribute ("name"))
                return; // Caller already explicitly added name attribute

            if (!node.GetChildValue ("has-name", context, true)) {
                return; // Caller explicitly told us he didn't want any name attribute value
            }

            // Making sure "input", "select" and "textarea" widgets have a name corresponding to 
            // their ID
            switch (widget.ElementType) {
                case "input":
                case "textarea":
                case "select":
                    widget ["name"] = widget.ID;
                    break;
            }
        }

        /*
         * Creates children widgets of widget
         */
        private static void CreateChildWidgets (ApplicationContext context, Widget widget, Node children)
        {
            // Looping through all child widgets declared in lambda object
            foreach (var idxChild in children.Children) {

                idxChild.Insert (0, new Node ("__parent", widget));
                context.RaiseNative ("p5.web.widgets." + idxChild.Name, idxChild);
            }
        }

        /*
         * Creates lambda events for Widget
         */
        private static void CreateWidgetLambdaEvents (ApplicationContext context, Widget widget, Node events)
        {
            // Looping through all events for widget
            foreach (var idxEvt in events.Children.ToList ()) {

                // Letting p5.webapp do the actual creation
                var eventNode = new Node (idxEvt.Name, widget);
                eventNode.AddRange (idxEvt.Children);
                context.RaiseNative ("p5.web.add-widget-lambda-event", eventNode);
            }
        }

        /*
         * Handles all default properties of Widget
         */
        private static void HandleDefaultProperty (ApplicationContext context, Widget widget, Node node)
        {
            // Checking if this is a declaration of an event handler
            if (node.Name.StartsWith ("on")) {

                // This is an event, creating it
                CreateEventHandler (context, widget, node);
            } else {

                // This is a normal attribute
                widget [node.Name] = node.Get<string> (context);
            }
        }

        /*
         * Creates an event handler on the given widget for the given node. If a value is given in node, the
         * event will be assumed to be a JavaScript event, and simply sent back to client as JavaScript. If node 
         * does not contain a value, the event will be handled as a server-side lambda event, assuming children 
         * widgets are lambda code to evaluate
         */
        private static void CreateEventHandler (ApplicationContext context, Widget widget, Node node)
        {
            if (node.Value != null) {

                // Javascript code to be executed
                widget [node.Name] = node.Get<string> (context);
            } else {

                // Raising the Active Event that actually creates our Ajax event handler for our lambda object
                var eventNode = new Node (node.Name, widget);
                eventNode.Add ("_event", widget.ID);
                eventNode.AddRange (node.Children);
                context.RaiseNative ("p5.web.add-widget-ajax-event", eventNode);
            }
        }
    }
}
