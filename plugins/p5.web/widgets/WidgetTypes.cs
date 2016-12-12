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
using System.Linq;
using System.Web.UI;
using p5.ajax.widgets;
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
        [ActiveEvent (Name = ".p5.web.widgets.container")]
        public void _p5_web_controls_container (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = CreateWidget<Container> (context, e.Args, "div");
        }

        /// <summary>
        ///     Creates an ajax literal web widget
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.web.widgets.literal")]
        public void _p5_web_controls_literal (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = CreateWidget<Literal> (context, e.Args, "p");
        }

        /// <summary>
        ///     Creates an ajax void web widget
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.web.widgets.void")]
        public void _p5_web_controls_void (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = CreateWidget<Void> (context, e.Args, "input");
        }

        /// <summary>
        ///     Creates a simple text web widget
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.web.widgets.text")]
        public void _p5_web_controls_text (ApplicationContext context, ActiveEventArgs e)
        {
            // Creating widget as persistent control
            var parent = e.Args.GetChildValue<Container> ("_parent", context);
            var position = e.Args.GetChildValue ("position", context, -1);
            var after = e.Args.GetChildValue <Control>("after", context, null);
            var before = e.Args.GetChildValue <Control>("before", context, null);

            // If before != null, or after != null, we use them, in that priority to figure out positioning
            if (before != null) {
                parent = before.Parent as Container;
                position = parent.Controls.IndexOf (before);
            } else if (after != null) {
                parent = after.Parent as Container;
                position = parent.Controls.IndexOf (after) + 1;
            }

            var ctrl = new LiteralControl ();
            ctrl.Text = e.Args.Get<string> (context);
            parent.Controls.AddAt (position, ctrl);
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
            var after = args.GetChildValue <Control>("after", context, null);
            var before = args.GetChildValue <Control>("before", context, null);

            // If parent is "cnt" (which is the default value) and position is -1 (which is default)
            // then we check if after or before was supplied, and if so, we use them to find both parent and position
            if (before != null) {
                parent = before.Parent as Container;
                position = parent.Controls.IndexOf (before);
            } else if (after != null) {
                parent = after.Parent as Container;
                position = parent.Controls.IndexOf (after) + 1;
            }

            // Creating control as persistent control, and setting HTML element type, making sure a widget with the same ID does not exist from before.
            var id = args.Get<string> (context);
            if (!string.IsNullOrEmpty (id) && FindControl<Control> (id, Manager.AjaxPage) != null)
                throw new LambdaException ("A widget with the same ID already exist on page!", args, context);

            var widget = parent.CreatePersistentControl<T> (id, position);
            try {
                widget.Element = elementType;

                // Decorating widget properties/events, and create child widgets
                DecorateWidget (context, widget, args);
            } catch {

                // Removing widget from parent, to make operation "atomic".
                parent.Controls.Remove (widget);
                throw;
            }

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
                    case "element":
                        widget.Element = idxArg.Get<string> (context);
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
                    case "before":
                    case "after":
                        // Skipping these buggers, since they're handled elsewhere
                        break;
                    case "oninit":

                        // Adding lambda event to lambda event storage
                        var clone = idxArg.Clone ();
                        Manager.WidgetAjaxEventStorage [widget.ID, "oninit"] = clone;
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
            if (widget.Element == "input" && widget ["type"] == "radio") {
                widget ["value"] = widget.ID;
            }
        }
            
        /*
         * Ensuring that the "name" property is the same as the "ID" of the widget, unless a name property is explicitly given,
         * or element type doesn't necessarily require a "name" to function properly
         */
        private void EnsureNameProperty (Widget widget, Node node, ApplicationContext context)
        {
            if (widget.HasAttribute ("name"))
                return; // Caller already explicitly added name attribute

            // Making sure "input", "select" and "textarea" widgets have a name corresponding to their ID
            switch (widget.Element) {
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

                // Passing in parent widget, when invoking creational Active Event for currently iterated widget.
                switch (idxChild.Name) {
                    case "literal":
                    case "container":
                    case "void":
                    case "text":

                        // "Native" widget
                        idxChild.Insert (0, new Node ("_parent", widget));
                        context.Raise (".p5.web.widgets." + idxChild.Name, idxChild);
                        break;
                    default:

                        // Checking if this is a "custom widget"
                        if (idxChild.Name.IndexOf (".") > 0) {

                            // Making sure we store the ID for widget
                            var id = idxChild.Value;

                            // This is a "custom widget", which means we invoke its name and arguments as an Active Event, and use the returned
                            // lambda object as the currently iterated child widget.
                            // Notice, we do NOT pass in [_parent] when invoking custom creation Active Event!
                            context.Raise (idxChild.Name, idxChild);
                            if (idxChild.Children.Count != 1)
                                throw new LambdaException ("Custom widget event did not return exactly one child value", idxChild, context);

                            // Making sure we decorate the ID for widget automatically
                            idxChild.FirstChild.Value = id;

                            // Recursively invoking self, with the children widgets, that should now be declared as the first child node of idxChild.
                            CreateChildWidgets (context, widget, idxChild);
                        } else {

                            // This is the "HTML element" helper syntax, declaring the element of the widget as the node's name
                            idxChild.Insert (0, new Node ("_parent", widget));
                            idxChild.Add ("element", idxChild.Name);
                            if (idxChild["innerValue"] != null) {
                                context.Raise (".p5.web.widgets.literal", idxChild);
                            } else if (idxChild["widgets"] != null) {
                                context.Raise (".p5.web.widgets.container", idxChild);
                            } else {
                                context.Raise (".p5.web.widgets.void", idxChild);
                            }
                        }
                        break;
                }
            }
        }

        /*
         * Creates lambda events for Widget
         */
        private void CreateWidgetLambdaEvents (ApplicationContext context, Widget widget, Node events)
        {
            // Looping through all events for widget
            foreach (var idxEvt in events.Children.ToList ()) {

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
            if (node.Name.StartsWith ("on") || node.Name.StartsWith ("_on") || node.Name.StartsWith(".on")) {

                // This is an event, creating it
                CreateEventHandler (context, widget, node);
            } else {

                // This is a normal attribute, making sure we escape attribute if necessary
                widget [node.Name.StartsWith ("\\") ? node.Name.Substring(1) : node.Name] = node.Get<string> (context);
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
