/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;
using p5.ajax.widgets;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using Void = p5.ajax.widgets.Void;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating creation of web widget types
    /// </summary>
    public class WidgetTypes : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetRetriever"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetTypes (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        /// <summary>
        ///     Creates an ajax container web widget
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.container", Protection = EventProtection.NativeClosed)]
        public void p5_web_controls_container (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = CreateWidget<Container> (context, e.Args, "div");
        }

        /// <summary>
        ///     Creates an ajax literal web widget
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.literal", Protection = EventProtection.NativeClosed)]
        public void p5_web_controls_literal (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = CreateWidget<Literal> (context, e.Args, "p");
        }

        /// <summary>
        ///     Creates an ajax void web widget
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.void", Protection = EventProtection.NativeClosed)]
        public void p5_web_controls_void (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = CreateWidget<Void> (context, e.Args, "input");
        }

        /*
         * Creates a widget from the given node
         */
        private Widget CreateWidget<T> (
            ApplicationContext context, 
            Node args, 
            string elementType) where T : Widget, new ()
        {
            // Creating widget as persistent control
            var parent = args.GetChildValue<Container> ("_parent", context);
            var position = args.GetChildValue ("position", context, -1);

            // Creating control as persistent control, and setting HTML element type
            var widget = parent.CreatePersistentControl<T> (
                args.Get<string> (context),
                position);
            widget.ElementType = elementType;

            // Decorating widget properties/events, and create child widgets
            DecorateWidget (context, widget, args);

            // Making sure we return Widget to caller
            return widget;
        }

        /*
         * Decorates widget with common properties
         */
        private Widget DecorateWidget (ApplicationContext context, Widget widget, Node args)
        {
            // Looping through all children nodes of Widget's node to decorate Widget
            foreach (var idxArg in args.Children.ToList ()) {

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
                    case "enable-viewstate":
                        widget.EnableViewState = idxArg.Get<bool> (context);
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
                    case "_parent":
                    case "position":
                    case "parent":
                    case "has-name":
                    case "oninit":
                        // Skipping these buggers, since they're handled elsewhere
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
         * Ensuring that the "value" property is the same as the "ID" of the widget.
         * But only if widget require a value attribute to function properly, and
         * widget has not explicitly been given a value
         */
        private void EnsureValueProperty (Widget widget, Node node, ApplicationContext context)
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
        private void EnsureNameProperty (Widget widget, Node node, ApplicationContext context)
        {
            if (widget.HasAttribute ("name"))
                return; // Caller already explicitly added name attribute

            if (!node.GetChildValue ("has-name", context, true)) {
                return; // Caller explicitly told us he didn't want any name attribute value
            }

            // Making sure "input", "select" and "textarea" widgets have a name corresponding to their ID
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
        private void CreateChildWidgets (ApplicationContext context, Widget widget, Node children)
        {
            // Looping through all child widgets declared in lambda object
            foreach (var idxChild in children.Children) {

                // Passing in parent widget, when invoking creational Active Event for currently iterated widget
                idxChild.Insert (0, new Node ("_parent", widget));
                context.RaiseNative ("p5.web.widgets." + idxChild.Name, idxChild);
            }
        }

        /*
         * Creates lambda events for Widget
         */
        private void CreateWidgetLambdaEvents (ApplicationContext context, Widget widget, Node events)
        {
            // Looping through all events for widget
            foreach (var idxEvt in events.Children.ToList ()) {

                // Verifying Active Event is not protected
                if (!CanOverrideEventInLambda(context, idxEvt.Name))
                    throw new LambdaException(
                        string.Format ("You cannot override Active Event '{0}' since it is protected", idxEvt.Name),
                        idxEvt,
                        context);

                // Registering our event
                Manager.WidgetLambdaEventStorage [idxEvt.Name, widget.ID] = idxEvt.UnTie ();
            }
        }

        /*
         * Handles all default properties of Widget
         */
        private void HandleDefaultProperty (ApplicationContext context, Widget widget, Node node)
        {
            // Checking if this is a declaration of an event handler, either "visible" or "invisible server-side"
            if (node.Name.StartsWith ("on") || node.Name.StartsWith ("_on")) {

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
        private void CreateEventHandler (ApplicationContext context, Widget widget, Node node)
        {
            // Checking what type of event this is, JavaScript or server-side lambda
            if (node.Value != null) {

                // Javascript code to be executed
                widget [node.Name] = node.Get<string> (context);
            } else {

                // Mapping the widget's ajax event to our common event handler on page
                // But intentionally dropping [oninit], since it's a purely server-side lambda event, and not supposed to be pushed to client
                if (node.Name != "oninit")
                    widget [node.Name] = "common_event_handler"; // Name of common WebMethod from page

                // Making sure event has a reference to ID of widget in its body as first child
                node.Insert (0, new Node ("_event", widget.ID));

                // Storing our Widget Ajax event in storage
                Manager.WidgetAjaxEventStorage [widget.ID, node.Name] = node.UnTie ();
            }
        }
    }
}
